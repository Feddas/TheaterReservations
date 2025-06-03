using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Reservations are stored on GoogleSheets.
/// This uses https://assetstore.unity.com/packages/tools/utilities/google-sheets-to-unity-73410
/// , but it needed so many modifications to use that I might as well had written my own library using https://youtu.be/qm-Ooj6XjvE
/// </summary>
public class TheaterReservationsGoogleSheets : MonoBehaviour
{
    private const string associatedSheet = "1J1ATTbOW7CzFHohqmJEy8ccKN-207iRGVS2vHwb78M8";
    private const string associatedWorksheet = "Reservations";

    [System.Serializable]
    public class Reservation
    {
        public int ShowingID;
        public string PartyName;
        public string Seats;

        public Reservation() { }

        public Reservation(List<string> row)
        {
            ShowingID = int.Parse(row[0]);
            PartyName = row[1];
            Seats = row[2];
        }
    }

    public static TheaterReservationsGoogleSheets Instance { get; set; }

    [Tooltip("Reservations syncing with a Google sheet")]
    public List<Reservation> Reservations = new List<Reservation>();

    private GoogleSheetsToUnity.GSTU_Search sheetAddress
    {
        get
        {
            if (_sheetAddress == null)
            {
                _sheetAddress = new GoogleSheetsToUnity.GSTU_Search(associatedSheet, associatedWorksheet);
            }
            return _sheetAddress;
        }
    }
    private GoogleSheetsToUnity.GSTU_Search _sheetAddress;

    private IEnumerator Start()
    {
        if (Instance != null)
        {
            throw new System.Exception($"More than one TheatherReservationsGoogleSheets exists. Remove it from {this.name} or {Instance.name}.");
        }
        Instance = this;

        yield return new WaitForSeconds(2); // Hack: if the user previously entered a reservation, give it a few seconds to save. Would be less hacky if LoadSheet waited for SaveSheet to finish, yet this scene is reloaded as soon as SaveSheet is called.
        LoadSheet();
    }

    /// <summary>
    /// Fills Reservations with the contents from google sheets
    /// </summary>
    [ContextMenu("LoadSheet")]
    public void LoadSheet()
    {
        GoogleSheetsToUnity.SpreadsheetManagerRaw.ReadRaw(sheetAddress, loadSheet);
    }

    /// <summary>
    /// Overwrites the contents in google sheets with the value in Reservations
    /// </summary>
    [ContextMenu("SaveReservationsToSheet")]
    public void SaveSheet()
    {
        List<List<string>> rawData = new List<List<string>>();
        foreach (var reservation in Reservations)
        {
            rawData.Add(new List<string>() { reservation.ShowingID.ToString(), reservation.PartyName, reservation.Seats });
        }
        GoogleSheetsToUnity.SpreadsheetManager.Write(sheetAddress, new GoogleSheetsToUnity.ValueRange(rawData), null);
    }

    /// <summary>
    /// Adds or modifies seats then saves to Google Sheet
    /// </summary>
    public void SetSeats(int theaterId, string partyName, int[] seats)
    {
        Reservation existingReservation = Reservations.FirstOrDefault(r => r.ShowingID == theaterId && r.PartyName == partyName);
        string newSeats = string.Join('n', seats);

        if (existingReservation != null && existingReservation.Seats == newSeats)
        {
            Debug.Log($"{partyName} left seats as {newSeats} for movie ID {theaterId}");
            return; // reservation was not modified, skip saving
        }
        else if (existingReservation != null)
        {
            existingReservation.Seats = newSeats;
        }
        else
        {
            Reservations.Add(new Reservation()
            {
                ShowingID = theaterId,
                PartyName = partyName,
                Seats = newSeats
            });
        }

        Debug.Log($"{partyName}'s seats are now {newSeats} for movie ID {theaterId}");
        SaveSheet();
    }

    public int[] GetMySeats(int theaterId, string partyName)
    {
        return GetSeats(theaterId, p => p == partyName);
    }

    public int[] GetOthersSeats(int theaterId, string partyName)
    {
        return GetSeats(theaterId, p => p != partyName);
    }

    /// <summary>
    /// Get seats reserved for the specific PartyName check i.e. p => p != "Sams"
    /// </summary>
    private int[] GetSeats(int theatherId, System.Func<string, bool> partyCheck = null)
    {
        // skip party check
        if (partyCheck == null)
        {
            partyCheck = p => true;
        }

        return Reservations
            .Where(r => r.ShowingID == theatherId && partyCheck(r.PartyName))
            .SelectMany(r => seatsToInts(r.Seats))
            .ToArray();
    }

    private int[] seatsToInts(string seats)
    {
        return System.Array.ConvertAll(seats.Split('n'), s => int.Parse(s));
    }

    private void loadSheet(GoogleSheetsToUnity.ValueRange sheetData)
    {
        foreach (var row in sheetData.values)
        {
            if (row.Count != 3)
            {
                Debug.LogWarning("GoogleSheets row skipped due to being malformed. " + string.Join(',', row));
            }
            else
            {
                loadRow(row);
            }
        }
    }

    private void loadRow(List<string> row)
    {
        Reservations.Add(new Reservation(row));
    }
}
