using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Camera sceneCamera;
    public GameStateManager gameStateManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public ProgressManager progressManager;
    public UpgradeManager upgradeManager;
    public InputManager inputManager;
    public LevelManager levelManager;
    public HUD hud;
    public SpawnManager spawnManager;
    public CurrencyManager currencyManager;
    public DataPersistenceManager dataPersistenceManager;

    private void Awake()
    {
        #region Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        #endregion

        #region CacheReferences
        sceneCamera = Camera.main;
        gameStateManager = GetComponentInChildren<GameStateManager>();
        inputManager = GetComponentInChildren<InputManager>();
        uiManager = GetComponentInChildren<UIManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        progressManager = GetComponentInChildren<ProgressManager>();
        upgradeManager = GetComponentInChildren<UpgradeManager>();
        levelManager = GetComponentInChildren<LevelManager>();
        spawnManager = GetComponentInChildren<SpawnManager>();
        currencyManager = GetComponentInChildren<CurrencyManager>();
        dataPersistenceManager = GetComponentInChildren<DataPersistenceManager>();
        #endregion
    }
}
