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

        // Adiciona referência deste script a todos os filhos
        RegisterChildrenColliders();
    }

    void RegisterChildrenColliders()
    {
        // Pega todos os colliders nos filhos
        Collider[] childColliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in childColliders)
        {
            // Adiciona um componente helper que referencia este asteroide
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
        // Aplica rotação constante
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

        // Adiciona pontuação (se o GameManager existir)
        // GameObject gmObj = GameObject.Find("GameManager");
        // if (gmObj != null)
        // {
        //     GameManager gameManager = gmObj.GetComponent<GameManager>();
        //     if (gameManager != null)
        //     {
        //         gameManager.AddScore(scoreValue);
        //     }
        // }

        // QUEBRA O ASTEROIDE EM PEDAÇOS (sua implementação)
        BreakAsteroid();

        // Fragmenta em asteroides menores (opcional)
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

        // Agenda destruição após as peças se espalharem
        Destroy(gameObject, 5f);
    }

    void BreakAsteroid()
    {
        // 1. Quebra todos os Joints (faz as peças se separarem)
        Joint[] joints = GetComponentsInChildren<Joint>();
        foreach (Joint joint in joints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        Debug.Log($"Quebrou {joints.Length} joints");

        // 2. Ativa física em todos os pedaços
        Rigidbody[] childRigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody childRb in childRigidbodies)
        {
            if (childRb != null && childRb != rb) // Não mexe no Rigidbody principal
            {
                childRb.isKinematic = false;
                childRb.useGravity = false; // Mantém sem gravidade (estamos no espaço!)

                // Adiciona força explosiva para espalhar os pedaços
                Vector3 explosionDirection = (childRb.transform.position - transform.position).normalized;
                childRb.AddForce(explosionDirection * 5f, ForceMode.Impulse);

                // Adiciona rotação aleatória
                childRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            }
        }

        // 3. Desativa o Rigidbody principal (para não interferir)
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // 4. Desativa o script Asteroid (não precisamos mais dele)
        this.enabled = false;

        // 5. Agenda destruição dos pedaços individualmente
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
        // Se colidir com o player, causa dano E destrói o asteroide
        if (collision.gameObject.CompareTag("Player"))
        {
            // Previne dupla colisão
            if (hasHitPlayer) return;
            hasHitPlayer = true;

            Debug.Log("Asteroid colidiu com Player!");

            // Causa dano no player (implementação do seu amigo)
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            // Asteroide é destruído na colisão
            Die();
        }
    }
}