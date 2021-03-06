/*<FILE_LICENSE>
 * Azos (A to Z Application Operating System) Framework
 * The A to Z Foundation (a.k.a. Azist) licenses this file to you under the MIT license.
 * See the LICENSE file in the project root for more information.
</FILE_LICENSE>*/

using Azos.Apps;
using Azos.Glue;
using Azos.Glue.Native;
using Azos.Glue.Protocol;
using Azos.Scripting;

using TestBusinessLogic;

namespace Azos.Tests.Integration.Glue
{
  [Runnable]
  public class JokeContracts: JokeTestBase
  {
    [Run]
    [Aver.Throws(typeof(ClientCallException))]//because there is no binding registered
    public void ExpectedExceptionWhenGlueNotConfiguredAndBindingNotRegistered()
    {
        var app = new TestApplication();
      var cl = new JokeContractClient(app.Glue, TestServerSyncNode);
    }

    [Run]
    public void GlueConfiguredByCode()
    {
      //This is an example of how to use Glue without pre-configured app container
      using(var app = new TestApplication())
      {
        var glue = new Azos.Glue.Implementation.GlueDaemon(app);
        glue.Start();
        try
        {
            var binding = new SyncBinding(glue, "sync");
            var cl = new JokeContractClient(glue, TestServerSyncNode);
        }
        finally
        {
            glue.WaitForCompleteStop();
        }
      }
    }

    [Run]
    public void GlueConfiguredByCode_MPX()
    {
      //This is an example of how to use Glue without pre-configured app container
      using (var app = new TestApplication() { Active = true })
      {
        var glue = new Azos.Glue.Implementation.GlueDaemon(app);
        glue.Start();
        try
        {
            var binding = new MpxBinding(glue, "mpx");
            var cl = new JokeContractClient(glue, TestServerMpxNode);


        }
        finally
        {
            glue.WaitForCompleteStop();
        }
      }
    }


    [Run]
    public void GlueConfiguredByCodeAndMakeCall_Sync()
    {
      //This is an example of how to use Glue without pre-configured app container
      using(var app = new TestApplication(){ Active = true })
      {
        var glue = new Azos.Glue.Implementation.GlueDaemon(app);
        glue.Start();
        try
        {
            using(var binding = new SyncBinding(glue, "sync"))
            {
              binding.Start();
              var cl = new JokeContractClient(glue, TestServerSyncNode);
              cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

              var result = cl.Echo("Gello A!");

              Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
            }
        }
        finally
        {
            glue.WaitForCompleteStop();
        }
      }
    }


    [Run]
    public void GlueConfiguredByCodeAndMakeCall_MPX()
    {
      //This is an example of how to use Glue without pre-configured app container
      using(var app = new TestApplication(){ Active = true })
      {
        var glue = new Azos.Glue.Implementation.GlueDaemon(app);
        glue.Start();
        try
        {
            using(var binding = new MpxBinding(glue, "mpx"))
            {
              binding.Start();
              var cl = new JokeContractClient(glue, TestServerMpxNode);
              cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

              var result = cl.Echo("Gello A!");

              Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
            }
        }
        finally
        {
            glue.WaitForCompleteStop();
        }
      }
    }

    [Run]
    public void Sync_JokeContract_Echo_ByCode()
    {
        using(var app = JokeHelper.MakeApp())
        {
          var cl = new JokeContractClient(app.Glue, TestServerSyncNode);
            cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

            var result = cl.Echo("Gello A!");

            Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
        }
    }

    [Run]
    public void MPX_JokeContract_Echo_ByCode()
    {
      using(var app = JokeHelper.MakeApp())
      {
        var cl = new JokeContractClient(app.Glue, TestServerMpxNode);
          cl.Headers.Add( new AuthenticationHeader( TestCredentials ) );

          var result = cl.Echo("Gello A!");

          Aver.IsTrue(result.StartsWith("Server echoed Gello A!"));
      }
    }

    [Run]
    public void Sync_JokeContract_Async_Echo_ByCode()
    {
      using (var app = JokeHelper.MakeApp())
      {
        var cl = new JokeContractClient(app.Glue, TestServerSyncNode);
        cl.Headers.Add(new AuthenticationHeader(TestCredentials));

          var call = cl.Async_Echo("Gello B!");

          var result = call.GetValue<string>();

          Aver.IsTrue(result.StartsWith("Server echoed Gello B!"));
      }
    }

    [Run]
    public void MPX_JokeContract_Async_Echo_ByCode()
    {
      using (var app = JokeHelper.MakeApp())
        {
          var cl = new JokeContractClient(app.Glue, TestServerMpxNode);
          cl.Headers.Add(new AuthenticationHeader(TestCredentials));

            var call = cl.Async_Echo("Gello B!");

            var result = call.GetValue<string>();

            Aver.IsTrue(result.StartsWith("Server echoed Gello B!"));
        }
    }

    [Run]
    [Aver.Throws(typeof(RemoteException), Message="Azos.Security.AuthorizationException")]
    public void Sync_JokeContract_Expected_Security_Exception()
    {
      using (var app = JokeHelper.MakeApp())
        {
            var cl = new JokeContractClient(app.Glue, TestServerSyncNode);

            var result = cl.Echo("Blah");//throws sec exception
        }
    }

    [Run]
    [Aver.Throws(typeof(RemoteException), Message="Azos.Security.AuthorizationException")]
    public void MPX_JokeContract_Expected_Security_Exception()
    {
      using (var app = JokeHelper.MakeApp())
        {
            var cl = new JokeContractClient(app.Glue, TestServerMpxNode);

            var result = cl.Echo("Blah");//throws sec exception
        }
    }


    }
}
