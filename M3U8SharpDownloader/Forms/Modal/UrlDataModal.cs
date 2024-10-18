using LForms.Controls.Base;
using LForms.Controls.Buttons;
using LForms.Controls.Forms;
using LForms.Controls.Panels;
using LForms.Controls.TextBoxes;
using LForms.Extensions;
using M3U8SharpDownloader.Converter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms.Modal;

internal sealed class UrlDataModal : LealForm
{
    internal delegate void LoadUrlData(UrlData urlData);
    internal event LoadUrlData? OnLoadUrlData;

    internal UrlDataModal(string title)
    {
        Text = title;
        TopMost = true;
        TopLevel = true;
        ShowIcon = true;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;
    }

    public override void LoadComponents()
    {
        Size = new Size(400, 600);
        var panel = new LealPanel()
        {
            Dock = DockStyle.Fill,
        };
        this.Add(panel);

        var inputUrl = new LealTextBox()
        {
            Width = 350,
            Height = 50,
            PlaceHolder = "Your Url",
        };
        var inputTitle = new LealTextBox()
        {
            Width = 350,
            Height = 50,
            PlaceHolder = "Movie/Serie title",
        };
        var lealbutton = new LealButton()
        {
            Width = 350,
        };

        panel.Add(inputUrl);
        panel.Add(inputTitle);
        panel.Add(lealbutton);

        inputUrl.HorizontalCentralize(panel);
        inputTitle.HorizontalCentralize(panel);
        lealbutton.HorizontalCentralize(panel);
        panel.Controls.WaterFallControlsOfType<Control>(20, 25);
    }
}