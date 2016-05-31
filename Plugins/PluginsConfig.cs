using MPNextControl.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPNextControl.Plugins
{
    public partial class PluginConfig
    {
        /*
         * ServerManager
         * Plugins Config
         * ---------------
         * Manage the config of a plugin
         */

        public class Config
        {
            /// <summary>
            /// If a configuration need to be for a specific player.
            /// </summary>
            public Dictionary<string, object> ConfigForPlayer = new Dictionary<string, object>();

            public object Value;
            public ConfigType Type;
        }

        public enum ConfigType
        {
            Command,
            Config
        }

        /// <summary>
        /// Configs for each plugins.
        /// [PluginName][ConfigName]Config
        /// </summary>
        public Dictionary<string, Dictionary<string, Config>> PluginsConfigs = new Dictionary<string, Dictionary<string, Config>>();

        public void LoadPluginsConfig(string pluginName)
        {
            var task = AsyncIO.ProcessRead(pluginName, "Config.json");
            task.Wait();
            var pluginconfStreamRod = task.Result;
            PluginsConfigs = new Dictionary<string, Dictionary<string, Config>>();
            if (!string.IsNullOrEmpty(pluginconfStreamRod))
            {
                // It's ok
                PluginsConfigs =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Config>>>(pluginconfStreamRod);
            }
            else
            {
                AsyncIO.ProcessWrite(pluginName, "Config.json",
                    Newtonsoft.Json.JsonConvert.SerializeObject(PluginsConfigs, Newtonsoft.Json.Formatting.Indented));
            }
        }

        private void savePluginConfig(string pluginName)
        {
            AsyncIO.ProcessWrite(pluginName, "Config.json",
                Newtonsoft.Json.JsonConvert.SerializeObject(PluginsConfigs, Newtonsoft.Json.Formatting.Indented));
        }

        public object GetConfig(string plName, ConfigType cftype, string cfName, object addObject)
        {
            var index = plName;
            //AyO.print(true, (PluginsConfigs [ index ].ContainsKey(cfName)).ToString() + cfName);
            if (!PluginsConfigs.ContainsKey(index) || !PluginsConfigs [ index ].ContainsKey(cfName))
            {
                if (!PluginsConfigs.ContainsKey(index)) PluginsConfigs.Add(index, new Dictionary<string, Config>());
                PluginsConfigs [ index ].Add(cfName, new Config { Type = cftype, Value = addObject });
                savePluginConfig(plName);
            }
            else if (addObject != null && (PluginsConfigs [ index ] == null || !PluginsConfigs [ index ].ContainsKey(cfName)))
            {
                PluginsConfigs [ index ] [ cfName ].Value = addObject;
                savePluginConfig(plName);
            }
            foreach (var conf in PluginsConfigs.Where(conf   => index == conf.Key && conf.Value [ cfName ].Type == cftype))
            {
                return conf.Value [ cfName ].Value;
            }
            return new object();
        }
        public void SetConfig(string plName, ConfigType cftype, string cfName, object addObject)
        {
            var index = plName;
            if (!PluginsConfigs.ContainsKey(index))
            {
                PluginsConfigs.Add(index, new Dictionary<string, Config>());
                PluginsConfigs [ index ].Add(cfName, new Config { Type = cftype, Value = addObject });
                savePluginConfig(plName);
                return;
            }
            PluginsConfigs [ index ] [ cfName ].Value = addObject;
            savePluginConfig(plName);
        }

        /*
         * ServerManager
         * Plugins Static Communication
         * --------------- DESC :
         * Manage value of static communication
         * --------------- EXEMPLE :
         * Plugins can have two interfaces ( manialinks )
         * but the plugin are set as default
         * If a plugin activate the second interface
         * Then all plugins will got their second interface
         */

        public Dictionary<string, object> StaticCommunication = new Dictionary<string, object>();

        public object GetStaticVar(string varName, object addObject)
        {
            if (!StaticCommunication.ContainsKey(varName))
            {
                StaticCommunication.Add(varName, addObject);
            }
            else if (addObject != null && StaticCommunication [ varName ] == null)
            {
                StaticCommunication [ varName ] = addObject;
            }
            foreach (var conf in StaticCommunication.Where(conf => varName == conf.Key))
            {
                return conf.Value;
            }
            return new object();
        }

        public void SetStaticVar(string varName, object addObject)
        {
            if (!StaticCommunication.ContainsKey(varName))
            {
                StaticCommunication.Add(varName, addObject);
                return;
            }
            StaticCommunication [ varName ] = addObject;
        }
    }
}
