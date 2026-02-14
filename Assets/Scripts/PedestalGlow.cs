using UnityEngine;

/// Creates pulsing glow effect for artifact pedestals

public class PedestalGlow : MonoBehaviour
{
    [Header("Settings")]
    public MeshRenderer meshRenderer;
    
    [ColorUsage(true, true)]
    public Color glowColor = Color.red;
    
    public float pulseSpeed = 1f;
    public float minIntensity = 2f;
    public float maxIntensity = 10f;

    private Material material;
    private static readonly int EmissionColorProperty = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        if (meshRenderer != null)
        {
            material = meshRenderer.material;
            material.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (material == null) return;

        float lerpTime = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float currentIntensity = Mathf.Lerp(minIntensity, maxIntensity, lerpTime);

        Color finalColor = glowColor * currentIntensity;
        material.SetColor(EmissionColorProperty, finalColor);
    }
}