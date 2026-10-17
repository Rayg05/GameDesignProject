using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float leftBorder = -10f;
    public float rightBorder = 10f;

    private bool movingRight = false;

    void Update()
    {
        // 1. Move the object in the current direction
        if (movingRight)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // 2. Check boundaries and flip direction if exceeded
        if (transform.position.x >= rightBorder)
        {
            movingRight = false;
            // Snap exactly to the border to prevent getting stuck
            transform.position = new Vector3(rightBorder, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= leftBorder)
        {
            movingRight = true;
            // Snap exactly to the border to prevent getting stuck
            transform.position = new Vector3(leftBorder, transform.position.y, transform.position.z);
        }
    }
}
