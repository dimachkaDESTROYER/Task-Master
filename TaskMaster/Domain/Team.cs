using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class Team : IOwner
    {
        public HashSet<Person> Persons;
        public int OwnerId { get; }
        public ICollection<ITask> OwnedTasks { get; }

        public Team(int ownerId)
        {
            OwnerId = ownerId;
        }

        public void AddPerson(Person person) => Persons.Add(person);
    }
}
