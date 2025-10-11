using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI; 

public class UIPanel : MonoBehaviour
{
    public Selectable firstSelectedElement;
    Button[] selectableButtons;

    private void Awake()
    {
        selectableButtons = GetComponentsInChildren<Button>(true);
    }
    private void OnEnable()
    {
        SelectFirstElement();
    }
    public void SelectFirstElement()
    {
        bool isGamepadConnected = (Gamepad.current != null);
        if (isGamepadConnected)
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("No EventSystem found in the scene! Cannot set selection.");
                return;
            }

            EventSystem.current.SetSelectedGameObject(null);

            if (firstSelectedElement != null)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedElement.gameObject);
            }
            else
            {
                Debug.LogWarning($"UIPanel on {gameObject.name} is missing a First Selected Element reference.");
            }
        }
    }

    private void Update()
    {
        if (firstSelectedElement.interactable == false)
        {
            foreach (Button button in selectableButtons)
            {
                if (button.interactable)
                {
                    firstSelectedElement = button;
                    SelectFirstElement();
                    break;
                }
            }
        }
    }
}