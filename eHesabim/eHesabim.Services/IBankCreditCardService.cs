using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IBankCreditCardService {
        Guid AddUpdateBankCreditCard(Guid id, int userId, Guid? parentId, int bankId, string name, string cardNo, decimal limit, string expiredMonth, string expiredYear, int? order, bool isActive, out string errMessage);
        
        List<BankCreditCardDataModel> GetBankCreditCardList(int userId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        List<SelectGuidDataModel> GetBankCreditCardList(int userId, Guid? hideBankCreditCardId, Guid? showBankCreditCardId);
        
        BankCreditCardDataModel GetBankCreditCardById(Guid id, int userId);
        
        void DeleteBankCreditCard(Guid id, int userId, out string errMessage);
        
        Guid AddUpdateBankCreditCardPeriod(Guid id, int userId, Guid creditCardId, DateTime startDate, DateTime endDate, DateTime paymentDate, bool setExpense, out string errMessage);
        
        List<BankCreditCardPeriodDataModel> GetBankCreditCardPeriodList(int userId, Guid? creditCardId, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        List<SelectGuidDataModel> GetBankCreditCardPeriodList(int userId, Guid creditCardId);
        
        BankCreditCardPeriodDataModel GetBankCreditCardPeriodById(Guid id, int userId);
        
        void DeleteBankCreditCardPeriod(Guid id, int userId, out string errMessage);
        
        Guid AddUpdateBankCreditCardPayment(Guid id, int userId, Guid creditCardId, DateTime paymentDateTime, decimal amount, bool hasPoint, Guid? bankAccountId, out string errMessage);
        
        List<BankCreditCardPaymentDataModel> GetBankCreditCardPaymentList(int userId, Guid? creditCardId, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        BankCreditCardPaymentDataModel GetBankCreditCardPaymentById(Guid id, int userId);
        
        void DeleteBankCreditCardPayment(Guid id, int userId, out string errMessage);
        
        List<BankCreditCardDataModel> GetDashboardBankCreditCardList(int userId);

        int GetBankId(Guid? bankCreditCardId);

        void SetExpensePeriod(Guid id, int userId);
    }
}
