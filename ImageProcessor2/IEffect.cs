using ImageProcessor2.Effects;

namespace ImageProcessor2
{
    public interface IEffect
    {
        string Name { get; }

        void Apply(Image image);
    }
}