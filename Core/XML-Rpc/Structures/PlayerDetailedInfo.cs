using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{

    public struct PlayerDetailedInfo
    {
        public bool Null;
        public string Login;
        public string Nickname;
        public string IPAdress;
        public int DownloadRate;
        public int UploadRate;
        public bool IsSpectator;
        public bool IsInOfficialMode;
        public int PlayerId;
        public int TeamId;
        public int SpectatorStatus;
        public int LadderRanking;
        public int Flags;
        public string Language;
        public RPCServer Server;
    }
}