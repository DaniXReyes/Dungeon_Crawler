using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 1.2f;
    public float gravity = -19.62f; // Gravedad aumentada (x2) para que el salto sea más ágil y realista

    [Header("Configuración de Cámara")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // Bloquea y oculta el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraLook();
    }

    private void HandleMovement()
    {
        bool isGrounded = controller.isGrounded;

        // Resetea la velocidad vertical si está tocando el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Inputs de movimiento (WASD / Flechas)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Comprobar si se mantiene pulsado Left Shift para correr
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        // Dirección local respecto a hacia dónde mira el personaje
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Salto con la barra espaciadora (solo si está en el suelo)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Fórmula física para alcanzar exactamente la altura deseada
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCameraLook()
    {
        // Inputs del ratón
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal: gira todo el cuerpo del jugador (Eje Y)
        transform.Rotate(Vector3.up * mouseX);

        // Rotación vertical: inclina solo la cámara arriba/abajo con límite (Eje X)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        
        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}