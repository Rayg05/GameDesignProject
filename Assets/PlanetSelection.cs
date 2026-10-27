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
    [SerializeField] private Color errorColor = new Color(0.8f, 0.1f, 0.1f, 0.9f); // banner background
    [SerializeField] private float errorDuration = 2f;
    private GameObject errorBanner; // fixed above the controls bar, not attached to the planet
    private float errorShownTime;
    private float errorHideTime;

    private GameObject controlsHint; // [A / D] Browse  [E] Select  [ESC] Back

    private string[] planetNames = { "Neptune", "Uranus", "Saturn", "Jupiter", "Other" };
    // boss scene for each planet above, in the same order; empty = no boss scene yet
    private string[] bossScenes = { "LarissaBoss", "UranusBoss", "", "", "" };
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
        if (errorBanner == null)
            errorBanner = KeyHintBar.CreateWarning("No Loadout Warning", noLoadoutMessage, errorColor);
    }

    private void OnDisable() {
        if (controlsHint != null) { controlsHint.SetActive(false); }
        if (errorBanner != null) { errorBanner.SetActive(false); }
    }

    void Start() {
        // Get the number of planets.
        numPlanets = planetPositions.Length;
        // Set initial position to the leftmost planet.
        targetPos = planetPositions[0].position;

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
                errorBanner.SetActive(true);
                errorShownTime = Time.unscaledTime;
                errorHideTime = errorShownTime + errorDuration;
            } else {
                string sceneName = bossScenes[targetX];

                // GO TO ROOM HERE (planets without a boss scene in the build yet do nothing).
                if (Application.CanStreamedLevelBeLoaded(sceneName))
                    SceneFader.LoadScene(sceneName);
                else
                    Debug.LogWarning($"No boss scene for {planetNames[targetX]} in the build yet");
            }
        }

        // Pop the warning in so it catches the eye, hide it once its time is up.
        if (errorBanner.activeSelf) {
            float t = Mathf.Clamp01((Time.unscaledTime - errorShownTime) / 0.15f);
            errorBanner.transform.GetChild(0).localScale = Vector3.one * Mathf.Lerp(1.25f, 1f, t);
            if (Time.unscaledTime >= errorHideTime) { errorBanner.SetActive(false); }
        }

        // Exit out of telescope screen with Escape (E is used to select).
        if(!SceneFader.IsTransitioning && Input.GetKeyDown(KeyCode.Escape)) {
            Telescope.lastToggleFrame = Time.frameCount;
            errorBanner.SetActive(false);
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
