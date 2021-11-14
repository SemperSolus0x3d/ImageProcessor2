using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageProcessorOOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEffect[] effects;
        private History history;

        public MainWindow()
        {
            InitializeComponent();
            InitializeEffects();

            history = new History();

            ProcessingAlgorithmsListBox.ItemsSource = effects;
        }

        private void InitializeEffects()
        {
            effects = new IEffect[]
            {
                new CycleBitShiftLeft(),
                new CycleBitShiftRight(),

                new SwapRedAndGreen(),
                new SwapGreenAndBlue(),
                new SwapRedAndBlue()
            };
        }

        private void LoadImage(Uri uri)
        {
            BitmapImage image = new BitmapImage(uri);

            if (image.IsDownloading)
            {
                image.DownloadCompleted += OnImageDownloadCompleted;
                image.DownloadFailed += (s, e) => MessageBox.Show("Download failed");
                image.DecodeFailed += (s, e) => MessageBox.Show("Decode failed");
            }
            else
                OnImageDownloadCompleted(image, null);
        }

        private void OnImageDownloadCompleted(object sender, EventArgs e)
        {
            history.Clear();
            history.Save(new Image(sender as BitmapSource));

            MainImage.BeginInit();
            MainImage.Source = history.CurrentState.Bitmap;
            MainImage.EndInit();
        }

        private void GridSplitter_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            MainGrid.RowDefinitions[5].Height =
                new GridLength(MainGrid.RowDefinitions[5].MinHeight);
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = null;

            if (string.IsNullOrWhiteSpace(UriTextBox.Text))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Выберите изображение";

                if (dialog.ShowDialog() ?? false)
                    uri = new Uri(dialog.FileName);
                else return;
            }
            else
                uri = new Uri(UriTextBox.Text);

            UriTextBox.Text = uri.AbsoluteUri;

            LoadImage(uri);
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (history.CurrentState == null)
                return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "PNG Изображения|*.png|Все файлы|*.*";

            if (dialog.ShowDialog() ?? false)
            {
                using (var file = dialog.OpenFile())
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(history.CurrentState.Bitmap));
                    encoder.Save(file);
                }
            }

            MessageBox.Show("Сохранено");
        }

        private void ProcessImageButton_Click(object sender, RoutedEventArgs e)
        {
            IEffect effect = ProcessingAlgorithmsListBox.SelectedItem as IEffect;

            var newImage = history.CurrentState.Clone();
            effect.Apply(newImage);
            history.Save(newImage);

            MainImage.BeginInit();
            MainImage.Source = newImage.Bitmap;
            MainImage.EndInit();
        }

        private void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] files =
                    e.Data.GetData(DataFormats.FileDrop, true) as string[];

                if ((files?.Length ?? 0) == 1)
                    e.Effects = DragDropEffects.All;
            }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] files =
                    e.Data.GetData(DataFormats.FileDrop, true) as string[];

                if ((files?.Length ?? 0) == 1)
                {
                    UriTextBox.Text = files[0];
                    LoadImage(new Uri(files[0]));
                }
            }
        }
    }
}
