using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    GameState previousState;
    UIManager uiManager;

    private void Start()
    {
        uiManager = GameManager.Instance.uiManager;
        if (uiManager != null)
        {
            currentState = GameState.MainMenu;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.MainMenu:
                uiManager.EnableMainMenuUI();
                break;
            case GameState.Gameplay:
                Cursor.visible = false;
                uiManager.EnableGameplayUI();
                break;
            case GameState.Pause:
                uiManager.EnablePauseUI();
                break;
            case GameState.Options:
                uiManager.EnableOptionsUI();
                break;
            case GameState.Upgrades:
                uiManager.EnableUpgradeUI();
                break;
            case GameState.Results:
                uiManager.EnableResultUI();
                break;
            case GameState.GameWin:
                uiManager.EnableGameWinUI();
                break;
        }
    }

    public void PlayGame()
    {
        ChangeState(GameState.Gameplay);
    }

    public void Upgrades()
    {
        ChangeState(GameState.Upgrades);
    }

    public void PauseGame()
    {
        previousState = currentState;
        ChangeState(GameState.Pause);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()     
    {
        ChangeState(previousState);
        Time.timeScale = 1f;
    }

    public void Options()
    {
        ChangeState(GameState.Options);
    }
    public void Results()
    {
        ChangeState(GameState.Results);
    }
    public void GameWin()
    {
        ChangeState(GameState.GameWin);
    }
}
public enum GameState
{
    MainMenu,
    Gameplay,
    Pause,
    Options,
    Upgrades,
    Results,
    GameWin
}
