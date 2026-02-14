using UnityEngine;
using TMPro;
using System.Collections;

/// Triggers end game sequence when player reaches escape zone
/// Features glitch effect and smooth transition to main menu

public class EndGameTrigger : MonoBehaviour
{
    [Header("References")]
    public MainMenuManager menuManager;
    public CanvasGroup fadePanel;
    public TextMeshProUGUI escapedText;
    
    [Header("Typewriter Settings")]
    public float typewriterSpeed = 0.05f;
    public float delayBeforeTyping = 1f;
    
    [Header("Timing")]
    public float fadeSpeed = 0.8f;
    public float waitAfterText = 2f;
    
    [Header("Audio")]
    public AudioSource successSound;
    public AudioClip glitchSound; // CHANGED: Now plays once at start, not per letter
    
    [Header("Text Content")]
    [TextArea(5, 10)]
    public string endingText = @"NEURAL LINK SEVERED.
MEMORY FRAGMENT 04: SECURED.

NEURAL STABILITY: CRITICAL.
RESETTING... DATA UPLOAD: 100% COMPLETE.
DE-SYNCHRONIZING...

ESCAPE SUCCESSFUL. CONTAINMENT BREACH LOGGED. STANDBY FOR NEXT UPLINK.


FINAL FRAGMENT AWAITS.";
    
    private bool hasTriggered = false;
    
    void Start()
    {
        if (menuManager == null)
        {
            menuManager = FindFirstObjectByType<MainMenuManager>();
        }
        
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.gameObject.SetActive(false);
        }
        
        if (escapedText != null)
        {
            escapedText.text = "";
            escapedText.gameObject.SetActive(false);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(EndGameSequence());
        }
    }
    
    IEnumerator EndGameSequence()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        
        if (successSound != null)
        {
            successSound.Play();
        }
        
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
        }
        
        // Fade to black
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            if (fadePanel != null)
            {
                fadePanel.alpha = timer;
            }
            yield return null;
        }
        
        // Freeze player
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
            playerMovement.canLook = false;
        }
        
        // Stop all audio
        AudioSource[] allAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource audio in allAudio)
        {
            if (audio != null && audio.isPlaying && audio != successSound)
            {
                audio.Stop();
            }
        }
        
        Time.timeScale = 0f;
        
        yield return new WaitForSecondsRealtime(delayBeforeTyping);
        
        if (escapedText != null)
        {
            escapedText.gameObject.SetActive(true);
            
            // FIXED: Play glitch sound ONCE at the start, not per letter
            if (glitchSound != null)
            {
                AudioSource.PlayClipAtPoint(glitchSound, Camera.main.transform.position, 0.5f);
            }
            
            yield return StartCoroutine(TypewriterEffect(endingText));
        }
        
        yield return new WaitForSecondsRealtime(waitAfterText);
        
        if (escapedText != null)
        {
            yield return StartCoroutine(FadeOutText());
        }
        
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
        }
        
        if (escapedText != null)
        {
            escapedText.gameObject.SetActive(false);
            escapedText.text = "";
        }
        
        Time.timeScale = 1f;
        
        if (menuManager != null)
        {
            PlayerPrefs.SetInt("GameCompleted", 1);
            PlayerPrefs.Save();
            menuManager.ResetToMenu();
        }
    }
    
    IEnumerator TypewriterEffect(string text)
    {
        escapedText.text = "";
        
        // FIXED: No sound per letter - just type smoothly
        foreach (char letter in text)
        {
            escapedText.text += letter;
            yield return new WaitForSecondsRealtime(typewriterSpeed);
        }
    }
    
    IEnumerator FadeOutText()
    {
        if (escapedText == null) yield break;
        
        Color originalColor = escapedText.color;
        float timer = 0f;
        float fadeDuration = 1f;
        
        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime / fadeDuration;
            
            Color newColor = originalColor;
            newColor.a = Mathf.Lerp(1f, 0f, timer);
            escapedText.color = newColor;
            
            yield return null;
        }
        
        escapedText.color = originalColor;
    }
}