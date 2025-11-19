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
    public Transform[] waypoints; // Pontos de patrulha (opcional)
    private int currentWaypointIndex = 0;

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
    public GameObject damageEffect; // Efeito quando leva dano

    private Rigidbody rb;

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

        // Se não houver waypoints, cria movimento aleatório
        if (waypoints == null || waypoints.Length == 0)
        {
            canMove = false; // Fica parado se não tiver waypoints
        }
    }

    void Update()
    {
        if (canMove && waypoints != null && waypoints.Length > 0)
        {
            MoveToWaypoint();
        }

        if (canShoot && player != null)
        {
            TryShootAtPlayer();
        }
    }

    void MoveToWaypoint()
    {
        if (currentWaypointIndex >= waypoints.Length) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;

        // Move em direção ao waypoint
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        // OPCIONAL: Rotaciona suavemente para a direção do movimento
        // Descomente as linhas abaixo se quiser que a nave vire na direção que está indo
        /*
        if (direction != Vector3.zero)
        {
            // Apenas rotação no eixo Y (horizontal)
            Vector3 flatDirection = new Vector3(direction.x, 0, direction.z);
            if (flatDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        */

        // Verifica se chegou no waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 2f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Loop nos waypoints
            }
        }
    }

    void TryShootAtPlayer()
    {
        if (Time.time < nextShootTime) return;
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Só atira se o player estiver no alcance
        if (distanceToPlayer <= shootRange)
        {
            ShootAtPlayer();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void ShootAtPlayer()
    {
        if (enemyProjectilePrefab == null || firePoint == null) return;

        // Calcula direção para o player
        Vector3 directionToPlayer = (player.position - firePoint.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

        // Cria o projétil
        GameObject projectile = Instantiate(enemyProjectilePrefab, firePoint.position, lookRotation);

        // Se o projétil tiver Rigidbody, aplica velocidade
        Rigidbody projRb = projectile.GetComponent<Rigidbody>();
        if (projRb != null)
        {
            projRb.linearVelocity = directionToPlayer * 50f; // Velocidade do tiro inimigo
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        Debug.Log(gameObject.name + " levou " + damage + " de dano. Vida restante: " + health);

        // Efeito visual de dano
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
        //GameObject gmObj = GameObject.Find("GameManager");
        //if (gmObj != null)
        //{
        //    GameManager gameManager = gmObj.GetComponent<GameManager>();
        //    if (gameManager != null)
        //    {
        //        gameManager.AddScore(scoreValue);
        //    }
        //}
        else
        {
            // Se não houver GameManager, só mostra no console
            Debug.Log("Pontos ganhos: " + scoreValue);
        }

        // Destroi a nave inimiga
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Se colidir com o player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Causa dano ao player (você pode implementar depois)
            // PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            // if (playerHealth != null) playerHealth.TakeDamage(50f);

            // A nave inimiga também é destruída
            Die();
        }
    }

    // Visualização dos waypoints no Editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawWireSphere(waypoints[i].position, 1f);

                // Desenha linha para o próximo waypoint
                if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        // Visualiza o alcance de tiro
        if (canShoot)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }
    }
}