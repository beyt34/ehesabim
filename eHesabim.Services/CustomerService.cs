using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using eHesabim.Core;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class CustomerService : ICustomerService {
        private readonly IRepository<Customer, Guid> customerRepository;
        private readonly IRepository<CustomerTransaction, Guid> customerTransactionRepository;
        private readonly IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository;

        public CustomerService(IRepository<Customer, Guid> customerRepository, IRepository<CustomerTransaction, Guid> customerTransactionRepository, IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository) {
            this.customerRepository = customerRepository;
            this.customerTransactionRepository = customerTransactionRepository;
            this.bankAccountTransactionRepository = bankAccountTransactionRepository;
        }

        public List<CustomerDataModel> GetDashboardCustomerList(int userId, bool showCustomerExclusion) {
            const int DebitId = (int)CustomerTransactionTypeEnum.Debit;
            const int ClaimId = (int)CustomerTransactionTypeEnum.Claim;

            Expression<Func<CustomerTransaction, bool>> query = f => !f.IsDeleted && f.UserId == userId;
            if (!showCustomerExclusion) {
                query = query.And(a => !a.Customer.IsExclusion);
            }

            return customerTransactionRepository
                        .Query(query)
                        .Include(i => i.Customer)
                        .GroupBy(g => new { g.Customer.Id, g.Customer.Name, g.Customer.IsExclusion })
                        .Select(s => new CustomerDataModel {
                            Id = s.Key.Id,
                            Name = s.Key.Name,
                            IsExclusion = s.Key.IsExclusion,
                            Debit = s.Sum(ss => ss.TypeId == DebitId ? ss.Amount : 0),
                            Claim = s.Sum(ss => ss.TypeId == ClaimId ? ss.Amount : 0),
                            Net = s.Sum(ss => ss.TypeId == DebitId ? ss.Amount : ss.Amount * -1),
                            DueDebit = s.Sum(ss => ss.TypeId == DebitId
                                                ? (ss.DueDateTime ?? DateTime.Today) <= DateTime.Today ? ss.Amount : 0
                                                : (ss.DueDateTime ?? DateTime.Today) <= DateTime.Today ? ss.Amount * -1 : 0)
                        })
                        .Where(a => a.Net != 0)
                        .OrderByDescending(o => o.IsExclusion).ThenBy(t => t.Name)
                        .ToList();
        }

        public Guid AddUpdateCustomer(Guid id, int userId, string name, string email, string phone, string address, int? cityId, int? countyId, DateTime? birthDate, string notes, bool isExclusion, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new Customer() : customerRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            if (!string.IsNullOrEmpty(email)) {
                if (
                    customerRepository.Query(a => a.Id != id && a.UserId == userId && a.Email == email)
                        .Any()) {
                    errMessage = "duplicate mail";
                    return Guid.Empty;
                }
            }

            entity.UserId = userId;
            entity.Name = name;
            entity.Email = email;
            entity.Phone = phone;
            entity.Address = address;
            entity.CityId = CommonHelper.CheckNullable(cityId);
            entity.CountyId = CommonHelper.CheckNullable(countyId);
            entity.BirthDate = birthDate;
            entity.Notes = notes;
            entity.IsExclusion = isExclusion;

            return customerRepository.AddUpdate(entity);
        }

        public List<CustomerDataModel> GetCustomerList(int userId, string name, string email, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal) {
            Expression<Func<Customer, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (string.IsNullOrEmpty(sort)) {
                sort = "Name";
                sortDescending = false;
            }

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            if (!string.IsNullOrEmpty(email)) {
                query = query.And(a => a.Email.Contains(email));
            }

            // get data
            var list = customerRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .ToListNoLock();

            // get model
            var model = list.Select(ToModel).ToList();

            // toplamlar
            if (model.Any()) {
                debitTotal = model.Sum(s => s.Debit);
                claimTotal = model.Sum(s => s.Claim);
            }
            else {
                debitTotal = 0;
                claimTotal = 0;
            }

            return model;
        }

        public List<SelectGuidDataModel> GetCustomerList(int userId) {
            return customerRepository
                       .Query(a => a.UserId == userId)
                       .OrderBy(a => a.Name)
                       .Select(x => new SelectGuidDataModel { Id = x.Id, Name = x.Name })
                       .ToListNoLock();
        }

        public CustomerDataModel GetCustomerById(Guid id, int userId) {
            // kullanıcının müşterisini getir
            var data = customerRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<Customer, CustomerDataModel>(data);
        }

        public void DeleteCustomer(Guid id, int userId, out string errMessage) {
            // todo: hareket kontrolü
            errMessage = string.Empty;
            var data = customerRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                customerRepository.Delete(id); // kullanıcının müşteri ise, müşteriyi sil
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateCustomerTransaction(Guid id, int userId, Guid customerId, DateTime trnDateTime, string name, int typeId, decimal amount, DateTime? dueDateTime, int? installmentNo, int? installmentTotal, Guid? bankAccountId, string fileName, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new CustomerTransaction() : customerTransactionRepository.Detail(id);
            var newRecord = id == Guid.Empty;

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.CustomerId = customerId;
            entity.TrnDateTime = trnDateTime;
            entity.Name = name;
            entity.TypeId = typeId;
            entity.Amount = amount;
            entity.DueDateTime = dueDateTime;
            entity.InstallmentNo = installmentNo;
            entity.InstallmentTotal = installmentTotal;
            entity.FileName = fileName;

            id = customerTransactionRepository.AddUpdate(entity);

            // yeni kayıt ve taksit varsa
            if (newRecord && installmentTotal > 1) {
                if (installmentNo == null) {
                    entity.InstallmentNo = 1;
                    customerTransactionRepository.AddUpdate(entity);
                }

                if (dueDateTime == null) {
                    entity.DueDateTime = trnDateTime;
                    customerTransactionRepository.AddUpdate(entity);
                }

                for (var i = 1; i < installmentTotal; i++) {
                    var entityInstallment = new CustomerTransaction {
                        //// sabit değerler
                        UserId = entity.UserId,
                        CustomerId = entity.CustomerId,
                        TrnDateTime = entity.TrnDateTime,
                        Name = entity.Name,
                        TypeId = entity.TypeId,
                        Amount = entity.Amount,
                        //// değişen değerler
                        ParentId = id,
                        DueDateTime = Convert.ToDateTime(entity.DueDateTime).AddMonths(i),
                        InstallmentNo = i + 1,
                        InstallmentTotal = entity.InstallmentTotal,
                    };

                    customerTransactionRepository.AddUpdate(entityInstallment);
                }
            }

            // hesap seçildi ise
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.CustomerTransactionId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountId != null && bankAccountId != Guid.Empty) {
                if (bankAccountTransaction == null) {
                    bankAccountTransaction = new BankAccountTransaction { UserId = userId, CustomerTransactionId = id };
                }

                // hesap hareketlerine ters bakiye (borç ise alacak, alacaksa borç)
                if (typeId == (int)CustomerTransactionTypeEnum.Debit) {
                    typeId = (int)BankAccountTransactionTypeEnum.Claim;
                }
                else {
                    typeId = (int)CustomerTransactionTypeEnum.Debit;
                }

                bankAccountTransaction.BankAccountId = bankAccountId.Value;
                bankAccountTransaction.TrnDateTime = trnDateTime;
                bankAccountTransaction.Name = name;
                bankAccountTransaction.TypeId = typeId;
                bankAccountTransaction.Amount = amount;
                bankAccountTransactionRepository.AddUpdate(bankAccountTransaction);
            }
            else {
                if (bankAccountTransaction != null) {
                    bankAccountTransactionRepository.Delete(bankAccountTransaction.Id);
                }
            }

            return id;
        }

        public void UpdateCustomerTransactionFileName(Guid id, string fileName) {
            var entity = customerTransactionRepository.Detail(id);
            if (entity != null) {
                entity.FileName = fileName;

                customerTransactionRepository.AddUpdate(entity);
            }
        }

        public List<CustomerTransactionDataModel> GetCustomerTransactionList(int userId, Guid? customerId, string name, DateTime? startDate, DateTime? endDate, int? excludeId, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "TrnDateTime";
                sortDescending = true;
            }

            Expression<Func<CustomerTransaction, bool>> query = f => !f.IsDeleted && f.UserId == userId;
            if (customerId != null && customerId != Guid.Empty) {
                query = query.And(a => a.CustomerId == customerId);
            }

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            if (startDate != null) {
                query = query.And(a => a.TrnDateTime >= startDate);
            }

            if (endDate != null) {
                query = query.And(a => a.TrnDateTime <= endDate);
            }

            if ((excludeId ?? 0) != 0) {
                query = query.And(a => a.Customer.IsExclusion == (excludeId == 1));
            }

            // get data
            string newSort;
            switch (sort) {
                case "CustomerName":
                    newSort = "Customer.Name " + (sortDescending ? "DESC" : "ASC") + ", TrnDateTime DESC";
                    break;
                case "Name":
                    newSort = "Name " + (sortDescending ? "DESC" : "ASC") + ", TrnDateTime DESC";
                    break;
                default:
                    newSort = "TrnDateTime " + (sortDescending ? "DESC" : "ASC") + ", CreatedDateTime DESC";
                    break;
            }

            const int DebitId = (int)CustomerTransactionTypeEnum.Debit;
            const int ClaimId = (int)CustomerTransactionTypeEnum.Claim;

            var skipCount = page * pageSize;
            var list =
                customerTransactionRepository
                    .Query(query)
                    .Include(i => i.Customer)
                    .OrderBy(newSort)
                    .Skip(skipCount)
                    .Take(pageSize)
                    .Select(s => new CustomerTransactionDataModel {
                        Id = s.Id,
                        CustomerName = s.Customer.Name,
                        TrnDateTime = s.TrnDateTime,
                        Name = s.Name,
                        Debit = s.TypeId == DebitId ? s.Amount : 0,
                        Claim = s.TypeId == ClaimId ? s.Amount : 0,
                        DueDateTime = s.DueDateTime,
                        Installment = s.InstallmentNo > 0 && s.InstallmentTotal > 0 ? s.InstallmentNo.ToString() + "/" + s.InstallmentTotal.ToString() : string.Empty
                    })
                    .ToList();

            // toplamlar
            var queryDebit = customerTransactionRepository.Query(query).Where(a => a.TypeId == DebitId);
            debitTotal = queryDebit.Any() ? queryDebit.Sum(s => s.Amount) : 0;

            var queryClaim = customerTransactionRepository.Query(query).Where(a => a.TypeId == ClaimId);
            claimTotal = queryClaim.Any() ? queryClaim.Sum(s => s.Amount) : 0;

            total = customerTransactionRepository.Query(query).Count();

            return list;
        }

        public CustomerTransactionDataModel GetCustomerTransactionById(Guid id, int userId) {
            var data = customerTransactionRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            var model = AutoMapperConfiguration.Mapper.Map<CustomerTransaction, CustomerTransactionDataModel>(data);

            // hesap hareketi varsa
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.CustomerTransactionId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountTransaction != null) {
                model.BankAccountId = bankAccountTransaction.BankAccountId;
            }

            return model;
        }

        public void DeleteCustomerTransaction(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = customerTransactionRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                customerTransactionRepository.Delete(id);

                // childleri varsa onları da sil
                var childList = customerTransactionRepository.Query(a => a.ParentId == id && a.UserId == userId).ToListNoLock();
                if (childList.Any()) {
                    foreach (var child in childList) {
                        customerTransactionRepository.Delete(child.Id);
                    }
                }

                // ilişkili kasa hareketi varsa onuda sil
                var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.CustomerTransactionId == id && a.UserId == userId).FirstOrDefault();
                if (bankAccountTransaction != null) {
                    bankAccountTransactionRepository.Delete(bankAccountTransaction.Id);
                }
            }
            else {
                errMessage = "record not found";
            }
        }

        public List<CustomerAbstractDataModel> GetCustomerAbstract(int userId, Guid customerId, DateTime? startDate, DateTime? endDate, out string customerName) {
            customerName = string.Empty;
            var customer = customerRepository.Query(q => q.Id == customerId).FirstOrDefault();
            if (customer != null) {
                customerName = customer.Name;
            }

            var list = new List<CustomerAbstractDataModel>();
            var query = customerTransactionRepository.Query(a => a.UserId == userId && a.CustomerId == customerId);

            var recordNo = 1;
            var balance = 0.0M;

            // devir
            if (startDate != null) {
                var prevData = query.Where(a => a.TrnDateTime < startDate).ToListNoLock();
                if (prevData.Any()) {
                    var prevAmount =
                        prevData.Sum(
                            s =>
                            s.TypeId == (int)CustomerTransactionTypeEnum.Debit
                                ? s.Amount
                                : s.Amount * (-1));

                    list.Add(new CustomerAbstractDataModel {
                        TrnDate = startDate.Value,
                        Name = "DEVİR",
                        Amount = prevAmount,
                        Balance = prevAmount,
                    });

                    balance = prevAmount;
                }
            }

            // get data
            if (startDate != null) {
                query = query.Where(a => a.TrnDateTime >= startDate);
            }

            if (endDate != null) {
                query = query.Where(a => a.TrnDateTime <= endDate);
            }

            var filePath = ConfigurationManager.AppSettings["FilePath"];
            var dataList = query.OrderBy(a => a.TrnDateTime).ToListNoLock();
            foreach (var dataItem in dataList) {
                var item = new CustomerAbstractDataModel {
                    No = recordNo,
                    TrnDate = dataItem.TrnDateTime,
                    Name = dataItem.Name,
                    Amount = dataItem.TypeId == (int)CustomerTransactionTypeEnum.Debit ? dataItem.Amount : dataItem.Amount * (-1),
                    FileName = !string.IsNullOrEmpty(dataItem.FileName) ? string.Format("{0}{1}/{2}", filePath, "files", dataItem.FileName) : string.Empty
                };

                balance = item.Balance = balance + item.Amount;
                list.Add(item);
                recordNo++;
            }

            return list;
        }

        private CustomerDataModel ToModel(Customer data) {
            var model = AutoMapperConfiguration.Mapper.Map<Customer, CustomerDataModel>(data);

            // set borç
            const int DebitId = (int)CustomerTransactionTypeEnum.Debit;
            var queryDebit = customerTransactionRepository.Query(a => a.CustomerId == data.Id && a.TypeId == DebitId);
            model.Debit = queryDebit.Any() ? queryDebit.Sum(s => s.Amount) : 0;

            // set alacak
            const int ClaimId = (int)CustomerTransactionTypeEnum.Claim;
            var queryClaim = customerTransactionRepository.Query(a => a.CustomerId == data.Id && a.TypeId == ClaimId);
            model.Claim = queryClaim.Any() ? queryClaim.Sum(s => s.Amount) : 0;

            // set net
            model.Net = model.Debit - model.Claim;

            return model;
        }
    }
}
