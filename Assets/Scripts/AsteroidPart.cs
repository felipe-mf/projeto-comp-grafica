using UnityEngine;
using UnityEngine.UIElements;

public class AsteroidPart : MonoBehaviour
{
    [HideInInspector]
    public Asteroid parentAsteroid;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            Debug.Log($"AsteroidPart {gameObject.name} foi atingido por projétil!");

            Projectile projectile = other.GetComponent<Projectile>();
            if (projectile != null && parentAsteroid != null)
            {
                parentAsteroid.TakeDamage(projectile.damage);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"AsteroidPart {gameObject.name} colidiu com Player!");

            if (parentAsteroid != null)
            {
                parentAsteroid.TakeDamage(parentAsteroid.health);
            }
        }
    }
}