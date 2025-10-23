using TMPro;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    TextMeshProUGUI resultText;

    private void OnEnable()
    {
        resultText = GameObject.Find("TXT_ResultsText").GetComponent<TextMeshProUGUI>();
        if (resultText != null)
        {
            resultText.text = "Enemies killed this run: " + GameManager.Instance.progressManager.EnemiesKilled;
        }
    }
}
