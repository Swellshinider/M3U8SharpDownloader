namespace M3U8SharpDownloader.Converter;

public record UrlData
{
    public UrlData(string url, string fileName)
    {
        Url = url;
        FileName = fileName;
    }

    public string Url { get; }
    public string FileName { get; }
}