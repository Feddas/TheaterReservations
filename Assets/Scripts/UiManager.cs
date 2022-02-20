using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI Toolkit reference
/// websites:
///  https://docs.unity3d.com/Packages/com.unity.ui@1.0/manual/index.html
///  https://github.com/Unity-Technologies/UIToolkitUnityRoyaleRuntimeDemo
///  https://forum.unity.com/forums/ui-toolkit.178
/// videos:
///  2 years old (Unite Copenhagen): https://youtu.be/t4tfgI1XvGs
///  1.5 years old (DapperDino): https://youtu.be/6zR3uvLVzc4
///  3 months old (KookaNova): https://youtu.be/EVdtUPnl3Do
/// </summary>
public class UiManager : MonoBehaviour
{
    /// <summary> Panel for name selection </summary>
    private VisualElement chooseName;
    private TextField nameParty;
    private Button nameContinue;

    /// <summary> Panel for day selection </summary>
    private VisualElement chooseDay;
    private DropdownField month;
    private DropdownField day;
    private DropdownField year;
    private Button dayContinue;

    private VisualElement chooseShowing;
    private VisualElement chooseSeats;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        initExit(root);
        initChooseName(root);
        initChooseDay(root);
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
        month = chooseDay.Q<DropdownField>("Month");
        day = chooseDay.Q<DropdownField>("Day");
        year = chooseDay.Q<DropdownField>("Year");

        nameContinue.RegisterCallback<ClickEvent>(ev => saveName());
    }

    private void saveName()
    {
        Debug.Log("Name was " + this.nameParty.text);
        string name = this.nameParty.text;

        chooseName.style.display = DisplayStyle.None;
        chooseDay.style.display = DisplayStyle.Flex;
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
