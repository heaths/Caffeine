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
#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1602 // Enumeration items must be documented
    internal static class NativeMethods
    {
        public const int POWER_REQUEST_CONTEXT_VERSION = 0;

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern SafePowerRequestHandle PowerCreateRequest([In] ref REASON_CONTEXT Context);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PowerSetRequest(
            SafePowerRequestHandle PowerRequest,
            [MarshalAs(UnmanagedType.U4)] PowerRequestType RequestType);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PowerClearRequest(
            SafePowerRequestHandle PowerRequest,
            [MarshalAs(UnmanagedType.U4)] PowerRequestType RequestType);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);
    }

    internal enum POWER_REQUEST_CONTEXT
    {
        SimpleString = 1,
        DetailedString,
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
#pragma warning restore SA1201 // Elements must appear in the correct order
}
