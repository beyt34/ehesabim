using System;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseWebModel : BaseWebModel<Guid> {
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

        /* dummy fields for sub total */
        public string ExpenseMe { get; set; }

        public string ExpenseExclusion { get; set; }

        public string ExpenseTotal { get; set; }
    }
}