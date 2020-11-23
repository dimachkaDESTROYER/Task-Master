using System;
using System.Collections.Generic;
using System.Text;
using TaskMaster.Domain;

namespace TaskMaster
{
    public class Builder
    {
        private readonly DataBase db;

        public Builder(DataBase dataBase)
        {
            db = dataBase;
        }
        public SimpleTask BuildSimpleTask(int ID, string topic, string description,
            TaskState state, DateTime start, DateTime finish, DateTime deadLine,
            int performerID, int ownerID)
        {
            var owner = db.GetOwner(ownerID);
            var performer = db.GetOwner(performerID);
            return new SimpleTask(ID, owner, performer, topic, description,
             state, start, finish, deadLine);
        }
    }
}
