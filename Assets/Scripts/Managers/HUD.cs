using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    public RectTransform soulMeterRect;
    private Vector3 originalScale;
    public Slider soulMeterSlider;
    public TextMeshProUGUI currencyText;
    public Slider healthBarSlider;
    public TextMeshProUGUI popupText;

    [Header("Key Display")]
    public Image keyIcon;

    [Header("Potion Display")]
    public TextMeshProUGUI potionText;

    private void Start()
    {
        PlayerStats.Instance.OnSoulGained += ScaleSoulSlider;
        originalScale = soulMeterRect.localScale;
    }

    private void OnEnable()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.OnSoulGained += ScaleSoulSlider;
    }

    private void OnDisable()
    {
        popupText.DOKill();
        soulMeterSlider.DOKill();
        soulMeterRect.DOKill();
        popupText.text = "";
        popupText.gameObject.SetActive(false);
        PlayerStats.Instance.OnSoulGained -= ScaleSoulSlider;
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        popupText.DOKill();
        soulMeterSlider.DOKill();
        soulMeterRect.DOKill();
        popupText.text = "";
        popupText.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void Update()
    {
        if (PlayerStats.Instance != null)
        {
            currencyText.text = "$" + GameManager.Instance.currencyManager.Currency.ToString();
        }
    }

    public void UpdateSlider(int newSoulHealth, int maxSoulHealth) 
    {
        soulMeterSlider.DOKill();
        float targetValue = (float)newSoulHealth / (float)maxSoulHealth;

        soulMeterSlider.DOValue(targetValue, 0.5f).SetEase(Ease.OutCubic);
    }

    public void UpdateSliderColor()
    {
        Image fillImage = soulMeterSlider.fillRect.GetComponent<Image>();
        fillImage.DOKill();
        fillImage.color = Color.white;
        fillImage.DOColor(Color.blue, 0.1f).OnComplete(() =>
        {
            fillImage.DOColor(Color.white, 0.5f);
        });
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

    public void ScaleSoulSlider()
    {
        soulMeterRect.DOKill();
        soulMeterRect.DOScale(originalScale * 1.05f, 0.3f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                soulMeterRect.DOScale(originalScale, 0.3f).SetEase(Ease.InBack);
            });
    }
    public void InitiatePopup(string text, Vector2 anchoredPosition, bool isReloading)
    {
        RectTransform rect = popupText.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPosition;

        StartCoroutine(ShowPopupText(text, isReloading));
    }

    public void DisplayReloadText()
    {
        InitiatePopup("Press R to reload", Vector2.zero, true);
    }

    public void RemoveReloadText()
    {
        if (popupText != null)
        {
            popupText.transform.DOKill();
            popupText.transform.localScale = Vector3.one;
            popupText.text = "";
            popupText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowPopupText(string text, bool isReloading)
    {
        popupText.text = text;
        popupText.transform.localScale = Vector3.one;
        popupText.gameObject.SetActive(true);

        popupText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        if (!isReloading)
        {
            yield return new WaitForSeconds(2.5f);

            popupText.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                popupText.gameObject.SetActive(false);
            });
            yield return new WaitForSeconds(0.5f);
        }
        if (isReloading)
        {
            // Redisplay reload text.
            DisplayReloadText();
        }

    }
}