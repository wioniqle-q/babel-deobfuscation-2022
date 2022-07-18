                            using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BabelDeobfuscator.Protections;
using BabelDeobfuscator.Protections.Constants;
using BabelDeobfuscator.Protections.ControlFlow;
using BabelDeobfuscator.Protections.MethodEncryption;
using BabelDeobfuscator.Protections.ProxyCalls;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;

namespace BabelDeobfuscator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filename;
            try
            {
                filename = args[0];
            }
            catch
            {
                Console.Write("Enter path: ");
                filename = Console.ReadLine().Replace(@"""", "");
            }
            Console.Title = "Atomic Deobfuscator By CursedSheep ;D";
            Console.WriteLine("Atomic Deobfuscator by Cursedsheep ;D");
            ModuleDefMD module = ModuleDefMD.Load(filename);
            ModuleWriterOptions mod = new ModuleWriterOptions(module);
            mod.MetaDataLogger = DummyLogger.NoThrowInstance;
            mod.MetaDataOptions.Flags |= MetaDataFlags.KeepOldMaxStack;

            AssemblyResolver assemblyResolver = new AssemblyResolver();
            ModuleContext moduleContext = new ModuleContext(assemblyResolver);
            assemblyResolver.DefaultModuleContext = moduleContext;
            assemblyResolver.EnableTypeDefCache = true;
            module.Location = filename;
            List<AssemblyRef> list = module.GetAssemblyRefs().ToList<AssemblyRef>();
            module.Context = moduleContext;
            foreach (AssemblyRef assemblyRef in list)
            {
                if (assemblyRef != null)
                {
                    AssemblyDef assemblyDef = assemblyResolver.Resolve(assemblyRef.FullName, module);
                    module.Context.AssemblyResolver.AddToCache(assemblyDef);
                }
            }

            Program.asm = Assembly.LoadFrom(filename);
            De4Dot.Cflow(module);
            VMDecryptor.run(module, Program.asm);
            Delegates.CleanDelegates(module);
            Ints.CleanInts(module);
            Ints.CleanFloats(module);
            Ints.CleanDouble(module);
            Strings.CleanStringMethodOne(module);
            Strings.CleanStringMethodTwo(module);
            De4Dot.Cflow(module);
            Ints.CleanInts(module);
            Ints.CleanFloats(module);
            Ints.CleanDouble(module);
            Strings.CleanStringMethodOne(module);
            Strings.CleanStringMethodTwo(module);
            De4Dot.Cflow(module);
            AntiTamper.AntiTamp(module);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Unpack Complete !");

            string patho = Path.GetDirectoryName(filename) + "\\" + Path.GetFileNameWithoutExtension(filename) + "-Deobfuscated" + Path.GetExtension(filename);
            module.Write(patho, mod);
            Console.WriteLine("saved to " + patho);
            Console.ReadKey();

  



        }
        public static Assembly asm;
    }
}