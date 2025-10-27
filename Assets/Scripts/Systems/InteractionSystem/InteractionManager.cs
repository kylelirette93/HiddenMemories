using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    // Configuration fields - set these in the Inspector
    public float interactionRadius = 0.5f; // How wide the sphere is
    public float interactionDistance = 2f; // How far the sphere is cast
    public LayerMask interactableLayer;    // Which layer to check for

    // We don't need 'currentInteractable' public field anymore for this simple setup

    public void Update()
    {
        CheckForInteractable();
    }
    // This method is called by the player's input system (e.g., when they press the 'E' key)
    public void CheckForInteractable()
    {
        // Perform the SphereCast
        // Arguments: origin, radius, direction, out hitInfo, maxDistance, layerMask
        if (Physics.SphereCast(
            transform.position,
            interactionRadius,
            transform.forward, // Cast in the direction the object is facing (e.g., player's "look" direction)
            out RaycastHit hit,
            interactionDistance,
            interactableLayer))
        {
            // We hit something! Try to get the Interactable component.
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact();
                if (interactable.type != InteractionType.Door)
                {
                    interactable.gameObject.SetActive(false);
                }
                Debug.Log("Interacting with: " + interactable.gameObject.name);
                // No need to store a reference, we used it immediately.
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // Draw the sphere at the start position
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        // Draw the sphere at the max hit distance (if it's not a hit)
        Vector3 endPoint = transform.position + transform.forward * interactionDistance;
        Gizmos.DrawWireSphere(endPoint, interactionRadius);

        // Draw a line connecting the centers
        Gizmos.DrawLine(transform.position, endPoint);
    }
}