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
        public List<MAIN.RPCServer> Servers;
        public MAIN.RPCServer MainServer;

        public STATIC.Manialink ManialinkManager;
        public STATIC.Helper HelperManager;

        public STATIC.GameInfo GameInfo;

        public Plugins.PluginConfig PluginConfig;

        public Core()
        {
            Config = STATIC.Config.Get().Result;
            ManialinkManager = new STATIC.Manialink();
            HelperManager = new STATIC.Helper();
        }

        public virtual void HandleOnGbxCallback(GbxRemote RemoteServer, GbxRemote.GbxCallbackEventArgs Call)
        {

        }
    }
}
