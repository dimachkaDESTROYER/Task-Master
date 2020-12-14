using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using TaskMaster.Domain;
using TaskMaster.Domain.Tasks;

namespace TaskMaster.DataBaseFolder
{
    public partial class DataBase : IDataBase
    {
        private ConnectionAPI connectionAPI;
        private ReaderAPI readerAPI;
        public DataBase()
        {
            connectionAPI = new ConnectionAPI();
            readerAPI = new ReaderAPI();
        }

        public void ChangeTask(ITask changedTask)
        {
            DeleteTask(changedTask.Id);
            AddTask(changedTask);//наверное...... это не очень производительно
        }

        public void ChangePerson(Person changed)
        {
            DeletePerson(changed.Id);
            AddPerson(changed);
        }

        public void ChangeTeam(Team changed)
        {
            DeleteTeam(changed.Id);
            AddTeam(changed);
        }

        public void CleanTasks()//эти 2 метода можно обобщить
        {
            var query = "DELETE * FROM Task";
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
            }
        }

        public void Clean()
        {
            var queryTask = "DELETE * FROM Task";
            var queryBranchedTask = "DELETE * FROM BranchedTask";
            var queryPerson = "DELETE * FROM Person";
            var queryTeam = "DELETE * FROM Team";
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(queryTask).ExecuteNonQuery();
                connectionAPI.GetCommand(queryBranchedTask).ExecuteNonQuery();
                connectionAPI.GetCommand(queryPerson).ExecuteNonQuery();
                connectionAPI.GetCommand(queryTeam).ExecuteNonQuery();
            }
        }

        public void DeletePerson(long personID)
        {
            var query = string.Format("DELETE * FROM Person WHERE id = '{0}'", personID);
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
            }
        }

        public void DeleteTeam(long teamID)
        {
            var query = string.Format("DELETE * FROM Team WHERE id = '{0}'", teamID);
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskID)
        {
            var query = string.Format("DELETE * FROM Task WHERE id = {0}", taskID);
            var queryBranched = string.Format("DELETE * FROM BranchedTask WHERE id = {0}", taskID);
            using (connectionAPI.Open())
            {
                connectionAPI.GetCommand(query).ExecuteNonQuery();
                connectionAPI.GetCommand(queryBranched).ExecuteNonQuery();
            }
        }

        public bool ContainsPerson(long personID)
        {
            var query = string.Format("SELECT * FROM Person WHERE ID = '{0}'", personID);
            using (connectionAPI.Open())
            using (var reader = readerAPI.Open(connectionAPI, query))
            { return !reader.IsEmpty; }
        }

        public bool ContainsTeam(long teamID)
        {
            var query = string.Format("SELECT * FROM Team WHERE ID = '{0}'", teamID);
            using (connectionAPI.Open())
            using (var reader = readerAPI.Open(connectionAPI, query))
            { return !reader.IsEmpty; }
        }

        public bool ContainsSimpleTask(int id)
        {
            var query = string.Format("SELECT * FROM Task WHERE ID = {0}", id);
            using (connectionAPI.Open())
            using (var reader = readerAPI.Open(connectionAPI, query))
            { return !reader.IsEmpty; }
        }
        public bool ContainsBranchedTask(int id)
        {
            var query = string.Format("SELECT * FROM BranchedTask WHERE ID = {0}", id);
            using (connectionAPI.Open())
            using (var reader = readerAPI.Open(connectionAPI, query))
            { return !reader.IsEmpty; }
        }

        private string ToStr(ICollection<ITask> tasks) => string.Join(',', tasks.ToList().Select(t => t.Id.ToString()));
        private string ToStr(ICollection<Person> persons) => string.Join(',', persons.ToList().Select(p => p.Id.ToString()));

        public void AddPerson(Person person)
        {
            if (ContainsPerson(person.Id))
                throw new ArgumentException("ID is already used");
            using (connectionAPI.Open())
            {
                var values = "VALUES ('{0}','{1}','{2}','{3}','{4}')";
                var columnNames = "Person(ID, TakenTasks, DoneTasks, OwnedTasks, PersonName )";
                var query = string.Format("INSERT INTO " + columnNames + values,
                    person.Id, ToStr(person.TakenTasks), ToStr(person.DoneTasks), ToStr(person.OwnedTasks), person.Name);
                var command = new OleDbCommand(query, connectionAPI.connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddTeam(Team team)
        {
            if (ContainsTeam(team.Id))
                throw new ArgumentException("ID is already used");
            using (connectionAPI.Open())
            {
                var values = "VALUES ('{0}','{1}','{2}','{3}')";
                var columnNames = "Team(ID, PersonsIDs, OwnedTasksIDs, TeamName )";
                var query = string.Format("INSERT INTO " + columnNames + values,
                    team.Id, ToStr(team.Persons), ToStr(team.OwnedTasks), team.Name);
                var command = new OleDbCommand(query, connectionAPI.connection);
                command.ExecuteNonQuery();
            }
        }

        public void AddTask(ITask task)
        {
            if (task is BranchedTask)
            {
                AddBranchedTask((BranchedTask)task);
                return;
            }
            if (task is SimpleTask)
                AddSimpleTask((SimpleTask)task);

        }

        private void AddBranchedTask(BranchedTask task)
        {
            using (connectionAPI.Open())
            {
                var values = "VALUES ({0},'{1}','{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')";
                var columnNames = "BranchedTask(ID, Topic, Description, State, Start, Finish, DeadLine, PerformerID, OwnerID, SubTasks";
                var query = string.Format("INSERT INTO " + columnNames + " ) " + values,
                    task.Id, task.Topic, task.Description, (int)task.State,
                    task.Start,
                    task.Finish == null ? DateTime.MaxValue : task.Finish,
                    task.DeadLine,
                    task.Performer == null ? "" : task.Performer.Id.ToString(),
                    task.Owner.Id,
                    ToStr(task.SubTasks));
                var command = new OleDbCommand(query, connectionAPI.connection);
                command.ExecuteNonQuery();
            }
        }

        private void AddSimpleTask(SimpleTask task)
        {
            using (connectionAPI.Open())
            {
                var values = "VALUES ({0},'{1}','{2}', {3}, '{4}', '{5}', '{6}', '{7}', '{8}')";
                var columnNames = "Task(ID, Topic, Description, State, Start, Finish, DeadLine, PerformerID, OwnerID";
                var query = string.Format("INSERT INTO " + columnNames + " ) " + values,
                    task.Id, task.Topic, task.Description, (int)task.State,
                    task.Start, task.Finish == null ? DateTime.MaxValue : task.Finish,
                    task.DeadLine, task.Performer == null ? "" : task.Performer.Id.ToString(),
                    task.Owner.Id);
                var command = new OleDbCommand(query, connectionAPI.connection);
                command.ExecuteNonQuery();
            }
        }

        private object GetByQuery(string query, Func<OleDbDataReader, object> buildAnObject)
        {
            using (connectionAPI.Open())
            using (var reader = readerAPI.Open(connectionAPI, query))
            {
                if (reader.IsEmpty)
                    throw new ArgumentException("ID not found");
                return buildAnObject(reader.reader);
            }
        }

        public ITask GetTask(int taskId)
        {
            if (ContainsSimpleTask(taskId))
                return GetSimpleTask(taskId);
            if (ContainsBranchedTask(taskId))
                return GetBranchedTask(taskId);
            throw new ArgumentException("ID is not found");
        }

        public BranchedTask GetBranchedTask(int taskId)
        {
            var query = string.Format("SELECT * FROM BranchedTask WHERE ID = {0}", taskId);
            var builder = new Builder(this);
            return (BranchedTask)GetByQuery(query, builder.BuildBraanchedTask);
        }

        public SimpleTask GetSimpleTask(int taskId)
        {
            var query = string.Format("SELECT * FROM Task WHERE ID = {0}", taskId);
            var builder = new Builder(this);
            return (SimpleTask)GetByQuery(query, builder.BuildSimpleTask);
        }

        public Person GetPerson(long ID)
        {
            var query = string.Format("SELECT * FROM Person WHERE ID = '{0}'", ID);
            var builder = new Builder(this);
            return (Person)GetByQuery(query, builder.BuildPerson);
        }

        public Person GetPartialPerson(long ID)
        {
            var query = string.Format("SELECT * FROM Person WHERE ID = '{0}'", ID);
            var builder = new Builder(this);
            return (Person)GetByQuery(query, builder.BuildPartialPerson);
        }

        public Team GetTeam(long ID)
        {
            var query = string.Format("SELECT * FROM Team WHERE ID = '{0}'", ID);
            var builder = new Builder(this);
            return (Team)GetByQuery(query, builder.BuildTeam);
        }

        private Team GetPartialTeam(long ID)
        {
            var query = string.Format("SELECT * FROM Team WHERE ID = '{0}'", ID);
            var builder = new Builder(this);
            return (Team)GetByQuery(query, builder.BuildPartialTeam);
        }

        public List<ITask> GetAllTasksOwnedBy(IOwner owner) /* не только SimpleTask*/
        {
            var query = string.Format("SELECT * FROM Task WHERE OwnerID = '{0}'", owner.Id);
            var builder = new Builder(this);
            return (List<ITask>)GetByQuery(query, builder.BuildTasksList);
        }

        public List<ITask> GetAllTasksPerformedBy(IPerformer performer)
        {
            var query = string.Format("SELECT * FROM Task WHERE PerformerID = '{0}'", performer.Id);
            var builder = new Builder(this);
            return (List<ITask>)GetByQuery(query, builder.BuildTasksList);
        }

        public IOwner GetOwner(long ownerId)
        {
            try { return GetPerson(ownerId); }
            catch (ArgumentException) {; }
            return GetTeam(ownerId);
        }

        public IOwner GetPartialOwner(long ownerId)
        {
            try { return GetPartialPerson(ownerId); }
            catch (ArgumentException) {; }
            return GetPartialTeam(ownerId);
        }

        public IPerformer GetPartialPerformer(long perfId)
        {
            return GetPartialPerson(perfId);
        }
    }
}
