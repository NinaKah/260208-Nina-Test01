using UnityEngine;

public class player_navigation : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Camera Alignment")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool alignMovementWithCamera = true;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        // WASD / Pfeiltasten Input ueber das alte Input System
        float horizontal = Input.GetAxisRaw("Horizontal");   // A/D, Links/Rechts
        float vertical   = Input.GetAxisRaw("Vertical");     // W/S, Vor/Zurueck

        Vector3 input = new Vector3(horizontal, 0f, vertical);
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        Vector3 moveDirection;

        // Bewegung optional an Blickrichtung der Kamera ausrichten
        if (alignMovementWithCamera && cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight   = cameraTransform.right;

            camForward.y = 0f;
            camRight.y   = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * input.z + camRight * input.x;
        }
        else
        {
            // Bewegung relativ zum eigenen Transform
            moveDirection = transform.TransformDirection(input);
        }

        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }
}
