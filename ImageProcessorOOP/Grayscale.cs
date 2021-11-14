namespace ImageProcessorOOP
{
    public class Grayscale : IEffect
    {
        public string Name => "Grayscale";

        public void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                unsafe
                {
                    byte* bufferPtr = (byte*)bitmap.BackBuffer.ToPointer();

                    int bufferSize = bitmap.PixelWidth * bitmap.PixelHeight * 4;

                    for (int i = 0; i < bufferSize; i += 4)
                        SetToAverage(
                            ref bufferPtr[i    ],
                            ref bufferPtr[i + 1],
                            ref bufferPtr[i + 2]
                        );
                }
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private static void SetToAverage(
            ref byte a, ref byte b, ref byte c
        )
        {
            a = b = c = (byte)((a + b + c) / 3);
        }
    }
}
