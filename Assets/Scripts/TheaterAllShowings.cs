using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A collection of all available movie showtimes that are available at a theater.
/// </summary>
[CreateAssetMenu(fileName = "AllTheaterShowings", menuName = "TheaterReservations/AllTheaterShowings")]
public class TheaterAllShowings : ScriptableObject
{
    [Tooltip("Name of the theater where reservations will be made. Only one theater per backend is supported (TheaterName isn't used to build showing ID hashcodes).")]
    public string TheaterName = "Cinema 2";

    [Tooltip("Movies this theater is showing.")]
    public List<TheaterMovie> ActiveMovies = new List<TheaterMovie>();
}
