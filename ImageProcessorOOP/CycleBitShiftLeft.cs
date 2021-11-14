using System.Windows.Media.Imaging;

namespace ImageProcessorOOP
{
    public class CycleBitShiftLeft : CycleBitShift
    {
        public override string Name => "Cycle bit shift left";

        public unsafe override void Apply(Image image)
        {
            WriteableBitmap bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                uint* bufferPtr = (uint*)bitmap.BackBuffer.ToPointer();
                int pixelsCount = bitmap.PixelWidth * bitmap.PixelHeight;

                for (int i = 0; i < pixelsCount; i++)
                    bufferPtr[i] = ShiftBitsLeft(bufferPtr[i]);
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private uint ShiftBitsLeft(uint a)
        {
            return (a << 1) | (a >> 31);
        }
    }
}
