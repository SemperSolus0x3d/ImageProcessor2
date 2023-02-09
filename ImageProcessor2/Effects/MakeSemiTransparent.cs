using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessor2.Effects
{
    public class MakeSemiTransparent : IEffect
    {
        public string Name => "Make semi-transparent";

        public unsafe void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                unsafe
                {
                    byte* pixels = (byte*)bitmap.BackBuffer.ToPointer();

                    var bytesCount = bitmap.PixelWidth * bitmap.PixelHeight * 4;

                    for (int i = 0; i < bytesCount; i += 4)
                    {
                        int avg = 0;

                        for (int j = 0; j < 3; j++)
                            avg += pixels[i + j];

                        avg /= 3;

                        pixels[i + 3] = (byte)avg;
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
