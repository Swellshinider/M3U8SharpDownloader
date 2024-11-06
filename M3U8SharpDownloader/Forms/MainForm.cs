using LForms.Controls.Buttons;
using LForms.Controls.Forms;
using LForms.Controls.Mischellaneous;
using LForms.Controls.Panels;
using LForms.Extensions;
using M3U8SharpDownloader.Forms.Tabs;
using M3U8SharpDownloader.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms;

public sealed class MainForm : LealForm
{
    private readonly LealPanel _topPanel = new(true);
    private readonly LealPanel _leftPanel = new(true);
    private readonly LealPanel _container = new(true);
    private readonly List<Size> _sizes = [
        new Size(1280, 720),
        new Size(1600, 900),
        new Size(1920, 1080)
    ];
    private int _currentSizeIndex = 2;

    public MainForm()
    {
        Text = "M3U8SharpDownloader | by: Swellshinider";
        KeyPreview = true;
        KeyDown += MainForm_KeyDown;
        FormBorderStyle = FormBorderStyle.None;
        BackColor = ColorPallete.MainBackgroundColor;
        Icon = Resource.ResourceManager.GetObject("M3U8SharpDownloader") as Icon;
        _currentSizeIndex = Settings.Default.SizeIndex >= _sizes.Count
            ? _sizes.Count - 1
            : _currentSizeIndex;
        SetFixedSize(_sizes[_currentSizeIndex].Width, _sizes[_currentSizeIndex].Height);
    }

    public override void ReDraw()
    {
        this.GenerateRoundRegion();
    }

    public override void LoadComponents()
    {
        _topPanel.Height = 48;
        _leftPanel.Width = 250;

        _topPanel.Dock = DockStyle.Top;
        _leftPanel.Dock = DockStyle.Left;
        _container.Dock = DockStyle.Fill;

        this.Add(_container);
        this.Add(_leftPanel);
        this.Add(_topPanel);

        /// =================
        /// Top Panel 
        /// =================
        var textLabel = new Label()
        {
            Text = "M3U8SharpDownloader",
            AutoSize = false,
            Width = 1000,
            ForeColor = ColorPallete.TextFontColor,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Rubik", 14, FontStyle.Regular),
        };
        _topPanel.Add(textLabel);
        textLabel.DockTopBottomLeftWithPadding(3, 3, 5);
        textLabel.Width = textLabel.Text.GetTextSize(textLabel.Font).Width + 10;
        textLabel.MouseDown += (s, e) => Handle.DragWindowOnMouseDown(e);

        var closeButton = new LealButton((s, e) => Application.Exit())
        {
            Text = "X",
            BorderSize = 0,
            Dock = DockStyle.Right,
            Width = _topPanel.Height,
            MouseHoverColor = Color.Red,
            MouseDownColor = Color.Red.Darken(0.1),
        };
        _topPanel.Add(closeButton);


        /// =================
        /// Left Panel
        /// =================
        _leftPanel.Add(new LealSeparator()
        {
            Dock = DockStyle.Right,
            Width = 1,
            LineSpacing = 0,
            LineThickness = 2,
            Orientation = Orientation.Vertical,
            LineColor = ColorPallete.HighLightColor,
        });

        _container.Add(new HomeTab());
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!e.Control)
            return;

        var previousSizeIndex = _currentSizeIndex;

        if (e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.Add)
        {
            if (_currentSizeIndex < _sizes.Count - 1)
                _currentSizeIndex++;
        }
        else if (e.KeyCode == Keys.OemMinus || e.KeyCode == Keys.Subtract)
        {
            if (_currentSizeIndex > 0)
                _currentSizeIndex--;
        }

        if (previousSizeIndex == _currentSizeIndex)
            return;

        SetFixedSize(_sizes[_currentSizeIndex].Width, _sizes[_currentSizeIndex].Height);
        Settings.Default.SizeIndex = _currentSizeIndex;
        Settings.Default.Save();
    }
}