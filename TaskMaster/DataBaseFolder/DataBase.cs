using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using TaskMaster.Domain;

namespace TaskMaster.DataBaseFolder
{
    public partial class DataBase
    {
        private ConnectionAPI connectionAPI;
        private ReaderAPI readerAPI;

        public DataBase()
        {
            connectionAPI = new ConnectionAPI();
            readerAPI = new ReaderAPI();
        }
        public void ChangeTask(SimpleTask changedTask)
        {
            DeleteTask(changedTask.Id);
            AddTask(changedTask);//наверное...... это не очень производительно
        }

        public void CleanTasks()//эти 2 метода можно обобщить
        {
            var query = "DELETE * FROM Task";
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
            }
        }

        public void DeleteTask(ulong taskID)
        {
            var query = string.Format("DELETE * FROM Task WHERE id = {0}", taskID);
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
            }
        }

        public void AddTask(ITask task)
        {
            if (task is SimpleTask)
                AddSimpleTask((SimpleTask)task);
        }

        private void AddSimpleTask(SimpleTask task)
        {
            using (connectionAPI.Open())
            {
                var values = "VALUES ({0},'{1}','{2}', {3}, '{4}', '{5}', '{6}', {7}, {8})";
                var columnNames = "Task(ID, Topic, Description, State, Start, Finish, DeadLine, PerformerID, OwnerID";
                var query = string.Format("INSERT INTO " + columnNames + " ) " + values,
                    task.Id, task.Topic, task.Description, (int)task.State,
                    task.Start, task.Finish, task.DeadLine, task.Performer.Id,
                    task.Owner.Id);
                var command = new OleDbCommand(query, connectionAPI.connection);
                try { command.ExecuteNonQuery(); }
                catch (OleDbException)
                { throw new ArgumentException("ID is already used"); }
            }
        }
        //private void AddTask(TreeTask task)
        //{
        //    ;
        //}


        //private TreeTask GetTreeTask(int taskId)
        //{
        //    ;
        //}

        private Builder GetByQuery(string query, Func<OleDbDataReader, Builder> buildAnObject)
        {
            using (connectionAPI.Open())
            {
                using (var reader = readerAPI.Open(connectionAPI, query))
                {
                    if (reader.IsEmpty)
                        throw new ArgumentException("ID not found");
                    return buildAnObject(reader.reader);
                }
            }
        }

        public ITask GetTask(int taskId)
        {
            return GetSimpleTask(taskId);
            //пока что так, при расширении написать запрос по нескольким таблицам
        }

        public SimpleTask GetSimpleTask(int taskId)
        {
            var query = string.Format("SELECT * FROM Task WHERE id = {0}", taskId);
            var builder = new Builder(this);
            return GetByQuery(query, builder.PrepareBuildingSimpleTask).BuildSimpleTask();
        }

        public Person GetPerson(int ID)
        {
            var query = string.Format("SELECT * FROM Person WHERE ID = {0}", ID);
            var builder = new Builder(this);
            return GetByQuery(query, builder.PrepareBuildingPerson).BuildPerson();
        }
        private Team GetTeam(int ID)
        {
            var query = string.Format("SELECT * FROM Team WHERE ID = {0}", ID);
            var builder = new Builder(this);
            return GetByQuery(query, builder.PrepareBuildingTeam).BuildTeam();
        }

        public List<ITask> GetAllTasksOwnedBy(IOwner owner)
        //при расширении написать запрос по нескольким таблицам
        {
            var query = string.Format("SELECT * FROM Task WHERE OwnerID = {0}", owner.Id);
            var builder = new Builder(this);
            return GetByQuery(query, builder.PrepareBuildingTasksList).BuildTasksList();
        }

        public IEnumerable<ITask> GetAllTasksPerformedBy(IPerformer performer)
        {
            var query = string.Format("SELECT * FROM Task WHERE PerformerID = {0}", performer.Id);
            var builder = new Builder(this);
            return GetByQuery(query, builder.PrepareBuildingTasksList).BuildTasksList();
        }

        public IOwner GetOwner(int ownerId)
        {
            try { return GetPerson(ownerId); }
            catch (ArgumentException) {; }
            return GetTeam(ownerId);
        }
    }
}
