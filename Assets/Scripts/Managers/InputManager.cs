using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, Input.IPlayerActions
{
    Input input;
    UI uiInput;
    void Awake()
    {
        try
        {
            input = new Input();
            input.Player.SetCallbacks(this);
            input.Player.Enable();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"InputManager: Exception during initialization: {ex.Message}");
        }
    }

    #region Input Events
    public event Action<Vector2> MoveInputEvent;
    public event Action<Vector2> LookInputEvent;
    public event Action<InputAction.CallbackContext> JumpEvent;
    public event Action<InputAction.CallbackContext> SprintEvent;
    public event Action<InputAction.CallbackContext> ShootEvent;
    public event Action<InputAction.CallbackContext> AimEvent;
    public event Action PauseEvent;
    public event Action<Vector2> ScrollWeaponEvent;
    #endregion

    void OnEnable()
    {
        if (input != null)
        {
            input.Player.Enable();
        }
    }

    void OnDestroy()
    {
        if (input != null)
        {
            input.Player.Disable();
        }
    }

  
    public void OnLook(InputAction.CallbackContext context)
    {
        LookInputEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInputEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        JumpEvent?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintEvent?.Invoke(context);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        ShootEvent?.Invoke(context);
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        AimEvent?.Invoke(context);
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            ScrollWeaponEvent?.Invoke(scrollValue);
        }
    }
}
