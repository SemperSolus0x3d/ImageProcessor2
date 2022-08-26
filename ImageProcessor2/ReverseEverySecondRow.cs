namespace ImageProcessor2
{
    public class ReverseEverySecondRow : IEffect
    {
        public string Name =>
            "Reverse every second row of pixels";

        public void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                unsafe
                {
                    uint* bufferPtr = (uint*)bitmap.BackBuffer.ToPointer();

                    for (int row = 0; row < bitmap.PixelHeight; row += 2)
                    {
                        int left = row * bitmap.PixelWidth;
                        int right = left + bitmap.PixelWidth - 1;

                        while (left < right)
                        {
                            uint temp = bufferPtr[left];
                            bufferPtr[left] = bufferPtr[right];
                            bufferPtr[right] = temp;

                            left++; right--;
                        }
                    }
                }
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }
}
