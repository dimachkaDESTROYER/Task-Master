using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TaskMaster
{
    public interface IOwner
    {
        ICollection<ITask> OwnedTasks {get;}
        public void AddTask(ITask task)
        {
            OwnedTasks.Add(task);
        }
    }
}