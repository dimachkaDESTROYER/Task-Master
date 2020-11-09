using System;
using System.Collections.Generic;
using TaskMaster.Domain;

namespace TaskMaster
{
    public interface ITask
    {
        string Topic { get; }
        string Description { get; }
        TaskState State { get; set; }
        DateTime? Start { get; set; }
        DateTime? Finish { get; set; }
        DateTime? DeadLine { get; set; }
        List<ITask> SubTasks { get; set; }
        Person Solver { get; set;}

        void Solve(Person solver = null)
        {
            if (solver is null)
                solver = Solver;
            foreach (var subTask in SubTasks)
                subTask.Solve(solver);
            State = TaskState.Done;
            Finish = DateTime.Now;
        }

        void AddSubTask(ITask task)
        {
            SubTasks.Add(task);
        }
    }
}
