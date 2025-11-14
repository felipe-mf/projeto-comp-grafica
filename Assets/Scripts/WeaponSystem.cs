using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Configurações de Tiro")]
    public GameObject projectilePrefab;
    public Transform[] firePoints; // Pontos de spawn dos tiros
    public float fireRate = 0.2f; // Tempo entre tiros (segundos)
    public KeyCode fireKey = KeyCode.Space;
    public bool automaticFire = false; // Segura para atirar continuamente

    [Header("Efeitos")]
    public AudioClip fireSound;
    public GameObject muzzleFlash;

    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        // Configura AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // Som 3D

        // Se não houver firePoints definidos, usa a posição da nave
        if (firePoints == null || firePoints.Length == 0)
        {
            firePoints = new Transform[] { transform };
        }
    }

    void Update()
    {
        // Verifica input de tiro
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

        // Atira de cada ponto de disparo
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null)
            {
                Debug.LogWarning("Um dos Fire Points está null!");
                continue;
            }

            // Cria o projétil
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Debug.Log("Projétil criado na posição: " + firePoint.position);

            // Se a nave estiver se movendo, adiciona a velocidade dela ao projétil
            Rigidbody shipRb = GetComponent<Rigidbody>();
            if (shipRb != null)
            {
                Rigidbody projRb = projectile.GetComponent<Rigidbody>();
                if (projRb != null)
                {
                    projRb.linearVelocity += shipRb.linearVelocity;
                }
            }

            // Efeito de flash na boca da arma
            if (muzzleFlash != null)
            {
                GameObject flash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
                flash.transform.SetParent(firePoint);
                Destroy(flash, 0.1f);
            }
        }

        // Toca som do tiro
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
}