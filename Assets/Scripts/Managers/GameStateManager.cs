using System;
using System.Data;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GameState currentState;
    GameState previousState;
    UIManager uiManager;
    public GameObject playerPrefab;
    LevelManager levelManager;
    Camera sceneCamera;
    InputManager input;
    SpawnManager spawnManager;
    bool isPaused = false;
    public GameObject playerInstance;
    public Transform initialSpawn;
    Quaternion spawnRotation;
    PlayerController playerController;
    // Whether or not game needs initialized, i.e spawning etc.
    bool gameInitialized = false;
    bool playerSpawned = false;
    
    private void Start()
    {
        sceneCamera = GameManager.Instance.sceneCamera;
        uiManager = GameManager.Instance.uiManager;
        levelManager = GameManager.Instance.levelManager;
        spawnManager = GameManager.Instance.spawnManager;
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
            case GameState.Instructions:
                uiManager.DisableAllMenuUI();
                uiManager.EnableInstructionsUI();
                break;
            case GameState.Controls:
                uiManager.DisableAllMenuUI();
                uiManager.EnableControlsUI();
                break;
            case GameState.Settings:
                uiManager.DisableAllMenuUI();
                uiManager.EnableSettingsUI();
                break;
            case GameState.Gameplay:
                Time.timeScale = 1;
                DisableCursor();
                if (!gameInitialized)
                {
                    StateActions.Start?.Invoke();
                    gameInitialized = true;
                }
                uiManager.DisableAllMenuUI();
                uiManager.EnableGameplayUI();
                break;
            case GameState.Pause:
                EnableCursor();
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
                EnableCursor();
                uiManager.DisableAllMenuUI();
                uiManager.EnableResultUI();
                break;
            case GameState.GameWin:
                EnableCursor();
                uiManager.DisableAllMenuUI();
                uiManager.EnableGameWinUI();
                break;
        }
    }

    private void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void PlayGame()
    {
        if (sceneCamera.isActiveAndEnabled)
        {
            sceneCamera.gameObject.SetActive(false);
        }
        ChangeState(GameState.Gameplay);

        if (!playerSpawned)
        {
            SpawnPlayer();
            playerSpawned = true;
        }
        else
        {
            ResetPlayer();
        }
    }


    public void SpawnPlayer()
    {
        Vector3 spawnPosition = levelManager.SpawnPoint;
        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        spawnRotation = playerInstance.transform.rotation;
        StateActions.PlayerSpawned?.Invoke(playerInstance);
        PlayerHealthActions.PlayerDied += HandlePlayerDeath;
        playerController = playerInstance.GetComponent<PlayerController>();
    }

    public void ResetPlayer()
    {
        sceneCamera.gameObject.SetActive(false);
        ChangeState(GameState.Gameplay);
        playerInstance.SetActive(true);
        playerInstance.transform.position = levelManager.SpawnPoint;
        playerInstance.transform.rotation = spawnRotation;
        StateActions.PlayerSpawned?.Invoke(playerInstance);
        PlayerHealthActions.PlayerDied += HandlePlayerDeath;
    }

    private void HandlePlayerDeath()
    {
        if (playerInstance != null)
        {
            playerInstance.SetActive(false);
        }
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        Results();
        PlayerHealthActions.PlayerDied -= HandlePlayerDeath;
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
        else if (!isPaused && currentState == GameState.Gameplay)
        {
            previousState = currentState;
            ChangeState(GameState.Pause);
            isPaused = true;
            Time.timeScale = 0f;
        }
    }

    public void ResumeGame()     
    {
        playerController.EnableLook();
        ChangeState(GameState.Gameplay);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Instructions()
    {
        ChangeState(GameState.Instructions);
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

    public void Controls()
    {
        ChangeState(GameState.Controls);
    }

    public void Options()
    {
        ChangeState(GameState.Options);
    }
    public void Results()
    {
        // If we go to results, reset the game.
        StateActions.Reset?.Invoke();
        ChangeState(GameState.Results);
        gameInitialized = false;
    }
    public void GameWin()
    {
        StateActions.Reset?.Invoke();
        // Rebind event to spawn guns before game restarts.
        spawnManager.RebindGunSpawnEvent();
        spawnManager.RespawnKeys();
        GameManager.Instance.upgradeManager.ClearUpgrades();
        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerSpawned = false;
        }
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        ChangeState(GameState.GameWin);
        gameInitialized = false;
    }
    private void OnDestroy()
    {
        PlayerHealthActions.PlayerDied -= HandlePlayerDeath;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
public enum GameState
{
    MainMenu,
    Instructions,
    Controls,
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
    public static Action Reset;
    public static Action Start;
}
