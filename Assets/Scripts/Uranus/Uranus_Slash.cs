using System.Net;
using UnityEngine;

public class Uranus_Slash : IState
{
    readonly Boss uranus;
    readonly Material lineMaterial;

    const float drawDuration = .3f;    // time to draw the line from start to end
    const float holdDuration = 2.2f;    // how long the finished line stays before the state ends
    float lineWidth = 1f;
    float arenaMinY = -100f; 
    float arenaMaxY = 100f;
    float arenaMinX = -100f;
    float arenaMaxX = 100f;
    float flashDuration = 0.3f;
    float totalDuration = drawDuration + holdDuration;
    const int numOfSlashes = 10;
    Color lineColor = Color.white;

    LineRenderer[] lineRenderers = new LineRenderer[numOfSlashes];
    Vector3[] startPoints = new Vector3[numOfSlashes];
    Vector3[] endPoints = new Vector3[numOfSlashes];
    float timer;

    public Uranus_Slash(Boss boss)
    {
        uranus = boss;
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void Enter()
    {
        Debug.Log("Entering Uranus_Slash.");
        timer = 0f;
        for (int i = 0; i < numOfSlashes; i++)
        {
            // Pick start and end points: a diagonal across the arena, random direction.
            float y1 = Random.Range(arenaMinY, arenaMaxY);
            float y2 = Random.Range(arenaMinY, arenaMaxY);

            bool leftToRight = Random.value < 0.5f;
            float x1 = leftToRight ? arenaMaxX : arenaMinX;
            float x2 = leftToRight ? arenaMinX : arenaMaxX;
            startPoints[i] = new Vector3(x1, y1, 0f);
            endPoints[i] = new Vector3(x2, y2, 0f); 

            //Debug.Log(y1 + "       " + y2);

            // Create the line. Not parented to the boss, because the boss flips its localScale.
            GameObject go = new GameObject("UranusSlash");
            lineRenderers[i] = go.AddComponent<LineRenderer>();
            lineRenderers[i].useWorldSpace = true;
            lineRenderers[i].material = lineMaterial;
            lineRenderers[i].sortingOrder = 50;
            lineRenderers[i].startWidth = lineWidth;
            lineRenderers[i].endWidth = lineWidth;
            lineRenderers[i].startColor = lineColor;
            lineRenderers[i].endColor = Color.red;
            lineRenderers[i].positionCount = 2;
            lineRenderers[i].SetPosition(0, startPoints[i]);
            lineRenderers[i].SetPosition(1, startPoints[i]);
        }
    }

    public void Execute()
    {
        // Advance the clock once per frame, not once per slash.
        timer += Time.deltaTime;

        // All slashes share the same progress value this frame.
        float progress = Mathf.Clamp01(timer / drawDuration);

        for (int i = 0; i < numOfSlashes; i++)
        {
            if (lineRenderers[i] == null) continue;

            //Debug.DrawLine(startPoints[i], endPoints[i], Color.red);
            lineRenderers[i].SetPosition(1, Vector3.Lerp(startPoints[i], endPoints[i], progress));
            //Debug.DrawLine(lineRenderers[i].GetPosition(0), lineRenderers[i].GetPosition(1), Color.green);
        }

        if (timer >= totalDuration - flashDuration && timer < totalDuration)
        {
            for(int i = 0; i < numOfSlashes; i++)
            {
                if (lineRenderers[i] == null) continue;
                // Flash the line red for a brief moment before leaving
                lineRenderers[i].startColor = Color.black;
                lineRenderers[i].endColor = Color.black;
            }
        }
        if (timer >= totalDuration)
        {
            uranus.ChooseNextState(this);
        }
    }

    public void Exit()
    {
        for(int i = 0; i < numOfSlashes; i++)
        {
            if (lineRenderers[i] != null)
                Object.Destroy(lineRenderers[i].gameObject);
            lineRenderers[i] = null;
        }
        Debug.Log("Exiting Uranus_Slash.");
    }
}