using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Weapons", order = 1)]
public class WeaponDataSO : ItemDataSO
{
    // Holds data tied to specific weapon.
    public float powerRate;
    public float fireRate;
    public int clipCapacity;
    public float reloadSpeed;
    public int spread = 0;
}
