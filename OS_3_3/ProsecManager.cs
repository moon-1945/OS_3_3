using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OS_3_3
{
    internal class ProsecManager : IEnumerable<Process>,INotifyCollectionChanged,IDisposable
    {
        private readonly List<Process> processes = new(4);
        private bool disposedValue;
        private bool IsUpdateThreadRunning = true;
        private const int UPDATE_TIMEOUT = 100;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public ProsecManager() 
        {
            ThreadStart threadStart = new ThreadStart(UpDatingThread);
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        public IEnumerator<Process> GetEnumerator()
        {
            return processes.GetEnumerator();
        }

        private void UpDatingThread()
        {
            while(IsUpdateThreadRunning)
            {
                GetUpdatedProcessInfo();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                Thread.Sleep(UPDATE_TIMEOUT);
            }
        }

        private void GetUpdatedProcessInfo()
        {
            foreach (var process in processes)
            {
                process.UpdateInfo();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }
                
                IsUpdateThreadRunning = false;

                foreach (var process in processes)
                {
                    process.Dispose();
                }
                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        ~ProsecManager()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
