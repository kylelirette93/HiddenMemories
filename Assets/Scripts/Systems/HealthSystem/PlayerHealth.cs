using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using System.Collections;

[DefaultExecutionOrder(1)]
public class PlayerHealth : Health
{
    PlayerStats playerStats;

    [Header("Post processing effect")]
    Volume globalVol;
    Vignette vignette;
    [SerializeField] Color damageColor;
    [SerializeField] Color healColor;
    [SerializeField] float maxIntensity = 0.4f;
    [SerializeField] float maxHealIntensity = 0.6f;
    [SerializeField] float fadeDuration = 0.25f;
    [SerializeField] float fadeOutDuration = 0.25f;
    [SerializeField] float holdTime = 0.25f;

    public AudioClip oofSound;
    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        globalVol = FindFirstObjectByType<Volume>();
        if (globalVol.profile.TryGet<Vignette>(out vignette))
        {
            vignette.active = true;
            vignette.intensity.value = maxIntensity;
            vignette.color.overrideState = true;
            vignette.intensity.overrideState = true;

            vignette.intensity.value = 0f;
        }
    }
    public void OnEnable()
    {
        maxHealth = playerStats.MaxHealth;
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(maxHealth, maxHealth);
    }

    private void OnDisable()
    {
        currentHealth = playerStats.MaxHealth;
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public override void Start()
    {
        base.Start();
    }

    public override void TakeDamage(int damage)
    {
        GameManager.Instance.audioManager.PlaySound("oof");
        currentHealth -= damage;
        StopAllCoroutines();
        StartCoroutine(DamageFlash());
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            PlayerHealthActions.PlayerDied?.Invoke();
            Heal(maxHealth);
        }
    }

    IEnumerator DamageFlash()
    {
        float startTime = Time.time;
        float progress = 0f;

        vignette.color.value = damageColor;
        vignette.intensity.value = maxIntensity;

        while (progress < 1f)
        {
            progress = (Time.time - startTime) / fadeDuration;
            vignette.intensity.value = Mathf.Lerp(0f, maxIntensity, progress);
            yield return null;
        }
        yield return new WaitForSeconds(holdTime);
        // Hold at max intensity.

        startTime = Time.time;
        progress = 0f;
        while (progress < 1f)
        {
            progress = (Time.time - startTime) / fadeOutDuration;
            vignette.intensity.value = Mathf.Lerp(maxIntensity, 0f, progress);
            yield return null;
        }
        vignette.intensity.value = 0f;
    }

    IEnumerator HealFlash()
    {
        float startTime = Time.time;
        float progress = 0f;

        vignette.color.value = healColor;

        while (progress < 1f)
        {
            progress = (Time.time - startTime) / fadeDuration;
            vignette.intensity.value = Mathf.Lerp(0f, maxHealIntensity, progress);
            yield return null;
        }

        // Hold at max intensity.
        yield return new WaitForSeconds(0.05f);

        startTime = Time.time;
        progress = 0f;
        while (progress < 1f)
        {
            progress = (Time.time - startTime) / fadeDuration;
            vignette.intensity.value = Mathf.Lerp(maxIntensity, 0f, progress);
            yield return null;
        }
        vignette.intensity.value = 0f;
    }


    public void Heal(int amount)    
    {
        StopAllCoroutines();
        currentHealth += amount;
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        PlayerHealthActions.OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        if (gameObject.activeInHierarchy)
        StartCoroutine(HealFlash());
    }
}
public static class PlayerHealthActions
{
    public static Action<int, int> OnPlayerHealthChanged;
    // Player died, notify game state manager.
    public static Action PlayerDied;
}
