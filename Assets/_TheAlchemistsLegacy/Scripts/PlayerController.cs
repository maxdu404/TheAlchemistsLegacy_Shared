using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.25f;
    [SerializeField] private Camera playerCamera;

    [Header("View Settings")]
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float maxLookUpAngle = 60f;
    [SerializeField] private float maxLookDownAngle = -60f;
    [SerializeField] private float cameraFieldOfView = 55f;

    [Header("Grounding Safety")]
    [SerializeField] private bool snapToGroundOnStart = true;
    [SerializeField] private bool preserveLevel0AndLevel1SpawnHeight = true;
    [SerializeField] private float groundSnapProbeHeight = 4.0f;
    [SerializeField] private float groundSnapProbeDistance = 12.0f;
    [SerializeField] private float groundClearance = 0.14f;
    [SerializeField] private float startSnapMaxDownDistance = 1.5f;
    [SerializeField] private float startSnapMaxUpDistance = 0.5f;

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

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = cameraFieldOfView;
        }

        if (snapToGroundOnStart)
        {
            if (ShouldSnapToGroundOnStartForScene())
            {
                transform.position = GetSafeGroundedPosition(transform.position, startSnapMaxDownDistance, startSnapMaxUpDistance);
            }

            velocity = Vector3.zero;
        }

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

        Vector3 footCenter = GetCharacterFootCenter();
        float radius = characterController.radius;

        isGrounded =
            Physics.CheckSphere(footCenter, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore) ||
            Physics.CheckSphere(footCenter + transform.forward * radius, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore) ||
            Physics.CheckSphere(footCenter - transform.forward * radius, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore) ||
            Physics.CheckSphere(footCenter - transform.right * radius, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore) ||
            Physics.CheckSphere(footCenter + transform.right * radius, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);

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
            float currentMoveSpeed = IsSprinting() ? moveSpeed * sprintMultiplier : moveSpeed;
            characterController.Move(moveDirection * currentMoveSpeed * Time.deltaTime);
        }

        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    private Vector3 GetCharacterFootCenter()
    {
        if (characterController == null)
        {
            return transform.position;
        }

        float bottomOffset = (characterController.height * 0.5f) - characterController.center.y;
        float probeLift = Mathf.Max(characterController.skinWidth + 0.03f, 0.08f);
        return transform.position + Vector3.up * (probeLift - bottomOffset);
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

    private bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private bool ShouldSnapToGroundOnStartForScene()
    {
        if (!preserveLevel0AndLevel1SpawnHeight)
        {
            return true;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName != "Level0" && sceneName != "Level1";
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

    public Vector3 GetSafeGroundedPosition(Vector3 desiredPosition)
    {
        return GetSafeGroundedPosition(desiredPosition, groundSnapProbeDistance, groundSnapProbeHeight);
    }

    public Vector3 GetSafeGroundedPosition(Vector3 desiredPosition, float maxSnapDownDistance, float maxSnapUpDistance)
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        float controllerBottomOffset = 0.0f;
        if (characterController != null)
        {
            controllerBottomOffset = Mathf.Max(0.0f, (characterController.height * 0.5f) - characterController.center.y);
        }

        float clearance = groundClearance;
        if (characterController != null)
        {
            clearance = Mathf.Max(clearance, characterController.skinWidth + 0.02f);
        }

        Vector3 probeOrigin = desiredPosition + Vector3.up * groundSnapProbeHeight;
        float probeDistance = groundSnapProbeHeight + groundSnapProbeDistance;

        bool controllerWasEnabled = characterController != null && characterController.enabled;
        if (controllerWasEnabled)
        {
            characterController.enabled = false;
        }

        bool foundGround = TryFindGround(probeOrigin, probeDistance, out RaycastHit hit);

        if (controllerWasEnabled)
        {
            characterController.enabled = true;
        }

        if (foundGround && IsGroundWithinSnapWindow(desiredPosition, hit.point.y, controllerBottomOffset, maxSnapDownDistance, maxSnapUpDistance))
        {
            desiredPosition.y = hit.point.y + controllerBottomOffset + clearance;
        }

        return desiredPosition;
    }

    public void ResetVerticalVelocity()
    {
        velocity = Vector3.zero;
    }

    private bool TryFindGround(Vector3 probeOrigin, float probeDistance, out RaycastHit hit)
    {
        if (groundMask.value != 0 && Physics.Raycast(probeOrigin, Vector3.down, out hit, probeDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            return true;
        }

        return Physics.Raycast(probeOrigin, Vector3.down, out hit, probeDistance, ~0, QueryTriggerInteraction.Ignore);
    }

    private bool IsGroundWithinSnapWindow(Vector3 desiredPosition, float groundY, float controllerBottomOffset, float maxSnapDownDistance, float maxSnapUpDistance)
    {
        float desiredGroundY = desiredPosition.y - controllerBottomOffset;
        float delta = groundY - desiredGroundY;

        if (delta < 0.0f)
        {
            return Mathf.Abs(delta) <= maxSnapDownDistance;
        }

        return delta <= maxSnapUpDistance;
    }
}
