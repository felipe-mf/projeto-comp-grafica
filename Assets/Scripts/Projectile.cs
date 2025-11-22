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

        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);

        Debug.Log("Projectile criado! Velocidade: " + rb.linearVelocity + " | Posição: " + transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projétil colidiu com: " + other.gameObject.name + " (Layer: " + LayerMask.LayerToName(other.gameObject.layer) + ")");

        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null)
        {
            asteroid.TakeDamage(damage);
            Debug.Log("Acertou asteroide! Dano: " + damage);
        }

        EnemyShip enemy = other.GetComponent<EnemyShip>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Acertou nave inimiga! Dano: " + damage);
        }

        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        Destroy(gameObject);
    }
}