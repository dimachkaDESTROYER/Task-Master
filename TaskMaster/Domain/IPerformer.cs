using System.Collections.Generic;

namespace TaskMaster
{
    public interface IPerformer
    {
        ulong Id { get; }
        ICollection<ITask> TakenTasks { get; }
        ICollection<ITask> DoneTasks { get; }
        public void Take(ITask task)
        {
            if (task.TryTake(this))
                TakenTasks.Add(task);
        }

        public void Untake(ITask task) => TakenTasks.Remove(task);

        public void Perform(ITask task)
        {
            if (!task.TryPerform(this)) return;
            TakenTasks.Remove(task);
            DoneTasks.Add(task);
        }
        public bool IsTaskTaken(ITask task) => TakenTasks.Contains(task);
    }
}