using System.IO;

namespace M3U8SharpDownloader.Converter;

internal sealed class MovieSeriesData
{
    internal MovieSeriesData(string title, int season, int episode)
    {
        Title = TreatTitle(title);
        Season = season;
        Episode = episode;
    }

    internal MovieSeriesData(string title, string year)
    {
        Title = TreatTitle(title);
        Year = year;
    }

    internal bool IsMovie => Season == null && Episode == null;
    internal string Title { get; private set; }
    internal string? Year { get; private set; }
    internal int? Season { get; private set; }
    internal int? Episode { get; private set; }

    private static string TreatTitle(string title)
    {
        var invalidFileNameChars = Path.GetInvalidFileNameChars();
        return string.Concat(title.Split(invalidFileNameChars));
    }

    override public string ToString() => IsMovie ? $"{Title} ({Year})" : $"{Title} S{Season:D2}E{Episode:D2}";
}