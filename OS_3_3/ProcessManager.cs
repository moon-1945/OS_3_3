using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace OS_3_3
{
    internal class ProcessManager : IEnumerable<Process>,INotifyCollectionChanged,IDisposable
    {
        private readonly List<Process> processes = new(4);
        private bool disposedValue;
        private bool IsUpdateThreadRunning = true;
        private const int UPDATE_TIMEOUT = 1000;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public ProcessManager() 
        {
            ThreadStart threadStart = new (UpDatingThread);
            Thread thread = new (threadStart);
            thread.Start();
        }

        public IEnumerator<Process> GetEnumerator()
        {
            return processes.GetEnumerator();
        }

        public void CreateNew(string comand)
        {
            Process pr = new (comand);
            processes.Add(pr);
            pr.Start();
        }

        private void UpDatingThread()
        {
            while(IsUpdateThreadRunning)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() => CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
                Thread.Sleep(UPDATE_TIMEOUT);
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
                
                IsUpdateThreadRunning = false;
                foreach (var process in processes)
                {
                    process.Dispose();
                }
              
                disposedValue = true;
            }
        }

       
        ~ProcessManager()
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
