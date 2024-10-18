namespace M3U8SharpDownloader.Converter;

public readonly struct UrlData(string url, string fileName, bool isSeries)
{
    public string Url { get; } = url;
    public string FileName { get; } = fileName;
    public bool IsSeries { get; } = isSeries;
}