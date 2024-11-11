namespace M3U8SharpDownloader.Converter;

internal class DownloadData
{
    internal DownloadData(string url, MovieSeriesData data)
    {
        Url = url;
        Data = data;
    }

    internal string Url { get; }
    internal MovieSeriesData Data { get; }

    public override string ToString() => Data.ToString();
}