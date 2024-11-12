using LForms;
using LForms.Controls.Buttons;
using LForms.Controls.Forms;
using LForms.Controls.Panels;
using LForms.Controls.TextBoxes;
using LForms.Enums;
using LForms.Extensions;
using M3U8SharpDownloader.Converter;
using M3U8SharpDownloader.Forms.Conversor;
using M3U8SharpDownloader.Forms.Modals;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Xabe.FFmpeg.Events;

namespace M3U8SharpDownloader.Forms.Tabs;

internal sealed class HomeTab : BaseTab
{
    private LealTextBox? _searchBox;
    private LealButton? _buttonDownload;
    private LealButton? _buttonAddUrl;
    private LealPanel? _urlsListPanel;
    private CancellationTokenSource? _cts;

    private readonly M3U8Converter _converter = new();

    protected override void ReDraw()
    {
        if (_searchBox == null)
            return;
        _searchBox.GenerateRoundRegion();
    }

    protected override void LoadComponents()
    {
        _converter.OnFileStarted += Converter_Started;
        _converter.OnFileProgress += Conversion_Progress;
        _converter.OnFileCompleted += Converter_Completed;
        _converter.OnFileCancelled += Converter_Canceled;
        _converter.OnErrorHappened += Converter_Error;

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

        _buttonDownload = new LealButton(StartDownload)
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

        _urlsListPanel = new LealPanel()
        {
            AutoScroll = true,
            BackColor = Color.WhiteSmoke,
        };
        this.Add(_urlsListPanel);
        _urlsListPanel.DockFillWithPadding(LealConstants.GAP, LealConstants.GAP, 0, _searchBox.Location.Y + _searchBox.Height + LealConstants.GAP);
    }

    private void Converter_Started(DownloadData urlData)
    {
        var conversionPanels = _urlsListPanel!.GetChildrenOfType<ConversionPanel>();
        var conversionPanel = conversionPanels.FirstOrDefault(c => c.UrlData.Url == urlData.Url);

        if (conversionPanel == null)
            return;

        conversionPanel.IsDownloading = true;
    }

    private void Conversion_Progress(DownloadData urlData, ConversionProgressEventArgs eventArgs, long timeSpent)
    {
        var conversionPanels = _urlsListPanel!.GetChildrenOfType<ConversionPanel>();
        var conversionPanel = conversionPanels.FirstOrDefault(c => c.UrlData.Url == urlData.Url);

        if (conversionPanel == null) 
            return;

        conversionPanel.Progress = eventArgs;
    }

    private void Converter_Completed(DownloadData urlData, string finalPath, long timeSpan)
    {
        var conversionPanels = _urlsListPanel!.GetChildrenOfType<ConversionPanel>();
        var conversionPanel = conversionPanels.FirstOrDefault(c => c.UrlData.Url == urlData.Url);

        if (conversionPanel == null)
            return;

        conversionPanel.IsDownloading = false;
    }

    private void Converter_Canceled(DownloadData urlData, CancellationToken cancellationToken)
    {

    }

    private void Converter_Error(DownloadData urlData, Exception exception)
    {

    }

    private async void StartDownload(object? sender, EventArgs e)
    {
        var conversionPanels = _urlsListPanel!.GetChildrenOfType<ConversionPanel>();
        var urlsData = new List<DownloadData>();
        _cts = new CancellationTokenSource();
        foreach (var conversionPanel in conversionPanels)
        {
            if (conversionPanel.IsDownloading)
                continue;

            conversionPanel.IsDownloading = true;
            urlsData.Add(conversionPanel.UrlData);
        }

        await _converter.ConvertAsync(urlsData, "C:\\Users\\dute2\\Downloads", ExtensionType.MP4, _cts.Token);
    }

    private void OpenModal(object? sender, EventArgs e)
    {
        if (sender is not LealButton lb)
            return;

        var xPoint = Width - (Width - _searchBox!.Location.X);
        var yPoint = lb.Location.Y + lb.Height;
        var pointScreen = PointToScreen(new Point(xPoint, yPoint));
        var modalUrl = new AddUrlModal(pointScreen);
        modalUrl.UrlDataAdd += ModalUrl_UrlDataAdd;
        modalUrl.ShowDialog();
    }

    private void ModalUrl_UrlDataAdd(object? sender, DownloadData urlData)
    {
        if (_urlsListPanel!.GetChildrenOfType<ConversionPanel>().ToList().Exists(c => c.UrlData == urlData))
        {
            var dialog = MessageBox.Show(
                "Url already added, are you sure about adding it again?",
                "Url already Exists", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Information);

            if (dialog != DialogResult.Yes)
                return;
        }

        _urlsListPanel!.Add(new ConversionPanel(urlData));
        _urlsListPanel!.GetChildrenOfType<ConversionPanel>().ToList().ForEach(c =>
        {
            c.Height = 150;
            c.Width = _urlsListPanel!.Width;
            c.Centralize();
        });

        _urlsListPanel!.WaterFallChildControlsOfTypeByY<ConversionPanel>(0, LealConstants.GAP / 2);
    }
}