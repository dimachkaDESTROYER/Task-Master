using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain.Tasks
{
    public class BranchedTask : SimpleTask
    {
<<<<<<< HEAD
        public List<ITask> SubTasks { get; set; } = new List<ITask>();
=======
        public List<ITask> SubTasks { get; private set; } = new List<ITask>();
>>>>>>> bced1462114c6a578d68fa50189240b2a3860566

        public BranchedTask(int id, IOwner owner, IPerformer performer, string topic, string description, List<ITask> subTasks)
            : base(id, owner, performer, topic, description)
        {
            SubTasks = subTasks;
        }

        public BranchedTask(int id, IOwner owner, IPerformer performer, string topic, string description,
            TaskState state, DateTime? start, DateTime? finish, DateTime deadline, List<ITask> subTasks)
            : base(id, topic, description, state, start, finish, deadline, owner, performer)
        {
            SubTasks = subTasks;
        }

        public BranchedTask(int id, string topic, string description, TaskState state, DateTime? start,
             DateTime? finish, DateTime deadline, IOwner owner, IPerformer performer, List<ITask> subTasks)
            : base(id, topic, description, state, start, finish, deadline, owner, performer)
        {
            SubTasks = subTasks;
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
