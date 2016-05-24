using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using PLUGIN = MPNextControl.Plugins;
using MPNextControl.Core.XML_Rpc.Gbx;

namespace MPNextControl.Core
{
    class Manager : Core
    {
        public Manager()
        {
            MemberwiseClone();
            ProcessAuthentication();
        }

        async void ProcessAuthentication()
        {
            // Authenticate to the server
            var callSuccess = await Server.Remote.AsyncRequest("Authenticate", new object[] {Config.SuperAdminLogin, Config.SuperAdminPassword});
            var response = Server.Remote.GetResponse(callSuccess.Handle);

            if ((bool)response.Params[0])
            {
                Console.WriteLine("Authenticated to the server!");
            }

            Process();
        }

        public void Process()
        {
            Server.Remote.Request("EnableCallbacks", new object[] { true });
            Server.Remote.EventGbxCallback += HandleOnGbxCallback;
        }

        public override async void HandleOnGbxCallback(GbxRemote RemoteServer, GbxRemote.GbxCallbackEventArgs Call)
        {
            //Console.WriteLine(Call.Response.MethodName);
        }
    }
}
