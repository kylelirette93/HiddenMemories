using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public InteractionType type;
    // Reference to scriptable object associated with this interactable.
    public ItemDataSO itemData;
    Action Pickup;
    public void Interact()
    {
        switch (type)
        {
            case InteractionType.Pickup:
                Pickup?.Invoke();
                break;
        }
    }
}

public enum InteractionType
{
    Pickup
}
