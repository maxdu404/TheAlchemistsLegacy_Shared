using System;
using UnityEngine;

public class Level3TorchGiver : MonoBehaviour
{
    [Header("Torch")]
    [SerializeField] private GameObject torchForIcePrefabOrObject;
    [SerializeField] private string heldTorchName = "torch_for_ice_l3";
    [SerializeField] private bool instantiateTorch = true;
    [SerializeField] private bool giveOnlyOnce = true;
    [SerializeField] private bool lightTorchWhenGiven = true;

    [Header("Interaction")]
    [SerializeField] private float interactionRange = 3.0f;

    private Camera playerCamera;
    private ItemPickup playerPickup;
    private bool hasGivenTorch;

    private void Start()
    {
        playerCamera = Camera.main;
        playerPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryGiveTorch();
        }
    }

    private void TryGiveTorch()
    {
        if (giveOnlyOnce && hasGivenTorch)
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

        if (playerCamera == null || playerPickup == null || playerPickup.handPosition == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            return;
        }

        if (!MatchesThisObject(hit.collider.transform))
        {
            return;
        }

        if (playerPickup.isHoldingItem)
        {
            Debug.Log("Drop your current item before taking the torch.");
            return;
        }

        GameObject torch = CreateTorchObject();
        if (torch == null)
        {
            Debug.LogWarning("Level3TorchGiver needs torch_for_ice_l3 assigned in the Inspector.");
            return;
        }

        GiveTorchToPlayer(torch);
        hasGivenTorch = true;
    }

    private bool MatchesThisObject(Transform hitTransform)
    {
        return hitTransform == transform || hitTransform.IsChildOf(transform);
    }

    private GameObject CreateTorchObject()
    {
        if (torchForIcePrefabOrObject == null)
        {
            return null;
        }

        GameObject torch = instantiateTorch
            ? Instantiate(torchForIcePrefabOrObject)
            : torchForIcePrefabOrObject;

        torch.name = heldTorchName;
        torch.SetActive(true);
        return torch;
    }

    private void GiveTorchToPlayer(GameObject torch)
    {
        Rigidbody body = torch.GetComponent<Rigidbody>();
        if (body != null)
        {
            body.isKinematic = true;
        }

        Collider[] colliders = torch.GetComponentsInChildren<Collider>(true);
        foreach (Collider torchCollider in colliders)
        {
            torchCollider.enabled = false;
        }

        torch.transform.SetParent(playerPickup.handPosition);
        torch.transform.localPosition = Vector3.zero;
        torch.transform.localRotation = Quaternion.identity;

        if (lightTorchWhenGiven)
        {
            torch.SendMessage("Ignite", SendMessageOptions.DontRequireReceiver);
            EnableFlameChildren(torch);
        }

        playerPickup.currentItem = torch;
        playerPickup.currentItemName = heldTorchName;
        playerPickup.isHoldingItem = true;

        Debug.Log("Received " + heldTorchName);
    }

    private void EnableFlameChildren(GameObject torch)
    {
        foreach (Transform child in torch.GetComponentsInChildren<Transform>(true))
        {
            string lowerName = child.name.ToLowerInvariant();
            if (lowerName.Contains("flame") || lowerName.Contains("fire"))
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}
