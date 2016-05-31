using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPNextControl.Core;
using MPNextControl.Core.XMLRPC;
using MPNextControl.Core.XML_Rpc.Gbx;
using MPNextControl.Services.Static;
using System.Reflection;

namespace MPNextControl.Plugins
{
    public interface IPlugin
    {
        void Init(Manager core, PluginConfig config = null);
        bool OnLoad();
        void OnEventCallback();
        void OnResponse();
        void OnUnload();
        void OnCrash();
        void OnException();
        void OnError();
    }

    public class Plugin : IPlugin
    {
        public Manager ServerManager;
        public List<RPCServer> Servers;
        public RPCServer MainServer;
        public Manialink ManialinkManager;
        public Helper HelperManager;
        public PluginConfig Config;

        /// <summary>
        /// Static Name of the plugin
        /// NEVER CHANGE THIS ONCE YOU BUILD IT
        /// <para>Will be used for internal purposes</para>
        /// </summary>
        virtual public string StaticName => "";
        /// <summary>
        /// Name of the plugin
        /// </summary>
        virtual public string Name => "";
        /// <summary>
        /// Author name of the plugin
        /// </summary>
        virtual public string Author => "";
        /// <summary>
        /// Version of the plugin
        /// Format : PRIMARY.SECONDARY
        /// </summary>
        virtual public double Version => 0;
        /// <summary>
        /// Build version of the plugin
        /// Leave 0 for non compiled plugins
        /// </summary>
        virtual public int VersionBuild => 0;

        public virtual void Init(Manager core, PluginConfig config = null)
        {
            ServerManager = core;
            Servers = core.Servers;
            MainServer = core.MainServer;
            ManialinkManager = core.ManialinkManager;
            HelperManager = core.HelperManager;
            Config = core.PluginConfig;
        }

        public virtual void OnCrash()
        {

        }

        public virtual void OnError()
        {

        }

        public virtual void OnGbxCallback(RPCServer Server, GbxRemote.GbxCallbackEventArgs Args)
        {

        }

        public virtual void OnEventCallback()
        {

        }

        public virtual void OnException()
        {

        }

        public virtual bool OnLoad()
        {
            return false;
        }

        public virtual void OnResponse()
        {

        }

        public virtual void OnUnload()
        {

        }

        /// <summary>
        /// Do a script method
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public object Do(string methodName, object[] mp)
        {
            object objectized = this;
            MethodInfo method = objectized.GetType().GetMethod(methodName);
            return method.Invoke(this, mp);
        }
    }
}
