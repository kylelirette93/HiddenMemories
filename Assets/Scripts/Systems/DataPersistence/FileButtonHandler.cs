using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(50)]
public class FileButtonHandler : MonoBehaviour
{
    public DataPersistenceManager dataPersistenceManager;
    public RectTransform button;
    public TextMeshProUGUI buttonText;

    private void OnEnable()
    {
        if (dataPersistenceManager.DoesSaveDataExist())
        {
            dataPersistenceManager.LoadGame();
            GameData data = dataPersistenceManager.GetGameData();
            if (data.timesWon > 0)
            {
                string ngplus = "";
                for (int i = 0; i < data.timesWon; i++) 
                {
                    ngplus += "+";
                }

                button.sizeDelta = new Vector2(300, 30);
                buttonText.text = "Continue(NG" + ngplus + ")";

                return;
            }
            else
            {
                button.sizeDelta = new Vector2(140, 30);
                buttonText.text = "Continue";
            }
        }
        else
        {
            button.sizeDelta = new Vector2(140, 30);
            buttonText.text = "New Game";
        }        
    }
}
