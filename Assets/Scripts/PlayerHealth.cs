using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Adiciona suporte para TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHits = 2; // Número de colisões até morrer
    private int currentHits = 0;

    [Header("UI - Mensagem de Aviso")]
    public GameObject warningPanel; // Painel com fundo escuro
    public TextMeshProUGUI warningText; // TextMeshPro
    public Text warningTextLegacy; // Unity UI Legacy (use um OU outro)
    public float warningDuration = 2f; // Tempo que a mensagem fica na tela

    [Header("UI - Game Over")]
    public GameObject gameOverPanel; // Painel de Game Over
    public TextMeshProUGUI gameOverText; // TextMeshPro
    public Text gameOverTextLegacy; // Unity UI Legacy (use um OU outro)
    public Button restartButton; // Botão de reiniciar (opcional)
    public Button menuButton;

    public string menuSceneName = "newMainMenu";



    [Header("Efeitos")]
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public AudioClip damageSound;

    [Header("Configurações")]
    public float respawnDelay = 2f; // Tempo antes de explodir/game over
    public bool destroyShipOnDeath = true; // Se true, destrói a nave
    public float damageCooldown = 1f; // Tempo de invencibilidade após levar dano

    private AudioSource audioSource;
    private bool isDead = false;
    private float lastDamageTime = -999f; // Controle de cooldown

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Esconde UI inicial
        if (warningPanel != null) warningPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);


        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }
        // Configura botão de restart
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // Verifica se colidiu com asteroide ou parede
        bool isAsteroid = collision.gameObject.GetComponent<Asteroid>() != null;
        bool isWall = collision.gameObject.CompareTag("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Default");
        bool isEnemy = collision.gameObject.GetComponent<EnemyShip>() != null;

        if (isAsteroid || isWall || isEnemy)
        {
            TakeDamage();
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;

        // Verifica cooldown (evita múltiplas colisões em sequência)
        if (Time.time - lastDamageTime < damageCooldown)
        {
            Debug.Log("Ainda em cooldown, dano ignorado");
            return;
        }

        lastDamageTime = Time.time;
        currentHits++;

        Debug.Log("Player levou dano! Colisões: " + currentHits + "/" + maxHits);

        // Toca som de dano
        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound, 7f);
        }

        if (currentHits >= maxHits)
        {
            // Morreu!
            Die();
        }
        else
        {
            // Ainda está vivo - mostra aviso
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        Debug.Log("AVISO: SPACESHIP DAMAGED!");

        if (warningPanel != null)
        {
            warningPanel.SetActive(true);

            // Esconde após alguns segundos
            Invoke("HideWarning", warningDuration);
        }

        // Efeito visual de "tremor" ou piscar (opcional)
        StartCoroutine(FlashEffect());
    }

    void HideWarning()
    {
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
    }

    System.Collections.IEnumerator FlashEffect()
    {
        // Faz a nave "piscar" brevemente
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < 3; i++)
        {
            // Esconde
            foreach (Renderer rend in renderers)
            {
                if (rend != null) rend.enabled = false;
            }
            yield return new WaitForSeconds(0.1f);

            // Mostra
            foreach (Renderer rend in renderers)
            {
                if (rend != null) rend.enabled = true;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("GAME OVER - Player morreu!");

        // Toca som de explosão
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 15f);
        }

        // Cria efeito de explosão
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        // Desabilita controles
        SpaceshipController controller = GetComponent<SpaceshipController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        WeaponSystem weapon = GetComponent<WeaponSystem>();
        if (weapon != null)
        {
            weapon.enabled = false;
        }

        // Para a nave
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Mostra Game Over após delay
        Invoke("ShowGameOver", respawnDelay);

        // Destrói a nave (opcional)
        if (destroyShipOnDeath)
        {
            // Esconde o modelo visual
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                if (rend != null) rend.enabled = false;
            }
        }
    }

    void ShowGameOver()
    {
        Debug.Log("Mostrando tela de Game Over");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Pausa o jogo (opcional)
        // Time.timeScale = 0f;

        // Mostra cursor para clicar no botão
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void RestartGame()
    {
        // Despausa o jogo
        Time.timeScale = 1f;

        // Recarrega a cena atual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método público para curar (se você quiser power-ups depois)
    public void Heal()
    {
        if (currentHits > 0)
        {
            currentHits--;
            Debug.Log("Player curado! Colisões: " + currentHits + "/" + maxHits);
        }
    }
}