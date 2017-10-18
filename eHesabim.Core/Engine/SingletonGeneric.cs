namespace eHesabim.Core.Engine {
    public class Singleton<T> : Singleton {
        private static T instance;

        public static T Instance {
            get {
                return instance;
            }

            set {
                instance = value;
                SingletonList[typeof(T)] = value;
            }
        }
    }
}
