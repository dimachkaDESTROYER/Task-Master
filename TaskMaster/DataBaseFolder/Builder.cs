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
            return FillSimpleTask(reader);
        }

        private SimpleTask FillSimpleTask(OleDbDataReader reader)
        {
            var id = reader.GetInt32(0);
            var topic = reader.GetString(1);
            var description = reader.GetString(2);
            var state = (TaskState)reader.GetInt32(3);
            var start = reader.GetDateTime(4);
            var readedFin = reader.GetDateTime(5);
            var deadline = reader.GetDateTime(6);
            var owner = db.GetPartialOwner(Int64.Parse(reader.GetString(8)));
            var readedPerf = reader.GetString(7);
            Person performer = readedPerf == "" ? null : db.GetPartialPerson(Int64.Parse(readedPerf));

            if (readedFin == DateTime.MaxValue)
                return new SimpleTask(id, topic, description, state, start, null, deadline, owner, performer);
            else
                return new SimpleTask(id, topic, description, state, start, readedFin, deadline, owner, performer);
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
            return new Person(id, taken, done, owned, name);
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
                tasksList.Add(FillSimpleTask(reader));
            }
            return tasksList;
        }
    }
}
