using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace TaskMaster
{
    public interface ITask
    {
        int Id { get; protected set; }
        string Topic { get; protected set; }
        string Description { get; protected set; }
        TaskState State { get; protected set; }
        DateTime? Start { get; protected set; }
        DateTime? Finish { get; protected set; }
        DateTime? DeadLine { get; protected set; }
        IPerformer Performer { get; protected set; }
        IOwner Owner { get; }

        bool TryTake(IPerformer performer)
        {
            if (State != TaskState.NotTaken) return false;
            Start = DateTime.Now;
            State = TaskState.InProcess;
            Performer = performer;
            return true;
        }

        bool TryPerform(IPerformer performer)
        {
            if (State != TaskState.NotTaken &&
                !(State == TaskState.InProcess || performer.TakenTasks.Contains(this))) return false;
            Finish = DateTime.Now;
            State = TaskState.Done;
            return true;
        }

        void ChangeTopic(string newTopic)
        {
            Topic = newTopic;
        }

        void ChangeDescription(string newDescription)
        {
            Description = newDescription;
        }
    }
}
