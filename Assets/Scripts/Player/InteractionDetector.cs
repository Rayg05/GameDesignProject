using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private Interactable interactableInRange = null; //Closest Interactable
    private InteractPrompt promptInRange = null;
    public GameObject interactionIcon;

    void Start()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            interactableInRange?.interact();
        }

        if (promptInRange != null)
            promptInRange.SetVisible(interactableInRange != null && interactableInRange.canInteract());
    }

    // Kept for a PlayerInput "Interact" action if we switch to the new Input System
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            interactableInRange?.interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable) && interactable.canInteract())
        {
            if (promptInRange != null)
                promptInRange.Hide();
            interactableInRange = interactable;
            promptInRange = collision.GetComponent<InteractPrompt>();
            if (promptInRange != null)
                promptInRange.Show();
            if (interactionIcon != null)
                interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Interactable interactable) && interactable == interactableInRange)
        {
            interactable.endInteraction();
            interactableInRange = null;
            if (promptInRange != null)
                promptInRange.Hide();
            promptInRange = null;
            if (interactionIcon != null)
                interactionIcon.SetActive(false);
        }
    }
}
