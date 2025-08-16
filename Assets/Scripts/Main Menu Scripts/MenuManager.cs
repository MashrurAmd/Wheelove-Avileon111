using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Config Data")]
    public GameSettingsData settingsData; // ScriptableObject reference

    [Header("Panels")]
    public GameObject chooseRegionPanel;
    public GameObject chooseModePanel;
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("UI References")]
    public TMP_Dropdown regionDropdown;
    public TMP_Dropdown languageDropdown;
    public TMP_Text modeText;
    public TMP_Text regionText;
    public TMP_Text languageText;

    [Header("Back Buttons")]
    public Button regionBackButton;
    public Button modeBackButton;

    private int currentRegionIndex = -1;
    private bool comingFromMainMenu = false;

    private void Start()
    {
        if (PlayerSettingsManager.Instance.IsFirstLaunch())
        {
            ShowChooseRegion(false); // first launch, no back button
        }
        else
        {
            ShowMainMenu();
        }
    }

    // REGION + LANGUAGE
    public void ShowChooseRegion(bool fromMainMenu)
    {
        HideAll();
        chooseRegionPanel.SetActive(true);
        comingFromMainMenu = fromMainMenu;

        // Show or hide back button
        regionBackButton.gameObject.SetActive(fromMainMenu);

        // Populate regions dropdown
        regionDropdown.ClearOptions();
        var regionNames = settingsData.regions.ConvertAll(r => r.regionName);
        regionDropdown.AddOptions(regionNames);

        // Listen for region change
        regionDropdown.onValueChanged.RemoveAllListeners();
        regionDropdown.onValueChanged.AddListener(OnRegionChanged);

        // Preselect saved region if coming from main menu
        if (fromMainMenu)
        {
            int savedRegionIndex = settingsData.regions.FindIndex(r => r.regionName == PlayerSettingsManager.Instance.selectedRegion);
            if (savedRegionIndex >= 0)
                regionDropdown.value = savedRegionIndex;
            else
                regionDropdown.value = 0;
        }
        else
        {
            regionDropdown.value = 0;
        }

        OnRegionChanged(regionDropdown.value);
    }

    private void OnRegionChanged(int regionIndex)
    {
        currentRegionIndex = regionIndex;
        PlayerSettingsManager.Instance.selectedRegion = settingsData.regions[regionIndex].regionName;

        // Update languages based on region
        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(settingsData.regions[regionIndex].languages);

        // Listen for language change
        languageDropdown.onValueChanged.RemoveAllListeners();
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Preselect saved language if coming from main menu
        if (comingFromMainMenu)
        {
            int savedLangIndex = settingsData.regions[regionIndex].languages.FindIndex(l => l == PlayerSettingsManager.Instance.selectedLanguage);
            languageDropdown.value = savedLangIndex >= 0 ? savedLangIndex : 0;
        }
        else
        {
            languageDropdown.value = 0;
            PlayerSettingsManager.Instance.selectedLanguage = settingsData.regions[regionIndex].languages[0];
        }
    }

    private void OnLanguageChanged(int languageIndex)
    {
        PlayerSettingsManager.Instance.selectedLanguage =
            settingsData.regions[currentRegionIndex].languages[languageIndex];
    }

    public void ConfirmRegionAndLanguage()
    {
        PlayerSettingsManager.Instance.Save();
        if (comingFromMainMenu)
            ShowMainMenu();
        else
            ShowChooseMode(false);
    }

    // MODE
    public void SetMode(int modeIndex)
    {
        PlayerSettingsManager.Instance.selectedMode = settingsData.modes[modeIndex];
        PlayerSettingsManager.Instance.Save();

        if (comingFromMainMenu)
            ShowMainMenu();
        else
            ShowMainMenu();
    }

    public void ShowChooseMode(bool fromMainMenu)
    {
        HideAll();
        chooseModePanel.SetActive(true);
        comingFromMainMenu = fromMainMenu;

        // Show or hide back button
        modeBackButton.gameObject.SetActive(fromMainMenu);
    }

    // MAIN MENU
    public void ShowMainMenu()
    {
        HideAll();
        mainMenuPanel.SetActive(true);

        regionText.text = $"Region: {PlayerSettingsManager.Instance.selectedRegion}";
        modeText.text = $"Mode: {PlayerSettingsManager.Instance.selectedMode}";
        languageText.text = $"Language: {PlayerSettingsManager.Instance.selectedLanguage}";
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
    public void SwitchScreen(int screenId)
    {
        SceneManager.LoadScene(screenId);
    }
}