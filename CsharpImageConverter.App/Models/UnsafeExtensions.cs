using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.App.Models
{
    static class UnsafeExtensions
    {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void RtlMoveMemory(IntPtr dest, IntPtr src, [MarshalAs(UnmanagedType.U4)] int length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemCopy(IntPtr dest, IntPtr src, int length)
             => RtlMoveMemory(dest, src, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemCopy(void* dest, void* src, int length)
            => RtlMoveMemory((IntPtr)dest, (IntPtr)src, length);

    }
}
