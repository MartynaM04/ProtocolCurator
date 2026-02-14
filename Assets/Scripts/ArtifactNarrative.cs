using UnityEngine;
using TMPro;
using System.Collections;

/// Plays narrative voiceover with synced subtitles when player discovers artifact

public class ArtifactNarrative : MonoBehaviour
{
    [Header("Quest Integration")]
    public string artifactQuestName;

    [Header("References")]
    public AudioSource playerVoiceSource;
    public AudioClip memoryClip;
    public TextMeshProUGUI subtitleUI;

    [Header("Narrative Content")]
    [TextArea(3, 10)]
    public string subtitleContent;

    [Header("Timing & Speed")]
    public float delayBeforeSpeaking = 0.5f;
    [Range(0.5f, 2.0f)]
    public float playbackSpeed = 1.0f;
    
    [Tooltip("0.8 means text finishes when 80% of audio is done")]
    [Range(0.1f, 1.0f)]
    public float textFinishPercent = 0.8f;
    
    public bool playOnlyOnce = true;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (QuestManager.Instance != null && !string.IsNullOrEmpty(artifactQuestName))
            {
                QuestManager.Instance.ArtifactSolved(artifactQuestName); // FIXED: Changed from MarkAsTruthDiscovered
            }

            StartCoroutine(PlayMemoryVoiceover());
            if (playOnlyOnce) hasTriggered = true;
        }
    }

    private IEnumerator PlayMemoryVoiceover()
    {
        yield return new WaitForSeconds(delayBeforeSpeaking);

        if (playerVoiceSource != null && memoryClip != null && subtitleUI != null)
        {
            float clipDuration = memoryClip.length / playbackSpeed;
            float typingDuration = clipDuration * textFinishPercent;
            
            subtitleUI.text = "";
            subtitleUI.gameObject.SetActive(true);
            
            playerVoiceSource.pitch = playbackSpeed;
            playerVoiceSource.PlayOneShot(memoryClip);

            yield return StartCoroutine(TypeSubtitle(subtitleContent, typingDuration));

            float remainingTime = (clipDuration - typingDuration) + 0.3f;
            yield return new WaitForSeconds(Mathf.Max(0, remainingTime));
            
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
            yield return new WaitForSeconds(charDelay);
        }
    }
}