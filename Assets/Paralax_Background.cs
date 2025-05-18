using UnityEngine;

public class Parallax_Background : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The main camera in the scene.")]
    public Transform cameraTransform;

    [Header("Layer Settings")]
    [Tooltip("How fast this layer moves relative to the camera. 0 = static, 1 = moves with camera.")]
    [Range(0f, 1f)]
    public float parallaxEffectX = 0.5f;
    [Range(0f, 1f)]
    public float parallaxEffectY = 0.1f; // Often Y parallax is less or zero

    [Header("Infinite Scrolling")]
    [Tooltip("Enable if this layer should loop horizontally.")]
    public bool infiniteHorizontal = true;
    [Tooltip("Enable if this layer should loop vertically.")]
    public bool infiniteVertical = false; // Usually less common

    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private Vector3 startPosition; // Starting position of this layer

    void Start()
    {
        if (cameraTransform == null)
        {
            // Try to find the main camera if not assigned
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
                Debug.LogWarning("ParallaxBackground: Camera not assigned, using Camera.main.");
            }
            else
            {
                Debug.LogError("ParallaxBackground: Camera not assigned and Camera.main not found. Disabling script.");
                enabled = false;
                return;
            }
        }

        lastCameraPosition = cameraTransform.position;
        startPosition = transform.position;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            Texture2D texture = spriteRenderer.sprite.texture;
            // Calculate the world size of the sprite
            // This assumes the sprite is not scaled in weird ways and pixels per unit is consistent
            textureUnitSizeX = (texture.width / spriteRenderer.sprite.pixelsPerUnit) * transform.localScale.x;
            textureUnitSizeY = (texture.height / spriteRenderer.sprite.pixelsPerUnit) * transform.localScale.y;
        }
        else if (infiniteHorizontal || infiniteVertical)
        {
            Debug.LogWarning("ParallaxBackground: SpriteRenderer or Sprite not found. Infinite scrolling may not work correctly without texture unit size.", this);
            // Fallback if no sprite, try to use bounds, but this is less reliable for precise tiling
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null) {
                textureUnitSizeX = renderer.bounds.size.x;
                textureUnitSizeY = renderer.bounds.size.y;
            } else {
                textureUnitSizeX = 1; // Default to avoid division by zero
                textureUnitSizeY = 1;
                if (infiniteHorizontal || infiniteVertical)
                    Debug.LogError("ParallaxBackground: Cannot determine texture size for infinite scrolling.", this);
            }
        }
    }

    void LateUpdate() // Use LateUpdate to ensure camera has finished its movement for the frame
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Calculate the parallax movement
        float parallaxX = deltaMovement.x * parallaxEffectX;
        float parallaxY = deltaMovement.y * parallaxEffectY;

        transform.position += new Vector3(parallaxX, parallaxY, 0);

        lastCameraPosition = cameraTransform.position;

        // Infinite scrolling logic
        if (infiniteHorizontal && textureUnitSizeX > 0)
        {
            // How far the camera has moved relative to the layer's "still" point
            // This is how much the background *appears* to have moved if it were static
            float cameraRelativeMoveX = cameraTransform.position.x * (1 - parallaxEffectX);

            if (cameraRelativeMoveX > startPosition.x + textureUnitSizeX)
            {
                // Moved one full texture width to the right, shift start position
                startPosition.x += textureUnitSizeX;
            }
            else if (cameraRelativeMoveX < startPosition.x - textureUnitSizeX)
            {
                // Moved one full texture width to the left, shift start position
                startPosition.x -= textureUnitSizeX;
            }
        }

        if (infiniteVertical && textureUnitSizeY > 0)
        {
            float cameraRelativeMoveY = cameraTransform.position.y * (1 - parallaxEffectY);
            if (cameraRelativeMoveY > startPosition.y + textureUnitSizeY)
            {
                startPosition.y += textureUnitSizeY;
            }
            else if (cameraRelativeMoveY < startPosition.y - textureUnitSizeY)
            {
                startPosition.y -= textureUnitSizeY;
            }
        }

        // Apply the new position considering the parallax and the wrapped start position
        // The 'dist' is how much this layer should have moved *purely* due to parallax from its effective starting point
        if (infiniteHorizontal || infiniteVertical)
        {
            float newX = transform.position.x;
            float newY = transform.position.y;

            if (infiniteHorizontal)
            {
                float distanceMovedByParallaxX = cameraTransform.position.x * parallaxEffectX;
                newX = startPosition.x + distanceMovedByParallaxX;
            }
            if (infiniteVertical)
            {
                float distanceMovedByParallaxY = cameraTransform.position.y * parallaxEffectY;
                newY = startPosition.y + distanceMovedByParallaxY;
            }
            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
}