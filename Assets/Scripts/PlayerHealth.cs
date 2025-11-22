using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public int maxHits = 2; // Número de colisões até morrer
    private int currentHits = 0;

    [Header("UI - Mensagem de Aviso")]
    public GameObject warningPanel;
    public TextMeshProUGUI warningText;
    public Text warningTextLegacy;
    public float warningDuration = 2f;

    [Header("UI - Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public Text gameOverTextLegacy;
    public Button restartButton;
    public Button menuButton;

    public string menuSceneName = "newMainMenu";



    [Header("Efeitos")]
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public AudioClip damageSound;

    [Header("Configurações")]
    public float respawnDelay = 2f;
    public bool destroyShipOnDeath = true;
    public float damageCooldown = 1f;

    private AudioSource audioSource;
    private bool isDead = false;
    private float lastDamageTime = -999f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (warningPanel != null) warningPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);


        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

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

        if (Time.time - lastDamageTime < damageCooldown)
        {
            Debug.Log("Ainda em cooldown, dano ignorado");
            return;
        }

        lastDamageTime = Time.time;
        currentHits++;

        Debug.Log("Player levou dano! Colisões: " + currentHits + "/" + maxHits);

        if (damageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(damageSound, 7f);
        }

        if (currentHits >= maxHits)
        {
            Die();
        }
        else
        {
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        Debug.Log("AVISO: SPACESHIP DAMAGED!");

        if (warningPanel != null)
        {
            warningPanel.SetActive(true);

            Invoke("HideWarning", warningDuration);
        }

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
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < 3; i++)
        {
            foreach (Renderer rend in renderers)
            {
                if (rend != null) rend.enabled = false;
            }
            yield return new WaitForSeconds(0.1f);

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

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 15f);
        }

        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

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

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Invoke("ShowGameOver", respawnDelay);

        if (destroyShipOnDeath)
        {
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
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para curar (se formos colocar power-ups)
    public void Heal()
    {
        if (currentHits > 0)
        {
            currentHits--;
            Debug.Log("Player curado! Colisões: " + currentHits + "/" + maxHits);
        }
    }
}