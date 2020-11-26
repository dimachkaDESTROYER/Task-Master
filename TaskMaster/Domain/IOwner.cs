using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace TaskMaster
{
    public interface IOwner
    {
<<<<<<< HEAD
        List<ITask> OwnedTasks {get;}
=======
        ulong Id { get; }
        ICollection<ITask> OwnedTasks {get;}
>>>>>>> ae978c5d94f77141a1442e6997e4e524536c8edc
        public void AddTask(ITask task)
        {
            OwnedTasks.Add(task);
        }
    }
}