using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    // Keep track of stats.
    int soulMeterMax = 100;
    HUD hud;

    private void Start()
    {
        hud = GameManager.Instance.hud;
    }

    private void Update()
    {
        hud.UpdateSoulMeter(soulMeterMax / 100f);
    }

}
