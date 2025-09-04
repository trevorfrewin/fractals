using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tfrewin.play.fractal.start.engine;
using tfrewin.play.fractal.start.utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Text.Json;

namespace tfrewin.play.fractal.avalonia;

public partial class MainWindow : Window
{
    private ImageParameters? _imageParameters;
    private SixLabors.ImageSharp.Image? _image;

    private MatrixEngine _engine = new();

    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        InitializeControls();
    }

    private void InitializeControls()
    {
        var colourWheels = new ColourWheelGenerator().GenerateColourWheels();
        var sortedColourWheelNames = colourWheels.Select(cw => cw.ColourWheelName).OrderBy(name => name).ToList();
        ColourWheelBox.ItemsSource = sortedColourWheelNames;
        ColourWheelBox.SelectedIndex = 0;

        SetNameBox.ItemsSource = new List<string> { "Mandelbrot", "Julia" };
        SetNameBox.SelectedIndex = 0;

        OutputQualityBox.ItemsSource = new List<string> { "Fast", "Medium", "FHD", "QHD", "High Quality", "Superb", "Ridiculous", "Ludicrous", "Plaid" };
        OutputQualityBox.SelectedIndex = 0;

        OutputTypeBox.ItemsSource = new List<string> { "JPG", "PNG" };
        OutputTypeBox.SelectedIndex = 0;

        ApplyButton.Click += ApplyButton_Click;
        ResetButton.Click += ResetButton_Click;
        ImportButton.Click += ImportButton_Click;
        SaveButton.Click += SaveButton_Click;
        FractalImage.PointerPressed += FractalImage_PointerPressed;
    }

    private async void ImportButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var options = new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Import Fractal Parameters",
            AllowMultiple = false,
            FileTypeFilter = new List<Avalonia.Platform.Storage.FilePickerFileType>
            {
                new Avalonia.Platform.Storage.FilePickerFileType("Parameter Files") { Patterns = new[] { "*.parameters.json" } },
                new Avalonia.Platform.Storage.FilePickerFileType("JSON Files") { Patterns = new[] { "*.json" } }
            }
        };

        var files = await this.StorageProvider.OpenFilePickerAsync(options);
        if (files == null || files.Count == 0)
            return;

        var file = files[0];
        var filePath = file.Path.LocalPath;
        if (!File.Exists(filePath))
            return;

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var parameters = JsonSerializer.Deserialize<ImageParameters>(json);

            if (parameters != null)
            {
                ZoomBox.Value = (decimal?)parameters.Zoom;
                MoveXBox.Value = (decimal?)parameters.MoveX;
                MoveYBox.Value = (decimal?)parameters.MoveY;

                // Set ComboBoxes by value
                SetNameBox.SelectedItem = parameters.SetName;
                OutputQualityBox.SelectedItem = $"{parameters.PlaneWidth}x{parameters.PlaneHeight}";
                ColourWheelBox.SelectedItem = parameters.ColourWheelName;

                IterationFactorBox.Value = (decimal?)parameters.IterationFactor;

                var qualityName = QualityResolutionMap.GetQualityName(parameters.PlaneWidth, parameters.PlaneHeight);
                if (qualityName != null)
                    OutputQualityBox.SelectedItem = qualityName;

                // Click Apply
                ApplyButton.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent));
            }
        }
        catch (Exception ex)
        {
            var msgBox = new Window
            {
                Title = "Error",
                Width = 300,
                Height = 100,
                Content = new TextBlock { Text = $"Failed to import parameters:\n{ex.Message}", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
            };
            await msgBox.ShowDialog(this);
        }
    }

    private async void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        ProgressBar.IsVisible = true;
        ApplyButton.IsEnabled = false;
        ResetButton.IsEnabled = false;
        ImportButton.IsEnabled = false;
        SaveButton.IsEnabled = false;
        await Task.Delay(100); // UI update

        var selectedQuality = OutputQualityBox.SelectedItem?.ToString() ?? "Fast";
        var selectedResolution = QualityResolutionMap.GetResolution(selectedQuality) ?? (300, 200);
        int planeWidth = selectedResolution.Width;
        int planeHeight = selectedResolution.Height;

        var parameters = new ImageParameters(
            DateTime.UtcNow,
            SetNameBox.SelectedItem?.ToString() ?? "Mandelbrot",
            planeWidth,
            planeHeight,
            (double)(ZoomBox.Value ?? 1),
            (double)(MoveXBox.Value ?? 0),
            (double)(MoveYBox.Value ?? 0),
            (int)(IterationFactorBox.Value ?? 1),
            ColourWheelBox.SelectedItem?.ToString() ?? "",
            0);

        var matrix = await _engine.PopulateMatrix(parameters);
        var results = _engine.PopulateImage(parameters, matrix);
        _image = results.Item1;
        _imageParameters = results.Item2;

        using var ms = new MemoryStream();
        var imgSharp = results.Item1 as Image<Rgba32>;
        if (imgSharp != null)
        {
            if ((OutputTypeBox.SelectedItem?.ToString() ?? "PNG") == "PNG")
                imgSharp.Save(ms, new PngEncoder());
            else
                imgSharp.Save(ms, new JpegEncoder());
            ms.Seek(0, SeekOrigin.Begin);
            FractalImage.Source = new Bitmap(ms);
        }

        ProgressBar.IsVisible = false;
        ApplyButton.IsEnabled = true;
        ResetButton.IsEnabled = true;
        ImportButton.IsEnabled = true;
        SaveButton.IsEnabled = true;
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBox.Value = 1;
        MoveXBox.Value = 0;
        MoveYBox.Value = 0;
        OutputQualityBox.SelectedIndex = 0;
        IterationFactorBox.Value = 1;
        ApplyButton_Click(this, new RoutedEventArgs());
    }

    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_imageParameters == null || FractalImage.Source == null)
            return;

        var dialog = new Window
        {
            Width = 400,
            Height = 120,
            Title = "Save Fractal Image"
        };
        var filenamePrefix = new StringBuilder();
        filenamePrefix.Append(DateTime.UtcNow.ToString("yyyyMMddHHmmss"))
            .Append('_')
            .Append(SetNameBox.SelectedItem?.ToString() ?? "Mandelbrot")
            .Append('_')
            .Append(ColourWheelBox.SelectedItem?.ToString() ?? "Colour");
        var textBox = new TextBox
        {
            Width = 350,
            Margin = new Thickness(10),
            Text = $"{filenamePrefix}.{(OutputTypeBox.SelectedItem?.ToString() ?? "png").ToLower()}"
        };
        var okButton = new Button
        {
            Content = "Save",
            Width = 80,
            Margin = new Thickness(10)
        };
        var panel = new StackPanel();
        panel.Children.Add(textBox);
        panel.Children.Add(okButton);
        dialog.Content = panel;
        string? filePath = null;
        okButton.Click += (_, __) => { filePath = textBox.Text; dialog.Close(); };
        await dialog.ShowDialog(this);
        if (!string.IsNullOrWhiteSpace(filePath))
        {
            using var ms = new MemoryStream();

            if (_image != null)
            {
                if ((OutputTypeBox.SelectedItem?.ToString() ?? "PNG") == "PNG")
                    _image.Save(ms, new PngEncoder());
                else
                    _image.Save(ms, new JpegEncoder());
                ms.Seek(0, SeekOrigin.Begin);
                File.WriteAllBytes(filePath, ms.ToArray());
            }

            // Save parameters as JSON
            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            var parametersJSON = JsonSerializer.Serialize(this._imageParameters, serializeOptions);
            File.WriteAllText(filePath + ".parameters.json", parametersJSON);

            var msgBox = new Window
            {
                Title = "Closes in 15 seconds",
                Width = 300,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                Content = new StackPanel {
                    Children = {
                        new TextBlock {
                            Text = "Save Complete!",
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            FontSize = 18,
                            Margin = new Thickness(10)
                        }
                    }
                }
            };
            var _ = Task.Run(async () => {
                await Task.Delay(15000);
                await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => msgBox.Close());
            });
            await msgBox.ShowDialog(this);
        }
    }

    private void FractalImage_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_imageParameters == null)
        {
            var msgBox = new Window
            {
                Title = "Info",
                Width = 300,
                Height = 100,
                Content = new TextBlock { Text = "Please Apply or Reset the form.", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
            };
            msgBox.ShowDialog(this);
            return;
        }

        try
        {
            // Get controls
            var zoomAmount = ZoomBox.Value ?? 1;
            var moveXAmount = MoveXBox.Value ?? 0;
            var moveYAmount = MoveYBox.Value ?? 0;

            // Mouse button logic
            var props = e.GetCurrentPoint(FractalImage);
            if (props.Properties.IsLeftButtonPressed)
            {
                zoomAmount *= 2;
            }
            else if (props.Properties.IsRightButtonPressed)
            {
                zoomAmount /= 2;
            }
            ZoomBox.Value = zoomAmount;

            // Matrix extents
            var extents = _imageParameters.MatrixExtents;
            var maxX = extents.BottomRightExtent.Item1;
            var maxY = extents.TopLeftExtent.Item2;
            var minX = extents.TopLeftExtent.Item1;
            var minY = extents.BottomRightExtent.Item2;

            var spanX = maxX - minX;
            var spanY = maxY - minY;

            // Get pointer position relative to image
            var pt = e.GetPosition(FractalImage);
            var imgWidth = FractalImage.Bounds.Width;
            var imgHeight = FractalImage.Bounds.Height;
            var portionX = pt.X / imgWidth;
            var portionY = pt.Y / imgHeight;

            var changeX = spanX * portionX;
            var changeY = spanY * portionY;

            var xOffset = minX + changeX;
            var yOffset = maxY - changeY;

            MoveXBox.Value = (decimal)xOffset;
            MoveYBox.Value = (decimal)yOffset;

            // Trigger Apply
            ApplyButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        catch (Exception)
        {
            var msgBox = new Window
            {
                Title = "Error",
                Width = 300,
                Height = 100,
                Content = new TextBlock { Text = "Something failed in rendering. Zoomed in or out too far?", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
            };
            msgBox.ShowDialog(this);
        }
    }
}