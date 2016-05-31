using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{
    public struct PlayerConnect
    {
        public string Login;
        public bool IsSpectator;
        public RPCServer Server;
    }
}
