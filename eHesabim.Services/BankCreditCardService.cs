using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using eHesabim.Core.Data;
using eHesabim.Data.Domain;
using eHesabim.Services.Models;

namespace eHesabim.Services {
    public class BankCreditCardService : IBankCreditCardService {
        private readonly IRepository<BankCreditCard, Guid> bankCreditCardRepository;
        private readonly IRepository<BankCreditCardPeriod, Guid> bankCreditCardPeriodRepository;
        private readonly IRepository<BankCreditCardPayment, Guid> bankCreditCardPaymentRepository;
        private readonly IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository;
        private readonly IRepository<Expense, Guid> expenseRepository;

        public BankCreditCardService(IRepository<BankCreditCard, Guid> bankCreditCardRepository, IRepository<BankCreditCardPeriod, Guid> bankCreditCardPeriodRepository, IRepository<BankCreditCardPayment, Guid> bankCreditCardPaymentRepository, IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository, IRepository<Expense, Guid> expenseRepository) {
            this.bankCreditCardRepository = bankCreditCardRepository;
            this.bankCreditCardPeriodRepository = bankCreditCardPeriodRepository;
            this.bankCreditCardPaymentRepository = bankCreditCardPaymentRepository;
            this.bankAccountTransactionRepository = bankAccountTransactionRepository;
            this.expenseRepository = expenseRepository;
        }

        public Guid AddUpdateBankCreditCard(Guid id, int userId, Guid? parentId, int bankId, string name, string cardNo, decimal limit, string expiredMonth, string expiredYear, int? order, bool isActive, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankCreditCard() : bankCreditCardRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.ParentId = parentId;
            entity.BankId = bankId;
            entity.Name = name;
            entity.CardNo = cardNo;
            entity.Limit = limit;
            entity.ExpiredMonth = expiredMonth;
            entity.ExpiredYear = expiredYear;
            entity.Order = order;
            entity.IsActive = isActive;

            return bankCreditCardRepository.AddUpdate(entity);
        }

        public List<BankCreditCardDataModel> GetBankCreditCardList(int userId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "Name";
                sortDescending = false;
            }

            Expression<Func<BankCreditCard, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (bankId != 0) {
                query = query.And(a => a.BankId == bankId);
            }

            // get data
            var list = bankCreditCardRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.Bank)
                        .ToListNoLock();

            // automap model
            var model = AutoMapperConfiguration.Mapper.Map<List<BankCreditCard>, List<BankCreditCardDataModel>>(list);

            // calculate totals
            for (var i = 0; i < model.Count; i++) {
                model[i] = GetCalculatedModel(model[i]);
            }

            return model;
        }

        public List<SelectGuidDataModel> GetBankCreditCardList(int userId, Guid? hideBankCreditCardId, Guid? showBankCreditCardId) {
            return bankCreditCardRepository
                    .Query(a => a.UserId == userId && a.Id != hideBankCreditCardId && (a.Id == showBankCreditCardId || a.IsActive))
                    .OrderBy(a => a.Order)
                    .ThenBy(a => a.Name)
                    .Select(x => new SelectGuidDataModel { Id = x.Id, Name = x.Name })
                    .ToListNoLock();
        }

        public BankCreditCardDataModel GetBankCreditCardById(Guid id, int userId) {
            var data = bankCreditCardRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                var model = AutoMapperConfiguration.Mapper.Map<BankCreditCard, BankCreditCardDataModel>(data);
                return model;
            }

            return new BankCreditCardDataModel();
        }

        public void DeleteBankCreditCard(Guid id, int userId, out string errMessage) {
            // todo: hareket kontrolü
            errMessage = string.Empty;
            var data = bankCreditCardRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankCreditCardRepository.Delete(id);
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateBankCreditCardPeriod(Guid id, int userId, Guid creditCardId, DateTime startDate, DateTime endDate, DateTime paymentDate, bool setExpense, out string errMessage) {
            // todo: start/end date validation
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankCreditCardPeriod() : bankCreditCardPeriodRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.BankCreditCardId = creditCardId;
            entity.StartDate = startDate;
            entity.EndDate = endDate;
            entity.PaymentDate = paymentDate;

            id = bankCreditCardPeriodRepository.AddUpdate(entity);
            if (setExpense) {
                SetExpensePeriod(id, userId);
            }

            return id;
        }

        public List<BankCreditCardPeriodDataModel> GetBankCreditCardPeriodList(int userId, Guid? creditCardId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "PaymentDate";
                sortDescending = true;
            }

            Expression<Func<BankCreditCardPeriod, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (creditCardId.HasValue && creditCardId != Guid.Empty) {
                query = query.And(a => a.BankCreditCardId == creditCardId);
            }

            // get data
            var list = bankCreditCardPeriodRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.BankCreditCard)
                        .ToListNoLock();

            var model = AutoMapperConfiguration.Mapper.Map<List<BankCreditCardPeriod>, List<BankCreditCardPeriodDataModel>>(list);
            return model;
        }

        public List<SelectGuidDataModel> GetBankCreditCardPeriodList(int userId, Guid creditCardId) {
            return bankCreditCardPeriodRepository
                      .Query(a => a.UserId == userId && a.BankCreditCardId == creditCardId)
                      .OrderByDescending(a => a.EndDate)
                      .ToListNoLock()
                      .Select(x => new SelectGuidDataModel { Id = x.Id, Name = GetBankCreditCardPeriodText(x.StartDate, x.EndDate) })
                      .ToList();
        }

        public BankCreditCardPeriodDataModel GetBankCreditCardPeriodById(Guid id, int userId) {
            var data = bankCreditCardPeriodRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                var model = AutoMapperConfiguration.Mapper.Map<BankCreditCardPeriod, BankCreditCardPeriodDataModel>(data);
                return model;
            }

            return new BankCreditCardPeriodDataModel();
        }

        public void DeleteBankCreditCardPeriod(Guid id, int userId, out string errMessage) {
            // todo: hareket kontrolü
            errMessage = string.Empty;
            var data = bankCreditCardPeriodRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankCreditCardPeriodRepository.Delete(id);
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateBankCreditCardPayment(Guid id, int userId, Guid creditCardId, DateTime paymentDateTime, decimal amount, bool hasPoint, Guid? bankAccountId, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankCreditCardPayment() : bankCreditCardPaymentRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.BankCreditCardId = creditCardId;
            entity.PaymentDateTime = paymentDateTime;
            entity.Amount = amount;
            entity.HasPoint = hasPoint;

            id = bankCreditCardPaymentRepository.AddUpdate(entity);

            // hesap seçildi ise
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditCardPaymentId == id && a.UserId == userId).FirstOrDefault();
            if (bankAccountId != null && bankAccountId != Guid.Empty) {
                if (bankAccountTransaction == null) {
                    // fix değerler
                    bankAccountTransaction = new BankAccountTransaction {
                        UserId = userId,
                        Name = "K.KARTI ÖDEMESİ",
                        TypeId = (int)BankAccountTransactionTypeEnum.Claim,
                        BankCreditCardPaymentId = id
                    };
                }

                // değişen değerler
                bankAccountTransaction.BankAccountId = bankAccountId.Value;
                bankAccountTransaction.TrnDateTime = paymentDateTime;
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

        public List<BankCreditCardPaymentDataModel> GetBankCreditCardPaymentList(int userId, Guid? creditCardId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "PaymentDateTime";
                sortDescending = true;
            }

            Expression<Func<BankCreditCardPayment, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (creditCardId.HasValue && creditCardId != Guid.Empty) {
                query = query.And(a => a.BankCreditCardId == creditCardId);
            }

            // get data
            var list = bankCreditCardPaymentRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.BankCreditCard)
                ////.Include(i => i.BankAccountTransactions).DefaultIfEmpty()
                ////.Include(i => i.BankAccountTransactions.Select(s => s.BankAccount)).DefaultIfEmpty()
                        .ToListNoLock();

            var model = AutoMapperConfiguration.Mapper.Map<List<BankCreditCardPayment>, List<BankCreditCardPaymentDataModel>>(list);
            return model;
        }

        public BankCreditCardPaymentDataModel GetBankCreditCardPaymentById(Guid id, int userId) {
            var data = bankCreditCardPaymentRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                var model = AutoMapperConfiguration.Mapper.Map<BankCreditCardPayment, BankCreditCardPaymentDataModel>(data);

                // hesap hareketi varsa
                var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditCardPaymentId == id && a.UserId == userId).FirstOrDefault();
                if (bankAccountTransaction != null) {
                    model.BankAccountId = bankAccountTransaction.BankAccountId;
                }

                return model;
            }

            return new BankCreditCardPaymentDataModel();
        }

        public void DeleteBankCreditCardPayment(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = bankCreditCardPaymentRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankCreditCardPaymentRepository.Delete(id);

                // ilişkili kasa hareketi varsa onuda sil
                var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.BankCreditCardPaymentId == id && a.UserId == userId).FirstOrDefault();
                if (bankAccountTransaction != null) {
                    bankAccountTransactionRepository.Delete(bankAccountTransaction.Id);
                }
            }
            else {
                errMessage = "record not found";
            }
        }

        public List<BankCreditCardDataModel> GetDashboardBankCreditCardList(int userId) {
            // get data
            var list = bankCreditCardRepository
                        .Query(a => a.UserId == userId)
                        .Include(i => i.Bank)
                        .OrderBy(a => a.Order)
                        .ThenBy(a => a.Name)
                        .ToListNoLock();

            // automap model
            var model = AutoMapperConfiguration.Mapper.Map<List<BankCreditCard>, List<BankCreditCardDataModel>>(list);

            // calculate totals
            for (var i = 0; i < model.Count; i++) {
                model[i] = GetCalculatedModel(model[i]);
            }

            // set parent/child card-limit & available limit
            foreach (var childCard in model.Where(a => a.ParentId != null)) {
                var parentCard = model.FirstOrDefault(a => a.Id == childCard.ParentId);
                if (parentCard != null) {
                    parentCard.Limit += childCard.Limit;
                    parentCard.AvailableLimit += childCard.AvailableLimit;

                    childCard.Limit = 0;
                    childCard.AvailableLimit = 0;
                }
            }

            return model.Where(a => a.DebtTotal != 0).ToList();
        }

        public int GetBankId(Guid? bankCreditCardId) {
            var data = bankCreditCardRepository.Query(q => q.Id == bankCreditCardId).FirstOrDefault();
            return data != null ? data.BankId : 0;
        }

        public void SetExpensePeriod(Guid id, int userId) {
            var period = bankCreditCardPeriodRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (period != null) {
                var expenseList = expenseRepository
                    .Query(a => a.BankCreditCardId == period.BankCreditCardId &&
                                a.BankCreditCardPeriodId == null &&
                                a.ExpenseDateTime >= period.StartDate &&
                                a.ExpenseDateTime <= period.EndDate).ToList();

                foreach (var expense in expenseList) {
                    expense.BankCreditCardPeriodId = period.Id;
                    expenseRepository.AddUpdate(expense);
                }
            }
        }

        private string GetBankCreditCardPeriodText(DateTime startDate, DateTime endDate) {
            return string.Format(
                    "{0}/{1} » ({2} - {3})",
                    endDate.ToString("yyyy"),
                    endDate.ToString("MM"),
                    startDate.ToString("dd.MM.yyyy"),
                    endDate.ToString("dd.MM.yyyy"));
        }

        private BankCreditCardDataModel GetCalculatedModel(BankCreditCardDataModel item) {
            // calculate totals
            var expenseTotal = expenseRepository
                                .Query(a => a.BankCreditCardId == item.Id)
                                .ToListNoLock()
                                .Sum(a => a.Amount);

            var paymentTotal = bankCreditCardPaymentRepository
                                .Query(a => a.BankCreditCardId == item.Id)
                                .ToListNoLock()
                                .Sum(s => s.Amount);

            item.DebtTotal = expenseTotal - paymentTotal;
            item.AvailableLimit = item.Limit - item.DebtTotal;

            DateTime lastPaymentDate;
            DateTime lastEndDate;
            DateTime currentPaymentDate;
            DateTime currentEndDate;
            Guid currentPeriodId;

            item.LastDebt = GetLastDebt(item.Id, out lastPaymentDate, out lastEndDate);                                     // ekstre borcu
            item.CurrentDebt = GetCurrentDebt(item.Id, out currentPaymentDate, out currentEndDate, out currentPeriodId);    // dönem içi
            item.NextDebt = GetNextDebt(item.Id);                                                                           // sonraki dönem

            // toplam borç 0 ise, tüm dönem borçları 0
            if (item.DebtTotal == 0) {
                item.LastDebt = 0;
                item.CurrentDebt = 0;
                item.NextDebt = 0;
            }
            else {
                // toplam borç, gelecek dönemden fazla ise
                if (item.DebtTotal > item.NextDebt) {
                    var tmpCurrent = item.DebtTotal - item.NextDebt;

                    // donemicinde peşin iade varsa, ekstre borcundan düşülecek, donemici borca eklenecek
                    var currentRefund =
                        expenseRepository
                           .Query(a => a.BankCreditCardId == item.Id && a.BankCreditCardPeriodId == currentPeriodId && a.ParentId == null && a.Amount < 0)
                           .ToListNoLock()
                           .Sum(a => a.Amount);

                    if (currentRefund < 0) {
                        currentRefund = currentRefund * -1; // - işlem, tersine çevir
                        item.LastDebt = item.LastDebt - currentRefund;
                        item.CurrentDebt = item.CurrentDebt + currentRefund;
                    }

                    // ekstre ve donemici borc hesaplama
                    if (tmpCurrent > item.CurrentDebt) {
                        item.LastDebt = tmpCurrent - item.CurrentDebt;
                        item.EndDateTime = lastEndDate;
                        item.PaymentDateTime = lastPaymentDate;
                    }
                    else {
                        item.LastDebt = 0;
                        item.CurrentDebt = tmpCurrent;
                        item.EndDateTime = currentEndDate;
                        item.PaymentDateTime = currentPaymentDate;
                    }
                }
                else { // değilse, toplam borç kadar gelecek dönem borç var, öncesi 0
                    item.LastDebt = 0;
                    item.CurrentDebt = 0;
                    item.NextDebt = item.DebtTotal;
                }
            }

            return item;
        }

        private decimal GetLastDebt(Guid creditCardId, out DateTime lastPaymentDate, out DateTime lastEndDate) {
            var lastPeriod =
                bankCreditCardPeriodRepository
                    .Query(a => a.BankCreditCardId == creditCardId && a.EndDate < DateTime.Today)
                    .OrderByDescending(a => a.EndDate)
                    .FirstOrDefault();

            if (lastPeriod != null) {
                lastPaymentDate = lastPeriod.PaymentDate;
                lastEndDate = lastPeriod.EndDate;

                return expenseRepository
                        .Query(a => a.BankCreditCardId == creditCardId && a.BankCreditCardPeriodId == lastPeriod.Id)
                        .ToListNoLock()
                        .Sum(a => a.Amount);
            }

            lastPaymentDate = DateTime.MinValue;
            lastEndDate = DateTime.MinValue;

            return 0;
        }

        private decimal GetCurrentDebt(Guid creditCardId, out DateTime currentPaymentDate, out DateTime currentEndDate, out Guid currentPeriodId) {
            var currentPeriod =
                bankCreditCardPeriodRepository
                    .Query(a => a.BankCreditCardId == creditCardId && DateTime.Today >= a.StartDate && DateTime.Today <= a.EndDate)
                    .OrderByDescending(a => a.EndDate)
                    .FirstOrDefault();

            if (currentPeriod != null) {
                currentPaymentDate = currentPeriod.PaymentDate;
                currentEndDate = currentPeriod.EndDate;
                currentPeriodId = currentPeriod.Id;

                return expenseRepository
                        .Query(a => a.BankCreditCardId == creditCardId && a.BankCreditCardPeriodId == currentPeriod.Id)
                        .ToListNoLock()
                        .Sum(a => a.Amount);
            }

            currentPaymentDate = DateTime.MinValue;
            currentEndDate = DateTime.MinValue;
            currentPeriodId = Guid.Empty;

            return 0;
        }

        private decimal GetNextDebt(Guid creditCardId) {
            // dönemi boş olan yada henüz donemine girmeden erken açılan 1 donem
            // ekstre kesilmeden once acılan donem icin
            var nextPeriodId = Guid.Empty;
            var nextPeriod =
                bankCreditCardPeriodRepository
                    .Query(a => a.BankCreditCardId == creditCardId && a.StartDate > DateTime.Today)
                    .FirstOrDefault();
            if (nextPeriod != null) {
                nextPeriodId = nextPeriod.Id;
            }

            return expenseRepository
                    .Query(a => a.BankCreditCardId == creditCardId && (a.BankCreditCardPeriodId == null || a.BankCreditCardPeriodId == nextPeriodId))
                    .ToListNoLock()
                    .Sum(a => a.Amount);
        }
    }
}
