using LForms.Controls.Panels;
using LForms.Extensions;
using M3U8SharpDownloader.Converter;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Xabe.FFmpeg.Events;

namespace M3U8SharpDownloader.Forms.Conversor;

internal sealed class ConversionPanel : LealPanel
{
    private Label? _labelProcessingTime;

    private ProgressBar? _progressBar;

    private readonly Color _defaultColor;
    private readonly Color _beginningColor;
    private readonly Color _finishedColor;

    internal ConversionPanel(DownloadData urlData, Color defaultColor, Color beginningColor, Color finishedColor)
    {
        UrlData = urlData;
        _defaultColor = defaultColor;
        _beginningColor = beginningColor;
        _finishedColor = finishedColor;
        BackColor = defaultColor;
    }

    internal DownloadData UrlData { get; }

    internal bool InProgress { get; set; } = false;

    private Color ProgressColor(int percent) => _beginningColor.BlendColors(_finishedColor, percent / 100f);

    internal void UpdateProgress(ConversionProgressEventArgs progress)
    {
        Trace.Assert(_progressBar != null);
        Trace.Assert(_labelProcessingTime != null);

        InProgress = true;
        BackColor = ProgressColor(progress.Percent);

        if (_progressBar.InvokeRequired)
        {
            _progressBar.Invoke(() =>
            {
                _progressBar.Value = CapPercentage(progress.Percent);
                _labelProcessingTime.Text =  $"Processing {FormatSpan(progress.Duration)}/{FormatSpan(progress.TotalLength)} ({progress.Percent}%)";
            });
        }
        else
        {
            _progressBar.Value = CapPercentage(progress.Percent);
            _labelProcessingTime.Text = $"Processing {FormatSpan(progress.Duration)}/{FormatSpan(progress.TotalLength)} ({progress.Percent}%)";
        }
    }

    internal void Finish(TimeSpan timeSpan)
    {
        Trace.Assert(_progressBar != null);
        Trace.Assert(_labelProcessingTime != null);

        InProgress = false;
        BackColor = _finishedColor;

        if (_progressBar.InvokeRequired)
        {
            _progressBar.Invoke(() =>
            {
                _progressBar.Value = 100;
                _labelProcessingTime.Text = $"Finished in {FormatSpan(timeSpan)}";
            });
        }
        else
        {
            _progressBar.Value = 100;
            _labelProcessingTime.Text = $"Finished in {FormatSpan(timeSpan)}";
        }
    }

    internal void Cancel()
    {
        Trace.Assert(_labelProcessingTime != null);

        InProgress = false;
        BackColor = _finishedColor;

        if (_labelProcessingTime.InvokeRequired)
        {
            _labelProcessingTime.Invoke(() =>
            {
                _labelProcessingTime.Text = $"Cancelled";
            });
        }
        else
        {
            _labelProcessingTime.Text = $"Cancelled";
        }
    }

    internal void SetError(string message)
    {
        Trace.Assert(_labelProcessingTime != null);

        InProgress = false;
        BackColor = _finishedColor;

        if (_labelProcessingTime.InvokeRequired)
        {
            _labelProcessingTime.Invoke(() =>
            {
                _labelProcessingTime.Text = $"Error: {message}";
            });
        }
        else
        {
            _labelProcessingTime.Text = $"Error: {message}";
        }
    }

    protected override void LoadComponents()
    {
        var labelTitle = new Label()
        {
            Height = 40,
            AutoSize = false,
            Dock = DockStyle.Top,
            ForeColor = Color.White,
            Text = UrlData.ToString(),
            Font = new Font("Rubik", 16, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
        };

        _progressBar = new ProgressBar()
        {
            Value = 0,
            Height = 10,
            Minimum = 0,
            Maximum = 100,
            Dock = DockStyle.Bottom,
            BackColor = _defaultColor,
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
        this.Add(labelTitle);
    }

    private static int CapPercentage(int percentage) => Math.Min(100, Math.Max(0, percentage));

    private static string FormatSpan(TimeSpan span)
        => span.Hours > 0 ? $"{span.Hours:D2}h{span.Minutes:D2}m{span.Seconds:D2}s" : $"{span.Minutes:D2}m{span.Seconds:D2}s";
}