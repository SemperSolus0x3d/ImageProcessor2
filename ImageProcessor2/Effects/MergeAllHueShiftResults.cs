namespace ImageProcessor2.Effects
{
    public class MergeAllHueShiftResults : IEffect
    {
        public const int DefaultStep = 1;

        public string Name => "Merge all hue shift results";

        public int Step { get; }

        private HueShift[] _shifts;

        public MergeAllHueShiftResults(int step = DefaultStep)
        {
            Step = step;

            _shifts = new HueShift[360 / step];

            for (int i = 0; i < _shifts.Length; i++)
                _shifts[i] = new HueShift(i * Step);
        }

        public unsafe void Apply(Image image)
        {
            var bitmap = image.Bitmap;

            try
            {
                bitmap.Lock();

                uint* pixels = (uint*)bitmap.BackBuffer.ToPointer();

                int width = bitmap.PixelWidth;
                int height = bitmap.PixelHeight;

                var regionHeight = height;
                var regionWidth = width / _shifts.Length;
                var regionsCount = (width + regionWidth - 1) / regionWidth;

                for (int i = 0; i < regionsCount; i++)
                {
                    int x1 = regionWidth * i;
                    int y1 = 0;

                    int x2 = regionWidth * (i + 1);
                    int y2 = regionHeight;

                    if (i == regionsCount - 1)
                        x2 = width;

                    _shifts[i % _shifts.Length].Apply(x1, y1, x2, y2, width, pixels);
                }
            }
            finally
            {
                bitmap.Unlock();
            }
        }
    }
}
