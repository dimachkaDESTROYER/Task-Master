using System;
using System.Data.OleDb;
using System.IO;

namespace TaskMasterBot.DataBaseFolder
{
    public partial class DataBase
    {
        public class ConnectionAPI : IDisposable
        {
            public OleDbConnection connection;//лучше не public
            private bool disposedValue;
            private int connectionUsersCount;

            public ConnectionAPI Open()
            {
                if (connectionUsersCount == 0)
                    connection.Open();
                connectionUsersCount++;
                return this;
            }

            public ConnectionAPI()
            {
                var currentDir = Directory.GetCurrentDirectory();
                var path = Directory.GetParent(currentDir).Parent.Parent.Parent.FullName
                    + @"\TaskMaster\DataBaseFolder\MyDataBase.mdb";
                //throw new ArgumentException(path);
                connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;
Data Source=" + path);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if (connectionUsersCount == 1)
                            connection.Close();
                        connectionUsersCount--;
                    }

                    // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                    // TODO: установить значение NULL для больших полей
                    disposedValue = true;
                }
            }

            public OleDbCommand GetCommand(string query)
            {
                return new OleDbCommand(query, connection);
            }

            public void Dispose()
            {
                // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}