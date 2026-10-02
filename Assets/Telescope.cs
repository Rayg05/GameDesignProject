using UnityEngine;

public class Telescope : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject selectionObj;
    private bool inRange = false;

    private void Update() {
        // When the player is inside the telescope trigger, they can press a key to open the menu.
        if(Input.GetKeyDown(KeyCode.Return) && inRange) {
            player.SetActive(false);
            selectionScreen.SetActive(true);
            selectionObj.SetActive(true);
            // Open the planet selection screen.
            Debug.Log("Open!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
        inRange = false;
    }
}
