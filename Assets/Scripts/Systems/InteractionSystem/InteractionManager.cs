using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public float interactionRadius = 0.5f; 
    public float interactionDistance = 2f; 
    public LayerMask interactableLayer; 

    public void Update()
    {
        CheckForInteractable();
    }
    public void CheckForInteractable()
    {
        // Sphere cast to detect objects in front of player.
        if (Physics.SphereCast(
            transform.position,
            interactionRadius,
            transform.forward, 
            out RaycastHit hit,
            interactionDistance,
            interactableLayer))
        { 
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if (interactable != null)
            {
                interactable.Interact();
                if (interactable.type != InteractionType.Door)
                {
                    interactable.gameObject.SetActive(false);
                }
                Debug.Log("Interacting with: " + interactable.gameObject.name);
            }
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        Vector3 endPoint = transform.position + transform.forward * interactionDistance;
        Gizmos.DrawWireSphere(endPoint, interactionRadius);

        Gizmos.DrawLine(transform.position, endPoint);
    }*/
}