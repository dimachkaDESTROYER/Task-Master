using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster;
using TaskMaster.Domain;
using TaskMaster.Domain.Tasks;
namespace DataBase.Tests
{
    class TeamTests
    {
        private TaskMaster.DataBaseFolder.DataBase db;
        [SetUp]
        public void Setup()
        {
            db = new TaskMaster.DataBaseFolder.DataBase();
            db.Clean();//долго создаётся, порядка 6 секунд
        }

        [Test]
        public void AddTeamWithOneEmtyPersoAndGetIt()
        {
            var person = new Person(5, new List<ITask>(), new List<ITask>(), new List<ITask>(), "Valera");            
            db.AddPerson(person);
            var team = new Team(3,new List<Person>() { person},new List<ITask>(), "NewTimeWith1Valera");
            db.AddTeam(team);
            var downloaded = db.GetTeam(team.Id);
            Assert.AreEqual(team.Id, downloaded.Id);
            Assert.AreEqual(team.Persons.Count, downloaded.Persons.Count);
            Assert.AreEqual(team.Persons[0].Id, downloaded.Persons[0].Id);
        }

        [Test]
        public void AddTeamWithPersoWithTaskAndGetIt()
        {
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var task = new SimpleTask(2, "the_topic", "the_description", TaskState.Done,
                 new DateTime(2020, 12, 5),null, new DateTime(2020, 12, 7), person, person);
            db.AddTask(task);
            person.OwnedTasks.Add(task);
            person.TakenTasks.Add(task);
            db.AddPerson(person);

            var team = new Team(3, new List<Person>() { person }, new List<ITask>(), "NewTimeWith1Valera");
            db.AddTeam(team);
            var downloaded = db.GetTeam(team.Id);

            Assert.AreEqual(team.Id, downloaded.Id);
            Assert.AreEqual(team.Persons.Count, downloaded.Persons.Count);
            Assert.AreEqual(team.Persons[0].Id, downloaded.Persons[0].Id);
            Assert.False(downloaded.Persons[0].IsFull);
            //Person тут неполный, поэтому у него нельзя позвать список тасок
        }
    }
}
