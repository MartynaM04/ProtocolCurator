using UnityEngine;
using UnityEngine.InputSystem;

/// Controls flashlight states: Off, Normal, and Bright

public class FlashlightSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject lightObject;
    private Light lightComponent;

    [Header("Brightness Settings")]
    public float normalBrightness = 1.5f;
    public float brightBrightness = 3.0f;

    [Header("Range Settings")]
    public float normalRange = 25f;
    public float brightRange = 45f;

    private int flashlightState = 0; // 0 = Off, 1 = Normal, 2 = Bright
    
    void Start()
    {
        if (lightObject != null)
        {
            lightComponent = lightObject.GetComponent<Light>();
        }
    }

    public void OnFlashlight(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        flashlightState = (flashlightState + 1) % 3;

        if (lightObject != null)
        {
            switch (flashlightState)
            {
                case 0: // Off
                    lightObject.SetActive(false);
                    break;

                case 1: // Normal brightness
                    lightObject.SetActive(true);
                    if (lightComponent != null)
                    {
                        lightComponent.intensity = normalBrightness;
                        lightComponent.range = normalRange;
                    }
                    break;

                case 2: // Bright
                    lightObject.SetActive(true);
                    if (lightComponent != null)
                    {
                        lightComponent.intensity = brightBrightness;
                        lightComponent.range = brightRange;
                    }
                    break;
            }
        }
    }
}