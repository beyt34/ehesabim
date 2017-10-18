using System.Collections.Generic;
using System.Reflection;

namespace eHesabim.Core.Engine {
    public class WebAppTypeFinder : AppDomainTypeFinder {
        private readonly IWebHelper webHelper;

        private bool ensureBinFolderAssembliesLoaded = true;

        private bool binFolderAssembliesLoaded;

        public WebAppTypeFinder(IWebHelper webHelper) {
            this.webHelper = webHelper;
            this.ensureBinFolderAssembliesLoaded = true;
            ////this_.ensurePluginFolderAssembliesLoaded = config.DynamicDiscovery;
        }

        public bool EnsureBinFolderAssembliesLoaded {
            get { return ensureBinFolderAssembliesLoaded; }
            set { ensureBinFolderAssembliesLoaded = value; }
        }

        public override IList<Assembly> GetAssemblies() {
            if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded) {
                binFolderAssembliesLoaded = true;
                LoadMatchingAssemblies(webHelper.MapPath("~/bin"));
            }

            return base.GetAssemblies();
        }
    }
}
