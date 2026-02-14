using UnityEngine;

/// Creates gentle swaying motion for hanging banners

public class BannerSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayIntensity = 0.5f;
    public float swaySpeed = 1f;

    private Quaternion startRotation;

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        float swayAngle = Mathf.Sin(Time.time * swaySpeed) * swayIntensity;
        transform.rotation = startRotation * Quaternion.Euler(swayAngle, 0, 0);
    }
}