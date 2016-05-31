using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Services
{
    public class Config
    {
        public Dictionary<string, object> Current;
        public Config()
        {
            Current = new Dictionary<string, object>();
        }
    }
}
