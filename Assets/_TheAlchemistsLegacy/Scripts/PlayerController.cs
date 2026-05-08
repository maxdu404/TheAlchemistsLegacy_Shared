using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Camera playerCamera;

    [Header("View Settings")]
    [SerializeField] private float mouseSensitivity = 0.8f;
    [SerializeField] private float maxLookUpAngle = 75f;
    [SerializeField] private float maxLookDownAngle = -75f;

    [Header("Jump Settings")]
    [SerializeField] private bool enableJump = false;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.4f;

    private float rotationX = 0f;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleMouseLook();
        HandleJump();
    }

    private void CheckGrounded()
    {
        if (characterController == null)
        {
            isGrounded = false;
            return;
        }

        Vector3 center = transform.position + characterController.center;
        float radius = characterController.radius;

        isGrounded =
            Physics.CheckSphere(center, groundCheckDistance, groundMask) ||
            Physics.CheckSphere(center + transform.forward * radius, groundCheckDistance, groundMask) ||
            Physics.CheckSphere(center - transform.forward * radius, groundCheckDistance, groundMask) ||
            Physics.CheckSphere(center - transform.right * radius, groundCheckDistance, groundMask) ||
            Physics.CheckSphere(center + transform.right * radius, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -1f;
        }
    }

    private void HandleMovement()
    {
        if (characterController == null || playerCamera == null)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (playerCamera == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, maxLookDownAngle, maxLookUpAngle);

        transform.Rotate(Vector3.up * mouseX);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    private void HandleJump()
    {
        if (!enableJump || characterController == null)
        {
            return;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ForceLookDirection(Vector3 worldForward, float pitch = 0f)
    {
        worldForward.y = 0f;
        if (worldForward.sqrMagnitude < 0.001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(worldForward.normalized, Vector3.up);
        rotationX = Mathf.Clamp(pitch, maxLookDownAngle, maxLookUpAngle);

        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }
}
