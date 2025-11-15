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
    public float gap = 5f; // Espaço do centro (para cruz)

    [Header("Posição")]
    public Vector2 screenOffset = Vector2.zero; // Ajuste a posição (X, Y)

    [Header("Efeitos Dinâmicos")]
    public bool expandOnFire = true;
    public float expandAmount = 5f;
    public float expandSpeed = 10f;

    [Header("Referências (Opcional)")]
    public Image crosshairImage; // Se usar imagem customizada

    private RectTransform canvasRect;
    private GameObject crosshairObject;
    private Image[] crosshairParts;
    private float currentExpansion = 0f;
    private bool isExpanding = false;

    public enum CrosshairType
    {
        Dot,        // Ponto simples
        Cross,      // Cruz
        Circle,     // Círculo
        Brackets,   // Colchetes
        Custom      // Imagem customizada
    }

    void Start()
    {
        CreateCrosshair();
    }

    void Update()
    {
        // Animação de expansão ao atirar
        if (isExpanding)
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
        // Cria Canvas se não existir
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        canvasRect = canvas.GetComponent<RectTransform>();

        // Cria objeto da mira
        crosshairObject = new GameObject("Crosshair");
        crosshairObject.transform.SetParent(canvas.transform, false);

        RectTransform crosshairRect = crosshairObject.AddComponent<RectTransform>();
        crosshairRect.anchorMin = new Vector2(0.5f, 0.5f);
        crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
        crosshairRect.pivot = new Vector2(0.5f, 0.5f);
        crosshairRect.anchoredPosition = screenOffset; // Aplica o offset configurado

        // Cria a mira baseada no tipo
        switch (crosshairType)
        {
            case CrosshairType.Dot:
                CreateDot();
                break;
            case CrosshairType.Cross:
                CreateCross();
                break;
            case CrosshairType.Circle:
                CreateCircle();
                break;
            case CrosshairType.Brackets:
                CreateBrackets();
                break;
            case CrosshairType.Custom:
                CreateCustom();
                break;
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

        // Torna o ponto circular
        img.sprite = CreateCircleSprite();

        crosshairParts = new Image[] { img };
    }

    void CreateCross()
    {
        crosshairParts = new Image[4];

        // Linha horizontal esquerda
        crosshairParts[0] = CreateLine(new Vector2(-size - gap, 0), new Vector2(size, thickness), 0);

        // Linha horizontal direita
        crosshairParts[1] = CreateLine(new Vector2(size + gap, 0), new Vector2(size, thickness), 0);

        // Linha vertical cima
        crosshairParts[2] = CreateLine(new Vector2(0, size + gap), new Vector2(thickness, size), 0);

        // Linha vertical baixo
        crosshairParts[3] = CreateLine(new Vector2(0, -size - gap), new Vector2(thickness, size), 0);
    }

    void CreateCircle()
    {
        GameObject circle = new GameObject("Circle");
        circle.transform.SetParent(crosshairObject.transform, false);

        Image img = circle.AddComponent<Image>();
        img.color = GetColorWithOpacity();
        img.sprite = CreateCircleSprite();
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillAmount = 1f;

        RectTransform rect = circle.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(size * 2, size * 2);

        // Cria outline (círculo interno vazio)
        Outline outline = circle.AddComponent<Outline>();
        outline.effectColor = GetColorWithOpacity();
        outline.effectDistance = new Vector2(thickness, thickness);

        // Torna transparente o interior
        img.color = new Color(crosshairColor.r, crosshairColor.g, crosshairColor.b, 0);

        crosshairParts = new Image[] { img };
    }

    void CreateBrackets()
    {
        crosshairParts = new Image[8];
        float bracketSize = size * 0.6f;

        // Canto superior esquerdo
        crosshairParts[0] = CreateLine(new Vector2(-size, size), new Vector2(bracketSize, thickness), 0);
        crosshairParts[1] = CreateLine(new Vector2(-size, size), new Vector2(thickness, bracketSize), 0);

        // Canto superior direito
        crosshairParts[2] = CreateLine(new Vector2(size, size), new Vector2(bracketSize, thickness), 180);
        crosshairParts[3] = CreateLine(new Vector2(size, size), new Vector2(thickness, bracketSize), 0);

        // Canto inferior esquerdo
        crosshairParts[4] = CreateLine(new Vector2(-size, -size), new Vector2(bracketSize, thickness), 0);
        crosshairParts[5] = CreateLine(new Vector2(-size, -size), new Vector2(thickness, bracketSize), 180);

        // Canto inferior direito
        crosshairParts[6] = CreateLine(new Vector2(size, -size), new Vector2(bracketSize, thickness), 180);
        crosshairParts[7] = CreateLine(new Vector2(size, -size), new Vector2(thickness, bracketSize), 180);
    }

    void CreateCustom()
    {
        if (crosshairImage != null)
        {
            GameObject custom = new GameObject("CustomCrosshair");
            custom.transform.SetParent(crosshairObject.transform, false);

            Image img = custom.AddComponent<Image>();
            img.sprite = crosshairImage.sprite;
            img.color = GetColorWithOpacity();

            RectTransform rect = custom.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(size, size);

            crosshairParts = new Image[] { img };
        }
        else
        {
            Debug.LogWarning("Imagem customizada não definida! Usando ponto padrão.");
            CreateDot();
        }
    }

    Image CreateLine(Vector2 position, Vector2 size, float rotation)
    {
        GameObject line = new GameObject("Line");
        line.transform.SetParent(crosshairObject.transform, false);

        Image img = line.AddComponent<Image>();
        img.color = GetColorWithOpacity();

        RectTransform rect = line.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
        rect.localRotation = Quaternion.Euler(0, 0, rotation);

        return img;
    }

    Sprite CreateCircleSprite()
    {
        // Cria um sprite circular simples
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

        // Atualiza baseado no tipo
        if (crosshairType == CrosshairType.Cross)
        {
            // Linhas horizontais
            crosshairParts[0].rectTransform.anchoredPosition = new Vector2(-expandedSize - gap, 0);
            crosshairParts[1].rectTransform.anchoredPosition = new Vector2(expandedSize + gap, 0);

            // Linhas verticais
            crosshairParts[2].rectTransform.anchoredPosition = new Vector2(0, expandedSize + gap);
            crosshairParts[3].rectTransform.anchoredPosition = new Vector2(0, -expandedSize - gap);
        }
    }

    // Método público para disparar a animação
    public void OnWeaponFired()
    {
        if (expandOnFire)
        {
            currentExpansion = expandAmount;
            isExpanding = true;
        }
    }

    // Permite mudar a cor em tempo real
    public void SetColor(Color newColor)
    {
        crosshairColor = newColor;
        if (crosshairParts != null)
        {
            foreach (Image part in crosshairParts)
            {
                if (part != null)
                    part.color = GetColorWithOpacity();
            }
        }
    }
}