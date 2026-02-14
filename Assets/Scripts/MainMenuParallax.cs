using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System [cite: 2026-01-21]

public class MainMenuParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float moveIntensity = 30f; 
    public float smoothSpeed = 5f;

    private Vector3 initialPosition;

    void Start()
    {
        // Store the starting position relative to the Canvas/Parent [cite: 2026-01-21]
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // Check if mouse is available [cite: 2026-01-21]
        if (Mouse.current == null) return;

        // Get mouse position from the New Input System [cite: 2026-01-21]
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Calculate offset (-1 to 1 range) [cite: 2026-01-21]
        float xOffset = (mousePos.x / Screen.width) * 2 - 1;
        float yOffset = (mousePos.y / Screen.height) * 2 - 1;

        // Target position based on mouse movement [cite: 2026-01-21]
        Vector3 targetPos = initialPosition + new Vector3(xOffset * moveIntensity, yOffset * moveIntensity, 0);

        // FIXED: Use unscaledDeltaTime because MainMenuManager sets timeScale to 0
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, 
            targetPos, 
            Time.unscaledDeltaTime * smoothSpeed
        );
    }
}