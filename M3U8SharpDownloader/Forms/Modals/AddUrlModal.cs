using LForms;
using LForms.Controls.Buttons;
using LForms.Controls.Forms;
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

namespace M3U8SharpDownloader.Forms.Modals;

internal sealed class AddUrlModal : LealForm
{
    internal event EventHandler<UrlData>? UrlDataAdd;

    internal AddUrlModal(Point openLocation)
    {
        StartPosition = FormStartPosition.Manual;
        FormBorderStyle = FormBorderStyle.None;

        Size = new Size(openLocation.X, 300);

        Location = openLocation;
    }

    public override void ReDraw()
    {
        this.GenerateRoundRegion();
    }

    public override void LoadComponents()
    {
        BackColor = Color.Black;
        var urlInput = new LealTextBox()
        {
            Height = 50,
            Width = (int)(Width * 0.8),
            Placeholder = "m3u8 Url",
            BorderStyle = BorderStyle.FixedSingle,
            ForeColor = ColorPallete.TextFontColor,
            BackColor = ColorPallete.SecondaryBackgroundColor,
        };
        this.Add(urlInput);

        var title = new LealTextBox()
        {
            Height = 50,
            Width = (int)(Width * 0.5),
            Placeholder = "Title",
            BorderStyle = BorderStyle.FixedSingle,
            ForeColor = ColorPallete.TextFontColor,
            BackColor = ColorPallete.SecondaryBackgroundColor,
        };
        this.Add(title);

        var buttonAdd = new LealButton((s, e) => AddNewUrlData())
        {
            Height = 50,
            Width = 125,
            Text = "Add",
            ForeColor = ColorPallete.HighLightColor
        };
        this.Add(buttonAdd);

        var buttonCancel = new LealButton((s, e) => Close())
        {
            Height = 50,
            Width = 125,
            Text = "Cancel",
            ForeColor = ColorPallete.HighLightColor,
        };
        this.Add(buttonCancel);

        this.WaterFallChildControlsOfTypeByY<LealTextBox>(LealConstants.GAP * 2, LealConstants.GAP);
        urlInput.HorizontalCentralize();
        title.SetX(urlInput.Location.X);

        buttonAdd.DockBottomWithPadding(LealConstants.GAP * 2);
        buttonAdd.HorizontalCentralize();
        buttonAdd.AddX(-buttonAdd.Width + LealConstants.GAP / 2);

        buttonCancel.DockBottomWithPadding(LealConstants.GAP * 2);
        buttonCancel.HorizontalCentralize();
        buttonCancel.AddX(buttonCancel.Width - LealConstants.GAP / 2);
    }

    private void AddNewUrlData()
    {

    }
}
