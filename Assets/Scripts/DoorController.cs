using UnityEngine;
using System.Collections;

/// <summary>
/// Controls door opening animation and end game trigger activation
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 openRotation = new Vector3(0, -90, 0);
    public float openSpeed = 2f;
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip openSound;
    
    [Header("End Game Settings")]
    public EndGameTrigger endGameTrigger;
    public float delayBeforeEnablingTrigger = 1f;

    private bool isOpen = false;

    void Start()
    {
        if (endGameTrigger != null)
        {
            endGameTrigger.enabled = false;
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        if (audioSource != null && openSound != null)
        {
            audioSource.PlayOneShot(openSound);
        }

        StartCoroutine(AnimateDoor());
    }

    private IEnumerator AnimateDoor()
    {
        Quaternion targetRotation = Quaternion.Euler(openRotation);
        Quaternion startRotation = transform.localRotation;
        float timer = 0;

        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * openSpeed;
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, timer);
            yield return null;
        }
        
        if (endGameTrigger != null)
        {
            yield return new WaitForSeconds(delayBeforeEnablingTrigger);
            endGameTrigger.enabled = true;
        }
    }
}