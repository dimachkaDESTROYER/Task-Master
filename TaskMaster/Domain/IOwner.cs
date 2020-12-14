using System.Collections.Generic;

namespace TaskMaster
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