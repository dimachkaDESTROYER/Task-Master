using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.DataBaseFolder;
using TaskMaster.Domain;

namespace TaskMaster.Tests
{
    [TestFixture]
    class DataBaseTests
    {
        [Test]
        public void TestTaskAfterUploadingAndDownloadingIsTheSame()
        {
            var db = new DataBase();
            db.CleanTasks();
            var person = new Person(3, new HashSet<ITask>(), new HashSet<ITask>(), new List<ITask>());
            var task = new SimpleTask(1, "myTopic", "myDesckription", TaskState.NotTaken, DateTime.MinValue,
                DateTime.MaxValue, DateTime.MaxValue, person, person);
            db.AddTask(task);
            var downloadedTask = db.GetTask(1);
            Assert.AreEqual(task.Description, downloadedTask.Description);
        }
    }
}
