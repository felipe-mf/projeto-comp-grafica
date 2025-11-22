using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Referências")]
    public EnemySpawner enemySpawner;
    public GameObject portal;

    [Header("UI de Vitória")]
    public GameObject victoryPanel;
    public Text victoryText;
    public Button menuButton;
    public Button restartButton;

    [Header("Configurações")]
    public float checkInterval = 1f;
    public string menuSceneName = "newMainMenu";

    private bool hasWon = false;
    private float nextCheckTime = 0f;

    void Start()
    {
        if (portal != null) portal.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }
    }

    void Update()
    {
        if (hasWon) return;

        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckVictoryCondition();
        }
    }

    void CheckVictoryCondition()
    {
        int aliveEnemies = 0;

        if (enemySpawner != null)
        {
            aliveEnemies = enemySpawner.GetAliveEnemyCount();
        }
        else
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            aliveEnemies = enemies.Length;
        }

        Debug.Log($"Inimigos restantes: {aliveEnemies}");

        if (aliveEnemies == 0)
        {
            ShowPortal();
        }
    }

    void ShowPortal()
    {
        if (hasWon) return;
        hasWon = true;

        Debug.Log("TODOS OS INIMIGOS DESTRUÍDOS! Portal aparecendo...");

        if (portal != null)
        {
            portal.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Portal não está atribuído! Vitória direta.");
            TriggerVictory();
        }
    }

    public void TriggerVictory()
    {
        Debug.Log("CONGRATULATIONS! YOU WON!");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpaceshipController controller = player.GetComponent<SpaceshipController>();
            if (controller != null) controller.enabled = false;

            WeaponSystem weapon = player.GetComponent<WeaponSystem>();
            if (weapon != null) weapon.enabled = false;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}