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
        SceneManager.LoadScene("SampleScene");
    }

    public void StartHard()
    {
        Debug.Log("Hard selecionado!");
        SceneManager.LoadScene("SampleScene");
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
