using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static OS_3_3.WindowsApi;



namespace OS_3_3
{
    public class Process
    {
        private string? _commandLine = null;
        private IntPtr _handle = IntPtr.Zero;

        //    private IntPtr _threadHandle; //???

        public uint Id { get; private set; }

        public string Name { get; private set; }

        //public string Priority
        //{
        //    get
        //    {
        //        return GetPriority() switch
        //        {
        //            ProcessPriorityClass.ABOVE_NORMAL => "Above Normal",//TODO
        //            ProcessPriorityClass.BELOW_NORMAL => "Below Normal",
        //            ProcessPriorityClass.HIGH => "High",
        //            ProcessPriorityClass.IDLE => "Idle",
        //            ProcessPriorityClass.NORMAL => "Normal",
        //            ProcessPriorityClass.REALTIME => "Realtime",
        //            _ => throw new NotImplementedException(),
        //        };
        //    }

        //    set
        //    {
        //        ProcessPriorityClass priority = (value) switch
        //        {
        //            "Realtime" => ProcessPriorityClass.REALTIME,
        //            "High" => ProcessPriorityClass.HIGH,
        //            "Above Normal" => ProcessPriorityClass.ABOVE_NORMAL,
        //            "Normal" => ProcessPriorityClass.NORMAL,
        //            "Below Normal" => ProcessPriorityClass.BELOW_NORMAL,
        //            "Idle" => ProcessPriorityClass.IDLE,
        //            _ => throw new NotImplementedException(),
        //        }; 
        //    }
        //}


        public ProcessPriorityClass Priority
        {
            get
            {
                return GetPriority();
            }

            set
            {
                SetPriority(value);
            }
        }

        public string AffinityMask
        {
            get
            {
                ulong mask = GetAffinityMask();

                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < 64; i++)
                {
                    if ((mask & (1ul << i)) != 0)
                    {
                        builder.Append(i);
                        builder.Append(' ');
                    }
                }

                return builder.ToString();
            }
        }
        public bool IsTerminated => (WaitForSingleObject(_handle, 0) == 0) ? true : false;

        //private int _num = 0;

        //private uint _mainThreadId = 0;

        //public bool IsSuspended
        //{
        //    get
        //    {
        //        uint id = 0;
        //        if (_num == 0)
        //        {
        //            id = GetMainThreadId();
        //            _mainThreadId = id;
        //        }
        //        else
        //        {
        //            id = _mainThreadId;
        //        }

        //        if (id == 0) return false;

        //        IntPtr handle = OpenThread(ThreadAccessFlags.ALL_ACCESS, false, id);

        //        if (ResumeThread(handle) > 0)
        //        {
        //            SuspendThread(handle);

        //            return true;
        //        }

        //        return false;
        //    }
        //}

        //public bool IsRunning
        //{
        //    get
        //    {
        //        if (!GetExitCodeProcess(_handle, out uint code)) throw new InvalidOperationException();

        //        if (IsSuspended) return false; // незн без цього не працює норм ...

        //        return code == 259;
        //    }
        //}

        //public string Status
        //{
        //    get
        //    {
        //        if (IsTerminated) return "Terminated";
        //        if (IsSuspended) return "Suspended";
        //        return "Running";
        //    }
        //}

        public Process(string commandLine)
        {
            _commandLine = commandLine;
        }

        public bool Start()
        {
            if (_commandLine == null)
            {
                throw new Exception("Process haven't info");
            }

            STARTUPINFO startupInfo = new STARTUPINFO();
            PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();

            startupInfo.cb = 104;

            bool isSuccessful = CreateProcess(null,
            _commandLine,
            IntPtr.Zero,
            IntPtr.Zero,
            false,
            0,
            IntPtr.Zero,
            null,
            ref startupInfo,
            out processInfo);

            _handle = processInfo.hProcess;

            Id = processInfo.dwProcessId;

            CloseHandle(processInfo.hThread);

            StringBuilder lpFileName = new StringBuilder((int)MAX_PATH); // 260 - MAX_PATH

            K32GetModuleFileNameExW(_handle, IntPtr.Zero, lpFileName, MAX_PATH);

            Name = Path.GetFileName(lpFileName.ToString());

            return isSuccessful;
        }

        public static Process? Start(string commandLine)
        {
            var process = new Process(commandLine);

            return process.Start() ? process : null;
        }

        public void WaitToEnd()
        {
            WaitForSingleObject(_handle, INFINITY);
        }

        public void Close()
        {
            CloseHandle(_handle);
        }

        private uint[] GetThreadIDs()
        {
            List<uint> ids = new List<uint>();

            IntPtr hThreadSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);

            if (hThreadSnapshot != IntPtr.Zero)
            {
                THREADENTRY32 threadEntry = new THREADENTRY32();
                threadEntry.dwSize = (uint)Marshal.SizeOf(typeof(THREADENTRY32));

                if (Thread32First(hThreadSnapshot, ref threadEntry))
                {
                    do
                    {
                        if (threadEntry.th32OwnerProcessID == Id)
                        {
                            ids.Add(threadEntry.th32ThreadID);
                        }
                    } while (Thread32Next(hThreadSnapshot, ref threadEntry));
                }

                CloseHandle(hThreadSnapshot);
            }
            else
            {
                Console.WriteLine("CreateToolhelp32Snapshot failed. Error code: " + Marshal.GetLastWin32Error());
            }

            return ids.ToArray();
        }

        public static uint[] GetProcessesIdByName(string name)
        {
            List<uint> ids = new List<uint>();

            PROCESSENTRY32 entry = new PROCESSENTRY32();
            entry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

            IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

            if (Process32First(snapshot, ref entry))
            {
                do
                {
                    if (Path.GetFileName(entry.szExeFile) == name || entry.szExeFile == name)
                    {
                        ids.Add(entry.th32ProcessID);
                    }
                } while (Process32Next(snapshot, ref entry));
            }

            CloseHandle(snapshot);

            return ids.ToArray();
        }


        public static Process? GetProcessById(uint id)
        {
            IntPtr handle = OpenProcess(ProcessAccessFlags.ALL_ACCESS, false, id);

            if (handle == IntPtr.Zero) return null;

            StringBuilder lpFileName = new StringBuilder(260); // 260 - MAX_PATH

            K32GetModuleFileNameExW(handle, IntPtr.Zero, lpFileName, 260);

            string path = lpFileName.ToString();

            Process? process = new Process(path)
            {
                Name = Path.GetFileName(path),
                Id = id,
            };

            process._handle = handle;

            return process;
        }

        public ulong GetAffinityMask()
        {
            if (!GetProcessAffinityMask(_handle, out UIntPtr processAffinityMask, out UIntPtr systemAffinityMask))
            {
                throw new Exception();
            }

            return processAffinityMask.ToUInt64();
        }

        public static ulong GetAffinityMask(uint id)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            return process.GetAffinityMask();
        }

        public void SetAffinityMask(ulong affinityMask)
        {
            if (!SetProcessAffinityMask(_handle, new UIntPtr(affinityMask)))
            {
                throw new Exception();
            }
        }

        public static void SetAffinityMask(uint id, ulong affinityMask)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            process.SetAffinityMask(affinityMask);
        }

        public ProcessPriorityClass GetPriority()
        {
            return GetPriorityClass(_handle);
        }

        public static ProcessPriorityClass GetPriority(uint id)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            return process.GetPriority();
        }

        public void SetPriority(ProcessPriorityClass priorityClass)
        {
            if (!SetPriorityClass(_handle, priorityClass))
            {
                throw new Exception();
            }
        }

        public static void SetPriority(uint id, ProcessPriorityClass priorityClass)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            process.SetPriority(priorityClass);
        }

        public void Suspend()
        {
          //  if (IsSuspended) return;

            uint[] threadIds = GetThreadIDs();

            for (int i = 0; i < threadIds.Length; i++)
            {
                IntPtr handle = OpenThread(ThreadAccessFlags.ALL_ACCESS, false, threadIds[i]);

                if (handle != IntPtr.Zero)
                {
                    SuspendThread(handle);
                }

                CloseHandle(handle);
            }
        }

        public static void Suspend(uint id)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            process.Suspend();
        }

        public static void Suspend(string name)
        {
            uint[] ids = GetProcessesIdByName(name);

            for (int i = 0; i < ids.Length; i++)
            {
                Suspend(ids[i]);
            }
        }

        public void Resume()
        {
            uint[] threadIds = GetThreadIDs();

            for (int i = 0; i < threadIds.Length; i++)
            {
                IntPtr handle = OpenThread(ThreadAccessFlags.ALL_ACCESS, false, threadIds[i]);

                if (handle != IntPtr.Zero)
                {
                    ResumeThread(handle);
                }

                CloseHandle(handle);
            }
        }

        public static void Resume(uint id)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            process.Resume();
        }

        public static void Resume(string name)
        {
            uint[] ids = GetProcessesIdByName(name);

            for (int i = 0; i < ids.Length; i++)
            {
                Resume(ids[i]);
            }
        }

        public void Kill()
        {
            TerminateProcess(_handle, 9);
            //close handle???
        }

        public static void Kill(uint id)
        {
            Process? process = GetProcessById(id);

            if (process == null) throw new Exception();

            process.Kill();
        }

        public static void Kill(string name)
        {
            uint[] ids = GetProcessesIdByName(name);

            for (int i = 0; i < ids.Length; i++)
            {
                Kill(ids[i]);
            }
        }

        public void GetTimes(
       out DateTime creationTime,
       out DateTime exitTime,
       out TimeSpan kernelTime,
       out TimeSpan userTime)
        {
            GetProcessTimes(_handle,
                out FILETIME fileCreatingTime,
                out FILETIME fileExitTime,
                out FILETIME fileKernelTime,
                out FILETIME fileUserTime);

            FileTimeToSystemTime(ref fileCreatingTime, out SYSTEMTIME systemCreatingTime);
            FileTimeToSystemTime(ref fileExitTime, out SYSTEMTIME systemExitTime);

            creationTime = new DateTime(
                systemCreatingTime.wYear,
                systemCreatingTime.wMonth,
                systemCreatingTime.wDay,
                systemCreatingTime.wHour,
                systemCreatingTime.wMinute,
                systemCreatingTime.wSecond,
                systemCreatingTime.wMilliseconds);

            exitTime = new DateTime(
                systemExitTime.wYear,
                systemExitTime.wMonth,
                systemExitTime.wDay,
                systemExitTime.wHour,
                systemExitTime.wMinute,
                systemExitTime.wSecond,
                systemExitTime.wMilliseconds);

            kernelTime = new TimeSpan(fileKernelTime.dwHighDateTime * 4294967296 + fileKernelTime.dwLowDateTime);
            userTime = new TimeSpan(fileUserTime.dwHighDateTime * 4294967296 + fileUserTime.dwLowDateTime);

        }


        public static void GetThreadTimesById(
          uint id,
          out DateTime creationTime,
          out DateTime exitTime,
          out TimeSpan kernelTime,
          out TimeSpan userTime)
        {
            IntPtr handle = OpenThread(ThreadAccessFlags.ALL_ACCESS, false, id);

            GetProcessTimes(handle,
                out FILETIME fileCreatingTime,
                out FILETIME fileExitTime,
                out FILETIME fileKernelTime,
                out FILETIME fileUserTime);

            FileTimeToSystemTime(ref fileCreatingTime, out SYSTEMTIME systemCreatingTime);
            FileTimeToSystemTime(ref fileExitTime, out SYSTEMTIME systemExitTime);

            creationTime = new DateTime(
                systemCreatingTime.wYear,
                systemCreatingTime.wMonth,
                systemCreatingTime.wDay,
                systemCreatingTime.wHour,
                systemCreatingTime.wMinute,
                systemCreatingTime.wSecond,
                systemCreatingTime.wMilliseconds);

            exitTime = new DateTime(
                systemExitTime.wYear,
                systemExitTime.wMonth,
                systemExitTime.wDay,
                systemExitTime.wHour,
                systemExitTime.wMinute,
                systemExitTime.wSecond,
                systemExitTime.wMilliseconds);

            kernelTime = new TimeSpan((long)fileKernelTime.dwHighDateTime * (long)4294967296 + (long)fileKernelTime.dwLowDateTime);
            userTime = new TimeSpan((long)fileUserTime.dwHighDateTime * (long)4294967296 + (long)fileUserTime.dwLowDateTime);

        }

        public uint GetMainThreadId()
        {
            uint[] ids = GetThreadIDs();

            if (ids.Length == 0) return 0;

            uint minId = ids[0];

            GetThreadTimesById(ids[0], out DateTime minCreationTime, out _, out _, out _);

            for (int i = 1; i < ids.Length; i++)
            {
                GetThreadTimesById(ids[i], out DateTime creationTime, out _, out _, out _);

                if (creationTime < minCreationTime)
                {
                    minCreationTime = creationTime;
                    minId = ids[i];
                }

            }

            return minId;
        }

        public void UpdateInfo()
        {
            throw new NotImplementedException();
        }
    }
}


