using UnityEngine;


/// Creates floating and rotating animation for artifacts on pedestals

public class ArtifactLevitation : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.05f;
    public float frequency = 1f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 20f;

    [HideInInspector] public bool isBeingHeld = false;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        if (isBeingHeld) return;

        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    public void ResetToPedestal()
    {
        isBeingHeld = false;
        transform.position = startPos;
        transform.rotation = startRot;
    }
}