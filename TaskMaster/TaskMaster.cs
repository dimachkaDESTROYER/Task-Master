using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.Domain;
using telBot;

namespace TaskMaster
{
    public class TaskMasters
    {
        // наконец-то поменять на бд!!!
        public static Dictionary<long, Person> users = new Dictionary<long, Person>();

        public static void CreateSimpleTask(long id, string name)
        {
            /* Надо придумать нормальный TaskID, тот что ниже ломается из-за невлезания в int */
            /* поэтому пока передам просто id*/

            //var taskId = int.Parse(id.ToString()
            //    + users[id].OwnedTasks.Count.ToString()
            //    + users[id].TakenTasks.Count.ToString()
            //    + users[id].DoneTasks.Count.ToString());

            ((IOwner)users[id]).AddTask(new SimpleTask(Convert.ToInt32(id), users[id], users[id], name));
        }

        //на кнопочки заифаю
        public static List<ITask> GetOwnedTasks(long id)
        {
            return users[id].OwnedTasks;
        }

        public static List<ITask> GetTakenTasks(long id)
        {
            return users[id].TakenTasks;
        }

        public static List<ITask> GetDoneTasks(long id)
        {
            return users[id].DoneTasks;
        }

        public static bool TryTakeTask(ITask task, long id)
        {
            return task.TryTake(users[id]);
        }
        public static bool TryPerformTask(ITask task, long id)
        {
            return task.TryPerform(users[id]);
        }

        public static void DeleteTask(ITask task, long id, ListTask list )
        {
            switch (list)
            {
                case ListTask.Taken:
                    {
                        users[id].TakenTasks.Remove(task);
                        break;
                    }
                case ListTask.Owned:
                    {
                        users[id].OwnedTasks.Remove(task);
                        break;
                    }
                case ListTask.Done:
                    {
                        users[id].DoneTasks.Remove(task);
                        break;
                    }
            }
        }

        // пока не работает как надо
        public static ITask GetTask(long id, ListTask list, int numberTask)
        {
            switch (list)
            {
                case ListTask.Taken:
                    {
                        return users[id].TakenTasks[numberTask];
                    }
                case ListTask.Owned:
                    {
                        return users[id].OwnedTasks[numberTask];
                    }
                case ListTask.Done:
                    {
                        return users[id].DoneTasks[numberTask];
                    }
                default:
                    {
                        return default;
                    }
                }
        }
    }
}
