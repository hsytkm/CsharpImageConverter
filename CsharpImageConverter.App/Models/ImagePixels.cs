using System;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.App.Models
{
    // C++型と合わせること
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    readonly struct ImagePixels
    {
        public readonly IntPtr PixelsPtr;
        public readonly int AllocSize;
        public readonly int Width;
        public readonly int Height;
        public readonly int BytesPerPixel;
        public readonly int Stride;

        public ImagePixels(int width, int height, int bytesPerPixel, int stride, IntPtr intPtr, int size)
        {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            Stride = stride;
            PixelsPtr = intPtr;
            AllocSize = size;
        }

        public readonly bool IsValid()
        {
            if (PixelsPtr == IntPtr.Zero) return false;
            if (Width == 0 || Height == 0) return false;
            if (Stride < Width * BytesPerPixel) return false;
            if (AllocSize < Width * BytesPerPixel * Height) return false;

            return true;    //valid
        }

        public readonly bool IsInvalid() => !IsValid();
    }

    readonly struct ImagePixelsContainer : IDisposable
    {
        public readonly ImagePixels Pixels;
        private readonly IntPtr UnmanagedPtr;

        private ImagePixelsContainer(int width, int height, int bytesPerPixels)
        {
            var stride = width * bytesPerPixels;
            var size = stride * height;

            UnmanagedPtr = Marshal.AllocCoTaskMem(size);
            Pixels = new ImagePixels(width, height, bytesPerPixels, stride, UnmanagedPtr, size);
        }

        public ImagePixelsContainer(int width, int height) : this(width, height, 3) { }

        public void Dispose()
        {
            if (UnmanagedPtr != IntPtr.Zero)
                Marshal.FreeCoTaskMem(UnmanagedPtr);
        }
    }
}