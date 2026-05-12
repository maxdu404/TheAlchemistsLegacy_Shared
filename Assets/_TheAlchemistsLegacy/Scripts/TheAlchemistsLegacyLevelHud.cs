using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheAlchemistsLegacyLevelHud : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool hideExistingCanvases = true;
    [SerializeField] private float raycastRange = 3.0f;
    [SerializeField] private float aimAssistRadius = 0.65f;

    [Header("Scene Objects")]
    [SerializeField] private string scrollName = "Scroll_MasterIntro";
    [SerializeField] private string chestName = "Chest_RuneContainer";
    [SerializeField] private string doorName = "Door_level0_exit";
    [SerializeField] private string firstSlotName = "StampSlot_1";
    [SerializeField] private string secondSlotName = "StampSlot_2";
    [SerializeField] private string fireBarrierName = "traps_fire";

    private const string CanvasName = "TheAlchemistsLegacy_LevelHUD";
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
    private Level3FurnaceCrafting level3Furnace;
    private Level3ExitKeySlot level3ExitSlot;
    private GameObject level2GateObject;
    private GameObject scrollObject;
    private GameObject chestObject;
    private GameObject doorObject;
    private GameObject fireBarrierObject;
    private bool isIntroOpen;
    private bool isScrollOpen;
    private Font defaultFont;
    private Text scrollTitleText;
    private Text scrollBodyText;
    private bool isLevel1Scene;
    private bool isLevel2Scene;
    private bool isLevel3Scene;
    private bool isLevel4Scene;

    private void Start()
    {
        ResumeRuntime();

        string sceneName = SceneManager.GetActiveScene().name;
        isLevel1Scene = sceneName == "Level1";
        isLevel2Scene = sceneName == "Level2";
        isLevel3Scene = sceneName == "Level3";
        isLevel4Scene = sceneName == "Level4";
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

        if (!isLevel1Scene
            && Input.GetMouseButtonDown(1)
            && (IsLookingAt(scrollObject) || (isLevel2Scene && IsLookingAtLevel2Clue()) || (isLevel4Scene && IsLookingAtLevel4Clue())))
        {
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

            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("StoneTablet_Level2");
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

        if (isLevel3Scene)
        {
            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("scroll_l3");
            }

            if (doorObject == null)
            {
                doorObject = GameObject.Find("Door_l3_exit");
            }

            if (fireBarrierObject == null)
            {
                fireBarrierObject = GameObject.Find("Traps_l3_exit");
            }

            if (level3Furnace == null)
            {
                level3Furnace = FindObjectOfType<Level3FurnaceCrafting>();
            }

            if (level3ExitSlot == null)
            {
                level3ExitSlot = FindObjectOfType<Level3ExitKeySlot>();
            }

            return;
        }

        if (isLevel4Scene)
        {
            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("scroll_level");
            }

            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("Scroll_Level");
            }

            if (scrollObject == null)
            {
                scrollObject = GameObject.Find("Central Tablet");
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
        GameObject panel = CreatePanel(parent, "ObjectivePanel", new Color(0.05f, 0.045f, 0.04f, 0.60f));
        RectTransform rect = panel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.0f, 1.0f), new Vector2(0.0f, 1.0f), new Vector2(32.0f, -28.0f), new Vector2(600.0f, 108.0f), new Vector2(0.0f, 1.0f));

        string titleTextValue = isLevel4Scene
            ? "Legacy Trial"
            : isLevel3Scene
                ? "Lantern Trial"
                : isLevel2Scene ? "Flame Trial" : isLevel1Scene ? "Workshop Trial" : "Apprentice Trial";
        Text title = CreateText(panel.transform, "Title", titleTextValue, 28, new Color(1.0f, 0.86f, 0.52f), TextAnchor.UpperLeft);
        Anchor(title.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(18.0f, -14.0f), new Vector2(-36.0f, 36.0f), new Vector2(0.0f, 1.0f));

        objectiveText = CreateText(panel.transform, "ObjectiveText", "", 24, Color.white, TextAnchor.UpperLeft);
        Anchor(objectiveText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(18.0f, -54.0f), new Vector2(-36.0f, 44.0f), new Vector2(0.0f, 1.0f));
    }

    private void BuildControlsPanel(Transform parent)
    {
        GameObject panel = CreatePanel(parent, "ControlsPanel", new Color(0.05f, 0.045f, 0.04f, 0.48f));
        RectTransform rect = panel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(32.0f, 32.0f), new Vector2(780.0f, 78.0f), new Vector2(0.0f, 0.0f));

        string controlsText = "「WASD」 Move   「Mouse」 Look   「Shift」 Sprint\n「E」 Pick up / Drop   「RMB」 Interact   「R」 Recall Item   「Y」 Skip to Hub";
        Text controls = CreateText(panel.transform, "ControlsText", controlsText, 19, Color.white, TextAnchor.MiddleLeft);
        controls.lineSpacing = 1.25f;
        Anchor(controls.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 1.0f), new Vector2(22.0f, 0.0f), new Vector2(-44.0f, -14.0f), new Vector2(0.0f, 0.5f));
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

        scrollTitleText = CreateText(scrollPanel.transform, "ScrollTitle", GetScrollTitle(), 36, new Color(0.14f, 0.08f, 0.03f), TextAnchor.UpperCenter);
        Anchor(scrollTitleText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(40.0f, -34.0f), new Vector2(-80.0f, 56.0f), new Vector2(0.5f, 1.0f));

        scrollBodyText = CreateText(scrollPanel.transform, "ScrollBody",
            GetScrollBody(),
            28,
            new Color(0.12f, 0.07f, 0.03f),
            TextAnchor.UpperLeft);
        scrollBodyText.lineSpacing = 1.08f;
        Anchor(scrollBodyText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(70.0f, -116.0f), new Vector2(-140.0f, 310.0f), new Vector2(0.0f, 1.0f));

        Text close = CreateText(scrollPanel.transform, "ScrollClose", "Right Click / E  Close", 22, new Color(0.16f, 0.10f, 0.05f), TextAnchor.LowerCenter);
        Anchor(close.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 28.0f), new Vector2(-80.0f, 36.0f), new Vector2(0.5f, 0.0f));

        scrollPanel.SetActive(false);
    }

    private void BuildIntroPanel(Transform parent)
    {
        introPanel = CreatePanel(parent, "IntroPanel", new Color(0.87f, 0.78f, 0.58f, 0.96f));
        RectTransform rect = introPanel.GetComponent<RectTransform>();
        Anchor(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900.0f, 560.0f), new Vector2(0.5f, 0.5f));

        string introTitle = isLevel4Scene ? "The Final Legacy" : "A Letter from the Master";
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
        return isLevel4Scene ? "Lamp Trial Note" : isLevel3Scene ? "The Master's Lantern" : isLevel2Scene ? "Stone Tablet" : "Master's Note";
    }

    private string GetScrollBody()
    {
        if (isLevel2Scene)
        {
            return "Three waiting flames remember the way.\n\nFirst, wake the light nearest your birth house.\nThen seek the flame within the castle walls.\nLast, carry the memory to the great gate.";
        }

        if (isLevel3Scene)
        {
            return "Three parts wake the master's lamp in the furnace.\n\nOil, base, and wick are needed, but fragile things are rarely left in the open.\n\nPay attention to the stable and the orange tent.\n\nWhen the parts are ready, bring them back to the castle furnace.";
        }

        if (isLevel4Scene)
        {
            return "Three tables hold three flames.\n\nThe dark left table points to the highest room.\nThe pale right table points to the middle room.\nThe plain center table holds a dead lamp.\n\nChoose the lamp by the table beneath it, then carry that light to the matching door.";
        }

        return "Apprentice,\n\nTwo seals wake the old door.\nOne rests in the open. One waits inside the chest.\n\nLet the first seal answer the left hand of the room.\nLet the second complete the right.\n\nCross only when the fire fades.";
    }

    private string GetIntroBody()
    {
        if (isLevel2Scene)
        {
            return "Apprentice,\n\nThis trial begins inside the birth house. Take the waiting torch before you leave.\n\nThen read the scroll here, carry the flame in order, and let the last light open the black gate.";
        }

        if (isLevel1Scene)
        {
            return "Apprentice,\n\nLevel 1 asks you to wake the forge. Place the three stamps on the pedestal first.\n\nWhen the forge wakes, bring wood and metal to the forge. The furnace will create a sword.\n\nSacrifice the sword at the old door to clear the exit.";
        }

        if (isLevel3Scene)
        {
            return "Apprentice,\n\nThis third trial begins in the birth house. One light inside does not belong with the others. Take it to open the way out, then read the master's note.\n\nGather the lamp parts, craft the lantern, and carry it upward to recover the exit key.";
        }

        if (isLevel4Scene)
        {
            return "Apprentice,\n\nThis is the final trial. The castle will not open for greed; it opens for one who restores what was left behind.\n\nBegin outside. Make ash from the old firewood, find the key to the salt chest, and return both offerings to their signs. When the main doors yield, read the lamp note inside.\n\nThe lamps lead to the remaining offerings: a yellow stone above the ice and a blue drop at the top of the castle. Bring all four materials back to the altar to restore the legacy.";
        }

        return "Apprentice,\n\nYou wake inside the old workshop. The master has left your first trial: learn the room, recover two stamps, and prove you can follow the marks of the craft.\n\nRead the note on the table first. It explains how to open the way out.";
    }

    private void UpdateObjective()
    {
        if (isLevel2Scene)
        {
            bool birthDoorOpen = (level2Sequence != null && level2Sequence.BirthDoorUnlocked) || IsHoldingItem("torch_level2");
            bool pathOpen = level2Sequence != null && level2Sequence.ExitUnlocked;
            objectiveText.text = pathOpen
                ? "Goal: the gate is open. Leave the castle."
                : birthDoorOpen
                    ? "Goal: carry the torch and wake the three flames."
                    : "Goal: take the torch before leaving the birth house.";
            return;
        }

        if (isLevel1Scene)
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            bool forgeAwake = level1Pedestal != null && level1Pedestal.IsUnlocked;
            bool finalSealReady = level1Furnace != null && level1Furnace.HasProducedSeal;

            objectiveText.text = pathOpen
                ? "Goal: the way is clear. Leave Level 1."
                : finalSealReady
                    ? "Goal: sacrifice the sword at the old door."
                    : forgeAwake
                        ? "Goal: bring " + GetMissingLevel1FurnaceItems(level1Furnace) + " to the forge."
                        : "Goal: place the three stamps to wake the forge.";
            return;
        }

        if (isLevel3Scene)
        {
            bool exitOpen = level3ExitSlot != null && level3ExitSlot.IsFilled;
            bool lampCrafted = level3Furnace != null && level3Furnace.IsCrafted;
            objectiveText.text = exitOpen
                ? "Goal: the exit is open. Leave Level 3."
                : lampCrafted
                    ? "Goal: carry the lamp upstairs and find the exit key."
                    : "Goal: bring " + GetMissingFurnaceIngredients(level3Furnace) + " to the furnace.";
            return;
        }

        if (isLevel4Scene)
        {
            if (IsHoldingItem("forge_tool"))
            {
                objectiveText.text = "Goal: use the forge tool on the old firewood.";
                return;
            }

            if (IsHoldingItem("Key_l4_salt"))
            {
                objectiveText.text = "Goal: unlock the salt chest.";
                return;
            }

            if (IsHoldingItem("torch_for_ice_l2"))
            {
                objectiveText.text = "Goal: melt the ice around the yellow stone.";
                return;
            }

            if (IsHoldingItem("Ash_l4") || IsHoldingItem("Salt_l4") || IsHoldingItem("YellowStone_l4") || IsHoldingItem("BlueDrop_l4"))
            {
                objectiveText.text = "Goal: place this offering on its matching sign.";
                return;
            }

            if (IsHoldingItem("LegacySeal_l4"))
            {
                objectiveText.text = "Goal: place the Legacy Seal in the castle-wall gate.";
                return;
            }

            if (IsHoldingItem("lamp_2th") || IsHoldingItem("lamp_3th") || IsHoldingItem("lamp_no"))
            {
                objectiveText.text = "Goal: carry this lamp to the door described by the note.";
                return;
            }

            objectiveText.text = "Goal: restore the four offerings: fire ash, salt, yellow stone, and blue drop.";
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

        if (isLevel3Scene)
        {
            return GetLevel3LookPrompt(hit);
        }

        if (isLevel4Scene)
        {
            return GetLevel4LookPrompt(hit);
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
            return reader.IsReadable ? "Right Click: Read the Level 1 note" : "Wake the forge first.";
        }

        if (target.GetComponentInParent<KeyChestController>() != null)
        {
            return "Right Click: Open the chest";
        }

        Level1PedestalController pedestal = target.GetComponentInParent<Level1PedestalController>();
        if (pedestal != null)
        {
            if (pedestal.IsUnlocked)
            {
                return "The forge is awake.";
            }

            return IsHoldingLevel1Stamp()
                ? "Right Click: Place this stamp"
                : "Place the three stamps here.";
        }

        Level1FurnaceController furnace = target.GetComponentInParent<Level1FurnaceController>();
        if (furnace != null)
        {
            if (furnace.HasProducedSeal)
            {
                return "The sword is ready.";
            }

            return IsHoldingNeededLevel1FurnaceItem(furnace)
                ? "Right Click: Add this furnace material"
                : "Still needed: " + GetMissingLevel1FurnaceItems(furnace) + ".";
        }

        ItemSlotController slot = target.GetComponentInParent<ItemSlotController>();
        if (slot != null)
        {
            if (slot.IsFilled)
            {
                return "The sword is sacrificed.";
            }

            return IsHoldingLevel1Sword()
                ? "Right Click: Sacrifice the sword"
                : "This place needs the sword.";
        }

        string level1PickupPrompt = GetLevel1PickupPrompt(target);
        if (!string.IsNullOrEmpty(level1PickupPrompt))
        {
            return level1PickupPrompt;
        }

        if (hit.collider.CompareTag("Pickup"))
        {
            return GetPickupPrompt(hit.collider.gameObject);
        }

        if (Matches(target, fireBarrierObject) || IsFireOrTrapTarget(target))
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            return pathOpen ? "The exit is clear." : "Sacrifice the sword before crossing the fire.";
        }

        if (Matches(target, doorObject) || target.name.ToLowerInvariant().Contains("door"))
        {
            bool pathOpen = level1DoorSealSlot != null && level1DoorSealSlot.IsFilled;
            return pathOpen ? "Walk forward to complete Level 1" : "This door still needs the sword.";
        }

        return "";
    }

    private string GetLevel2LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;
        string lowerTargetName = GetLowerHierarchyName(target);

        if (Matches(target, scrollObject) || IsLevel2ClueTarget(lowerTargetName))
        {
            return "Right Click: Read the stone tablet";
        }

        if (IsBirthHouseDoor(target))
        {
            bool birthDoorOpen = (level2Sequence != null && level2Sequence.BirthDoorUnlocked) || IsHoldingItem("torch_level2");
            return birthDoorOpen
                ? "The birth-house door is open."
                : "Take the torch before leaving the birth house.";
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


    private string GetLevel3LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;

        if (Matches(target, scrollObject))
        {
            return "Right Click: Read the lantern note";
        }

        if (target.GetComponentInParent<Level3TorchGiver>() != null)
        {
            return "Right Click: Take the unusual light";
        }

        KeyChestController chest = target.GetComponentInParent<KeyChestController>();
        if (chest != null)
        {
            if (chest.IsOpen)
            {
                return "The chest is open.";
            }

            if (!chest.RequiresKey)
            {
                return "Right Click: Open the chest";
            }

            return IsHoldingItem(chest.KeyItemName)
                ? "Right Click: Unlock the chest"
                : "This chest needs a key.";
        }

        if (target.GetComponentInParent<Level3FurnaceCrafting>() != null)
        {
            Level3FurnaceCrafting furnace = target.GetComponentInParent<Level3FurnaceCrafting>();
            if (furnace.IsCrafted)
            {
                return "The lamp is ready.";
            }

            return IsHoldingNeededLampPart(furnace)
                ? "Right Click: Add this lamp part"
                : "Still needed: " + GetMissingFurnaceIngredients(furnace) + ".";
        }

        if (target.GetComponentInParent<Level3IceMelt>() != null)
        {
            bool holdingTorch = IsHoldingItem("torch_for_ice_l3");
            return holdingTorch ? "Right Click: Melt the ice" : "A fire torch could melt this ice.";
        }

        if (IsBirthHouseDoor(target))
        {
            return IsHoldingItem("torch_for_ice_l3")
                ? "The birth-house door is open."
                : "Take the unusual birth-house light before leaving.";
        }

        Level3DoorTeleporter teleporter = target.GetComponentInParent<Level3DoorTeleporter>();
        if (teleporter != null)
        {
            string lowerName = teleporter.gameObject.name.ToLowerInvariant();
            if (lowerName.Contains("1th"))
            {
                bool holdingLamp = IsHoldingItem("lamp_l3");
                return holdingLamp ? "Right Click: Carry the lamp upstairs" : "Craft and hold the lamp before using this door.";
            }

            return "Right Click: Return to the first floor";
        }

        Level3ExitKeySlot exitSlot = target.GetComponentInParent<Level3ExitKeySlot>();
        if (exitSlot != null)
        {
            if (exitSlot.IsFilled)
            {
                return "The exit key is set.";
            }

            return itemPickup != null && itemPickup.isHoldingItem
                ? "Right Click: Place the exit key"
                : "The exit slot needs the third-floor key.";
        }

        string level3PickupPrompt = GetLevel3PickupPrompt(target);
        if (!string.IsNullOrEmpty(level3PickupPrompt))
        {
            return level3PickupPrompt;
        }

        if (hit.collider.CompareTag("Pickup"))
        {
            return GetPickupPrompt(hit.collider.gameObject);
        }

        if (Matches(target, fireBarrierObject) || IsFireOrTrapTarget(target))
        {
            bool exitOpen = level3ExitSlot != null && level3ExitSlot.IsFilled;
            return exitOpen ? "The exit path is clear." : "Place the exit key in the slot before leaving.";
        }

        if (Matches(target, doorObject) || target.name.ToLowerInvariant().Contains("door"))
        {
            bool exitOpen = level3ExitSlot != null && level3ExitSlot.IsFilled;
            return exitOpen ? "Walk forward to complete Level 3" : "You cannot leave until the exit key is placed.";
        }

        return "";
    }

    private string GetLevel4LookPrompt(RaycastHit hit)
    {
        Transform target = hit.collider.transform;
        string lowerTargetName = GetLowerHierarchyName(target);

        if ((lowerTargetName.Contains("central") && lowerTargetName.Contains("tablet")) || lowerTargetName.Contains("scroll_level"))
        {
            return "Right Click: Read the clue";
        }

        Level4AshFromFirewood ashFromFirewood = target.GetComponentInParent<Level4AshFromFirewood>();
        if (ashFromFirewood != null)
        {
            if (ashFromFirewood.IsBurned)
            {
                return "Ash is ready below the fire sign.";
            }

            return IsHoldingItem(ashFromFirewood.RequiredToolName)
                ? "Right Click: Burn the firewood into ash"
                : "Find the forge tool first.";
        }

        KeyChestController saltChest = target.GetComponentInParent<KeyChestController>();
        if (saltChest != null && lowerTargetName.Contains("salt"))
        {
            if (saltChest.IsOpen)
            {
                return "The salt chest is open.";
            }

            return IsHoldingItem("Key_l4_salt")
                ? "Right Click: Unlock the salt chest"
                : "Find the small key near the market stall.";
        }

        Level3TorchGiver torchGiver = target.GetComponentInParent<Level3TorchGiver>();
        if (torchGiver != null && lowerTargetName.Contains("candle"))
        {
            return itemPickup != null && itemPickup.isHoldingItem
                ? "Free your hands before taking a torch."
                : "Right Click: Take a lit torch";
        }

        Level3IceMelt iceMelt = target.GetComponentInParent<Level3IceMelt>();
        if (iceMelt != null || IsLevel4IceTarget(lowerTargetName))
        {
            return IsHoldingItem("torch_for_ice_l2")
                ? "Right Click: Melt the ice"
                : "A lit torch could melt this ice.";
        }

        Level4LampDoorTeleporter lampDoor = target.GetComponentInParent<Level4LampDoorTeleporter>();
        if (lampDoor != null)
        {
            string requiredLamp = lampDoor.RequiredHeldItemName;
            if (string.IsNullOrWhiteSpace(requiredLamp))
            {
                return "Right Click: Enter";
            }

            return IsHoldingItem(requiredLamp)
                ? "Right Click: Carry " + GetFriendlyLampName(requiredLamp) + " through this door"
                : "This door needs " + GetFriendlyLampName(requiredLamp) + ".";
        }

        if (lowerTargetName.Contains("lamp_2th") || lowerTargetName.Contains("lamp_3th") || lowerTargetName.Contains("lamp_no"))
        {
            string lampPickupPrompt = GetLevel4PickupPrompt(target);
            return !string.IsNullOrEmpty(lampPickupPrompt) ? lampPickupPrompt : "Choose the lamp that matches the door clue.";
        }

        if (lowerTargetName.Contains("lantern_l4") || lowerTargetName.Contains("lantern"))
        {
            return hit.collider.CompareTag("Pickup") ? "E: Take the master's lantern" : "The lantern reveals marks hidden in dark stone.";
        }

        if (lowerTargetName.Contains("dark") && lowerTargetName.Contains("pedestal"))
        {
            return IsHoldingItem("lantern_l4") || IsHoldingItem("lantern")
                ? "Right Click: Reveal the hidden mark"
                : "Carry the lantern to read this dark stone.";
        }

        if (lowerTargetName.Contains("sealpillar") || (lowerTargetName.Contains("seal") && lowerTargetName.Contains("pillar")))
        {
            return "Right Click: Set the seal order";
        }

        if (lowerTargetName.Contains("final") && lowerTargetName.Contains("door"))
        {
            return "The final gate waits for the restored seal.";
        }

        ItemSlotController slot = target.GetComponentInParent<ItemSlotController>();
        if (slot != null)
        {
            string requiredItemName = GetFriendlySlotRequirement(slot.AcceptedItemName);
            if (slot.IsFilled)
            {
                return requiredItemName + " is placed.";
            }

            if (itemPickup != null && itemPickup.isHoldingItem && itemPickup.currentItem != null)
            {
                return IsHoldingItem(slot.AcceptedItemName)
                    ? "Right Click: Place " + requiredItemName + " here"
                    : "This slot needs " + requiredItemName + ".";
            }

            if (IsLevel4AlchemySlot(slot.AcceptedItemName))
            {
                return "This slot needs " + requiredItemName + ".";
            }

            if (lowerTargetName.Contains("circle"))
            {
                return "This mark waits for the Circle Relic.";
            }

            if (lowerTargetName.Contains("square"))
            {
                return "This mark waits for the Square Relic.";
            }

            if (lowerTargetName.Contains("triangle"))
            {
                return "This mark waits for the Triangle Relic.";
            }

            return "Find the matching relic first.";
        }

        GameObject pickupObject = FindPickupTarget(target);
        if (pickupObject != null)
        {
            string level4PickupPrompt = GetLevel4PickupPrompt(pickupObject.transform);
            return !string.IsNullOrEmpty(level4PickupPrompt) ? level4PickupPrompt : GetPickupPrompt(pickupObject);
        }

        return "";
    }

    private bool IsLevel4IceTarget(string lowerHierarchyName)
    {
        return !string.IsNullOrEmpty(lowerHierarchyName)
            && (lowerHierarchyName.Contains("ice_l2") || lowerHierarchyName.Contains("ice l2"));
    }

    private GameObject FindPickupTarget(Transform target)
    {
        while (target != null)
        {
            if (target.CompareTag("Pickup"))
            {
                return target.gameObject;
            }

            target = target.parent;
        }

        return null;
    }

    private string GetFriendlyLampName(string lampName)
    {
        if (string.IsNullOrWhiteSpace(lampName))
        {
            return "lamp";
        }

        string lowerName = lampName.ToLowerInvariant();
        if (lowerName.Contains("lamp_2th"))
        {
            return "the second-floor lamp";
        }

        if (lowerName.Contains("lamp_3th"))
        {
            return "the third-floor lamp";
        }

        if (lowerName.Contains("lamp_no"))
        {
            return "the broken lamp";
        }

        return lampName;
    }

    private bool IsLevel4AlchemySlot(string acceptedItemName)
    {
        if (string.IsNullOrWhiteSpace(acceptedItemName))
        {
            return false;
        }

        string lowerName = acceptedItemName.ToLowerInvariant();
        return lowerName.Contains("ash_l4")
            || lowerName.Contains("salt_l4")
            || lowerName.Contains("yellowstone_l4")
            || lowerName.Contains("yellow_stone")
            || lowerName.Contains("bluedrop_l4")
            || lowerName.Contains("blue_drop")
            || lowerName.Contains("legacyseal_l4")
            || lowerName.Contains("legacy_seal");
    }

    private string GetFriendlySlotRequirement(string acceptedItemName)
    {
        if (string.IsNullOrWhiteSpace(acceptedItemName))
        {
            return "the matching item";
        }

        string lowerName = acceptedItemName.ToLowerInvariant();
        if (lowerName.Contains("ash_l4"))
        {
            return "Ash";
        }

        if (lowerName.Contains("salt_l4"))
        {
            return "Salt";
        }

        if (lowerName.Contains("yellowstone_l4") || lowerName.Contains("yellow_stone"))
        {
            return "Yellow Stone";
        }

        if (lowerName.Contains("bluedrop_l4") || lowerName.Contains("blue_drop"))
        {
            return "Blue Drop";
        }

        if (lowerName.Contains("legacyseal_l4") || lowerName.Contains("legacy_seal"))
        {
            return "Legacy Seal";
        }

        if (lowerName.Contains("circle"))
        {
            return "Circle Relic";
        }

        if (lowerName.Contains("square"))
        {
            return "Square Relic";
        }

        if (lowerName.Contains("triangle"))
        {
            return "Triangle Relic";
        }

        return "the matching item";
    }

    private string GetLowerHierarchyName(Transform target)
    {
        string names = "";
        while (target != null)
        {
            if (!string.IsNullOrEmpty(names))
            {
                names += " ";
            }

            names += target.name.ToLowerInvariant();
            target = target.parent;
        }

        return names;
    }

    private string GetLevel1PickupPrompt(Transform target)
    {
        while (target != null)
        {
            string lowerName = target.name.ToLowerInvariant();

            if (lowerName.Contains("woodlevel1"))
            {
                return "E: Take wood";
            }

            if (lowerName.Contains("metallevel1"))
            {
                return "E: Take metal";
            }

            if ((lowerName.Contains("finalseal") || lowerName.Contains("sword")) && !lowerName.Contains("placed"))
            {
                return "E: Take sword";
            }

            if (lowerName.Contains("stamp1"))
            {
                return "E: Take stamp";
            }

            if (lowerName.Contains("stamp2"))
            {
                return "E: Take stamp";
            }

            if (lowerName.Contains("stamp3"))
            {
                return "E: Take stamp";
            }

            target = target.parent;
        }

        return "";
    }

    private string GetPickupPrompt(GameObject item)
    {
        string lowerName = item.name.ToLowerInvariant();

        if (lowerName.Contains("woodlevel1"))
        {
            return "E: Take wood";
        }

        if (lowerName.Contains("metallevel1"))
        {
            return "E: Take metal";
        }

        if (lowerName.Contains("fireball_l3"))
        {
            return "E: Take the fragile lantern oil";
        }

        if (lowerName.Contains("lantern_base_l3"))
        {
            return "E: Take the lantern base";
        }

        if (lowerName.Contains("l3_wick"))
        {
            return "E: Take the lantern wick";
        }

        if (lowerName.Contains("key_l3_chest"))
        {
            return "E: Take the chest key";
        }

        if (lowerName.Contains("key_l3_exit"))
        {
            return "E: Take the exit key";
        }

        if (lowerName.Contains("lamp_l3"))
        {
            return "E: Take the lantern";
        }

        if (lowerName.Contains("torch_for_ice_l3"))
        {
            return "E: Take the fire torch";
        }

        if (lowerName.Contains("torch_for_ice_l2"))
        {
            return "E: Take the lit torch";
        }

        if (lowerName.Contains("torch_level2"))
        {
            return "E: Take the birth-house torch";
        }

        if (lowerName.Contains("ash_l4"))
        {
            return "E: Take Ash";
        }

        if (lowerName.Contains("salt_l4"))
        {
            return "E: Take Salt";
        }

        if (lowerName.Contains("yellowstone_l4") || lowerName.Contains("yellow_stone"))
        {
            return "E: Take Yellow Stone";
        }

        if (lowerName.Contains("bluedrop_l4") || lowerName.Contains("blue_drop"))
        {
            return "E: Take Blue Drop";
        }

        if (lowerName.Contains("key_l4_salt"))
        {
            return "E: Take the salt key";
        }

        if (lowerName.Contains("forge_tool"))
        {
            return "E: Take the forge tool";
        }

        if (lowerName.Contains("legacyseal_l4") || lowerName.Contains("legacy_seal"))
        {
            return "E: Take the Legacy Seal";
        }

        if (lowerName.Contains("lamp_2th"))
        {
            return "E: Take the second-floor lamp";
        }

        if (lowerName.Contains("lamp_3th"))
        {
            return "E: Take the third-floor lamp";
        }

        if (lowerName.Contains("lamp_no"))
        {
            return "E: Take the broken lamp";
        }

        if (lowerName.Contains("circlerelic_l4") || lowerName.Contains("circle_relic") || (lowerName.Contains("circle") && lowerName.Contains("relic")))
        {
            return "E: Take the Circle Relic";
        }

        if (lowerName.Contains("squarerelic_l4") || lowerName.Contains("square_relic") || (lowerName.Contains("square") && lowerName.Contains("relic")))
        {
            return "E: Take the Square Relic";
        }

        if (lowerName.Contains("trianglerelic_l4") || lowerName.Contains("triangle_relic") || (lowerName.Contains("triangle") && lowerName.Contains("relic")))
        {
            return "E: Take the Triangle Relic";
        }

        if (lowerName.Contains("lantern_l4"))
        {
            return "E: Take the master's lantern";
        }

        if (lowerName.Contains("finalseal") || lowerName.Contains("sword"))
        {
            return "E: Take sword";
        }

        if (lowerName.Contains("stamp1"))
        {
            return "E: Take stamp";
        }

        if (lowerName.Contains("stamp2"))
        {
            return "E: Take stamp";
        }

        if (lowerName.Contains("stamp3"))
        {
            return "E: Take stamp";
        }

        if (lowerName.Contains("stamp"))
        {
            return "E: Gather the seal";
        }

        if (lowerName.Contains("wood") || lowerName.Contains("firewood") || lowerName.Contains("timber"))
        {
            return "E: Take wood";
        }

        if (lowerName.Contains("metal") || lowerName.Contains("iron") || lowerName.Contains("sawblade"))
        {
            return "E: Take metal";
        }

        return "E: Take it";
    }

    private string GetLevel3PickupPrompt(Transform target)
    {
        while (target != null)
        {
            string lowerName = target.name.ToLowerInvariant();

            if (lowerName.Contains("fireball_l3"))
            {
                return "E: Take the fragile lantern oil";
            }

            if (lowerName.Contains("lantern_base_l3"))
            {
                return "E: Take the lantern base";
            }

            if (lowerName.Contains("l3_wick"))
            {
                return "E: Take the lantern wick";
            }

            if (lowerName.Contains("key_l3_chest"))
            {
                return "E: Take the chest key";
            }

            if (lowerName.Contains("key_l3_exit"))
            {
                return "E: Take the exit key";
            }

            if (lowerName.Contains("lamp_l3"))
            {
                return "E: Take the lantern";
            }

            if (lowerName.Contains("torch_for_ice_l3"))
            {
                return "E: Take the fire torch";
            }

            target = target.parent;
        }

        return "";
    }

    private string GetLevel4PickupPrompt(Transform target)
    {
        while (target != null)
        {
            string lowerName = target.name.ToLowerInvariant();

            if (lowerName.Contains("ash_l4"))
            {
                return "E: Take Ash";
            }

            if (lowerName.Contains("salt_l4"))
            {
                return "E: Take Salt";
            }

            if (lowerName.Contains("yellowstone_l4") || lowerName.Contains("yellow_stone"))
            {
                return "E: Take Yellow Stone";
            }

            if (lowerName.Contains("bluedrop_l4") || lowerName.Contains("blue_drop"))
            {
                return "E: Take Blue Drop";
            }

            if (lowerName.Contains("key_l4_salt"))
            {
                return "E: Take the salt key";
            }

            if (lowerName.Contains("forge_tool"))
            {
                return "E: Take the forge tool";
            }

            if (lowerName.Contains("torch_for_ice_l2"))
            {
                return "E: Take the lit torch";
            }

            if (lowerName.Contains("legacyseal_l4") || lowerName.Contains("legacy_seal"))
            {
                return "E: Take the Legacy Seal";
            }

            if (lowerName.Contains("lamp_2th"))
            {
                return "E: Take the second-floor lamp";
            }

            if (lowerName.Contains("lamp_3th"))
            {
                return "E: Take the third-floor lamp";
            }

            if (lowerName.Contains("lamp_no"))
            {
                return "E: Take the broken lamp";
            }

            target = target.parent;
        }

        return "";
    }

    private string GetFriendlyItemName(GameObject item)
    {
        if (item == null)
        {
            return "item";
        }

        string lowerName = item.name.ToLowerInvariant();

        if (lowerName.Contains("fireball_l3"))
        {
            return "Fragile Lantern Oil";
        }

        if (lowerName.Contains("lantern_base_l3"))
        {
            return "Lantern Base";
        }

        if (lowerName.Contains("l3_wick"))
        {
            return "Lantern Wick";
        }

        if (lowerName.Contains("key_l3_chest"))
        {
            return "Chest Key";
        }

        if (lowerName.Contains("key_l3_exit"))
        {
            return "Exit Key";
        }

        if (lowerName.Contains("torch_for_ice_l3"))
        {
            return "Ice Torch";
        }

        if (lowerName.Contains("torch_for_ice_l2"))
        {
            return "Lit Torch";
        }

        if (lowerName.Contains("lamp_l3"))
        {
            return "Master's Lantern";
        }

        if (lowerName.Contains("torch_level2"))
        {
            return "Torch";
        }

        if (lowerName.Contains("ash_l4"))
        {
            return "Ash";
        }

        if (lowerName.Contains("salt_l4"))
        {
            return "Salt";
        }

        if (lowerName.Contains("yellowstone_l4") || lowerName.Contains("yellow_stone"))
        {
            return "Yellow Stone";
        }

        if (lowerName.Contains("bluedrop_l4") || lowerName.Contains("blue_drop"))
        {
            return "Blue Drop";
        }

        if (lowerName.Contains("key_l4_salt"))
        {
            return "Salt Key";
        }

        if (lowerName.Contains("forge_tool"))
        {
            return "Forge Tool";
        }

        if (lowerName.Contains("legacyseal_l4") || lowerName.Contains("legacy_seal"))
        {
            return "Legacy Seal";
        }

        if (lowerName.Contains("lamp_2th"))
        {
            return "Second-Floor Lamp";
        }

        if (lowerName.Contains("lamp_3th"))
        {
            return "Third-Floor Lamp";
        }

        if (lowerName.Contains("lamp_no"))
        {
            return "Broken Lamp";
        }

        if (lowerName.Contains("circlerelic_l4") || lowerName.Contains("circle_relic") || (lowerName.Contains("circle") && lowerName.Contains("relic")))
        {
            return "Circle Relic";
        }

        if (lowerName.Contains("squarerelic_l4") || lowerName.Contains("square_relic") || (lowerName.Contains("square") && lowerName.Contains("relic")))
        {
            return "Square Relic";
        }

        if (lowerName.Contains("trianglerelic_l4") || lowerName.Contains("triangle_relic") || (lowerName.Contains("triangle") && lowerName.Contains("relic")))
        {
            return "Triangle Relic";
        }

        if (lowerName.Contains("lantern_l4"))
        {
            return "Master's Lantern";
        }

        if (lowerName.Contains("finalseal") || lowerName.Contains("sword"))
        {
            return "sword";
        }

        if (lowerName.Contains("stamp1"))
        {
            return "stamp";
        }

        if (lowerName.Contains("stamp2"))
        {
            return "stamp";
        }

        if (lowerName.Contains("stamp3"))
        {
            return "stamp";
        }

        if (lowerName.Contains("stamp"))
        {
            return "Seal";
        }

        if (lowerName.Contains("wood") || lowerName.Contains("firewood") || lowerName.Contains("timber"))
        {
            return "wood";
        }

        if (lowerName.Contains("metal") || lowerName.Contains("iron") || lowerName.Contains("sawblade"))
        {
            return "metal";
        }

        return item.name.Replace("(Clone)", "").Trim();
    }

    private bool IsLookingAt(GameObject targetObject)
    {
        return targetObject != null && TryLook(out RaycastHit hit) && Matches(hit.collider.transform, targetObject);
    }

    private bool IsLookingAtLevel2Clue()
    {
        if (!TryLook(out RaycastHit hit))
        {
            return false;
        }

        return IsLevel2ClueTarget(GetLowerHierarchyName(hit.collider.transform));
    }

    private bool IsLevel2ClueTarget(string lowerHierarchyName)
    {
        if (string.IsNullOrEmpty(lowerHierarchyName))
        {
            return false;
        }

        return lowerHierarchyName.Contains("stonetablet_level2")
            || lowerHierarchyName.Contains("stone tablet") && lowerHierarchyName.Contains("level2")
            || lowerHierarchyName.Contains("stone") && lowerHierarchyName.Contains("tablet") && lowerHierarchyName.Contains("level2");
    }

    private bool IsLookingAtLevel4Clue()
    {
        if (!TryLook(out RaycastHit hit))
        {
            return false;
        }

        string lowerTargetName = GetLowerHierarchyName(hit.collider.transform);
        return lowerTargetName.Contains("scroll_level")
            || lowerTargetName.Contains("scroll") && lowerTargetName.Contains("level")
            || lowerTargetName.Contains("central") && lowerTargetName.Contains("tablet");
    }

    private bool TryLook(out RaycastHit hit)
    {
        hit = default;
        if (playerCamera == null)
        {
            return false;
        }

        return AimInteraction.Cast(playerCamera, raycastRange, aimAssistRadius, out hit);
    }

    private bool Matches(Transform transformToCheck, GameObject targetObject)
    {
        return targetObject != null
            && (transformToCheck == targetObject.transform || transformToCheck.IsChildOf(targetObject.transform));
    }

    private bool IsHoldingItem(string itemNamePart)
    {
        if (itemPickup == null || !itemPickup.isHoldingItem || string.IsNullOrEmpty(itemNamePart))
        {
            return false;
        }

        string lowerItemNamePart = itemNamePart.ToLowerInvariant();
        bool currentNameMatches = !string.IsNullOrEmpty(itemPickup.currentItemName)
            && itemPickup.currentItemName.ToLowerInvariant().Contains(lowerItemNamePart);
        bool currentObjectMatches = itemPickup.currentItem != null
            && itemPickup.currentItem.name.ToLowerInvariant().Contains(lowerItemNamePart);

        return currentNameMatches || currentObjectMatches;
    }

    private bool IsHoldingLevel1Stamp()
    {
        return IsHoldingItem("Stamp1")
            || IsHoldingItem("Stamp2")
            || IsHoldingItem("Stamp3");
    }

    private bool IsHoldingLevel1Sword()
    {
        return IsHoldingItem("sword") || IsHoldingItem("FinalSeal");
    }

    private bool IsHoldingNeededLevel1FurnaceItem(Level1FurnaceController furnace)
    {
        if (furnace == null || furnace.HasProducedSeal)
        {
            return false;
        }

        return (!furnace.HasWood && IsHoldingItem("wood"))
            || (!furnace.HasMetal && IsHoldingItem("metal"));
    }

    private string GetMissingLevel1FurnaceItems(Level1FurnaceController furnace)
    {
        if (furnace == null)
        {
            return "wood, metal";
        }

        if (furnace.HasProducedSeal)
        {
            return "nothing";
        }

        string missing = "";
        AddMissingIngredient(ref missing, !furnace.HasWood, "wood");
        AddMissingIngredient(ref missing, !furnace.HasMetal, "metal");

        return string.IsNullOrEmpty(missing) ? "nothing" : missing;
    }

    private bool IsHoldingNeededLampPart(Level3FurnaceCrafting furnace)
    {
        if (furnace == null || furnace.IsCrafted)
        {
            return false;
        }

        return (!furnace.HasOil && IsHoldingItem("fireball_l3"))
            || (!furnace.HasBase && IsHoldingItem("lantern_base_l3"))
            || (!furnace.HasWick && IsHoldingItem("l3_wick"));
    }

    private string GetMissingFurnaceIngredients(Level3FurnaceCrafting furnace)
    {
        if (furnace == null)
        {
            return "oil, base, and wick";
        }

        if (furnace.IsCrafted)
        {
            return "nothing";
        }

        string missing = "";
        AddMissingIngredient(ref missing, !furnace.HasOil, "oil");
        AddMissingIngredient(ref missing, !furnace.HasBase, "base");
        AddMissingIngredient(ref missing, !furnace.HasWick, "wick");

        return string.IsNullOrEmpty(missing) ? "nothing" : missing;
    }

    private void AddMissingIngredient(ref string missing, bool shouldAdd, string ingredientName)
    {
        if (!shouldAdd)
        {
            return;
        }

        if (string.IsNullOrEmpty(missing))
        {
            missing = ingredientName;
            return;
        }

        missing += ", " + ingredientName;
    }

    private bool IsBirthHouseDoor(Transform target)
    {
        while (target != null)
        {
            string lowerName = target.name.ToLowerInvariant();
            if (lowerName.Contains("door_birth") || lowerName.Contains("door_1_venge"))
            {
                return true;
            }

            target = target.parent;
        }

        return false;
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

    public void ShowMessage(string title, string body)
    {
        if (scrollTitleText != null)
        {
            scrollTitleText.text = title;
        }

        if (scrollBodyText != null)
        {
            scrollBodyText.text = body;
        }

        SetScrollOpen(true);
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
