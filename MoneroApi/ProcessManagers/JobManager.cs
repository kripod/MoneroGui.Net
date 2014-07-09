using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Jojatekok.MoneroAPI.ProcessManagers
{
    sealed class JobManager : IDisposable
    {
        private IntPtr Handle { get; set; }

        public JobManager()
        {
            Handle = NativeMethods.CreateJobObject(IntPtr.Zero, null);

            var infoBasic = new JOBOBJECT_BASIC_LIMIT_INFORMATION { LimitFlags = 0x2000 };
            var infoExtended = new JOBOBJECT_EXTENDED_LIMIT_INFORMATION { BasicLimitInformation = infoBasic };

            var length = Marshal.SizeOf(typeof(JOBOBJECT_EXTENDED_LIMIT_INFORMATION));
            var extendedInfoPtr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(infoExtended, extendedInfoPtr, false);

            if (!NativeMethods.SetInformationJobObject(Handle, JobObjectInfoType.ExtendedLimitInformation, extendedInfoPtr, (uint)length)) {
                throw new Win32Exception(string.Format(Helper.InvariantCulture, "Unable to set information. Error: {0}", Marshal.GetLastWin32Error()));
            }
        }

        public bool AddProcess(Process process)
        {
            return NativeMethods.AssignProcessToJobObject(Handle, process.Handle);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~JobManager()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing) {
                if (Handle != IntPtr.Zero) {
                    NativeMethods.CloseHandle(Handle);
                    Handle = IntPtr.Zero;
                }
            }
        }
    }

    static class NativeMethods
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateJobObject(IntPtr a, string lpName);

        [DllImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType, IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

        [DllImport("Kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }

    [StructLayout(LayoutKind.Sequential)]
    struct IO_COUNTERS
    {
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_BASIC_LIMIT_INFORMATION
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
    {
        public JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;
        public IO_COUNTERS IoInfo;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }

    public enum JobObjectInfoType
    {
        ExtendedLimitInformation = 9
    }
}
