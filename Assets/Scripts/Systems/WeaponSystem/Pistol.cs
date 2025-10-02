using UnityEngine;

public class Pistol : WeaponBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void OnEnable()
    {
        base.OnEnable();
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
}
