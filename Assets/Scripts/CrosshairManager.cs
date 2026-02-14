using UnityEngine;
using UnityEngine.UI;

/// Manages crosshair appearance when hovering over interactable objects

public class CrosshairManager : MonoBehaviour
{
    public Camera playerCamera;
    public Image crosshairImage;
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    public Color defaultColor = Color.white;
    public Color interactColor = Color.cyan;

    void Update()
    {
        CheckForInteractable();
    }

    void CheckForInteractable()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            crosshairImage.color = interactColor;
            crosshairImage.transform.localScale = Vector3.Lerp(
                crosshairImage.transform.localScale,
                new Vector3(1.2f, 1.2f, 1.2f),
                Time.deltaTime * 10f
            );
        }
        else
        {
            crosshairImage.color = defaultColor;
            crosshairImage.transform.localScale = Vector3.Lerp(
                crosshairImage.transform.localScale,
                Vector3.one,
                Time.deltaTime * 10f
            );
        }
    }
}