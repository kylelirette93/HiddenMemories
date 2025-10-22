using System.Collections.Generic;
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
    public float spreadAngle = 0;
    public float recoil = 0;
    public float bulletSpread = 0;
    public bool IsUnlocked = false;
    public GameObject weaponPrefab;
    public AudioClip gun_fire;
    public AudioClip gun_reload;
    public AudioClip gun_cock;
    public Vector3 firePoint;
    public int index;
    public GameObject bulletPrefab;
    public Sprite icon;
    public List<UpgradeDataSO> AvailableUpgrades = new List<UpgradeDataSO>();
}
