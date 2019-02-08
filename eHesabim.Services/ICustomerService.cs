using System;
using System.Collections.Generic;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public interface ICustomerService {
        List<CustomerDataModel> GetDashboardCustomerList(int userId, bool showCustomerExclusion);

        Guid AddUpdateCustomer(Guid id, int userId, string name, string email, string phone, string address, int? cityId, int? countyId, DateTime? birthDate, string notes, bool isExclusion, bool isActive, out string errMessage);

        List<CustomerDataModel> GetCustomerList(int userId, string name, string email, int? excludeId, int? activeId, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal);
        
        List<SelectGuidDataModel> GetCustomerList(int userId);

        CustomerDataModel GetCustomerById(Guid id, int userId);

        void DeleteCustomer(Guid id, int userId, out string errMessage);

        Guid AddUpdateCustomerTransaction(Guid id, int userId, Guid customerId, DateTime trnDateTime, DateTime? dueDateTime, string name, string fileName, int typeId, decimal amount, int? installmentNo, int? installmentTotal, bool issales, Guid? bankAccountId, out string errMessage);

        void UpdateCustomerTransactionFileName(Guid id, string fileName);

        List<CustomerTransactionDataModel> GetCustomerTransactionList(int userId, Guid? customerId, string name, DateTime? startDate, DateTime? endDate, int? excludeId, int? saleId, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal);

        CustomerTransactionDataModel GetCustomerTransactionById(Guid id, int userId);

        void DeleteCustomerTransaction(Guid id, int userId, out string errMessage);

        List<CustomerAbstractDataModel> GetCustomerAbstract(int userId, Guid customerId, DateTime? startDate, DateTime? endDate, out string customerName);
    }
}
