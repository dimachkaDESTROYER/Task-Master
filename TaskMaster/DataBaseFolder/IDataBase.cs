using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.Domain;

namespace TaskMaster
{
    public interface IDataBase
    {
        void AddTask(ITask task);
        void AddPerson(Person person);
        void AddTeam(Team team);
        Team GetTeam(long id);
        Person GetPerson(long id);
        void ChangePerson(Person person);
        void ChangeTeam(Team team);
        ITask GetTask(int idTask);
        void DeleteTask(int id);
        void ChangeTask(ITask task);
        bool ContainsPerson(long id);
        bool ContainsTeam(long id);
    }
}
