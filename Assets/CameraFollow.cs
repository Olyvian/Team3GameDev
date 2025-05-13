using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The player or object to follow

    [Header("Camera Behavior")]
    public float smoothTime = 0.3f; // Approximate time for the camera to reach the target
    public Vector3 offset = new Vector3(0f, 2f, -10f); // Offset from the target (X, Y, Z). Z is crucial for 2D.

    [Header("Camera Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minCameraBounds; // Bottom-left corner of your level
    public Vector2 maxCameraBounds; // Top-right corner of your level

    private Vector3 velocity = Vector3.zero; // Used by SmoothDamp
    private Camera cam; // Reference to the camera component
    private float camHalfHeight;
    private float camHalfWidth;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("CameraFollow2D: Target not assigned!");
            enabled = false; // Disable script if no target
            return;
        }

        // Ensure the Z offset keeps the camera from being at the same Z as 2D sprites
        if (offset.z >= 0)
        {
            Debug.LogWarning("CameraFollow2D: Z offset should be negative (e.g., -10) to see 2D sprites.");
            offset.z = -10f;
        }

        // Get camera dimensions if using bounds
        if (useBounds)
        {
            cam = GetComponent<Camera>();
            if (cam == null)
            {
                Debug.LogError("CameraFollow2D: Camera component not found on this GameObject, but 'useBounds' is true.");
                useBounds = false; // Disable bounds if no camera
            }
            else if (!cam.orthographic)
            {
                Debug.LogWarning("CameraFollow2D: Camera is not orthographic. Bounds might not work as expected. Orthographic size is used for calculations.");
                // For perspective cameras, bounds are much more complex.
                // This script is primarily designed for orthographic 2D.
            }
            else
            {
                camHalfHeight = cam.orthographicSize;
                camHalfWidth = cam.aspect * camHalfHeight;
            }
        }
    }

    // LateUpdate is called after all Update functions have been called.
    // This is the best place to_update camera positions, as it ensures the target has finished moving for the frame.
    void LateUpdate()
    {
        if (target == null) return;

        // Calculate the desired position for the camera
        Vector3 targetPosition = target.position + offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Apply bounds if enabled
        if (useBounds && cam != null && cam.orthographic)
        {
            // Clamp X position
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x,
                                             minCameraBounds.x + camHalfWidth,
                                             maxCameraBounds.x - camHalfWidth);
            // Clamp Y position
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y,
                                             minCameraBounds.y + camHalfHeight,
                                             maxCameraBounds.y - camHalfHeight);
        }

        // Update the camera's position
        transform.position = smoothedPosition;
    }

    // Optional: Draw Gizmos in the editor to visualize bounds
    void OnDrawGizmosSelected()
    {
        if (useBounds)
        {
            Gizmos.color = Color.red;
            // Calculate the actual camera view rectangle based on bounds
            Vector3 center = new Vector3((minCameraBounds.x + maxCameraBounds.x) / 2, (minCameraBounds.y + maxCameraBounds.y) / 2, offset.z);
            Vector3 size = new Vector3(maxCameraBounds.x - minCameraBounds.x, maxCameraBounds.y - minCameraBounds.y, 0);
            Gizmos.DrawWireCube(center, size);
        }
    }
}