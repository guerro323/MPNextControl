using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{
    public struct PlayerDisconnect
    {
        public string Login;
        public string NickName;
        public RPCServer Server;
    }
}
