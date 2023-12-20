// CameraFollowScript.cs

using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform target;        // The target to follow
    public float smoothSpeed = 0.125f;  // The speed at which the camera follows the target
    public Vector3 offset;          // The offset between the camera and the target

    [Header("Bounds")]
    public bool useBounds = false;   // Enable if you want to restrict camera movement within certain bounds
    public Vector2 minBounds;        // Minimum bounds for the camera
    public Vector2 maxBounds;        // Maximum bounds for the camera

    [Header("Damping")]
    public bool useDamping = true;   // Enable for smooth damping
    public float dampingFactor = 2f; // Damping factor when using damping

    void LateUpdate()
    {
        if (target == null)
        {
            // If there is no target, do nothing
            return;
        }

        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + offset;

        // Apply bounds to the desired position if enabled
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // Use smoothing or damping based on user preference
        if (useDamping)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 1 / (dampingFactor * Time.deltaTime));
            transform.position = smoothedPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }

        // Make sure the camera is always looking at the target
        transform.LookAt(target);
    }
}
