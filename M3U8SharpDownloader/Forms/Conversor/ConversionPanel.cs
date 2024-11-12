using LForms.Controls.Panels;
using LForms.Extensions;
using M3U8SharpDownloader.Converter;
using System;
using System.Drawing;
using System.Windows.Forms;
using Xabe.FFmpeg.Events;

namespace M3U8SharpDownloader.Forms.Conversor;

internal sealed class ConversionPanel : LealPanel
{
    private Label? _labelTitle;
    private ProgressBar? _progressBar;

    private bool _downloading;
    private ConversionProgressEventArgs? _progress;
    private Label? _labelProcessingTime;

    internal ConversionPanel(DownloadData urlData)
    {
        UrlData = urlData;
        BackColor = ColorPallete.SecondaryBackgroundColor;
    }

    internal DownloadData UrlData { get; }

    internal bool IsDownloading
    {
        get => _downloading;
        set
        {
            _downloading = value;
            if (value)
            {
                BackColor = Color.Red;
                Progress = new ConversionProgressEventArgs(TimeSpan.Zero, TimeSpan.Zero, 0);
            }
            else
            {
                BackColor = ColorPallete.SecondaryBackgroundColor;
                Progress = null;
            }
        }
    }

    internal ConversionProgressEventArgs? Progress
    {
        get => _progress;
        set
        {
            _progress = value;

            if (value == null || value.Percent < 0)
                return;

            if (_progressBar!.InvokeRequired)
            {
                _progressBar.Invoke(new Action(() =>
                {
                    _progressBar.Value = value.Percent;
                    _labelProcessingTime!.Text = $"Processing Time: {value.Duration:hh\\mm\\ss}";
                    BackColor = Color.Red.BlendColors(Color.Blue, value.Percent / 100.0f);
                    Invalidate();
                }));
            }
            else
            {
                _progressBar.Value = value.Percent;
                _labelProcessingTime!.Text = $"Processing Time: {value.Duration:hh\\mm\\ss}";
                BackColor = Color.Red.BlendColors(Color.Blue, value.Percent / 100.0f);
                Invalidate();
            }
        }
    }

    protected override void LoadComponents()
    {
        _progressBar = new ProgressBar()
        {
            Value = 0,
            Height = 5,
            Minimum = 0,
            Maximum = 100,
            Dock = DockStyle.Bottom,
        };

        _labelTitle = new Label()
        {
            Height = 40,
            AutoSize = false,
            Text = $"{(UrlData.Data.IsMovie ? "Movie" : "Serie")}: {UrlData.Data.Title[..Math.Min(50, UrlData.Data.Title.Length)]}",
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Font = new Font("Rubik", 16, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _labelProcessingTime = new Label() 
        {
            AutoSize = false,
            Text = "Not started",
            ForeColor = Color.White,
            Dock = DockStyle.Bottom,
            Font = new Font("Rubik", 12, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        this.Add(_labelProcessingTime);
        this.Add(_progressBar);
        this.Add(_labelTitle);
    }
}