using UnityEngine;
using System.Collections;

/// Creates atmospheric flickering effect for lights

public class FlickeringLight : MonoBehaviour
{
    public Light lightSource;
    public float minDelay = 0.01f;
    public float maxDelay = 0.2f;

    void Start()
    {
        if (lightSource == null)
            lightSource = GetComponentInChildren<Light>();
        
        if (lightSource != null)
            StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            if (lightSource != null) lightSource.enabled = !lightSource.enabled;
            
            yield return new WaitForSeconds(Random.Range(0.01f, 0.05f));
            if (lightSource != null) lightSource.enabled = true;
            
            yield return new WaitForSeconds(Random.Range(2f, 10f));
        }
    }
}