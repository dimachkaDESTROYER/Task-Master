using System.Collections.Generic;
using System.Reflection;
using OfficeOpenXml;

namespace TaskMaster.Infrastructure
{
    class ExcelPartialSerializatorFiller<T>
    {
        public void FillSheet(ExcelWorksheet sheet, List<T> tasks, List<PropertyInfo> properties)
        {
            FillHeaders(properties, sheet);
            for (var row = 2; row < tasks.Count + 2; row++)
            {
                for (var column = 1; column < properties.Count + 1; column++)
                {
                    var v = properties[column - 1].GetValue(tasks[row - 2]);
                    if (v != null)
                        sheet.Cells[row, column].Value = v.ToString();
                }
            }
        }

        private static void FillHeaders(List<PropertyInfo> properties, ExcelWorksheet sheet)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = properties[i].Name;
            }
        }
    }
}
