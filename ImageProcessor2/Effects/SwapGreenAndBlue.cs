namespace ImageProcessor2.Effects
{
    public class SwapGreenAndBlue : ComponentsSwap
    {
        public override string Name
            => "Swap green and blue components";

        public override void Apply(Image image)
        {
            SwapPixelComponents(
                image,
                Component.Green,
                Component.Blue
            );
        }
    }
}
