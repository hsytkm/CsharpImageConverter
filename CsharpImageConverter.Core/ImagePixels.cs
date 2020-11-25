using System;
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
        private IntPtr _allocatedMemoryPointer;

        private ImagePixelsContainer(int width, int height, int bytesPerPixels)
        {
            var stride = width * bytesPerPixels;
            var size = stride * height;

            _allocatedMemoryPointer = Marshal.AllocCoTaskMem(size);
            Pixels = new ImagePixels(width, height, bytesPerPixels, stride, _allocatedMemoryPointer, size);
        }

        public ImagePixelsContainer(int width, int height) : this(width, height, 3) { }

        public void Dispose()
        {
            if (_allocatedMemoryPointer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(_allocatedMemoryPointer);
                _allocatedMemoryPointer = IntPtr.Zero;
            }
        }
    }
}