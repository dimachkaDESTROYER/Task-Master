using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class Team : IOwner
    {
        public ulong Id { get; }
        public List<Person> Persons { get; }
        public ICollection<ITask> OwnedTasks { get; } = new HashSet<ITask>();

        public Team(ulong id)
        {
            Id = id;
        }

        public Team(ulong id, List<Person> persons, HashSet<ITask> ownedTasks)
        {
            Id = id;
            Persons = persons;
            OwnedTasks = ownedTasks;
        }

        public void AddPerson(Person person) => Persons.Add(person);
    }
}
