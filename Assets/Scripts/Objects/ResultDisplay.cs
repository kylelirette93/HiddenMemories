using TMPro;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI doorsText;

    private void OnEnable()
    {
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
            int doorCount = gameData.doorsOpened.Count;
            int doorsLeft = 2 - doorCount;
            if (doorsLeft > 0)
            {
                doorsText.text = doorCount.ToString() + "/2 doors opened." + "\n" + doorsLeft + " doors left to open";
            }
            else
            {
                doorsText.text = "All doors unlocked, find the exit!";
            }
        }
    }
}
