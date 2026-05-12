using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3ExitKeyReturnTeleporter : MonoBehaviour
{
    [Header("Trigger Item")]
    [SerializeField] private string exitKeyName = "Key_l3_exit";

    [Header("Teleport Target")]
    [SerializeField] private Transform targetPoint;
    [SerializeField] private string targetObjectName = "StampSlot_l3_exit";
    [SerializeField] private Vector3 targetOffset = new Vector3(0.0f, 0.0f, -1.5f);
    [SerializeField] private bool useFixedLandingPosition = true;
    [SerializeField] private Vector3 fixedLandingPosition = new Vector3(-42.257f, 1.0f, 66.629f);
    [SerializeField] private bool faceTarget = true;
    [SerializeField] private bool faceNegativeZAfterTeleport = true;
    [SerializeField] private float maxSnapDownDistance = 1.5f;
    [SerializeField] private float maxSnapUpDistance = 0.5f;

    private bool hasTeleported;
    private ItemPickup playerPickup;
    private CharacterController playerController;

    private void Start()
    {
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }

        if (!useFixedLandingPosition && targetPoint == null)
        {
            GameObject targetObject = FindSceneObjectByName(targetObjectName);
            if (targetObject != null)
            {
                targetPoint = targetObject.transform;
            }
        }
    }

    private void Update()
    {
        if (hasTeleported)
        {
            return;
        }

        if (playerPickup == null)
        {
            playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (IsExitKeyHeld())
        {
            TeleportPlayerToExitSlot();
        }
    }

    private bool IsExitKeyHeld()
    {
        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            return false;
        }

        bool thisKeyIsHeld = playerPickup.currentItem == gameObject;
        bool matchingName = NameContains(playerPickup.currentItem.name, exitKeyName)
            || NameContains(playerPickup.currentItemName, exitKeyName);

        return thisKeyIsHeld || matchingName;
    }

    private void TeleportPlayerToExitSlot()
    {
        if (targetPoint == null)
        {
            GameObject targetObject = FindSceneObjectByName(targetObjectName);
            if (targetObject != null)
            {
                targetPoint = targetObject.transform;
            }
        }

        if (!useFixedLandingPosition && targetPoint == null)
        {
            Debug.LogWarning("Level3ExitKeyReturnTeleporter could not find " + targetObjectName);
            return;
        }

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
        Vector3 destination = useFixedLandingPosition
            ? fixedLandingPosition
            : targetPoint.position + targetOffset;
        if (playerControllerScript != null)
        {
            destination = playerControllerScript.GetSafeGroundedPosition(destination, maxSnapDownDistance, maxSnapUpDistance);
        }

        player.transform.position = destination;

        if (faceNegativeZAfterTeleport)
        {
            ForcePlayerLookDirection(player, Vector3.back);
        }
        else if (faceTarget && targetPoint != null)
        {
            Vector3 direction = targetPoint.position - player.transform.position;
            direction.y = 0.0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                ForcePlayerLookDirection(player, direction.normalized);
            }
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

        hasTeleported = true;
        Debug.Log("Key_l3_exit collected. Returned player to StampSlot_l3_exit.");
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

    private bool NameContains(string source, string expected)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(expected))
        {
            return false;
        }

        return source.IndexOf(expected, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
        {
            return null;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        foreach (GameObject rootObject in activeScene.GetRootGameObjects())
        {
            Transform match = FindChildByName(rootObject.transform, objectName);
            if (match != null)
            {
                return match.gameObject;
            }
        }

        return null;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        if (root.name == objectName)
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
