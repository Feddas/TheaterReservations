using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// A movie with multiple dates that it will be shown in a theater.
/// Used to structure a scriptable object that makes it easy to create and track what show times are available for each movie.
/// </summary>
[System.Serializable]
[CreateAssetMenu(fileName = "SingleTheaterShowing", menuName = "TheaterReservations/SingleTheaterShowing")]
public class TheaterMovie : ScriptableObject
{
    [Tooltip("Name of the movie")]
    public string MovieName = "Unnamed movie";

    [Tooltip("This color is a fill in for having an image for theater cover art")]
    public Color CoverArt = Color.white;

    [Tooltip("Times the movie will be shown")]
    public List<UDateTime> ShownAtTimes = new List<UDateTime>(new UDateTime[] { System.DateTime.Now }); // initalize with at least one showing

    /// <summary> Builds ID used to cross-reference with reservations </summary>
    public int GetHashFor(System.DateTime atDate)
    {
        if (false == ShownAtTimes.Any(s => s.dateTime == atDate))
        {
            throw new System.Exception(this.name + " does not contain a showing at " + atDate.ToString());
        }

        return TheaterShowing.GetHashCode(MovieName, atDate); ;
    }
}
