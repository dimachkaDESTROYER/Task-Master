using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class Team : IOwner
    {
        public ulong Id { get; }
        public HashSet<Person> Persons;
        public List<ITask> OwnedTasks { get; }

        public Team(ulong id)
        {
            Id = id;
        }

        public void AddPerson(Person person) => Persons.Add(person);
    }
}
