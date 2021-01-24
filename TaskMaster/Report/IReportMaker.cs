using System.Collections.Generic;

namespace TaskMasterBot.Report
{
    public interface IReportMaker
    {
        string CreateTasksReport(List<ITask> tasks);
    }
}
