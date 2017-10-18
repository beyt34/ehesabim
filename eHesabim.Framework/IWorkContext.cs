using eHesabim.Services.Models;

namespace eHesabim.Framework {
    public interface IWorkContext {
        UserDataModel CurrentUser { get; }

        void SignIn(UserDataModel user, bool createPersistentCookie);

        void SignOut();
    }
}