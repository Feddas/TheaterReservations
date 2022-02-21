using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTestTheaterReservations
{
    [Test]
    public void CanReadReservations()
    {
        // Arrange
        var gameObject = new GameObject();
        var seats = gameObject.AddComponent<TheaterReservationsGoogleSheets>();

        // Act
        seats.LoadSheet();

        // Assert
        Assert.IsNotNull(seats.Reservations);
    }
}
