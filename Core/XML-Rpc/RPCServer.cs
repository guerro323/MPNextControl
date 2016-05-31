using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPNextControl.Core.XML_Rpc.Gbx;
using System.Reflection;

namespace MPNextControl.Core.XMLRPC
{
    public class ServiceRPCServer
    {
        public Services.Static.GameInfo GameInfo = new Services.Static.GameInfo();
    }

    public class RPCServer
    {
        public ServiceRPCServer Service;
        public string SuperAdminLogin;
        public string SuperAdminPassword;

        public dynamic Script;

        public GbxRemote Remote;

        /// <summary>
        /// Invoke a method.
        /// Use this function only in a script
        /// else, use Script.Method(.'arg'.);
        /// </summary>
        /// <param name="methodName">The method name</param>
        /// <param name="methodParams">Params</param>
        public object Do(string methodName, object methodParams)
        {
            object objectized = Script;
            MethodInfo method = objectized.GetType().GetMethod(methodName);
            if (methodParams is object[]) return method.Invoke(Script, (object[])methodParams);
            else return method.Invoke(Script, new object [ ] { methodParams });
        }

        public RPCServer(string Ip, int Port)
        {
            Remote = new GbxRemote(Ip, Port);
            Remote.TryConnect();

            Service = new ServiceRPCServer();

            // Search a RPCServer script to get callbacks and responses manager
            Script = Plugins.PluginManager.Load(Services.StaticPower.CurrentCore, "Plugins/handle.cs");
            Script.SetRPCServer(this);
            Service.GameInfo = Script.GetGameInfo();
        }
    }
}
