using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Configurações")]
    public float health = 100f;
    public int scoreValue = 50;
    public float rotationSpeed = 30f;

    [Header("Movimento")]
    public bool canMove = true;
    public float moveSpeed = 10f;
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    [Header("Movimento Aleatório")]
    public bool useRandomMovement = true;
    public float changeDirectionInterval = 3f;
    public float movementRadius = 300f;
    private Vector3 randomTargetPosition;
    private float nextDirectionChangeTime = 0f;
    private Vector3 spawnPosition;

    [Header("Combate")]
    public bool canShoot = false;
    public GameObject enemyProjectilePrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float shootRange = 50f;
    private float nextShootTime = 0f;
    private Transform player;

    [Header("Efeitos")]
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    public GameObject damageEffect;

    private Rigidbody rb;
    private bool hasHitPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
        }

        // Encontra o player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Salva posição inicial para movimento aleatório
        spawnPosition = transform.position;
        Debug.Log(gameObject.name + " spawn position: " + spawnPosition);

        // Gera primeira posição aleatória
        if (useRandomMovement)
        {
            Debug.Log(gameObject.name + " usando movimento ALEATÓRIO");
            GenerateRandomTarget();
        }
        else
        {
            Debug.Log(gameObject.name + " usando WAYPOINTS");
            if (waypoints == null || waypoints.Length == 0)
            {
                canMove = false;
                Debug.LogWarning(gameObject.name + " sem waypoints! Vai ficar parado.");
            }
        }
    }

    void Update()
    {
        if (canMove)
        {
            if (useRandomMovement)
            {
                MoveRandomly();
            }
            else if (waypoints != null && waypoints.Length > 0)
            {
                MoveToWaypoint();
            }
        }

        if (canShoot && player != null)
        {
            TryShootAtPlayer();
        }
    }

    void MoveRandomly()
    {
        // Muda de direção periodicamente
        if (Time.time >= nextDirectionChangeTime)
        {
            GenerateRandomTarget();
            nextDirectionChangeTime = Time.time + changeDirectionInterval;
        }

        // Move em direção ao alvo aleatório
        Vector3 direction = (randomTargetPosition - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // Se chegou perto do alvo, gera novo
        float distanceToTarget = Vector3.Distance(transform.position, randomTargetPosition);
        if (distanceToTarget < 5f)
        {
            GenerateRandomTarget();
        }
    }

    void GenerateRandomTarget()
    {
        // Gera posição aleatória dentro de um raio da posição de spawn
        Vector2 randomCircle = Random.insideUnitCircle * movementRadius;

        randomTargetPosition = spawnPosition + new Vector3(
            randomCircle.x,
            Random.Range(-10f, 10f),
            randomCircle.y
        );

        Debug.Log(gameObject.name + " nova direção: " + randomTargetPosition);
    }

    void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 2f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
    }

    void TryShootAtPlayer()
    {
        if (Time.time < nextShootTime) return;
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootRange)
        {
            ShootAtPlayer();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        if (enemyProjectilePrefab == null || firePoint == null) return;

        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        GameObject projectile = Instantiate(enemyProjectilePrefab, firePoint.position, lookRotation);

        Rigidbody projRb = projectile.GetComponent<Rigidbody>();
        if (projRb != null)
        {
            projRb.linearVelocity = directionToPlayer * 50f;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log(gameObject.name + " levou " + damage + " de dano. Vida restante: " + health);

        if (damageEffect != null)
        {
            GameObject effect = Instantiate(damageEffect, transform.position, Quaternion.identity);
            Destroy(effect, 1f);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " foi destruído!");

        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Debug.Log("Pontos ganhos: " + scoreValue);

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasHitPlayer) return;
            hasHitPlayer = true;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage();
            }

            Die();
        }
    }

    void OnDrawGizmos()
    {
        if (useRandomMovement && Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(spawnPosition, movementRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(randomTargetPosition, 2f);
            Gizmos.DrawLine(transform.position, randomTargetPosition);
        }

        if (!useRandomMovement && waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawWireSphere(waypoints[i].position, 1f);

                    if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    }
                }
            }
        }

        if (canShoot)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }
    }
}