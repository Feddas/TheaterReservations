using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTestTheaterReservations
{
    /// <summary> integration test for google sheets API </summary>
    [UnityTest] // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use `yield return null;` to skip a frame.
    public IEnumerator CanReadReservationsIn5Seconds()
    {
        // Arrange
        var gameObject = new GameObject();
        var seats = gameObject.AddComponent<TheaterReservationsGoogleSheets>();

        // Act
        seats.LoadSheet();
        yield return new WaitForSecondsRealtime(5); // guesstimate time for google sheets API to respond

        // Assert
        Assert.IsNotNull(seats.Reservations);
    }
}
