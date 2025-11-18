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
        // Pega o renderer e cria uma instância única do material
        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            Debug.LogError("AnimatedLaserGrid precisa estar em um objeto com Renderer!");
            enabled = false;
            return;
        }

        // Cria instância do material (não afeta outros objetos)
        material = renderer.material;
        baseColor = material.color;

        // Detecta qual tipo de shader está sendo usado
        usesCustomShader = material.shader.name.Contains("LaserGrid");

        // Salva valores originais
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
    }

    void Update()
    {
        if (material == null) return;

        // Calcula o pulso usando seno (movimento suave)
        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // Normaliza 0-1

        // ===== ANIMAÇÃO DO ALPHA (Transparência) =====
        float currentAlpha = Mathf.Lerp(minAlpha, maxAlpha, pulse);

        if (usesCustomShader && material.HasProperty("_Alpha"))
        {
            // Shader custom: usa propriedade _Alpha
            material.SetFloat("_Alpha", currentAlpha);
        }
        else
        {
            // Shader padrão: usa color.a
            Color newColor = baseColor;
            newColor.a = currentAlpha;
            material.color = newColor;
        }

        // ===== ANIMAÇÃO DA EMISSÃO (Brilho) =====
        if (enableEmissionPulse)
        {
            float emissionIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, pulse);

            if (material.HasProperty("_EmissionStrength"))
            {
                // Shader custom: usa _EmissionStrength
                material.SetFloat("_EmissionStrength", emissionIntensity);
            }
            else if (material.HasProperty("_EmissionColor"))
            {
                // Shader padrão: usa _EmissionColor
                Color emissionColor = material.GetColor("_EmissionColor");
                Color baseEmission = new Color(emissionColor.r, emissionColor.g, emissionColor.b, 1f);
                material.SetColor("_EmissionColor", baseEmission * emissionIntensity);
            }
        }

        // ===== SCROLL DA TEXTURA =====
        if (enableScroll && material.mainTexture != null)
        {
            textureOffset += scrollSpeed * Time.deltaTime;
            material.mainTextureOffset = textureOffset;
        }
    }

    void OnDestroy()
    {
        // Limpa o material instanciado
        if (material != null)
        {
            Destroy(material);
        }
    }

    // Método para debug - mostra valores atuais
    void OnGUI()
    {
        if (material != null && Input.GetKey(KeyCode.F1))
        {
            GUILayout.Label($"Shader: {material.shader.name}");
            GUILayout.Label($"Custom Shader: {usesCustomShader}");

            if (material.HasProperty("_Alpha"))
                GUILayout.Label($"Alpha: {material.GetFloat("_Alpha"):F2}");

            if (material.HasProperty("_EmissionStrength"))
                GUILayout.Label($"Emission: {material.GetFloat("_EmissionStrength"):F2}");
        }
    }
}