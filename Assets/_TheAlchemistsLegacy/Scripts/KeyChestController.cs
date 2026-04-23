using UnityEngine;

public class KeyChestController : MonoBehaviour
{
    [Header("Chest")]
    public Transform lid;
    public float openAngle = -90f;
    public float openSpeed = 2f;

    [Header("Interaction")]
    public float interactionRange = 3f;

    [Header("Contents")]
    [SerializeField] private GameObject[] contents;
    [SerializeField] private bool hideContentsUntilOpen = true;

    public bool IsOpen
    {
        get { return isOpen; }
    }

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Camera playerCamera;

    private void Awake()
    {
        if (lid == null)
        {
            lid = transform.Find("chest_top");
        }
    }

    private void Start()
    {
        playerCamera = Camera.main;
        SetContentsAvailable(false);

        if (lid != null)
        {
            closedRotation = lid.localRotation;
            openRotation = closedRotation * Quaternion.Euler(openAngle, 0.0f, 0.0f);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isOpen)
        {
            TryOpenChest();
        }

        if (lid != null)
        {
            Quaternion targetRotation = isOpen ? openRotation : closedRotation;
            lid.localRotation = Quaternion.Slerp(lid.localRotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    private void TryOpenChest()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            return;
        }

        if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
        {
            OpenChest();
        }
    }

    public void OpenChest()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;
        SetContentsAvailable(true);
    }

    private void SetContentsAvailable(bool isAvailable)
    {
        if (contents == null)
        {
            return;
        }

        foreach (GameObject content in contents)
        {
            if (content == null)
            {
                continue;
            }

            if (hideContentsUntilOpen)
            {
                content.SetActive(isAvailable);
            }

            foreach (Collider contentCollider in content.GetComponentsInChildren<Collider>(true))
            {
                contentCollider.enabled = isAvailable;
            }

            foreach (Rigidbody contentBody in content.GetComponentsInChildren<Rigidbody>(true))
            {
                contentBody.isKinematic = !isAvailable;
            }
        }
    }
}
