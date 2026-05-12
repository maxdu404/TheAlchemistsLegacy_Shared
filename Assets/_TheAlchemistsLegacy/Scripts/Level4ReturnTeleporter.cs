using UnityEngine;

public class Level4ReturnTeleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    [SerializeField] private Vector3 landingPosition = new Vector3(-41.904f, 1.297f, 75.152f);
    [SerializeField] private float maxSnapDownDistance = 1.5f;
    [SerializeField] private float maxSnapUpDistance = 0.5f;
    [SerializeField] private bool faceLeft = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.5f;
    [SerializeField] private float proximityRange = 2.0f;
    [SerializeField] private float teleportCooldown = 1.0f;

    private static float lastTeleportTime = -999.0f;
    private Camera playerCamera;
    private CharacterController playerController;
    private Transform playerTransform;

    private void Start()
    {
        playerCamera = Camera.main;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        TryTeleportFromProximity();

        if (Input.GetMouseButtonDown(1))
        {
            TryTeleportFromAim();
        }
    }

    private void TryTeleportFromProximity()
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            return;
        }

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                return;
            }

            playerTransform = player.transform;
            playerController = player.GetComponent<CharacterController>();
        }

        if (Vector3.Distance(playerTransform.position, transform.position) > proximityRange)
        {
            return;
        }

        TeleportPlayer();
        lastTeleportTime = Time.time;
    }

    private void TryTeleportFromAim()
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            return;
        }

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
        {
            return;
        }

        if (hit.collider.transform != transform && !hit.collider.transform.IsChildOf(transform))
        {
            return;
        }

        TeleportPlayer();
        lastTeleportTime = Time.time;
    }

    private void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            return;
        }

        if (playerController == null)
        {
            playerController = player.GetComponent<CharacterController>();
        }

        bool hadController = playerController != null && playerController.enabled;
        if (hadController)
        {
            playerController.enabled = false;
        }

        PlayerController playerControllerScript = player.GetComponent<PlayerController>();
        Vector3 destination = landingPosition;
        if (playerControllerScript != null)
        {
            destination = playerControllerScript.GetSafeGroundedPosition(destination, maxSnapDownDistance, maxSnapUpDistance);
        }

        player.transform.position = destination;

        if (faceLeft)
        {
            ForcePlayerLookDirection(player, Vector3.left);
        }

        if (hadController)
        {
            if (playerControllerScript != null)
            {
                playerControllerScript.ResetVerticalVelocity();
            }

            playerController.enabled = true;
            playerController.Move(Vector3.zero);
        }

        Debug.Log(gameObject.name + ": returned player to " + destination);
    }

    private void ForcePlayerLookDirection(GameObject player, Vector3 worldForward)
    {
        PlayerController playerControllerScript = player.GetComponent<PlayerController>();
        if (playerControllerScript != null)
        {
            playerControllerScript.ForceLookDirection(worldForward, 0.0f);
            return;
        }

        worldForward.y = 0.0f;
        if (worldForward.sqrMagnitude > 0.001f)
        {
            player.transform.rotation = Quaternion.LookRotation(worldForward.normalized, Vector3.up);
        }
    }
}
