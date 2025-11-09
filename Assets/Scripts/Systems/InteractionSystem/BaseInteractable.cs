using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Outline))]
public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Highlight Effect Settings")]
    protected Outline outline;
    protected bool isFocused = false;
    protected string interactionPromptText;


    void Awake()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
        #region Initialize Highlight Effect
        outline = GetComponent<Outline>();
        outline.OutlineColor = Color.green;
        outline.OutlineWidth = 4f;
        outline.enabled = false;
        #endregion
    }
    public string GetInteractionPrompt()
    {
        if (string.IsNullOrWhiteSpace(interactionPromptText))
        {
            return "Press E to Interact";
        }
        else
        {
            return interactionPromptText;
        }
    }

    public virtual void OnInteract()
    {
       // Debug.Log("Interacted with " + gameObject.name);
    }

    public void SetFocus(bool focused)
    {
        if (isFocused == focused) return;

        isFocused = focused;

        if (isFocused)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
        }
    }
}