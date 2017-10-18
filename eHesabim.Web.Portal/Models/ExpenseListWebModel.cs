using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseListWebModel : BaseWebModel<Guid> {
        public int SearchTypeId { get; set; }

        public SelectList SearchTypeList { get; set; }

        public Guid? SearchExpenseStoreId { get; set; }

        public SelectList SearchExpenseStoreList { get; set; }

        public Guid? SearchExpenseGroupId { get; set; }

        public SelectList SearchExpenseGroupList { get; set; }

        public Guid? SearchBankCreditCardId { get; set; }

        public SelectList SearchBankCreditCardList { get; set; }

        public Guid? SearchBankCreditCardPeriodId { get; set; }

        public SelectList SearchBankCreditCardPeriodList { get; set; }

        public DateTime? SearchStartDate { get; set; }

        public DateTime? SearchEndDate { get; set; }

        public int SearchNextPeriodId { get; set; }
        
        public SelectList SearchNextPeriodList { get; set; }

        public int SearchExcludeId { get; set; }

        public SelectList SearchExcludeList { get; set; }

        public string SearchNotes { get; set; }

        public IEnumerable<ExpenseWebModel> Data { get; set; }

        public DeleteWebModel DeleteData { get; set; }
    }
}