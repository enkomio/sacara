using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LoadEncryptedAssembly
{
    class Program
    {
        static Program()
        {
            DropDependencies();
        }

        static void PrintBanner()
        {
            Console.WriteLine("-=[ Dynamically load encrypted Assembly SacaraVm sample ]=-");
            Console.WriteLine("For more information pass -h as argument");
        }

        static void PrintUsage()
        {
            Console.WriteLine(@"
Accepted arguments:
-h                  print this help
-b  <exe> <pwd>     build a copy of this program by embedding the <exe> program 
                    in the resource and using <pwd> as encryption password
");
        }
        
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly assemblyLoaded = null;
            if (args.Name.StartsWith("ES.Sacara.Ir.Assembler"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.ES_Sacara_Ir_Assembler);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("ES.Sacara.Ir.Core"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.ES_Sacara_Ir_Core);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("ES.Sacara.Ir.Obfuscator"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.ES_Sacara_Ir_Obfuscator);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("Newtonsoft.Json"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.Newtonsoft_Json);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("ES.Sacara.Ir.Parser"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.ES_Sacara_Ir_Parser);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("FsLexYacc.Runtime"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.FsLexYacc_Runtime);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            else if (args.Name.StartsWith("ES.SacaraVm"))
            {
                assemblyLoaded = Assembly.Load(Properties.Resources.ES_SacaraVm);
                Console.WriteLine("Loaded: {0}", args.Name);
            }
            return assemblyLoaded;
        }

        static Boolean DropFile(String filename, Byte[] value)
        {
            var curDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(curDir, filename);
            if (!File.Exists(fullPath))
            {
                File.WriteAllBytes(fullPath, value);
                return true;
            }
            return false;
        }

        static Boolean DropFile(String filename, String value)
        {
            var curDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(curDir, filename);
            if (!File.Exists(fullPath))
            {
                File.WriteAllText(fullPath, value);
                return true;
            }
            return false;
        }

        static void DropDependencies()
        {
            var configFile = Path.GetFileName(Assembly.GetEntryAssembly().Location) + ".config";
            var fileDropped =
                DropFile("FSharp.Core.dll", Properties.Resources.FSharp_Core) ||
                DropFile(configFile, Properties.Resources.LoadEncryptedAssembly_exe) ||
                DropFile("vm_opcodes.json", Properties.Resources.vm_opcodes);

            if (fileDropped)
            {
                Console.WriteLine("Dependencies dropped, restart");

                // restart program
                var arguments = String.Join(" ", Environment.GetCommandLineArgs().Skip(1));
                var procInfo = new ProcessStartInfo(Assembly.GetEntryAssembly().Location, arguments)
                {
                    UseShellExecute = false
                };
                Process.Start(procInfo).WaitForExit();
                Environment.Exit(0);
            }            
        }

        static void Main(string[] args)
        {
            PrintBanner();            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            if (args.Contains("-h"))
            {
                PrintUsage();
            }
            else if (args.Contains("-b") && args.Length > 2)
            {
                BuildCommand.Build(args[1], args[2]);                
            }
            else
            {
                RunCommand.Run();
            }
        }
    }
}
