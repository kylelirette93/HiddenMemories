using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Slider soulMeterSlider;
    public TextMeshProUGUI currencyText;



    public void Update()
    {
        if (PlayerStats.Instance != null)
        {
            soulMeterSlider.value = (float)PlayerStats.Instance.SoulHealth / (float)PlayerStats.Instance.MaxSoulHealth;
            currencyText.text = "$" + PlayerStats.Instance.Currency.ToString();
        }

    }
}
