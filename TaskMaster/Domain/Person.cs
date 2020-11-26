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

        public Person(ulong id, HashSet<ITask> takenTasks, HashSet<ITask> doneTasks, HashSet<ITask> ownedTasks)
        {
            Id = id;
            TakenTasks = takenTasks;
            DoneTasks = doneTasks;
            OwnedTasks = ownedTasks;
        }

        public ulong Id { get; }
        public ICollection<ITask> TakenTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> DoneTasks { get; } = new HashSet<ITask>();
        public List<ITask> OwnedTasks { get; } = new List<ITask>();
    }
}