using UnityEngine;

public class Rifle : WeaponBase
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

    public void AddPowerRate(float amount)
    {
        powerRate += amount;
    }

    public void ReduceReloadSpeed(float amount)
    {
        reloadSpeed -= amount;
    }
}
