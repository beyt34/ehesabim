using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface IExpenseService {
        Guid AddUpdateExpense(Guid id, int userId, int typeId, DateTime expenseDateTime, Guid storeId, string storeName, Guid groupId, string groupName, decimal amount, int? installmentNo, int? installmentTotal, Guid? creditCardId, Guid? creditCardPeriodId, bool isExclusion, string notes, Guid? bankAccountId, out string errMessage);

        List<ExpenseDataModel> GetExpenseList(int userId, Guid? creditCardId, Guid? creditCardPeriodId, int typeId, Guid? storeId, Guid? groupId, DateTime? startDate, DateTime? endDate, int? nextPeriodId, int? excludeId, string notes, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal expenseMe, out decimal expenseExclusion);
        
        ExpenseDataModel GetExpenseById(Guid id, int userId);
        
        void DeleteExpense(Guid id, int userId, out string errMessage);
        
        Guid AddUpdateExpenseGroup(Guid id, int userId, string name, out string errMessage);
        
        List<ExpenseGroupDataModel> GetExpenseGroupList(int userId, string name, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        List<SelectGuidDataModel> GetExpenseGroupList(int userId);
        
        ExpenseGroupDataModel GetExpenseGroupById(Guid id, int userId);
        
        void DeleteExpenseGroup(Guid id, int userId, out string errMessage);
        
        List<int> GetExpenseYearList(int userId);
        
        List<ViewExpenseGroupReportDataModel> GetExpenseGroupReport(int userId, int year);
        
        Guid AddUpdateExpenseStore(Guid id, int userId, string name, out string errMessage);
        
        List<ExpenseStoreDataModel> GetExpenseStoreList(int userId, string name, string sort, bool sortDescending, int page, int pageSize, out int total);
        
        List<SelectGuidDataModel> GetExpenseStoreList(int userId);
        
        ExpenseStoreDataModel GetExpenseStoreById(Guid id, int userId);
        
        void DeleteExpenseStore(Guid id, int userId, out string errMessage);
    }
}
