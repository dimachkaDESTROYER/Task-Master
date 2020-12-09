using NUnit.Framework;
using System;
using System.Collections.Generic;
using TaskMaster.Domain;
using TaskMaster;
using System.Diagnostics;

namespace DataBase.Tests
{
    public class Tests
    {
        private TaskMaster.DataBaseFolder.DataBase db;
        [SetUp]
        public void Setup()
        {
            db = new TaskMaster.DataBaseFolder.DataBase();
            db.Clean();//долго создаётся, порядка 6 секунд
        }

        [Test]
        public void AddSimpleTaskAndGetIt()
        {
            var person = new Person(5, new List<ITask>(), new List<ITask>(), new List<ITask>(), "Valera");
            db.AddPerson(person);
            var task = new SimpleTask(1, "myTopic", "myDesckription", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            db.AddTask(task);
            var downloadedTask = db.GetTask(1);
            Assert.AreEqual(task.Id, downloadedTask.Id);
            Assert.AreEqual(task.Start, downloadedTask.Start);
            Assert.AreEqual(task.Finish, downloadedTask.Finish);
            Assert.AreEqual(task.DeadLine, downloadedTask.DeadLine);
            Assert.AreEqual(task.Description, downloadedTask.Description);
        }

        [Test]
        public void AddPersonWithTask()
        {
            var sw = new Stopwatch();            
            var db = new TaskMaster.DataBaseFolder.DataBase();
            db.Clean();
            sw.Start();
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var task = new SimpleTask(2, "the_topic", "the_description", TaskState.Done,
                 new DateTime(2020, 12, 5), new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            var tasks = new List<SimpleTask>() { task };
            db.AddTask(task);
            person.OwnedTasks.Add(task);
            person.TakenTasks.Add(task);
            db.AddPerson(person);
            var downloadedPerson = db.GetPerson(1);
            sw.Stop();
            Assert.AreEqual(person.Id, downloadedPerson.Id);
            Assert.AreEqual(person.OwnedTasks[0].Id, downloadedPerson.OwnedTasks[0].Id);
            Assert.AreEqual(person.TakenTasks[0].Id, downloadedPerson.TakenTasks[0].Id);
            Assert.Less(sw.ElapsedMilliseconds, 200);
        }

        [Test]
        public void AddEmptyPerson()
        {
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            db.AddPerson(person);
            var downloadedPerson = db.GetPerson(1);
            Assert.AreEqual(person.Id, downloadedPerson.Id);
            Assert.AreEqual(person.DoneTasks, downloadedPerson.DoneTasks);
            Assert.AreEqual(person.OwnedTasks, downloadedPerson.OwnedTasks);
            Assert.AreEqual(person.TakenTasks, downloadedPerson.TakenTasks);
        }

        [Test]
        public void GetAllTasksOwnedByPerson()
        {
            db.Clean();
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var t1 = new SimpleTask(1, "myTopic1", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            var t2 = new SimpleTask(2, "myTopic2", "myDesckription2", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            person.OwnedTasks.Add(t1);
            person.OwnedTasks.Add(t2);
            person.TakenTasks.Add(t1);
            person.TakenTasks.Add(t2);
            db.AddPerson(person);
            db.AddTask(t1);
            db.AddTask(t2);

            var downloadedTasks = db.GetAllTasksOwnedBy(person);
            Assert.AreEqual(2, downloadedTasks.Count);
        }

        [Test]
        public void GetEmptyPersonAsOwner()
        {
            var sw = new Stopwatch();
            sw.Start();
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            db.AddPerson(person);
            var downloaded = db.GetOwner(1);
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 100);
            Assert.AreEqual(1, ((Person)downloaded).Id);
            Assert.AreEqual(1, downloaded.Id);
        }
    }
}