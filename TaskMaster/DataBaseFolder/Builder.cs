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

        public Builder(DataBase db) => this.db = db;

        public SimpleTask BuildSimpleTask(OleDbDataReader reader)
        {
            reader.Read();
            var performerId = Int64.Parse(reader.GetString(7));
            var performer = db.GetPartialPerformer(performerId);
            var ownerId = Int64.Parse(reader.GetString(8));
            var owner = db.GetPartialOwner(ownerId);
            return new SimpleTask(
                 id: Convert.ToInt32(reader.GetInt32(0)),
            topic: reader.GetString(1),
            description: reader.GetString(2),
             state: (TaskState)reader.GetInt32(3),
             start: reader.GetDateTime(4),
            finish: reader.GetDateTime(5),
             deadline: reader.GetDateTime(6),
             performer: performer,
            owner: owner);
        }

        public Person BuildPerson(OleDbDataReader reader)
        {
            reader.Read();
            var id = Int64.Parse(reader.GetString(0));
            var taken = reader.GetString(1).Split(',')
                .Where(s => s != "").Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList();
            var done = reader.GetString(2).Split(',')
                .Where(s => s != "").Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList();
            var owned = reader.GetString(3).Split(',')
                .Where(s => s != "").Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList();
            var name = reader.GetString(4);
            return new Person(id, taken, done, owned);
        }

        public Person BuildPartialPerson(OleDbDataReader reader)
        {
            reader.Read();
            var id = Int64.Parse(reader.GetString(0));
            var name = reader.GetString(4);
            return new Person(id, name);
        }

        public Team BuildTeam(OleDbDataReader reader)
        {
            reader.Read();
            return new Team(id: Int64.Parse(reader.GetString(0)),
                persons: reader.GetString(1).Split(',').Select(pid => db.GetPerson(Convert.ToInt32(pid))).ToList(),
                ownedTasks: reader.GetString(2).Split(',').Select(tid => db.GetTask(Convert.ToInt32(tid))).ToList(),
                name: reader.GetString(3));
        }

        public Team BuildPartialTeam(OleDbDataReader reader)
        {
            reader.Read();
            return new Team(id: Int64.Parse(reader.GetString(0)),
                name: reader.GetString(3));
        }

        public List<ITask> BuildTasksList(OleDbDataReader reader)
        {
            var tasksList = new List<ITask>();
            while (reader.Read())
            {
                tasksList.Add(new SimpleTask(
            id: reader.GetInt32(0),
            topic: reader.GetString(1),
            description: reader.GetString(2),
            state: (TaskState)reader.GetInt32(3),
            start: reader.GetDateTime(4),
            finish: reader.GetDateTime(5),
            deadline: reader.GetDateTime(6),
            owner: db.GetOwner(Int64.Parse(reader.GetString(8))),
            performer: db.GetPerson(Int64.Parse(reader.GetString(7)))));
            }
            return tasksList;
        }
    }
}
