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
        public static DataBase db = new DataBase();
        public static void CreateSimpleTask(long id, string name, string description, DateTime deadline)
        {
            var person = db.GetPerson(id);
            var taskId = Math.Abs(id + name.GetHashCode());
            var task = new SimpleTask(unchecked((int)taskId), name, description,
                                      TaskState.NotTaken, DateTime.Now, null, 
                                      deadline, person, null);

            db.AddTask(task);
            person.OwnedTasks.Add(task);
            db.ChangePerson(person);
        }

        public static List<ITask> GetOwnedTasks(long id, string name) => db.GetPerson(id).OwnedTasks;
        public static List<ITask> GetTakenTasks(long id, string name) => db.GetPerson(id).TakenTasks;
        public static List<ITask> GetDoneTasks(long id) => db.GetPerson(id).DoneTasks;
        public static ITask GetTask(int idTask) => db.GetTask(idTask);

        public static void DeleteTask(long personID, ITask task)
        {
            var person = db.GetPerson(personID);
            var taskToRemove = person.OwnedTasks.Where(t => t.Id == task.Id);
            if (taskToRemove.Any())
                person.OwnedTasks.Remove(taskToRemove.First());

            taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
            if (taskToRemove.Any())
                person.TakenTasks.Remove(taskToRemove.First());

            taskToRemove = person.DoneTasks.Where(t => t.Id == task.Id);
            if (taskToRemove.Any())
                person.DoneTasks.Remove(taskToRemove.First());

            db.ChangePerson(person);
            db.DeleteTask(task.Id);
        }

        public static bool TryTakeTask(ITask task, long id)
        {
            var person = db.GetPerson(id);
            if (task.TryTake(person))
            {
                db.ChangeTask(task);
                person.TakenTasks.Add(task);
                db.ChangePerson(person);
                return true;
            }
            return false;
        }

        public static bool TryPerformTask(ITask task, long id)
        {
            var person = db.GetPerson(id);
            if (task.TryPerform(person))
            {
                db.ChangeTask(task);
                person.OwnedTasks.Remove(task);
                person.TakenTasks.Remove(task);
                person.DoneTasks.Add(task);
                db.ChangePerson(person);
                return true;
            }
            return false;
        }
    }
}
