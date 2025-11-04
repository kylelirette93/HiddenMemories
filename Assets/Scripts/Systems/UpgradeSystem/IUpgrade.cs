using UnityEngine;

public interface IUpgrade
{
    public void Purchase();
    public void Apply(WeaponBase weapon, int tier);
}
