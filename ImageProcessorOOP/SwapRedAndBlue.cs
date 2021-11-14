namespace ImageProcessorOOP
{
    public class SwapRedAndBlue : ComponentsSwap
    {
        public override string Name
            => "Swap red and blue components";

        public override void Apply(Image image)
        {
            SwapPixelComponents(
                image,
                Component.Red,
                Component.Blue
            );
        }
    }
}
