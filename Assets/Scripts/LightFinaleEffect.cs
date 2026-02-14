using UnityEngine;
using System.Collections;

/// Creates dramatic light shutdown effect during finale sequence

public class LightFinaleEffect : MonoBehaviour
{
    [Header("Auto-Assigned")]
    public Light lightSource;

    void Awake()
    {
        if (lightSource == null)
            lightSource = GetComponentInChildren<Light>();
    }

    public void StartShutdown(float duration)
    {
        FlickeringLight ambientFlicker = GetComponent<FlickeringLight>();
        if (ambientFlicker != null)
        {
            ambientFlicker.StopAllCoroutines();
            ambientFlicker.enabled = false;
        }

        StartCoroutine(ShutdownRoutine(duration));
    }

    private IEnumerator ShutdownRoutine(float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float step = Random.Range(0.01f, 0.08f);
            if (lightSource != null) lightSource.enabled = !lightSource.enabled;
            yield return new WaitForSeconds(step);
            elapsed += step;
        }

        if (lightSource != null)
        {
            lightSource.enabled = false;
        }
    }
}