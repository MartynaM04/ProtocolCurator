using UnityEngine;
using TMPro;
using System.Collections;

/// Creates dramatic glitch effect when all artifacts are decoded

[RequireComponent(typeof(CanvasGroup))]
public class FinaleUIEffect : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI subText;
    public RectTransform mainContainer;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip glitchSound;

    [Header("Timing Settings")]
    public float glitchDuration = 7f;
    public float displayDuration = 4f;
    public float fadeSpeed = 1.5f;

    [Header("Visual Settings")]
    public float shakeIntensity = 12f;
    public string finalMainString = "SYSTEM OVERRIDE: MEMORY RECOVERED";
    public string finalSubString = "Find Terminal and enter password";
    
    private string glitchChars = "!@#$%^&*()_0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ<>/[]";
    private Vector3 originalPosition;

    public void TriggerEffect(Color truthColor)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        
        if (mainText != null) mainText.color = Color.white;
        if (subText != null)
        {
            subText.color = truthColor;
            subText.alpha = 0;
        }
        
        originalPosition = mainContainer.localPosition;

        if (audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        canvasGroup.alpha = 0;
        float timer = 0;

        while (timer < glitchDuration)
        {
            timer += Time.deltaTime;
            
            if (canvasGroup.alpha < 1)
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;

            mainContainer.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
            mainText.text = GenerateGlitchString(finalMainString.Length);
            
            yield return null;
        }

        mainContainer.localPosition = originalPosition;
        mainText.text = finalMainString;
        subText.text = finalSubString;
        
        float subFadeTimer = 0;
        while (subFadeTimer < 0.4f)
        {
            subFadeTimer += Time.deltaTime;
            subText.alpha = subFadeTimer / 0.4f;
            yield return null;
        }
        subText.alpha = 1;

        yield return new WaitForSeconds(displayDuration);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            mainContainer.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * (shakeIntensity * 0.3f);
            yield return null;
        }

        mainContainer.localPosition = originalPosition;
        gameObject.SetActive(false);
    }

    private string GenerateGlitchString(int length)
    {
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = glitchChars[Random.Range(0, glitchChars.Length)];
        }
        return new string(result);
    }
}