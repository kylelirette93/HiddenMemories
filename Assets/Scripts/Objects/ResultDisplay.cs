using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI doorsText;
    public string deathResult;
    public TextMeshProUGUI deathText;

    private void OnEnable()
    {
        GameManager.Instance.dataPersistenceManager.SaveGame();
        if (resultText != null)
        {
            resultText.text = "Enemies killed this run: " + GameManager.Instance.progressManager.EnemiesKilled;
        }
        if (currencyText != null)
        {
            currencyText.text = "Currency earned this run: " + "$" + GameManager.Instance.progressManager.Currency;
        }
       if (doorsText != null)
       {  
            GameData gameData = GameManager.Instance.dataPersistenceManager.GetGameData();
            int doorCount = 0;

            // Get count of doors opened.
            foreach (bool doorOpened in gameData.doorsOpened)
            {
                if (doorOpened)
                {
                    doorCount++;
                }
            }

            int doorLeft = 2 - doorCount;

            if (doorCount >= 2)
            {
                doorsText.text = "All doors unlocked, find the exit!";
            }
            else
            {
                doorsText.text = doorCount.ToString() + "/2 doors opened. " + doorLeft + " doors left to open";
            }
       }
        if (deathText != null && deathResult != null)
        {
            deathText.text = GameManager.Instance.gameStateManager.lastDeathResult;
        }
    }

    private void OnDisable()
    {
        GameManager.Instance.gameStateManager.lastDeathResult = "";
    }
}
