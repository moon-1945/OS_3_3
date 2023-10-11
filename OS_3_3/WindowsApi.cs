using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OS_3_3
{
    public static class WindowsApi
    {

        public const int TH32CS_SNAPTHREAD = 0x00000004;
        public const uint THREAD_ALL_ACCESS = 0x001F03FF;
        public const uint INFINITY = 0xFFFFFFFF;
        public const uint MAX_PATH = 260;
        public const uint TH32CS_SNAPPROCESS = 0x00000002;
        public const int INVALID_HANDLE_VALUE = -1;


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CreateProcess(
               string lpApplicationName,
               string lpCommandLine,
               IntPtr lpProcessAttributes,
               IntPtr lpThreadAttributes,
               bool bInheritHandles,
               uint dwCreationFlags,
               IntPtr lpEnvironment,
               string lpCurrentDirectory,
               [In] ref STARTUPINFO lpStartupInfo,
               out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        public static extern uint SuspendThread([In] IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread([In] IntPtr hThread);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint K32GetModuleFileNameExW(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, uint nSize);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        public static extern bool Thread32First(IntPtr hSnapshot, ref THREADENTRY32 lpte);

        [DllImport("kernel32.dll")]
        public static extern bool Thread32Next(IntPtr hSnapshot, ref THREADENTRY32 lpte);

        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenThread(ThreadAccessFlags dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        public static extern bool GetProcessAffinityMask(IntPtr hProcess, out UIntPtr processAffinityMask, out UIntPtr systemAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern bool SetProcessAffinityMask(IntPtr hProcess, UIntPtr processAffinityMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ProcessPriorityClass GetPriorityClass(IntPtr hProcess);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetPriorityClass(IntPtr hProcess, ProcessPriorityClass dwPriorityClass);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THREADENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ThreadID;
            public uint th32OwnerProcessID;
            public uint tpBasePri;
            public uint tpDeltaPri;
            public uint dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

    }
}

[Flags]
public enum ProcessAccessFlags : uint
{
    TERMINATE = 0x0001,
    CREATE_THREAD = 0x0002,
    SET_SESSIONID = 0x0004,
    VM_OPERATION = 0x0008,
    VM_READ = 0x0010,
    VM_WRITE = 0x0020,
    DUP_HANDLE = 0x0040,
    CREATE_PROCESS = 0x0080,
    SET_QUOTA = 0x0100,
    SET_INFORMATION = 0x0200,
    QUERY_INFORMATION = 0x0400,
    SUSPEND_RESUME = 0x0800,
    QUERY_LIMITED_INFORMATION = 0x1000,
    SET_LIMITED_INFORMATION = 0x2000,
    ALL_ACCESS = 0xFFFF,
}

[Flags]
public enum ThreadAccessFlags : uint
{
    TERMINATE = 0x0001,
    SUSPEND_RESUME = 0x0002,
    GET_CONTEXT = 0x0008,
    SET_CONTEXT = 0x0010,
    QUERY_INFORMATION = 0x0040,
    SET_INFORMATION = 0x0020,
    SET_THREAD_TOKEN = 0x0080,
    IMPERSONATE = 0x0100,
    DIRECT_IMPERSONATION = 0x0200,
    SET_LIMITED_INFORMATION = 0x0400,
    QUERY_LIMITED_INFORMATION = 0x0800,
    RESUME = 0x1000,
    ALL_ACCESS = 0xFFFF,
}


public enum ProcessPriorityClass : uint
{
    ABOVE_NORMAL = 0x8000,
    BELOW_NORMAL = 0x4000,
    HIGH = 0x80,
    IDLE = 0x40,
    NORMAL = 0x20,
    REALTIME = 0x100
}





