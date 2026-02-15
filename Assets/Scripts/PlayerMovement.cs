using UnityEngine;
using UnityEngine.InputSystem;

/// Handles player movement, sprinting, jumping, and camera look controls

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public CharacterController controller;

    [Header("Physics")]
    public float gravity = -15f;
    public float jumpForce = 4f;
    private Vector3 velocity;
    private bool isGrounded;

    [Header("Look Settings")]
    public float mouseSensitivity = 1f;
    public Transform playerCamera;

    [HideInInspector] public bool canMove = true;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool canLook = true;
    
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float xRotation = 0f;

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnLook(InputValue value) => lookInput = value.Get<Vector2>();
    
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded && canMove)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void OnSprint(InputValue value)
    {
        if (value.isPressed)
        {
            isSprinting = !isSprinting;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!canMove) return;

        // Stop sprinting when not moving
        if (moveInput.magnitude == 0)
        {
            isSprinting = false;
        }

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Determine movement speed
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        

        // Calculate movement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        Vector3 horizontalMove = move * currentSpeed;

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Combine and apply movement
        Vector3 finalMovement = horizontalMove + velocity;
        controller.Move(finalMovement * Time.deltaTime);

        HandleLook();
    }

    void HandleLook()
    {
        if (!canLook) return;

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}