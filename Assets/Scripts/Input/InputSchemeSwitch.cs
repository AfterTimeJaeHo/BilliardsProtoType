using System.Collections.Generic;
using System.Linq;
using Aftertime.TeachingAssistantLife.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

namespace Aftertime.TeachingAssistantLife
{
    public enum InputControllerType
    {
        KeyboardMouse,
        DualSense,
        DefaultGamePad,
        DualShock,
        Xbox,
        Switch
    }

    public class InputSchemeSwitch : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;

        public static InputControllerType LastPressedController { get; private set; }
        private InputControllerType _curInputController;

        private void Start()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onAnyButtonPress.Call(OnAnyButtonPress);

            InputDevice inputDevice = null;
            if (InputSystem.devices.Count > 0)
                inputDevice = InputSystem.devices.Last();
            InputControllerType inputControllerType = GetCurrentInputControllerType(inputDevice);
            SwitchScheme(inputControllerType);
        }

        private void OnAnyButtonPress(InputControl inputControl)
        {
            if (Application.isPlaying == false)
                return;

            InputDevice device = inputControl.device;
            InputControllerType inputController = GetCurrentInputControllerType(device);
            if (_curInputController != inputController)
            {
                SwitchScheme(inputController);
            }

            LastPressedController = inputController;
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange changeState)
        {
            InputControllerType controllerType = GetCurrentInputControllerType(device);
            SwitchScheme(controllerType);
        }

        private InputControllerType GetCurrentInputControllerType(InputDevice inputDevice)
        {
            InputControllerType inputControllerType = InputControllerType.KeyboardMouse;
#if UNITY_SWITCH
            inputControllerType = InputControllerType.Switch;
#else
            switch (inputDevice)
            {
                case null:
                    inputControllerType = InputControllerType.DefaultGamePad;
                    break;
                case SwitchProControllerHID:
                    inputControllerType = InputControllerType.Switch;
                    break;
                case DualSenseGamepadHID:
                    inputControllerType = InputControllerType.DualSense;
                    break;
                case DualShockGamepad or DualShock3GamepadHID or DualShock4GamepadHID:
                    inputControllerType = InputControllerType.DualShock;
                    break;
                case XInputController:
                    inputControllerType = InputControllerType.Xbox;
                    break;
                case Gamepad:
                    inputControllerType = InputControllerType.DefaultGamePad;
                    break;
                case Keyboard:
                case Mouse:
                    inputControllerType = InputControllerType.KeyboardMouse;
                    break;
                default:
                    inputControllerType = InputControllerType.KeyboardMouse;
                    break;
            }

#endif
            return inputControllerType;
        }

        private void SwitchScheme(InputControllerType inputControllerType)
        {
            string schemaName = inputControllerType.ToString();
            InputDevice[] devices = null;
            switch (inputControllerType)
            {
                case InputControllerType.KeyboardMouse:
                    devices = new InputDevice[] { Keyboard.current, Mouse.current };
                    break;

                case InputControllerType.DualSense:
                case InputControllerType.DefaultGamePad:
                case InputControllerType.DualShock:
                case InputControllerType.Xbox:
                case InputControllerType.Switch:
                    devices = new InputDevice[] { Gamepad.current };
                    break;
            }


            _curInputController = inputControllerType;

            _playerInput.SwitchCurrentControlScheme(schemaName, devices);

            UpdateBindingMask(inputControllerType);
            UpdateGuideImage(inputControllerType);
        }

        private void UpdateBindingMask(InputControllerType inputControllerType)
        {
            ReadOnlyArray<InputActionMap> maps = UIInputAction.Instance.asset.actionMaps;
            foreach (InputActionMap map in maps)
            {
                map.bindingMask = InputBinding.MaskByGroup(inputControllerType.ToString());
            }
        }

        private void UpdateGuideImage(InputControllerType inputControllerType)
        {
        }
    }
}