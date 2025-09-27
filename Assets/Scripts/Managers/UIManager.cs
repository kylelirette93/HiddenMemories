using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject InstructionsPanel;
    public GameObject SettingsPanel;
    public GameObject GameplayPanel;
    public GameObject PausePanel;
    public GameObject OptionsPanel;
    public GameObject UpgradesPanel;
    public GameObject ResultsPanel;
    public GameObject GameWinPanel;

    public void DisableAllMenuUI()
    {
        // Disable all UI panels.
        MainMenuPanel.SetActive(false);
        InstructionsPanel.SetActive(false);
        SettingsPanel.SetActive(false);
        GameplayPanel.SetActive(false);
        PausePanel.SetActive(false);
        OptionsPanel.SetActive(false);
        UpgradesPanel.SetActive(false);
        ResultsPanel.SetActive(false);
        GameWinPanel.SetActive(false);
    }

    public void EnableMainMenuUI()
    {
        DisableAllMenuUI();
        MainMenuPanel.SetActive(true);
    }

    public void EnableInstructionsUI()
    {
        DisableAllMenuUI();
        InstructionsPanel.SetActive(true);
    }

    public void EnableSettingsUI()
    {
        DisableAllMenuUI();
        SettingsPanel.SetActive(true);
    }

    public void EnableGameplayUI()
    {
        DisableAllMenuUI();
        GameplayPanel.SetActive(true);
    }

    public void EnablePauseUI()
    {
        DisableAllMenuUI();
        PausePanel.SetActive(true);
    }

    public void EnableOptionsUI()
    {
        DisableAllMenuUI();
        OptionsPanel.SetActive(true);
    }

    public void EnableUpgradeUI()
    {
        DisableAllMenuUI();
        UpgradesPanel.SetActive(true);
    }

    public void EnableResultUI()
    {
        DisableAllMenuUI();
        ResultsPanel.SetActive(true);
    }

    public void EnableGameWinUI()
    {
        DisableAllMenuUI();
        GameWinPanel.SetActive(true);
    }
}
