using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Services.Static
{
    static public class KnowCallbacks
    {
        static public class ManiaPlanet
        {
            static private string prefix => "ManiaPlanet.";
            static public string PlayerConnect => prefix + "PlayerConnect";
            static public string PlayerDisconnect => prefix + "PlayerDisconnect";
            static public string PlayerChat => prefix + "PlayerChat";
            static public string PlayerManialinkPageAnswer => prefix + "PlayerManialinkPageAnswer";
            static public string Echo => prefix + "Echo";
            static public string ServerStart => prefix + "ServerStart";
            static public string ServerStop => prefix + "ServerStop";
            static public string BeginMatch => prefix + "BeginMatch";
            static public string EndMatch => prefix + "EndMatch";
            static public string BeginMap => prefix + "BeginMap";
            static public string EndMap => prefix + "EndMap";
            static public string StatusChanged => prefix + "StatusChanged";
        }

        static public class TrackMania
        {
            static private string prefix => "TrackMania.";
            static public string PlayerCheckpoint => prefix + "PlayerCheckpoint";
            static public string PlayerFinish => prefix + "PlayerFinish";
            static public string PlayerIncoherence => prefix + "PlayerIncoherence";
        }
    }
}
