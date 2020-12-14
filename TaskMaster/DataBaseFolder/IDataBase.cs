using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.Domain;

namespace TaskMaster.DataBaseFolder
{
    interface IDataBase
    {
        void AddTask(ITask task);
        void AddPerson(Person person);
        void AddTeam(Team team);

    }
}
