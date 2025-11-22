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
    private bool hasHitPlayer = false; // Previne múltiplas colisões

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Rotação constante
        randomRotation = new Vector3(
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed),
            Random.Range(-rotationSpeed, rotationSpeed)
        );

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        // Adiciona referência deste script a todos os filhos
        RegisterChildrenColliders();
    }

    void RegisterChildrenColliders()
    {
        Collider[] childColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in childColliders)
        {
            AsteroidPart part = col.gameObject.GetComponent<AsteroidPart>();
            if (part == null)
            {
                part = col.gameObject.AddComponent<AsteroidPart>();
            }
            part.parentAsteroid = this;
        }

        Debug.Log($"Asteroid {gameObject.name}: Registrou {childColliders.Length} colliders");
    }

    void FixedUpdate()
    {
        // rotação constante
        if (rb != null)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(randomRotation * Time.fixedDeltaTime));
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"Asteroid {gameObject.name} recebeu {damage} de dano. HP restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"Asteroid {gameObject.name} foi destruído!");

        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        BreakAsteroid();

        if (canSplit && smallerAsteroidPrefab != null)
        {
            for (int i = 0; i < splitCount; i++)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                GameObject fragment = Instantiate(smallerAsteroidPrefab, transform.position, Random.rotation);

                Rigidbody fragRb = fragment.GetComponent<Rigidbody>();
                if (fragRb != null && rb != null)
                {
                    fragRb.linearVelocity = rb.linearVelocity + randomDirection * splitForce;
                }
            }
        }

        Destroy(gameObject, 5f);
    }

    void BreakAsteroid()
    {
        Joint[] joints = GetComponentsInChildren<Joint>();
        foreach (Joint joint in joints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        Debug.Log($"Quebrou {joints.Length} joints");

        Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody childRb in childRigidbodies)
        {
            if (childRb != null && childRb != rb)
            {
                childRb.isKinematic = false;
                childRb.useGravity = false;

                Vector3 explosionDirection = (childRb.transform.position - transform.position).normalized;
                childRb.AddForce(explosionDirection * 5f, ForceMode.Impulse);

                childRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }

        if (rb != null)
        {
            rb.isKinematic = true;
        }

        this.enabled = false;

        foreach (Rigidbody childRb in childRigidbodies)
        {
            if (childRb != null && childRb != rb)
            {
                Destroy(childRb.gameObject, Random.Range(3f, 6f));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasHitPlayer) return;
            hasHitPlayer = true;

            Debug.Log("Asteroid colidiu com Player!");

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            Die();
        }
    }
}