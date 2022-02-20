using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
