using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTestTheaterShowings
{
    [Test]
    public void MoviesCanGenerateHashCodes()
    {
        // Arrange
        var movie = ScriptableObject.CreateInstance<TheaterMovie>();
        var name = movie.MovieName;
        var time = movie.ShownAtTimes[0].dateTime;

        // Act
        var actualHash = movie.GetHashFor(time);

        // Assert
        Assert.AreNotEqual(actualHash, 0);
    }

    [Test]
    public void MoviesGenerateExpectedHashCodes()
    {
        // Arrange
        var movie = ScriptableObject.CreateInstance<TheaterMovie>();
        var name = movie.MovieName;
        var time = movie.ShownAtTimes[0].dateTime;

        // Act
        var actualHash = movie.GetHashFor(time);
        var expectedHash = name.GetHashCode() ^ time.GetHashCode();

        // Assert
        Assert.AreEqual(actualHash, expectedHash);
    }

    [Test]
    public void HashCodesAreUnique()
    {
        // Arrange
        var movie = ScriptableObject.CreateInstance<TheaterMovie>();
        var name = movie.MovieName;
        var time = movie.ShownAtTimes[0].dateTime;
        var time2 = System.DateTime.Now.AddDays(2);
        movie.ShownAtTimes.Add(time2);

        // Act
        var time1Hash = movie.GetHashFor(time);
        var time2Hash = movie.GetHashFor(time2);

        // Assert
        Assert.AreNotEqual(time1Hash, time2Hash);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator UnitTestBackendWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
