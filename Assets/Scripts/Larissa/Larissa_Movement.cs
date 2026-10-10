using UnityEngine;

public class Larissa_Movement : MonoBehaviour
{
    private Animator Larissa_Idle_Larissa_Animator;
    [SerializeField] private float amplitude = 0.25f;  //distance it moves
    [SerializeField] private float frequency = 1f;     //frequency at which the object goes up or down
    private Vector3 startPos;

    void Start()
    {
        Larissa_Idle_Larissa_Animator = GetComponent<Animator>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * frequency * Mathf.PI * 2f) * amplitude;
        transform.position = new Vector3(startPos.x, startPos.y + offset, startPos.z);
    }
}
