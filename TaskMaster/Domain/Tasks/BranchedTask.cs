using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskMaster.Domain.Tasks
{
    public class SubTasks : List<ITask>
    {
        public SubTasks(IEnumerable<ITask> tasks) : base(tasks)
        { }

        public SubTasks() : base()
        { }

        public override string ToString() => String.Join(',', this.Select(t => t.Topic));
    }

    public class BranchedTask : SimpleTask
    {
        public SubTasks SubTasks { get; private set; }

        public BranchedTask(int id, IOwner owner, IPerformer performer, string topic, string description, List<ITask> subTasks)
            : base(id, owner, performer, topic, description)
        {
            SubTasks = new SubTasks(subTasks);
        }

        public BranchedTask(int id, string topic, string description, TaskState state, DateTime? start,
             DateTime? finish, DateTime deadline, IOwner owner, IPerformer performer, List<ITask> subTasks)
            : base(id, topic, description, state, start, finish, deadline, owner, performer)
        {
            SubTasks = new SubTasks(subTasks);
        }

        public override bool TryPerform(IPerformer performer)
        {
            if (!((SimpleTask)this).TryPerform(performer)) return false;
            foreach (var subTask in SubTasks)
                subTask.TryPerform(performer);
            return true;
        }
    }
}
