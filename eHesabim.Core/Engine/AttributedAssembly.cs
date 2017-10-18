using System;
using System.Reflection;

namespace eHesabim.Core.Engine {
    public class AttributedAssembly {
        internal Assembly Assembly { get; set; }

        internal Type PluginAttributeType { get; set; }
    }
}
