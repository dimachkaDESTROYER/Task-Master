using System;
using System.Data.OleDb;
using static TaskMasterBot.DataBaseFolder.DataBase;

namespace TaskMasterBot.DataBaseFolder
{
    public class ReaderAPI : IDisposable
    {
        private bool disposedValue;
        public OleDbDataReader reader { get; private set; }
        public bool IsEmpty => !reader.HasRows;

        public ReaderAPI Open(ConnectionAPI connectionAPI, string query)
        {
            reader = connectionAPI
                .GetCommand(query)
                .ExecuteReader();
            return this;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    reader.Close();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~ReaderAPI()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
