using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class Team : IOwner
    {
        public ulong Id { get; }
        public List<Person> Persons { get; }
        public List<ITask> OwnedTasks { get; } = new List<ITask>();

        public Team(ulong id)
        {
            Id = id;
        }

        public Team(ulong id, List<Person> persons, List<ITask> ownedTasks)
        {
            Id = id;
            Persons = persons;
            OwnedTasks = ownedTasks;
        }

        public void AddPerson(Person person) => Persons.Add(person);
    }
}
