using UnityEngine;

public class player_navigation : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 3f;

    [Header("Camera Alignment")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool alignMovementWithCamera = true;

    [Header("Mouse Look")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private bool holdRightMouseToLook = true;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Scroll Movement")]
    [SerializeField] private float scrollSpeed = 20f;

    private float yaw;   // Drehung um die Y-Achse (links/rechts)
    private float pitch; // Blick nach oben/unten

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float startYaw;
    private float startPitch;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (cameraTransform != null)
        {
            // Startwerte aus aktueller Transform übernehmen
            Vector3 euler = cameraTransform.rotation.eulerAngles;
            yaw = euler.y;
            pitch = euler.x;

            startYaw = yaw;
            startPitch = pitch;
        }
    }

    private void Update()
    {
        HandleMouseLook();

        // Debug / Fallback: Zur Ausgangsposition per "R"-Taste zurueckkehren
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetToStart();
        }

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

        // Scrollrad separat als Vor-/Zurueckbewegung entlang der Blickrichtung auswerten
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 scrollMove = Vector3.zero;
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            Vector3 forward = alignMovementWithCamera && cameraTransform != null
                ? cameraTransform.forward
                : transform.forward;

            forward.y = 0f;
            forward.Normalize();

            scrollMove = forward * (scroll * scrollSpeed);
        }

        transform.position += moveDirection * currentSpeed * Time.deltaTime
                     + scrollMove * Time.deltaTime;
    }

    public void ResetToStart()
    {
        // Position und Rotation des Spielers zurücksetzen
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Blickrichtung zurücksetzen
        yaw = startYaw;
        pitch = startPitch;

        if (cameraTransform != null)
        {
            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
    }

    private void HandleMouseLook()
    {
        if (cameraTransform == null)
            return;

        // Optional nur drehen, solange rechte Maustaste gehalten wird
        if (holdRightMouseToLook && !Input.GetMouseButton(1))
            return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw   += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity; // invertiert, damit hoch/runter „natürlich“ wirkt
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Spieler um Y drehen (links/rechts)
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);

        // Kamera an Spieler koppeln und nur um X kippen (hoch/runter)
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
