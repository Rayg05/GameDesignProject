public interface Interactable
{
    void interact();
    bool canInteract();
    void endInteraction(); // Called when the player leaves interaction range
}