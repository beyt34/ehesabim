using System;
using System.Collections.Generic;

namespace eHesabim.Web.Portal.Models {
    public class ExpenseGroupReportWebModel : BaseWebModel<Guid> {
        public int Year { get; set; }

        public List<int> YearList { get; set; }

        public IEnumerable<ViewExpenseGroupReportWebModel> Data { get; set; }
    }
}