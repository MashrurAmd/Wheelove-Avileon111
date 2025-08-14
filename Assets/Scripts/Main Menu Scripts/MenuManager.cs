using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Config")]
    public GameConfig config;

    [Header("Panels")]

    public GameObject chooseRegionPanel;
    public GameObject chooseModePanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Main Menu UI")]
    public TMP_Text modeText;
    public TMP_Text regionText;

    private void Start()
    {
        config.Load();

        if (config.IsFirstLaunch())
        {
            ShowMainMenu();
            //ShowChooseRegion();
        }
        //else
        //{
            
        //}
    }

    // REGION SELECTION

    public void SetRegion(int regionIndex)
    {
        config.currentRegion = (GameRegion)regionIndex;
    }

    public void SetLanguage(string lang)
    {
        config.language = lang;
    }

    public void ConfirmRegionAndLanguage()
    {
        ShowChooseMode();
    }

    
    // MODE SELECTION
    
    public void SetMode(int modeIndex)
    {
        config.currentMode = (GameMode)modeIndex;
        config.Save();
        ShowMainMenu();
    }

    // UI NAVIGATION

    public void ShowChooseRegion()
    {
        HideAll();
        chooseRegionPanel.SetActive(true);
    }

    public void ShowChooseMode()
    {
        HideAll();
        chooseModePanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);
        modeText.text = $"Current Mode: {config.currentMode}";
        regionText.text = $"Current Region: {config.currentRegion}";
    }

    public void PlayGame(int sceneIndex)
    {
        HideAll();
        SceneManager.LoadScene(sceneIndex);
    }

    public void ShowSettings()
    {
        HideAll();
        settingsPanel.SetActive(true);
    }

    private void HideAll()
    {
        chooseRegionPanel.SetActive(false);
        chooseModePanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
