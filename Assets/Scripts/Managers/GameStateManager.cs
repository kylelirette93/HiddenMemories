using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    GameState previousState;
    UIManager uiManager;

    private void Start()
    {
        uiManager = GameManager.Instance.uiManager;
    }

    public void ChangeState(GameState state)
    {
        if (currentState != previousState)
        {
            currentState = state;
        }
        previousState = currentState;
    }

    public void Update()
    {
        UpdateState(currentState);
    }

    public void UpdateState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                uiManager.EnableMainMenuUI();
                break;
            case GameState.Gameplay:
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
