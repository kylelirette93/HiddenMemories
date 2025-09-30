using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponBase : MonoBehaviour
{
    // Base properties for all weapons.
    protected WeaponDataSO weaponData;
    public int CurrentAmmo { get { return currentAmmo; } }
    protected int currentAmmo;
    protected float lastShotTime;
    protected InputManager input;
    protected Transform firePoint;
    protected float bulletSpeed = 200f;
    protected AudioClip fireSound;
    protected Camera playerCamera;
    protected GameObject bulletPrefab;
    protected Transform weaponParent;
    ParticleSystem muzzleFlash;
    protected RectTransform crosshairUI;
    private void Awake()
    {
        input = GameManager.Instance.inputManager;
        playerCamera = GameObject.Find("Camera").GetComponent<Camera>();
        weaponParent = GameObject.Find("WeaponParent").transform;
        crosshairUI = GameObject.Find("Crosshair").GetComponent<RectTransform>();
    }
    private void OnEnable()
    {
        input.ShootEvent += OnShoot;
    }

    private void OnDestroy()
    {
        input.ShootEvent -= OnShoot;
    }
    public void Initialize(WeaponDataSO data)
    {
        // Weapon is initialized with stats after being instantiated. 
        weaponData = data;
        currentAmmo = weaponData.clipCapacity;
        fireSound = weaponData.gun_fire;
        fireSound.name = weaponData.name + "_fire";
        bulletPrefab = weaponData.bulletPrefab;
        lastShotTime = -1;
        firePoint = transform.Find("Firepoint");
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
    }

    public void Update()
    {
        if (weaponData == null || firePoint == null) return;

        // Set crosshair in center of screen.
        crosshairUI.sizeDelta = new Vector2(40, 40);
        crosshairUI.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (weaponData != null && Time.time < lastShotTime + (1f / weaponData.fireRate))
            {
                return;
            }
            if (currentAmmo > 0)
            {
                lastShotTime = Time.time;
                currentAmmo--;

                if (fireSound != null)
                {
                    ShootRecoil();
                    GameManager.Instance.audioManager.PlaySFX(fireSound);
                    muzzleFlash?.Play();
                }

                Debug.Log("Firing weapon: " + weaponData.name + ". Ammo left: " + currentAmmo);

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

                GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, transform.rotation);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                // Direction of bullet is towards the target hit.
                Vector3 direction = (targetPoint - firePoint.position).normalized;
                // Fire the bullet with newly calculated direction.
                bulletRb.linearVelocity = direction * bulletSpeed;
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    protected IEnumerator Reload()
    {
        for (int i = currentAmmo; i < weaponData.clipCapacity; i++)
        {
            // Small delay to simulate reloading time.
            // TODO: Add reload sound effect, add reload animation.
            yield return new WaitForSeconds(1f * weaponData.reloadSpeed);
            Debug.Log("Reloading... " + (i + 1) + "/" + weaponData.clipCapacity);
            currentAmmo++;
        }
    }

    public void ShootRecoil()
    {
        Vector3 originalPos = transform.localPosition;
        Vector3 originalRot = transform.localEulerAngles;

        float recoilRot = -5f;
        float recoilBack = -0.3f;

        Sequence recoilSequence = DOTween.Sequence();

        // Rotate up & move back at the same time
        recoilSequence.Append(transform.DOLocalRotate(new Vector3(recoilRot, 0f, 0f), 0.05f));
        recoilSequence.Join(transform.DOLocalMoveZ(originalPos.z + recoilBack, 0.05f));

        // Return to original position & rotation
        recoilSequence.Append(transform.DOLocalRotate(originalRot, 0.1f));
        recoilSequence.Join(transform.DOLocalMoveZ(originalPos.z, 0.1f));
    }
}
