using UnityEngine;

public class PlanetSelection : MonoBehaviour
{
    [SerializeField]
    private Transform[] planetPositions;
    [SerializeField]
    public GameObject maincam;
    private int numPlanets;
    private int targetX = 0;
    private int targetY = 0;
    Vector3 targetPos;

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
        }

        // RIGHT
        if (Input.GetKeyDown(KeyCode.D)) {
            // Move selection.
            if(targetX >= numPlanets-1) { targetX = 0; } else { targetX++; }
            // Set target position.
            targetPos = planetPositions[targetX].position;
        }



        // Set selection position.
        if(transform.position != targetPos) {
            //maincam.transform.position = Vector3.Lerp(maincam.transform.position, targetPos, 0.2f);
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.2f);
        }
    }
}
