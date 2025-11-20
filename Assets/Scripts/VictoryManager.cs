using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    [Header("Referências")]
    public EnemySpawner enemySpawner;
    public GameObject portal; // Portal que aparece ao vencer

    [Header("UI de Vitória")]
    public GameObject victoryPanel;
    public Text victoryText;
    public Button menuButton;
    public Button restartButton;

    [Header("Configurações")]
    public float checkInterval = 1f; // Verifica a cada 1 segundo
    public string menuSceneName = "MainMenu";

    private bool hasWon = false;
    private float nextCheckTime = 0f;

    void Start()
    {
        // Esconde portal e UI de vitória
        if (portal != null) portal.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Configura botões
        if (menuButton != null)
        {
            menuButton.onClick.AddListener(GoToMenu);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        // Busca spawner se não estiver atribuído
        if (enemySpawner == null)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }
    }

    void Update()
    {
        if (hasWon) return;

        // Verifica se todos os inimigos foram destruídos
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckVictoryCondition();
        }
    }

    void CheckVictoryCondition()
    {
        // Conta inimigos vivos
        int aliveEnemies = 0;

        if (enemySpawner != null)
        {
            aliveEnemies = enemySpawner.GetAliveEnemyCount();
        }
        else
        {
            // Fallback: busca por tag
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            aliveEnemies = enemies.Length;
        }

        Debug.Log($"Inimigos restantes: {aliveEnemies}");

        // Se não há mais inimigos, mostra o portal
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

            // Opcional: Anima o portal aparecendo
            //portal.transform.localScale = Vector3.zero;
            //LeanTween.scale(portal, Vector3.one * 5f, 1f).setEaseOutBack();
        }
        else
        {
            Debug.LogWarning("Portal não está atribuído! Vitória direta.");
            TriggerVictory();
        }
    }

    // Chamado quando o player entra no portal
    public void TriggerVictory()
    {
        Debug.Log("CONGRATULATIONS! YOU WON!");

        // Desabilita controles do player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SpaceshipController controller = player.GetComponent<SpaceshipController>();
            if (controller != null) controller.enabled = false;

            WeaponSystem weapon = player.GetComponent<WeaponSystem>();
            if (weapon != null) weapon.enabled = false;

            // Para a nave
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Mostra tela de vitória
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Mostra cursor
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