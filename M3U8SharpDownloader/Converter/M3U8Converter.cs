using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Events;

namespace M3U8SharpDownloader.Converter;

internal sealed class M3U8Converter
{
    internal delegate void FileStarted(DownloadData urlData);
    internal event FileStarted? OnFileStarted;

    internal delegate void FileProgress(DownloadData urlData, ConversionProgressEventArgs eventArgs, TimeSpan timeSpent);
    internal event FileProgress? OnFileProgress;

    internal delegate void FileCompleted(DownloadData urlData, string finalPath, TimeSpan timeSpent);
    internal event FileCompleted? OnFileCompleted;

    internal delegate void FileCancelled(DownloadData urlData, CancellationToken cancellationToken);
    internal event FileCancelled? OnFileCancelled;

    internal delegate void FileError(DownloadData urlData, Exception exception);
    internal FileError? OnErrorHappened;

    internal M3U8Converter() { }

    internal async Task ConvertAsync(List<DownloadData> urls, string outputFolder, ExtensionType outputFormat, CancellationToken cancellationToken, bool useGpu = true, int maxThreads = 4)
    {
        using var semaphore  = new SemaphoreSlim(maxThreads);

        var processTasks = urls.Select(async urlData =>
        {
            await semaphore.WaitAsync();

            try
            {
                await ConvertSingleAsync(urlData, outputFolder, outputFormat, cancellationToken, useGpu);
            }
            finally
            {
                semaphore.Release();
            }

        }).ToList();

        await Task.WhenAll(processTasks);
    }

    private async Task ConvertSingleAsync(DownloadData urlData, string outputFolder, ExtensionType outputFormat, CancellationToken cancellationToken, bool useGpu = true)
    {
        var finalPath = Path.Combine(outputFolder, $"{urlData.Data.Title}{GetText(outputFormat)}");
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{urlData.Url}\"", ParameterPosition.PreInput)
            .AddParameter(useGpu ? "-c:v h264_nvenc" : "-c:v libx264");
        conversion.SetPriority(ProcessPriorityClass.AboveNormal);

        switch (outputFormat)
        {
            case ExtensionType.MP3:
                conversion.AddParameter("-vn"); // Disable video
                conversion.SetOutput(finalPath);
                break;
            default:
                conversion.SetOutput(finalPath);
                break;
        }

        try
        {
            var stopWatch = Stopwatch.StartNew();
            stopWatch.Start();

            OnFileStarted?.Invoke(urlData); 

            conversion.OnProgress += (sender, eventArgs) =>
            {
                OnFileProgress?.Invoke(urlData, eventArgs, stopWatch.Elapsed);
            };

            await conversion.Start(cancellationToken);

            stopWatch.Stop();
            OnFileCompleted?.Invoke(urlData, finalPath, stopWatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            OnFileCancelled?.Invoke(urlData, cancellationToken);
        }
        catch (Exception ex)
        {
            OnErrorHappened?.Invoke(urlData, ex);
        }
    }

    private static string GetText(ExtensionType extensionType) => extensionType switch
    {
        ExtensionType.MP3 => ".mp3",
        ExtensionType.MP4 => ".mp4",
        _ => throw new InvalidCastException($"Invalid ExtensionType {extensionType}"),
    };
}