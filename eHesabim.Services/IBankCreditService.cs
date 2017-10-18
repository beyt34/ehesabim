using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IBankCreditService {
        Guid AddUpdateBankCredit(Guid id, int userId, int bankId, DateTime creditDateTime, decimal capital, decimal rate, int installment, decimal monthlyPayment, decimal expense, out string errMessage);
        
        List<BankCreditDataModel> GetBankCreditList(int userId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        List<SelectGuidDataModel> GetBankCreditList(int userId);
        
        BankCreditDataModel GetBankCreditById(Guid id, int userId);
        
        void DeleteBankCredit(Guid id, int userId, out string errMessage);
        
        Guid AddUpdateBankCreditSub(Guid id, int userId, Guid bankCreditId, int installment, DateTime installmentDate, DateTime? paymentDate, decimal installmentAmount, decimal capitalAmount, decimal interestAmount, Guid? bankAccountId, Guid? expenseGroupId, out string errMessage);
        
        List<BankCreditSubDataModel> GetBankCreditSubList(int userId, Guid? bankCreditId, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        BankCreditSubDataModel GetBankCreditSubById(Guid id, int userId);
        
        void DeleteBankCreditSub(Guid id, int userId, out string errMessage);
        
        List<BankCreditDataModel> GetDashboardBankCreditList(int userId);
    }
}
