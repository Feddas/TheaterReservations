using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

/// <summary>
/// UI Toolkit reference
/// websites:
///  https://docs.unity3d.com/Packages/com.unity.ui@1.0/manual/index.html
///  https://docs.unity3d.com/Packages/com.unity.ui@1.0/api/UnityEngine.UIElements.html
///  https://docs.unity3d.com/2020.1/Documentation/Manual/UITK-package.html
///  https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo
///  https://github.com/Unity-Technologies/UIElementsUniteCPH2019RuntimeDemo/blob/master/Assets/Tanks/Scripts/Managers/GameManager.cs#L279
///  https://forum.unity.com/forums/ui-toolkit.178
///  https://www.raywenderlich.com/6452218-uielements-tutorial-for-unity-getting-started
/// videos:
///  3 years old (2018.3): https://youtu.be/sVEmJ5-dr5E
///  2 years old (Unite Copenhagen): https://youtu.be/t4tfgI1XvGs
///  1.5 years old (DapperDino): https://youtu.be/6zR3uvLVzc4
///  3 months old (KookaNova): https://youtu.be/EVdtUPnl3Do
/// </summary>
public class UiManager : MonoBehaviour
{
    public TheaterAllShowings AllMovieShowings;
    public VisualTreeAsset MovieItem;

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
        //Debug.Log("Name was " + this.nameParty.text);
        string name = this.nameParty.text;

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

    private void saveDay()
    {
        Debug.Log("Selected " + day.value);
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

    private bool containsDay(TheaterMovie movie, string dayInDformat)
    {
        var result = movie.ShownAtTimes.Any(d => d.dateTime.ToString("D") == dayInDformat);
        return result;
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
        TheaterShowing showing = selection.First() as TheaterShowing;
        Debug.Log(showing.MovieName + " selected " + showing.ShownAtTime.ToString("t"));
    }
    #endregion [ Showing ]

    #region [ Seats ]
    private void initChooseSeats(VisualElement root)
    {
        chooseSeats = root.Q<VisualElement>("ChooseSeats");
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
