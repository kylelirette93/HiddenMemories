using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    GameState previousState;
    UIManager uiManager;
    public GameObject playerPrefab;
    LevelManager levelManager;
    Camera sceneCamera;
    InputManager input;
    bool isPaused = false;
    public GameObject playerInstance;
    private void Start()
    {
        sceneCamera = GameManager.Instance.sceneCamera;
        uiManager = GameManager.Instance.uiManager;
        levelManager = GameManager.Instance.levelManager;
        if (uiManager != null)
        {
            currentState = GameState.MainMenu;
        }
        input = GameManager.Instance.inputManager;
        if (input != null)
        {
            input.PauseEvent += PauseGame;
        }
    }

    public void ChangeState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;

        switch (currentState)
        {
            case GameState.MainMenu:
                uiManager.DisableAllMenuUI();
                uiManager.EnableMainMenuUI();
                break;
            case GameState.Settings:
                uiManager.DisableAllMenuUI();
                uiManager.EnableSettingsUI();
                break;
            case GameState.Gameplay:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                uiManager.DisableAllMenuUI();
                uiManager.EnableGameplayUI();
                break;
            case GameState.Pause:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                uiManager.DisableAllMenuUI();
                uiManager.EnablePauseUI();
                break;
            case GameState.Options:
                uiManager.DisableAllMenuUI();
                uiManager.EnableOptionsUI();
                break;
            case GameState.Upgrades:
                uiManager.DisableAllMenuUI();
                uiManager.EnableUpgradeUI();
                break;
            case GameState.Results:
                uiManager.DisableAllMenuUI();
                uiManager.EnableResultUI();
                break;
            case GameState.GameWin:
                uiManager.DisableAllMenuUI();
                uiManager.EnableGameWinUI();
                break;
        }
    }

    public void PlayGame()
    {
        sceneCamera.gameObject.SetActive(false);
        ChangeState(GameState.Gameplay);
        SpawnPlayer();
    }
    public void SpawnPlayer()
    {
        Vector3 spawnPosition = levelManager.SpawnPoint;
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        StateActions.PlayerSpawned?.Invoke(playerInstance);
    }
    public void Upgrades()
    {
        ChangeState(GameState.Upgrades);
    }

    public void PauseGame()
    {
        if (isPaused)
        {
            ResumeGame(); 
        }
        else
        {
            previousState = currentState;
            ChangeState(GameState.Pause);
            isPaused = true;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()     
    {
        ChangeState(GameState.Gameplay);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void GoBack()
    {
        ChangeState(previousState);
    }

    public void MainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void Settings()
    {
        ChangeState(GameState.Settings);
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
    Settings,
    Gameplay,
    Pause,
    Options,
    Upgrades,
    Results,
    GameWin
}

public static class StateActions
{
    public static Action<GameObject> PlayerSpawned;
}
