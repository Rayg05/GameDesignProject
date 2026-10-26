using TMPro;
using UnityEngine;

public class PlanetSelection : MonoBehaviour
{
    [SerializeField] private Transform[] planetPositions;
    [SerializeField] private GameObject maincam;
    [SerializeField] private TextMeshProUGUI tmp;
    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject player;

    [Header("No loadout error")]
    [SerializeField] private string noLoadoutMessage = "No loadout equipped";
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private float errorDuration = 2f;
    [SerializeField] private Vector3 errorOffset = new Vector3(0f, -1.5f, 0f); // from the planet name text
    private TextMeshProUGUI errorText;
    private float errorHideTime;

    private GameObject controlsHint; // [A / D] Browse  [E] Select  [ESC] Back

    private string[] planetNames = { "Neptune", "Uranus", "Saturn", "Jupiter", "Other" };
    private int numPlanets;
    private int targetX = 0;
    private int targetY = 0;
    Vector3 targetPos;

    private void OnEnable() {
        // Stop following the player and point the camera at the menu.
        maincam.GetComponent<CameraFollow>().enabled = false;
        Vector3 menuPos = selectionScreen.transform.position;
        maincam.transform.position = new Vector3(menuPos.x, menuPos.y, maincam.transform.position.z);

        // Show the controls while the menu is open.
        if (controlsHint == null)
            controlsHint = KeyHintBar.Create("Planet Select Controls", ("A / D", "Browse"), ("E", "Select"), ("ESC", "Back"));
        controlsHint.SetActive(true);
    }

    private void OnDisable() {
        if (controlsHint != null) { controlsHint.SetActive(false); }
    }

    void Start() {
        // Get the number of planets.
        numPlanets = planetPositions.Length;
        // Set initial position to the leftmost planet.
        targetPos = planetPositions[0].position;

        // Copy the planet name text so the error uses the same font and canvas.
        errorText = Instantiate(tmp, tmp.transform.parent);
        errorText.name = "NoLoadoutError";
        errorText.text = noLoadoutMessage;
        errorText.color = errorColor;
        errorText.gameObject.SetActive(false);

        // DEBUG
        for(int i = 0; i < planetPositions.Length; i++) {
            Debug.Log(planetPositions[i].position.x);
        }
    }

    private void Update() {
        // LEFT
        if (Input.GetKeyDown(KeyCode.A)) {
            // Move selection.
            if (targetX <= 0) { targetX = numPlanets-1; } else { targetX--; }
            // Set target position.
            targetPos = planetPositions[targetX].position;
            tmp.text = planetNames[targetX];
        }

        // RIGHT
        if (Input.GetKeyDown(KeyCode.D)) {
            // Move selection.
            if(targetX >= numPlanets-1) { targetX = 0; } else { targetX++; }
            // Set target position.
            targetPos = planetPositions[targetX].position;
            tmp.text = planetNames[targetX];
        }


        // Check for selection.
        /*
         *                          IMPORTANT NOTE:
         *          SCENES MUST BE IN YOUR CURRENT BUILD PROFILE.
         *          THEY DO NOT SAVE OVER GITHUB. THEY ARE PER-DEVICE.
         */
        // Skip the frame the telescope opened, so the E that opened it doesn't also pick a planet.
        bool selectPressed = Input.GetKeyDown(KeyCode.E) && Time.frameCount != Telescope.lastToggleFrame;
        if(!SceneFader.IsTransitioning && selectPressed) {
            // Clean up current scene here.

            if (!Loadout.HasAnyEquipped()) {
                // Can't fight a boss empty-handed.
                errorText.gameObject.SetActive(true);
                errorHideTime = Time.unscaledTime + errorDuration;
            } else {
                // Create name of scene ([Planet]Boss)
                string sceneName = planetNames[targetX] + "Boss";

                // GO TO ROOM HERE.
                SceneFader.LoadScene(sceneName);
            }
        }

        // Keep the error under the planet name, hide it once its time is up.
        if (errorText.gameObject.activeSelf) {
            errorText.transform.position = tmp.transform.position + errorOffset;
            if (Time.unscaledTime >= errorHideTime) { errorText.gameObject.SetActive(false); }
        }

        // Exit out of telescope screen with Escape (E is used to select).
        if(!SceneFader.IsTransitioning && Input.GetKeyDown(KeyCode.Escape)) {
            Telescope.lastToggleFrame = Time.frameCount;
            errorText.gameObject.SetActive(false);
            player.SetActive(true);
            selectionScreen.SetActive(false);
            // Go back to following the player.
            CameraFollow follow = maincam.GetComponent<CameraFollow>();
            follow.enabled = true;
            follow.SnapToTarget();
            gameObject.SetActive(false);
        }


        // Set selection position.
        if(transform.position != targetPos) {
            //maincam.transform.position = Vector3.Lerp(maincam.transform.position, targetPos, 0.2f);
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
            tmp.transform.position = new Vector3(transform.position.x, transform.position.y - 3, 0);
                //new Vector3(targetPos.x, targetPos.y - 3, 0);
        }
    }
}
