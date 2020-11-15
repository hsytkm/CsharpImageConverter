using System.Runtime.InteropServices;

namespace CsharpImageConverter.Core
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal readonly ref struct Pixel3ch
    {
        public readonly byte Ch0;
        public readonly byte Ch1;
        public readonly byte Ch2;
    }
}
