using UnityEngine;
using System.Collections;

/// Creates dramatic lighting effects when player enters propaganda zones
/// Features aggressive flickering followed by unstable power restoration

public class PropagandaEffect : MonoBehaviour
{
    [Header("Group 1: Normal Lights (Banners)")]
    public Light[] normalLights;
    public float normalBaseIntensity = 50f;

    [Header("Group 2: High Intensity Lights")]
    public Light[] highIntensityLights;
    public float highBaseIntensity = 600f;

    [Header("References")]
    public AudioSource voiceSource;

    [Header("Initial Flicker Settings")]
    public int initialFlickerCount = 3;
    public float minFlickerDelay = 0.05f;
    public float maxFlickerDelay = 0.15f;
    public float flashIntensity = 1000f;

    [Header("Delay Settings")]
    public float darkPeriodDuration = 2.0f;

    [Header("Unstable Power Effect")]
    [Range(0f, 1f)]
    public float jitterAmount = 0.15f;
    public float jitterSpeed = 0.1f;
    public float jitterDuration = 5.0f;

    [Header("Logic")]
    public bool playOnlyOnce = false;
    public float cooldownTime = 5.0f;

    private bool isProcessing = false;
    private bool alreadyPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !gameObject.activeInHierarchy)
            return;

        // Skip if quest is complete
        if (QuestManager.Instance != null && QuestManager.Instance.IsQuestComplete())
            return;

        if (playOnlyOnce && alreadyPlayed)
            return;

        if (isProcessing)
            return;

        StartCoroutine(FullSequence());
    }

    private IEnumerator FullSequence()
    {
        isProcessing = true;
        alreadyPlayed = true;

        if (voiceSource != null && !voiceSource.isPlaying)
        {
            voiceSource.Play();
        }

        // Phase 1: Aggressive flickering
        for (int i = 0; i < initialFlickerCount; i++)
        {
            SetAllIntensity(flashIntensity);
            yield return new WaitForSecondsRealtime(Random.Range(minFlickerDelay, maxFlickerDelay));
            SetAllIntensity(0f);
            yield return new WaitForSecondsRealtime(Random.Range(minFlickerDelay, maxFlickerDelay));
        }

        // Phase 2: Dark period
        SetAllIntensity(0f);
        yield return new WaitForSecondsRealtime(darkPeriodDuration);

        // Phase 3: Unstable power restoration
        float timer = 0;
        while (timer < jitterDuration)
        {
            if (!gameObject.activeInHierarchy)
                yield break;

            float normalJitter = normalBaseIntensity * Random.Range(1f - jitterAmount, 1f + jitterAmount);
            SetGroupIntensity(normalLights, normalJitter);

            float highJitter = highBaseIntensity * Random.Range(1f - jitterAmount, 1f + jitterAmount);
            SetGroupIntensity(highIntensityLights, highJitter);

            timer += jitterSpeed;
            yield return new WaitForSecondsRealtime(jitterSpeed);
        }

        // Phase 4: Stabilize to final intensity
        SetGroupIntensity(normalLights, normalBaseIntensity);
        SetGroupIntensity(highIntensityLights, highBaseIntensity);

        yield return new WaitForSecondsRealtime(cooldownTime);
        isProcessing = false;
    }

    private void SetAllIntensity(float value)
    {
        SetGroupIntensity(normalLights, value);
        SetGroupIntensity(highIntensityLights, value);
    }

    private void SetGroupIntensity(Light[] lights, float value)
    {
        if (lights == null) return;
        
        foreach (Light light in lights)
        {
            if (light != null)
            {
                light.intensity = value;
            }
        }
    }
}