using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HUD : MonoBehaviour
{
    public Slider soulMeterSlider;
    public TextMeshProUGUI currencyText;
    public Slider healthBarSlider;
    public TextMeshProUGUI popupText;

    [Header("Key Display")]
    public Image keyIcon;

    [Header("Potion Display")]
    public TextMeshProUGUI potionText;

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }


    public void Update()
    {
        if (PlayerStats.Instance != null)
        {
            soulMeterSlider.value = (float)PlayerStats.Instance.SoulHealth / (float)PlayerStats.Instance.MaxSoulHealth;
            currencyText.text = "$" + GameManager.Instance.currencyManager.Currency.ToString();
        }
    }

    public void AddKeyToHud()
    {
        keyIcon.enabled = true;
    }

    public void RemoveKeyFromHud()
    {
        keyIcon.enabled = false;
    }

    public void UpdatePotionCount(int count)
    {
        potionText.text = count.ToString();
    }
    public void InitiatePopup(string text)
    {
        StartCoroutine(ShowPopupText(text));
    }

    private IEnumerator ShowPopupText(string text)
    {
       // Do a fancy dot tween popup for text.
        popupText.text = text;
        popupText.transform.localScale = Vector3.zero;
        popupText.gameObject.SetActive(true);
        popupText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(2f);
        popupText.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() => popupText.gameObject.SetActive(false));
    }
}
