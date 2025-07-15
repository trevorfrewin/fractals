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

        outputBox.Controls.Add(imageContainer);
        this.Controls.Add(outputBox);

        var applyButtonWidth = 80;
        var applyButton = new Button
        {
            Text = "&Apply",
            Top = layoutBox.Top + layoutBox.Height + 10,
            Width = applyButtonWidth,
            Left = (layoutBox.Left + layoutBox.Width) - applyButtonWidth
        };
        applyButton.Click += MyNewButton_Click;

        this.Controls.Add(applyButton);
    }

    private void MyNewButton_Click(object sender, EventArgs e)
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

        var interimStream = new MemoryStream();
        results.Item1.SaveAsJpeg(interimStream);
        interimStream.Seek(0, SeekOrigin.Begin);

        var image = System.Drawing.Image.FromStream(interimStream);
        var pictureBox = (PictureBox)this.Controls.Find("ImageContainer", true).First();
        pictureBox.Image = image;
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
            DecimalPlaces = 5,
            Maximum = 100000,
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
            DecimalPlaces = 5,
            Maximum = 2,
            Minimum = -2,
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
            DecimalPlaces = 5,
            Maximum = 2,
            Minimum = -2,
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
            DecimalPlaces = 5,
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
