using UnityEngine;

public class Telescope : MonoBehaviour
{
    private bool inRange = false;

    private void Update() {
        // When the player is inside the telescope trigger, they can press a key to open the menu.
        if(Input.GetKeyDown(KeyCode.Return) && inRange) {
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
