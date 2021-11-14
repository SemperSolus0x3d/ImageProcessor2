using System.Windows.Media.Imaging;

namespace ImageProcessorOOP
{
    public class CycleBitShiftRight : CycleBitShift
    {
        public override string Name => "Cycle bit shift right";

        public unsafe override void Apply(Image image)
        {
            WriteableBitmap bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                uint* bufferPtr = (uint*)bitmap.BackBuffer.ToPointer();
                int pixelsCount = bitmap.PixelWidth * bitmap.PixelHeight;

                for (int i = 0; i < pixelsCount; i++)
                    bufferPtr[i] = ShiftBitsRight(bufferPtr[i]);
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private uint ShiftBitsRight(uint a)
        {
            return (a >> 1) | (a << 31);
        }
    }
}
