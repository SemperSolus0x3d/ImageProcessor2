using System;

namespace ImageProcessor2.Effects
{
    public class HueShift : IEffect
    {
        public const double DefaultStep = 20d;

        public string Name => "Hue shift";

        public double Step { get; }

        private double[,] matrix;

        public HueShift(double step = DefaultStep)
        {
            Step = step;
            matrix = new double[3, 3];

            double radians = Step / 180d * Math.PI;

            double sinA = Math.Sin(radians);
            double cosA = Math.Cos(radians);

            matrix[0, 0] = cosA + (1d - cosA) / 3d;
            matrix[0, 1] = 1d / 3d * (1d - cosA) - Math.Sqrt(1d / 3d) * sinA;
            matrix[0, 2] = 1d / 3d * (1d - cosA) + Math.Sqrt(1d / 3d) * sinA;
            matrix[1, 0] = 1d / 3d * (1d - cosA) + Math.Sqrt(1d / 3d) * sinA;
            matrix[1, 1] = cosA + 1d / 3d * (1d - cosA);
            matrix[1, 2] = 1d / 3d * (1d - cosA) - Math.Sqrt(1d / 3d) * sinA;
            matrix[2, 0] = 1d / 3d * (1d - cosA) - Math.Sqrt(1d / 3d) * sinA;
            matrix[2, 1] = 1d / 3d * (1d - cosA) + Math.Sqrt(1d / 3d) * sinA;
            matrix[2, 2] = cosA + 1d / 3d * (1d - cosA);
        }

        public void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                unsafe
                {
                    uint* pixels = (uint*)bitmap.BackBuffer.ToPointer();

                    Apply(0, 0, bitmap.PixelWidth, bitmap.PixelHeight, bitmap.PixelWidth, pixels);
                }
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        public unsafe void Apply(int x1, int y1, int x2, int y2, int width, uint* pixels)
        {
            for (int y = y1; y < y2; y++)
                for (int x = x1; x < x2; x++)
                {
                    byte* pixelPtr = (byte*)(pixels + y * width + x);

                    byte* b = pixelPtr;
                    byte* g = pixelPtr + 1;
                    byte* r = pixelPtr + 2;

                    ShiftColor(ref *r, ref *g, ref *b);
                }
        }

        private byte Clamp(double val)
        {
            return (byte)Math.Ceiling(Math.Clamp(val, 0d, 255d));
        }

        private void ShiftColor(
            ref byte r, ref byte g, ref byte b
        )
        {
            double rx = r * matrix[0, 0] + g * matrix[0, 1] + b * matrix[0, 2];
            double gx = r * matrix[1, 0] + g * matrix[1, 1] + b * matrix[1, 2];
            double bx = r * matrix[2, 0] + g * matrix[2, 1] + b * matrix[2, 2];

            r = Clamp(rx);
            g = Clamp(gx);
            b = Clamp(bx);
        }
    }
}
