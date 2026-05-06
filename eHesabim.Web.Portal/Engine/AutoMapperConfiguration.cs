using System.Configuration;
using AutoMapper;
using eHesabim.Services.Models;
using eHesabim.Web.Portal.Models;

namespace eHesabim.Web.Portal.Engine
{
    public static class AutoMapperConfiguration
    {
        public static IMapper Mapper { get; private set; }

        public static MapperConfiguration MapperConfiguration { get; private set; }

        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(
                cfg =>
                {
                    cfg.CreateMap<CustomerDataModel, CustomerWebModel>();
                    cfg.CreateMap<CustomerDataModel, CustomerEditWebModel>();
                    cfg.CreateMap<CustomerDataModel, DashboardCustomerWebModel>();

                    cfg.CreateMap<CustomerTransactionDataModel, CustomerTransactionWebModel>();
                    cfg.CreateMap<CustomerTransactionDataModel, CustomerTransactionEditWebModel>()
                       .ForMember(dest => dest.FullPath, src => src.MapFrom(opt => GetFullPath(opt.FileName)));
                    cfg.CreateMap<CustomerAbstractDataModel, CustomerAbstractWebModel>();

                    cfg.CreateMap<BankAccountDataModel, BankAccountWebModel>();
                    cfg.CreateMap<BankAccountDataModel, BankAccountEditWebModel>();
                    cfg.CreateMap<BankAccountDataModel, DashboardBankAccountWebModel>();

                    cfg.CreateMap<BankAccountTransactionDataModel, BankAccountTransactionWebModel>();
                    cfg.CreateMap<BankAccountTransactionDataModel, BankAccountTransactionEditWebModel>();

                    cfg.CreateMap<BankCreditDataModel, BankCreditWebModel>();
                    cfg.CreateMap<BankCreditDataModel, BankCreditEditWebModel>();

                    cfg.CreateMap<BankCreditSubDataModel, BankCreditSubWebModel>();
                    cfg.CreateMap<BankCreditSubDataModel, BankCreditSubEditWebModel>();

                    cfg.CreateMap<BankCreditCardDataModel, BankCreditCardWebModel>();
                    cfg.CreateMap<BankCreditCardDataModel, BankCreditCardEditWebModel>();

                    cfg.CreateMap<BankCreditCardPeriodDataModel, BankCreditCardPeriodWebModel>();
                    cfg.CreateMap<BankCreditCardPeriodDataModel, BankCreditCardPeriodEditWebModel>();

                    cfg.CreateMap<BankCreditCardPaymentDataModel, BankCreditCardPaymentWebModel>();
                    cfg.CreateMap<BankCreditCardPaymentDataModel, BankCreditCardPaymentEditWebModel>();

                    cfg.CreateMap<ExpenseDataModel, ExpenseWebModel>();
                    cfg.CreateMap<ExpenseDataModel, ExpenseEditWebModel>();

                    cfg.CreateMap<ExpenseGroupDataModel, ExpenseGroupWebModel>();
                    cfg.CreateMap<ExpenseGroupDataModel, ExpenseGroupEditWebModel>();

                    cfg.CreateMap<ViewExpenseGroupReportDataModel, ViewExpenseGroupReportWebModel>();

                    cfg.CreateMap<ExpenseStoreDataModel, ExpenseStoreWebModel>();
                    cfg.CreateMap<ExpenseStoreDataModel, ExpenseStoreEditWebModel>();

                    cfg.CreateMap<UserDataModel, UserWebModel>();
                    cfg.CreateMap<UserDataModel, UserEditWebModel>();
                    cfg.CreateMap<UserDataModel, UserUpdateProfileWebModel>();
                });

            Mapper = MapperConfiguration.CreateMapper();
        }

        private static string GetFullPath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return string.Empty;
            }

            return string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["FilePath"], "files", fileName);
        }
    }
}