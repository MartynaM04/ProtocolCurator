using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Manages game introduction sequence with narrative text and ambient sound timing.
/// Controls fade transitions, text typing effects, and player activation.
/// </summary>
public class IntroSequenceManager : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup controlsGroup;
    public CanvasGroup narrativeGroup;
    public CanvasGroup blackBackgroundGroup;
    public GameObject gameplayHUD;

    [Header("Text Reference")]
    public TextMeshProUGUI narrativeText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip narrativeSound;
    [Range(0f, 1f)] public float narrativeVolume = 0.7f;

    [Header("Player Reference")]
    public PlayerInput playerInput;
    public PlayerMovement playerMovement;

    [Header("Timing Settings")]
    public float controlsDuration = 4f;
    [Range(0.01f, 0.2f)] public float narrativeTypingSpeed = 0.04f;
    public float waitAfterTyping = 3f;
    public float fadeSpeed = 2f;

    [Header("Ambient Sounds")]
    public AudioSource[] ambientSounds;

    private void OnEnable()
    {
        if (playerInput == null || controlsGroup == null || narrativeGroup == null || blackBackgroundGroup == null)
            return;

        playerInput.enabled = false;
        
        if (playerMovement != null) 
            playerMovement.canMove = false;
        
        if (gameplayHUD != null) 
            gameplayHUD.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Stop all ambient sounds before intro
        StopAllAmbientSounds();
        
        StopAllCoroutines();
        StartCoroutine(PlayIntro());
    }

    void StopAllAmbientSounds()
    {
        if (ambientSounds != null)
        {
            foreach (AudioSource source in ambientSounds)
            {
                if (source != null)
                {
                    source.Stop();
                }
            }
        }
    }

    private IEnumerator PlayIntro()
    {
        // Initialize canvas groups
        controlsGroup.alpha = 0;
        narrativeGroup.alpha = 0;
        blackBackgroundGroup.alpha = 1;

        // Show controls panel
        yield return StartCoroutine(FadeCanvas(controlsGroup, 0, 1));
        yield return new WaitForSeconds(controlsDuration);
        yield return StartCoroutine(FadeCanvas(controlsGroup, 1, 0));

        // Show narrative panel
        narrativeGroup.alpha = 1;
        
        if (audioSource != null && narrativeSound != null)
        {
            audioSource.clip = narrativeSound;
            audioSource.volume = narrativeVolume;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        yield return StartCoroutine(TypeText(narrativeText));
        yield return new WaitForSeconds(waitAfterTyping);
        
        if (audioSource != null) 
            audioSource.Stop();
        
        // Fade out narrative and black background
        yield return StartCoroutine(FadeCanvas(narrativeGroup, 1, 0));
        yield return StartCoroutine(FadeCanvas(blackBackgroundGroup, 1, 0));

        // Enable player and gameplay
        if (playerInput != null) 
            playerInput.enabled = true;
        
        if (playerMovement != null) 
            playerMovement.canMove = true;
        
        if (gameplayHUD != null) 
            gameplayHUD.SetActive(true);

        // Start ambient sounds after intro
        StartAmbientSounds();

        // Enable pause manager
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager != null) 
            pauseManager.enabled = true;

        gameObject.SetActive(false);
    }

    void StartAmbientSounds()
    {
        if (ambientSounds != null)
        {
            foreach (AudioSource source in ambientSounds)
            {
                if (source != null && !source.isPlaying)
                {
                    source.Play();
                }
            }
        }
    }

    private IEnumerator TypeText(TextMeshProUGUI textElement)
    {
        string fullText = textElement.text;
        textElement.text = "";
        
        foreach (char letter in fullText.ToCharArray())
        {
            textElement.text += letter;
            yield return new WaitForSeconds(narrativeTypingSpeed);
        }
    }

    private IEnumerator FadeCanvas(CanvasGroup group, float startAlpha, float endAlpha)
    {
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * fadeSpeed;
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, timer);
            yield return null;
        }
        group.alpha = endAlpha;
    }
}