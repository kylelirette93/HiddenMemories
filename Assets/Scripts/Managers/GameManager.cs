using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStateManager gameStateManager;
    public UIManager uiManager;
    public AudioManager audioManager;
    public ProgressManager progressManager;
    public UpgradeManager upgradeManager;

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
        gameStateManager = GetComponentInChildren<GameStateManager>();
        uiManager = GetComponentInChildren<UIManager>();
        audioManager = GetComponentInChildren<AudioManager>();
        progressManager = GetComponentInChildren<ProgressManager>();
        upgradeManager = GetComponentInChildren<UpgradeManager>();
        #endregion
    }
}
