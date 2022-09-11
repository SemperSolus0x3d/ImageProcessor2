namespace ImageProcessor2.Effects
{
    public abstract class CycleBitShift : IEffect
    {
        public abstract string Name { get; }

        public abstract void Apply(Image image);
    }
}
