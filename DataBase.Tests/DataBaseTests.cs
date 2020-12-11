using NUnit.Framework;
using System;
using System.Collections.Generic;
using TaskMaster.Domain;
using TaskMaster;
using System.Diagnostics;
using TaskMaster.Domain.Tasks;

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
        public void GetAllTasksPrformedByPerson()
        {
            db.Clean();
            var performer = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var owner = new Person(2, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var t1 = new SimpleTask(1, "myTopic1", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), owner: owner, performer: performer);
            var t2 = new SimpleTask(2, "myTopic2", "myDesckription2", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), owner: performer, performer: owner);
            owner.OwnedTasks.Add(t1);
            performer.OwnedTasks.Add(t2);
            db.AddPerson(performer);
            db.AddPerson(owner);
            db.AddTask(t1);
            db.AddTask(t2);

            var downloadedTasks = db.GetAllTasksPerformedBy(performer);
            Assert.AreEqual(1, downloadedTasks.Count);
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

        [Test]
        public void SecondAdd()
        {
            db.AddPerson(new Person(121, "Petya"));
            var person = db.GetPerson(121);
            var task = new SimpleTask(1, "first", "smth", TaskState.NotTaken, DateTime.Now,
                DateTime.Now, DateTime.Now, person, person);
            person.OwnedTasks.Add(task);
            db.Change(person);
        }
        public void ChangeTask()
        {
            var sw = new Stopwatch();
            sw.Start();
            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var t1 = new SimpleTask(1, "Topic", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            person.TakenTasks.Add(t1);
            db.AddPerson(person);
            t1.Topic = "newTopic";
            db.ChangeTask(t1);
            var downloaded = db.GetTask(1);
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 200);
            Assert.AreEqual(t1.Topic, downloaded.Topic);
            Assert.AreEqual(1, downloaded.Id);
        }
        [Test]
        public void ChangePersonAfterAddingTask()
        {
            var sw = new Stopwatch();
            sw.Start();

            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            db.AddPerson(person);
            var t1 = new SimpleTask(1, "Topic", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person);
            db.AddTask(t1);//проблема: это делать обязательно, а лучше бы AddPerson сам это делал
            person.TakenTasks.Add(t1);
            db.Change(person);
            var downloaded = db.GetPerson(1);

            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 200);
            Assert.AreEqual(person.Id, downloaded.Id);
            Assert.AreEqual(person.TakenTasks.Count, downloaded.TakenTasks.Count);
            Assert.AreEqual(person.TakenTasks[0].Id, downloaded.TakenTasks[0].Id);
        }

        [Test]
        public void GetAllTasksPerformedByPerson()
        {
            var performer = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var owner = new Person(2, new List<ITask>(), new List<ITask>(), new List<ITask>());
            var t1 = new SimpleTask(1, "myTopic1", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), owner: owner, performer: performer);
            var t2 = new SimpleTask(2, "myTopic2", "myDesckription2", TaskState.NotTaken, new DateTime(2020, 12, 5),
                new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), owner: performer, performer: owner);
            owner.OwnedTasks.Add(t1);
            performer.OwnedTasks.Add(t2);
            db.AddPerson(performer);
            db.AddPerson(owner);
            db.AddTask(t1);
            db.AddTask(t2);

            var downloadedTasks = db.GetAllTasksPerformedBy(performer);
            Assert.AreEqual(1, downloadedTasks.Count);
        }



        [Test]
        public void AddTaskWithNulledFinish()
        {
            var sw = new Stopwatch();
            sw.Start();

            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            db.AddPerson(person);
            var t1 = new SimpleTask(89, "Topic", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                null, new DateTime(2020, 12, 7), person, person);

            person.TakenTasks.Add(t1);
            db.AddTask(t1);
            var downloaded = db.GetTask(t1.Id);

            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 200);
            Assert.AreEqual(t1.Id, downloaded.Id);
        }

        [Test]
        public void AddTaskWithNulledPerformer()
        {
            var sw = new Stopwatch();
            sw.Start();

            var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
            db.AddPerson(person);
            var t1 = new SimpleTask(1, "Topic", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
                null, new DateTime(2020, 12, 7), person, null);
            db.AddTask(t1);
            person.TakenTasks.Add(t1);
            var downloaded = db.GetTask(t1.Id);
            sw.Stop();
            Assert.Less(sw.ElapsedMilliseconds, 200);
            Assert.AreEqual(t1.Id, downloaded.Id);
            Assert.AreEqual(null, downloaded.Performer);
        }

        [Test]
        public void GettingCorrectPersonAfterCahngingAndSaving()
        {
            db.AddPerson(new Person(121, "Petya"));
            var person = db.GetPerson(121);
            var task = new SimpleTask(1, "first", "smth", TaskState.NotTaken, DateTime.Now,
                DateTime.Now, DateTime.Now, person, person);
            db.AddTask(task);
            person.OwnedTasks.Add(task);
            db.Change(person);
            var downloaded = db.GetPerson(person.Id);
            Assert.AreEqual(1, downloaded.OwnedTasks.Count);
            Assert.AreEqual(person.OwnedTasks[0].Topic, downloaded.OwnedTasks[0].Topic);

        }

        [Test]
        public void ThrowExceptionTryingToAddTheSamePerson()
        {
            var person = new Person(121, "Petya");
            db.AddPerson(person);
            Assert.Throws<ArgumentException>(() => db.AddPerson(person));
        }

        [Test]
        public void PersonExistsInDBAfterAdding()
        {
            var person = new Person(121, "Petya");
            db.AddPerson(person);
            Assert.True(db.Contains(person.Id));
        }

        //[Test]
        //public void AddAndGetBranchedEmptyTask()
        //{
        //    var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
        //    var brTask = new BranchedTask(1, "myTopic1", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
        //        new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person, new List<ITask>());
        //    db.AddPerson(person);
        //    db.AddTask(brTask);
        //    var downloaded = db.GetTask(brTask.Id);
        //    Assert.AreEqual(brTask.Id, downloaded.Id);
        //    Assert.AreEqual(brTask.Owner.Id, downloaded.Owner.Id);
        //    Assert.AreEqual(0, brTask.SubTasks.Count);
        //}

        //[Test]
        //public void AddAndGetBranchedEmptyTask()
        //{
        //    var person = new Person(1, new List<ITask>(), new List<ITask>(), new List<ITask>());
        //    var brTask = new BranchedTask(1, "myTopic1", "myDesckription1", TaskState.NotTaken, new DateTime(2020, 12, 5),
        //        new DateTime(2020, 12, 6), new DateTime(2020, 12, 7), person, person, new List<ITask>());
        //    db.AddPerson(person);
        //    db.AddTask(brTask);
        //    var downloaded = db.GetTask(brTask.Id);
        //    Assert.AreEqual(brTask.Id, downloaded.Id);
        //    Assert.AreEqual(brTask.Owner.Id, downloaded.Owner.Id);
        //    Assert.AreEqual(0, brTask.SubTasks.Count);
        //}
    }
}