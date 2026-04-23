using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TheAlchemistsLegacy_Level0 : MonoBehaviour
{
    [Header("Scene Flow")]
    [SerializeField] private string trialCompleteSceneName = "TrailComplete";
    [SerializeField] private Transform player;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float exitDistance = 1.5f;
    [SerializeField] private float completionDelay = 1.0f;

    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ItemPickup playerPickup;
    [SerializeField] private float interactionRange = 3.0f;

    [Header("Objects")]
    [SerializeField] private bool useExternalBucketScripts = false;
    [SerializeField] private GameObject scrollObject;
    [SerializeField] private GameObject chestObject;
    [SerializeField] private Transform chestLid;
    [SerializeField] private GameObject runeAObject;
    [SerializeField] private GameObject runeBObject;
    [SerializeField] private GameObject runeASlotObject;
    [SerializeField] private GameObject runeBSlotObject;
    [SerializeField] private Transform runeAPlacedPoint;
    [SerializeField] private Transform runeBPlacedPoint;
    [SerializeField] private Transform doorPivot;

    [Header("Optional Visuals")]
    [SerializeField] private GameObject runeAPlacedVisual;
    [SerializeField] private GameObject runeBPlacedVisual;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("UI")]
    [SerializeField] private GameObject scrollPanel;
    [SerializeField] private TextMeshProUGUI scrollText;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI heldItemText;
    [SerializeField] private TextMeshProUGUI successText;

    [Header("Text")]
    [TextArea(3, 8)]
    [SerializeField] private string scrollMessage =
        "My apprentice, read slow and look close.\n\nWhat is marked in stone cannot be forgotten.\nMatch the two marks, and the path will open.";
    [SerializeField] private string successMessage = "Good. You remember how to see.";

    [Header("Animation")]
    [SerializeField] private float chestOpenAngle = -90.0f;
    [SerializeField] private float chestOpenSpeed = 4.0f;
    [SerializeField] private float doorOpenAngle = 90.0f;
    [SerializeField] private float doorOpenSpeed = 2.0f;

    private bool scrollIsOpen;
    private bool chestIsOpen;
    private bool runeAPlaced;
    private bool runeBPlaced;
    private bool doorIsOpen;
    private bool levelIsCompleting;
    private float promptOverrideUntil;

    private Quaternion chestClosedRotation;
    private Quaternion chestOpenRotation;
    private Quaternion doorClosedRotation;
    private Quaternion doorOpenRotation;

    private void Start()
    {
        CacheReferences();
        SetupInitialState();
    }

    private void Update()
    {
        AnimateChest();
        AnimateDoor();
        UpdateHeldItemText();
        UpdateExternalBucketState();

        if (levelIsCompleting)
        {
            return;
        }

        if (scrollIsOpen)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseScroll();
            }

            return;
        }

        UpdatePrompt();

        if (Input.GetMouseButtonDown(1))
        {
            TryInteract();
        }

        if (doorIsOpen && exitPoint != null && player != null)
        {
            if (Vector3.Distance(player.position, exitPoint.position) <= exitDistance)
            {
                CompleteLevel();
            }
        }
    }

    private void CacheReferences()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerPickup == null)
        {
            playerPickup = FindObjectOfType<ItemPickup>();
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (chestLid != null)
        {
            chestClosedRotation = chestLid.localRotation;
            chestOpenRotation = chestClosedRotation * Quaternion.Euler(chestOpenAngle, 0.0f, 0.0f);
        }

        if (doorPivot != null)
        {
            doorClosedRotation = doorPivot.localRotation;
            doorOpenRotation = doorClosedRotation * Quaternion.Euler(0.0f, doorOpenAngle, 0.0f);
        }
    }

    private void SetupInitialState()
    {
        SetActive(scrollPanel, false);
        SetActive(successText != null ? successText.gameObject : null, false);
        SetActive(runeBObject, false);
        SetActive(runeAPlacedVisual, false);
        SetActive(runeBPlacedVisual, false);

        if (scrollText != null)
        {
            scrollText.text = scrollMessage;
        }

        if (promptText != null)
        {
            promptText.text = "";
        }

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0.0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    private void TryInteract()
    {
        if (!RaycastFromView(out RaycastHit hit))
        {
            return;
        }

        GameObject target = hit.collider.gameObject;

        if (Matches(target, scrollObject))
        {
            OpenScroll();
            return;
        }

        if (Matches(target, chestObject))
        {
            OpenChest();
            return;
        }

        if (Matches(target, runeASlotObject))
        {
            if (useExternalBucketScripts)
            {
                return;
            }

            TryPlaceRune(true);
            return;
        }

        if (Matches(target, runeBSlotObject))
        {
            if (useExternalBucketScripts)
            {
                return;
            }

            TryPlaceRune(false);
        }
    }

    private void UpdatePrompt()
    {
        if (promptText == null)
        {
            return;
        }

        if (Time.unscaledTime < promptOverrideUntil)
        {
            return;
        }

        if (!RaycastFromView(out RaycastHit hit))
        {
            promptText.text = "";
            return;
        }

        GameObject target = hit.collider.gameObject;

        if (Matches(target, scrollObject))
        {
            promptText.text = "[RMB] Read";
        }
        else if (Matches(target, chestObject))
        {
            promptText.text = chestIsOpen ? "The chest is open." : "[RMB] Open Chest";
        }
        else if (Matches(target, runeASlotObject))
        {
            promptText.text = runeAPlaced ? "This mark is complete." : "[RMB] Use held ball";
        }
        else if (Matches(target, runeBSlotObject))
        {
            promptText.text = runeBPlaced ? "This mark is complete." : "[RMB] Use held ball";
        }
        else
        {
            promptText.text = "";
        }
    }

    private bool RaycastFromView(out RaycastHit hit)
    {
        hit = default;

        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        return Physics.Raycast(ray, out hit, interactionRange);
    }

    private void OpenScroll()
    {
        scrollIsOpen = true;
        SetActive(scrollPanel, true);
        Time.timeScale = 0.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseScroll()
    {
        scrollIsOpen = false;
        SetActive(scrollPanel, false);
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OpenChest()
    {
        if (chestIsOpen)
        {
            return;
        }

        chestIsOpen = true;
        SetActive(runeBObject, true);
    }

    private void TryPlaceRune(bool firstSlot)
    {
        if (playerPickup == null || !playerPickup.isHoldingItem || playerPickup.currentItem == null)
        {
            ShowTemporaryPrompt("Hold the matching rune first.");
            return;
        }

        GameObject expectedRune = firstSlot ? runeAObject : runeBObject;
        bool slotAlreadyComplete = firstSlot ? runeAPlaced : runeBPlaced;

        if (slotAlreadyComplete)
        {
            ShowTemporaryPrompt("This mark is already complete.");
            return;
        }

        if (!MatchesHeldItem(expectedRune))
        {
            ShowTemporaryPrompt("This rune does not match this mark.");
            return;
        }

        PlaceHeldRune(firstSlot);
        CheckDoorState();
    }

    private bool MatchesHeldItem(GameObject expectedRune)
    {
        if (expectedRune == null || playerPickup.currentItem == null)
        {
            return false;
        }

        if (playerPickup.currentItem == expectedRune)
        {
            return true;
        }

        return playerPickup.currentItem.name.Contains(expectedRune.name);
    }

    private void PlaceHeldRune(bool firstSlot)
    {
        GameObject heldItem = playerPickup.currentItem;
        Transform placedPoint = firstSlot ? runeAPlacedPoint : runeBPlacedPoint;
        GameObject placedVisual = firstSlot ? runeAPlacedVisual : runeBPlacedVisual;

        if (placedPoint != null)
        {
            heldItem.transform.SetParent(placedPoint);
            heldItem.transform.localPosition = Vector3.zero;
            heldItem.transform.localRotation = Quaternion.identity;
        }
        else
        {
            SetActive(heldItem, false);
        }

        Collider heldCollider = heldItem.GetComponent<Collider>();
        if (heldCollider != null)
        {
            heldCollider.enabled = false;
        }

        Rigidbody heldBody = heldItem.GetComponent<Rigidbody>();
        if (heldBody != null)
        {
            heldBody.isKinematic = true;
        }

        playerPickup.currentItem = null;
        playerPickup.currentItemName = "";
        playerPickup.isHoldingItem = false;

        SetActive(placedVisual, true);

        if (firstSlot)
        {
            runeAPlaced = true;
        }
        else
        {
            runeBPlaced = true;
        }
    }

    private void CheckDoorState()
    {
        if (runeAPlaced && runeBPlaced && !doorIsOpen)
        {
            doorIsOpen = true;

            if (successText != null)
            {
                successText.text = successMessage;
                successText.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateExternalBucketState()
    {
        if (!useExternalBucketScripts)
        {
            return;
        }

        if (!runeAPlaced && runeAPlacedVisual != null && runeAPlacedVisual.activeSelf)
        {
            runeAPlaced = true;
        }

        if (!runeBPlaced && runeBPlacedVisual != null && runeBPlacedVisual.activeSelf)
        {
            runeBPlaced = true;
        }

        CheckDoorState();
    }

    private void CompleteLevel()
    {
        if (levelIsCompleting)
        {
            return;
        }

        levelIsCompleting = true;
        StartCoroutine(CompleteLevelRoutine());
    }

    private IEnumerator CompleteLevelRoutine()
    {
        yield return new WaitForSeconds(completionDelay);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0.0f;

            while (elapsed < 1.0f)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed);
                yield return null;
            }
        }

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(trialCompleteSceneName);
    }

    private void AnimateChest()
    {
        if (chestLid == null)
        {
            return;
        }

        Quaternion targetRotation = chestIsOpen ? chestOpenRotation : chestClosedRotation;
        chestLid.localRotation = Quaternion.Slerp(chestLid.localRotation, targetRotation, Time.deltaTime * chestOpenSpeed);
    }

    private void AnimateDoor()
    {
        if (doorPivot == null || !doorIsOpen)
        {
            return;
        }

        doorPivot.localRotation = Quaternion.Slerp(doorPivot.localRotation, doorOpenRotation, Time.deltaTime * doorOpenSpeed);
    }

    private void UpdateHeldItemText()
    {
        if (heldItemText == null || playerPickup == null)
        {
            return;
        }

        heldItemText.text = playerPickup.isHoldingItem ? $"Holding: {playerPickup.currentItemName}" : "";
    }

    private void ShowTemporaryPrompt(string message)
    {
        if (promptText == null)
        {
            Debug.Log(message);
            return;
        }

        promptText.text = message;
        promptOverrideUntil = Time.unscaledTime + 1.5f;
    }

    private bool Matches(GameObject target, GameObject expected)
    {
        if (target == null || expected == null)
        {
            return false;
        }

        return target == expected || target.transform.IsChildOf(expected.transform);
    }

    private void SetActive(GameObject target, bool isActive)
    {
        if (target != null)
        {
            target.SetActive(isActive);
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1.0f;
    }
}
