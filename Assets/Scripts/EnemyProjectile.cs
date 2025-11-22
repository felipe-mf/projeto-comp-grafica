using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Configurações")]
    public float speed = 50f;
    public float lifetime = 5f;
    public float damage = 1f;

    [Header("Efeitos")]
    public GameObject impactEffect;
    public TrailRenderer trail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("EnemyProjectile precisa de um Rigidbody!");
            return;
        }

        if (rb.linearVelocity.magnitude < 1f)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        Destroy(gameObject, lifetime);

        Debug.Log("Projétil inimigo criado! Velocidade: " + rb.linearVelocity);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tiro inimigo colidiu com: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
                Debug.Log("Tiro inimigo acertou o player!");
            }
        }

        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        Destroy(gameObject);
    }
}