using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Linq;

namespace NetCoreConsoleApp
{
    public class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        private List<Assembly> _loaded;
        private Dictionary<string, Assembly> _shared;

        private string _path;

        private AssemblyDependencyResolver _resolver;

        public ModuleAssemblyLoadContext(string name, string path, params Type[] sharedTypes) : base(name)
        {
            _path = path;
            _resolver = new AssemblyDependencyResolver(_path);

            _loaded = new List<Assembly>();
            _shared = new Dictionary<string, Assembly>();

            if (sharedTypes != null)
            {
                foreach (Type sharedType in sharedTypes)
                {
                    _shared[Path.GetFileName(sharedType.Assembly.Location)] = sharedType.Assembly;
                }
            }
        }

        public void Scan()
        {
            foreach (string dll in Directory.EnumerateFiles(_path, "*.dll"))
            {
                var file = Path.GetFileName(dll);

                if (_shared.ContainsKey(file))
                {
                    continue;
                }

                var asm = this.LoadFromAssemblyPath(dll);

                _loaded.Add(asm);
            }
        }

        public IEnumerable<T> GetImplementations<T>()
        {
            return _loaded
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(T).IsAssignableFrom(t))
                .Select(t => Activator.CreateInstance(t))
                .Cast<T>();
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            string filename = $"{assemblyName.Name}.dll";
            if (_shared.ContainsKey(filename))
            {
                return _shared[filename];
            }

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }
}
