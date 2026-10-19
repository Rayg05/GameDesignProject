using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; //target used for player
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -5f); //initial offset of camera
    [SerializeField] private float smoothSpeed = 5f; //movement speed of camera
    [SerializeField] private Renderer bounds; //optional: camera won't show past the left/right edges of this (e.g. the floor)
    [SerializeField] private bool snapOnStart = true; //start already on the target instead of gliding/zooming to it

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (snapOnStart) SnapToTarget(); //skip the intro zoom
    }

    public void SnapToTarget()
    {
        if (target != null) transform.position = GetDesiredPosition(); //jump straight to the target without gliding
    }

    void LateUpdate()
    {
        if (target == null) return; // base case

        transform.position = Vector3.Lerp(transform.position, GetDesiredPosition(), smoothSpeed * Time.deltaTime); //move the camera
    }

    Vector3 GetDesiredPosition()
    {
        Vector3 desiredPosition = target.position + offset; //calculate desired position of camera based on target position and offset
        if (bounds != null) desiredPosition.x = ClampToBounds(desiredPosition); //keep the view inside the bounds
        return desiredPosition;
    }

    float ClampToBounds(Vector3 cameraPosition)
    {
        Bounds b = bounds.bounds;

        // half the width the camera can see at the depth of the bounds object
        float halfHeight;
        if (cam.orthographic)
            halfHeight = cam.orthographicSize;
        else
            halfHeight = Mathf.Abs(b.center.z - cameraPosition.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float halfWidth = halfHeight * cam.aspect;

        float minX = b.min.x + halfWidth;
        float maxX = b.max.x - halfWidth;
        if (minX > maxX) return b.center.x; // bounds narrower than the view: just center on it

        return Mathf.Clamp(cameraPosition.x, minX, maxX);
    }
}
