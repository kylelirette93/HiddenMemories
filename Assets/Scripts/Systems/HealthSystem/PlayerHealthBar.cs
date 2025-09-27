using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider playerHealthBar;
    PlayerHealth health;


    private void Start()
    {
        health = GetComponent<PlayerHealth>();
        playerHealthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        PlayerHealthActions.OnPlayerHealthChanged += UpdateHealthBar;
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

    private void UpdateHealthBar(int newHealth, int maxHealth)
    {
        playerHealthBar.value = (float)newHealth / (float)maxHealth;
    }
}
