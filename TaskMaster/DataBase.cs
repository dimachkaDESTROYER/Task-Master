using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using TaskMaster.Domain;
using System.Linq;

namespace TaskMaster
{
    public class DataBase
    {
        private static string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;
Data Source=Database.mdb";//руками добавлена в debug. Переделать.
        public OleDbConnection myConnection;
        private Builder builder;

        public DataBase()
        {
            myConnection = new OleDbConnection(connectionString);
            builder = new Builder(this);
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
        //закомментировано так как не хватает некотрых полей и конструкторов модели
        //public void AddTask(ITask task)
        //{
        //    Open();
        //    var values = "VALUES ({0},'{1}','{2}', {3}, '{4}', '{5}', '{6}', {7}, {8})";
        //    var columnNames = "Task(ID, Topic, Description, State, Start, Finish, DeadLine, PerformerID, OwnerID";
        //    var query = string.Format("INSERT INTO " + columnNames + " ) " + values,
        //        task.ID,
        //        task.Topic,
        //        task.Description,
        //        (int)task.State,
        //        task.Start,
        //        task.Finish,
        //        task.DeadLine,
        //        task.Performer.Id,
        //        task.Owner.Id);
        //    var command = new OleDbCommand(query, myConnection);
        //    try
        //    {
        //        command.ExecuteNonQuery();
        //    }
        //    catch (OleDbException)
        //    {
        //        Close();
        //        throw new InvalidOperationException("ID is already used");
        //    }
        //    Close();
        //}

        //public SimpleTask GetTask(int taskId)
        //{
        //    var query = string.Format("SELECT * FROM Task WHERE id = {0}", taskId);
        //    Open();
        //    var command = new OleDbCommand(query, myConnection);
        //    var reader = command.ExecuteReader();
        //    if (!reader.HasRows)
        //    {
        //        reader.Close();
        //        Close();
        //        throw new ArgumentException("ID not found");
        //    }

        //    var res = builder.BuildSimpleTask(taskId, reader.GetString(1),
        //        reader.GetString(2), (TaskState)reader.GetInt32(3), reader.GetDateTime(4),
        //        reader.GetDateTime(5), reader.GetDateTime(6), reader.GetInt32(7), reader.GetInt32(8));

        //    reader.Close();
        //    Close();
        //    return res;
        //}

        //public void ChangeTask(ITask changedTask)
        //{
        //    DeleteTask(changedTask.ID);
        //    AddTask(changedTask);//наверное...... это не очень производительно
        //}


        //public IEnumerable<ITask> GetAllTasksOwnedBy(IOwner owner)
        //{
        //    var query = string.Format("SELECT * FROM Task WHERE OwnerID = {0}", owner.Id);
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
        //        yield return new SimpleTask(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
        //            (TaskState)reader.GetInt32(3), reader.GetDateTime(4),
        //        reader.GetDateTime(5), reader.GetDateTime(6), GetOwner(reader.GetInt32(7)), owner);
        //    }
        //    reader.Close();
        //    Close();
        //}

        //public IEnumerable<ITask> GetAllTasksPerformedBy(IPerformer performer)
        //{
        //    var query = string.Format("SELECT * FROM Task WHERE PerformerID = {0}", performer.Id);
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
        //        yield return new SimpleTask(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
        //            (TaskState)reader.GetInt32(3), reader.GetDateTime(4),
        //        reader.GetDateTime(5), reader.GetDateTime(6), performer, GetOwner(reader.GetInt32(8)));
        //    }
        //    reader.Close();
        //    Close();
        //}

        //public IOwner GetOwner(int ownerId)
        //{
        //    var query = string.Format("SELECT * FROM Person WHERE ID = {0}", ownerId);
        //    //+ ещё запрос по коммандам
        //    Open();
        //    var command = new OleDbCommand(query, myConnection);
        //    var reader = command.ExecuteReader();
        //    if (reader.HasRows)
        //    {
        //        var takenTasks = reader.GetString(1).Split(',').Select(tid => GetTask(Convert.ToInt32(tid)));
        //        var doneTasks = reader.GetString(2).Split(',').Select(tid => GetTask(Convert.ToInt32(tid)));
        //        var ownedTasks = reader.GetString(3).Split(',').Select(tid => GetTask(Convert.ToInt32(tid)));
        //        reader.Close();
        //        Close();
        //        return new Person(ownerId, takenTasks, doneTasks, ownedTasks);
        //    }
        //    query = string.Format("SELECT * FROM Team WHERE ID = {0}", ownerId);
        //    command = new OleDbCommand(query, myConnection);
        //    reader = command.ExecuteReader();
        //    if (!reader.HasRows)
        //    {
        //        reader.Close();
        //        Close();
        //        throw new ArgumentException("ID not found");
        //    }
        //    var ownedTasks=reader.GetString....
        //    var teamMates=reader.GetString....
        //    reader.Close();
        //    Close();
        //    return new Team(ownerId, ownedTasks, teamMates);
        //}
    }
}
