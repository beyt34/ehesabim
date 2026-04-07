using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services
{
    public static class AutoMapperConfiguration
    {
        public static IMapper Mapper { get; private set; }

        public static MapperConfiguration MapperConfiguration { get; private set; }

        public static void Init()
        {
            MapperConfiguration = new MapperConfiguration(ConfigureMapper);

            Mapper = MapperConfiguration.CreateMapper();
        }

        private static void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            // CUSTOMER
            cfg.CreateMap<Customer, CustomerDataModel>();
            cfg.CreateMap<CustomerTransaction, CustomerTransactionDataModel>()
               .ForMember(dest => dest.CustomerName, src => src.MapFrom(opt => opt.Customer != null ? opt.Customer.Name : string.Empty))
               .ForMember(dest => dest.Debit, src => src.MapFrom(opt => opt.TypeId == (int)CustomerTransactionTypeEnum.Debit ? opt.Amount : 0))
               .ForMember(dest => dest.Claim, src => src.MapFrom(opt => opt.TypeId == (int)CustomerTransactionTypeEnum.Claim ? opt.Amount : 0))
               .ForMember(dest => dest.Installment, src => src.MapFrom(opt => SetInstallment(opt.InstallmentNo, opt.InstallmentTotal)));

            // BANK ACCOUNT
            cfg.CreateMap<BankAccount, BankAccountDataModel>()
               .ForMember(dest => dest.TypeName, src => src.MapFrom(opt => opt.Type != null ? opt.Type.Name : string.Empty))
               .ForMember(dest => dest.BankName, src => src.MapFrom(opt => opt.Bank != null ? opt.Bank.Name : string.Empty));
            cfg.CreateMap<BankAccountTransaction, BankAccountTransactionDataModel>()
               .ForMember(dest => dest.BankAccountName, src => src.MapFrom(opt => opt.BankAccount != null ? opt.BankAccount.Name : string.Empty))
               .ForMember(dest => dest.Debit, src => src.MapFrom(opt => opt.TypeId == (int)CustomerTransactionTypeEnum.Debit ? opt.Amount : 0))
               .ForMember(dest => dest.Claim, src => src.MapFrom(opt => opt.TypeId == (int)CustomerTransactionTypeEnum.Claim ? opt.Amount : 0))
               .ForMember(dest => dest.Explanation, src => src.MapFrom(opt => GetExplanation(opt.BankCreditSub, opt.BankCreditCardPayment, opt.CustomerTransaction, opt.Expense)));

            // BANK CREDIT
            cfg.CreateMap<BankCredit, BankCreditDataModel>()
               .ForMember(dest => dest.BankName, src => src.MapFrom(opt => opt.Bank != null ? opt.Bank.Name : string.Empty));
            cfg.CreateMap<BankCredit, SelectGuidDataModel>()
               .ForMember(dest => dest.Name, src => src.MapFrom(opt => SetCreditName(opt.Bank, opt.CreditDateTime, opt.Capital, opt.Installment)));
            cfg.CreateMap<BankCreditSub, BankCreditSubDataModel>()
               .ForMember(dest => dest.BankName, src => src.MapFrom(opt => opt.BankCredit != null && opt.BankCredit.Bank != null ? opt.BankCredit.Bank.Name : string.Empty))
               .ForMember(dest => dest.CreditTerm, src => src.MapFrom(opt => opt.BankCredit != null ? opt.BankCredit.CreditDateTime.ToString("yyyy/MM") : string.Empty))
               .ForMember(dest => dest.CreditInfo, src => src.MapFrom(opt => opt.BankCredit != null ? $"{opt.BankCredit.Capital:N0}/{opt.BankCredit.Installment}"
                   : string.Empty));

            // BANK CREDIT CARD
            cfg.CreateMap<BankCreditCard, BankCreditCardDataModel>()
               .ForMember(dest => dest.BankName, src => src.MapFrom(opt => opt.Bank != null ? opt.Bank.Name : string.Empty));
            cfg.CreateMap<BankCreditCardPeriod, BankCreditCardPeriodDataModel>()
               .ForMember(dest => dest.BankCreditCardName, src => src.MapFrom(opt => opt.BankCreditCard != null ? opt.BankCreditCard.Name : string.Empty));
            cfg.CreateMap<BankCreditCardPayment, BankCreditCardPaymentDataModel>()
               .ForMember(dest => dest.BankCreditCardName, src => src.MapFrom(opt => opt.BankCreditCard != null ? opt.BankCreditCard.Name : string.Empty))
               .ForMember(dest => dest.BankAccountName, src => src.MapFrom(opt => GetBankAccountName(opt.BankAccountTransactions)));

            // EXPENSE
            cfg.CreateMap<Expense, ExpenseDataModel>()
               .ForMember(dest => dest.TypeName, src => src.MapFrom(opt => opt.Type != null ? opt.Type.Name : string.Empty))
               .ForMember(dest => dest.ExpenseStoreName, src => src.MapFrom(opt => opt.ExpenseStore != null ? opt.ExpenseStore.Name : string.Empty))
               .ForMember(dest => dest.ExpenseGroupName, src => src.MapFrom(opt => opt.ExpenseGroup != null ? opt.ExpenseGroup.Name : string.Empty))
               .ForMember(dest => dest.Installment, src => src.MapFrom(opt => SetInstallment(opt.InstallmentNo, opt.InstallmentTotal)))
               .ForMember(dest => dest.BankCreditCardName, src => src.MapFrom(opt => opt.BankCreditCard != null ? opt.BankCreditCard.Name : string.Empty))
               .ForMember(dest => dest.BankCreditCardPeriodName, src => src.MapFrom(opt => SetCreditCardPeriod(opt.BankCreditCardPeriod)))
               .ForMember(dest => dest.Explanation, src => src.MapFrom(opt => GetExplanation(opt.BankCreditSub, null, null, null)));
            cfg.CreateMap<ExpenseGroup, ExpenseGroupDataModel>();
            cfg.CreateMap<ExpenseStore, ExpenseStoreDataModel>();
            cfg.CreateMap<ViewExpenseGroupReport, ViewExpenseGroupReportDataModel>();

            // USER
            cfg.CreateMap<User, UserDataModel>()
               .ForMember(dest => dest.ShowCustomer, src => src.MapFrom(opt => opt.ShowCustomer ?? false))
               .ForMember(dest => dest.ShowAccount, src => src.MapFrom(opt => opt.ShowAccount ?? false))
               .ForMember(dest => dest.ShowCredit, src => src.MapFrom(opt => opt.ShowCredit ?? false))
               .ForMember(dest => dest.ShowCard, src => src.MapFrom(opt => opt.ShowCard ?? false))
               .ForMember(dest => dest.ShowExpense, src => src.MapFrom(opt => opt.ShowExpense ?? false))
               .ForMember(dest => dest.ShowCustomerExclusion, src => src.MapFrom(opt => opt.ShowCustomerExclusion ?? false))
               .ForMember(dest => dest.Role, src => src.MapFrom(opt => opt.Role ?? 0));

            // EMAIL
            cfg.CreateMap<EmailQueue, EmailQueueDataModel>();
            cfg.CreateMap<EmailTemplate, EmailTemplateDataModel>();
        }

        private static string SetInstallment(int? no, int? total)
        {
            if (no > 0 && total > 0)
            {
                return $"{no}/{total}";
            }

            return string.Empty;
        }

        private static string SetCreditCardPeriod(BankCreditCardPeriod bankCreditCardPeriod)
        {
            if (bankCreditCardPeriod != null)
            {
                return $"{bankCreditCardPeriod.EndDate:yy}/{bankCreditCardPeriod.EndDate:MM}";
            }

            return string.Empty;
        }

        private static string SetCreditName(Bank bank, DateTime creditDate, decimal capital, int installment)
        {
            return $"{bank?.Name ?? string.Empty} {creditDate:yyyy/MM} {capital:N0}/{installment}";
        }

        private static string GetBankAccountName(ICollection<BankAccountTransaction> bankAccountTransactions)
        {
            var bankAccountTransaction = bankAccountTransactions?.FirstOrDefault(a => !a.IsDeleted);
            if (bankAccountTransaction?.BankAccount != null)
            {
                return bankAccountTransaction.BankAccount.Name;
            }

            return string.Empty;
        }

        private static string GetExplanation(BankCreditSub bankCreditSub, BankCreditCardPayment bankCreditCardPayment, CustomerTransaction customerTransaction, Expense expense)
        {
            if (bankCreditSub?.BankCredit != null)
            {
                var bankCredit = bankCreditSub.BankCredit;
                var creditName = SetCreditName(bankCredit.Bank, bankCredit.CreditDateTime, bankCredit.Capital, bankCredit.Installment);
                return $"Kredi: {creditName}-{bankCreditSub.Installment}";
            }

            if (bankCreditCardPayment?.BankCreditCard != null)
            {
                return $"Kredi Kartı: {bankCreditCardPayment.BankCreditCard.Name}";
            }

            if (customerTransaction?.Customer != null)
            {
                return $"Cari: {customerTransaction.Customer.Name}";
            }

            if (expense?.ExpenseGroup != null)
            {
                return $"Harcama: {expense.ExpenseGroup.Name}";
            }

            return string.Empty;
        }
    }
}
