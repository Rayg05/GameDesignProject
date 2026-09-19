using UnityEngine;

public class Basic_Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveSpeed = 10f; // Speed at which the object moves
                              // Checks if the "D" key or Right Arrow is currently being held down
        float jumpForce = 20f; // Force applied when jumping

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    // Checks if the "A" key or Left Arrow is currently being held down
    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
    {
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
    }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.up * jumpForce * Time.deltaTime);
        }
    }
}
