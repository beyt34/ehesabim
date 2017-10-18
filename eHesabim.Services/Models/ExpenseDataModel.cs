using System;

namespace eHesabim.Services.Models {
    public class ExpenseDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public int TypeId { get; set; }

        public string TypeName { get; set; }

        public Guid? ParentId { get; set; }

        public DateTime ExpenseDateTime { get; set; }

        public Guid ExpenseStoreId { get; set; }

        public string ExpenseStoreName { get; set; }

        public Guid ExpenseGroupId { get; set; }

        public string ExpenseGroupName { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal Amount { get; set; }

        public int? InstallmentNo { get; set; }

        public int? InstallmentTotal { get; set; }

        public string Installment { get; set; }

        public Guid? BankCreditCardId { get; set; }

        public string BankCreditCardName { get; set; }

        public Guid? BankCreditCardPeriodId { get; set; }

        public string BankCreditCardPeriodName { get; set; }

        public bool IsExclusion { get; set; }

        public string Notes { get; set; }

        public Guid? BankAccountId { get; set; }

        public string Explanation { get; set; }
    }
}
