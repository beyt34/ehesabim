using System;

namespace eHesabim.Services.Models {
    public class CustomerAbstractDataModel {
        public int No { get; set; }

        public DateTime TrnDate { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public string FileName { get; set; }
    }
}
