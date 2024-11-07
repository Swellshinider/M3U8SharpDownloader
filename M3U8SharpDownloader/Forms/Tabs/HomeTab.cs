using LForms;
using LForms.Controls.Buttons;
using LForms.Controls.Panels;
using LForms.Controls.TextBoxes;
using LForms.Extensions;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms.Tabs;

internal sealed class HomeTab : BaseTab
{
    private LealTextBox? _searchBox;

    protected override void ReDraw()
    {
        if (_searchBox == null)
            return;
        _searchBox.GenerateRoundRegion();
    }

    protected override void LoadComponents()
    {
        _searchBox = new LealTextBox()
        {
            Width = 500,
            Height = 50,
            Placeholder = "Search",
            ForeColor = ColorPallete.TextFontColor,
            BackColor = ColorPallete.SecondaryBackgroundColor,
            BorderStyle = BorderStyle.FixedSingle,
        };
        this.Add(_searchBox);

        _searchBox.DockTopLeftWithPadding(0, LealConstants.GAP);

        var buttonAddUrl = new LealButton((s, e) => MessageBox.Show("aa"))
        {
            Text = "Butto Add",
        };

        this.Add(buttonAddUrl);

        buttonAddUrl.DockTopRightWithPadding(0, LealConstants.GAP);
    }
}