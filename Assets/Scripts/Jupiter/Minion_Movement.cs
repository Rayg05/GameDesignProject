using UnityEngine;

public class Minion_Movement : MonoBehaviour
{
    [SerializeField] float leftX = -75f;
    [SerializeField] float rightX = 75f;
    [SerializeField] float speed = 35f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool spriteFacesRight = true;   // which way the sprite art points by default

    float targetX;
    float curY;

    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        curY = transform.position.y;

        // start by heading to whichever edge is farther away
        float x = transform.position.x;
        targetX = Mathf.Abs(x - rightX) > Mathf.Abs(x - leftX) ? rightX : leftX;
        UpdateFacing();
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, speed * Time.deltaTime);
        pos.y = curY;
        transform.position = pos;

        if (Mathf.Approximately(pos.x, targetX))
        {
            targetX = Mathf.Approximately(targetX, rightX) ? leftX : rightX;
            UpdateFacing();
        }
    }

    void UpdateFacing()
    {
        if (spriteRenderer == null) return;

        bool movingRight = targetX > transform.position.x;
        spriteRenderer.flipX = spriteFacesRight ? !movingRight : movingRight;
    }
}