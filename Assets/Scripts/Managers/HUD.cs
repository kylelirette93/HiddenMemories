using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    public RectTransform soulMeterRect;
    public Slider soulMeterSlider;
    public TextMeshProUGUI currencyText;
    public Slider healthBarSlider;
    public TextMeshProUGUI popupText;

    [Header("Key Display")]
    public Image keyIcon;

    [Header("Potion Display")]
    public TextMeshProUGUI potionText;

    private Coroutine activePopupCoroutine;

    private void Awake()
    {
        PlayerStats.Instance.OnSoulGained += ScaleSoulSlider;
    }

    private void OnEnable()
    {
        PlayerStats.Instance.OnSoulGained += ScaleSoulSlider;
    }

    private void OnDisable()
    {
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
            activePopupCoroutine = null;
        }
        popupText.DOKill();
        popupText.text = "";
        popupText.gameObject.SetActive(false);
        PlayerStats.Instance.OnSoulGained -= ScaleSoulSlider;
    }

    private void OnDestroy()
    {
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
        }
        popupText.DOKill();
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

    public void ScaleSoulSlider()
    {
        soulMeterRect.DOScale(new Vector3(1.2f, 1.2f, 0), 2f).SetEase(Ease.InBack);
    }
    public void InitiatePopup(string text)
    {
        // Stop any existing popup
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
        }

        // Kill any active tweens on the popup text
        popupText.transform.DOKill();

        // Start new popup
        activePopupCoroutine = StartCoroutine(ShowPopupText(text));
    }

    public void DisplayReloadText()
    {
        InitiatePopup("Press R to reload");
    }

    public void RemoveReloadText()
    {
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
            activePopupCoroutine = null;
        }

        if (popupText != null)
        {
            popupText.transform.DOKill();
            popupText.transform.localScale = Vector3.one;
            popupText.text = "";
            popupText.gameObject.SetActive(false);
        }
    }

    private IEnumerator ShowPopupText(string text)
    {
        if (activePopupCoroutine != null)
        {
            StopCoroutine(activePopupCoroutine);
        }
        popupText.text = text;
        popupText.transform.localScale = Vector3.one;
        popupText.gameObject.SetActive(true);

        popupText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(2.5f);

        popupText.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            popupText.gameObject.SetActive(false);
        });

        yield return new WaitForSeconds(0.5f);
        activePopupCoroutine = null;
    }
}