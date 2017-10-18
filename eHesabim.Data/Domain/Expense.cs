using System;

namespace eHesabim.Data.Domain {
    public class Expense : BaseEntity<Guid> {
        public int UserId { get; set; }

        public int TypeId { get; set; }

        public virtual Type Type { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime ExpenseDateTime { get; set; }

        public Guid ExpenseStoreId { get; set; }

        public virtual ExpenseStore ExpenseStore { get; set; }

        public Guid ExpenseGroupId { get; set; }

        public virtual ExpenseGroup ExpenseGroup { get; set; }

        public decimal Amount { get; set; }

        public int? InstallmentNo { get; set; }

        public int? InstallmentTotal { get; set; }

        public Guid? BankCreditCardId { get; set; }

        public virtual BankCreditCard BankCreditCard { get; set; }

        public Guid? BankCreditCardPeriodId { get; set; }

        public virtual BankCreditCardPeriod BankCreditCardPeriod { get; set; }

        public bool IsExclusion { get; set; }

        public string Notes { get; set; }

        public Guid? BankCreditSubId { get; set; }

        public virtual BankCreditSub BankCreditSub { get; set; }
    }
}
