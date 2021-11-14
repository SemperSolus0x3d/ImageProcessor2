namespace ImageProcessorOOP
{
    public abstract class ComponentsSwap : IEffect
    {
        public abstract string Name { get; }

        public abstract void Apply(Image image);

        protected enum Component
        {
            Blue = 0,
            Green,
            Red,
            Alpha
        }

        protected static void SwapPixelComponents(
            Image image,
            Component component1,
            Component component2
        )
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();
                
                unsafe
                {
                    byte* bufferPtr = (byte*)bitmap.BackBuffer.ToPointer();

                    int pixelsCount = bitmap.PixelWidth * bitmap.PixelHeight;
                    int bufferSize = pixelsCount * 4;

                    for (int i = 0; i < bufferSize; i += 4)
                    {
                        byte temp = bufferPtr[i + (int)component1];

                        bufferPtr[i + (int)component1] =
                            bufferPtr[i + (int)component2];

                        bufferPtr[i + (int)component2] = temp;
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
