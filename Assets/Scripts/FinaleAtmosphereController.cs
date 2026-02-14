using UnityEngine;

/// <summary>
/// Controls the sci-fi atmosphere for the finale scene at the top of the tower
/// Manages lighting, fog, and environmental effects
/// </summary>
public class FinaleAtmosphereController : MonoBehaviour
{
    [Header("Skybox Settings")]
    [Tooltip("Main skybox material for the scene")]
    public Material skyboxMaterial;
    
    [Header("Fog Settings - NIGHT + CLOUDS")]
    [Tooltip("Enable fog effect")]
    public bool enableFog = true;
    
    [Tooltip("Fog color - dark blue/black for night above clouds")]
    [ColorUsage(false)]
    public Color fogColor = new Color(0.05f, 0.1f, 0.15f, 1f); // Very dark blue
    
    [Tooltip("Fog density - higher for mysterious atmosphere")]
    [Range(0f, 0.1f)]
    public float fogDensity = 0.015f;
    
    [Header("Ambient Light - NIGHT SCENE")]
    [Tooltip("Sky color for ambient lighting - dark blue for night")]
    [ColorUsage(false)]
    public Color ambientSkyColor = new Color(0.05f, 0.1f, 0.2f, 1f); // Dark blue
    
    [Tooltip("Equator color for ambient lighting")]
    [ColorUsage(false)]
    public Color ambientEquatorColor = new Color(0.02f, 0.05f, 0.1f, 1f); // Darker blue
    
    [Tooltip("Ground color for ambient lighting")]
    [ColorUsage(false)]
    public Color ambientGroundColor = new Color(0.01f, 0.02f, 0.05f, 1f); // Almost black
    
    [Tooltip("Intensity of ambient light - low for night")]
    [Range(0f, 2f)]
    public float ambientIntensity = 0.3f;
    
    [Header("Directional Light (Moon Light)")]
    [Tooltip("Reference to the main directional light (acting as moonlight)")]
    public Light mainDirectionalLight;
    
    [Tooltip("Color of the moon light - pale blue")]
    [ColorUsage(false)]
    public Color mainLightColor = new Color(0.7f, 0.8f, 1f, 1f); // Pale blue
    
    [Tooltip("Intensity of moon light - very low")]
    [Range(0f, 3f)]
    public float mainLightIntensity = 0.3f;
    
    [Header("Cloud Layer Animation")]
    [Tooltip("Enable rotating clouds outside windows")]
    public bool animateClouds = true;
    
    [Tooltip("Speed of cloud rotation")]
    public float cloudRotationSpeed = 0.5f;
    
    private Transform cloudLayer;

    void Start()
    {
        SetupAtmosphere();
        FindCloudLayer();
    }

    void Update()
    {
        if (animateClouds && cloudLayer != null)
        {
            // Slowly rotate clouds around the tower
            cloudLayer.Rotate(Vector3.up, cloudRotationSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Applies all atmosphere settings to the scene
    /// </summary>
    void SetupAtmosphere()
    {
        // Apply skybox
        if (skyboxMaterial != null)
        {
            RenderSettings.skybox = skyboxMaterial;
        }

        // Setup fog
        RenderSettings.fog = enableFog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = fogDensity;

        // Setup ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = ambientSkyColor;
        RenderSettings.ambientEquatorColor = ambientEquatorColor;
        RenderSettings.ambientGroundColor = ambientGroundColor;
        RenderSettings.ambientIntensity = ambientIntensity;

        // Setup main directional light
        if (mainDirectionalLight != null)
        {
            mainDirectionalLight.color = mainLightColor;
            mainDirectionalLight.intensity = mainLightIntensity;
            mainDirectionalLight.shadows = LightShadows.Soft;
        }
    }

    /// <summary>
    /// Finds the cloud layer object if it exists
    /// </summary>
    void FindCloudLayer()
    {
        GameObject cloudObj = GameObject.Find("CloudLayer");
        if (cloudObj != null)
        {
            cloudLayer = cloudObj.transform;
        }
    }

    /// <summary>
    /// Updates fog intensity - useful for dramatic moments
    /// </summary>
    public void SetFogIntensity(float density)
    {
        fogDensity = Mathf.Clamp(density, 0f, 0.1f);
        RenderSettings.fogDensity = fogDensity;
    }

    /// <summary>
    /// Changes the fog color - useful for dramatic transitions
    /// </summary>
    public void SetFogColor(Color newColor)
    {
        fogColor = newColor;
        RenderSettings.fogColor = fogColor;
    }
}