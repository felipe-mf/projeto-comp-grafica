using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Configurações do Projétil")]
    public float speed = 100f;
    public float lifetime = 5f;
    public float damage = 25f;

    [Header("Efeitos Visuais")]
    public GameObject impactEffect;
    public TrailRenderer trail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Projectile precisa de um Rigidbody!");
            return;
        }

        // Ignora colisão com o player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            Collider projectileCollider = GetComponent<Collider>();
            if (playerCollider != null && projectileCollider != null)
            {
                Physics.IgnoreCollision(projectileCollider, playerCollider);
            }
        }

        // Aplica velocidade inicial
        rb.linearVelocity = transform.forward * speed;

        // Destroi após o tempo de vida
        Destroy(gameObject, lifetime);

        Debug.Log("Projectile criado! Velocidade: " + rb.linearVelocity);
    }

    void OnTriggerEnter(Collider other)
    {
        // Ignora colisão com outros projéteis
        if (other.CompareTag("Projectile")) return;

        // Verifica se acertou um asteroide
        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
        }

        // Cria efeito de impacto
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Destroi o projétil
        Destroy(gameObject);
    }
}