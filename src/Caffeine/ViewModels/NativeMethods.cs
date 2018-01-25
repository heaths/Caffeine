// <copyright file="NativeMethods.cs" company="Heath Stewart">
// Copyright (c) 2017 Heath Stewart
// See the LICENSE.txt file in the project root for more information.
// </copyright>

namespace Caffeine.ViewModels
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

#pragma warning disable SA1201 // Elements must appear in the correct order
#pragma warning disable SA1307 // Accessible fields must begin with upper-case letter
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1602 // Enumeration items must be documented
    [ExcludeFromCodeCoverage]
    internal static class NativeMethods
    {
        public const int POWER_REQUEST_CONTEXT_VERSION = 0;

        public const int WM_QUERYENDSESSION = 0x0011;
        public const int ENDSESSION_CRITICAL = 0x40000000;

        public const int TOKEN_QUERY = 0x0008;
        public const int TOKEN_ADJUST_PRIVILEGES = 0x0020;

        public const int SE_PRIVILEGE_ENABLED = 0x0002;
        public static readonly string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        public static readonly Version Vista = new Version(6, 2);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern SafeObjectHandle PowerCreateRequest([In] ref REASON_CONTEXT Context);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PowerSetRequest(
            SafeObjectHandle PowerRequest,
            [MarshalAs(UnmanagedType.U4)] PowerRequestType RequestType);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PowerClearRequest(
            SafeObjectHandle PowerRequest,
            [MarshalAs(UnmanagedType.U4)] PowerRequestType RequestType);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetProcessShutdownParameters(
            [MarshalAs(UnmanagedType.U4)] int dwLevel,
            [MarshalAs(UnmanagedType.U4)] int dwFlags);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "InitiateSystemShutdownW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InitiateSystemShutdown(
            string lpMachineName,
            string lpMessage,
            [MarshalAs(UnmanagedType.U4)] int dwTimeout,
            [MarshalAs(UnmanagedType.Bool)] bool bForceAppsClosed,
            [MarshalAs(UnmanagedType.Bool)] bool bRebootAfterShutdown);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShutdownBlockReasonCreate(
            IntPtr hWnd,
            string pwszReason);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShutdownBlockReasonDestroy(IntPtr hWnd);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            IntPtr ProcessHandle,
            [MarshalAs(UnmanagedType.U4)] int DesiredAccess,
            out SafeObjectHandle TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(
            string lpSystemName,
            string lpName,
            out LUID lpLuid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges(
            SafeObjectHandle TokenHandle,
            [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
            [In] ref TOKEN_PRIVILEGES NewState,
            [MarshalAs(UnmanagedType.U4)] int BufferLength,
            IntPtr PreviousState,
            [MarshalAs(UnmanagedType.U4)] out int ReturnLength);
    }

    internal enum POWER_REQUEST_CONTEXT
    {
        SimpleString = 1,
        DetailedString,
    }

    [ExcludeFromCodeCoverage]
    [StructLayout(LayoutKind.Sequential)]
    internal struct LASTINPUTINFO
    {
        private static readonly int Size = Marshal.SizeOf(typeof(LASTINPUTINFO));

        public static LASTINPUTINFO Default => new LASTINPUTINFO
        {
            cbSize = Size,
        };

        [MarshalAs(UnmanagedType.U4)]
        public int cbSize;

        [MarshalAs(UnmanagedType.U4)]
        public int dwTime;
    }

    [ExcludeFromCodeCoverage]
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID
    {
        [MarshalAs(UnmanagedType.U4)]
        public int LuidLowPart;

        [MarshalAs(UnmanagedType.I4)]
        public int LuidHighPart;
    }

    [ExcludeFromCodeCoverage]
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_PRIVILEGES
    {
        [MarshalAs(UnmanagedType.U4)]
        public int PrivilegeCount;

        public LUID Luid;

        [MarshalAs(UnmanagedType.U4)]
        public int Attributes;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct REASON_CONTEXT
    {
        [ExcludeFromCodeCoverage]
        public REASON_CONTEXT(string reason)
        {
            Version = NativeMethods.POWER_REQUEST_CONTEXT_VERSION;
            Flags = POWER_REQUEST_CONTEXT.SimpleString;
            SimpleReasonString = reason;
        }

        [MarshalAs(UnmanagedType.U4)]
        public int Version;

        [MarshalAs(UnmanagedType.U4)]
        public POWER_REQUEST_CONTEXT Flags;

        public string SimpleReasonString;
    }
#pragma warning restore SA1602 // Enumeration items must be documented
#pragma warning restore SA1600 // Elements must be documented
#pragma warning restore SA1307 // Accessible fields must begin with upper-case letter
#pragma warning restore SA1201 // Elements must appear in the correct order
}
