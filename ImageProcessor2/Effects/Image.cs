using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageProcessor2.Effects
{
    public class Image
    {
        public PixelFormat PreferredFormat { get; }
            = PixelFormats.Bgra32;

        public WriteableBitmap Bitmap { get; private set; }

        public Image(BitmapSource source)
        {
            // Create new Bitmap from source
            // and convert format if needed
            Bitmap = new WriteableBitmap(
                source.Format == PreferredFormat ?
                source :
                ConvertFormatToPreferred(source)
            );
        }

        public Image Clone()
        {
            return new Image(Bitmap.Clone());
        }

        private BitmapSource ConvertFormatToPreferred(
            BitmapSource original
        )
        {
            FormatConvertedBitmap converted = new FormatConvertedBitmap();

            converted.BeginInit();
            converted.Source = original;
            converted.DestinationFormat = PreferredFormat;
            converted.EndInit();

            return converted;
        }
    }
}
