using LForms;
using LForms.Controls.Buttons;
using LForms.Controls.TextBoxes;
using LForms.Extensions;
using M3U8SharpDownloader.Converter;
using M3U8SharpDownloader.Forms.Modals;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace M3U8SharpDownloader.Forms.Tabs;

internal sealed class HomeTab : BaseTab
{
    private LealTextBox? _searchBox;
    private LealButton? _buttonDownload;
    private LealButton? _buttonAddUrl;

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

        _searchBox.DockTopLeftWithPadding(LealConstants.GAP, LealConstants.GAP);

        _buttonDownload = new LealButton()
        {
            Height = 50,
            Text = "Start Download",
            ForeColor = Color.WhiteSmoke
        };
        this.Add(_buttonDownload);

        _buttonDownload.DockTopRightWithPadding(LealConstants.GAP, LealConstants.GAP);

        _buttonAddUrl = new LealButton(OpenModal)
        {
            Text = "+",
            Width = 50,
            Height = 50,
            ForeColor = Color.WhiteSmoke
        };
        this.Add(_buttonAddUrl);
        _buttonAddUrl.DockTopRightWithPadding(LealConstants.GAP, _buttonDownload.Width + (LealConstants.GAP * 2));
    }

    private void OpenModal(object? sender, EventArgs e)
    {
        if (sender is not LealButton lb)
            return;

        var xPoint = Width - (Width - _searchBox!.Location.X);
        var yPoint = lb.Location.Y + lb.Height;

        var modalUrl = new AddUrlModal(PointToScreen(new Point(xPoint, yPoint)));
        modalUrl.UrlDataAdd += ModalUrl_UrlDataAdd;
        modalUrl.ShowDialog();

    }

    private void ModalUrl_UrlDataAdd(object? sender, UrlData e)
    {

    }
}