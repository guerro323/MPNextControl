using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Core.XMLRPC.Structures
{
    public struct SMapInfo
    {
        public string Uid;
        public string Name;
        public string FileName;
        public string Author;
        public string Environnement;
        public string Mood;
        public int BronzeTime;
        public int SilverTime;
        public int GoldTime;
        public int AuthorTime;
        public int CopperPrice;
        public bool LapRace;
        public int NbLaps;
        public int NbCheckpoints;
        public string MapType;
        public string MapStyle;
    }
}
