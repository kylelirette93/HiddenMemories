using UnityEngine;

public interface IUpgrade
{
    public void Purchase();
    public void Upgrade(WeaponBase weapon, int tier);
}
