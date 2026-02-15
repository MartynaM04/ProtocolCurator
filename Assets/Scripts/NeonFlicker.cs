using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// Creates flickering neon effect

public class NeonFlicker : MonoBehaviour
{
    [Header("Target Components")]

    public List<Shadow> neonComponents = new List<Shadow>();

    [Header("Flicker Settings")]
    [Range(0f, 1f)] public float minAlpha = 0.2f;
    [Range(0f, 1f)] public float maxAlpha = 1f;
    public float flickerSpeed = 0.07f;

    private Color currentColor;

    void Start()
    {
        // Automatically find all Outline and Shadow components if list is empty
        if (neonComponents.Count == 0)
        {
            neonComponents.AddRange(GetComponents<Shadow>());
        }

        if (neonComponents.Count > 0) currentColor = neonComponents[0].effectColor;
        
        StartCoroutine(FlickerRoutine());
    }

    public void SwitchToTruthColor(Color cyanColor)
    {
        currentColor = cyanColor;
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float randomAlpha = Random.Range(minAlpha, maxAlpha);
            
            foreach (var comp in neonComponents)
            {
                if (comp != null)
                {
                    Color updatedColor = currentColor;
                    updatedColor.a = randomAlpha;
                    comp.effectColor = updatedColor;
                }
            }

            yield return new WaitForSeconds(Random.Range(flickerSpeed, flickerSpeed * 2.5f));
        }
    }
}
