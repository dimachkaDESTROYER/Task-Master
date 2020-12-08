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

        public Person(long id, List<ITask> takenTasks, List<ITask> doneTasks, List<ITask> ownedTasks)
        {
            Id = id;
            TakenTasks = takenTasks;
            DoneTasks = doneTasks;
            OwnedTasks = ownedTasks;
        }

        public string Name { get; }
        public long Id { get; }
        public List<ITask> TakenTasks { get; } = new List<ITask>();
        public List<ITask> DoneTasks { get; } = new List<ITask>();
        public List<ITask> OwnedTasks { get; } = new List<ITask>();
    }
}