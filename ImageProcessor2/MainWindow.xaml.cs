using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ImageProcessor2.Effects;

namespace ImageProcessor2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEffect[] Effects { get; private set; }
        public History History { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeEffects();

            History = new History();

            DataContext = this;
        }

        private void InitializeEffects()
        {
            Effects = new IEffect[]
            {
                new CycleBitShiftLeft(),
                new CycleBitShiftRight(),

                new SwapRedAndGreen(),
                new SwapGreenAndBlue(),
                new SwapRedAndBlue(),

                new HueShift(),

                new ReverseEverySecondRow(),
                new ReverseEverySecondColumn(),

                new Grayscale()
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
            History.Clear();
            History.Save(new Image(sender as BitmapSource));
        }

        // Ctrl+Z hotkey handler
        private void CancelHotkeyHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var source = History.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // Ctrl+Shift+Z hotkey handler
        private void RevertHotkeyHandler(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var source = History.GoForward();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void GridSplitter_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainGrid.RowDefinitions[5].Height =
                    new GridLength(MainGrid.RowDefinitions[5].MinHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (History.CurrentState == null)
                    return;

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "PNG Изображения|*.png|Все файлы|*.*";

                if (dialog.ShowDialog() ?? false)
                {
                    using (var file = dialog.OpenFile())
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(History.CurrentState.Bitmap));
                        encoder.Save(file);
                    }

                    MessageBox.Show("Сохранено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void ProcessImageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IEffect effect = ProcessingAlgorithmsListBox.SelectedItem as IEffect;

                var newImage = History.CurrentState.Clone();
                effect.Apply(newImage);
                History.Save(newImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void MainWindow_DragOver(object sender, DragEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void MainWindow_Drop(object sender, DragEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
