using NetStandardCommon;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreModule
{
    public class Module : IModule
    {
        public void Start()
        {
            Program.Main(null);
        }
    }
}
