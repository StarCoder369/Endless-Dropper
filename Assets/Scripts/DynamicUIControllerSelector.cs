using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DynamicUIControllerSelector : MonoBehaviour
{
    public static DynamicUIControllerSelector Instance { get; private set; }

    private DynamicUIScreen currentActiveScreen;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (HasControllerConnected())
        {
            SelectFirstElementOnCurrentScreen();
        }
    }

    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added && (device is Gamepad || device is Joystick))
        {
            SelectFirstElementOnCurrentScreen();
        }
    }

    public void RegisterAndCheckScreen(DynamicUIScreen newScreen)
    {
        currentActiveScreen = newScreen;

        if (HasControllerConnected())
        {
            SelectFirstElementOnCurrentScreen();
        }
        else
        {
            Debug.Log("Clearing!");
            ClearSelection();
        }
    }

    private void SelectFirstElementOnCurrentScreen()
    {
        if (EventSystem.current == null || currentActiveScreen == null)
        {
            Debug.Log("Eventsystem null or currentactivescreen is null");
            return;
        }

        GameObject targetButton = currentActiveScreen.GetFirstSelectable();
        if (targetButton != null)
        {
            Debug.Log("Setting Target");
            EventSystem.current.SetSelectedGameObject(targetButton);
        }
        else
        {
            Debug.Log("Target Null");
        }

    }

    private void ClearSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private bool HasControllerConnected()
    {
        return Gamepad.all.Count > 0 || Joystick.all.Count > 0;
    }
}
