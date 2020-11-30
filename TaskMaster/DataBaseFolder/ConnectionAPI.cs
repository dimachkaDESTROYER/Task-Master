using System;
using System.Data.OleDb;

namespace TaskMaster.DataBaseFolder
{
    public partial class DataBase
    {
        public class ConnectionAPI : IDisposable
        {
            public OleDbConnection connection;//лучше не public
            private bool disposedValue;

            public ConnectionAPI Open()
            {
                connection.Open();//а если оно уже открыто?
                return this;
            }

            public ConnectionAPI()
            {
                connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;
Data Source=Database.mdb");
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        connection.Close();
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