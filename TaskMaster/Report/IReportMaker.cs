using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Report
{
    interface IReportMaker
    {
        string CreateTasksReport(List<ITask> tasks);
    }
}
