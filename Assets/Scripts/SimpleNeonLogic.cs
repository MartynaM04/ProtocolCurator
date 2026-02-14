using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SimpleNeonLogic : MonoBehaviour
{
    [Header("UI Elements to Control")]
    public List<Graphic> neonParts = new List<Graphic>(); // Image and Text
    private List<Shadow> neonShadows = new List<Shadow>(); // For the glow effect

    [Header("Colors")]
    public Color propagandaColor = Color.red;
    public Color truthColor = Color.cyan;

    [Header("Animation Settings")]
    public float flickerSpeed = 8f;
    private bool isFlickering = true;

    void Awake()
    {
        // Automatically find all Shadow components on this object and children
        // This ensures the glow changes color too!
        neonShadows.AddRange(GetComponentsInChildren<Shadow>());
    }

    void Update()
    {
        if (isFlickering)
        {
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            float alpha = Mathf.Lerp(0.3f, 1f, noise);
            
            UpdateNeonAlpha(alpha);
        }
    }

    public void SwitchToTruth()
    {
        isFlickering = false; 
        
        // 1. Update main graphics (Image/Text)
        foreach (Graphic g in neonParts)
        {
            if (g != null)
            {
                Color c = truthColor;
                c.a = 1f; 
                g.color = c;
            }
        }

        // 2. Update all Shadow components (The Glow)
        foreach (Shadow s in neonShadows)
        {
            if (s != null)
            {
                Color c = truthColor;
                c.a = 1f;
                s.effectColor = c;
            }
        }
    }

    private void UpdateNeonAlpha(float a)
    {
        // Sync alpha for Graphics
        foreach (Graphic g in neonParts)
        {
            if (g != null)
            {
                Color c = g.color;
                c.a = a;
                g.color = c;
            }
        }

        // Sync alpha for Shadows
        foreach (Shadow s in neonShadows)
        {
            if (s != null)
            {
                Color c = s.effectColor;
                c.a = a;
                s.effectColor = c;
            }
        }
    }
}