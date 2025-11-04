using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
[DefaultExecutionOrder(2)]
public class PlayerHealthBar : MonoBehaviour
{
    public Slider playerHealthBar;
    public TextMeshProUGUI healthText;
    PlayerHealth health;
    bool isHealing = false;
    public Image fillImage;


    private void OnEnable()
    {
        health = FindFirstObjectByType<PlayerHealth>();
        InitializeHealthBar();
        InitializeHealthText();
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthBar;
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthText;
    }

    private void OnDisable()
    {
        PlayerHealthActions.OnPlayerHealthChanged -= UpdateHealthBar;
        PlayerHealthActions.OnPlayerHealthChanged -= UpdateHealthText;
    }

    private void Start()
    {
        InitializeHealthBar();
        InitializeHealthText();
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthBar;
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthText;
    }

    private void OnDestroy()
    {
        PlayerHealthActions.OnPlayerHealthChanged -= UpdateHealthBar;
        PlayerHealthActions.OnPlayerHealthChanged -= UpdateHealthText;
    }
    /// <summary>
    /// For initializing a health bar at start of scene.
    /// </summary>
    public void InitializeHealthBar()
    {
        if (playerHealthBar == null || health == null) return;
        playerHealthBar.maxValue = health.MaxHealth;
        playerHealthBar.value = health.CurrentHealth;
    }

    public void InitializeHealthText()
    {
        if (healthText == null || health == null) return;
        healthText.text = "HP: " + health.CurrentHealth + " / " + health.MaxHealth;
    }

    private void UpdateHealthBar(int newHealth, int maxHealth)
    {
        if (newHealth >= maxHealth) isHealing = true;
        float targetValue = (float)newHealth / (float)maxHealth;

        playerHealthBar.DOKill();
        playerHealthBar.DOValue(targetValue * playerHealthBar.maxValue, 0.25f);

        if (fillImage != null)
        {
            if (!isHealing)
            {
                fillImage.DOKill();
                fillImage.color = Color.green;
                fillImage.DOColor(Color.red, 0.1f).OnComplete(() =>
                {
                    fillImage.DOColor(Color.green, 0.5f);
                });
            }
            else
            {
                fillImage.DOKill();
                fillImage.color = Color.green;
                fillImage.DOColor(Color.HSVToRGB(120f / 360f, 0.3f, 1f), 0.1f).OnComplete(() =>
                {
                    fillImage.DOColor(Color.green, 0.5f);
                    isHealing = false;
                });
            }
        }
    }

    private void UpdateHealthText(int newHealth, int maxHealth)
    {
        healthText.text = "HP: " + newHealth + " / " + maxHealth;
    }
}
