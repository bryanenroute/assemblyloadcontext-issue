using NetStandardCommon;
using System;
using System.IO;

namespace NetCoreConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            LoadNetCoreModule();
            LoadAspNetCoreModule();
        }

        static void LoadNetCoreModule()
        {
            //Works!
            FileInfo asm = new FileInfo(@"..\..\..\..\NetCoreModule\bin\debug\netcoreapp3.0\NetCoreModule.dll");
            var moduleDirectory = asm.DirectoryName;

            ModuleAssemblyLoadContext context = new ModuleAssemblyLoadContext(asm.Name, moduleDirectory, typeof(IModule));
            context.Scan();

            foreach (var module in context.GetImplementations<IModule>())
            {
                module.Start();
            }
        }

        static void LoadAspNetCoreModule()
        {
            //Fails!
            FileInfo asm = new FileInfo(@"..\..\..\..\AspNetCoreApp\bin\debug\netcoreapp3.0\AspNetCoreApp.dll");
            var moduleDirectory = asm.DirectoryName;

            ModuleAssemblyLoadContext context = new ModuleAssemblyLoadContext(asm.Name, moduleDirectory, typeof(IModule));
            context.Scan();

            foreach (var module in context.GetImplementations<IModule>())
            {
                module.Start();
            }
        }
    }
}
