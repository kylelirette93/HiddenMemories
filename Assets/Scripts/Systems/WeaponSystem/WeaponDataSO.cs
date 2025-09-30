using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScritableObjects/Weapons", order = 1)]
public class WeaponDataSO : ItemDataSO
{
    // Holds data tied to specific weapon.
    public string name;
    public float powerRate;
    public float fireRate;
    public int clipCapacity;
    public float reloadSpeed;
    public int spread = 0;
    public GameObject weaponPrefab;
    public AudioClip gun_fire;
    public Vector3 firePoint;
    public int index;
    public GameObject bulletPrefab;
    public Sprite icon;
}
