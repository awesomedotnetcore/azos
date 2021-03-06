/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using System;
using System.Collections.Generic;
using System.Linq;

using Azos.Conf;
using Azos.Log;

namespace Azos.Instrumentation
{
  /// <summary>
  /// Represents a provider that writes aggregated datums to multiple destination provider
  /// </summary>
  public class CompositeInstrumentationProvider : InstrumentationProvider
  {
    #region .ctor

    public CompositeInstrumentationProvider(InstrumentationDaemon director) : base(director) { }

    #endregion

    #region Fields

    private List<InstrumentationProvider> m_Providers = new List<InstrumentationProvider>();

    #endregion

    #region Properties

    /// <summary>
    /// Returns destinations that this destination wraps. This call is thread safe
    /// </summary>
    public IEnumerable<InstrumentationProvider> Providers
    {
      get { lock (m_Providers) return m_Providers.ToList(); }
    }

    #endregion

    #region Public

    public void RegisterProvider(InstrumentationProvider provider)
    {
      lock (m_Providers)
      {
        if (!m_Providers.Contains(provider))
          m_Providers.Add(provider);
      }
    }

    public bool UnRegisterProvider(InstrumentationProvider provider)
    {
      lock (m_Providers)
      {
        return m_Providers.Remove(provider);
      }
    }

    #endregion

    #region Protected

    protected override void DoConfigure(IConfigSectionNode node)
    {
      base.DoConfigure(node);

      foreach (var dnode in node.Children.Where(n => n.Name.EqualsIgnoreCase(InstrumentationDaemon.CONFIG_PROVIDER_SECTION)))
      {
        var dest = FactoryUtils.MakeAndConfigure(dnode, args: new[] { ComponentDirector }) as InstrumentationProvider;
        this.RegisterProvider(dest);
      }
    }

    protected internal override object BeforeBatch()
    {
      var dict = new Dictionary<string, object>(m_Providers.Count);
      lock (m_Providers)
        foreach (var provider in m_Providers)
        {
          var batchContext = provider.BeforeBatch();
          if (batchContext != null)
            dict.Add(provider.Name, batchContext);
        }
      return dict;
    }

    protected internal override void AfterBatch(object batchContext)
    {
      var dict = batchContext as Dictionary<string, object>;
      lock (m_Providers)
        foreach (var provider in m_Providers)
        {
          object providerBatchContext = null;
          if (dict != null)
            dict.TryGetValue(provider.Name, out providerBatchContext);
          provider.AfterBatch(providerBatchContext);
        }
    }

    protected internal override object BeforeType(Type type, object batchContext)
    {
      var batchDict = batchContext as Dictionary<string, object>;
      var dict = new Dictionary<string, object>(m_Providers.Count);
      lock (m_Providers)
        foreach (var provider in m_Providers)
        {
          object providerBatchContext = null;
          if (batchDict != null)
            batchDict.TryGetValue(provider.Name, out providerBatchContext);
          var typeContext = provider.BeforeType(type, providerBatchContext);
          if (batchContext != null)
            dict.Add(provider.Name, typeContext);
        }
      return dict;
    }

    protected internal override void AfterType(Type type, object batchContext, object typeContext)
    {
      var batchDict = batchContext as Dictionary<string, object>;
      var dict = typeContext as Dictionary<string, object>;
      lock (m_Providers)
        foreach (var provider in m_Providers)
        {
          object providerBatchContext = null;
          if (batchDict != null)
            batchDict.TryGetValue(provider.Name, out providerBatchContext);
          object providerTypeContext = null;
          if (dict != null)
            dict.TryGetValue(provider.Name, out providerTypeContext);
          provider.AfterType(type, providerBatchContext, providerTypeContext);
        }
    }

    protected internal override void Write(Datum aggregatedDatum, object batchContext, object typeContext)
    {
      var batchDict = batchContext as Dictionary<string, object>;
      var typeDict = typeContext as Dictionary<string, object>;
      lock (m_Providers)
        foreach (var provider in m_Providers)
          try
          {
            object providerBatchContext = null;
            if (batchDict != null)
              batchDict.TryGetValue(provider.Name, out providerBatchContext);
            object providerTypeContext = null;
            if (typeDict != null)
              typeDict.TryGetValue(provider.Name, out providerTypeContext);
            provider.Write(aggregatedDatum, providerBatchContext, providerTypeContext);
          }
          catch (Exception error)
          {
            ComponentDirector.WriteLog(MessageType.Error, GetType().Name + ".Write", error.ToMessageWithType(), error);
          }
    }

    #endregion

  }
}
