using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace eHesabim.Web.Portal.Export {
    public class ExportToExcel {
        public static byte[] ExportExcel<T>(List<T> records, IEnumerable<ExcelColumn> columns, string sheetName) {
            var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add(sheetName);
            var colIndex = 1;
            foreach (var column in columns) {
                ws.Cells[1, colIndex].Value = column.TextField;
                ws.Cells[1, colIndex].Style.Font.Bold = true;
                if (!string.IsNullOrEmpty(column.ColumnFormat)) {
                    ws.Column(colIndex).Style.Numberformat.Format = column.ColumnFormat;
                }

                if (column.Width > 0) {
                    ws.Column(colIndex).Width = column.Width;
                }
                else {
                    ws.Column(colIndex).AutoFit();
                }
                
                colIndex++;
            }

            if (!records.Any()) {
                return package.GetAsByteArray();
            }

            ws.Cells["A2"].LoadFromCollection(records, false);
            return package.GetAsByteArray();
        }
    }
}