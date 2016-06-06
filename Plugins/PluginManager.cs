using CSScriptLibrary;
using MPNextControl.Services;
using MPNextControl.Services.RunTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis.Scripting;

namespace MPNextControl.Plugins
{
    static class PluginManager
    {
        static public Dictionary<string, dynamic> LoadedPlugins = new Dictionary<string, dynamic>();

        static public Plugin Load(Core.Manager core, string pluginName)
        {
            if (pluginName == null || pluginName.Length == 0) return new Plugin();
            if (pluginName == "*")
            {
                FileInfo[] files = new DirectoryInfo("Plugins").GetFiles("*", SearchOption.AllDirectories);
                List<string> foundPlugins = new List<string>();
                List<string> foundDll = new List<string>();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    if (fileInfo.Name.StartsWith("_") ||
                        fileInfo.Name.Contains("handle") ||
                        fileInfo.Name.Contains("Helper")) continue;
                    if (fileInfo.Extension.ToLower() == ".cs")
                    {
                        foundPlugins.Add(fileInfo.FullName);
                    }
                    if (fileInfo.Extension.ToLower() == ".dll")
                    {
                        foundDll.Add(fileInfo.FullName);
                    }
                }
                foreach (var plugin in foundPlugins)
                {
                    Console.WriteLine(plugin);
                    var compiler = CSScript.RoslynEvaluator;
                    compiler.Reset(false);
                    compiler.ReferenceDomainAssemblies(DomainAssemblies.AllStaticNonGAC);
                    //compiler.;
                    var assembly = compiler.CompileCode(ReadText(plugin));
                    var types = assembly.GetTypes();
                    dynamic pluginScript = Activator.CreateInstance(types.Where(t => t.BaseType == typeof(Plugin)).ToArray()[0]);
                    pluginScript.Init(core, core.PluginConfig);
                    core.PluginConfig.LoadPluginsConfig(pluginScript.StaticName);
                    var Success = pluginScript.OnLoad();
                    if (Success) LoadedPlugins.Add(pluginScript.StaticName, pluginScript);
                    Console.WriteLine("keking?");
                }
                foreach (var dll in foundDll)
                {
                    var assembly = Assembly.LoadFile(dll);
                    var types = assembly.GetTypes();
                    if (!types.Where(t => t.BaseType == typeof(Plugin)).ToArray().Any())
                        return new Plugin();
                    Plugin pluginScript = (Plugin)Activator.CreateInstance(types.Where(t => t.BaseType == typeof(Plugin)).ToArray()[0]);
                    pluginScript.Init(core, core.PluginConfig);
                    core.PluginConfig.LoadPluginsConfig(pluginScript.StaticName);
                    var Success = pluginScript.OnLoad();
                    //if (Success) LoadedPlugins.Add(pluginScript.StaticName, pluginScript);
                }
                return new Plugin();
            }
            else
            {
                if (pluginName.EndsWith(".cs"))
                    
                {
                    var compiler = CSScript.MonoEvaluator;
                    compiler.Reset(true);
                    //compiler.CompilerSettings.Version = Mono.CSharp.LanguageVersion.Future;
                    var assembly = compiler.CompileCode(ReadText(pluginName));
                    var types = assembly.GetTypes();
                    dynamic pluginScript = Activator.CreateInstance(types.Where(t => t.BaseType == typeof(Plugin)).ToArray()[0]);
                    pluginScript.Init(core);
                    var Success = pluginScript.OnLoad();
                    //if (Success) LoadedPlugins.Add(pluginScript.StaticName, pluginScript);
                    return pluginScript;
                }
                else
                {
                    var assembly = Assembly.LoadFile(Environment.CurrentDirectory + "/" + pluginName);
                    var types = assembly.GetTypes();
                    dynamic pluginScript = Activator.CreateInstance(types.Where(t => t.BaseType == typeof(Plugin)).ToArray()[0]);
                    pluginScript.Init(core);
                    var Success = pluginScript.OnLoad();
                    //if (Success) LoadedPlugins.Add(pluginScript.StaticName, pluginScript);
                    Console.WriteLine(pluginScript.StaticName);
                    return pluginScript;
                }
            }
            throw new Exception("Plugin not found");
        }
        static public void Unload()
        {
            // TODO
        }

        static public string ReadText(string path)
        {
            return File.ReadAllText(@path);
        }

    }
}
