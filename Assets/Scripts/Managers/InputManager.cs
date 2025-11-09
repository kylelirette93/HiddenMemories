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
    public event Action<InputAction.CallbackContext> JumpInputEvent;
    public event Action<InputAction.CallbackContext> CrouchInputEvent;
    public event Action<InputAction.CallbackContext> SprintInputEvent;
    public event Action<InputAction.CallbackContext> ShootEvent;
    public event Action<InputAction.CallbackContext> AimEvent;
    public event Action<InputAction.CallbackContext> UsePotionEvent;
    public event Action PauseEvent;
    public event Action<Vector2> ScrollWeaponEvent;
    public event Action<InputAction.CallbackContext> ReloadEvent;
    public event Action<InputAction.CallbackContext> InteractInputEvent;
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
        JumpInputEvent?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        SprintInputEvent?.Invoke(context);
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

    public void OnPotion(InputAction.CallbackContext context)
    {
        UsePotionEvent?.Invoke(context);
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        ReloadEvent?.Invoke(context);
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        CrouchInputEvent?.Invoke(context);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        InteractInputEvent?.Invoke(context);
    }
}
