using UnityEngine;

public class Telescope : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject selectionObj;
    private bool inRange = false;
    // Frame the menu was last opened/closed, so one E press can't both open and close it.
    public static int lastToggleFrame = -1;

    private void Update() {
        // When the player is inside the telescope trigger, they can press E to open the menu.
        if(Input.GetKeyDown(KeyCode.E) && inRange && Time.frameCount != lastToggleFrame) {
            lastToggleFrame = Time.frameCount;
            player.SetActive(false);
            selectionScreen.SetActive(true);
            selectionObj.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        inRange = false;
    }
}
