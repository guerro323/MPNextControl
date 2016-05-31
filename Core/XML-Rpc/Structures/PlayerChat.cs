using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{
    public struct PlayerChat
    {
        public int PlayerUid;
        public string Login;
        public string Text;
        /// <summary>
        /// Access to the server of where the player is located
        /// Useful for CrossServers or Multiples servers
        /// </summary>
        public RPCServer Server;
        public bool IsRegisteredCmd;
    }
}
