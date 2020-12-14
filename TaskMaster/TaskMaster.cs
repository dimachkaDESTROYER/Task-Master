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
            IOwner owner;
            if (id < 0)
                owner = db.GetTeam(id);
            else
                owner = db.GetPerson(id);

            var taskId = Math.Abs(id + name.GetHashCode()) + deadline.GetHashCode();
            var task = new SimpleTask(unchecked((int)taskId), name, description,
                                      TaskState.NotTaken, DateTime.Now, null,
                                      deadline, owner, null);

            db.AddTask(task);
            owner.OwnedTasks.Add(task);

            if (owner is Person person)
                db.ChangePerson(person);
            else if (owner is Team team)
                db.ChangeTeam(team);
        }

        public static void CreateBranchedTask(long id, string name, string description, DateTime deadline)
        {

        }

        public static List<ITask> GetOwnedTasks(long id, string name)
        {
            if (id > 0)
                return db.GetPerson(id).OwnedTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.NotTaken).ToList();
        }

        public static List<ITask> GetTakenTasks(long id, string name)
        {
            if (id > 0)
                return db.GetPerson(id).TakenTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.InProcess).ToList();
        }

        public static List<ITask> GetDoneTasks(long id)
        {
            if (id > 0)
                return db.GetPerson(id).DoneTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.Done).ToList();
        }

        public static ITask GetTask(int idTask) => db.GetTask(idTask);

        public static bool TryDeleteTask(long id, ITask task)
        {
            IOwner owner;
            if (id > 0) owner = db.GetPerson(id);
            else owner = db.GetTeam(id);

            if (owner is Team team)
            {
                var taskToRemove = team.OwnedTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                {
                    team.OwnedTasks.Remove(taskToRemove.First());
                    if (task.Performer != null)
                    {
                        var person = db.GetPerson(task.Performer.Id);
                        taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
                        if (taskToRemove.Any())
                            person.TakenTasks.Remove(taskToRemove.First());

                        taskToRemove = person.DoneTasks.Where(t => t.Id == task.Id);
                        if (taskToRemove.Any())
                            person.DoneTasks.Remove(taskToRemove.First());
                        db.ChangePerson(person);
                    }
                    db.DeleteTask(task.Id);
                    db.ChangeTeam(team);
                    return true;
                }

            }
            else if (owner is Person person && task.Owner.Id > 0)
            {
                var taskToRemove = person.OwnedTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                {
                    person.OwnedTasks.Remove(taskToRemove.First());
                }

                taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                {
                    person.TakenTasks.Remove(taskToRemove.First());
                }

                taskToRemove = person.DoneTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                {
                    person.DoneTasks.Remove(taskToRemove.First());
                }

                db.DeleteTask(task.Id);
                db.ChangePerson(person);
                return true;
            }
            return false;
        }

        public static bool TryTakeTask(ITask task, long id)
        {
            var person = db.GetPerson(id);
            if (task.TryTake(person))
            {
                db.ChangeTask(task);
                person.TakenTasks.Add(task);

                var taskToRemove = person.OwnedTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                    person.OwnedTasks.Remove(taskToRemove.First());

                db.ChangePerson(person);
                return true;
            }
            return false;
        }

        public static bool TryPerformTask(ITask task, long id, long personId)
        {
            var person = db.GetPerson(personId);
            if (personId == task.Performer.Id)
            {
                if (task.TryPerform(person))
                {
                    db.ChangeTask(task);
                    if (id < 0)
                    {
                        var team = db.GetTeam(id);

                        var taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
                        if (taskToRemove.Any())
                            person.TakenTasks.Remove(taskToRemove.First());
                        db.ChangeTeam(team);
                    }
                    else
                    {
                        var taskToRemove = person.OwnedTasks.Where(t => t.Id == task.Id);
                        if (taskToRemove.Any())
                            person.TakenTasks.Remove(taskToRemove.First());

                        taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
                        if (taskToRemove.Any())
                            person.TakenTasks.Remove(taskToRemove.First());

                        person.DoneTasks.Add(task);
                    }
                    db.ChangePerson(person);
                    return true;
                }
            }
            return false;
        }

        public static void EditTask(ITask task, long id, string param, string change)
        {
            var property = task.GetType().GetProperty(param);

            if (property.PropertyType == typeof(DateTime?))
            {
                var date = change.Split('.').Select(v => int.Parse(v)).ToArray();
                property.SetValue(task, new DateTime(date[2], date[1], date[0]));
            }

            else if (property.PropertyType == typeof(string))
                property.SetValue(task, change);

            db.ChangeTask(task);
        }
    }
}
