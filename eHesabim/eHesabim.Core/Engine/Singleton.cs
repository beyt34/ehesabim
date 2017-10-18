using System;
using System.Collections.Generic;

namespace eHesabim.Core.Engine {
    public class Singleton {
        public static readonly IDictionary<Type, object> AllSingletons;

        static Singleton() {
            AllSingletons = new Dictionary<Type, object>();
        }

        public static IDictionary<Type, object> SingletonList {
            get { return AllSingletons; }
        }
    }
}
