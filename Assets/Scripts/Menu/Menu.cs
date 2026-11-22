
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _optionsPanel;
    [SerializeField] private GameObject _creditsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("PhoebeHouseInterior");
    }

    public void OpenOptions()
    {
        _optionsPanel.SetActive(true);
        _creditsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        _creditsPanel.SetActive(true);
        _optionsPanel.SetActive(false);
    }

    public void ClosePanels()
    {
        _optionsPanel.SetActive(false);
        _creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
