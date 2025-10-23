using TMPro;
using UnityEngine;

public class ResultDisplay : MonoBehaviour
{
    TextMeshProUGUI resultText;

    private void OnEnable()
    {
        resultText = GetComponentInChildren<TextMeshProUGUI>();
        if (resultText != null)
        {
            resultText.text = "Enemies killed: " + GameManager.Instance.progressManager.EnemiesKilled;
        }
    }
}
