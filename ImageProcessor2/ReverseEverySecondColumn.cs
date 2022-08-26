namespace ImageProcessor2
{
    public class ReverseEverySecondColumn : IEffect
    {
        public string Name =>
            "Reverse every second column of pixels";

        public void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                unsafe
                {
                    uint* bufferPtr = (uint*)bitmap.BackBuffer.ToPointer();

                    for (int column = 0; column < bitmap.PixelWidth; column += 2)
                    {
                        int top = column;
                        int bottom =
                            (bitmap.PixelHeight - 1) *
                            bitmap.PixelWidth +
                            column;

                        while (top < bottom)
                        {
                            uint temp = bufferPtr[top];
                            bufferPtr[top] = bufferPtr[bottom];
                            bufferPtr[bottom] = temp;

                            top += bitmap.PixelWidth;
                            bottom -= bitmap.PixelWidth;
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
