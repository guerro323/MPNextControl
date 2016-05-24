using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScriptCs.Hosting.ReplCommands;

using LOW = MPNextControl.Core;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace MPNextControl
{
    class Program
    {
        static void Main(string [ ] args)
        {
            Console.WriteLine("Launching. . .");
            LOW.Manager core = new LOW.Manager();
            Console.WriteLine("Core Launched !");

            string code = @"
using System;

class CodeToBeCompiled
{
    static void Main()
    {
        Console.WriteLine(""Hello world"");
    }
}";

            var codeProvider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true
            };
            var results = codeProvider.CompileAssemblyFromSource
            (parameters, new[] { code });
            results.CompiledAssembly.EntryPoint.Invoke(null, null);

            while (true)
            {
                Thread.Sleep(1);
            }
           
        }
    }
}
