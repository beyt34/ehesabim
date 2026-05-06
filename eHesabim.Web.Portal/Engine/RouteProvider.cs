using System.Web.Mvc;
using System.Web.Routing;
using eHesabim.Core.Routes;

namespace eHesabim.Web.Portal.Engine
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get { return 0; }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            // main
            routes.MapRoute(RouteNames.Login, "giris", new { controller = "Login", action = "Index" });
            routes.MapRoute(RouteNames.Logout, "cikis", new { controller = "Login", action = "Logout" });
            routes.MapRoute(RouteNames.Register, "uye-ol", new { controller = "Login", action = "Register" });
            routes.MapRoute(RouteNames.PasswordRecovery, "sifremi-unuttum", new { controller = "Login", action = "PasswordRecovery" });
            routes.MapRoute(RouteNames.PasswordRecoveryConfirm, "sifre-belirle/{token}", new { controller = "Login", action = "PasswordRecoveryConfirm", token = UrlParameter.Optional });
            routes.MapRoute(RouteNames.Home, "dashboard", new { controller = "Home", action = "Index" });
            routes.MapRoute(RouteNames.Landing, string.Empty, new { controller = "Landing", action = "Index" });
            routes.MapRoute(RouteNames.Heartbeat, "heartbeat", new { controller = "Landing", action = "Heartbeat" });

            // cari
            routes.MapRoute(RouteNames.CustomerList, "cari-kartlar", new { controller = "Customer", action = "CustomerList" });
            routes.MapRoute(RouteNames.CustomerTransactionList, "cari-hareketler", new { controller = "Customer", action = "CustomerTransactionList" });
            routes.MapRoute(RouteNames.CustomerAbstract, "cari-ekstre", new { controller = "Customer", action = "CustomerAbstract" });

            // banka
            routes.MapRoute(RouteNames.BankAccountList, "hesaplar", new { controller = "BankAccount", action = "BankAccountList" });
            routes.MapRoute(RouteNames.BankAccountTransactionList, "hesap-hareketleri", new { controller = "BankAccount", action = "BankAccountTransactionList" });

            routes.MapRoute(RouteNames.BankCreditList, "krediler", new { controller = "Bank", action = "BankCreditList" });
            routes.MapRoute(RouteNames.BankCreditSubList, "kredi-taksitleri", new { controller = "Bank", action = "BankCreditSubList" });

            routes.MapRoute(RouteNames.BankCreditCardList, "kredi-kartlari", new { controller = "Bank", action = "BankCreditCardList" });
            routes.MapRoute(RouteNames.BankCreditCardPeriodList, "kredi-karti-donemleri", new { controller = "Bank", action = "BankCreditCardPeriodList" });
            routes.MapRoute(RouteNames.BankCreditCardPaymentList, "kredi-karti-odemeleri", new { controller = "Bank", action = "BankCreditCardPaymentList" });

            // gider
            routes.MapRoute(RouteNames.ExpenseList, "harcamalar", new { controller = "Expense", action = "ExpenseList" });
            routes.MapRoute(RouteNames.ExpenseGroupList, "gider-gruplari", new { controller = "Expense", action = "ExpenseGroupList" });
            routes.MapRoute(RouteNames.ExpenseGroupReport, "gider-raporu", new { controller = "Expense", action = "ExpenseGroupReport" });
            routes.MapRoute(RouteNames.ExpenseStoreList, "magazalar", new { controller = "Expense", action = "ExpenseStoreList" });

            // sistem
            routes.MapRoute(RouteNames.UserList, "kullanici-listesi", new { controller = "User", action = "UserList" });
            routes.MapRoute(RouteNames.UserEdit, "kullanici-detay/{id}", new { controller = "User", action = "UserEdit", id = UrlParameter.Optional });
            routes.MapRoute(RouteNames.PermissionList, "yetki-listesi", new { controller = "User", action = "PermissionList" });
            routes.MapRoute(RouteNames.PermissionEdit, "yetki-detay/{id}", new { controller = "User", action = "PermissionEdit", id = UrlParameter.Optional });

            routes.MapRoute(RouteNames.AccessDenied, "yetki-yok", new { controller = "Base", action = "AccessDenied" });
            routes.MapRoute(RouteNames.Error404, "sayfa-bulunamadi", new { controller = "Base", action = "Error404" });
            routes.MapRoute(RouteNames.Error500, "hata", new { controller = "Base", action = "Error500" });

            routes.MapRoute(RouteNames.Default, "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}