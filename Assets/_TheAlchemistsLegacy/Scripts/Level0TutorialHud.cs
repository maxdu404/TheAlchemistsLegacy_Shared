using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level0TutorialHud : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool hideExistingCanvases = true;
    [SerializeField] private float raycastRange = 3.0f;

    [Header("Scene Objects")]
    [SerializeField] private string scrollName = "Scroll_MasterIntro";
    [SerializeField] private string chestName = "Chest_RuneContainer";
    [SerializeField] private string doorName = "Door_level0_exit";
    [SerializeField] private string firstSlotName = "StampSlot_1";
    [SerializeField] private string secondSlotName = "StampSlot_2";
    [SerializeField] private string fireBarrierName = "traps_fire";

    private const string CanvasName = "TheAlchemistsLegacy_Level0HUD";
    private const string TrialCompleteSceneName = "TrailComplete";

    private Canvas hudCanvas;
    private Text objectiveText;
    private Text promptText;
    private Text heldItemText;
    private GameObject promptPanel;
    private GameObject scrollPanel;
    private GameObject introPanel;

    private Camera playerCamera;
    private ItemPickup itemPickup;
    private ItemSlotController firstSlot;
    private ItemSlotController secondSlot;
    private ItemSlotController level1DoorSealSlot;
    private Level1PedestalController level1Pedestal;
    private Level1FurnaceController level1Furnace;
    private Level2TorchSequenceController level2Sequence;
    private GameObject level2GateObject;
    private GameObject scrollObject;
    private GameObject chestObject;
    private GameObject doorObject;
    private GameObject fireBarrierObject;
    private bool hasReadScroll;
    private bool isIntroOpen;
    private bool isScrollOpen;
    private Font defaultFont;
    private bool isLevel1Scene;
    private bool isLevel2Scene;

    private void Start()
    {
        ResumeRuntime();

        string sceneName = SceneManager.GetActiveScene().name;
        isLevel1Scene = sceneName == "Level1";
        isLevel2Scene = sceneName == "Level2";
        CacheReferences();

        if (hideExistingCanvases)
        {
            HideExistingCanvases();
        }

        BuildHud();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ReturnToTrialComplete();
            return;
        }

        CacheReferences();

        if (isIntroOpen)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                SetIntroOpen(false);
            }

            return;
        }

        if (isScrollOpen)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                SetScrollOpen(false);
            }

            return;
        }

        if (!isLevel1Scene && Input.GetMouseButtonDown(1) && IsLookingAt(scrollObject))
        {
            hasReadScroll = true;
            SetScrollOpen(true);
            return;
        }

        UpdateObjective();
        UpdateHeldItem();
        UpdatePrompt();
    }

    private void CacheReferences()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (itemPickup == null)
        {
            itemPickup = ItemPickup.instance != null ? ItemPickup.instance : FindObjectOfType<ItemPickup>();
        }

        if (isLevel1Scene)
        {
            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("Scroll2");
            }

            if (chestObject == null)
            {
                KeyChestController level1Chest = FindObjectOfType<KeyChestController>();
                chestObject = level1Chest != null ? level1Chest.gameObject : null;
            }

            if (doorObject == null)
            {
                doorObject = GameObject.Find("Door_level1_exit");
            }

            if (fireBarrierObject == null)
            {
                fireBarrierObject = GameObject.Find(fireBarrierName);
            }

            if (level1DoorSealSlot == null)
            {
                GameObject slotObject = GameObject.Find("DoorSealSlot_Level1");
                level1DoorSealSlot = slotObject != null ? slotObject.GetComponent<ItemSlotController>() : null;
            }

            if (level1Pedestal == null)
            {
                GameObject pedestalObject = GameObject.Find(firstSlotName);
                level1Pedestal = pedestalObject != null ? pedestalObject.GetComponent<Level1PedestalController>() : null;
            }

            if (level1Furnace == null)
            {
                level1Furnace = FindObjectOfType<Level1FurnaceController>();
            }

            return;
        }

        if (isLevel2Scene)
        {
            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("StoneTablet_Level2_Scroll");
            }

            if (doorObject == null)
            {
                doorObject = GameObject.Find("Door_l2_Exit");
            }

            if (fireBarrierObject == null)
            {
                fireBarrierObject = GameObject.Find("Traps_l2_exit");
            }

            if (level2GateObject == null)
            {
                level2GateObject = GameObject.Find("Gate_Level2_Exit");
            }

            if (level2Sequence == null)
            {
                level2Sequence = FindObjectOfType<Level2TorchSequenceController>();
            }

            return;
        }

        if (scrollObject == null)
        {
            scrollObject = GameObject.Find(scrollName);
        }

        if (chestObject == null)
        {
            chestObject = GameObject.Find(chestName);
        }

        if (doorObject == null)
        {
            doorObject = GameObject.Find(doorName);
        }

        if (fireBarrierObject == null)
        {
            fireBarrierObject = GameObject.Find(fireBarrierName);
        }

        if (firstSlot == null)
        {
            firstSlot = FindSlot(firstSlotName);
        }

        if (secondSlot == null)
        {
            secondSlot = FindSlot(secondSlotName);
        }
    }

    private ItemSlotController FindSlot(string objectName)
    {
        GameObject slotObject = GameObject.Find(objectName);
        return slotObject != null ? slotObject.GetComponent<ItemSlotController>() : null;
    }

    private void HideExistingCanvases()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas != null && canvas.name != CanvasName)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    private void BuildHud()
    {
        defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        GameObject canvasObject = new GameObject(CanvasName);
        hudCanvas = canvasObject.AddComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.sortingOrder = 50;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        BuildObjectivePanel(canvasObject.transform);
        BuildControlsPanel(canvasObject.transform);
        BuildPromptPanel(canvasObject.transform);
        BuildHeldItemPanel(canvasObject.transform);
        BuildCrosshair(canvasObject.transform);
        BuildScrollPanel(canvasObject.transform);
        BuildIntroPanel(canvasObject.transform);

        UpdateObjective();
        UpdateHeldItem();
        UpdatePrompt();
        SetIntroOpen(true);
    }

    private void BuildObjectivePanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "ObjectivePanel", new Color(0.05f, 0.045f, 0.04f, 0.78f));
        RectTransform rect = panel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.0f, 1.0f), new Vector2(0.0f, 1.0f), new Vector2(26.0f, -24.0f), new Vector2(650.0f, 108.0f), new Vector2(0.0f, 1.0f));

        string titleTextValue = isLevel2Scene ? "Flame Trial" : isLevel1Scene ? "Workshop Trial" : "Apprentice Trial";
        Text title = CreateText(panel.transform, "Title", titleTextValue, 28, new Color(1.0f, 0.86f, 0.52f), TextAnchor.UpperLeft);
        Anchor(title.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(18.0f, -14.0f), new Vector2(-36.0f, 36.0f), new Vector2(0.0f, 1.0f));

        objectiveText = CreateText(panel.transform, "ObjectiveText", "", 24, Color.white, TextAnchor.UpperLeft);
        Anchor(objectiveText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(18.0f, -54.0f), new Vector2(-36.0f, 44.0f), new Vector2(0.0f, 1.0f));
    }

    private void BuildControlsPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "ControlsPanel", new Color(0.05f, 0.045f, 0.04f, 0.70f));
        RectTransform rect = panel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(26.0f, 26.0f), new Vector2(1030.0f, 74.0f), new Vector2(0.0f, 0.0f));

        string controlsText = isLevel2Scene
            ? "WASD: Move    E: Pick up / drop    Right Click: Read or light    Y: Trial Complete"
            : "WASD: Move    E: Pick up / drop    Right Click: Read, open, or place    Y: Trial Complete";
        Text controls = CreateText(panel.transform, "ControlsText", controlsText, 20, Color.white, TextAnchor.MiddleLeft);
        Anchor(controls.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(18.0f, 0.0f), new Vector2(-36.0f, 0.0f), new Vector2(0.0f, 0.5f));
    }

    private void BuildPromptPanel(Transform parent)
    {
        promptPanel = CreatePanel(parent, "PromptPanel", new Color(0.05f, 0.045f, 0.04f, 0.82f));
        RectTransform rect = promptPanel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.0f), new Vector2(0.5f, 0.0f), new Vector2(0.0f, 116.0f), new Vector2(780.0f, 68.0f), new Vector2(0.5f, 0.0f));

        promptText = CreateText(promptPanel.transform, "PromptText", "", 26, Color.white, TextAnchor.MiddleCenter);
        Anchor(promptText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Vector2(0.5f, 0.5f));
    }

    private void BuildHeldItemPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "HeldItemPanel", new Color(0.05f, 0.045f, 0.04f, 0.70f));
        RectTransform rect = panel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(1.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(-90.0f, -24.0f), new Vector2(470.0f, 62.0f), new Vector2(1.0f, 1.0f));

        heldItemText = CreateText(panel.transform, "HeldItemText", "", 22, Color.white, TextAnchor.MiddleCenter);
        Anchor(heldItemText.rectTransform, Vector2.zero, Vector2.one, new Vector2(0.0f, 0.0f), new Vector2(-36.0f, 0.0f), new Vector2(0.5f, 0.5f));
    }

    private void BuildCrosshair(Transform parent)
    {
        GameObject root = new GameObject("Crosshair");
        root.transform.SetParent(parent, false);
        RectTransform rect = root.AddComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(26.0f, 26.0f), new Vector2(0.5f, 0.5f));

        CreateLine(root.transform, "Horizontal", new Vector2(18.0f, 2.0f));
        CreateLine(root.transform, "Vertical", new Vector2(2.0f, 18.0f));
    }

    private void BuildScrollPanel(Transform parent)
    {
        scrollPanel = CreatePanel(parent, "ScrollPanel", new Color(0.87f, 0.78f, 0.58f, 0.96f));
        RectTransform rect = scrollPanel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900.0f, 560.0f), new Vector2(0.5f, 0.5f));

        Text title = CreateText(scrollPanel.transform, "ScrollTitle", GetScrollTitle(), 36, new Color(0.14f, 0.08f, 0.03f), TextAnchor.UpperCenter);
        Anchor(title.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(40.0f, -34.0f), new Vector2(-80.0f, 56.0f), new Vector2(0.5f, 1.0f));

        Text body = CreateText(scrollPanel.transform, "ScrollBody",
            GetScrollBody(),
            28,
            new Color(0.12f, 0.07f, 0.03f),
            TextAnchor.UpperLeft);
        body.lineSpacing = 1.08f;
        Anchor(body.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(70.0f, -116.0f), new Vector2(-140.0f, 310.0f), new Vector2(0.0f, 1.0f));

        Text close = CreateText(scrollPanel.transform, "ScrollClose", "Right Click / E  Close", 22, new Color(0.16f, 0.10f, 0.05f), TextAnchor.LowerCenter);
        Anchor(close.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 28.0f), new Vector2(-80.0f, 36.0f), new Vector2(0.5f, 0.0f));

        scrollPanel.SetActive(false);
    }

    private void BuildIntroPanel(Transform parent)
    {
        introPanel = CreatePanel(parent, "IntroPanel", new Color(0.87f, 0.78f, 0.58f, 0.96f));
        RectTransform rect = introPanel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900.0f, 560.0f), new Vector2(0.5f, 0.5f));

        string introTitle = "A Letter from the Master";
        string introBody = GetIntroBody();

        Text title = CreateText(introPanel.transform, "IntroTitle", introTitle, 36, new Color(0.14f, 0.08f, 0.03f), TextAnchor.UpperCenter);
        Anchor(title.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(40.0f, -34.0f), new Vector2(-80.0f, 56.0f), new Vector2(0.5f, 1.0f));

        Text body = CreateText(introPanel.transform, "IntroBody", introBody, 27, new Color(0.12f, 0.07f, 0.03f), TextAnchor.UpperLeft);
        body.lineSpacing = 1.08f;
        Anchor(body.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(70.0f, -116.0f), new Vector2(-140.0f, 310.0f), new Vector2(0.0f, 1.0f));

        Text close = CreateText(introPanel.transform, "IntroClose", "Right Click / E  Begin", 22, new Color(0.16f, 0.10f, 0.05f), TextAnchor.LowerCenter);
        Anchor(close.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 28.0f), new Vector2(-80.0f, 36.0f), new Vector2(0.5f, 0.0f));

        introPanel.SetActive(false);
    }

    private string GetScrollTitle()
    {
        return isLevel2Scene ? "Stone Tablet" : "Master's Note";
    }

    private string GetScrollBody()
    {
        if (isLevel2Scene)
        {
            return "Three waiting flames remember the way.\n\nFirst, wake the light nearest your first shelter.\nThen seek the flame watched by the quiet wall.\nLast, carry the memory to the black gate.";
        }

        return "Apprentice,\n\nTwo seals wake the old door.\nOne rests in the open. One waits inside the chest.\n\nLet the first seal answer the left hand of the room.\nLet the second complete the right.\n\nCross only when the fire fades.";
    }

    private string GetIntroBody()
    {
        if (isLevel2Scene)
        {
            return "Apprentice,\n\nMemory is a flame passed from hand to hand. A torch waits in your first shelter, but the walls will answer only in order.\n\nRead the stone, carry the flame, and let the last light open the black gate.";
        }

        if (isLevel1Scene)
        {
            return "Apprentice,\n\nThree quiet seals lie where habit seldom lingers. One waits beneath a lid, while two keep to the room's far edges.\n\nOffer them to the basin at the workshop's heart, and the sleeping forge will remember its breath.";
        }

        return "Apprentice,\n\nYou wake inside the old workshop. The master has left a final trial: learn the room, recover two stamps, and prove you can follow the marks of the craft.\n\nRead the note on the table first. It explains how to open the way out.";
    }

    private void UpdateObjective()
    {
        if (isLevel2Scene)
        {
            bool pathOpen = level2Sequence != null && level2Sequence.ExitUnlocked;
            objectiveText.text = pathOpen
                ? "Goal: the gate is open. Leave the castle."
                : "Goal: carry the torch and wake the three flames.";
            return;
        }

        if (isLevel1Scene)
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            objectiveText.text = pathOpen
                ? "Goal: the way is clear. Leave the workshop."
                : "Goal: wake the forge and find the quiet way out.";
            return;
        }

        bool firstDone = firstSlot != null && firstSlot.IsFilled;
        bool secondDone = secondSlot != null && secondSlot.IsFilled;

        objectiveText.text = firstDone && secondDone
            ? "Goal: exit the room."
            : "Goal: place both stamps, then exit.";
    }

    private void UpdateHeldItem()
    {
        if (heldItemText == null)
        {
            return;
        }

        if (itemPickup != null && itemPickup.isHoldingItem && itemPickup.currentItem != null)
        {
            heldItemText.text = "Holding: " + GetFriendlyItemName(itemPickup.currentItem);
        }
        else
        {
            heldItemText.text = "Hands empty";
        }
    }

    private void UpdatePrompt()
    {
        string message = GetLookPrompt();
        bool hasPrompt = !string.IsNullOrWhiteSpace(message);

        promptPanel.SetActive(hasPrompt);
        promptText.text = message;
    }

    private string GetLookPrompt()
    {
        if (!TryLook(out RaycastHit hit))
        {
            return "";
        }

        if (isLevel2Scene)
        {
            return GetLevel2LookPrompt(hit);
        }

        return isLevel1Scene ? GetLevel1LookPrompt(hit) : GetLevel0LookPrompt(hit);
    }

    private string GetLevel0LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;

        if (Matches(target, scrollObject))
        {
            return "Right Click: Read the note";
        }

        if (Matches(target, chestObject))
        {
            return "Right Click: Open the chest";
        }

        if (Matches(target, fireBarrierObject) || IsFireOrTrapTarget(target))
        {
            bool ready = firstSlot != null && firstSlot.IsFilled && secondSlot != null && secondSlot.IsFilled;
            return ready ? "The fire has faded. Go to the door." : "Find both stamps and place them in the bowls before leaving.";
        }

        ItemSlotController slot = target.GetComponentInParent<ItemSlotController>();
        if (slot != null)
        {
            if (slot.IsFilled)
            {
                return "This seal is complete";
            }

            if (itemPickup != null && itemPickup.isHoldingItem && itemPickup.currentItem != null)
            {
                return "Right Click: Place " + GetFriendlyItemName(itemPickup.currentItem);
            }

            return "Find the matching seal first";
        }

        if (hit.collider.CompareTag("Pickup"))
        {
            return "E: Pick up " + GetFriendlyItemName(hit.collider.gameObject);
        }

        if (Matches(target, doorObject))
        {
            bool ready = firstSlot != null && firstSlot.IsFilled && secondSlot != null && secondSlot.IsFilled;
            return ready ? "Walk forward to finish the trial" : "Two seals are required";
        }

        return "";
    }

    private string GetLevel1LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;

        Level1ScrollReader reader = target.GetComponentInParent<Level1ScrollReader>();
        if (reader != null)
        {
            return reader.IsReadable ? "Right Click: Read the note" : "";
        }

        if (target.GetComponentInParent<KeyChestController>() != null)
        {
            return "Right Click: Lift the lid";
        }

        if (target.GetComponentInParent<Level1PedestalController>() != null)
        {
            if (itemPickup != null && itemPickup.isHoldingItem)
            {
                return "Right Click: Offer what you carry";
            }

            return "Three quiet seals may wake the forge.";
        }

        if (target.GetComponentInParent<Level1FurnaceController>() != null)
        {
            if (itemPickup != null && itemPickup.isHoldingItem)
            {
                return "Right Click: Feed the forge";
            }

            return "The forge waits for timber and iron.";
        }

        ItemSlotController slot = target.GetComponentInParent<ItemSlotController>();
        if (slot != null)
        {
            if (slot.IsFilled)
            {
                return "The mark now rests in place.";
            }

            if (itemPickup != null && itemPickup.isHoldingItem)
            {
                return "Right Click: Set the forged mark";
            }

            return "A finished mark belongs here.";
        }

        if (Matches(target, fireBarrierObject) || IsFireOrTrapTarget(target))
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            return pathOpen ? "The flames have bowed. The way is yours." : "The flames still refuse the way.";
        }

        if (hit.collider.CompareTag("Pickup"))
        {
            return GetPickupPrompt(hit.collider.gameObject);
        }

        if (Matches(target, doorObject) || target.name.ToLowerInvariant().Contains("door"))
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            return pathOpen ? "Walk on to finish the trial" : "The way is not yet quiet.";
        }

        return "";
    }

    private string GetLevel2LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;

        if (Matches(target, scrollObject))
        {
            return "Right Click: Read the stone";
        }

        Level2TorchPoint torch = target.GetComponentInParent<Level2TorchPoint>();
        if (torch != null)
        {
            if (torch.IsLit)
            {
                return "This flame remembers.";
            }

            if (itemPickup != null && itemPickup.isHoldingItem && itemPickup.currentItem != null)
            {
                return "Right Click: Wake the flame";
            }

            return "Carry the torch before waking this flame.";
        }

        if (Matches(target, fireBarrierObject) || IsFireOrTrapTarget(target) || Matches(target, level2GateObject))
        {
            bool pathOpen = level2Sequence != null && level2Sequence.ExitUnlocked;
            return pathOpen ? "The black gate is open." : "The last flame has not answered yet.";
        }

        if (hit.collider.CompareTag("Pickup"))
        {
            return GetPickupPrompt(hit.collider.gameObject);
        }

        if (Matches(target, doorObject) || target.name.ToLowerInvariant().Contains("door"))
        {
            bool pathOpen = level2Sequence != null && level2Sequence.ExitUnlocked;
            return pathOpen ? "Walk forward to finish the trial" : "Wake the three flames first.";
        }

        return "";
    }

    private string GetPickupPrompt(GameObject item)
    {
        string lowerName = item.name.ToLowerInvariant();

        if (lowerName.Contains("torch_level2"))
        {
            return "E: Take the torch";
        }

        if (lowerName.Contains("finalseal"))
        {
            return "E: Take the forged seal";
        }

        if (lowerName.Contains("stamp"))
        {
            return "E: Gather the seal";
        }

        if (lowerName.Contains("wood") || lowerName.Contains("firewood") || lowerName.Contains("timber"))
        {
            return "E: Gather the timber";
        }

        if (lowerName.Contains("metal") || lowerName.Contains("iron") || lowerName.Contains("sawblade"))
        {
            return "E: Gather the iron";
        }

        return "E: Take it";
    }

    private string GetFriendlyItemName(GameObject item)
    {
        if (item == null)
        {
            return "item";
        }

        string lowerName = item.name.ToLowerInvariant();

        if (lowerName.Contains("torch_level2"))
        {
            return "Torch";
        }

        if (lowerName.Contains("finalseal"))
        {
            return "Forged Seal";
        }

        if (lowerName.Contains("stamp"))
        {
            return "Seal";
        }

        if (lowerName.Contains("wood") || lowerName.Contains("firewood") || lowerName.Contains("timber"))
        {
            return "Dry Timber";
        }

        if (lowerName.Contains("metal") || lowerName.Contains("iron") || lowerName.Contains("sawblade"))
        {
            return "Cold Iron";
        }

        return item.name.Replace("(Clone)", "").Trim();
    }

    private bool IsLookingAt(GameObject targetObject)
    {
        return targetObject != null && TryLook(out RaycastHit hit) && Matches(hit.collider.transform, targetObject);
    }

    private bool TryLook(out RaycastHit hit)
    {
        hit = default;
        if (playerCamera == null)
        {
            return false;
        }

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        return Physics.Raycast(ray, out hit, raycastRange);
    }

    private bool Matches(Transform transformToCheck, GameObject targetObject)
    {
        return targetObject != null
            && (transformToCheck == targetObject.transform || transformToCheck.IsChildOf(targetObject.transform));
    }

    private bool IsFireOrTrapTarget(Transform target)
    {
        while (target != null)
        {
            string lowerName = target.name.ToLowerInvariant();
            if (lowerName.Contains("fire") || lowerName.Contains("trap"))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
    }

    private void SetScrollOpen(bool isOpen)
    {
        isScrollOpen = isOpen;
        scrollPanel.SetActive(isOpen);
        promptPanel.SetActive(!isOpen && !isIntroOpen);
    }

    private void SetIntroOpen(bool isOpen)
    {
        isIntroOpen = isOpen;
        introPanel.SetActive(isOpen);
        promptPanel.SetActive(!isOpen && !isScrollOpen);
    }

    private void ReturnToTrialComplete()
    {
        ResumeRuntime();
        SceneManager.LoadScene(TrialCompleteSceneName);
    }

    private void ResumeRuntime()
    {
        Time.timeScale = 1.0f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = false;
#endif
    }

    private GameObject CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panel = new GameObject(objectName);
        panel.transform.SetParent(parent, false);
        Image image = panel.AddComponent<Image>();
        image.color = color;
        return panel;
    }

    private Text CreateText(Transform parent, string objectName, string content, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        Text text = textObject.AddComponent<Text>();
        text.font = defaultFont;
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;

        return text;
    }

    private void CreateLine(Transform parent, string objectName, Vector2 size)
    {
        GameObject line = CreatePanel(parent, objectName, new Color(1.0f, 1.0f, 1.0f, 0.8f));
        RectTransform rect = line.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, size, new Vector2(0.5f, 0.5f));
    }

    private void Anchor(RectTransform rect, Vector2 min, Vector2 max, Vector2 position, Vector2 size, Vector2 pivot)
    {
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.pivot = pivot;
    }
}
