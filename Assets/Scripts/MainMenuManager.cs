//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuManager : MonoBehaviour
//{
//    [Header("Panels")]
//    public GameObject mainMenuPanel;
//    public GameObject levelsPanel;
//    public GameObject rankingPanel;
//    public GameObject aboutPanel;

//    private void Start()
//    {
//        mainMenuPanel.SetActive(true);
//        levelsPanel.SetActive(false);
//        rankingPanel.SetActive(false);
//        aboutPanel.SetActive(false);
//    }

//    public void Jogar()
//    {
//        mainMenuPanel.SetActive(false);
//        levelsPanel.SetActive(true);
//    }

//    public void Ranking()
//    {
//        mainMenuPanel.SetActive(false);
//        rankingPanel.SetActive(true);
//    }

//    public void AboutGame()
//    {
//        mainMenuPanel.SetActive(false);
//        aboutPanel.SetActive(true);
//    }

//    public void EasyOption()
//    {
//    Debug.Log("Nível Fácil selecionado.");
//    SceneManager.LoadScene("SampleScene");
//    }

//    public void HardOption()
//{
//    Debug.Log("Nível Difícil selecionado.");
//    SceneManager.LoadScene("SampleScene");
//}

//    public void ReturnButton()
//    {
//        levelsPanel.SetActive(false);
//        rankingPanel.SetActive(false);
//        aboutPanel.SetActive(false);

//        mainMenuPanel.SetActive(true);
//    }
//}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Botões do Menu")]
    public Button easyButton;    // Botão "Fácil" ou pode usar o "JogarButton"
    public Button hardButton;    // Botão "Difícil"
    public Button rankingButton; // Ignora
    public Button aboutButton;   // Ignora (SobreButton)

    [Header("Configurações")]
    public string gameSceneName = "SampleScene"; // Nome da cena do jogo

    [Header("UI Feedback (Opcional)")]
    public Text difficultyText; // Mostra qual dificuldade está selecionada

    void Start()
    {
        // Garante que o GameSettings existe
        if (GameSettings.Instance == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settingsObj.AddComponent<GameSettings>();
        }

        // Conecta os botões
        if (easyButton != null)
        {
            easyButton.onClick.AddListener(StartEasyMode);
        }

        if (hardButton != null)
        {
            hardButton.onClick.AddListener(StartHardMode);
        }

        // Atualiza UI se existir
        UpdateDifficultyText();
    }

    public void StartEasyMode()
    {
        Debug.Log("Iniciando modo FÁCIL");

        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetDifficulty(GameSettings.Difficulty.Easy);
        }

        LoadGameScene();
    }

    public void StartHardMode()
    {
        Debug.Log("Iniciando modo DIFÍCIL");

        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetDifficulty(GameSettings.Difficulty.Hard);
        }

        LoadGameScene();
    }

    void LoadGameScene()
    {
        Debug.Log("Carregando cena: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    void UpdateDifficultyText()
    {
        if (difficultyText != null && GameSettings.Instance != null)
        {
            difficultyText.text = "Dificuldade: " + GameSettings.Instance.selectedDifficulty;
        }
    }

    // Métodos para os outros botões (se quiser usar depois)
    public void OpenRanking()
    {
        Debug.Log("Abrindo Ranking (não implementado)");
        // SceneManager.LoadScene("RankingScene");
    }

    public void OpenAbout()
    {
        Debug.Log("Abrindo Sobre (não implementado)");
        // SceneManager.LoadScene("AboutScene");
    }
}