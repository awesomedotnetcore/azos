﻿/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Azos.Apps;
using Azos.Log;
using Azos.Serialization.JSON;

using Azos.Data.Access.MongoDb;
using Azos.Conf;
using Azos.Data.Access.MongoDb.Connector;
using Azos.Serialization.BSON;
using System.Linq;

namespace Azos.Sky.Chronicle.Server
{
  /// <summary>
  /// Implements ILogChronicleStoreLogic and IInstrumentationChronicleStoreLogic using MongoDb
  /// </summary>
  public sealed class MongoChronicleStoreLogic : Daemon, ILogChronicleStoreLogicImplementation, IInstrumentationChronicleStoreLogicImplementation
  {
    public const string CONFIG_BUNDLED_MONGO_SECTION = "bundled-mongo";

    public const string DEFAULT_DB = "sky_chron";

    public const string COLLECTION_LOG = "sky_log";
    public const string COLLECTION_INSTR = "sky_ins";

    public const int MAX_DOC_COUNT = 8 * 1024;
    public const int FETCH_BY_LOG = 128;


    public MongoChronicleStoreLogic(IApplication application) : base(application) { }
    public MongoChronicleStoreLogic(IModule parent) : base(parent) { }

    protected override void Destructor()
    {
      DisposeAndNull(ref m_Bundled);
      base.Destructor();
    }

    [Config(Default = DEFAULT_DB)]
    private string m_DbNameLog = DEFAULT_DB;

    [Config(Default = DEFAULT_DB)]
    private string m_DbNameInstr = DEFAULT_DB;


    public string DbNameLog => m_DbNameLog.Default(DEFAULT_DB);
    public string DbNameInstr => m_DbNameInstr.Default(DEFAULT_DB);

    public Database LogDb => m_Bundled.GetDatabase(DbNameLog);
    public Database InstrDb => m_Bundled.GetDatabase(DbNameInstr);

    public override string ComponentLogTopic => throw new NotImplementedException();

    private BundledMongoDb m_Bundled;

    protected override void DoConfigure(IConfigSectionNode node)
    {
      base.DoConfigure(node);
      DisposeAndNull(ref m_Bundled);
      if (node==null) return;

      var nbundled = node[CONFIG_BUNDLED_MONGO_SECTION];
      if (nbundled.Exists)
      {
        m_Bundled = FactoryUtils.MakeAndConfigureDirectedComponent<BundledMongoDb>(this, nbundled);
      }
    }

    protected override void DoStart()
    {
      base.DoStart();
      if (m_Bundled!=null) m_Bundled.Start();
    }

    protected override void DoSignalStop()
    {
      base.DoSignalStop();
      if (m_Bundled != null) m_Bundled.SignalStop();
    }

    protected override void DoWaitForCompleteStop()
    {
      base.DoWaitForCompleteStop();
      if (m_Bundled != null) m_Bundled.WaitForCompleteStop();
    }

    private static BSONSerializer s_LogBson = new BSONSerializer(new BSONTypeResolver(typeof(Message)));
    public Task<IEnumerable<Message>> GetAsync(LogChronicleFilter filter) => Task.FromResult(get(filter));
    private IEnumerable<Message> get(LogChronicleFilter filter)
    {
      filter.NonNull(nameof(filter));
      var cLog = LogDb[COLLECTION_LOG];
      var query = Query.ID_EQ_Int32(123);//<--- todo: BUILD query for filter
      using(var cursor = cLog.Find(query, filter.PagingStartIndex, FETCH_BY_LOG))
      {
        int i = 0;
        foreach(var bdoc in cursor)
        {
          var msg = new Message();
          s_LogBson.Deserialize(bdoc, msg);

          yield return msg;

          if (++i > MAX_DOC_COUNT) break;
        }
      }
    }

    public Task WriteAsync(LogBatch data)
    {
      var toSend = data.NonNull(nameof(data)).Data.NonNull(nameof(data));
      var cLog = LogDb[COLLECTION_LOG];

      foreach(var batch in toSend.BatchBy(16))
      {
        var bsons = batch.Select(msg => {
          //todo Assign GDID
          //msg.Gdid =
          return s_LogBson.Serialize(msg);
        });
        cLog.Insert(bsons.ToArray());
      }

      return Task.CompletedTask;
    }

    public Task<IEnumerable<JsonDataMap>> GetAsync(InstrumentationChronicleFilter filter)
    {
      throw new NotImplementedException();
    }

    public Task WriteAsync(InstrumentationBatch data)
    {
      throw new NotImplementedException();
    }
  }
}
