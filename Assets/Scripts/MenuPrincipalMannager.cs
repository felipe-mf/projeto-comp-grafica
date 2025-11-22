using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject rankingMenu;
    public GameObject aboutMenu;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        playMenu.SetActive(false);
        rankingMenu.SetActive(false);
        aboutMenu.SetActive(false);
    }

    public void ShowPlayMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(true);
        rankingMenu.SetActive(false);
        aboutMenu.SetActive(false);
    }

    public void ShowRankingMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        rankingMenu.SetActive(true);
        aboutMenu.SetActive(false);
    }

    public void ShowAboutMenu()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        rankingMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    // --------------------------
    //  Funções para iniciar o jogo
    // --------------------------
    public void StartEasy()
    {
        Debug.Log("Easy selecionado!");

        // ===== DEFINE A DIFICULDADE =====
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetDifficulty(GameSettings.Difficulty.Easy);
            Debug.Log("Dificuldade definida para EASY");
        }
        else
        {
            Debug.LogWarning("GameSettings não encontrado! Criando um novo...");
            CreateGameSettings(GameSettings.Difficulty.Easy);
        }

        SceneManager.LoadScene("SampleScene");
    }

    public void StartHard()
    {
        Debug.Log("Hard selecionado!");

        // ===== DEFINE A DIFICULDADE =====
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetDifficulty(GameSettings.Difficulty.Hard);
            Debug.Log("Dificuldade definida para HARD");
        }
        else
        {
            Debug.LogWarning("GameSettings não encontrado! Criando um novo...");
            CreateGameSettings(GameSettings.Difficulty.Hard);
        }

        SceneManager.LoadScene("SampleScene");
    }

    // Cria GameSettings se não existir (fallback)
    void CreateGameSettings(GameSettings.Difficulty difficulty)
    {
        GameObject settingsObj = new GameObject("GameSettings");
        GameSettings settings = settingsObj.AddComponent<GameSettings>();
        settings.SetDifficulty(difficulty);
        Debug.Log($"GameSettings criado com dificuldade: {difficulty}");
    }

    // --------------------------
    //  Função para Fechar o Jogo
    // --------------------------
    public void QuitGame()
    {
        Debug.Log("Fechando o jogo...");
        // Fechar o jogo no build final
        Application.Quit();

        // Fechar Play Mode no Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}