using System;

namespace eHesabim.Data.Domain {
    public class ViewExpenseGroupReport : ReadOnlyEntity<int> {
        public int UserId { get; set; }

        public Guid GroupId { get; set; }

        public string GroupName { get; set; }

        public int GroupYear { get; set; }

        public decimal Amount01 { get; set; }

        public decimal Amount02 { get; set; }

        public decimal Amount03 { get; set; }

        public decimal Amount04 { get; set; }

        public decimal Amount05 { get; set; }

        public decimal Amount06 { get; set; }

        public decimal Amount07 { get; set; }

        public decimal Amount08 { get; set; }

        public decimal Amount09 { get; set; }

        public decimal Amount10 { get; set; }

        public decimal Amount11 { get; set; }

        public decimal Amount12 { get; set; }

        public decimal AmountTotal { get; set; }
    }
}
