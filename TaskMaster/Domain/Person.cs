using System.Collections.Generic;
using System.Linq;

namespace TaskMaster
{
    public class Person : IPerformer, IOwner
    {
        public Person(ulong id)
        {
            Id = id;
        }
        public ulong Id { get; }
        public ICollection<ITask> TakenTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> DoneTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> OwnedTasks { get; } = new HashSet<ITask>();
    }
}