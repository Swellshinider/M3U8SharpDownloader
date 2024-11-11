namespace M3U8SharpDownloader.Converter;

internal sealed class MovieSeriesData
{
    internal MovieSeriesData(string tile, int season, int episode)
    {
        Title = tile;
        Season = season;
        Episode = episode;
    }

    internal MovieSeriesData(string title, string year)
    {
        Title = title;
        Year = year;
    }

    internal bool IsMovie => Season == null && Episode == null;
    internal string Title { get; private set; }
    internal string? Year { get; private set; }
    internal int? Season { get; private set; }
    internal int? Episode { get; private set; }

    override public string ToString() => IsMovie ? $"{Title} ({Year})" : $"{Title} S{Season:D2}E{Episode:D2}";
}