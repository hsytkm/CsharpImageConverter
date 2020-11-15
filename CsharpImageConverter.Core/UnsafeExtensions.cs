using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.Core
{
    static class UnsafeExtensions
    {
        #region MemCopy
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void RtlMoveMemory(IntPtr dest, IntPtr src, [MarshalAs(UnmanagedType.U4)] int length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MemCopy(IntPtr dest, IntPtr src, int length)
             => RtlMoveMemory(dest, src, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MemCopy(void* dest, void* src, int length)
            => RtlMoveMemory((IntPtr)dest, (IntPtr)src, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void MyMemCopy(IntPtr dest, IntPtr src, int length)
             => MyMemCopy(dest.ToPointer(), src.ToPointer(), length);

        public static unsafe void MyMemCopy(void* dest, void* src, int length)
        {
            byte* destPtr = (byte*)dest;
            byte* srcPtr = (byte*)src;
            var tail = destPtr + length;

            while (destPtr + 7 < tail)
            {
                *(ulong*)destPtr = *(ulong*)src;
                srcPtr += 8;
                destPtr += 8;
            }

            if (destPtr + 3 < tail)
            {
                *(uint*)destPtr = *(uint*)src;
                srcPtr += 4;
                destPtr += 4;
            }

            while (destPtr < tail)
            {
                *destPtr = *srcPtr;
                ++srcPtr;
                ++destPtr;
            }
        }
        #endregion

        /// <summary>構造体をシリアライズして byte[] に書き出します</summary>
        public static void CopyStructToArray<T>(T srcData, byte[] destArray) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var ptr = Marshal.AllocCoTaskMem(size);
            try
            {
                Marshal.StructureToPtr(srcData, ptr, false);
                Marshal.Copy(ptr, destArray, 0, size);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptr);
            }
        }

    }
}
