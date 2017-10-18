using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IBankAccountService {
        List<BankAccountDataModel> GetDashboardBankAccountList(int userId);

        Guid AddUpdateBankAccount(Guid id, int userId, int typeId, string name, int? bankId, string iban, decimal? limit, bool isActive, out string errMessage);

        List<BankAccountDataModel> GetBankAccountList(int userId, int typeId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total);

        List<SelectGuidDataModel> GetBankAccountList(int userId);

        BankAccountDataModel GetBankAccountById(Guid id, int userId);

        void DeleteBankAccount(Guid id, int userId, out string errMessage);

        Guid AddUpdateBankAccountTransaction(Guid id, int userId, Guid bankAccountId, DateTime dateTime, string name, int typeId, decimal amount, Guid? tobankAccountId, out string errMessage);

        List<BankAccountTransactionDataModel> GetBankAccountTransactionList(int userId, Guid? bankAccountId, string name, DateTime? startDate, DateTime? endDate, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal);

        BankAccountTransactionDataModel GetBankAccountTransactionById(Guid id, int userId);

        void DeleteBankAccountTransaction(Guid id, int userId, out string errMessage);
    }
}
