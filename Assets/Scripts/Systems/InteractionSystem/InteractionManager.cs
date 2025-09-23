using System;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Interactable currentInteractable = null;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.GetComponent<Interactable>();
            currentInteractable.Interact();
            currentInteractable = null;
            other.gameObject.SetActive(false);
            Debug.Log("Interacting with: " + currentInteractable);
            // Ideally show feedback or prompt for interactable as well.
        }
    }

    private void Update()
    {
       // TODO: Handle clicking with input manager in future. 
    }



}
