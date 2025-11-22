using UnityEngine;

public class AnimatedLaserGrid : MonoBehaviour
{
    [Header("Configuração de Animação")]
    [Tooltip("Velocidade do pulso (maior = mais rápido)")]
    public float pulseSpeed = 2f;

    [Tooltip("Transparência mínima (0 = invisível, 1 = opaco)")]
    [Range(0f, 1f)]
    public float minAlpha = 0.2f;

    [Tooltip("Transparência máxima")]
    [Range(0f, 1f)]
    public float maxAlpha = 0.6f;

    [Header("Cores por Dificuldade")]
    [Tooltip("Cor no modo fácil")]
    public Color easyColor = new Color(0f, 1f, 1f); // Ciano

    [Tooltip("Cor no modo difícil")]
    public Color hardColor = Color.red; // Vermelho

    [Header("Efeito Extra (Opcional)")]
    [Tooltip("Ativa pulsação do brilho (emission)")]
    public bool enableEmissionPulse = false;

    [Tooltip("Intensidade mínima do brilho")]
    [Range(0f, 5f)]
    public float minEmissionIntensity = 1.5f;

    [Tooltip("Intensidade máxima do brilho")]
    [Range(0f, 5f)]
    public float maxEmissionIntensity = 3.5f;

    [Header("Scroll da Textura (Opcional)")]
    [Tooltip("Move a textura da grade")]
    public bool enableScroll = false;
    public Vector2 scrollSpeed = new Vector2(0.1f, 0);

    private Material material;
    private Color baseColor;
    private float originalAlpha;
    private float originalEmission;
    private Vector2 textureOffset;
    private bool usesCustomShader = false;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogError("AnimatedLaserGrid precisa estar em um objeto com Renderer!");
            enabled = false;
            return;
        }

        material = renderer.material;

        SetColorBasedOnDifficulty();

        baseColor = material.color;

        usesCustomShader = material.shader.name.Contains("LaserGrid");

        if (material.HasProperty("_Alpha"))
        {
            originalAlpha = material.GetFloat("_Alpha");
        }

        if (material.HasProperty("_EmissionStrength"))
        {
            originalEmission = material.GetFloat("_EmissionStrength");
        }

        Debug.Log($"AnimatedLaserGrid iniciado em: {gameObject.name}");
        Debug.Log($"Material: {material.shader.name}");
        Debug.Log($"Shader Custom detectado: {usesCustomShader}");
        Debug.Log($"Cor definida: {baseColor}");
    }

    void SetColorBasedOnDifficulty()
    {
        if (GameSettings.Instance == null)
        {
            Debug.LogWarning("GameSettings não encontrado! Usando cor padrão (Easy).");
            baseColor = easyColor;
            ApplyColorToMaterial(easyColor);
            return;
        }

        GameSettings.Difficulty currentDifficulty = GameSettings.Instance.selectedDifficulty;

        Color selectedColor;
        if (currentDifficulty == GameSettings.Difficulty.Hard)
        {
            selectedColor = hardColor;
            Debug.Log("Dificuldade DIFÍCIL detectada - Paredes VERMELHAS");
        }
        else
        {
            selectedColor = easyColor;
            Debug.Log("Dificuldade FÁCIL detectada - Paredes CIANO");
        }

        ApplyColorToMaterial(selectedColor);
    }

    void ApplyColorToMaterial(Color color)
    {
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", color);
        }

        Color currentColor = material.color;
        currentColor.r = color.r;
        currentColor.g = color.g;
        currentColor.b = color.b;
        material.color = currentColor;

        if (material.HasProperty("_EmissionColor"))
        {
            material.SetColor("_EmissionColor", color * 2f);
        }

        baseColor = currentColor;
    }

    void Update()
    {
        if (material == null) return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

        if (usesCustomShader && material.HasProperty("_Alpha"))
        {
            material.SetFloat("_Alpha", currentAlpha);
        }
        else
        {
            Color newColor = baseColor;
            newColor.a = currentAlpha;
            material.color = newColor;
        }

        if (enableEmissionPulse)
        {
            float emissionIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, pulse);

            if (material.HasProperty("_EmissionStrength"))
            {
                material.SetFloat("_EmissionStrength", emissionIntensity);
            }
            else if (material.HasProperty("_EmissionColor"))
            {
                Color emissionColor = baseColor;
                material.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            }
        }

        if (enableScroll && material.mainTexture != null)
        {
            textureOffset += scrollSpeed * Time.deltaTime;
            material.mainTextureOffset = textureOffset;
        }
    }

    void OnDestroy()
    {
        if (material != null)
        {
            Destroy(material);
        }
    }

    public void SetColor(Color newColor)
    {
        if (material != null)
        {
            ApplyColorToMaterial(newColor);
        }
    }

    void OnGUI()
    {
        if (material != null && Input.GetKey(KeyCode.F1))
        {
            GUILayout.Label($"Shader: {material.shader.name}");
            GUILayout.Label($"Custom Shader: {usesCustomShader}");
            GUILayout.Label($"Cor Base: {baseColor}");

            if (GameSettings.Instance != null)
            {
                GUILayout.Label($"Dificuldade: {GameSettings.Instance.selectedDifficulty}");
            }

            if (material.HasProperty("_Alpha"))
                GUILayout.Label($"Alpha: {material.GetFloat("_Alpha"):F2}");

            if (material.HasProperty("_EmissionStrength"))
                GUILayout.Label($"Emission: {material.GetFloat("_EmissionStrength"):F2}");
        }
    }
}