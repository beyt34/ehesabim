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
    public class BankAccountService : IBankAccountService {
        private readonly IRepository<BankAccount, Guid> bankAccountRepository;
        private readonly IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository;
        private readonly IRepository<BankCreditCardPayment, Guid> bankCreditCardPaymentRepository;
        private readonly IRepository<BankCreditSub, Guid> bankCreditSubRepository;
        private readonly IRepository<CustomerTransaction, Guid> customerTransactionRepository;

        public BankAccountService(IRepository<BankAccount, Guid> bankAccountRepository, IRepository<BankAccountTransaction, Guid> bankAccountTransactionRepository, IRepository<BankCreditCardPayment, Guid> bankCreditCardPaymentRepository, IRepository<BankCreditSub, Guid> bankCreditSubRepository, IRepository<CustomerTransaction, Guid> customerTransactionRepository) {
            this.bankAccountRepository = bankAccountRepository;
            this.bankAccountTransactionRepository = bankAccountTransactionRepository;
            this.bankCreditCardPaymentRepository = bankCreditCardPaymentRepository;
            this.bankCreditSubRepository = bankCreditSubRepository;
            this.customerTransactionRepository = customerTransactionRepository;
        }

        public List<BankAccountDataModel> GetDashboardBankAccountList(int userId) {
            const int DebitId = (int)BankAccountTransactionTypeEnum.Debit;
            const int ClaimId = (int)BankAccountTransactionTypeEnum.Claim;

            return bankAccountTransactionRepository
                        .Query(a => a.UserId == userId)
                        .Include(i => i.BankAccount)
                        .GroupBy(g => new { g.BankAccount.Id, g.BankAccount.TypeId, g.BankAccount.Name })
                        .Select(s => new BankAccountDataModel {
                            Id = s.Key.Id,
                            TypeId = s.Key.TypeId,
                            Name = s.Key.Name,
                            DebitTotal = s.Sum(ss => ss.TypeId == DebitId ? ss.Amount : 0),
                            ClaimTotal = s.Sum(ss => ss.TypeId == ClaimId ? ss.Amount : 0),
                            NetTotal = s.Sum(ss => ss.TypeId == DebitId ? ss.Amount : ss.Amount * -1)
                        })
                        .Where(a => a.NetTotal != 0)
                        .OrderBy(o => o.TypeId).ThenBy(t => t.Name)
                        .ToList();
        }

        public Guid AddUpdateBankAccount(Guid id, int userId, int typeId, string name, int? bankId, string iban, decimal? limit, bool isActive, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankAccount() : bankAccountRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.TypeId = typeId;
            entity.Name = name;
            entity.BankId = CommonHelper.CheckNullable(bankId);
            entity.Iban = iban;
            entity.Limit = CommonHelper.CheckNullable(limit);
            entity.IsActive = isActive;

            return bankAccountRepository.AddUpdate(entity);
        }

        public List<BankAccountDataModel> GetBankAccountList(int userId, int typeId, int bankId, string sort, bool sortDescending, int page, int pageSize, out int total) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "TypeId";
                sortDescending = false;
            }

            Expression<Func<BankAccount, bool>> query = f => !f.IsDeleted && f.UserId == userId;

            if (typeId != 0) {
                query = query.And(a => a.TypeId == typeId);
            }

            if (bankId != 0) {
                query = query.And(a => a.BankId == bankId);
            }

            // get data
            var list = bankAccountRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(i => i.Type)
                        .Include(i => i.Bank)
                        .ToListNoLock();

            // automap model
            var model = AutoMapperConfiguration.Mapper.Map<List<BankAccount>, List<BankAccountDataModel>>(list);

            // calculate totals
            ////for (var i = 0; i < model.Count; i++) {
            ////    model[i] = GetCalculatedModel(model[i]);
            ////}

            return model;
        }

        public List<SelectGuidDataModel> GetBankAccountList(int userId) {
            return bankAccountRepository
                       .Query(a => a.UserId == userId)
                       .OrderBy(o => o.TypeId).ThenBy(t => t.Name)
                       .Select(x => new SelectGuidDataModel { Id = x.Id, Name = x.Name })
                       .ToListNoLock();
        }

        public BankAccountDataModel GetBankAccountById(Guid id, int userId) {
            var data = bankAccountRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                var model = AutoMapperConfiguration.Mapper.Map<BankAccount, BankAccountDataModel>(data);
                return model;
            }

            return new BankAccountDataModel();
        }

        public void DeleteBankAccount(Guid id, int userId, out string errMessage) {
            // todo: hareket kontrolü
            errMessage = string.Empty;
            var data = bankAccountRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankAccountRepository.Delete(id);
            }
            else {
                errMessage = "record not found";
            }
        }

        public Guid AddUpdateBankAccountTransaction(Guid id, int userId, Guid bankAccountId, DateTime dateTime, string name, int typeId, decimal amount, Guid? tobankAccountId, out string errMessage) {
            errMessage = string.Empty;
            var entity = id == Guid.Empty ? new BankAccountTransaction() : bankAccountTransactionRepository.Detail(id);

            if (entity == null) {
                errMessage = "record not found";
                return Guid.Empty;
            }

            entity.UserId = userId;
            entity.BankAccountId = bankAccountId;
            entity.TrnDateTime = dateTime;
            entity.Name = name;
            entity.TypeId = typeId;
            entity.Amount = amount;

            id = bankAccountTransactionRepository.AddUpdate(entity);

            // hesap seçildi ise
            var bankAccountTransaction = bankAccountTransactionRepository.Query(a => a.RelatedId == id && a.UserId == userId).FirstOrDefault();
            if (tobankAccountId != null && tobankAccountId != Guid.Empty) {
                if (bankAccountTransaction == null) {
                    bankAccountTransaction = new BankAccountTransaction { UserId = userId, RelatedId = id };
                }

                // hesap hareketlerine ters bakiye (borç ise alacak, alacaksa borç)
                if (typeId == (int)BankAccountTransactionTypeEnum.Debit) {
                    typeId = (int)BankAccountTransactionTypeEnum.Claim;
                }
                else {
                    typeId = (int)BankAccountTransactionTypeEnum.Debit;
                }

                bankAccountTransaction.BankAccountId = tobankAccountId.Value;
                bankAccountTransaction.TrnDateTime = dateTime;
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

        public List<BankAccountTransactionDataModel> GetBankAccountTransactionList(int userId, Guid? bankAccountId, string name, DateTime? startDate, DateTime? endDate, string sort, bool sortDescending, int page, int pageSize, out int total, out decimal debitTotal, out decimal claimTotal) {
            if (string.IsNullOrEmpty(sort)) {
                sort = "TrnDateTime";
                sortDescending = true;
            }

            Expression<Func<BankAccountTransaction, bool>> query = f => !f.IsDeleted && f.UserId == userId;
            if (bankAccountId != null && bankAccountId != Guid.Empty) {
                query = query.And(a => a.BankAccountId == bankAccountId);
            }

            if (startDate != null) {
                query = query.And(a => a.TrnDateTime >= startDate);
            }

            if (endDate != null) {
                query = query.And(a => a.TrnDateTime <= endDate);
            }

            if (!string.IsNullOrEmpty(name)) {
                query = query.And(a => a.Name.Contains(name));
            }

            // get data
            var list = bankAccountTransactionRepository
                        .Filter(query, page, pageSize, sort, sortDescending, out total)
                        .Include(a => a.BankAccount)
                        .ToListNoLock();

            // toplamlar
            const int DebitId = (int)BankAccountTransactionTypeEnum.Debit;
            var queryDebit = bankAccountTransactionRepository.Query(query).Where(a => a.TypeId == DebitId);
            debitTotal = queryDebit.Any() ? queryDebit.Sum(s => s.Amount) : 0;

            const int ClaimId = (int)BankAccountTransactionTypeEnum.Claim;
            var queryClaim = bankAccountTransactionRepository.Query(query).Where(a => a.TypeId == ClaimId);
            claimTotal = queryClaim.Any() ? queryClaim.Sum(s => s.Amount) : 0;

            return AutoMapperConfiguration.Mapper.Map<List<BankAccountTransaction>, List<BankAccountTransactionDataModel>>(list);
        }

        public BankAccountTransactionDataModel GetBankAccountTransactionById(Guid id, int userId) {
            var data =
                bankAccountTransactionRepository
                    .Query(a => a.Id == id && a.UserId == userId)
                    .Include(i => i.BankCreditSub).DefaultIfEmpty()
                    .Include(i => i.BankCreditSub.BankCredit).DefaultIfEmpty()
                    .Include(i => i.BankCreditSub.BankCredit.Bank).DefaultIfEmpty()
                    .Include(i => i.BankCreditCardPayment).DefaultIfEmpty()
                    .Include(i => i.BankCreditCardPayment.BankCreditCard).DefaultIfEmpty()
                    .Include(i => i.CustomerTransaction).DefaultIfEmpty()
                    .Include(i => i.CustomerTransaction.Customer).DefaultIfEmpty()
                    .Include(i => i.Expense).DefaultIfEmpty()
                    .Include(i => i.Expense.ExpenseGroup).DefaultIfEmpty()
                    .Where(a => a != null)
                    .FirstOrDefault();

            var model = AutoMapperConfiguration.Mapper.Map<BankAccountTransaction, BankAccountTransactionDataModel>(data);

            // hesap hareketi varsa
            var related = bankAccountTransactionRepository.Query(a => a.RelatedId == id && a.UserId == userId).FirstOrDefault();
            if (related != null) {
                model.RelatedBankAccountId = related.BankAccountId;
            }

            return model;
        }

        public void DeleteBankAccountTransaction(Guid id, int userId, out string errMessage) {
            errMessage = string.Empty;
            var data = bankAccountTransactionRepository.Query(a => a.Id == id && a.UserId == userId).FirstOrDefault();
            if (data != null) {
                bankAccountTransactionRepository.Delete(id);

                // ilişkili customertransaction kaydı varsa onuda sil
                if (data.CustomerTransactionId.HasValue) {
                    customerTransactionRepository.Delete(data.CustomerTransactionId.Value);
                }

                // ilişkili k.kartı ödemesi varsa onuda sil
                if (data.BankCreditCardPaymentId.HasValue) {
                    bankCreditCardPaymentRepository.Delete(data.BankCreditCardPaymentId.Value);
                }

                // ilişkili kredi ödemesi varsa ödeme tarihini nulla
                if (data.BankCreditSubId.HasValue) {
                    var bankCreditSub = bankCreditSubRepository.Query(a => a.Id == data.BankCreditSubId.Value).FirstOrDefault();
                    if (bankCreditSub != null) {
                        bankCreditSub.PaymentDateTime = null;
                        bankCreditSubRepository.AddUpdate(bankCreditSub);
                    }
                }

                // ilişkili kasa kaydı varsa onuda sil
                var relatedAccount = bankAccountTransactionRepository.Query(a => a.RelatedId == id && a.UserId == userId).FirstOrDefault();
                if (relatedAccount != null) {
                    bankAccountTransactionRepository.Delete(relatedAccount.Id);
                }

                if (data.RelatedId.HasValue) {
                    bankAccountTransactionRepository.Delete(data.RelatedId.Value);
                }
            }
            else {
                errMessage = "record not found";
            }
        }
    }
}
