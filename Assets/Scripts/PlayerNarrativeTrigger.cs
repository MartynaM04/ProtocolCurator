using UnityEngine;
using TMPro;
using System.Collections;

// Handles player narrative voice overs

public class PlayerNarrativeTrigger : MonoBehaviour
{
    [Header("References")]
    public AudioSource playerVoiceSource; 
    public AudioClip monologueClip;       
    public TextMeshProUGUI subtitleUI; 

    [Header("Narrative Content")]
    [TextArea(3, 10)]
    public string subtitleContent;

    [Header("Timing & Speed")]
    public float delayBeforeSpeaking = 1.0f; 
    [Range(0.5f, 2.0f)]
    public float playbackSpeed = 1.0f; 
    
    [Range(0.1f, 1.0f)]
    public float textFinishPercent = 0.8f; 

    [Tooltip("How long to wait after the VOICE ends before hiding text.")]
    public float postAudioDelay = 0.3f; 

    [Header("Logic")]
    public bool triggerOnce = true;
    private bool hasSpoken = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasSpoken)
        {
            if (QuestManager.Instance != null && QuestManager.Instance.IsQuestComplete()) 
                return;
            
            if (subtitleUI != null)
            {
                StopAllCoroutines();
                StartCoroutine(PlayMonologue());
                if (triggerOnce) hasSpoken = true;
            }
            else
            {
                Debug.LogWarning($"NarrativeTrigger on {gameObject.name}: Subtitle UI is not assigned!");
            }
        }
    }

    private IEnumerator PlayMonologue()
    {
        yield return new WaitForSecondsRealtime(delayBeforeSpeaking);

        if (playerVoiceSource != null && monologueClip != null && subtitleUI != null)
        {
            float clipDuration = monologueClip.length / playbackSpeed;
            float typingDuration = clipDuration * textFinishPercent;

            // Make sure UI is active and reset text [cite: 2026-01-21]
            subtitleUI.gameObject.SetActive(true);
            subtitleUI.text = "";

            playerVoiceSource.pitch = playbackSpeed;
            playerVoiceSource.PlayOneShot(monologueClip);

            yield return StartCoroutine(TypeSubtitle(subtitleContent, typingDuration));

            float timeToWait = (clipDuration - typingDuration) + postAudioDelay;
            yield return new WaitForSecondsRealtime(Mathf.Max(0, timeToWait));
            
            subtitleUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator TypeSubtitle(string text, float duration)
    {
        if (text.Length == 0) yield break;
        float charDelay = duration / text.Length;
        subtitleUI.text = "";
        foreach (char letter in text.ToCharArray())
        {
            subtitleUI.text += letter;
            yield return new WaitForSecondsRealtime(charDelay);
        }
    }
}