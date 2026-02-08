using UnityEngine;

public class rotationblubb : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 1, -3);
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 2f;
    
    [Header("Levitation")]
    [SerializeField] private float levitationSpeed = 1f;
    [SerializeField] private float levitationHeight = 0.5f;
    
    [Header("Rotation")]
    [SerializeField] private Vector3 rotationDirection = Vector3.up;
    [SerializeField] private float rotationSpeed = 50f;
    
    [Header("Drag")]
    [SerializeField] private float dragDistance = 10f;
    
    private Vector3 startPosition;
    private Vector3 currentVelocity;
    private bool isDragging = false;
    private Vector3 dragOffset;
    
    void Start()
    {
        startPosition = transform.position;
        
        // Cube rot färben
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        
        // Falls keine Kamera im Inspector gesetzt wurde, suche die Main Camera
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                cameraTransform = mainCamera.transform;
        }
    }

    void Update()
    {
        // Nur Levitation und Rotation wenn nicht gedraggt wird
        if (!isDragging)
        {
            // Levitation - auf und ab bewegen
            float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;
            Vector3 newPosition = new Vector3(transform.position.x, newY, transform.position.z);
            transform.position = newPosition;
            
            // Rotation - in die gewünschte Richtung
            transform.Rotate(rotationDirection.normalized * rotationSpeed * Time.deltaTime);
            
            // WASD Bewegung
            HandleMovement();
        }
        else
        {
            // Während des Draggings Position aktualisieren
            if (Camera.main != null)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = dragDistance;
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = worldPos + dragOffset;
                startPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);
            }
        }
        
        // Kamera folgen
        UpdateCamera();
    }
    
    void OnMouseDown()
    {
        // Wenn auf den Cube geklickt wird
        isDragging = true;
        
        if (Camera.main != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = dragDistance;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            dragOffset = transform.position - worldPos;
        }
    }
    
    void OnMouseUp()
    {
        // Wenn Maus losgelassen wird
        isDragging = false;
    }
    
    
    void HandleMovement()
    {
        Vector3 moveInput = Vector3.zero;
        
        // WASD Input mit Legacy Input System
        if (Input.GetKey(KeyCode.W))
            moveInput += Vector3.forward;
        if (Input.GetKey(KeyCode.S))
            moveInput += Vector3.back;
        if (Input.GetKey(KeyCode.A))
            moveInput += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            moveInput += Vector3.right;
        
        moveInput = moveInput.normalized;
        
        // Geschwindigkeit mit Shift erhöhen
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            currentSpeed *= sprintMultiplier;
        
        // Bewegung anwenden
        transform.Translate(moveInput * currentSpeed * Time.deltaTime, Space.Self);
        startPosition = new Vector3(transform.position.x, startPosition.y, transform.position.z);
    }
    
    void UpdateCamera()
    {
        if (cameraTransform != null)
        {
            // Kamera relativ zum Objekt positionieren
            cameraTransform.position = transform.position + cameraOffset;
            cameraTransform.LookAt(transform.position + Vector3.up * 0.5f);
        }
    }
}
