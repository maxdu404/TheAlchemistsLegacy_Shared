using System;
using UnityEngine;

public class Level3DoorTeleporter : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform targetDoor;
    [SerializeField] private Vector3 targetOffset = Vector3.zero;
    [SerializeField] private bool faceTargetForward = true;
    [SerializeField] private bool faceNegativeZAfterTeleport = true;
    [SerializeField] private float maxSnapDownDistance = 1.5f;
    [SerializeField] private float maxSnapUpDistance = 0.5f;

    [Header("Requirement")]
    [SerializeField] private bool requireHeldItem = false;
    [SerializeField] private string requiredHeldItemName = "lamp_l3";
    [SerializeField] private bool keepUnlockedAfterSuccessfulUse = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private bool teleportWhenPlayerIsNear = false;
    [SerializeField] private float proximityRange = 2.0f;
    [SerializeField] private float teleportCooldown = 1.0f;

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private CharacterController playerController;
    private Transform playerTransform;
    private bool isUnlocked;
    private static float lastTeleportTime = -999.0f;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (teleportWhenPlayerIsNear)
        {
            TryTeleportFromProximity();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryTeleport();
        }
    }

    private void TryTeleportFromProximity()
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            return;
        }

        if (targetDoor == null)
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

        float distance = Vector3.Distance(playerTransform.position, transform.position);
        if (distance > proximityRange)
        {
            return;
        }

        if (!CanUseDoor())
        {
            return;
        }

        TeleportPlayer();
        lastTeleportTime = Time.time;

        if (keepUnlockedAfterSuccessfulUse)
        {
            isUnlocked = true;
        }
    }

    private void TryTeleport()
    {
        if (targetDoor == null)
        {
            return;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerPickup == null)
        {
            playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            return;
        }

        if (!MatchesThisDoor(hit.collider.transform))
        {
            return;
        }

        if (!CanUseDoor())
        {
            Debug.Log(gameObject.name + " needs " + requiredHeldItemName);
            return;
        }

        TeleportPlayer();
        lastTeleportTime = Time.time;

        if (keepUnlockedAfterSuccessfulUse)
        {
            isUnlocked = true;
        }
    }

    private bool CanUseDoor()
    {
        if (!requireHeldItem || isUnlocked)
        {
            return true;
        }

        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            return false;
        }

        return NameContains(playerPickup.currentItem.name, requiredHeldItemName)
            || NameContains(playerPickup.currentItemName, requiredHeldItemName);
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool MatchesThisDoor(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
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
        Vector3 destination = targetDoor.position + targetOffset;
        if (playerControllerScript != null)
        {
            destination = playerControllerScript.GetSafeGroundedPosition(destination, maxSnapDownDistance, maxSnapUpDistance);
        }

        player.transform.position = destination;

        if (faceNegativeZAfterTeleport)
        {
            ForcePlayerLookDirection(player, Vector3.back);
        }
        else if (faceTargetForward)
        {
            Vector3 eulerAngles = player.transform.eulerAngles;
            eulerAngles.y = targetDoor.eulerAngles.y;
            player.transform.eulerAngles = eulerAngles;
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

        Debug.Log("Teleported from " + gameObject.name + " to " + targetDoor.name);
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

        Camera playerCamera = player.GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.identity;
        }
    }
}
