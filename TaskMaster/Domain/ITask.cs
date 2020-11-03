using System;

namespace TaskMaster
{
    public interface ITask
    {
        string Message { get;}
        TaskState State { get;}
        DateTime Start { get;}
        DateTime Finish { get;}
    }
}
