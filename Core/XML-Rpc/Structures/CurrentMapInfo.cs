﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPNextControl.Core.XMLRPC.Structures
{
    public struct CurrentMapInfo
    {
        /// <summary>
        /// If CurrentMapInfo is null or not
        /// </summary>
        public bool Null;
        public string UId;
        public string Name;
        public string FileName;
        public string Environnement;
        public string Author;
        public int GoldTime;
        public int CopperPrice;
        public string MapType;
        public string MapStyle;
        public int NbCheckpoints;
    }
}
