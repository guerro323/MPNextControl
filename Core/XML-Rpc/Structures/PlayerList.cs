using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{

    public struct PlayerList
    {
        public bool Null;
        public string Login;
        public string Nickname;
        public int PlayerId;
        public int TeamId;
        public int SpectatorStatus;
        public int LadderRanking;
        public int Flags;
        public RPCServer Server;
    }
}
