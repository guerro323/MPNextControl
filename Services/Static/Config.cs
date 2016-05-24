using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AsyncIO = MPNextControl.Services;

namespace MPNextControl.Services.Static
{
    public class Config
    {
        public string Ip = "127.0.0.1";
        public int Port = 5000;
        public string SuperAdminPassword = "SuperAdmin";
        public string SuperAdminLogin = "SuperAdmin";

        static public async Task<Config> Get()
        {
            return await GetAsync();
        }

        static private async Task<Config> GetAsync()
        {
            await Task.Delay(1);

            return new Config();
        }
    }
}
