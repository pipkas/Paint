using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PaintApp.Src;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Input;
using System.ComponentModel;
using System;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace PaintApp.UI;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    public int CanvasWidth { get; set; } = 550;
    public int CanvasHeight { get; set; } = 400;
    public WriteableBitmap Bitmap {get; set;}
    private readonly PaintManager painter;
    private readonly UISettings settings;

    public MainWindow()
    {
        InitializeComponent();

        settings = new UISettings();
        painter = new PaintManager();

        InitBitmap();

        DataContext = painter.Settings;
        CanvasImage.Source = Bitmap;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == ClientSizeProperty && painter?.Image != null)
        { 
            HandleWindowResize(ClientSize);
        }
    }

    private void InitBitmap()
    {
        Bitmap = new WriteableBitmap(
            new PixelSize(CanvasWidth , CanvasHeight),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Opaque);

        UpdateBitmap();
    }

    private void HandleWindowResize(Size newWindowSize)
    {
        var newCanvasWidth = (int)newWindowSize.Width - UISettings.DeltaWindowCanvasWidth;
        var newCanvasHeight = (int)newWindowSize.Height - UISettings.DeltaWindowCanvasHeight;
        if (newCanvasWidth <= CanvasImage.Width 
            || newCanvasHeight <= CanvasImage.Height)
        {
            return;
        }
        painter.Image.Resize(newCanvasWidth, newCanvasHeight);

        InitBitmap();
        CanvasImage.Source = Bitmap;
        CanvasImage.Width = newCanvasWidth;
        CanvasImage.Height = newCanvasHeight;
    }

    private void OnColorClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string rgb)
        {
            painter.Settings.Color = ColorParser.ParseColorRGB(rgb);
        }  
    }

    private async void OnLineClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button)
        {
            // если line использовали только что, то сохраняем значение толщины, иначе дефолтное = 1
            var dialog = painter.Settings.Tool is Line line
            ? new LineSettingsWindow(line.Thickness)
            : new LineSettingsWindow();

            await dialog.ShowDialog(this);

            if (dialog.ResultThickness.HasValue)
            {
                painter.Settings.Tool = new Line(dialog.ResultThickness.Value);
                settings.FirstLinePoint = null;
            }
        }
    }

    private async void OnStampClick(object? sender, RoutedEventArgs e)
    {
        var dialog = painter.Settings.Tool is Stamp stamp
            ? new StampSettingsWindow(stamp)
            : new StampSettingsWindow();

        await dialog.ShowDialog(this);

        if (dialog.ResultedStamp != null)
        {
            painter.Settings.Tool = dialog.ResultedStamp;
        }
    }

    private void OnFillingClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button)
        {
            painter.Settings.Tool = painter.Settings.Tool is Filling 
                ? painter.Settings.Tool 
                : new Filling();

            settings.FirstLinePoint = null;
        }
    }


    private async void OnHelpClick(object? sender, RoutedEventArgs e)
    {
        var messageBox = MessageBoxManager.GetMessageBoxStandard(
            "О программе",      
            settings.HelpClickText,
            ButtonEnum.Ok 
        );
        await messageBox.ShowWindowDialogAsync(this);
    }

    private async void OnFileClick(object? sender, RoutedEventArgs e)
    {
        var options = new FilePickerOpenOptions
        {
            Title = "Открыть изображение",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Images")
                {
                    Patterns =
                    [
                        "*.png",
                        "*.jpg",
                        "*.jpeg",
                        "*.bmp",
                        "*.gif"
                    ]
                }
            ]
        };

        var files = await this.StorageProvider.OpenFilePickerAsync(options);

        if (files.Count == 0)
            return;

        await using var stream = await files[0].OpenReadAsync();

        var picture = FileManager.LoadFromStream(stream);

        painter.Image = picture;
        CanvasWidth = painter.Image.Width;
        CanvasHeight = painter.Image.Height;
        InitBitmap();
        CanvasImage.Source = Bitmap;
        CanvasImage.Width = painter.Image.Width;
        CanvasImage.Height = painter.Image.Height;
    }

    private void OnEraseClick(object? sender, RoutedEventArgs e)
    {
        painter.Image.Clean();
        settings.FirstLinePoint = null;
        UpdateBitmap();
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        await SaveFileAsync(this);
    }

    public async Task SaveFileAsync(Window parent)
    {
        var options = new FilePickerSaveOptions
        {
            Title = "Сохранить изображение",
            SuggestedFileName = "image.png",
            FileTypeChoices =
            [
                new FilePickerFileType("PNG Image")
                {
                    Patterns = ["*.png"]
                }
            ]
        };

        var file = await parent.StorageProvider.SaveFilePickerAsync(options);

        if (file != null)
        {
            await using var stream = await file.OpenWriteAsync();
            FileManager.SaveToPng(painter.Image, stream);
        }
    }

    private void OnImagePointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (painter.Settings.Tool is null) return;

        var point = e.GetPosition(CanvasImage);
        
        var coord = new Coord(
            (int)Math.Round(point.X),
            (int)Math.Round(point.Y)
        );

        switch (painter.Settings.Tool)
        {
            case Line line: 
                if (settings.FirstLinePoint is Point startPoint)
                {
                    var start = new Coord(
                        (int)Math.Round(startPoint.X),
                        (int)Math.Round(startPoint.Y)
                    );
                    painter.DrawLine(start, coord, line);
                    settings.FirstLinePoint = null;
                }
                else
                {
                    settings.FirstLinePoint = point;
                    return;
                }
                break;

            case Stamp stamp:
                painter.PutStamp(coord, stamp);
                break;

            case Filling:
                painter.Filling(coord);
                break;
        }

        UpdateBitmap();
    }

    private void UpdateBitmap()
    {
        if (Bitmap == null) return;
        
        using var fb = Bitmap.Lock();
        Marshal.Copy(
            painter.Image.PixelsBuffer,
            0,
            fb.Address,
            painter.Image.Width * painter.Image.Height * Picture.ColorBytesCount
        );
        CanvasImage?.InvalidateVisual();
    }

}