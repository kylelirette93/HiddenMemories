using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractionManager : MonoBehaviour
{
    public Interactable currentInteractable = null;
    private Camera mainCamera;
    private Outline currentOutline;
    public float interactionDistance = 1000f;
    float sphereRadius = 0.5f;
    Vector2 mousePosition;

    private void Awake()
    {
        mainCamera = GameObject.Find("CameraHolder").GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        CheckForInteractable();
        HandleInteraction();
    }

    private void CheckForInteractable()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + transform.forward * 0.5f;
        if (Physics.SphereCast(origin, sphereRadius, transform.forward, out hit, 5f))
        {
            // Check if it's an interactable
            if (hit.transform.CompareTag("Interactable"))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                if (interactable.itemData.itemType == ItemType.Cash && Vector3.Distance(transform.position + transform.forward, interactable.gameObject.transform.position) < 0.1f)
                {
                    interactable.Interact();
                }

                // If this is a new interactable, update it
                if (interactable != currentInteractable)
                {
                    ClearCurrentInteractable();

                    currentInteractable = interactable;
                    currentOutline = currentInteractable.GetComponent<Outline>();

                    if (currentOutline != null)
                    {
                        currentOutline.enabled = true;
                    }

                    Debug.Log("Looking at: " + currentInteractable.name);
                }
                return;
            }
        }

        // If we didn't hit an interactable, clear the current one
        ClearCurrentInteractable();
    }

    private void HandleInteraction()
    {
        // Can use E key or mouse click
        if (currentInteractable != null &&
            (Keyboard.current.eKey.wasPressedThisFrame ||
             Mouse.current.leftButton.wasPressedThisFrame))
        {
            currentInteractable.Interact();
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
        }

        currentInteractable = null;
        currentOutline = null;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Visualize the spherecast
        Vector3 startPos = transform.position + transform.forward * 0.5f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPos, sphereRadius);
        Gizmos.DrawLine(startPos, startPos + transform.forward * interactionDistance);
        Gizmos.DrawWireSphere(startPos + transform.forward * interactionDistance, sphereRadius);
    }
}