using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster;
using TaskMaster.Domain;
using TaskMaster.Domain.Tasks;

namespace DataBase.Tests
{
    class BranchedTaskTests
    {
        private TaskMaster.DataBaseFolder.DataBase db;
        [SetUp]
        public void Setup()
        {
            db = new TaskMaster.DataBaseFolder.DataBase();
            db.Clean();//долго создаётся, порядка 6 секунд
        }

        [Test]
        public void AddEmptyBranchedTaskAndGetIt()
        {
            var person = new Person(5, new List<ITask>(), new List<ITask>(), new List<ITask>(), "Valera");
            var task = new BranchedTask(1, "branchedTopic", "branchedDesckription", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person, new List<ITask>());
            db.AddTask(task);
            person.OwnedTasks.Add(task);
            db.AddPerson(person);

            BranchedTask downloadedTask = (BranchedTask)db.GetTask(task.Id);
            Assert.AreEqual(task.Id, downloadedTask.Id);
            Assert.AreEqual(task.Start, downloadedTask.Start);
            Assert.AreEqual(task.Finish, downloadedTask.Finish);
            Assert.AreEqual(task.DeadLine, downloadedTask.DeadLine);
            Assert.AreEqual(task.Description, downloadedTask.Description);
            Assert.AreEqual(task.SubTasks, downloadedTask.SubTasks);
        }

        [Test]
        public void AddBranchedWithOneSubTaskTaskAndGetIt()
        {
            var person = new Person(5, new List<ITask>(), new List<ITask>(), new List<ITask>(), "Valera");

            var sub = new SimpleTask(1, "SimpleTopic", "SimpleDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                null, new DateTime(2020, 12, 7), person, null);
            var task = new BranchedTask(2, "branchedTopic", "branchedDesckription", TaskState.NotTaken, new DateTime(2020, 12, 5),
                null, new DateTime(2020, 12, 7), person, person, new List<ITask>() { sub });
            db.AddTask(task);
            db.AddTask(sub);
            person.OwnedTasks.Add(task);
            db.AddPerson(person);

            BranchedTask downloadedTask = (BranchedTask)db.GetTask(task.Id);
            Assert.AreEqual(task.Id, downloadedTask.Id);
            Assert.AreEqual(task.Start, downloadedTask.Start);
            Assert.AreEqual(task.Finish, downloadedTask.Finish);
            Assert.AreEqual(task.DeadLine, downloadedTask.DeadLine);
            Assert.AreEqual(task.Description, downloadedTask.Description);
            Assert.AreEqual(task.SubTasks.Count, downloadedTask.SubTasks.Count);
            Assert.AreEqual(task.SubTasks[0].Topic, downloadedTask.SubTasks[0].Topic);
            Assert.AreEqual(task.SubTasks[0].Id, downloadedTask.SubTasks[0].Id);
            Assert.AreEqual(task.SubTasks[0].Owner.Id, downloadedTask.SubTasks[0].Owner.Id);
            Assert.AreEqual(task.SubTasks[0].Finish, downloadedTask.SubTasks[0].Finish);
        }
    }
}
