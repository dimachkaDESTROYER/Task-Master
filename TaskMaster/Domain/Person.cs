using System.Collections.Generic;
using System.Linq;

namespace TaskMaster
{
    public class Person : IPerformer, IOwner
    {

        public bool IsFull { get; } = true;

        public Person(long id, string name)
        {
            IsFull = false;
            Id = id;
            Name = name;
        }

        public Person(long id, HashSet<ITask> takenTasks, HashSet<ITask> doneTasks, List<ITask> ownedTasks)
        {
            Id = id;
            TakenTasks = takenTasks;
            DoneTasks = doneTasks;
            OwnedTasks = ownedTasks;
        }

        public string Name { get; }
        public long Id { get; }
        public ICollection<ITask> TakenTasks { get; } = new HashSet<ITask>();
        public ICollection<ITask> DoneTasks { get; } = new HashSet<ITask>();
        public List<ITask> OwnedTasks { get; } = new List<ITask>();
    }
}