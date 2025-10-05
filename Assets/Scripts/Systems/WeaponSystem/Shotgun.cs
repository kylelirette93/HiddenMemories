using UnityEngine;

public class Shotgun : WeaponBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnDisable();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    public override void Initialize(WeaponDataSO data)
    {
        base.Initialize(data);
    }

    public override void Update()
    {
        base.Update();
    }


    public void AddClipCapacity(int amount)
    {
        clipCapacity += amount;
        currentAmmo += amount;
    }

    public void AddFireRate(float amount)
    {
        // Add fire rate as percentage.
        fireRate += (int)(fireRate * amount);
    }
}
