using System;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Interactable currentInteractable = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            Interactable currentInteractable = other.GetComponent<Interactable>();
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
                Debug.Log("Interacting with: " + currentInteractable);
                other.gameObject.SetActive(false);
            }
            // Ideally show feedback or prompt for interactable as well.
        }
    }

    private void Update()
    {
       // TODO: Handle clicking with input manager in future. 
    }



}
