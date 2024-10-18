using LForms.Controls.Buttons;
using LForms.Controls.Forms;
using LForms.Extensions;
using M3U8SharpDownloader.Forms.Tabs;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms;

public sealed class MainForm : LealForm
{
    private readonly Panel _leftPanel = new();
    private readonly Panel _container = new();

    public MainForm(string text)
    {
        Text = text;
    }

    public override void LoadComponents()
    {
        _container.Dock = DockStyle.Fill;
        this.Add(_container);

        BackColor = Color.White.Darken(0.75);

        _leftPanel.Width = 250;
        _leftPanel.Dock = DockStyle.Left;
        _leftPanel.BackColor = Color.DeepSkyBlue;
        this.Add(_leftPanel);

        var topPanel = new Panel()
        {
            Height = 100,
            Dock = DockStyle.Top,
        };
        var homeButton = GenerateSelectableButton("Home", true, new MainTab());
        var configurationButton = GenerateSelectableButton("Configuration", false);
        var downloadButton = GenerateSelectableButton("Download", false);
        var historyButton = GenerateSelectableButton("History", false);
        var supportButton = GenerateSelectableButton("Help/Support", false);

        var labelAppName = new Label()
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = "M3U8SharpDownloader",
            ForeColor = Color.NavajoWhite,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        labelAppName.Font = new Font(labelAppName.Font.FontFamily, 12, FontStyle.Bold);
        topPanel.Add(labelAppName);

        var v = Assembly.GetExecutingAssembly().GetName().Version!;
        var bottomInfo = new Label()
        {
            Height = 100,
            AutoSize = false,
            Dock = DockStyle.Bottom,
            Text = $"Made by: Swellshinider\nv{v.Major}.{v.Minor}.{v.Build}.{v.Revision}",
            ForeColor = Color.NavajoWhite,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        _leftPanel.Add(bottomInfo);

        _leftPanel.Add(topPanel);
        _leftPanel.Add(homeButton);
        _leftPanel.Add(configurationButton);
        _leftPanel.Add(downloadButton);
        _leftPanel.Add(historyButton);
        _leftPanel.Add(supportButton);
        _leftPanel.Controls.WaterFallControlsOfType<LealSelectableButton>(topPanel.Location.Y + topPanel.Height + 5, 5);

        TabClick(homeButton.ObjectRef as Panel);
    }

    private LealSelectableButton GenerateSelectableButton(string title, bool selected, Panel? tab = null)
    {
        var selectableButton = new LealSelectableButton(tab)
        {
            Height = 50,
            Width = 225,
            Text = title,
            BorderSize = 0,
            AutoSearch = true,
            Selected = selected,
            ForeColor = Color.NavajoWhite,
            SelectedColor = _container.BackColor,
            UnSelectedColor = _leftPanel.BackColor,
        };
        selectableButton.GenerateCustomRoundRegion(25, true, false, true, false);
        selectableButton.SetXFromLeft(_leftPanel, 0);
        selectableButton.Click += (s, e) => TabClick(tab);
        return selectableButton;
    }

    private void TabClick(Panel? tab)
    {
        _container.Controls.Clear();

        if (tab != null)
            _container.Add(tab);
    }
}