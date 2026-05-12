using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level4LampDoorTeleporter : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform targetDoor;
    [SerializeField] private string targetDoorName = "";
    [SerializeField] private Vector3 targetOffset = Vector3.zero;
    [SerializeField] private bool faceTargetForward = true;
    [SerializeField] private bool faceNegativeZAfterTeleport = true;
    [SerializeField] private float maxSnapDownDistance = 1.5f;
    [SerializeField] private float maxSnapUpDistance = 0.5f;

    [Header("Required Lamp")]
    [SerializeField] private string requiredHeldItemName = "";
    [SerializeField] private bool keepUnlockedAfterSuccessfulUse = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;
    [SerializeField] private float interactionAimRadius = 0.22f;
    [SerializeField] private float teleportCooldown = 1.0f;

    public string RequiredHeldItemName
    {
        get { return requiredHeldItemName; }
    }

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private CharacterController playerController;
    private bool isUnlocked;
    private static float lastTeleportTime = -999.0f;

    private void Reset()
    {
        AutoConfigureFromName();
    }

    private void OnValidate()
    {
        AutoConfigureFromName();
    }

    private void Start()
    {
        AutoConfigureFromName();
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        if (targetDoor == null && !string.IsNullOrWhiteSpace(targetDoorName))
        {
            GameObject targetObject = FindSceneObjectByName(targetDoorName);
            if (targetObject != null)
            {
                targetDoor = targetObject.transform;
            }
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(1))
        {
            return;
        }

        TryTeleport();
    }

    private void AutoConfigureFromName()
    {
        string lowerName = gameObject.name.ToLowerInvariant();

        if (lowerName.Contains("2th"))
        {
            if (string.IsNullOrWhiteSpace(targetDoorName))
            {
                targetDoorName = "Wall_DoorStand_2th_tar";
            }

            if (string.IsNullOrWhiteSpace(requiredHeldItemName))
            {
                requiredHeldItemName = "lamp_l4_2th";
            }
        }
        else if (lowerName.Contains("3th"))
        {
            if (string.IsNullOrWhiteSpace(targetDoorName))
            {
                targetDoorName = "Wall_DoorStand_3th_tar";
            }

            if (string.IsNullOrWhiteSpace(requiredHeldItemName))
            {
                requiredHeldItemName = "lamp_3th";
            }
        }
    }

    private void TryTeleport()
    {
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            return;
        }

        if (targetDoor == null && !string.IsNullOrWhiteSpace(targetDoorName))
        {
            GameObject targetObject = FindSceneObjectByName(targetDoorName);
            if (targetObject != null)
            {
                targetDoor = targetObject.transform;
            }
        }

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

        if (!AimInteraction.Cast(playerCamera, interactionRange, interactionAimRadius, out RaycastHit hit))
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
        if (isUnlocked)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(requiredHeldItemName))
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

    private bool MatchesThisDoor(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
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

        Vector3 flatForward = worldForward;
        flatForward.y = 0.0f;
        if (flatForward.sqrMagnitude < 0.001f)
        {
            return;
        }

        player.transform.rotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, objectName);
            if (match != null && match.gameObject != gameObject)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        if (root.name.Equals(objectName, StringComparison.OrdinalIgnoreCase))
        {
            return root;
        }

        foreach (Transform child in root)
        {
            Transform match = FindChildByName(child, objectName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
