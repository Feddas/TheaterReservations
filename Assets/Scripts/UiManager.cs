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
///  https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo
///  https://github.com/Unity-Technologies/UIElementsUniteCPH2019RuntimeDemo/blob/master/Assets/Tanks/Scripts/Managers/GameManager.cs#L279
///  https://forum.unity.com/forums/ui-toolkit.178
/// videos:
///  3 years old (2018.3): https://youtu.be/sVEmJ5-dr5E
///  2 years old (Unite Copenhagen): https://youtu.be/t4tfgI1XvGs
///  1.5 years old (DapperDino): https://youtu.be/6zR3uvLVzc4
///  3 months old (KookaNova): https://youtu.be/EVdtUPnl3Do
/// </summary>
public class UiManager : MonoBehaviour
{
    public TheaterAllShowings AllMovieShowings;

    /// <summary> Panel for name selection </summary>
    private VisualElement chooseName;
    private TextField nameParty;
    private Button nameContinue;

    /// <summary> Panel for day selection </summary>
    private VisualElement chooseDay;
    private DropdownField day;

    private VisualElement chooseShowing;
    private VisualElement chooseSeats;

    private void OnEnable()
    {
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

    private void initChooseName(VisualElement root)
    {
        chooseName = root.Q<VisualElement>("ChooseName");
        nameParty = chooseName.Q<TextField>("PartyName");
        nameContinue = chooseName.Q<Button>("Continue");

        nameContinue.RegisterCallback<ClickEvent>(ev => saveName());
    }

    private void initChooseDay(VisualElement root)
    {
        chooseDay = root.Q<VisualElement>("ChooseDay");
        day = chooseDay.Q<DropdownField>("Day");

        // all possible days movies can be shown on
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

    private void initChooseShowing(VisualElement root)
    {
        chooseShowing = root.Q<VisualElement>("ChooseShowing");
    }

    private void initChooseSeats(VisualElement root)
    {
        chooseSeats = root.Q<VisualElement>("ChooseSeats");
    }

    private void saveDay()
    {
        Debug.Log("Selected " + day.value);

        gotoScreen(chooseShowing);
    }

    private void saveName()
    {
        Debug.Log("Name was " + this.nameParty.text);
        string name = this.nameParty.text;

        gotoScreen(chooseDay);
    }

    private void gotoScreen(VisualElement fullScreenPanel)
    {
        chooseName.style.display = DisplayStyle.None;
        chooseDay.style.display = DisplayStyle.None;
        chooseShowing.style.display = DisplayStyle.None;
        chooseSeats.style.display = DisplayStyle.None;

        fullScreenPanel.style.display = DisplayStyle.Flex;
    }

    //public UiManager()
    //{
    //    this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    //}

    //void OnGeometryChange(GeometryChangedEvent geoEvent)
    //{
    //    nameScreen = this.Q("NameScreen");
    //    dayScreen = this.Q("DayScreen");
    //    timeScreen = this.Q("TimeScreen");
    //    seatsScreen = this.Q("SeatsScreen");

    //    nameScreen?.Q("Submit")?.RegisterCallback<ClickEvent>(ev => GotoDayScreen());

    //    seatsScreen?.Q("Finished")?.RegisterCallback<ClickEvent>(ev => GotoNameScreen());

    //    this.UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
    //}

    void GotoNameScreen()
    {
        chooseName.style.display = DisplayStyle.Flex;
        chooseDay.style.display = DisplayStyle.None;
        chooseShowing.style.display = DisplayStyle.None;
        chooseSeats.style.display = DisplayStyle.None;
    }

    void GotoDayScreen()
    {
        chooseName.style.display = DisplayStyle.None;
        chooseDay.style.display = DisplayStyle.Flex;
        chooseShowing.style.display = DisplayStyle.None;
        chooseSeats.style.display = DisplayStyle.None;
    }
}
