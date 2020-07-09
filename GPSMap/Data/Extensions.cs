using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Data;

namespace GPSMap.Data
{
    public static class ExtensionMethods
    {
        public static System.Web.Mvc.SelectList ToSelectList<TEnum>(this TEnum obj, string selectedValues = null)
        where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return new SelectList(Enum.GetValues(typeof(TEnum))
            .OfType<Enum>()
            .Select(x => new SelectListItem
            {
                Text = Enum.GetName(typeof(TEnum), x),
                Value = (Convert.ToString(x))
            }), "Value", "Text");
        }

        public static decimal ToDecimal(this string str)
        {
            // you can throw an exception or return a default value here
            if (string.IsNullOrEmpty(str))
                return 0;

            decimal d;

            // you could throw an exception or return a default value on failure
            if (!decimal.TryParse(str, out d))
                return 0;

            return d;
        }

        //public static DataTable ToDataTable(this ExcelPackage package)
        public static DataTable ToDataTable(this ExcelWorksheet workSheet)
        {
            //ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            List<string> collist = new List<string>();
            int columnCount = 1;
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                if (table.Columns.Contains(firstRowCell.Text))
                {
                    while (table.Columns.Contains(firstRowCell.Text + columnCount.ToString()))
                    {
                        columnCount += 1;
                    }
                    table.Columns.Add(firstRowCell.Text + columnCount.ToString());
                }
                else
                {
                    columnCount = 1;
                    table.Columns.Add(firstRowCell.Text);
                }

                var column = workSheet.Cells[1, table.Columns.Count, workSheet.Dimension.End.Row, table.Columns.Count];
                if (column.Any(x => !string.IsNullOrEmpty(x.Text)))
                {
                    collist.Add(table.Columns[table.Columns.Count - 1].ToString());
                }
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                if (row.Any(x => !string.IsNullOrEmpty(x.Text)))
                {
                    var newRow = table.NewRow();
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }
                    table.Rows.Add(newRow);
                }
            }
            //string[] colnames = new string[] { };
            //colnames.add
            //"Object", "Period from" };


            //String[] str = collist.ToArray();
            //return table;
            return table.DefaultView.ToTable(false, collist.ToArray());
        }


        public static DataTable ToDataTablePara(this ExcelWorksheet workSheet)
        {
            //ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
            DataTable table = new DataTable();
            List<string> collist = new List<string>();
            int columnCount = 1;
            foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
            {
                if (table.Columns.Contains(firstRowCell.Text))
                {
                    while (table.Columns.Contains(firstRowCell.Text + columnCount.ToString()))
                    {
                        columnCount += 1;
                    }
                    table.Columns.Add(firstRowCell.Text + columnCount.ToString());
                }
                else
                {
                    columnCount = 1;
                    table.Columns.Add(firstRowCell.Text);
                }

                var column = workSheet.Cells[1, table.Columns.Count, workSheet.Dimension.End.Row, table.Columns.Count];
                if (column.Any(x => !string.IsNullOrEmpty(x.Text)))
                {
                    collist.Add(table.Columns[table.Columns.Count - 1].ToString());
                }
            }

            for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                if (row.Any(x => !string.IsNullOrEmpty(x.Text)))
                {
                    var newRow = table.NewRow();
                    foreach (var cell in row)
                    {
                        newRow[cell.Start.Column - 1] = cell.Text;
                    }
                    table.Rows.Add(newRow);
                }
            }
            //string[] colnames = new string[] { };
            //colnames.add
            //"Object", "Period from" };


            //String[] str = collist.ToArray();
            //return table;
            return table.DefaultView.ToTable(false, collist.ToArray());
        }

    }
}