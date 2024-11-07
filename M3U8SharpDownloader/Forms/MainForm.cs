using LForms.Controls.Buttons;
using LForms.Controls.Forms;
using LForms.Controls.Panels;
using LForms.Extensions;
using M3U8SharpDownloader.Forms.Tabs;
using M3U8SharpDownloader.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms;

public sealed class MainForm : LealForm
{
    private readonly LealPanel _topPanel = new(true);
    private readonly LealPanel _leftPanel = new(true);
    private readonly LealPanel _container = new(true);
    private readonly List<BaseTab> _tabs = [];
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
            : Settings.Default.SizeIndex;
        SetFixedSize(_sizes[_currentSizeIndex].Width, _sizes[_currentSizeIndex].Height);
        Location = new Point(
            (Screen.PrimaryScreen!.WorkingArea.Width - Width) / 2,
            (Screen.PrimaryScreen!.WorkingArea.Height - Height) / 2);
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
        var configButton = new LealButton()
        {
            Text = "C",
            BorderSize = 0,
            Dock = DockStyle.Right,
            Width = _topPanel.Height,
        };
        var minimizeButton = new LealButton((s, e) => WindowState = FormWindowState.Minimized)
        {
            Text = "-",
            BorderSize = 0,
            Dock = DockStyle.Right,
            Width = _topPanel.Height,
        };

        _topPanel.Add(minimizeButton);
        _topPanel.Add(configButton);
        _topPanel.Add(closeButton);


        /// =================
        /// Left Panel
        /// =================
        /// 
        ChangeTab(typeof(HomeTab));
    }

    private void ChangeTab(Type tabType)
    {
        if (tabType == null || !typeof(BaseTab).IsAssignableFrom(tabType))
            return;

        // Check if a tab of the given type already exists
        var existingTab = _tabs.FirstOrDefault(t => t.GetType() == tabType);

        if (existingTab != null)
        {
            // Reload the existing tab
            _container.Controls.Clear();
            _container.Add(existingTab);
        }
        else
        {
            // Create a new instance of the tab and add it to the list
            var newTab = Activator.CreateInstance(tabType) as BaseTab ??
                throw new Exception("Could not instantiate tab with activator");

            _tabs.Add(newTab);
            _container.Controls.Clear();
            _container.Add(newTab);
        }
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
        Location = new Point(
            (Screen.PrimaryScreen!.WorkingArea.Width - Width) / 2,
            (Screen.PrimaryScreen!.WorkingArea.Height - Height) / 2);
        Settings.Default.SizeIndex = _currentSizeIndex;
        Settings.Default.Save();
    }
}