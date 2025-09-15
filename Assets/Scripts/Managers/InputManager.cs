using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Input.IPlayerActions
{
    Input input;
    void Awake()
    {
        input = new Input();
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    }
    public void OnLook(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}
