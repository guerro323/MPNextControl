using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LOW = MPNextControl.Core;
using PLUGIN = MPNextControl.Plugins;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace MPNextControl
{
    class Program
    {
        static void Main(string [ ] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveEventHandler;

            Console.WriteLine("Launching. . .");
            /*Console.Beep(120, 100);
            Console.Beep(500, 300);
            Console.Beep(1500, 100);
            Thread.Sleep(900);*/
            LOW.Manager core = new LOW.Manager();
            PLUGIN.PluginManager.Load(Services.StaticPower.CurrentCore, "*");
            Console.WriteLine("Core Launched !");

            while (true)
            {
                Thread.Sleep(1);
                string command = Console.ReadLine();
            }

        }

        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {

            String dllName = new AssemblyName(args.Name).Name + ".dll";

            var assem = Assembly.GetExecutingAssembly();

            String resourceName = assem.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));

            if (resourceName == null) return null; // Not found, maybe another handler will find it

            using (var stream = assem.GetManifestResourceStream(resourceName))
            {

                Byte[] assemblyData = new Byte[stream.Length];

                stream.Read(assemblyData, 0, assemblyData.Length);

                return Assembly.Load(assemblyData);

            }
        }
    }
}