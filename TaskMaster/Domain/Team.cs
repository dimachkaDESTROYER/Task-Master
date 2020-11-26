using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class Team : IOwner
    {
        public ulong Id { get; }
<<<<<<< HEAD
        public HashSet<Person> Persons;
        public List<ITask> OwnedTasks { get; }
=======
        public List<Person> Persons { get; }
        public ICollection<ITask> OwnedTasks { get; } = new HashSet<ITask>();
>>>>>>> ae978c5d94f77141a1442e6997e4e524536c8edc

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
