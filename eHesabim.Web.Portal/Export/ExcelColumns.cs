using System.Collections.Generic;
using eHesabim.Framework.Localization;

namespace eHesabim.Web.Portal.Export {
    public class ExcelColumns {
        public static List<ExcelColumn> GetCustomerAbstractColumns() {
            return new List<ExcelColumn> {
                    new ExcelColumn { TextField = Labels.Id, ValueField = "No", Width = 5 },
                    new ExcelColumn { TextField = Labels.Date, ValueField = "TrnDate", Width = 10, ColumnFormat = "dd.mm.yyyy" },
                    new ExcelColumn { TextField = Labels.Description, ValueField = "Name", Width = 50 },
                    new ExcelColumn { TextField = Labels.Amount, ValueField = "Amount", Width = 15, ColumnFormat = "#,###.00 ₺" },
                    new ExcelColumn { TextField = Labels.Balance, ValueField = "Balance", Width = 15, ColumnFormat = "#,###.00 ₺" },
                    new ExcelColumn { TextField = Labels.FileName, ValueField = "FileName", Width = 50 },
            };
        }
    }
}