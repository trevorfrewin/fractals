
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Controls.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tfrewin.play.fractal.start.engine;
using tfrewin.play.fractal.start.utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace tfrewin.play.fractal.avalonia;

public partial class MainWindow : Window
{
    private ImageParameters? _imageParameters;
    private MatrixEngine _engine = new();

    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools();
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Populate ComboBoxes
        var colourWheels = new ColourWheelGenerator().GenerateColourWheels();

        ColourWheelBox.ItemsSource = colourWheels.Select(cw => cw.ColourWheelName).ToList();
        ColourWheelBox.SelectedIndex = 0;

        SetNameBox.ItemsSource = new List<string> { "mandelbrot", "julia" };
        SetNameBox.SelectedIndex = 0;

        OutputQualityBox.ItemsSource = new List<string> { "Fast", "Medium", "High Quality", "Better", "Best", "Extreme", "Insane" };
        OutputQualityBox.SelectedIndex = 0;

        OutputTypeBox.ItemsSource = new List<string> { "JPG", "PNG" };
        OutputTypeBox.SelectedIndex = 0;

        // Button events
        ApplyButton.Click += ApplyButton_Click;
        ResetButton.Click += ResetButton_Click;
        SaveButton.Click += SaveButton_Click;
        FractalImage.PointerPressed += FractalImage_PointerPressed;
    }

    private async void ApplyButton_Click(object? sender, RoutedEventArgs e)
    {
        ProgressBar.IsVisible = true;
        ApplyButton.IsEnabled = false;
        ResetButton.IsEnabled = false;
        SaveButton.IsEnabled = false;
        await Task.Delay(100); // UI update

        int planeWidth = 300, planeHeight = 200;
        switch (OutputQualityBox.SelectedItem)
        {
            case "Fast": break;
            case "Medium": planeWidth *= 3; planeHeight *= 3; break;
            case "High Quality": planeWidth *= 8; planeHeight *= 8; break;
            case "Better": planeWidth *= 15; planeHeight *= 15; break;
            case "Best": planeWidth *= 20; planeHeight *= 20; break;
            case "Extreme": planeWidth *= 30; planeHeight *= 30; break;
            case "Insane": planeWidth *= 60; planeHeight *= 60; break;
        }


        var parameters = new ImageParameters(
            DateTime.UtcNow,
            SetNameBox.SelectedItem?.ToString() ?? "mandelbrot",
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
        SaveButton.IsEnabled = true;
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        ZoomBox.Value = 1;
        MoveXBox.Value = 0;
        MoveYBox.Value = 0;
        OutputQualityBox.SelectedIndex = 0;
        ApplyButton_Click(this, new RoutedEventArgs());
    }

    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_imageParameters == null || FractalImage.Source == null)
            return;

        // Fallback: prompt for file path using a simple dialog
        var dialog = new Window
        {
            Width = 400,
            Height = 120,
            Title = "Save Fractal Image"
        };
        var textBox = new TextBox { Width = 350, Margin = new Thickness(10), Text = $"fractal.{(OutputTypeBox.SelectedItem?.ToString() ?? "png").ToLower()}" };
        var okButton = new Button { Content = "Save", Width = 80, Margin = new Thickness(10) };
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
            var imgSharp2 = _engine.PopulateImage(_imageParameters, await _engine.PopulateMatrix(_imageParameters)).Item1 as Image<Rgba32>;
            if (imgSharp2 != null)
            {
                if ((OutputTypeBox.SelectedItem?.ToString() ?? "PNG") == "PNG")
                    imgSharp2.Save(ms, new PngEncoder());
                else
                    imgSharp2.Save(ms, new JpegEncoder());
                ms.Seek(0, SeekOrigin.Begin);
                File.WriteAllBytes(filePath, ms.ToArray());
            }
        }
    }

    private void FractalImage_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (_imageParameters == null)
        {
            var msgBox = new Window { Title = "Info", Width = 300, Height = 100, Content = new TextBlock { Text = "Please Apply or Reset the form.", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center } };
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
            var msgBox = new Window { Title = "Error", Width = 300, Height = 100, Content = new TextBlock { Text = "Something failed in rendering. Zoomed in or out too far?", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center } };
            msgBox.ShowDialog(this);
        }
    }
}