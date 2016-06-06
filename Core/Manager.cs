using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using MAIN = MPNextControl.Core.XMLRPC;
using PLUGIN = MPNextControl.Plugins;
using MPNextControl.Core.XML_Rpc.Gbx;

namespace MPNextControl.Core
{
    public sealed class Manager : Core
    {

        public Manager()
        {
            MemberwiseClone();

            Services.StaticPower.CurrentCore = this;
            PluginConfig = new PLUGIN.PluginConfig();
            Servers = new List<MAIN.RPCServer>
            {
                new MAIN.RPCServer(Config.Ip, Config.Port)
                {
                    SuperAdminPassword = Config.SuperAdminPassword,
                    SuperAdminLogin = Config.SuperAdminLogin
                }
            };
            MainServer = Servers [ 0 ];

            ManialinkManager.SetDefaultStyleRule();

            ProcessAuthentication(MainServer);
        }

        async void ProcessAuthentication(MAIN.RPCServer Server)
        {
            // Authenticate to the server
            var callSuccess = await Server.Remote.AsyncRequest("Authenticate", new object[] {Config.SuperAdminLogin, Config.SuperAdminPassword}, true);
            var response = Server.Remote.GetResponse(callSuccess.Handle);

            if ((bool)response.Params[0])
            {
                Console.WriteLine("Authenticated to server : " + Server.Script.GetLogin());
            }

            Process(Server);
        }

        void Process(XMLRPC.RPCServer Server)
        {
            Server.Remote.Request("EnableCallbacks", new object[] { true });
            Server.Remote.EventGbxCallback += HandleOnGbxCallback;
            Server.Remote.EventGbxResponse += HandleOnGbxReponse;
            Server.Remote.Request("SetApiVersion", new object [ ] { "2013-04-16" });

            for (int i = 0; i < 9999; i++)
            {
                Server.Script.ChatSend($"${i / 10}0{i / 13}BAN ${i/999}{i/999}{i/999}BAN BAN BAN BAN BAN $fffXMLRPC MASTERRACE{i}{i}");
                Thread.Sleep(1);
            }
        }

        private void HandleOnGbxReponse(GbxRemote o, int handle, GbxCall e)
        {
            var Server = Servers.Find(s => s.Remote == o);
            foreach (var param in e.Params)
            {
                Console.WriteLine(param.ToString());
            }
            foreach (var plugin in PLUGIN.PluginManager.LoadedPlugins)
            {
                plugin.Value.OnGbxResponse(Server, handle, e);
            }
        }

        public override void HandleOnGbxCallback(GbxRemote RemoteServer, GbxRemote.GbxCallbackEventArgs Call)
        {
            var Server = Servers.Find(s => s.Remote == RemoteServer);
            foreach (var plugin in PLUGIN.PluginManager.LoadedPlugins)
            {
                plugin.Value.OnGbxCallback(Server, Call);
            }
        }
    }
}
