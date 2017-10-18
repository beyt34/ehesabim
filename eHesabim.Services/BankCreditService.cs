using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class BankCreditService : IBankCreditService {
        private readonly IRepository<BankCredit, Guid> bankCreditRepository;
        private readonly IRepository<BankCreditSub, Guid> bankCreditSubRepository;
        private readonly IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository;
        private readonly IRepository<Expense, Guid> expenseRepository;
        private readonly IRepository<ExpenseStore, Guid> expenseStoreRepository;

        public BankCreditService(IRepository<BankCredit, Guid> bankCreditRepository, IRepository<BankCreditSub, Guid> bankCreditSubRepository, IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository, IRepository<Expense, Guid> expenseRepository, IRepository<ExpenseStore, Guid> expenseStoreRepository) {
            this.bankCreditRepository = bankCreditRepository;
            this.bankCreditSubRepository = bankCreditSubRepository;
            this.bankAccountTransactionRepository = bankAccountTransactionRepository;
            this.expenseRepository = expenseRepository;
            this.expenseStoreRepository = expenseStoreRepository;
        }

        public Guid AddUpdateBankCredit(Guid id, int userId, int bankId, DateTime creditDateTime, decimal capital, decimal rate, int installment, decimal monthlyPayment, decimal expense, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankCredit() : bankCreditRepository.Detail(id);
            var newRecord = id == Guid.Empty;

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.BankId = bankId;
            entity.CreditDateTime = creditDateTime;
            entity.Capital = capital;
            entity.Rate = rate;
            entity.Installment = installment;
            entity.MonthlyPayment = monthlyPayment;
            entity.Expense = expense;

            id = bankCreditRepository.AddUpdate(entity);

            // yeni kayıt ise
            if (newRecord) {
                for (var i = 0; i < installment; i++) {
                    // todo: calculate capital, interest
                    var entitySub = new BankCreditSub {
                        UserId = userId,
                        BankCreditId = id,
                        Installment = i + 1,
                        InstallmentDateTime = creditDateTime.AddMonths(i + 1),
                        InstallmentAmount = monthlyPayment
                    };

                    bankCreditSubRepository.AddUpdate(entitySub);
                }
            }

            return id;
        }

        public List<BankCreditDataModel> GetBankCreditList(int userId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "CreditDateTime";
                sortDescending = true;
            }

            Expression<Func<BankCredit, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (bankId != 0) {
                query = query.And(a => a.BankId == bankId);
            }

            // get data
            var list = bankCreditRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.Bank)
                        .ToListNoLock();

            return GetBankCreditDataModelList(list);
        }

        public List<SelectGuidDataModel> GetBankCreditList(int userId) {
            var list =
                bankCreditRepository
                    .Query(a => a.UserId == userId && !a.IsDeleted)
                    .OrderByDescending(a => a.CreditDateTime)
                    .Include(i => i.Bank)
                    .ToListNoLock();

            return AutoMapperConfiguration.Mapper.Map<List<BankCredit>, List<SelectGuidDataModel>>(list);
        }

        public BankCreditDataModel GetBankCreditById(Guid id, int userId) {
            var data = bankCreditRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            return AutoMapperConfiguration.Mapper.Map<BankCredit, BankCreditDataModel>(data);
        }

        public void DeleteBankCredit(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = bankCreditRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankCreditRepository.Delete(id);

                // childleri varsa onları da sil
                var childList = bankCreditSubRepository.Query(a => a.BankCreditId == id && a.UserId == userId).ToListNoLock();
                if (childList.Any()) {
                    foreach (var child in childList) {
                        bankCreditSubRepository.Delete(child.Id);
                    }
                }
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateBankCreditSub(Guid id, int userId, Guid bankCreditId, int installment, DateTime installmentDate, DateTime? paymentDate, decimal installmentAmount, decimal capitalAmount, decimal interestAmount, Guid? bankAccountId, Guid? expenseGroupId, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankCreditSub() : bankCreditSubRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.BankCreditId = bankCreditId;
            entity.Installment = installment;
            entity.InstallmentDateTime = installmentDate;
            entity.PaymentDateTime = paymentDate;
            entity.InstallmentAmount = installmentAmount;
            entity.CapitalAmount = capitalAmount;
            entity.InterestAmount = interestAmount;

            id = bankCreditSubRepository.AddUpdate(entity);

            // hesap seçildi ise
            AddBankAccountTransaction(id, userId, paymentDate, installmentAmount, bankAccountId);

            // gider grubu seçildi ise
            AddExpense(id, userId, paymentDate, interestAmount, expenseGroupId);

            return id;
        }

        public List<BankCreditSubDataModel> GetBankCreditSubList(int userId, Guid? bankCreditId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "InstallmentDateTime";
                sortDescending = true;
            }

            Expression<Func<BankCreditSub, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if ((bankCreditId ?? Guid.Empty) != Guid.Empty) {
                query = query.And(a => a.BankCreditId == bankCreditId);
            }

            // get data
            var list = bankCreditSubRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.BankCredit)
                        .Include(i => i.BankCredit.Bank)
                        .ToListNoLock();

            var retList = AutoMapperConfiguration.Mapper.Map<List<BankCreditSub>, List<BankCreditSubDataModel>>(list);

            var creditIds = list.Select(s => s.BankCreditId).Distinct().ToList();
            if (creditIds.Any()) {
                var allSubs =
                    bankCreditSubRepository
                        .Query(q => creditIds.Contains(q.BankCreditId) && !q.IsDeleted)
                        .Include(i => i.BankCredit)
                        .Select(s => new {
                            BankCreditSubBankCreditId = s.BankCreditId,
                            BankCreditCapital = s.BankCredit.Capital,
                            BankCreditSubInstallmentDateTime = s.InstallmentDateTime,
                            BankCreditSubCapitalAmount = s.CapitalAmount
                        })
                        .ToList();

                if (allSubs.Any()) {
                    foreach (var item in retList) {
                        var itemAll = allSubs.Where(a => a.BankCreditSubBankCreditId == item.BankCreditId).ToList();
                        var capital = itemAll.FirstOrDefault().BankCreditCapital;
                        var beforeCapital = itemAll.Where(a => a.BankCreditSubInstallmentDateTime <= item.InstallmentDateTime).Sum(s => s.BankCreditSubCapitalAmount);

                        item.RemainCapital = capital - beforeCapital;
                    }
                }
            }

            return retList;
        }

        public BankCreditSubDataModel GetBankCreditSubById(Guid id, int userId) {
            var data = bankCreditSubRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            var model = AutoMapperConfiguration.Mapper.Map<BankCreditSub, BankCreditSubDataModel>(data);

            // hesap hareketi varsa
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditSubId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountTransaction != null) {
                model.BankAccountId = bankAccountTransaction.BankAccountId;
            }

            // harcama hareketi varsa
            var expense = expenseRepository.Query(a => a.BankCreditSubId == id && a.UserId == userId).FirstOrDefault();
            if (expense != null) {
                model.ExpenseGroupId = expense.ExpenseGroupId;
            }

            return model;
        }

        public void DeleteBankCreditSub(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = bankCreditSubRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankCreditRepository.Delete(id);

                // ilişkili kasa hareketi varsa onuda sil
                var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditSubId == id && a.UserId == userId).FirstOrDefault();
                if (bankAccountTransaction != null) {
                    bankAccountTransactionRepository.Delete(bankAccountTransaction.Id);
                }
            }
            else {
                errMessage = "record not found";
            }
        }

        public List<BankCreditDataModel> GetDashboardBankCreditList(int userId) {
            int total;
            var list = GetBankCreditList(userId, 0, string.Empty, false, 0, 100, out total);
            return list.Where(a => a.RemainCapital > 0).ToList();
        }

        private List<BankCreditDataModel> GetBankCreditDataModelList(List<BankCredit> list) {
            var model = AutoMapperConfiguration.Mapper.Map<List<BankCredit>, List<BankCreditDataModel>>(list);
            foreach (var item in model) {
                var item1 = item;
                var subList = bankCreditSubRepository.Query(a => a.BankCreditId == item1.Id).OrderBy(a => a.InstallmentDateTime).ToListNoLock();
                if (subList.Any()) {
                    item.RemainCapital = item.Capital;
                    item.RemainInstallment = item.Installment;

                    var subListPayed = subList.Where(a => a.PaymentDateTime.HasValue).ToList();
                    if (subListPayed.Any()) {
                        item.RemainCapital -= subListPayed.Sum(s => s.CapitalAmount);
                        item.RemainInstallment -= subListPayed.Count;
                    }

                    var firstNotPayed = subList.Where(a => !a.PaymentDateTime.HasValue).OrderBy(a => a.InstallmentDateTime).FirstOrDefault();
                    if (firstNotPayed != null) {
                        item.PaymentDateTime = firstNotPayed.InstallmentDateTime;
                    }
                }
            }

            return model;
        }

        private void AddBankAccountTransaction(Guid bankCreditSubId, int userId, DateTime? paymentDate, decimal installmentAmount, Guid? bankAccountId) {
            // get account transaction
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditSubId == bankCreditSubId && a.UserId == userId).FirstOrDefault();

            // hesap seçildi ise
            if (bankAccountId != null && bankAccountId != Guid.Empty && paymentDate.HasValue) {
                // add
                if (bankAccountTransaction == null) {
                    // fix değerler
                    bankAccountTransaction = new BankAccountTransaction {
                        UserId = userId,
                        Name = "KREDİ ÖDEMESİ",
                        TypeId = (int)BankAccountTransactionTypeEnum.Claim,
                        BankCreditSubId = bankCreditSubId
                    };
                }

                // değişen değerler
                bankAccountTransaction.BankAccountId = bankAccountId.Value;
                bankAccountTransaction.TrnDateTime = paymentDate.Value;
                bankAccountTransaction.Amount = installmentAmount;
                bankAccountTransactionRepository.AddUpdate(bankAccountTransaction);
            }
            else {
                // delete
                if (bankAccountTransaction != null) {
                    bankAccountTransactionRepository.Delete(bankAccountTransaction.Id);
                }
            }
        }

        private void AddExpense(Guid bankCreditSubId, int userId, DateTime? paymentDate, decimal interestAmount, Guid? expenseGroupId) {
            // get expense
            var expense = expenseRepository.Query(a => a.BankCreditSubId == bankCreditSubId && a.UserId == userId).FirstOrDefault();

            // gider grubu seçildi ise
            if (expenseGroupId != null && expenseGroupId != Guid.Empty && paymentDate.HasValue) {
                // add
                if (expense == null) {
                    // fix değerler
                    expense = new Expense {
                        UserId = userId,
                        TypeId = (int)ExpenseTypeEnum.Cash,
                        Notes = "KREDİ FAİZİ",
                        BankCreditSubId = bankCreditSubId
                    };
                }

                // değişen değerler
                var storeId = GetExpenseStoreId(bankCreditSubId, userId);
                expense.ExpenseStoreId = storeId;
                expense.ExpenseGroupId = expenseGroupId.Value;
                expense.ExpenseDateTime = paymentDate.Value;
                expense.Amount = interestAmount;
                expenseRepository.AddUpdate(expense);
            }
            else {
                // delete
                if (expense != null) {
                    expenseRepository.Delete(expense.Id);
                }
            }
        }

        private Guid GetExpenseStoreId(Guid bankCreditSubId, int userId) {
            var storeId = new Guid();
            var bankCreditSub =
                bankCreditSubRepository.Query(a => a.Id == bankCreditSubId)
                    .Include(i => i.BankCredit)
                    .Include(i => i.BankCredit.Bank)
                    .FirstOrDefault();

            // data
            if (bankCreditSub != null && bankCreditSub.BankCredit != null && bankCreditSub.BankCredit.Bank != null) {
                var storeName = bankCreditSub.BankCredit.Bank.Name;
                var store = expenseStoreRepository.Query(a => a.Name == storeName && a.UserId == userId && !a.IsDeleted).FirstOrDefault();
                if (store != null) {
                    storeId = store.Id;
                }
                else {
                    var expenseStore = new ExpenseStore { UserId = userId, Name = storeName };
                    storeId = expenseStoreRepository.AddUpdate(expenseStore);
                }
            }

            return storeId;
        }
    }
}
