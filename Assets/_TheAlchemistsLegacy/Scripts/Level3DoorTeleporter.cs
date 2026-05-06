using System;
using UnityEngine;

public class Level3DoorTeleporter : MonoBehaviour
{
    [Header("Teleport")]
    [SerializeField] private Transform targetDoor;
    [SerializeField] private Vector3 targetOffset = Vector3.zero;
    [SerializeField] private bool faceTargetForward = true;

    [Header("Requirement")]
    [SerializeField] private bool requireHeldItem = false;
    [SerializeField] private string requiredHeldItemName = "lamp_l3";
    [SerializeField] private bool keepUnlockedAfterSuccessfulUse = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private CharacterController playerController;
    private bool isUnlocked;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryTeleport();
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

        player.transform.position = targetDoor.position + targetOffset;

        if (faceTargetForward)
        {
            Vector3 eulerAngles = player.transform.eulerAngles;
            eulerAngles.y = targetDoor.eulerAngles.y;
            player.transform.eulerAngles = eulerAngles;
        }

        if (hadController)
        {
            playerController.enabled = true;
            playerController.Move(Vector3.zero);
        }

        Debug.Log("Teleported from " + gameObject.name + " to " + targetDoor.name);
    }
}
