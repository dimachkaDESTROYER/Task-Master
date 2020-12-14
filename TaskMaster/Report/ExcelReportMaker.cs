using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TaskMaster.Domain;
using TaskMaster.Domain.Tasks;

namespace TaskMaster.Report
{
    public class ExcelReportMaker : IReportMaker
    {
        private List<PropertyInfo> SimpleTaskProperties;
        private List<PropertyInfo> BranchedTaskProperties;

        public ExcelReportMaker()
        {
            SimpleTaskProperties = GetProperties(typeof(SimpleTask));
            BranchedTaskProperties = GetProperties(typeof(BranchedTask));
        }
        public static void Main()
        {
            var person = new Person(5, new List<ITask>(), new List<ITask>(), new List<ITask>(), "Valera");
            var task = new SimpleTask(1, "myTopic", "myDesckription", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            var reporter = new ExcelReportMaker();
            reporter.CreateTasksReport(new List<ITask>() { task });
        }
        public string CreateTasksReport(List<ITask> tasks)
        {
            var currentDir = Directory.GetCurrentDirectory();
            var directoryPath = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName + @"\TaskMaster\";

            string fileName = @"newreport.xlsx";
            var path = directoryPath + fileName;

            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets
                .Add("Simple Tasks");
            FillSimpleTaskSheet(sheet, tasks.Where(t => t is SimpleTask).Select(t => (SimpleTask)t).ToList());

            sheet = package.Workbook.Worksheets
                .Add("Branched Tasks");
            File.WriteAllBytes(path, package.GetAsByteArray());
            return path;
        }

        private void FillSimpleTaskSheet(ExcelWorksheet sheet, List<SimpleTask> tasks)
        {
            FillHeaders(SimpleTaskProperties, sheet);

            for (var row = 2; row < tasks.Count + 2; row++)
            {
                for (int column = 1; column < SimpleTaskProperties.Count + 1; column++)
                {
                    var v = SimpleTaskProperties[column - 1].GetValue(tasks[row - 2]);
                    if (v != null)
                        sheet.Cells[row, column].Value = v.ToString();
                }
            }
        }

        private void FillHeaders(List<PropertyInfo> properties, ExcelWorksheet sheet)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                sheet.Cells[1, i + 1].Value = properties[i].Name;
            }
        }

        private List<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties().Where(p => p.Name != "Id").ToList();
        }
    }
}
