using UnityEngine;
using UnityEngine.InputSystem; 

// Creates parallax effect on Main Menu background

public class MainMenuParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    public float moveIntensity = 30f; 
    public float smoothSpeed = 5f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
       
        float xOffset = (mousePos.x / Screen.width) * 2 - 1;
        float yOffset = (mousePos.y / Screen.height) * 2 - 1;
       
        Vector3 targetPos = initialPosition + new Vector3(xOffset * moveIntensity, yOffset * moveIntensity, 0);

        transform.localPosition = Vector3.Lerp(
            transform.localPosition, 
            targetPos, 
            Time.unscaledDeltaTime * smoothSpeed
        );
    }
}