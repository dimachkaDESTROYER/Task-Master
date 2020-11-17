using System.Collections.Generic;
using System.Linq;

namespace TaskMaster
{
    public class Person : IPerformer, IOwner
    {
        public Person(int performerId, int ownerId)
        {
            PerformerId = performerId;
            OwnerId = ownerId;
        }
        public int PerformerId { get; }
        public int OwnerId { get; }
        public ICollection<ITask> TakenTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> DoneTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> OwnedTasks { get; } = new HashSet<ITask>();
    }
}