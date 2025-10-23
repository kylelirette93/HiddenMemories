using TMPro;
using UnityEngine;
[DefaultExecutionOrder(50)]
public class FileButtonHandler : MonoBehaviour
{
    public DataPersistenceManager dataPersistenceManager;
    public TextMeshProUGUI buttonText;

    private void OnEnable()
    {
        if (dataPersistenceManager.DoesSaveDataExist())
        {
            dataPersistenceManager.LoadGame();
            GameData data = dataPersistenceManager.GetGameData();
            if (data.timesWon > 0)
            {
                buttonText.text = "New Game+";
                return;
            }
            else
            {
                buttonText.text = "Continue";
            }
        }
        else
        {
            buttonText.text = "New Game";
        }
        
    }
}
