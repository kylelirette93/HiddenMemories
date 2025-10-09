using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider playerHealthBar;
    public TextMeshProUGUI healthText;
    PlayerHealth health;


    private void OnEnable()
    {
        health = FindFirstObjectByType<PlayerHealth>();
        InitializeHealthBar();
        InitializeHealthText();
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthBar;
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthText;
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
        playerHealthBar.value = (float)newHealth / (float)maxHealth;
    }

    private void UpdateHealthText(int newHealth, int maxHealth)
    {
        healthText.text = "HP: " + newHealth + " / " + maxHealth;
    }
}
