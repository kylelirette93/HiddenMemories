using TMPro;
using UnityEngine;

public class FileButtonHandler : MonoBehaviour
{
    public DataPersistenceManager dataPersistenceManager;
    public TextMeshProUGUI buttonText;

    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (dataPersistenceManager.DoesSaveDataExist())
        {
            buttonText.text = "Continue";
        }
        else
        {
            buttonText.text = "New Game";
        }
        
    }
}
