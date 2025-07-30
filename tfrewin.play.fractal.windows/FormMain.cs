using System.Drawing;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SixLabors.ImageSharp;

using tfrewin.play.fractal.start.engine;
using tfrewin.play.fractal.start.processor;
using tfrewin.play.fractal.start.utilities;

namespace tfrewin.play.fractal.windows;

public partial class FormMain : Form
{
    private ImageParameters _imageParameters = null!;

    private const int _labelWidth = 140;

    public FormMain()
    {
        this.Text = "Fractal Play Place";
        this.Size = new System.Drawing.Size(1500, 1000);

        var splitControlBase = new SplitContainer
        {
            TabIndex = 0,
            Name = "MainSplitContainer",
            Panel1MinSize = 320,
            Dock = DockStyle.Fill,
            Width = this.Width
        };
        splitControlBase.Panel2MinSize = 1000;

        var layoutBox = new GroupBox
        {
            Name = "Layout",
            Width = 370,
            Dock = DockStyle.Fill
        };

        Label label = AddColourWheelControl(30, 20, layoutBox);
        label = AddSetNameControl(label.Top + 30, label.Left, layoutBox);
        label = AddOutputQualityControl(label.Top + 30, label.Left, layoutBox);
        label = AddOutputTypeControl(label.Top + 30, label.Left, layoutBox);
        label = AddZoomControl(label.Top + 30, label.Left, layoutBox);
        label = AddMoveXControl(label.Top + 30, label.Left, layoutBox);
        label = AddMoveYControl(label.Top + 30, label.Left, layoutBox);
        label = AddIterationFactorControl(label.Top + 30, label.Left, layoutBox);

        var buttonWidth = 80;
        var applyButton = new Button
        {
            Text = "Apply",
            Height = 40,
            Width = buttonWidth,
            Name = "Apply",
            Top = label.Top + label.Height + 20,
            Left = 20
        };
        applyButton.Click += applyButton_Click;
        layoutBox.Controls.Add(applyButton);

        var spinnerContainer = new PictureBox
        {
            Height = 40,
            Width = 40,
            Left = applyButton.Left + buttonWidth + 20,
            Name = "SpinnerContainer",
            Top = applyButton.Top + 10,
            Visible = false
        };
        spinnerContainer.Load("./resources/spinner.gif");
        layoutBox.Controls.Add(spinnerContainer);

        var resetButton = new Button
        {
            Text = "Reset",
            Height = 40,
            Width = buttonWidth,
            Left = applyButton.Left,
            Name = "Reset",
            Top = applyButton.Top + applyButton.Height + 20
        };
        resetButton.Click += resetButton_Click;
        layoutBox.Controls.Add(resetButton);

        var saveButton = new Button
        {
            Text = "Save",
            Height = 40,
            Width = buttonWidth,
            Left = applyButton.Left,
            Name = "Save",
            Top = resetButton.Top + resetButton.Height + 20
        };
        saveButton.Click += saveButton_Click;
        layoutBox.Controls.Add(saveButton);

        var imageContainer = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Name = "ImageContainer"
        };
        imageContainer.Click += imageContainer_Click;

        splitControlBase.Panel1.Controls.Add(layoutBox);
        splitControlBase.Panel2.Controls.Add(imageContainer);

        this.Controls.Add(splitControlBase);

        InitializeComponent();
    }

    private void imageContainer_Click(object? sender, EventArgs e)
    {
        if (this._imageParameters == null)
        {
            MessageBox.Show("Please Apply or Reset the form.");
            return;
        }

        try
        {
            var zoomControl = (NumericUpDown)this.Controls.Find("Zoom", true).First();
            var moveXControl = (NumericUpDown)this.Controls.Find("MoveX", true).First();
            var moveYControl = (NumericUpDown)this.Controls.Find("MoveY", true).First();

            var zoomAmount = zoomControl.Value;

            var mouseEvent = (MouseEventArgs)e;

            if (mouseEvent.Button == MouseButtons.Left)
            {
                zoomAmount *= 2;
            }

            if (mouseEvent.Button == MouseButtons.Right)
            {
                zoomAmount /= 2;
            }
            zoomControl.Value = zoomAmount;

            // The extents of the underlying cartesian plane
            var maxX = this._imageParameters.MatrixExtents.BottomRightExtent.Item1;
            var maxY = this._imageParameters.MatrixExtents.TopLeftExtent.Item2;
            var minX = this._imageParameters.MatrixExtents.TopLeftExtent.Item1;
            var minY = this._imageParameters.MatrixExtents.BottomRightExtent.Item2;

            var spanX = maxX - minX;
            var spanY = maxY - minY;

            var currentBox = (PictureBox)sender!;
            var portionX = (double)mouseEvent.Location.X / currentBox.Width;
            var portionY = (double)mouseEvent.Location.Y / currentBox.Height;

            var changeX = spanX * portionX;
            var changeY = spanY * portionY;

            var xOffset = minX + changeX;
            var yOffset = maxY - changeY;

            moveXControl.Value = (decimal)xOffset;
            moveYControl.Value = (decimal)yOffset;

            var applyButton = (Button)this.Controls.Find("Apply", true).First();
            applyButton.PerformClick();
        }
        catch (Exception)
        {
            MessageBox.Show(this, "Something failed in rendering. Zoomed in or out too far?");
        }
    }

    private async void applyButton_Click(object? sender, EventArgs e)
    {
        var setNameControl = (ComboBox)this.Controls.Find("SetName", true).First();
        var zoomControl = (NumericUpDown)this.Controls.Find("Zoom", true).First();
        var outputQualityControl = (ComboBox)this.Controls.Find("OutputQuality", true).First();
        var outputTypeControl = (ComboBox)this.Controls.Find("OutputType", true).First();
        var moveXControl = (NumericUpDown)this.Controls.Find("MoveX", true).First();
        var moveYControl = (NumericUpDown)this.Controls.Find("MoveY", true).First();
        var iterationFactorControl = (NumericUpDown)this.Controls.Find("IterationFactor", true).First();
        var colourWheelControl = (ComboBox)this.Controls.Find("ColourWheel", true).First();

        var planeWidth = 300;
        var planeHeight = 200;
        switch (outputQualityControl.Text)
        {
            case "Fast":
                {
                    planeWidth *= 1;
                    planeHeight *= 1;
                    break;
                }
            case "Medium":
                {
                    planeWidth *= 3;
                    planeHeight *= 3;
                    break;
                }
            case "High Quality":
                {
                    planeWidth *= 8;
                    planeHeight *= 8;
                    break;
                }
            case "Better":
                {
                    planeWidth *= 15;
                    planeHeight *= 15;
                    break;
                }
            case "Best":
                {
                    planeWidth *= 20;
                    planeHeight *= 20;
                    break;
                }
            case "Extreme":
                {
                    planeWidth *= 30;
                    planeHeight *= 30;
                    break;
                }
            case "Insane":
                {
                    planeWidth *= 60;
                    planeHeight *= 60;
                    break;
                }
        }

        var engine = new tfrewin.play.fractal.start.engine.MatrixEngine();

        var imageParameters = new ImageParameters(
                    DateTime.UtcNow,
                    setNameControl.Text,
                    planeWidth,
                    planeHeight,
                    (double)zoomControl.Value,
                    (double)moveXControl.Value,
                    (double)moveYControl.Value,
                    (int)iterationFactorControl.Value,
                    colourWheelControl.Text, 0);

        var matrix = await engine.PopulateMatrix(imageParameters);
        //Tuple<SixLabors.ImageSharp.Image, ImageParameters> results
        var results = engine.PopulateImage(imageParameters, matrix);

        this._imageParameters = results.Item2; // Has MatrixExtents on this after engine execution.

        var interimStream = new MemoryStream();
        switch (outputTypeControl.Text)
        {
            case "PNG":
                results.Item1.SaveAsPng(interimStream);
                break;
            case "JPG":
                results.Item1.SaveAsJpeg(interimStream);
                break;
        }
        interimStream.Seek(0, SeekOrigin.Begin);

        var image = System.Drawing.Image.FromStream(interimStream);
        var pictureBox = (PictureBox)this.Controls.Find("ImageContainer", true).First();
        pictureBox.Image = image;
    }

    private void resetButton_Click(object? sender, EventArgs e)
    {
        var zoomControl = (NumericUpDown)this.Controls.Find("Zoom", true).First();
        var moveXControl = (NumericUpDown)this.Controls.Find("MoveX", true).First();
        var moveYControl = (NumericUpDown)this.Controls.Find("MoveY", true).First();
        var outputQualityControl = (ComboBox)this.Controls.Find("OutputQuality", true).First();

        zoomControl.Value = 1;
        moveXControl.Value = 0;
        moveYControl.Value = 0;
        outputQualityControl.Text = "Fast";

        var applyButton = (Button)this.Controls.Find("Apply", true).First();
        applyButton.PerformClick();
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        if (this._imageParameters == null)
        {
            return;
        }

        var outputTypeControl = (ComboBox)this.Controls.Find("OutputType", true).First();

        var saveDialog = new SaveFileDialog();

        switch (outputTypeControl.Text)
        {
            case "PNG":
                saveDialog.Filter = "PNG (*.png)|*.png";
                break;
            case "JPG":
                saveDialog.Filter = "JPG (*.jpg)|*.jpg";
                break;    
        }

        var result = saveDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            String fileName = saveDialog.FileName;

            var pictureBox = (PictureBox)this.Controls.Find("ImageContainer", true).First();

            var imageBytes = (byte[])(new ImageConverter()).ConvertTo(pictureBox.Image, typeof(byte[]));
            File.WriteAllBytes(fileName, imageBytes);

            var serializeOptions = new JsonSerializerOptions { WriteIndented = true };
            var parametersJSON = JsonSerializer.Serialize(this._imageParameters, serializeOptions);
            File.WriteAllText(fileName + ".parameters.json", parametersJSON);
        }
    }

    private Label AddColourWheelControl(int top, int left, GroupBox owner)
    {
        var colourWheelLabel = new Label
        {
            Text = "Colour Wheel:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(colourWheelLabel);

        var colourWheels = new ColourWheelGenerator().GenerateColourWheels();
        List<string> colourWheelNames = colourWheels.OrderBy((wheel) => wheel.ColourWheelName).Select((wheel) => wheel.ColourWheelName).ToList();
        var colourWheelBox = new ComboBox
        {
            DataSource = colourWheelNames,
            Text = "Colour Wheel:",
            Top = colourWheelLabel.Top - 5,
            Left = colourWheelLabel.Left + _labelWidth + 20,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Name = "ColourWheel"
        };
        owner.Controls.Add(colourWheelBox);

        return colourWheelLabel;
    }

    private Label AddSetNameControl(int top, int left, GroupBox owner)
    {
        var setNameLabel = new Label
        {
            Text = "Fractal Set:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(setNameLabel);

        var setNames = new List<string> { "mandelbrot", "julia" };
        var setNameBox = new ComboBox
        {
            DataSource = setNames,
            Text = "Set Name:",
            Top = setNameLabel.Top - 5,
            Left = setNameLabel.Left + _labelWidth + 20,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Name = "SetName"
        };
        owner.Controls.Add(setNameBox);

        return setNameLabel;
    }

    private Label AddOutputQualityControl(int top, int left, GroupBox owner)
    {
        var outputQualityLabel = new Label
        {
            Text = "Output Quality:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(outputQualityLabel);

        var qualityOptions = new List<string> { "Fast", "Medium", "High Quality", "Better", "Best", "Extreme", "Insane" };
        var qualityOptionsBox = new ComboBox
        {
            DataSource = qualityOptions,
            Text = "Output Quality:",
            Top = outputQualityLabel.Top - 5,
            Left = outputQualityLabel.Left + _labelWidth + 20,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Name = "OutputQuality"
        };
        owner.Controls.Add(qualityOptionsBox);

        return outputQualityLabel;
    }

    private Label AddOutputTypeControl(int top, int left, GroupBox owner)
    {
        var outputTypeLabel = new Label
        {
            Text = "Output Type:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(outputTypeLabel);

        var outputOptions = new List<string> { "JPG", "PNG" };
        var outputTypeBox = new ComboBox
        {
            DataSource = outputOptions,
            Text = "Output Type:",
            Top = outputTypeLabel.Top - 5,
            Left = outputTypeLabel.Left + _labelWidth + 20,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Name = "OutputType"
        };
        owner.Controls.Add(outputTypeBox);

        return outputTypeLabel;
    }

    private Label AddZoomControl(int top, int left, GroupBox owner)
    {
        var zoomLabel = new Label
        {
            Text = "Zoom:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(zoomLabel);

        var zoomBox = new NumericUpDown
        {
            Text = "Zoom:",
            DecimalPlaces = 2,
            Maximum = 900000000000000,
            Minimum = 0.1M,
            Value = 1,
            Increment = 0.1M,
            Top = zoomLabel.Top - 5,
            Left = zoomLabel.Left + _labelWidth + 20,
            Name = "Zoom"
        };
        owner.Controls.Add(zoomBox);

        return zoomLabel;
    }

    private Label AddMoveXControl(int top, int left, GroupBox owner)
    {
        var moveXLabel = new Label
        {
            Text = "Move X:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(moveXLabel);

        var moveXBox = new NumericUpDown
        {
            Text = "Move X:",
            DecimalPlaces = 15,
            Maximum = 8,
            Minimum = -8,
            Value = 0,
            Increment = 0.01M,
            Top = moveXLabel.Top - 5,
            Left = moveXLabel.Left + _labelWidth + 20,
            Name = "MoveX"
        };
        owner.Controls.Add(moveXBox);

        return moveXLabel;
    }

    private Label AddMoveYControl(int top, int left, GroupBox owner)
    {
        var moveYLabel = new Label
        {
            Text = "Move Y:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(moveYLabel);

        var moveYBox = new NumericUpDown
        {
            Text = "Move Y:",
            DecimalPlaces = 15,
            Maximum = 8,
            Minimum = -8,
            Value = 0,
            Increment = 0.01M,
            Top = moveYLabel.Top - 5,
            Left = moveYLabel.Left + _labelWidth + 20,
            Name = "MoveY"
        };
        owner.Controls.Add(moveYBox);

        return moveYLabel;
    }

    private Label AddIterationFactorControl(int top, int left, GroupBox owner)
    {
        var iterationFactorLabel = new Label
        {
            Text = "Iteration Factor:",
            Width = _labelWidth,
            Top = top,
            Left = left
        };
        owner.Controls.Add(iterationFactorLabel);

        var iterationFactorBox = new NumericUpDown
        {
            Text = "Iteration Factor:",
            DecimalPlaces = 0,
            Maximum = 4,
            Minimum = 1,
            Value = 1,
            Increment = 1,
            Top = iterationFactorLabel.Top - 5,
            Left = iterationFactorLabel.Left + _labelWidth + 20,
            Name = "IterationFactor"
        };
        owner.Controls.Add(iterationFactorBox);

        return iterationFactorLabel;
    }
}
