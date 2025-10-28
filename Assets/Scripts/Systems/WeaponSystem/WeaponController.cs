using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class WeaponController : MonoBehaviour
{
    public WeaponDataSO currentWeapon;
    public Transform weaponParent;
    private GameObject currentWeaponInstance = null;
    public Transform playerCamera;
    float powerRate;
    float fireRate;
    int clipCapacity;
    float reloadSpeed;
    float bulletSpeed = 150f;
    int spread = 0;
    // Sound when firing gun.
    AudioClip fireSound;
    public GameObject bulletPrefab;
    public Transform firePoint;
    InputManager input;
    ParticleSystem muzzleFlash;

    private void Awake()
    {
        input = GameManager.Instance.inputManager;
        input.ShootEvent += Shoot;
        InteractableActions.AddWeapon += AddWeapon;
    }

    private void OnDestroy()
    {
        input.ShootEvent -= Shoot;
        InteractableActions.AddWeapon -= AddWeapon;
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
            fireSound = currentWeapon.gun_fire;
            fireSound.name = currentWeapon.name + "_fire";
        }
    }

    private void AddWeapon(ItemDataSO weapon)
    {
        if (weapon is WeaponDataSO _currentWeapon)
        {
            currentWeapon = _currentWeapon;
            EquipWeapon();
        }
        Debug.Log("Added weapon " + weapon);
    }

    private void EquipWeapon()
    {
        /*if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }*/
        if (currentWeapon.weaponPrefab != null)
        {
            currentWeaponInstance = Instantiate(currentWeapon.weaponPrefab, weaponParent);
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            currentWeaponInstance.transform.localRotation = Quaternion.identity;
            muzzleFlash = currentWeaponInstance.GetComponentInChildren<ParticleSystem>();
            Transform newFirepoint = currentWeaponInstance.transform.Find("Firepoint");
            newFirepoint.transform.localScale = Vector3.one;
            newFirepoint.transform.localRotation = Quaternion.identity;
            if (newFirepoint != null)
            {
                firePoint = newFirepoint;
            }
            else
            {
                Debug.Log("Fire point is null");
            }
        }
        PopulateWeaponData();
    }

    private void Update()
    {
        weaponParent.transform.rotation = playerCamera.transform.rotation;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("I'm shooting right now.");
            int spreadCount = spread;
            if (fireSound != null)
            {
                ShootRecoil();

                GameManager.Instance.audioManager.PlaySound(fireSound.name);
                muzzleFlash?.Play();
            }
            if (bulletPrefab == null || firePoint == null)
            {
                Debug.LogWarning("Bullet prefab or fire point is not assigned.");
                return;
            }
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, weaponParent.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }

    public void ShootRecoil()
    {
        Vector3 originalPos = weaponParent.localPosition;
        Vector3 originalRot = weaponParent.localEulerAngles;

        float recoilRot = -5f;
        float recoilBack = -0.3f;

        Sequence recoilSequence = DOTween.Sequence();

        // Rotate up & move back at the same time
        recoilSequence.Append(weaponParent.DOLocalRotate(new Vector3(recoilRot, 0f, 0f), 0.05f));
        recoilSequence.Join(weaponParent.DOLocalMoveZ(originalPos.z + recoilBack, 0.05f));

        // Return to original position & rotation
        recoilSequence.Append(weaponParent.DOLocalRotate(originalRot, 0.1f));
        recoilSequence.Join(weaponParent.DOLocalMoveZ(originalPos.z, 0.1f));
    }

    private void OnDrawGizmos()
    {
        // Draw a line from the fire point forward to show the bullet direction.
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 10f);
        }
    }
}
