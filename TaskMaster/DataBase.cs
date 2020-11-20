using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using TaskMaster.Domain;

namespace TaskMaster
{
    class DataBase
    {
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;
Data Source=Database.mdb";//руками добавлена в debug. Переделать.
        public OleDbConnection myConnection;

        public DataBase()
        {
            myConnection = new OleDbConnection(connectionString);
        }
        public void Open() => myConnection.Open();
        public void Close() => myConnection.Close();

        public void Clear()
        {
            var command = new OleDbCommand("DELETE * FROM Task ", myConnection);
            Open();
            command.ExecuteNonQuery();
            Close();
        }

        public void DeleteTask(int taskID)
        {
            var query = string.Format("DELETE * FROM Task WHERE id = {0}", taskID);
            var command = new OleDbCommand(query, myConnection);
            Open();
            command.ExecuteNonQuery();
            Close();
        }

        public void Add(int taskId, string topic, int taskState)
        {
            Open();
            var values = "VALUES ({0},'{1}','{2}', '{3}', '{4}', '{5}', '{6}', {7}, {8})";
            var columnNames = "Task(ID, Topic, Description, State, Start, Finish, DeadLine, PerformerID, OwnerID";
            var query = string.Format("INSERT INTO " + columnNames + " ) " + values,
                taskId, topic, "deskriptuon", taskState.ToString() + "state", DateTime.Now, DateTime.Now, DateTime.Now, 3, 4);
            var command = new OleDbCommand(query, myConnection);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (OleDbException)
            {
                Close();
                throw new InvalidOperationException("ID is already used");
            }
            Close();
        }

        //public SimpleTask GetTask(int id)
        //{
        //    var query = string.Format("SELECT * FROM Task WHERE id = {0}", id);
        //    Open();
        //    var command = new OleDbCommand(query, myConnection);
        //    var reader = command.ExecuteReader();
        //    if (!reader.HasRows)
        //    {
        //        reader.Close();
        //        Close();
        //        throw new ArgumentException("ID not found");
        //    }
        //    reader.Close();
        //    Close();
        //    return new SimpleTask(//...arguments..reader.GetInt32(0).........//);
        //}

        //public void ChangeTask(ITask changedTask)
        //{
        //    DeleteTask(changedTask.ID);
        //    Add(changedTask);//наверное...... это не очень производительно
        //}


        //public IEnumerable<ITask> GetAllTasksOwnedBy(int ownerID)
        //{
        //    var query = string.Format("SELECT * FROM Task WHERE OwnerID = {0}", ownerID);
        //    Open();
        //    var command = new OleDbCommand(query, myConnection);
        //    var reader = command.ExecuteReader();
        //    if (!reader.HasRows)
        //    {
        //        reader.Close();
        //        Close();
        //        throw new ArgumentException("ID not found");
        //    }
        //    while (reader.Read())
        //    {
        //        yield return new SimpleTask(//...arguments..reader.GetInt32(0)...reader[0].ToString()......//)
        //    }
        //    reader.Close();
        //    Close();
        //}
    }
}
