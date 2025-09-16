using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    int currentSoulAmount;
    int soulMeterMax = 100;

    private void Start()
    {
        currentSoulAmount = soulMeterMax;
    }
    public void AddSoul(int amount)
    {
        currentSoulAmount += amount;
    }
}
