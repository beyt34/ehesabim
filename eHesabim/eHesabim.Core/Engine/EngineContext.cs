using System.Runtime.CompilerServices;

namespace eHesabim.Core.Engine {
    public class EngineContext {
        public static IEngine Current {
            get {
                if (Singleton<IEngine>.Instance == null) {
                    Initialize(false);
                }

                return Singleton<IEngine>.Instance;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IEngine Initialize(bool forceRecreate) {
            if (Singleton<IEngine>.Instance == null || forceRecreate) {
                Singleton<IEngine>.Instance = new Engine();
                Singleton<IEngine>.Instance.Initialize();
            }

            return Singleton<IEngine>.Instance;
        }

        public static void Replace(IEngine engine) {
            Singleton<IEngine>.Instance = engine;
        }
    }
}
