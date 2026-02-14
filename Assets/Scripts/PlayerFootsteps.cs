using UnityEngine;

// Controls player footsteeps

public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    public AudioSource footstepSource;
    public CharacterController characterController;
    public PlayerMovement playerMovement; 

    [Header("Audio Settings")]
    public AudioClip[] footstepClips;
    public float walkStepInterval = 0.4f;
    public float sprintStepInterval = 0.25f; 
    public float walkVelocityThreshold = 0.5f;

    [Header("Character Feel")]

    [Range(0.1f, 1.0f)]
    public float footstepVolume = 1.0f; 
    
    [Range(0.8f, 1.5f)]
    public float basePitch = 1.15f;
    public float pitchRandomness = 0.05f;

    [Header("Detection Settings")]
    public float rayDistance = 1.6f; 

    private float stepTimer;

    void Update()
    {
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        if (characterController.isGrounded && currentSpeed > walkVelocityThreshold)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0)
            {
                PlayFootstepSound();
                bool isSprinting = playerMovement != null && playerMovement.isSprinting;
                stepTimer = isSprinting ? sprintStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    void PlayFootstepSound()
    {
        if (footstepClips.Length == 0 || footstepSource == null) return;

        int index = Random.Range(0, footstepClips.Length);
        
        footstepSource.pitch = basePitch + Random.Range(-pitchRandomness, pitchRandomness);
        footstepSource.volume = footstepVolume;

        footstepSource.PlayOneShot(footstepClips[index]);
    }
}