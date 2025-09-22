using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    public WeaponDataSO currentWeapon;
    float powerRate;
    float fireRate;
    int clipCapacity;
    float reloadSpeed;
    float bulletSpeed = 300f;
    int spread = 0;
    public GameObject bulletPrefab;
    public Transform firePoint;
    InputManager input;

    private void Awake()
    {
        input = GameManager.Instance.inputManager;
        input.ShootEvent += Shoot;
        InteractableActions.AddWeapon += AddWeapon;
        PopulateWeaponData();
    }

    private void PopulateWeaponData()
    {
        if (currentWeapon != null)
        {
            powerRate = currentWeapon.powerRate;
            fireRate = currentWeapon.fireRate;
            clipCapacity = currentWeapon.clipCapacity;
            reloadSpeed = currentWeapon.reloadSpeed;
            spread = currentWeapon.spread;
        }
    }

    private void AddWeapon(ItemDataSO weapon)
    {
        if (weapon is WeaponDataSO _currentWeapon)
        {
            currentWeapon = _currentWeapon;
        }
        Debug.Log("Added weapon " + weapon);
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("I'm shooting right now.");
            int spreadCount = spread;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = transform.forward * bulletSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
