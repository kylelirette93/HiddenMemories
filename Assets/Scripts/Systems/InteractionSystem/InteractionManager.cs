using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public Interactable currentInteractable = null;
    InputManager input;

    private void Awake()
    {
        input = GameManager.Instance.inputManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            currentInteractable = other.GetComponent<Interactable>();
            // Ideally show feedback or prompt for interactable as well.
        }
    }

    private void Update()
    {
       // TODO: Handle clicking with input manager in future. 
    }



}
