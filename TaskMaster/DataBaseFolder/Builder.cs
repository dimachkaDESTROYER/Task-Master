using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.Domain;
using TaskMaster.DataBaseFolder;
using System.Data.OleDb;
using System.Linq;

namespace TaskMaster.DataBaseFolder
{
    public class Builder
    {
        private DataBase db;

        private SimpleTask simpleTask;
        private Person person;
        private Team team;
        private List<ITask> tasksList;

        public Builder(DataBase db) => this.db = db;

        public Builder PrepareBuildingSimpleTask(OleDbDataReader reader)
        {
            simpleTask = new SimpleTask(
            id: (int)reader.GetInt64(0),
            topic: reader.GetString(1),
            description: reader.GetString(2),
            state: (TaskState)reader.GetInt32(3),
            start: reader.GetDateTime(4),
            finish: reader.GetDateTime(5),
            deadline: reader.GetDateTime(6),
            performer: db.GetPerson(reader.GetInt32(7)),
            owner: db.GetOwner(reader.GetInt32(8)));
            return this;
        }

        public Builder PrepareBuildingPerson(OleDbDataReader reader)
        {
            person = new Person(id: (int)reader.GetInt64(0),
                takenTasks: reader.GetString(1).Split(',').Select(tid => db.GetTask(Convert.ToInt32(tid))).ToHashSet(),
                doneTasks: reader.GetString(2).Split(',').Select(tid => db.GetTask(Convert.ToInt32(tid))).ToHashSet(),
                ownedTasks: reader.GetString(3).Split(',').Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList());
            return this;
        }

        public Builder PrepareBuildingTeam(OleDbDataReader reader)
        {
            //TODO: aхахаххах тут тоже имя
            team = new Team(id: (long)reader.GetInt64(0),
                persons: reader.GetString(1).Split(',').Select(pid => db.GetPerson(Convert.ToInt32(pid))).ToList(),
                ownedTasks: reader.GetString(2).Split(',').Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList(),
                "VITALIK");//TODO: NOT A VITALIK
            return this;
        }

        public Builder PrepareBuildingTasksList(OleDbDataReader reader)
        {
            tasksList = new List<ITask>();
            while (reader.Read())
            {
                tasksList.Add(new SimpleTask(
            id: (int)reader.GetInt64(0),
            topic: reader.GetString(1),
            description: reader.GetString(2),
            state: (TaskState)reader.GetInt32(3),
            start: reader.GetDateTime(4),
            finish: reader.GetDateTime(5),
            deadline: reader.GetDateTime(6),
            performer: db.GetPerson(reader.GetInt32(7)),
            owner: db.GetOwner(reader.GetInt32(8))));
            }
            return this;
        }

        public List<ITask> BuildTasksList() => tasksList;
        public SimpleTask BuildSimpleTask() => simpleTask;
        public Person BuildPerson() => person;
        public Team BuildTeam() => team;
    }
}
