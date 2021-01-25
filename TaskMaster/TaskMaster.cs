using System;
using System.Collections.Generic;
using System.Linq;
using TaskMasterBot.Domain;
using TaskMasterBot.Domain.Tasks;

namespace TaskMasterBot
{
    public class TaskMaster
    {
        private readonly IDataBase db;
        public TaskMaster(IDataBase db) => this.db = db;

        public void MakeTeam(long id, long personId, string pName, string cName)
        {
            if (!db.ContainsTeam(id))
            {
                var person = new Person(personId, pName);
                if (!db.ContainsPerson(personId))
                    db.AddPerson(person);

                var team = new Team(id, cName);
                team.AddPerson(person);
                db.AddTeam(team);
            }
            else if (!db.ContainsPerson(personId))
            {
                var person = new Person(personId, pName);
                db.AddPerson(person);
                var team = db.GetTeam(id);
                team.AddPerson(person);
            }
            else
            {
                var person = db.GetPerson(personId);
                var team = db.GetTeam(id);
                team.AddPerson(person);
            }
        }

        public void MakePerson(long id, string name)
        {
            if (!db.ContainsPerson(id))
                db.AddPerson(new Person(id, name));
        }

        public void CreateSimpleTask(long id, string name, string description, DateTime deadline)
        {
            IOwner owner;
            if (id < 0) owner = db.GetTeam(id);
            else owner = db.GetPerson(id);

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

        public void CreateBranchedTask(ITask currentTask, long id, string name, string description, DateTime deadline)
        {
            IOwner owner;
            if (id < 0) owner = db.GetTeam(id);
            else owner = db.GetPerson(id);

            var taskId = Math.Abs(id + name.GetHashCode()) + deadline.GetHashCode();
            if (currentTask is BranchedTask branchedTask)
            {
                var sub = new SimpleTask(unchecked((int)taskId), name, description,
                                      TaskState.NotTaken, DateTime.Now, null,
                                      deadline, owner, null);
                branchedTask.AddSubTask(sub);
                db.AddTask(sub);
                db.ChangeTask(branchedTask);
            }
            else
            {
                var subs = new List<ITask>();
                var subTask = new SimpleTask(unchecked((int)taskId), name, description,
                                      TaskState.NotTaken, DateTime.Now, null,
                                      deadline, owner, null);
                subs.Add(subTask);
                var task = new BranchedTask(currentTask.Id, currentTask.Topic,
                    currentTask.Description, currentTask.State,
                    currentTask.Start, currentTask.Finish,
                    currentTask.DeadLine, currentTask.Owner,
                    currentTask.Performer, subs);
                
                db.DeleteTask(currentTask.Id);
                db.AddTask(task);
                db.AddTask(subTask);

                var taskToRemove = owner.OwnedTasks.Where(t => t.Id == currentTask.Id);
                if (taskToRemove.Any())
                    owner.OwnedTasks.Remove(taskToRemove.First());
                owner.OwnedTasks.Add(task);
            }
            if (owner is Person person)
                db.ChangePerson(person);
            else if (owner is Team team)
                db.ChangeTeam(team);
        }

        public List<ITask> GetOwnedTasks(long id)
        {
            if (id > 0)
                return db.GetPerson(id).OwnedTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.NotTaken).ToList();
        }

        public List<ITask> GetTakenTasks(long id)
        {
            if (id > 0)
                return db.GetPerson(id).TakenTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.InProcess).ToList();
        }

        public List<ITask> GetDoneTasks(long id)
        {
            if (id > 0)
                return db.GetPerson(id).DoneTasks;
            else
                return db.GetTeam(id).OwnedTasks.Where(t => t.State == TaskState.Done).ToList();
        }

        public ITask GetTask(int idTask) => db.GetTask(idTask);

        public bool TryDeleteTask(long id, ITask task)
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
                    person.OwnedTasks.Remove(taskToRemove.First());

                taskToRemove = person.TakenTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                    person.TakenTasks.Remove(taskToRemove.First());

                taskToRemove = person.DoneTasks.Where(t => t.Id == task.Id);
                if (taskToRemove.Any())
                    person.DoneTasks.Remove(taskToRemove.First());

                db.DeleteTask(task.Id);
                db.ChangePerson(person);
                return true;
            }
            return false;
        }

        public bool TryTakeTask(ITask task, long id)
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

        public bool TryPerformTask(ITask task, long id, long personId)
        {
            var person = db.GetPerson(personId);
            if (task.Performer != null && personId == task.Performer.Id)
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

        public void EditTask(ITask task, long id, string param, string change)
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
