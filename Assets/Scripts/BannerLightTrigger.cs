using UnityEngine;

/// Activates banner spotlight when player enters trigger zone

public class BannerLightTrigger : MonoBehaviour
{
    public Light bannerLight;
    public float activeIntensity = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bannerLight.intensity = activeIntensity;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bannerLight.intensity = 0f;
        }
    }
}