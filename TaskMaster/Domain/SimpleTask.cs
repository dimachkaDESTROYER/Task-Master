using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class SimpleTask : ITask
    {
        private string _topic;
        private string _description;
        private TaskState _state;
        private DateTime? _start;
        private DateTime? _finish;
        private DateTime? _deadLine;
        private IPerformer _performer;
        private int _id;

        public SimpleTask(IOwner owner, string topic, string description)
        {
            Owner = owner;
            _topic = topic;
            _description = description;
        }
        //TODO: надо на рефлексию переписать
        public SimpleTask(int id, IOwner owner, IPerformer performer, string topic, string description, TaskState state,
            DateTime? start, DateTime? finish, DateTime deadline)
        {
            _id = id;
            Owner = owner;
            _performer = performer;
            _topic = topic;
            _description = description;
            _state = state;
            _start = start;
            _finish = finish;
            _deadLine = deadline;
        }


        int ITask.Id
        {
            get => _id;
            set => _id = value;
        }

        string ITask.Topic
        {
            get => _topic;
            set => _topic = value;
        }

        string ITask.Description
        {
            get => _description;
            set => _description = value;
        }

        TaskState ITask.State
        {
            get => _state;
            set => _state = value;
        }

        DateTime? ITask.Start
        {
            get => _start;
            set => _start = value;
        }

        DateTime? ITask.Finish
        {
            get => _finish;
            set => _finish = value;
        }

        DateTime? ITask.DeadLine
        {
            get => _deadLine;
            set => _deadLine = value;
        }

        IPerformer ITask.Performer
        {
            get => _performer;
            set => _performer = value;
        }

        public IOwner Owner { get; }
    }
}
