using UnityEngine;

public class Larissa_Movement : MonoBehaviour
{
    private Animator idle_Larissa_Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        idle_Larissa_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float animationInput = Input.GetAxisRaw("Horizontal");
        idle_Larissa_Animator.SetFloat("isMoving", Mathf.Abs(animationInput));
        if (animationInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (animationInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

    }
}
