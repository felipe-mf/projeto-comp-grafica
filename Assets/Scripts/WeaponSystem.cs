using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform[] firePoints;
    public float fireRate = 0.2f;
    public KeyCode fireKey = KeyCode.Space;
    public bool automaticFire = false;

    [Header("Efeitos")]
    public AudioClip fireSound;
    public GameObject muzzleFlash;

    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        if (firePoints == null || firePoints.Length == 0)
        {
            firePoints = new Transform[] { transform };
        }
    }

    void Update()
    {
        bool shouldFire = automaticFire ? Input.GetKey(fireKey) : Input.GetKeyDown(fireKey);

        if (shouldFire && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        Debug.Log("Fire() chamado!");

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab não está atribuído no WeaponSystem!");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogError("Fire Points não estão configurados!");
            return;
        }

        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null)
            {
                Debug.LogWarning("Um dos Fire Points está null!");
                continue;
            }

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log("Projétil criado na posição: " + firePoint.position);

            Rigidbody shipRb = GetComponent<Rigidbody>();
            if (shipRb != null)
            {
                Rigidbody projRb = projectile.GetComponent<Rigidbody>();
                if (projRb != null)
                {
                    projRb.linearVelocity += shipRb.linearVelocity;
                }
            }

            if (muzzleFlash != null)
            {
                GameObject flash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
                flash.transform.SetParent(firePoint);
                Destroy(flash, 0.1f);
            }
        }

        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
}