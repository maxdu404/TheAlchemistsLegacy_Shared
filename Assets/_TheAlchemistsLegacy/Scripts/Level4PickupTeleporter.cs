using UnityEngine;

public class Level4PickupTeleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    [SerializeField] private Vector3 landingPosition = new Vector3(-41.904f, 1.297f, 75.152f);
    [SerializeField] private float maxSnapDownDistance = 1.5f;
    [SerializeField] private float maxSnapUpDistance = 0.5f;
    [SerializeField] private bool faceNegativeZ = true;

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

        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem != gameObject)
        {
            return;
        }

        TeleportPlayer();
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

        if (faceNegativeZ)
        {
            ForcePlayerLookDirection(player, Vector3.back);
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
        Debug.Log(gameObject.name + " picked up — teleported player to " + destination);
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
