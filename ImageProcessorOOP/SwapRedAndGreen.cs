namespace ImageProcessorOOP
{
    public class SwapRedAndGreen : ComponentsSwap
    {
        public override string Name
            => "Swap red and green components";

        public override void Apply(Image image)
        {
            SwapPixelComponents(
                image,
                Component.Red,
                Component.Green
            );
        }
    }
}
