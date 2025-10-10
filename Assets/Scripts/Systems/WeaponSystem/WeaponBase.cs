using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponBase : MonoBehaviour
{
    // Base properties for all weapons.
    protected WeaponDataSO weaponData;
    protected PlayerController playerController;
    public int CurrentAmmo { get { return currentAmmo; } }
    protected int currentAmmo;
    public int ClipCapacity { get { return clipCapacity; } }
    protected int clipCapacity;
    protected float reloadSpeed;
    protected float fireRate;
    protected float lastShotTime;
    protected int spreadCount;
    protected float spreadAngle = 0f;
    protected float powerRate;
    protected float recoil;
    protected InputManager input;
    protected Transform firePoint;
    protected float bulletSpeed = 200f;
    protected AudioClip fireSound;
    protected Camera playerCamera;
    protected GameObject bulletPrefab;
    protected Transform weaponParent;
    ParticleSystem muzzleFlash;
    protected RectTransform crosshairUI;
    public bool IsReloading { get { return isReloading; } }
    protected bool isReloading = false;

    protected Vector2 targetRecoil;
    protected Vector2 currentRecoil;
    protected float recoilRecoverySpeed = 8f;
    public List<UpgradeDataSO> AvailableUpgrades { get { return availableUpgrades; } }
    [SerializeField] protected List<UpgradeDataSO> availableUpgrades = new List<UpgradeDataSO>();

    public bool IsUnlocked { get { return isUnlocked; } }
    protected bool isUnlocked = false;
    bool isShootingHeld = false;

    public virtual void Awake()
    {
        input = GameManager.Instance.inputManager;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        weaponParent = GameObject.Find("WeaponParent").transform;
        crosshairUI = GameObject.Find("Crosshair").GetComponent<RectTransform>();
    }

    public virtual void OnEnable()
    {
        input.ShootEvent += OnShoot;
        ApplyAllUpgrades();
        currentAmmo = clipCapacity;
    }

    public virtual void OnDisable()
    {
        input.ShootEvent -= OnShoot;
    }   

    public virtual void OnDestroy()
    {
        input.ShootEvent -= OnShoot;
    }
    public virtual void Initialize(WeaponDataSO data)
    {
        UnlockWeapon();
        // Weapon is initialized with stats after being instantiated. 
        weaponData = data;
        currentAmmo = weaponData.clipCapacity;
        reloadSpeed = weaponData.reloadSpeed;
        clipCapacity = weaponData.clipCapacity;
        fireRate = weaponData.fireRate;
        fireSound = weaponData.gun_fire;
        fireSound.name = weaponData.name + "_fire";
        bulletPrefab = weaponData.bulletPrefab;
        spreadCount = weaponData.spread;
        spreadAngle = weaponData.spreadAngle;
        powerRate = (int)weaponData.powerRate;
        recoil = weaponData.recoil;
        lastShotTime = -1;
        firePoint = transform.Find("Firepoint");
        muzzleFlash = GetComponentInChildren<ParticleSystem>();

        ResetToBaseStats();
        ApplyAllUpgrades();
    }

    public void UnlockWeapon()
    {
        isUnlocked = true;
        WeaponActions.UnlockWeapon?.Invoke(this);
    }

    protected void ResetToBaseStats()
    {
        if (weaponData == null) return;
        clipCapacity = weaponData.clipCapacity;
        currentAmmo = clipCapacity;
        reloadSpeed = weaponData.reloadSpeed;
        fireRate = weaponData.fireRate;
        spreadCount = weaponData.spread;
        spreadAngle = weaponData.spreadAngle;
        powerRate = (int)weaponData.powerRate;
        recoil = weaponData.recoil;
    }

    public virtual void Update()
    {
        if (weaponData == null || firePoint == null) return;

        if (isShootingHeld && !isReloading)
        {
            HandleShooting();
        }

        // Set crosshair in center of screen.
        crosshairUI.sizeDelta = new Vector2(40, 40);
        crosshairUI.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
    }

    protected void HandleShooting()
    {
        if (weaponData != null && Time.time < lastShotTime + (1f / fireRate))
        {
            return;
        }
        if (currentAmmo > 0)
        {
            lastShotTime = Time.time;
            currentAmmo--;

            if (fireSound != null)
            {
                ApplyShootRecoil();
                GameManager.Instance.audioManager.PlaySFX(fireSound);
                muzzleFlash?.Play();
            }

            Debug.Log("Firing weapon: " + name + ". Ammo left: " + currentAmmo);

            // Gives a little pop effect for crosshair when firing.
            crosshairUI.DOSizeDelta(new Vector2(60, 60), 0.1f).OnComplete(() =>
            {
                crosshairUI.DOSizeDelta(new Vector2(40, 40), 0.8f);
            });

            // Cast a ray from center of screen.
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                targetPoint = hit.point;
            }
            else
            {
                // If nothing gets hit, set it to a point far away.
                targetPoint = ray.GetPoint(1000f);
            }

            Vector3 baseDirection = (targetPoint - firePoint.position).normalized;
            Quaternion baseRotation = Quaternion.LookRotation(baseDirection);

            for (int i = 0; i < spreadCount; i++)
            {
                float randomAngle = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
                Quaternion spreadRot = Quaternion.Euler(0, randomAngle, 0f);

                Quaternion finalRot = baseRotation * spreadRot;

                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, finalRot);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Damage = powerRate;
                }
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

                // Fire the bullet. No need to normalize here as the velocity handles the speed.
                bulletRb.linearVelocity = bullet.transform.forward * bulletSpeed;
            }
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    protected void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started) isShootingHeld = true;
        if (context.canceled) isShootingHeld = false;
    }
    protected IEnumerator Reload()
    {
        for (int i = currentAmmo; i < clipCapacity; i++)
        {
            isReloading = true;
            // Small delay to simulate reloading time.
            // TODO: Add reload sound effect, add reload animation.
            yield return new WaitForSeconds(1f * reloadSpeed);
            Debug.Log("Reloading... " + (i + 1) + "/" + clipCapacity);
            currentAmmo++;
            currentAmmo = Mathf.Clamp(currentAmmo, 0, clipCapacity);
        }
        isReloading = false;
    }

    protected void ApplyShootRecoil()
    {
        playerController.DisableLook();
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localEulerAngles;

        float recoilRot = -5f;
        float recoilBack = -0.3f;

        // Create recoil based on stats, and store previous rotation of camera.
        float cameraRecoil = recoil * 2f;
        Vector3 cameraOriginalRot = playerCamera.transform.localEulerAngles;

        Sequence recoilSequence = DOTween.Sequence();

        // Rotate up & move back at the same time.
        recoilSequence.Append(transform.DOLocalRotate(new Vector3(recoilRot, 0f, 0f), 0.05f));
        recoilSequence.Join(transform.DOLocalMoveZ(originalPos.z + recoilBack, 0.05f));

        recoilSequence.Join(playerCamera.transform.DOLocalRotate(new Vector3(-cameraRecoil, cameraRecoil, 0f), 0.1f, RotateMode.LocalAxisAdd));

        // Return to original position & rotation.
        recoilSequence.Append(transform.DOLocalRotate(originalRot, 0.1f));
        recoilSequence.Join(transform.DOLocalMoveZ(originalPos.z, 0.1f));

        recoilSequence.Join(playerCamera.transform.DOLocalRotate(new Vector3(cameraRecoil, -cameraRecoil, 0f), 0.15f, RotateMode.LocalAxisAdd)).SetEase(Ease.OutQuad);

        recoilSequence.OnComplete(() => playerController.EnableLook());
    }

    protected virtual void ApplyAllUpgrades()
    {
        ResetToBaseStats();
        // Check weapon for applicable upgrades and apply them.
        foreach (var upgrade in availableUpgrades)
        {
            int tier = GameManager.Instance.upgradeManager.GetUpgradeTier(upgrade);
            if (tier > 0)
            {
                upgrade.Upgrade(this, tier);
            }
        }
    }
}

public static class WeaponActions
{
    public static Action<WeaponBase> UnlockWeapon;
}
