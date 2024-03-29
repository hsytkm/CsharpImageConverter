﻿using System;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.Core
{
    // C++型と合わせること
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct ImagePixels
    {
        public readonly IntPtr PixelsPtr;
        public readonly int AllocSize;
        public readonly int Width;
        public readonly int Height;
        public readonly int BytesPerPixel;
        public readonly int Stride;

        public ImagePixels(int width, int height, int bytesPerPixel, int stride, IntPtr intPtr, int size)
        {
            if (IntPtr.Size != 8) throw new NotSupportedException();
            if (Marshal.SizeOf(typeof(ImagePixels)) != 8 + 4 * 5) throw new NotSupportedException();

            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            Stride = stride;
            PixelsPtr = intPtr;
            AllocSize = size;
        }

        public readonly int BitsPerPixel => BytesPerPixel * 8;

        public readonly bool IsContinuous => Width * BytesPerPixel == Stride;

        public readonly bool IsValid
        {
            get {
                if (PixelsPtr == IntPtr.Zero) return false;
                if (Width == 0 || Height == 0) return false;
                if (Stride < Width * BytesPerPixel) return false;
                if (AllocSize < Width * BytesPerPixel * Height) return false;

                return true;    //valid
            }
        }

        public readonly bool IsInvalid => !IsValid;
    }

    public class ImagePixelsContainer : IDisposable
    {
        public readonly ImagePixels Pixels;
        private readonly IntPtr _allocatedMemoryPointer;
        private readonly int _allocatedSize;
        private bool _disposed;

        public ImagePixelsContainer(int width, int height, int bytesPerPixels)
        {
            // Alpha付いてたら除去します(無理やりでイマイチです…)
            if (bytesPerPixels == 4)
                bytesPerPixels = 3;

            var stride = width * bytesPerPixels;
            var size = stride * height;

            _allocatedSize = size;
            _allocatedMemoryPointer = Marshal.AllocCoTaskMem(size);
            GC.AddMemoryPressure(size);

            Pixels = new ImagePixels(width, height, bytesPerPixels, stride, _allocatedMemoryPointer, size);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Marshal.FreeCoTaskMem(_allocatedMemoryPointer);
                GC.RemoveMemoryPressure(_allocatedSize);

                _disposed = true;
            }
        }
    }
}