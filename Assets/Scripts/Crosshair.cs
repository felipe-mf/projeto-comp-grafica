using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Configurações da Mira")]
    public CrosshairType crosshairType = CrosshairType.Dot;
    public Color crosshairColor = Color.white;
    [Range(0f, 1f)]
    public float opacity = 0.8f;
    [Range(5f, 50f)]
    public float size = 20f;
    [Range(1f, 10f)]
    public float thickness = 2f;
    [Range(0f, 20f)]
    public float gap = 5f;

    [Header("Posição")]
    public Vector2 screenOffset = Vector2.zero;

    [Header("Efeitos Dinâmicos")]
    public bool expandOnFire = true;
    public float expandAmount = 5f;
    public float expandSpeed = 10f;

    private GameObject crosshairObject;
    private Image[] crosshairParts;
    private float currentExpansion = 0f;
    private bool isExpanding = false;

    public static Crosshair Instance { get; private set; }

    public enum CrosshairType
    {
        Dot,
        Cross,
        Brackets
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            crosshairType = CrosshairType.Dot;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (!IsInMenuScene())
        {
            CreateCrosshair();
        }
    }

    void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (IsInMenuScene())
        {
            DestroyCrosshair();
        }
        else
        {
            Invoke("CreateCrosshair", 0.1f);
        }
    }

    bool IsInMenuScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneName.ToLower().Contains("menu");
    }

    void Update()
    {
        if (!IsInMenuScene() && isExpanding)
        {
            currentExpansion = Mathf.Lerp(currentExpansion, 0f, Time.deltaTime * expandSpeed);
            if (currentExpansion < 0.1f)
            {
                currentExpansion = 0f;
                isExpanding = false;
            }
            UpdateCrosshairSize();
        }
    }

    void CreateCrosshair()
    {
        if (IsInMenuScene() || crosshairObject != null) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        crosshairObject = new GameObject("Crosshair");
        crosshairObject.transform.SetParent(canvas.transform, false);

        RectTransform crosshairRect = crosshairObject.AddComponent<RectTransform>();
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRect.pivot = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = screenOffset;

        switch (crosshairType)
        {
            case CrosshairType.Dot:
                CreateDot();
                break;
            case CrosshairType.Cross:
                CreateCross();
                break;
            case CrosshairType.Brackets:
                CreateBrackets();
                break;
        }

        Debug.Log("Mira criada: " + crosshairType);
    }

    void DestroyCrosshair()
    {
        if (crosshairObject != null)
        {
            Destroy(crosshairObject);
            crosshairObject = null;
            crosshairParts = null;
        }
    }

    void CreateDot()
    {
        GameObject dot = new GameObject("Dot");
        dot.transform.SetParent(crosshairObject.transform, false);

        Image img = dot.AddComponent<Image>();
        img.color = GetColorWithOpacity();

        RectTransform rect = dot.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size, size);
        img.sprite = CreateCircleSprite();

        crosshairParts = new Image[] { img };
    }

    void CreateCross()
    {
        crosshairParts = new Image[4];
        crosshairParts[0] = CreateLine(new Vector2(-size - gap, 0), new Vector2(size, thickness), 0);
        crosshairParts[1] = CreateLine(new Vector2(size + gap, 0), new Vector2(size, thickness), 0);
        crosshairParts[2] = CreateLine(new Vector2(0, size + gap), new Vector2(thickness, size), 0);
        crosshairParts[3] = CreateLine(new Vector2(0, -size - gap), new Vector2(thickness, size), 0);
    }

    void CreateBrackets()
    {
        crosshairParts = new Image[8];
        float bracketSize = size * 0.6f;

        crosshairParts[0] = CreateLine(new Vector2(-size, size), new Vector2(bracketSize, thickness), 0);
        crosshairParts[1] = CreateLine(new Vector2(-size, size), new Vector2(thickness, bracketSize), 0);
        crosshairParts[2] = CreateLine(new Vector2(size, size), new Vector2(bracketSize, thickness), 180);
        crosshairParts[3] = CreateLine(new Vector2(size, size), new Vector2(thickness, bracketSize), 0);
        crosshairParts[4] = CreateLine(new Vector2(-size, -size), new Vector2(bracketSize, thickness), 0);
        crosshairParts[5] = CreateLine(new Vector2(-size, -size), new Vector2(thickness, bracketSize), 180);
        crosshairParts[6] = CreateLine(new Vector2(size, -size), new Vector2(bracketSize, thickness), 180);
        crosshairParts[7] = CreateLine(new Vector2(size, -size), new Vector2(thickness, bracketSize), 180);
    }

    Image CreateLine(Vector2 position, Vector2 lineSize, float rotation)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(crosshairObject.transform, false);

        Image img = line.AddComponent<Image>();
        img.color = GetColorWithOpacity();

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = lineSize;
        rect.localRotation = Quaternion.Euler(0, 0, rotation);

        return img;
    }

    Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(32, 32);
        Color[] pixels = new Color[32 * 32];

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                pixels[y * 32 + x] = distance < 16 ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
    }

    Color GetColorWithOpacity()
    {
        return new Color(crosshairColor.r, crosshairColor.g, crosshairColor.b, opacity);
    }

    void UpdateCrosshairSize()
    {
        if (crosshairParts == null) return;

        float expandedSize = size + currentExpansion;

        if (crosshairType == CrosshairType.Cross)
        {
            crosshairParts[0].rectTransform.anchoredPosition = new Vector2(-expandedSize - gap, 0);
            crosshairParts[1].rectTransform.anchoredPosition = new Vector2(expandedSize + gap, 0);
            crosshairParts[2].rectTransform.anchoredPosition = new Vector2(0, expandedSize + gap);
            crosshairParts[3].rectTransform.anchoredPosition = new Vector2(0, -expandedSize - gap);
        }
    }

    public void OnWeaponFired()
    {
        if (expandOnFire && !IsInMenuScene())
        {
            currentExpansion = expandAmount;
            isExpanding = true;
        }
    }

    public void SetCrosshairType(CrosshairType newType)
    {
        crosshairType = newType;

        if (!IsInMenuScene())
        {
            DestroyCrosshair();
            CreateCrosshair();
        }
    }
}