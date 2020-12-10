using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskMaster.DataBaseFolder;
using TaskMaster.Domain;
using telBot;

namespace TaskMaster
{
    public class TaskMasters
    {
        public static DataBase database = new DataBase();

        //timeDeadline
        public static void CreateSimpleTask(long id, string name, string description, DateTime deadline)
        {
            var person = database.GetPerson(id);
            var h = Math.Abs(name.GetHashCode());
            var i = Math.Abs(id)
                + h;
            var taskId = i;
            var task = new SimpleTask(unchecked((int)taskId), name, description, TaskState.NotTaken, DateTime.Now,
                null, deadline, person, null);
            person.OwnedTasks.Add(task);
            database.Change(person);
        }

        public static List<ITask> GetOwnedTasks(long id, string name)
        {
            var person = database.GetPerson(id);
            return person.OwnedTasks;
        }

        public static List<ITask> GetTakenTasks(long id, string name)
        {
            var person = database.GetPerson(id);
            return person.TakenTasks;
        }

        public static List<ITask> GetDoneTasks(long id)
        {
            var person = database.GetPerson(id);
            return person.DoneTasks;
        }

        public static bool TryTakeTask(ITask task, long id)
        {

            return task.TryTake(database.GetPerson(id));
        }
        public static bool TryPerformTask(ITask task, long id)
        {
            return task.TryPerform(database.GetPerson(id));
        }

        public static void DeleteTask(int idTask)
        {
            database.DeleteTask(idTask);
        }


        public static ITask GetTask(int idTask)
        {
            return database.GetTask(idTask);
        }
    }
}
