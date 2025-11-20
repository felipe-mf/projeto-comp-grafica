using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Script helper que fica nos filhos do asteroide e repassa dano para o pai
/// </summary>
public class AsteroidPart : MonoBehaviour
{
    [HideInInspector]
    public Asteroid parentAsteroid;

    void OnTriggerEnter(Collider other)
    {
        // Se for atingido por um projétil
        if (other.CompareTag("Projectile"))
        {
            Debug.Log($"AsteroidPart {gameObject.name} foi atingido por projétil!");

            // Pega o dano do projétil
            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null && parentAsteroid != null)
            {
                parentAsteroid.TakeDamage(projectile.damage);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se colidir com o player
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"AsteroidPart {gameObject.name} colidiu com Player!");

            if (parentAsteroid != null)
            {
                parentAsteroid.TakeDamage(parentAsteroid.health); // Mata o asteroide
            }
        }
    }
}