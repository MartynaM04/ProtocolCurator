using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Settings")]
    public float walkingBobSpeed = 14f;
    public float bobAmount = 0.05f; // Keep this low to avoid motion sickness
    public CharacterController playerController;

    private float defaultPosY = 0;
    private float timer = 0;

    void Start()
    {
        // Save the starting local Y position of the camera
        defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        // Check if the player is moving on the ground
        if (playerController.isGrounded && playerController.velocity.magnitude > 0.1f)
        {
            // Calculate the bobbing movement using Sine wave
            timer += Time.deltaTime * walkingBobSpeed;
            float newY = defaultPosY + Mathf.Sin(timer) * bobAmount;
            
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                newY,
                transform.localPosition.z
            );
        }
        else
        {
            // Reset camera position smoothly when idle
            timer = 0;
            Vector3 targetPos = new Vector3(transform.localPosition.x, defaultPosY, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition.y < defaultPosY ? transform.localPosition : transform.localPosition, targetPos, Time.deltaTime * 5f);
            
            // Fixed reset for simplicity
            if(transform.localPosition.y != defaultPosY && playerController.velocity.magnitude <= 0.1f)
            {
                 transform.localPosition = new Vector3(transform.localPosition.x, Mathf.MoveTowards(transform.localPosition.y, defaultPosY, Time.deltaTime * 0.2f), transform.localPosition.z);
            }
        }
    }
}