using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Configurações")]
    public float health = 100f;
    public float rotationSpeed = 20f;
    public int scoreValue = 10;

    [Header("Efeitos de Destruição")]
    public GameObject explosionEffect;
    public AudioClip explosionSound;

    [Header("Fragmentação (opcional)")]
    public bool canSplit = false;
    public GameObject smallerAsteroidPrefab;
    public int splitCount = 3;
    public float splitForce = 5f;

    private Rigidbody rb;
    private Vector3 randomRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rotação aleatória constante
        randomRotation = new Vector3(
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed)
        );

        // Se não tiver Rigidbody, adiciona
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }

    void FixedUpdate()
    {
        // Aplica rotação constante
        rb.MoveRotation(rb.rotation * Quaternion.Euler(randomRotation * Time.fixedDeltaTime));
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Efeito de explosão
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        // Som de explosão
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        // Adiciona pontuação (se você tiver um sistema de score)
        //GameManager gameManager = FindFirstObjectByType<GameManager>();
        //if (gameManager != null)
        //{
        //    gameManager.AddScore(scoreValue);
        //}

        // Fragmenta em asteroides menores
        if (canSplit && smallerAsteroidPrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                GameObject fragment = Instantiate(smallerAsteroidPrefab, transform.position, Random.rotation);

                Rigidbody fragRb = fragment.GetComponent<Rigidbody>();
                if (fragRb != null)
                {
                    fragRb.linearVelocity = rb.linearVelocity + randomDirection * splitForce;
                }
            }
        }

        // Destroi o asteroide
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se colidir com o player, causa dano (você pode implementar isso depois)
        if (collision.gameObject.CompareTag("Player"))
        {
            // PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(25f);
            // }
            Die();
        }
    }
}