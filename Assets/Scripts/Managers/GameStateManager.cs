using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour, IDataPersistence
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
    UI uiInput;
    int timesWon = 0;
    private void Awake()
    {
        if (uiInput == null) uiInput = new UI();

        if (uiInput != null) uiInput.Controls.Pause.performed += ctx => PauseGame();
    }

    private void OnEnable()
    {
        uiInput.Controls.Enable();
    }

    private void OnDisable()
    {
        uiInput.Controls.Disable();
    }

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
            case GameState.Credits:
                uiManager.DisableAllMenuUI();
                uiManager.EnableCreditsUI();
                break;
            case GameState.Instructions:
                uiManager.DisableAllMenuUI();
                uiManager.EnableInstructionsUI();
                break;
            case GameState.Instructions2:
                uiManager.DisableAllMenuUI();
                uiManager.EnableSecondInstructionsUI();
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
                GameManager.Instance.progressManager.Reset();
                break;
            case GameState.Pause:
                playerController.lookEnabled = false;
                Time.timeScale = 0f;
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
        GameManager.Instance.dataPersistenceManager.LoadGame();
        spawnManager.SpawnGuns();
        spawnManager.SpawnKeys();
    }


    public void SpawnPlayer()
    {
        Vector3 spawnPosition = levelManager.SpawnPoint;
        playerInstance = Instantiate(playerPrefab, spawnPosition, playerPrefab.transform.rotation);
        playerInstance.transform.rotation = levelManager.SpawnRotation;
        StateActions.PlayerSpawned?.Invoke(playerInstance);
        PlayerHealthActions.PlayerDied += HandlePlayerDeath;
        playerController = playerInstance.GetComponent<PlayerController>();
    }

    public void ResetPlayer()
    {
        Debug.Log("Resetting player");
        sceneCamera.gameObject.SetActive(false);
        ChangeState(GameState.Gameplay);
        playerInstance.transform.position = levelManager.SpawnPoint;
        playerInstance.transform.rotation = levelManager.SpawnRotation;
        playerInstance.SetActive(true);
        if (playerInstance != null)
        {
            Camera playerCam = playerInstance.GetComponentInChildren<Camera>();
            if (playerCam != null) playerCam.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (playerController != null)
        {
            playerController.lookEnabled = true;
        }
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
            playerController.lookEnabled = false;
            ResumeGame();
        }
        else if (!isPaused && currentState == GameState.Gameplay)
        {
            previousState = currentState;
            ChangeState(GameState.Pause);
            isPaused = true;
        }
    }

    public void ResumeGame()     
    {
        playerController.lookEnabled = true;
        ChangeState(GameState.Gameplay);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Instructions()
    {
        ChangeState(GameState.Instructions);
    }

    public void Instructions2()
    {
        ChangeState(GameState.Instructions2);
    }

    public void GoBack()
    {
        ChangeState(previousState);
    }

    public void MainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    public void Credits()
    {
        ChangeState(GameState.Credits);
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
        gameInitialized = false;
        StateActions.Reset?.Invoke();
        ChangeState(GameState.Results);
    }
    public void GameWin()
    {
        timesWon++;
        GameManager.Instance.dataPersistenceManager.NewGamePlusSaveClear();
        GameManager.Instance.dataPersistenceManager.SaveGame();
        StateActions.Reset?.Invoke();
        // Rebind event to spawn guns before game restarts.
        gameInitialized = false;
        spawnManager.ClearPickups();
        spawnManager.CloseDoors();
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
    }
    private void OnDestroy()
    {
        PlayerHealthActions.PlayerDied -= HandlePlayerDeath;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadData(GameData data)
    {
        timesWon = data.timesWon;
        Debug.Log(timesWon);
    }

    public void SaveData(ref GameData data)
    {
        data.timesWon = timesWon;
    }
}
public enum GameState
{
    MainMenu,
    Credits,
    Instructions,
    Instructions2,
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
