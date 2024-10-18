using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Events;

namespace M3U8SharpDownloader.Converter;

internal sealed class M3U8Converter
{
    internal delegate void FileStarted(UrlData urlData);
    internal event FileStarted? OnFileStarted;

    internal delegate void FileProgress(UrlData urlData, ConversionProgressEventArgs eventArgs);
    internal event FileProgress? OnFileProgress;

    internal delegate void FileCompleted(UrlData urlData, string finalPath, long timeSpan);
    internal event FileCompleted? OnFileCompleted;

    internal delegate void FileError(Exception exception);
    internal FileError? OnErrorHappened;

    internal M3U8Converter() { }

    internal async Task ConvertAsync(List<UrlData> urls, string outputFolder, ExtensionType outputFormat, CancellationToken cancellationToken, bool useGpu = true, int maxThreads = 4)
    {
        using var semaphore  = new SemaphoreSlim(maxThreads);

        var processTasks = urls.Select(async urlData =>
        {
            await semaphore.WaitAsync();

            try
            {
                await ConvertSingleAsync(urlData, outputFolder, outputFormat, cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }

        }).ToList();

        await Task.WhenAll(processTasks);
    }

    private async Task ConvertSingleAsync(UrlData urlData, string outputFolder, ExtensionType outputFormat, CancellationToken cancellationToken, bool useGpu = true)
    {
        var finalPath = Path.Combine(outputFolder, $"{urlData.FileName}{GetText(outputFormat)}");
        var conversion = FFmpeg.Conversions.New()
            .AddParameter($"-i \"{urlData.Url}\"", ParameterPosition.PreInput)
            .AddParameter(useGpu ? "-c:v h264_nvenc" : "-c:v libx264");

        switch (outputFormat)
        {
            case ExtensionType.MP3:
                conversion.AddParameter("-vn"); // Disable video
                conversion.SetOutput(finalPath);
                break;
            case ExtensionType.MP4:
            default:
                conversion.SetOutput(finalPath);
                break;
        }

        try
        {
            var startSpan = DateTime.Now.Ticks;
            OnFileStarted?.Invoke(urlData); 

            conversion.OnProgress += (sender, eventArgs) =>
            {
                OnFileProgress?.Invoke(urlData, eventArgs);
            };

            await conversion.Start(cancellationToken);

            OnFileCompleted?.Invoke(urlData, finalPath, DateTime.Now.Ticks - startSpan);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            OnErrorHappened?.Invoke(ex);
        }
    }

    private static string GetText(ExtensionType extensionType) => extensionType switch
    {
        ExtensionType.MP3 => ".mp3",
        ExtensionType.MP4 => ".mp4",
        _ => throw new InvalidCastException($"Invalid ExtensionType {extensionType}"),
    };
}