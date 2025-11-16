using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelsPanel;
    public GameObject rankingPanel;
    public GameObject aboutPanel;

    private void Start()
    {
        mainMenuPanel.SetActive(true);
        levelsPanel.SetActive(false);
        rankingPanel.SetActive(false);
        aboutPanel.SetActive(false);
    }

    public void Jogar()
    {
        mainMenuPanel.SetActive(false);
        levelsPanel.SetActive(true);
    }

    public void Ranking()
    {
        mainMenuPanel.SetActive(false);
        rankingPanel.SetActive(true);
    }

    public void AboutGame()
    {
        mainMenuPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void EasyOption()
    {
    Debug.Log("Nível Fácil selecionado.");
    SceneManager.LoadScene("SampleScene");
    }

    public void HardOption()
{
    Debug.Log("Nível Difícil selecionado.");
    SceneManager.LoadScene("SampleScene");
}

    public void ReturnButton()
    {
        levelsPanel.SetActive(false);
        rankingPanel.SetActive(false);
        aboutPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
    }
}
