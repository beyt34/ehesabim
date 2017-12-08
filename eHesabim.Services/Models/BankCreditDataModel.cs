using System;

namespace eHesabim.Services.Models {
    public class BankCreditDataModel : BaseDataModel<Guid> {
        public int UserId { get; set; }

        public int BankId { get; set; }

        public string BankName { get; set; }

        public DateTime CreditDateTime { get; set; }

        public decimal Capital { get; set; }

        public decimal Rate { get; set; }

        public int Installment { get; set; }

        public decimal MonthlyPayment { get; set; }

        public decimal Expense { get; set; }
        
        public decimal RemainCapital { get; set; }

        public int RemainInstallment { get; set; }

        public DateTime PaymentDateTime { get; set; }

        public bool IsActive { get; set; }
    }
}
