using System;

namespace eHesabim.Services.Models {
    public class CustomerTransactionDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public Guid? ParentId { get; set; }

        public Guid CustomerId { get; set; }

        public string CustomerName { get; set; }

        public DateTime TrnDateTime { get; set; }

        public string Name { get; set; }

        public int TypeId { get; set; }
        
        public decimal Amount { get; set; }

        public decimal Debit { get; set; }

        public decimal Claim { get; set; }

        public DateTime? DueDateTime { get; set; }

        public int? InstallmentNo { get; set; }

        public int? InstallmentTotal { get; set; }

        public string Installment { get; set; }

        public Guid? BankAccountId { get; set; }

        public string FileName { get; set; }
    }
}
