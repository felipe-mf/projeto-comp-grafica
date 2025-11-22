using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("Configurações do Timer")]
    [Tooltip("Tempo total em segundos (420 = 7 minutos)")]
    public float totalTime = 420f; // 7 minutos

    [Header("UI")]
    [Tooltip("Texto para mostrar o timer (TextMeshPro)")]
    public TextMeshProUGUI timerText;

    [Header("Aviso de Tempo Acabando")]
    [Tooltip("Tempo em segundos para começar a piscar (ex: 60 = último minuto)")]
    public float warningTime = 60f;

    [Tooltip("Cor normal do timer")]
    public Color normalColor = Color.white;

    [Tooltip("Cor de aviso (quando está acabando)")]
    public Color warningColor = Color.red;

    [Header("Referências")]
    [Tooltip("Referência ao PlayerHealth para chamar Die()")]
    public PlayerHealth playerHealth;

    [Header("Sons (Opcional)")]
    public AudioClip tickSound;
    public AudioClip timeUpSound;

    private float currentTime;
    private bool isRunning = true;
    private bool hasWarned = false;
    private AudioSource audioSource;

    void Start()
    {
        currentTime = totalTime;

        if (playerHealth == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerHealth = player.GetComponent<PlayerHealth>();
            }
        }

        if (playerHealth == null)
        {
            Debug.LogError("GameTimer: PlayerHealth não encontrado! Atribua manualmente ou adicione tag 'Player'.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (tickSound != null || timeUpSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        UpdateTimerDisplay();

        if (currentTime <= warningTime && !hasWarned)
        {
            hasWarned = true;
            StartWarning();
        }

        if (currentTime <= 10f && currentTime > 0f && tickSound != null)
        {
            if (Mathf.Ceil(currentTime) != Mathf.Ceil(currentTime + Time.deltaTime))
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(tickSound);
                }
            }
        }

        if (currentTime <= 0f)
        {
            TimeUp();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText != null)
        {
            timerText.text = timeString;
        }

        if (currentTime <= warningTime)
        {
            if (currentTime <= 10f)
            {
                float alpha = Mathf.PingPong(Time.time * 2f, 1f);
                Color flashColor = Color.Lerp(normalColor, warningColor, alpha);

                if (timerText != null) timerText.color = flashColor;
            }
            else
            {
                if (timerText != null) timerText.color = warningColor;
            }
        }
        else
        {
            if (timerText != null) timerText.color = normalColor;
        }
    }

    void StartWarning()
    {
        Debug.Log("AVISO: Tempo acabando!");
    }

    void TimeUp()
    {
        if (!isRunning) return;

        isRunning = false;
        currentTime = 0f;

        Debug.Log("TEMPO ESGOTADO!");

        if (timeUpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(timeUpSound);
        }

        UpdateTimerDisplay();

        if (playerHealth != null)
        {
            playerHealth.Die();
        }
        else
        {
            Debug.LogError("PlayerHealth não está atribuído! Não foi possível chamar Die().");
        }
    }
}