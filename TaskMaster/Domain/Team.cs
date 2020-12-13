using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    public class Team : IOwner
    {
        public bool IsFull { get; } = true;

        public long Id { get; }
        public string Name { get; }
        public List<Person> Persons { get; } = new List<Person>();
        public List<ITask> OwnedTasks { get; } = new List<ITask>();

        public Team(long id, string name)
        {
            IsFull = false;
            Id = id;
            Name = name;
        }

        public Team(long id, List<Person> persons, List<ITask> ownedTasks, string name)
        {
            Id = id;
            Persons = persons;
            OwnedTasks = ownedTasks;
            Name = name;
        }

        public void AddPerson(Person person) => Persons.Add(person);

        public override string ToString() => Name;
    }
}
