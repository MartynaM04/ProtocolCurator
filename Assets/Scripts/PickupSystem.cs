using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;


/// Manages artifact pickup, inspection, rotation, and truth discovery mechanics.
/// Handles UI display, depth of field effects, and quest integration.

public class PickupSystem : MonoBehaviour
{
    [Header("References")]
    public Transform holdingPoint;
    public Camera playerCamera;
    public Volume globalVolume;
    public GameObject descriptionUI;
    public PlayerMovement playerMovement; 

    [Header("Ending Sequence")]
    public FinaleUIEffect finaleUI; 
    public DoorController exitDoor;
    
    [Space(10)]
    [Header("Text & UI References")]
    public TextMeshProUGUI titleLabel;
    public TextMeshProUGUI descriptionLabel;
    public TextMeshProUGUI headerTitle; 
    public TextMeshProUGUI statusLabel; 
    public Image statusIndicator; 

    [Header("Discovery Audio")]
    public AudioSource glitchAudioSource; 
    public AudioClip glitchSound;        

    [Header("Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;
    public LayerMask obstacleLayers;
    public float rotationSpeed = 20f;
    public float lerpSpeed = 10f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoomDist = 0.6f;
    public float maxZoomDist = 2.5f;
    private float targetZoomDist;
    private float defaultZoomDist;

    [Header("Collision Prevention")]
    public float artifactRadius = 0.05f;

    [Header("Depth of Field Settings")]
    public float focusOnExamine = 1f;
    public float focusNormal = 10f;
    public float dofTransitionSpeed = 17f;

    [Header("Narrative Colors")]
    public Color propagandaColor = Color.red;
    public Color truthColor = Color.cyan;

    [Header("UI Polish")]
    public CanvasGroup uiCanvasGroup; 
    public float fadeSpeed = 8f;

    [Header("Discovery Settings")]
    public float rotationThreshold = 30f; 
    public int glitchIterations = 40; 
    public float glitchSpeed = 0.05f;
    public float cameraShakeIntensity = 0.004f;

    [Header("UI Interaction")]
    public TextMeshProUGUI interactionPrompt;

    [Header("UX Feedback")]
    public GameObject contextualCrosshair;
    public AudioSource uiAudioSource;      
    public AudioClip hoverSound;          

    private bool isHovering = false; 
    [HideInInspector] public bool isInspecting = false;
    private bool isShowingTruth = false;
    private int originalLayer;

    private GameObject heldObject;
    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector2 rotateInput;
    private Vector2 zoomInput;
    private DepthOfField dofComponent;
    
    private bool currentArtifactIsDecoded = false;

    public GameObject GetHeldObject() 
    { 
        return heldObject; 
    }

    public ArtifactItem GetHeldArtifact() 
    {
        return heldObject != null ? heldObject.GetComponentInParent<ArtifactItem>() : null;
    }

    public void ForceSwapObject(ArtifactItem newObject)
    {
        if (newObject == null) 
        {
            heldObject = null;
            isInspecting = false;
            if (descriptionUI != null) descriptionUI.SetActive(false);
            if (playerMovement != null) 
            {
                playerMovement.canMove = true;
                playerMovement.canLook = true;
            }
            return;
        }

        heldObject = newObject.gameObject;
        heldObject.transform.SetParent(holdingPoint);
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
        heldObject.layer = LayerMask.NameToLayer("HeldObject");
        isInspecting = true;

        if (newObject.isNote)
        {
            if (descriptionUI != null) descriptionUI.SetActive(false);
        }
        else
        {
            if (descriptionUI != null) descriptionUI.SetActive(true);
            DisplayPropaganda(newObject.data);
        }
    }

    void Start()
    {
        if (globalVolume.profile.TryGet(out DepthOfField tmpDof)) dofComponent = tmpDof;
        if (playerMovement == null) playerMovement = GetComponent<PlayerMovement>();

        defaultZoomDist = holdingPoint.localPosition.z;
        targetZoomDist = defaultZoomDist;

        if (uiCanvasGroup != null) uiCanvasGroup.alpha = 0;
    }

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (heldObject == null)
        {
            TryPickUp();
        }
        else
        {
            DropObject();
        }
    }

    public void OnLook(InputValue value) => rotateInput = value.Get<Vector2>();
    public void OnZoom(InputValue value) => zoomInput = value.Get<Vector2>();

    void Update()
    {
        HandleDepthOfField();
        HandleUIFade(); 
        HandleInteractionPrompt();

        if (isInspecting && heldObject != null)
        {
            // Allow player to look/rotate while inspecting
            playerMovement.canLook = true;
            RotateObject();
            HandleZoom();
            UpdateHeldObjectPosition();
        }
    }

    public void ResetPickupSystem() 
    {
        isInspecting = false;
        heldObject = null;
        currentArtifactIsDecoded = false;
        
        if (playerMovement != null) 
        {
            playerMovement.canMove = true;
            playerMovement.canLook = true;
        }
    }

    void HandleInteractionPrompt()
    {
        if (interactionPrompt == null) return;

        if (isInspecting && heldObject != null)
        {
            interactionPrompt.text = "PRESS [E] TO RELEASE";
            interactionPrompt.gameObject.SetActive(true);
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (!Physics.Linecast(playerCamera.transform.position, hit.point, obstacleLayers))
            {
                ArtifactItem item = hit.collider.GetComponentInParent<ArtifactItem>();
                
                if (item != null && item.enabled && !item.isNote)
                {
                    interactionPrompt.text = "PRESS [E] TO EXAMINE";
                    interactionPrompt.gameObject.SetActive(true);

                    if (!isHovering)
                    {
                        isHovering = true;
                        if (uiAudioSource != null && hoverSound != null)
                        {
                            uiAudioSource.PlayOneShot(hoverSound);
                        }
                    }
                    return;
                }
            }
        }

        if (isHovering)
        {
            isHovering = false;
        }
        HideInteractionPrompt();
    }

    void TryPickUp()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayer))
        {
            if (Physics.Linecast(playerCamera.transform.position, hit.point, obstacleLayers))
            {
                return;
            }

            ArtifactItem item = hit.collider.GetComponentInParent<ArtifactItem>();
            if (item == null || !item.enabled || item.isNote) return;

            heldObject = hit.collider.gameObject;
            originalLayer = heldObject.layer;
            originalPos = heldObject.transform.position;
            originalRot = heldObject.transform.rotation;

            // Check if artifact already decoded
            currentArtifactIsDecoded = false;
            if (QuestManager.Instance != null && item.data != null)
            {
                currentArtifactIsDecoded = QuestManager.Instance.IsArtifactSolved(item.data.trueName);
            }

            PrepareHeldObject(heldObject);
            
            if (descriptionUI != null) descriptionUI.SetActive(true);
            
            // Display truth if already solved, otherwise show propaganda
            if (currentArtifactIsDecoded)
            {
                DisplayTruth(item.data);
            }
            else
            {
                DisplayPropaganda(item.data);
            }

            isInspecting = true;
            targetZoomDist = defaultZoomDist;
        
            if (playerMovement != null) playerMovement.canMove = false;
        }
    }

    void PrepareHeldObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = obj.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Disable levitation when picking up artifact
        ArtifactLevitation levitation = obj.GetComponent<ArtifactLevitation>();
        if (levitation != null)
        {
            levitation.isBeingHeld = true;
        }

        obj.layer = LayerMask.NameToLayer("HeldObject");
        obj.transform.SetParent(holdingPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    void RotateObject()
    {
        if (heldObject == null) return;

        float horizontal = rotateInput.x * rotationSpeed;
        float vertical = -rotateInput.y * rotationSpeed;
        
        heldObject.transform.Rotate(playerCamera.transform.up, horizontal, Space.World);
        heldObject.transform.Rotate(playerCamera.transform.right, vertical, Space.World);

        // Check if truth should be revealed
        CheckForTruth();
    }

    void DropObject()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        heldObject.layer = originalLayer;

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Collider col = heldObject.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        ArtifactLevitation levitation = heldObject.GetComponent<ArtifactLevitation>();
        if (levitation != null)
        {
            levitation.ResetToPedestal();
        }
        else
        {
            heldObject.transform.position = originalPos;
            heldObject.transform.rotation = originalRot;
        }

        heldObject = null;
        isInspecting = false;
        isShowingTruth = false;
        currentArtifactIsDecoded = false;

        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (playerMovement != null) 
        {
            playerMovement.canMove = true;
            playerMovement.canLook = true;
        }
    }

    void DisplayPropaganda(ArtifactData data)
    {
        if (data == null) return;
        
        titleLabel.text = data.officialName;
        titleLabel.color = propagandaColor;
        
        descriptionLabel.text = data.propagandaDescription;
        descriptionLabel.color = Color.white;
        
        headerTitle.text = "OFFICIAL DESIGNATION";
        headerTitle.color = propagandaColor;
        
        statusLabel.text = data.statusText;
        statusLabel.color = propagandaColor;
        
        statusIndicator.color = propagandaColor;
    }

    void DisplayTruth(ArtifactData data)
    {
        if (data == null) return;
        
        titleLabel.text = data.trueName;
        titleLabel.color = Color.white;
        
        descriptionLabel.text = data.hiddenTruthDescription;
        descriptionLabel.color = Color.white;
        
        headerTitle.text = "RECOVERED MEMORY";
        headerTitle.color = truthColor;
        
        statusLabel.text = "STATUS: UNLOCKED";
        statusLabel.color = truthColor;
        
        statusIndicator.color = truthColor;
        
        isShowingTruth = true;
    }

    void CheckForTruth()
    {
        // Don't check if already showing truth or artifact already decoded
        if (heldObject == null || isShowingTruth || currentArtifactIsDecoded) return;

        ArtifactItem item = heldObject.GetComponent<ArtifactItem>();
        if (item == null || item.data == null) return;

        Vector3 currentRotation = heldObject.transform.localEulerAngles;
        Vector3 targetRotation = item.data.revealRotation;

        float angleDifference = Quaternion.Angle(
            Quaternion.Euler(currentRotation),
            Quaternion.Euler(targetRotation)
        );

        if (angleDifference <= rotationThreshold)
        {
            RevealTruth(item.data);
        }
    }

    void RevealTruth(ArtifactData data)
    {
        isShowingTruth = true;
        currentArtifactIsDecoded = true;
        StartCoroutine(GlitchEffect(data.trueName, data.hiddenTruthDescription));
        
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ArtifactSolved(data.trueName);
        }
    }

    System.Collections.IEnumerator TriggerFinale()
    {
        yield return new WaitForSeconds(2f);

        if (finaleUI != null) 
        {
            finaleUI.TriggerEffect(truthColor);
        }

        yield return new WaitForSeconds(7f);

        if (exitDoor != null)
        {
            exitDoor.OpenDoor();
        }

        LightFinaleEffect[] lights = FindObjectsByType<LightFinaleEffect>(FindObjectsSortMode.None);
        foreach (var light in lights)
        {
            light.StartShutdown(5f);
        }
    }

    void HandleDepthOfField()
    {
        if (dofComponent == null) return;

        float target = isInspecting ? focusOnExamine : focusNormal;
        dofComponent.focusDistance.value = Mathf.Lerp(
            dofComponent.focusDistance.value, 
            target, 
            Time.deltaTime * dofTransitionSpeed
        );
    }

    void HandleUIFade()
    {
        if (uiCanvasGroup == null) return;

        float target = isInspecting ? 1f : 0f;
        uiCanvasGroup.alpha = Mathf.Lerp(uiCanvasGroup.alpha, target, Time.deltaTime * fadeSpeed);
    }

    void HandleZoom()
    {
        float scrollDelta = zoomInput.y;
        if (Mathf.Abs(scrollDelta) > 0.0001f)
        {
            targetZoomDist -= scrollDelta * zoomSpeed * Time.deltaTime;
            targetZoomDist = Mathf.Clamp(targetZoomDist, minZoomDist, maxZoomDist);
        }
        
        holdingPoint.localPosition = new Vector3(0, 0, targetZoomDist);
    }

    void UpdateHeldObjectPosition()
    {
        if (heldObject == null) return;
        
        if (heldObject.transform.parent != holdingPoint)
        {
            return;
        }

        bool isHoldingMuseumArtifact = IsHoldingMuseumArtifact();
        
        if (!isHoldingMuseumArtifact)
        {
            heldObject.transform.position = Vector3.Lerp(
                heldObject.transform.position, 
                holdingPoint.position, 
                Time.deltaTime * lerpSpeed
            );
        }
    }

    private System.Collections.IEnumerator GlitchEffect(string targetTitle, string targetDesc)
    {
        if (glitchAudioSource != null && glitchSound != null) 
            glitchAudioSource.PlayOneShot(glitchSound);

        Vector3 titleOrigPos = titleLabel.rectTransform.localPosition;
        Vector3 descOrigPos = descriptionLabel.rectTransform.localPosition;
        Vector3 headerOrigPos = headerTitle.rectTransform.localPosition;
        Vector3 camOrigPos = playerCamera.transform.localPosition;

        string chars = "!@#$%^&*()_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        for (int i = 0; i < glitchIterations; i++)
        {
            float jitter = 6f;
            Vector3 uiOffset = new Vector3(Random.Range(-jitter, jitter), Random.Range(-jitter, jitter), 0);
            
            titleLabel.text = "DECRYPTING_" + chars[Random.Range(0, chars.Length)];
            titleLabel.rectTransform.localPosition = titleOrigPos + uiOffset;
            titleLabel.color = (Random.value > 0.5f) ? Color.white : truthColor;
            
            descriptionLabel.rectTransform.localPosition = descOrigPos + uiOffset * 0.5f;
            descriptionLabel.color = (Random.value > 0.7f) ? Color.black : Color.white;
            
            headerTitle.rectTransform.localPosition = headerOrigPos + uiOffset * 0.8f;
            
            Vector3 camOffset = new Vector3(
                Random.Range(-cameraShakeIntensity, cameraShakeIntensity), 
                Random.Range(-cameraShakeIntensity, cameraShakeIntensity), 
                0
            );
            playerCamera.transform.localPosition = camOrigPos + camOffset;
            
            yield return new WaitForSecondsRealtime(glitchSpeed);
        }

        // Reset positions and display truth
        titleLabel.rectTransform.localPosition = titleOrigPos;
        descriptionLabel.rectTransform.localPosition = descOrigPos;
        headerTitle.rectTransform.localPosition = headerOrigPos;
        playerCamera.transform.localPosition = camOrigPos;
        
        titleLabel.text = targetTitle;
        titleLabel.color = Color.white;
        
        descriptionLabel.text = targetDesc;
        descriptionLabel.color = Color.white;
        
        headerTitle.text = "RECOVERED MEMORY";
        headerTitle.color = truthColor;
        
        statusLabel.text = "STATUS: UNLOCKED";
        statusLabel.color = truthColor;
        
        statusIndicator.color = truthColor;
    }

    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
            interactionPrompt.color = Color.white;
        }
    }

    private bool IsHoldingMuseumArtifact()
    {
        if (heldObject == null) return false;
        
        ArtifactItem item = heldObject.GetComponentInParent<ArtifactItem>();
        if (item == null || item.data == null) return false;
        
        string nameCheck = item.data.officialName.Trim().ToLower();
        return nameCheck != "rusted syringe";
    }
}