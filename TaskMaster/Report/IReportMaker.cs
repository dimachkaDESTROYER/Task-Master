using System.Collections.Generic;

namespace TaskMaster.Report
{
    public interface IReportMaker
    {
        string CreateTasksReport(List<ITask> tasks);
    }
}
