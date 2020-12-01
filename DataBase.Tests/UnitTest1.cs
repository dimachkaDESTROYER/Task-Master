using NUnit.Framework;
using System;
using System.Collections.Generic;
using TaskMaster.Domain;
using TaskMaster;

namespace DataBase.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.True(true);
        }

        //[Test]
        //public void Test2()
        //{
        //    var db = new TaskMaster.DataBaseFolder.DataBase();
        //    db.CleanTasks();
        //    var person = new Person(3, new HashSet<ITask>(), new HashSet<ITask>(), new List<ITask>());
        //    var task = new SimpleTask(1, "myTopic", "myDesckription", TaskState.NotTaken, DateTime.MinValue,
        //        DateTime.MaxValue, DateTime.MaxValue, person, person);
        //    db.AddTask(task);
        //    var downloadedTask = db.GetTask(1);
        //    Assert.AreEqual(task.Description, downloadedTask.Description);
        //}
    }
}