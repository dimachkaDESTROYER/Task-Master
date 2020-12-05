using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain.Tasks
{
    class BranchedTask : SimpleTask
    {
        private List<ITask> SubTasks { get; set; } = new List<ITask>();

        public BranchedTask(IOwner owner, string topic, string description, List<ITask> subTasks) : base(owner, topic, description)
        {
            SubTasks = subTasks;
        }

        public BranchedTask(int id, IOwner owner, IPerformer performer, string topic, string description,
            TaskState state, DateTime? start, DateTime? finish, DateTime deadline, List<ITask> subTasks)
            : base(id, owner, performer, topic, description, state, start, finish, deadline)
        {
            SubTasks = subTasks;
        }

        public override bool TryPerform(IPerformer performer)
        {
            if (!((SimpleTask) this).TryPerform(performer)) return false;
            foreach (var subTask in SubTasks)
                subTask.TryPerform(performer);
            return true;
        }
    }
}
