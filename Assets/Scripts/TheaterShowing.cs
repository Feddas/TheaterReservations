using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pairing of a movie with a single showing time.
/// Used to transfer showing data amonst object.
/// </summary>
public class TheaterShowing
{
    public string MovieName;
    public Color CoverArt;
    public System.DateTime ShownAtTime;

    public TheaterShowing(string movieName, Color coverArt, System.DateTime shownAtTime)
    {
        this.MovieName = movieName;
        this.CoverArt = coverArt;
        this.ShownAtTime = shownAtTime;
    }

    public TheaterShowing(TheaterMovie movie, System.DateTime shownAtTime)
        : this(movie.MovieName, movie.CoverArt, shownAtTime)
    {
    }

    public override int GetHashCode()
    {
        return GetHashCode(MovieName, ShownAtTime);
    }

    /// <summary>
    /// Builds ID used to reference a movie matched with a showtime
    /// </summary>
    public static int GetHashCode(string movieName, System.DateTime shownAtTime)
    {
        return movieName.GetHashCode() ^ shownAtTime.GetHashCode();
    }
}
