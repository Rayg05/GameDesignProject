using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; //target used for player
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -5f); //initial offset of camera 
    [SerializeField] private float smoothSpeed = 5f; //movement speed of camera 

    void LateUpdate()
    {
        if (target == null) return; // base case 

        Vector3 desiredPosition = target.position + offset; //calculate desired position of camera based on target position and offset
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); //move the camera 
    }
}