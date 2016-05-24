using MPNextControl.Core.XML_Rpc.Gbx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MAIN = MPNextControl.Core.XMLRPC;
using STATIC = MPNextControl.Services.Static;

namespace MPNextControl.Core
{
    public class Core
    {
        public STATIC.Config Config;
        public MAIN.RPCServer Server;

        public STATIC.GameInfo GameInfo;

        public Core()
        {
            Config = STATIC.Config.Get().Result;
            Server = new MAIN.RPCServer(Config.Ip, Config.Port)
            {
                SuperAdminPassword = Config.SuperAdminPassword,
                SuperAdminLogin = Config.SuperAdminLogin
            };
        }

        public virtual async void HandleOnGbxCallback(GbxRemote RemoteServer, GbxRemote.GbxCallbackEventArgs Call)
        {

        }
    }
}
