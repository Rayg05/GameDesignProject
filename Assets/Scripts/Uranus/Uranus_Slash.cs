using NUnit;
using System.Net;
using UnityEngine;

public class Uranus_Slash : IState
{
    readonly Uranus_Boss uranus;
    readonly Material lineMaterial;
    readonly Animator animator;

    const float drawDuration = .3f;    // time to draw the line from start to end
    const float holdDuration = 2.2f;    // how long the finished line stays before the state ends
    float lineWidth = 2f;
    float arenaMinY = -100f; 
    float arenaMaxY = 100f;
    float arenaMinX = -100f;
    float arenaMaxX = 100f;
    float flashDuration = 0.3f;
    float totalDuration = drawDuration + holdDuration;
    const int numOfSlashes = 10;

    LineRenderer[] lineRenderers = new LineRenderer[numOfSlashes];
    private BoxCollider2D[] boxColliders = new BoxCollider2D[numOfSlashes];
    Vector3[] startPoints = new Vector3[numOfSlashes];
    Vector3[] endPoints = new Vector3[numOfSlashes];
    float timer;

    public Uranus_Slash(Uranus_Boss boss)
    {
        uranus = boss;
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        animator = uranus.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("Entering Uranus_Slash.");
        timer = 0f;
        animator.SetBool("Slash", true);
        for (int i = 0; i < numOfSlashes; i++)
        {
            // pick start and end points for each slash
            float y1 = Random.Range(arenaMinY, arenaMaxY);
            float y2 = Random.Range(arenaMinY, arenaMaxY);

            //randomly choose if it should draw from right to left or left to right
            bool leftToRight = Random.value < 0.5f;
            float x1 = leftToRight ? arenaMaxX : arenaMinX;
            float x2 = leftToRight ? arenaMinX : arenaMaxX;

            //gives a vector for the start and end points of the line
            startPoints[i] = new Vector3(x1, y1, 0f);
            endPoints[i] = new Vector3(x2, y2, 0f);

            //used for box colllider
            Vector3 dir = endPoints[i] - startPoints[i];
            float length = dir.magnitude;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //create a new GameObject for the line renderer and box collider
            GameObject go = new GameObject("UranusSlash");
            BossDamage.Apply(go, uranus.slashDamage);
            go.transform.position = (startPoints[i] + endPoints[i]) / 2f;
            go.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            LineRenderer lr = go.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.startColor = Color.white;
            lr.endColor = Color.red;
            lr.positionCount = 2;
            lr.SetPosition(0, startPoints[i]);
            lr.SetPosition(1, startPoints[i]);
            lineRenderers[i] = lr;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(length, lineWidth);
            col.enabled = false;   // only turned on during the flash
            boxColliders[i] = col;
        }
    }

    public void Execute()
    {
        // clock the timer for this state
        timer += Time.deltaTime;

        // slashes share the same progress value 
        float progress = Mathf.Clamp01(timer / drawDuration);

        for (int i = 0; i < numOfSlashes; i++)
        {
            //if the line renderer is null, skip this iteration
            if (lineRenderers[i] == null) continue;

            //otherwise, update the line renderer's end position based on the progress
            lineRenderers[i].SetPosition(1, Vector3.Lerp(startPoints[i], endPoints[i], progress));
        }

        // After the draw duration, hold the lines for a while before exiting the state
        if (timer >= totalDuration - flashDuration && timer < totalDuration)
        {
            // Flash the lines black for a brief moment before leaving indicating when the attack hits
            for (int i = 0; i < numOfSlashes; i++)
            {
                if (lineRenderers[i] == null) continue;
                // Flash the line red for a brief moment before leaving
                lineRenderers[i].startColor = Color.black;
                lineRenderers[i].endColor = Color.black;
                boxColliders[i].enabled = true; //this actually does the damage 
            }
        }

        //check if the total duration has been reached to exit the state
        if (timer >= totalDuration)
        {
            uranus.ChooseNextState(this);
        }
    }

    public void Exit()
    {
        animator.SetBool("Slash", false);
        // Clean up the line renderers when exiting the state
        for (int i = 0; i < numOfSlashes; i++)
        {
            if (lineRenderers[i] != null)
                Object.Destroy(lineRenderers[i].gameObject);
            lineRenderers[i] = null;
            boxColliders[i] = null;
        }
        Debug.Log("Exiting Uranus_Slash.");
    }
}