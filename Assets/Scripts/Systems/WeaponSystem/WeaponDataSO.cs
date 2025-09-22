using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Weapons", order = 1)]
public class WeaponDataSO : ItemDataSO
{
    // Holds data tied to specific weapon.
    float powerRate;
    float fireRate;
    int clipCapacity;
    float reloadSpeed;
    int spread = 0;
}
