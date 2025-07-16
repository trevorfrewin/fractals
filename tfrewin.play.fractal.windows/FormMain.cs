using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using tfrewin.play.fractal.start;
using tfrewin.play.fractal.start.processor;
using tfrewin.play.fractal.start.utilities;
using SixLabors.ImageSharp;


namespace tfrewin.play.fractal.windows;

public partial class FormMain : Form
{
    private tfrewin.play.fractal.start.ImageParameters _imageParameters = null;

    public FormMain()
    {
        InitializeComponent();

        this.Text = "Fractal Play Place";

        var layoutBox = new GroupBox
        {
            Left = 10,
            Text = "Layout Choices",
            ClientSize = new System.Drawing.Size(250, 275)
        };

        Label label = AddColourWheelControl(20, 20, layoutBox);
        label = AddSetNameControl(label.Top + 30, label.Left, layoutBox);
        label = AddPlaneWidthControl(label.Top + 30, label.Left, layoutBox);
        label = AddPlaneHeightControl(label.Top + 30, label.Left, layoutBox);
        label = AddZoomControl(label.Top + 30, label.Left, layoutBox);
        label = AddMoveXControl(label.Top + 30, label.Left, layoutBox);
        label = AddMoveYControl(label.Top + 30, label.Left, layoutBox);
        label = AddIterationFactorControl(label.Top + 30, label.Left, layoutBox);

        this.Controls.Add(layoutBox);

        var outputBox = new GroupBox
        {
            Left = layoutBox.Left + layoutBox.Width + 10,
            Text = "Output",
            ClientSize = new System.Drawing.Size(800, 600)
        };

        var imageContainer = new PictureBox
        {
            Left = 20,
            Top = 20,
            Size = new System.Drawing.Size(760, 560),
            SizeMode = PictureBoxSizeMode.StretchImage,
            Name = "ImageContainer"
        };
        imageContainer.Click += imageContainer_Click;

        outputBox.Controls.Add(imageContainer);
        this.Controls.Add(outputBox);

        var buttonWidth = 80;
        var applyButton = new Button
        {
            Text = "Apply",
            Top = layoutBox.Top + layoutBox.Height + 10,
            Width = buttonWidth,
            Left = (layoutBox.Left + layoutBox.Width) - buttonWidth,
            Name = "Apply"
        };
        applyButton.Click += applyButton_Click;

        this.Controls.Add(applyButton);

        var resetButton = new Button
        {
            Text = "Reset",
            Top = layoutBox.Top + layoutBox.Height + applyButton.Height + 20,
            Width = buttonWidth,
            Left = (layoutBox.Left + layoutBox.Width) - buttonWidth,
            Name = "Reset"
        };
        resetButton.Click += resetButton_Click;

        this.Controls.Add(resetButton);
    }

    private void imageContainer_Click(object sender, EventArgs e)
    {
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

            var currentBox = (PictureBox)sender;
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
        catch (Exception ex)
        {
            MessageBox.Show(this, "Something failed in rendering. Zoomed in or out too far?");
        }
    }

    private void applyButton_Click(object sender, EventArgs e)
    {
        var setNameControl = (ComboBox)this.Controls.Find("SetName", true).First();
        var zoomControl = (NumericUpDown)this.Controls.Find("Zoom", true).First();
        var planeWidthControl = (NumericUpDown)this.Controls.Find("PlaneWidth", true).First();
        var planeHeightControl = (NumericUpDown)this.Controls.Find("PlaneHeight", true).First();
        var moveXControl = (NumericUpDown)this.Controls.Find("MoveX", true).First();
        var moveYControl = (NumericUpDown)this.Controls.Find("MoveY", true).First();
        var iterationFactorControl = (NumericUpDown)this.Controls.Find("IterationFactor", true).First();
        var colourWheelControl = (ComboBox)this.Controls.Find("ColourWheel", true).First();

        var program = new tfrewin.play.fractal.start.Program();
        Tuple<SixLabors.ImageSharp.Image, ImageParameters> results
            = program.PaintFile(
                new ImageParameters(
                    DateTime.UtcNow,
                    setNameControl.Text,
                    (int)planeWidthControl.Value,
                    (int)planeHeightControl.Value,
                    (double)zoomControl.Value,
                    (double)moveXControl.Value,
                    (double)moveYControl.Value,
                    (int)iterationFactorControl.Value,
                    colourWheelControl.Text, 0));

        this._imageParameters = results.Item2; // Has MatrixExtents on this after engine execution.

        var interimStream = new MemoryStream();
        results.Item1.SaveAsJpeg(interimStream);
        interimStream.Seek(0, SeekOrigin.Begin);

        var image = System.Drawing.Image.FromStream(interimStream);
        var pictureBox = (PictureBox)this.Controls.Find("ImageContainer", true).First();
        pictureBox.Image = image;
    }

    private void resetButton_Click(object sender, EventArgs e)
    {
        var zoomControl = (NumericUpDown)this.Controls.Find("Zoom", true).First();
        var moveXControl = (NumericUpDown)this.Controls.Find("MoveX", true).First();
        var moveYControl = (NumericUpDown)this.Controls.Find("MoveY", true).First();

        zoomControl.Value = 1;
        moveXControl.Value = 0;
        moveYControl.Value = 0;

        var applyButton = (Button)this.Controls.Find("Apply", true).First();
        applyButton.PerformClick();
    }

    private Label AddColourWheelControl(int top, int left, GroupBox owner)
    {
        var colourWheelLabel = new Label
        {
            Text = "Colour Wheel:",
            Top = top,
            Left = left
        };
        owner.Controls.Add(colourWheelLabel);

        var colourWheels = new ColourWheelGenerator().GenerateColourWheels();
        List<string> colourWheelNames = colourWheels.Select((wheel) => wheel.ColourWheelName).ToList();
        var colourWheelBox = new ComboBox
        {
            DataSource = colourWheelNames,
            Text = "Colour Wheel:",
            Top = colourWheelLabel.Top - 5,
            Left = colourWheelLabel.Left + 100,
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
            Left = setNameLabel.Left + 100,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Name = "SetName"
        };
        owner.Controls.Add(setNameBox);

        return setNameLabel;
    }

    private Label AddPlaneWidthControl(int top, int left, GroupBox owner)
    {
        var planeWithLabel = new Label
        {
            Text = "Plane Width:",
            Top = top,
            Left = left
        };
        owner.Controls.Add(planeWithLabel);

        var planeWidthBox = new NumericUpDown
        {
            Text = "Plane Width:",
            Maximum = 24000,
            Minimum = 400,
            Value = 600,
            Increment = 100,
            Top = planeWithLabel.Top - 5,
            Left = planeWithLabel.Left + 100,
            Name = "PlaneWidth"
        };
        owner.Controls.Add(planeWidthBox);

        return planeWithLabel;
    }

    private Label AddPlaneHeightControl(int top, int left, GroupBox owner)
    {
        var planeHeightLabel = new Label
        {
            Text = "Plane Height:",
            Top = top,
            Left = left
        };
        owner.Controls.Add(planeHeightLabel);

        var planeHeightBox = new NumericUpDown
        {
            Text = "Plane Height:",
            Maximum = 8000,
            Minimum = 200,
            Value = 400,
            Increment = 100,
            Top = planeHeightLabel.Top - 5,
            Left = planeHeightLabel.Left + 100,
            Name = "PlaneHeight"
        };
        owner.Controls.Add(planeHeightBox);

        return planeHeightLabel;
    }

    private Label AddZoomControl(int top, int left, GroupBox owner)
    {
        var zoomLabel = new Label
        {
            Text = "Zoom:",
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
            Left = zoomLabel.Left + 100,
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
            Left = moveXLabel.Left + 100,
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
            Left = moveYLabel.Left + 100,
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
            Top = top,
            Left = left
        };
        owner.Controls.Add(iterationFactorLabel);

        var iterationFactorBox = new NumericUpDown
        {
            Text = "Iteration Factor:",
            DecimalPlaces = 2,
            Maximum = 4,
            Minimum = 0.5M,
            Value = 1,
            Increment = 0.1M,
            Top = iterationFactorLabel.Top - 5,
            Left = iterationFactorLabel.Left + 100,
            Name = "IterationFactor"
        };
        owner.Controls.Add(iterationFactorBox);

        return iterationFactorLabel;
    }
}
