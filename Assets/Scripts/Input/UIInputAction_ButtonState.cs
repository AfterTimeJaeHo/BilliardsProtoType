using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class UIInputAction
{
    public static UIInputAction Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIInputAction();
                Init();
            }

            return _instance;
        }
    }

    private static UIInputAction _instance;
    private static Dictionary<InputActionType, InputAction> _actions;

    private static void Init()
    {
        _instance.Enable();
        _actions = new Dictionary<InputActionType, InputAction>();
        ReadOnlyArray<InputActionMap> actionMaps = _instance.asset.actionMaps;

        foreach (InputActionMap actionMap in actionMaps)
        {
            if (actionMap.name != "Global")
                actionMap.Disable();
        }

        List<InputAction> inputActions = actionMaps.SelectMany(map => map.actions).ToList();
        foreach (InputAction inputAction in inputActions)
        {
            string actionName = inputAction.name;
            InputActionType inputActionType = Enum.Parse<InputActionType>(actionName);
            _actions.Add(inputActionType, inputAction);
        }
    }

    public bool GetButtonDown(InputActionType inputActionType)
    {
        bool isButtonDown = _actions[inputActionType].WasPerformedThisFrame();

        return isButtonDown;
    }

    public bool GetButton(InputActionType inputActionType)
    {
        bool isButtonStay = _actions[inputActionType].IsInProgress();

        return isButtonStay;
    }

    public bool GetButtonUp(InputActionType inputActionType)
    {
        bool isButtonUp = _actions[inputActionType].WasReleasedThisFrame();

        return isButtonUp;
    }

    public bool IsAnyMousePressed()
    {
        bool isMousePressed = Mouse.current != null && (Mouse.current.leftButton.isPressed ||
                                                        Mouse.current.leftButton.wasPressedThisFrame ||
                                                        Mouse.current.leftButton.wasReleasedThisFrame);

        return isMousePressed;
    }
}

public enum InputActionType
{
    ProgressStory,
    MoveMenuLeft,
    MoveMenuRight,
    AnyKey,
    StopSkip,
    ChangeCursorIcon,
    PopPopup,
    HideUI,
    ShowSave,
    ShowLoad,
    SetActiveSound,
    SkipStory,
    ShowSetting,
    ShowTimeline,
    ShowControlGuide,
    ShowBackLog,
    ProgressMessage,
    SkipTitleEnter,
    ChatProgressButton,
    ChatProgressMouse,
    PopUI,
    QuickSave,
    QuickLoad,
    Auto,
    Navigate,
    Submit,
    Cancel,
    Point,
    Click,
    ScrollWheel,
    MiddleClick,
    RightClick,
    TrackedDevicePosition,
    TrackedDeviceOrientation,
    MoveLeft,
    MoveRight,
    DeleteSave,
    OnOffSideMenu,
    Mute,
    ResetSound,
    LeftClick
}