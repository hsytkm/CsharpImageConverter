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

        public ImagePixels(int width, int height, int bytesPerPixel, int stride, IntPtr ptr)
        {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            Stride = stride;
            AllocSize = height * stride;
            PixelsPtr = ptr;
        }

        public readonly bool IsValid()
        {
            if (PixelsPtr == IntPtr.Zero || AllocSize == 0) return false;
            if (Width == 0 || Height == 0 || BytesPerPixel == 0) return false;
            if (Width * BytesPerPixel > Stride) return false;
            return true;
        }
    }

    readonly struct ImagePixelsContainer : IDisposable
    {
        public readonly ImagePixels Payload;
        private readonly IntPtr UnmanagedPtr;

        private ImagePixelsContainer(int width, int height, int bytesPerPixels)
        {
            var stride = width * bytesPerPixels;
            var size = stride * height;
            UnmanagedPtr = Marshal.AllocCoTaskMem(size);
            Payload = new ImagePixels(width, height, bytesPerPixels, stride, UnmanagedPtr);
        }

        public ImagePixelsContainer(int width, int height) : this(width, height, 3) { }

        public void Dispose()
        {
            if (UnmanagedPtr != IntPtr.Zero)
                Marshal.FreeCoTaskMem(UnmanagedPtr);
        }
    }
}