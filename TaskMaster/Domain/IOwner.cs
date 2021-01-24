using System.Collections.Generic;

namespace TaskMasterBot
{
    public interface IOwner
    {
        List<ITask> OwnedTasks {get;}
        long Id { get; }
        public void AddTask(ITask task)
        {
            OwnedTasks.Add(task);
        }
    }
}