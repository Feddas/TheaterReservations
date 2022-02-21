using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Sheets = TheaterReservationsGoogleSheets;

/// <summary>
/// Manages how UI Toolkit interfaces with the rest of the project.
/// 
/// This is my first project using UI Toolkit. I often came back to the following references:
/// websites:
///  https://unity.com/features/ui-toolkit
///  https://docs.unity3d.com/Packages/com.unity.ui@1.0/manual/index.html
///  https://docs.unity3d.com/Packages/com.unity.ui@1.0/api/UnityEngine.UIElements.html
///  https://docs.unity3d.com/2020.1/Documentation/Manual/UITK-package.html
///  https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo
///  https://github.com/Unity-Technologies/UIElementsUniteCPH2019RuntimeDemo/blob/master/Assets/Tanks/Scripts/Managers/GameManager.cs#L279
///  https://forum.unity.com/forums/ui-toolkit.178
///  https://www.raywenderlich.com/6452218-uielements-tutorial-for-unity-getting-started
/// videos:
///  Jan2019 (2018.3): https://youtu.be/sVEmJ5-dr5E
///  Oct2019 (Unite Copenhagen): https://youtu.be/t4tfgI1XvGs
///  Oct2020 (DapperDino): https://youtu.be/6zR3uvLVzc4
///  Nov2021 (KookaNova): https://youtu.be/EVdtUPnl3Do
/// </summary>
public class UiManager : MonoBehaviour
{
    public TheaterAllShowings AllMovieShowings;
    public VisualTreeAsset MovieItem;

    // values stored before sending a reservation to the backend
    private string selectedName = "unset name";
    private TheaterShowing selectedShowing = new TheaterShowing("unset movie name", Color.cyan, System.DateTime.Now);

    /// <summary> Panel for name selection </summary>
    private VisualElement chooseName;
    private TextField nameParty;
    private Button nameContinue;

    /// <summary> Panel for day selection </summary>
    private VisualElement chooseDay;
    private DropdownField day;

    /// <summary> Panel for showing time selection </summary>
    private VisualElement chooseShowing;
    private ListView movieList;
    private List<TheaterShowing> filteredShowings;

    /// <summary> Panel for seat selection </summary>
    private VisualElement chooseSeats;
    private IEnumerable<VisualElement> allSeats;

    private void OnEnable()
    {
        // initialize
        var root = GetComponent<UIDocument>().rootVisualElement;
        initExit(root);
        initChooseName(root);
        initChooseDay(root);
        initChooseShowing(root);
        initChooseSeats(root);

        gotoScreen(chooseName);
    }

    private void initExit(VisualElement root)
    {
        root.Q<Button>("Exit")
            .RegisterCallback<ClickEvent>(ev => Application.Quit());
    }

    #region [ Name ]
    private void initChooseName(VisualElement root)
    {
        chooseName = root.Q<VisualElement>("ChooseName");
        nameParty = chooseName.Q<TextField>("PartyName");
        nameContinue = chooseName.Q<Button>("Continue");

        nameContinue.RegisterCallback<ClickEvent>(ev => nameChosen());
    }

    private void nameChosen()
    {
        selectedName = this.nameParty.text;

        gotoScreen(chooseDay);
    }
    #endregion [ Name ]

    #region [ Day ]
    private void initChooseDay(VisualElement root)
    {
        chooseDay = root.Q<VisualElement>("ChooseDay");
        day = chooseDay.Q<DropdownField>("Day");

        // fill Dropdown with list of all possible days movies can be shown on
        var availableDays = AllMovieShowings.ActiveMovies
            .SelectMany(m => m.ShownAtTimes)
            .Select(date => date.dateTime)
            .OrderBy(date => date)
            .Select(date => date.ToString("D"))
            .Distinct()
            .ToList();
        day.choices = availableDays;

        day.RegisterValueChangedCallback(ev => saveDay());
    }

    private bool containsDay(TheaterMovie movie, string dayInDformat)
    {
        var result = movie.ShownAtTimes.Any(d => d.dateTime.ToString("D") == dayInDformat);
        return result;
    }

    private void saveDay()
    {
        // Debug.Log("Selected " + day.value);
        filteredShowings = AllMovieShowings.ActiveMovies
            .Where(m => containsDay(m, day.value))
            .SelectMany(movie => movie.ShownAtTimes.Select(d => d.dateTime)
                .Where(d => d.ToString("D") == day.value)
                .Select(day => new TheaterShowing(movie, day)))
            .OrderBy(showing => showing.ShownAtTime)
            .ToList();
        movieList.itemsSource = filteredShowings;
        movieList.Rebuild();

        gotoScreen(chooseShowing);
    }
    #endregion [ Day ]

    #region [ Showing ]
    private void initChooseShowing(VisualElement root)
    {
        chooseShowing = root.Q<VisualElement>("ChooseShowing");
        movieList = chooseShowing.Q<ListView>("MovieList");

        // setup how ListView binds to items https://docs.unity3d.com/ScriptReference/UIElements.ListView.html
        movieList.fixedItemHeight = 140;
        movieList.makeItem = () => MovieItem.CloneTree();
        movieList.bindItem = BindMovies;
        movieList.itemsSource = new List<string>() { "dummy", "set" }; // note: couldn't get path binding to work https://forum.unity.com/threads/can-someone-explain-binding-to-me-in-the-context-of-ui-toolkit.855142/
        movieList.onSelectionChange += timeSelected;
    }

    private void BindMovies(VisualElement element, int index)
    {
        var coverArt = element.Q<VisualElement>("CoverArt");
        coverArt.style.backgroundColor = filteredShowings[index].CoverArt;
        element.Q<Label>("MovieName").text = filteredShowings[index].MovieName;
        element.Q<Label>("MovieDay").text = filteredShowings[index].ShownAtTime.ToString("D"); // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings
        element.Q<Label>("MovieTime").text = filteredShowings[index].ShownAtTime.ToString("t");
    }

    private void timeSelected(IEnumerable<object> selection)
    {
        if (selection == null || selection.Count() == 0) return; // forms were reset

        // save the theater showing time that was selected by the user
        selectedShowing = selection.First() as TheaterShowing;
        updateSeats();
        // Debug.Log(selectedShowing.MovieName + " selected " + selectedShowing.ShownAtTime.ToString("t"));

        gotoScreen(chooseSeats);
    }
    #endregion [ Showing ]

    #region [ Seats ]
    private void initChooseSeats(VisualElement root)
    {
        chooseSeats = root.Q<VisualElement>("ChooseSeats");

        // Register all seats
        allSeats = chooseSeats.Q<VisualElement>("AllSeats").Children();
        foreach (var seat in allSeats)
        {
            seat.RegisterCallback<ClickEvent>(ev => seatToggled(ev, seat));
        }

        // Finished with seats
        var seatsContinue = chooseSeats.Q<Button>("Continue");
        seatsContinue.RegisterCallback<ClickEvent>(ev => seatsChosen(allSeats));
    }

    /// <summary>
    /// Update the color tint of all seats for the selected theaterId
    /// </summary>
    private void updateSeats()
    {
        int theaterId = selectedShowing.GetHashCode();
        int[] previousReservation = Sheets.Instance.GetMySeats(theaterId, selectedName);
        int[] takenSeats = Sheets.Instance.GetOthersSeats(theaterId, selectedName);
        var seats = allSeats.ToArray();
        for (int i = 0; i < seats.Count(); i++)
        {
            seats[i].style.unityBackgroundImageTintColor = previousReservation.Contains(i) ? Color.green
                                                         : takenSeats.Contains(i) ? Color.red
                                                         : Color.white;
        }
    }

    private void seatToggled(ClickEvent ev, VisualElement seat)
    {
        var currentColor = seat.style.unityBackgroundImageTintColor;

        if (currentColor == Color.red)
        {
            return;
        }
        else if (currentColor == Color.green)
        {
            seat.style.unityBackgroundImageTintColor = Color.white;
        }
        else if (currentColor == Color.white)
        {
            seat.style.unityBackgroundImageTintColor = Color.green;
        }
    }

    private void seatsChosen(IEnumerable<VisualElement> seats)
    {
        var myReservation = seats.Select((Element, Index) => new { Element, Index })
            .Where(seat => seat.Element.style.unityBackgroundImageTintColor == Color.green)
            .Select(seat => seat.Index).ToArray();

        Sheets.Instance.SetSeats(selectedShowing.GetHashCode(), selectedName, myReservation);

        // return back to first screen
        //nameParty.value = selectedName = "";
        //day.index = -1;
        //movieList.selectedIndex = -1;
        //selectedShowing = null;
        //gotoScreen(chooseName);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    #endregion [ Seats ]

    private void gotoScreen(VisualElement fullScreenPanel)
    {
        chooseName.style.display = DisplayStyle.None;
        chooseDay.style.display = DisplayStyle.None;
        chooseShowing.style.display = DisplayStyle.None;
        chooseSeats.style.display = DisplayStyle.None;

        fullScreenPanel.style.display = DisplayStyle.Flex;
    }
}
