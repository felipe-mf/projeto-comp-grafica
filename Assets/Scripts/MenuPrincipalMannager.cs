using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject playMenu;
    public GameObject rankingMenu;
    public GameObject aboutMenu;

    private void Start()
    {
        // Começa mostrando só o menu principal
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
}
