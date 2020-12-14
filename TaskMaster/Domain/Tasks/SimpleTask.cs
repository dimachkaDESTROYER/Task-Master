using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskMaster.Domain
{
    public class SimpleTask : ITask
    {
        public SimpleTask(int id, IOwner owner, IPerformer performer, string topic, string description = null)
        {
            Id = id;
            Owner = owner;
            Performer = performer;
            if (owner is Person person)
            {
                State = TaskState.InProcess;
                Performer = person;
            }
            else if (owner is Team team)
                State = TaskState.NotTaken;
            else
                throw new ArgumentException("Неизвестный тип владельца задания");

            Topic = topic;
            Description = description;
        }

        //TODO: надо на рефлексию переписать в infrastructure
        public SimpleTask(int id, string topic, string description, TaskState state,
            DateTime? start, DateTime? finish, DateTime deadline, IOwner owner, IPerformer performer)
        {
            Id = id;
            Owner = owner;
            Performer = performer;
            Topic = topic;
            Description = description;
            State = state;
            Start = start;
            Finish = finish;
            DeadLine = deadline;
        }


        public int Id { get; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? Finish { get; set; }
        public DateTime? DeadLine { get; set; }
        public IPerformer Performer { get; set; }
        public IOwner Owner { get; }

        public bool TryTake(IPerformer performer)
        {
            if (State != TaskState.NotTaken) return false;
            Start = DateTime.Now;
            State = TaskState.InProcess;
            Performer = performer;
            return true;
        }

        public virtual bool TryPerform(IPerformer performer)
        {
            if (State != TaskState.NotTaken &&
                !(State == TaskState.InProcess || performer.TakenTasks.Contains(this))) return false;
            Finish = DateTime.Now;
            State = TaskState.Done;
            return true;
        }

        public override string ToString() => string.Join("/n", GetType()
                                                                            .GetProperties()
                                                                            .Where(p => p.Name != "Id")
                                                                            .Select(p => $"{p.Name} : {p.GetValue(this)}"));
    }
}
