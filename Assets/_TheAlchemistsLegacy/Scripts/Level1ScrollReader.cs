using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level1ScrollReader : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool readableAtStart = true;
    [SerializeField] private float interactionRange = 3.0f;

    [Header("Content")]
    [TextArea(4, 10)]
    [SerializeField] private string scrollTitle = "Forge Note";
    [TextArea(6, 14)]
    [SerializeField] private string scrollBody =
        "The forge wants two offerings.\n\nFeed it wood first, then metal. When the sword is born, sacrifice it at the old door to open the way.";

    private const string CanvasName = "Level1ScrollCanvas";

    private Camera playerCamera;
    private PlayerController playerController;
    private Canvas scrollCanvas;
    private GameObject panel;
    private Text titleText;
    private Text bodyText;
    private Text closeText;
    private Collider[] cachedColliders;
    private Font defaultFont;
    private bool isOpen;
    private bool isReadable;

    public bool IsReadable
    {
        get { return isReadable; }
    }

    private void Awake()
    {
        cachedColliders = GetComponentsInChildren<Collider>(true);
        ApplySceneCopy();
    }

    private void Start()
    {
        playerCamera = Camera.main;
        playerController = FindObjectOfType<PlayerController>();
        defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

        BuildCanvas();
        SetReadable(readableAtStart);
    }

    private void Update()
    {
        if (isOpen)
        {
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))
            {
                CloseScroll();
            }

            return;
        }

        if (!isReadable)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryOpenScroll();
        }
    }

    public void SetReadable(bool readable)
    {
        isReadable = readable;

        if (cachedColliders == null)
        {
            cachedColliders = GetComponentsInChildren<Collider>(true);
        }

        foreach (Collider itemCollider in cachedColliders)
        {
            if (itemCollider != null)
            {
                itemCollider.enabled = readable;
            }
        }
    }

    private void ApplySceneCopy()
    {
        if (SceneManager.GetActiveScene().name == "Level1" && gameObject.name.Contains("Scroll2"))
        {
            scrollTitle = "Forge Note";
            scrollBody =
                "When the forge wakes, feed it wood first, then metal.\n\nThe forge will return a sword. Sacrifice the sword at the old door; only then will the door let you pass.";
        }
    }

    private void TryOpenScroll()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
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

        if (hit.collider.transform != transform && !hit.collider.transform.IsChildOf(transform))
        {
            return;
        }

        OpenScroll();
    }

    private void OpenScroll()
    {
        if (scrollCanvas == null)
        {
            BuildCanvas();
        }

        if (titleText != null)
        {
            titleText.text = scrollTitle;
        }

        if (bodyText != null)
        {
            bodyText.text = scrollBody;
        }

        isOpen = true;

        if (scrollCanvas != null)
        {
            scrollCanvas.gameObject.SetActive(true);
        }

        Time.timeScale = 0.0f;

        if (playerController != null)
        {
            playerController.UnlockCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void CloseScroll()
    {
        if (!isOpen)
        {
            return;
        }

        isOpen = false;

        if (scrollCanvas != null)
        {
            scrollCanvas.gameObject.SetActive(false);
        }

        Time.timeScale = 1.0f;

        if (playerController != null)
        {
            playerController.LockCursor();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void BuildCanvas()
    {
        GameObject canvasObject = new GameObject(CanvasName + "_" + gameObject.name);
        scrollCanvas = canvasObject.AddComponent<Canvas>();
        scrollCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        scrollCanvas.sortingOrder = 80;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920.0f, 1080.0f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        panel = CreatePanel(canvasObject.transform, new Color(0.87f, 0.78f, 0.58f, 0.96f));
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Anchor(panelRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900.0f, 560.0f), new Vector2(0.5f, 0.5f));

        titleText = CreateText(panel.transform, "Title", scrollTitle, 36, new Color(0.14f, 0.08f, 0.03f), TextAnchor.UpperCenter);
        Anchor(titleText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(40.0f, -34.0f), new Vector2(-80.0f, 56.0f), new Vector2(0.5f, 1.0f));

        bodyText = CreateText(panel.transform, "Body", scrollBody, 28, new Color(0.12f, 0.07f, 0.03f), TextAnchor.UpperLeft);
        bodyText.lineSpacing = 1.08f;
        Anchor(bodyText.rectTransform, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), new Vector2(70.0f, -116.0f), new Vector2(-140.0f, 310.0f), new Vector2(0.0f, 1.0f));

        closeText = CreateText(panel.transform, "CloseHint", "Right Click / E  Close", 22, new Color(0.16f, 0.10f, 0.05f), TextAnchor.LowerCenter);
        Anchor(closeText.rectTransform, new Vector2(0.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 28.0f), new Vector2(-80.0f, 36.0f), new Vector2(0.5f, 0.0f));

        scrollCanvas.gameObject.SetActive(false);
    }

    private GameObject CreatePanel(Transform parent, Color color)
    {
        GameObject panelObject = new GameObject("Panel");
        panelObject.transform.SetParent(parent, false);

        panelObject.AddComponent<RectTransform>();
        Image image = panelObject.AddComponent<Image>();
        image.color = color;

        return panelObject;
    }

    private Text CreateText(Transform parent, string objectName, string content, int fontSize, Color color, TextAnchor alignment)
    {
        GameObject textObject = new GameObject(objectName);
        textObject.transform.SetParent(parent, false);

        textObject.AddComponent<RectTransform>();
        Text text = textObject.AddComponent<Text>();
        text.font = defaultFont;
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    private void Anchor(
        RectTransform rectTransform,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 sizeDelta,
        Vector2 pivot)
    {
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.pivot = pivot;
    }

    private void OnDisable()
    {
        if (isOpen)
        {
            CloseScroll();
        }
    }
}
