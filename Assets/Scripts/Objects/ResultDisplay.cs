using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI doorsText;

    private void OnEnable()
    {
        resultText.text = "";
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

            // Count how many doors are actually opened (true values)
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
    }
}
