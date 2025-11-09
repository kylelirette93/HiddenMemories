public interface IInteractable
{
    // Called when the player interacts with the object.
    public void OnInteract();
    // Called when the object gains or loses focus.
    public void SetFocus(bool isFocused);
    // Returns the interaction prompt text for the object.
    string GetInteractionPrompt();
}