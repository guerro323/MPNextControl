using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPNextControl.Core.XML_Rpc.Gbx;

namespace MPNextControl.Core.XMLRPC
{
    public class ServiceRPCServer
    {
        public Services.Static.GameInfo GetGameInfo()
        {
            return new Services.Static.GameInfo();
        }
    }

    public class RPCServer
    {
        public ServiceRPCServer Service;
        public string SuperAdminLogin;
        public string SuperAdminPassword;

        public GbxRemote Remote;


        public RPCServer(string Ip, int Port)
        {
            Remote = new GbxRemote(Ip, Port);
            Remote.TryConnect();
        }
    }
}
