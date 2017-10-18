using System;
using System.Linq;
using Autofac;

namespace eHesabim.Core.Engine {
    public class Engine : IEngine {
        private ContainerManager containerManager;

        public Engine()
            : this(new ContainerConfigurer()) {
        }

        public Engine(ContainerConfigurer configurer) {
            InitializeContainer(configurer);
        }

        public IContainer Container {
            get { return containerManager.Container; }
        }

        public ContainerManager ContainerManager {
            get { return containerManager; }
        }

        public void Initialize() {
            ////startup tasks
            RunStartupTasks();
        }

        public T Resolve<T>() where T : class {
            return ContainerManager.Resolve<T>();
        }

        public object Resolve(Type type) {
            return ContainerManager.Resolve(type);
        }

        public T[] ResolveAll<T>() {
            return ContainerManager.ResolveAll<T>();
        }

        private void RunStartupTasks() {
            var typeFinder = containerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = startUpTaskTypes.Select(startUpTaskType => (IStartupTask)Activator.CreateInstance(startUpTaskType)).ToList();
            ////sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks) {
                startUpTask.Execute();
            }
        }

        private void InitializeContainer(ContainerConfigurer configurer) {
            var builder = new ContainerBuilder();

            containerManager = new ContainerManager(builder.Build());
            configurer.Configure(this, this.containerManager);
        }
    }
}
