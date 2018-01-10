using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using eHesabim.Core;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class ExpenseService : IExpenseService {
        private readonly IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository;
        private readonly IRepository<Expense, Guid> expenseRepository;
        private readonly IRepository<ExpenseGroup, Guid> expenseGroupRepository;
        private readonly IRepository<ExpenseStore, Guid> expenseStoreRepository;
        private readonly IReadOnlyRepository<ViewExpenseGroupReport, int> viewExpenseGroupReportRepository;

        public ExpenseService(IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository, IRepository<Expense, Guid> expenseRepository, IRepository<ExpenseGroup, Guid> expenseGroupRepository, IRepository<ExpenseStore, Guid> expenseStoreRepository, IReadOnlyRepository<ViewExpenseGroupReport, int> viewExpenseGroupReportRepository) {
            this.bankAccountTransactionRepository = bankAccountTransactionRepository;
            this.expenseRepository = expenseRepository;
            this.expenseGroupRepository = expenseGroupRepository;
            this.expenseStoreRepository = expenseStoreRepository;
            this.viewExpenseGroupReportRepository = viewExpenseGroupReportRepository;
        }

        public Guid AddUpdateExpense(Guid id, int userId, int typeId, DateTime expenseDateTime, Guid storeId, string storeName, Guid groupId, string groupName, decimal amount, int? installmentNo, int? installmentTotal, Guid? creditCardId, Guid? creditCardPeriodId, bool isExclusion, string notes, Guid? bankAccountId, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new Expense { UserId = userId, TypeId = typeId } : expenseRepository.Detail(id);
            var newRecord = id == Guid.Empty;

            if (entity == null) {
                errMessage = "record not found.";
                return Guid.Empty;
            }

            if (storeId == Guid.Empty) {
                if (!string.IsNullOrEmpty(storeName)) {
                    var expenseStore = new ExpenseStore { UserId = userId, Name = storeName };
                    storeId = expenseStoreRepository.AddUpdate(expenseStore);
                }
                else {
                    errMessage = "expense store required.";
                    return Guid.Empty;
                }
            }

            if (groupId == Guid.Empty) {
                if (!string.IsNullOrEmpty(groupName)) {
                    var expenseGroup = new ExpenseGroup { UserId = userId, Name = groupName };
                    groupId = expenseGroupRepository.AddUpdate(expenseGroup);
                }
                else {
                    errMessage = "expense group required.";
                    return Guid.Empty;
                }
            }

            entity.ExpenseDateTime = expenseDateTime;
            entity.ExpenseStoreId = storeId;
            entity.ExpenseGroupId = groupId;
            entity.Amount = amount;
            entity.InstallmentNo = installmentNo;
            entity.InstallmentTotal = installmentTotal;
            entity.BankCreditCardId = CommonHelper.CheckNullable(creditCardId);
            entity.BankCreditCardPeriodId = CommonHelper.CheckNullable(creditCardPeriodId);
            entity.IsExclusion = isExclusion;
            entity.Notes = notes;
            id = expenseRepository.AddUpdate(entity);

            // yeni kayıt ve taksit varsa
            if (newRecord && installmentTotal > 1) {
                if (installmentNo == null) {
                    entity.InstallmentNo = 1;
                    expenseRepository.AddUpdate(entity);
                }

                for (var i = 1; i < installmentTotal; i++) {
                    var entityInstallment = new Expense {
                        //// sabit değerler
                        UserId = entity.UserId,
                        TypeId = entity.TypeId,
                        ExpenseStoreId = entity.ExpenseStoreId,
                        ExpenseGroupId = entity.ExpenseGroupId,
                        Amount = entity.Amount,
                        IsExclusion = entity.IsExclusion,
                        Notes = entity.Notes,
                        BankCreditCardId = entity.BankCreditCardId,
                        //// null değerler
                        BankCreditCardPeriodId = null,
                        //// değişen değerler
                        ParentId = id,
                        ExpenseDateTime = Convert.ToDateTime(entity.ExpenseDateTime).AddMonths(i),
                        InstallmentNo = i + 1,
                        InstallmentTotal = entity.InstallmentTotal,
                    };

                    expenseRepository.AddUpdate(entityInstallment);
                }
            }

            // hesap seçildi ise
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.ExpenseId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountId != null && bankAccountId != Guid.Empty) {
                if (bankAccountTransaction == null) {
                    bankAccountTransaction = new BankAccountTransaction { UserId = userId, ExpenseId = id };
                }

                bankAccountTransaction.BankAccountId = bankAccountId.Value;
                bankAccountTransaction.TrnDateTime = expenseDateTime;
                bankAccountTransaction.Name = notes;
                bankAccountTransaction.TypeId = (int)BankAccountTransactionTypeEnum.Claim;
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

        public List<ExpenseDataModel> GetExpenseList(int userId, Guid? creditCardId, Guid? creditCardPeriodId, int typeId, Guid? storeId, Guid? groupId, DateTime? startDate, DateTime? endDate, int? nextPeriodId, int? excludeId, string notes, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal expenseMe, out decimal expenseExclusion) {
            Expression<Func<Expense, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (sort == "Installment") {
                sort = "InstallmentNo";
            }

            // filter
            if ((creditCardId ?? Guid.Empty) != Guid.Empty) {
                query = query.And(a => a.BankCreditCardId == creditCardId);
            }

            if ((creditCardPeriodId ?? Guid.Empty) != Guid.Empty) {
                query = query.And(a => a.BankCreditCardPeriodId == creditCardPeriodId);
            }

            if (typeId > 0) {
                query = query.And(a => a.TypeId == typeId);
            }

            if ((storeId ?? Guid.Empty) != Guid.Empty) {
                query = query.And(a => a.ExpenseStoreId == storeId);
            }

            if ((groupId ?? Guid.Empty) != Guid.Empty) {
                query = query.And(a => a.ExpenseGroupId == groupId);
            }

            if (startDate != null) {
                query = query.And(a => a.ExpenseDateTime >= startDate);
            }

            if (endDate != null) {
                query = query.And(a => a.ExpenseDateTime <= endDate);
            }

            if ((nextPeriodId ?? 0) != 0) {
                query = nextPeriodId == 1
                    ? query.And(a => a.BankCreditCardId != null && a.BankCreditCardPeriodId == null)
                    : query.And(a => a.BankCreditCardId != null && a.BankCreditCardPeriodId != null);
            }

            if ((excludeId ?? 0) != 0) {
                query = query.And(a => a.IsExclusion == (excludeId == 1));
            }

            if (!string.IsNullOrEmpty(notes)) {
                query = query.And(a => a.Notes.Contains(notes));
            }

            // get data
            var list = expenseRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.Type)
                        .Include(i => i.ExpenseStore)
                        .Include(i => i.ExpenseGroup)
                        .Include(i => i.BankCreditCard).DefaultIfEmpty()
                        .Include(i => i.BankCreditCardPeriod).DefaultIfEmpty()
                        .Where(a => a != null)
                        .ToListNoLock();

            // toplamlar
            var queryMe = expenseRepository.Query(query).Where(a => !a.IsExclusion);
            expenseMe = queryMe.Any() ? queryMe.Sum(s => s.Amount) : 0;

            var queryExclusion = expenseRepository.Query(query).Where(a => a.IsExclusion);
            expenseExclusion = queryExclusion.Any() ? queryExclusion.Sum(s => s.Amount) : 0;

            // get model
            return list.Select(ToModel).ToList();
        }

        public ExpenseDataModel GetExpenseById(Guid id, int userId) {
            var data =
                expenseRepository
                    .Query(a => a.Id == id && a.UserId == userId)
                    .Include(i => i.Type)
                    .Include(i => i.ExpenseStore)
                    .Include(i => i.ExpenseGroup).Include(i => i.BankCreditSub)
                    .DefaultIfEmpty().Include(i => i.BankCreditSub.BankCredit)
                    .DefaultIfEmpty().Include(i => i.BankCreditSub.BankCredit.Bank)
                    .DefaultIfEmpty()
                    .FirstOrDefault(a => a != null);

            var model = AutoMapperConfiguration.Mapper.Map<Expense, ExpenseDataModel>(data);

            // hesap hareketi varsa
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.ExpenseId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountTransaction != null) {
                model.BankAccountId = bankAccountTransaction.BankAccountId;
            }

            return model;
        }

        public void DeleteExpense(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = expenseRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                expenseRepository.Delete(id);

                // childleri varsa onları da sil
                var childList = expenseRepository.Query(a => a.ParentId == id && a.UserId == userId).ToListNoLock();
                if (childList.Any()) {
                    foreach (var child in childList) {
                        expenseRepository.Delete(child.Id);
                    }
                }
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateExpenseGroup(Guid id, int userId, string name, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new ExpenseGroup() : expenseGroupRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            //// todo : check duplicate name

            entity.UserId = userId;
            entity.Name = name;

            return expenseGroupRepository.AddUpdate(entity);
        }

        public List<ExpenseGroupDataModel> GetExpenseGroupList(int userId, string name, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "Name";
                sortDescending = false;
            }

            Expression<Func<ExpenseGroup, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            // get data
            var list = expenseGroupRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .ToListNoLock();

            return AutoMapperConfiguration.Mapper.Map<List<ExpenseGroup>, List<ExpenseGroupDataModel>>(list);
        }

        public List<SelectGuidDataModel> GetExpenseGroupList(int userId) {
            return expenseGroupRepository
                       .Query(a => a.UserId == userId)
                       .OrderBy(a => a.Name)
                       .Select(x => new SelectGuidDataModel { Id = x.Id, Name = x.Name })
                       .ToListNoLock();
        }

        public ExpenseGroupDataModel GetExpenseGroupById(Guid id, int userId) {
            var data = expenseGroupRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<ExpenseGroup, ExpenseGroupDataModel>(data);
        }

        public void DeleteExpenseGroup(Guid id, int userId, out string errMessage) {
            //// todo : check expense
            errMessage = string.Empty;
            var data = expenseGroupRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                expenseGroupRepository.Delete(id);
            }
            else {
                errMessage = "record not found";
            }
        }

        public List<int> GetExpenseYearList(int userId) {
            var list =
                expenseRepository
                    .Query(a => a.UserId == userId)
                    .GroupBy(g => g.ExpenseDateTime.Year)
                    .Select(s => s.Key)
                    .OrderByDescending(o => o)
                    .ToListNoLock();

            if (!list.Contains(DateTime.Today.Year)) {
                list.Insert(0, DateTime.Today.Year);
            }

            return list;
        }

        public List<ViewExpenseGroupReportDataModel> GetExpenseGroupReport(int userId, int year) {
            var list = viewExpenseGroupReportRepository
                        .Query(a => a.UserId == userId && a.GroupYear == year)
                        .OrderBy(a => a.GroupName)
                        .ToList();

            return AutoMapperConfiguration.Mapper.Map<List<ViewExpenseGroupReport>, List<ViewExpenseGroupReportDataModel>>(list);
        }

        public Guid AddUpdateExpenseStore(Guid id, int userId, string name, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new ExpenseStore() : expenseStoreRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            //// todo : check duplicate name

            entity.UserId = userId;
            entity.Name = name;

            return expenseStoreRepository.AddUpdate(entity);
        }

        public List<ExpenseStoreDataModel> GetExpenseStoreList(int userId, string name, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "Name";
                sortDescending = false;
            }

            Expression<Func<ExpenseStore, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            // get data
            var list = expenseStoreRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .ToListNoLock();

            return AutoMapperConfiguration.Mapper.Map<List<ExpenseStore>, List<ExpenseStoreDataModel>>(list);
        }

        public List<SelectGuidDataModel> GetExpenseStoreList(int userId) {
            return expenseStoreRepository
                        .Query(a => a.UserId == userId)
                        .OrderBy(a => a.Name)
                        .Select(x => new SelectGuidDataModel { Id = x.Id, Name = x.Name })
                        .ToListNoLock();
        }

        public ExpenseStoreDataModel GetExpenseStoreById(Guid id, int userId) {
            var data = expenseStoreRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<ExpenseStore, ExpenseStoreDataModel>(data);
        }

        public void DeleteExpenseStore(Guid id, int userId, out string errMessage) {
            //// todo : check expense
            errMessage = string.Empty;
            var data = expenseStoreRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                expenseStoreRepository.Delete(id);
            }
            else {
                errMessage = "record not found";
            }
        }

        private ExpenseDataModel ToModel(Expense data) {
            var model = AutoMapperConfiguration.Mapper.Map<Expense, ExpenseDataModel>(data);

            model.TotalAmount = data.ParentId.HasValue
                ? expenseRepository.Query(f => f.Id == data.ParentId || f.ParentId == data.ParentId).ToListNoLock().Sum(s => s.Amount)
                : expenseRepository.Query(f => f.Id == data.Id || f.ParentId == data.Id).ToListNoLock().Sum(s => s.Amount);

            return model;
        }
    }
}
