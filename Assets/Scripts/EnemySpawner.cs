//using UnityEngine;
//using System.Collections.Generic;

//public class EnemySpawner : MonoBehaviour
//{
//    [Header("Prefab do Inimigo")]
//    public GameObject enemyPrefab;

//    [Header("Área de Spawn")]
//    public Vector3 spawnAreaCenter = Vector3.zero;
//    public Vector3 spawnAreaSize = new Vector3(50, 10, 50);

//    [Header("Waypoints (Opcional)")]
//    public Transform[] sharedWaypoints; // Waypoints que todos os inimigos vão usar

//    [Header("Configurações")]
//    public bool spawnOnStart = true;
//    public float spawnDelay = 0.5f; // Delay entre cada spawn

//    private List<GameObject> spawnedEnemies = new List<GameObject>();

//    void Start()
//    {
//        if (spawnOnStart)
//        {
//            SpawnEnemies();
//        }
//    }

//    public void SpawnEnemies()
//    {
//        if (enemyPrefab == null)
//        {
//            Debug.LogError("Enemy Prefab não está atribuído!");
//            return;
//        }

//        // Limpa lista de inimigos anteriores
//        spawnedEnemies.Clear();

//        // Pega configurações de dificuldade
//        int enemyCount = 5; // Padrão
//        float enemyHealth = 100f;
//        float enemySpeed = 10f;
//        bool canShoot = false;

//        if (GameSettings.Instance != null)
//        {
//            enemyCount = GameSettings.Instance.GetEnemyCount();
//            enemyHealth = GameSettings.Instance.GetEnemyHealth();
//            enemySpeed = GameSettings.Instance.GetEnemySpeed();
//            canShoot = GameSettings.Instance.CanEnemiesShoot();

//            Debug.Log($"Spawnando {enemyCount} inimigos - Dificuldade: {GameSettings.Instance.selectedDifficulty}");
//        }
//        else
//        {
//            Debug.LogWarning("GameSettings não encontrado! Usando valores padrão.");
//        }

//        // Spawna os inimigos
//        for (int i = 0; i < enemyCount; i++)
//        {
//            SpawnSingleEnemy(i, enemyHealth, enemySpeed, canShoot);
//        }
//    }

//    void SpawnSingleEnemy(int index, float health, float speed, bool canShoot)
//    {
//        // Posição aleatória na área de spawn
//        Vector3 randomPosition = spawnAreaCenter + new Vector3(
//            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
//            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
//            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
//        );

//        // Rotação aleatória
//        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

//        // Cria o inimigo
//        GameObject enemy = Instantiate(enemyPrefab, randomPosition, randomRotation);
//        enemy.name = "Enemy_" + (index + 1);

//        // Configura propriedades do inimigo
//        EnemyShip enemyScript = enemy.GetComponent<EnemyShip>();
//        if (enemyScript != null)
//        {
//            enemyScript.health = health;
//            enemyScript.moveSpeed = speed;
//            enemyScript.canShoot = canShoot;

//            // Se tiver waypoints compartilhados, atribui
//            if (sharedWaypoints != null && sharedWaypoints.Length > 0)
//            {
//                enemyScript.waypoints = sharedWaypoints;
//            }
//        }

//        // Adiciona à lista
//        spawnedEnemies.Add(enemy);

//        Debug.Log($"Inimigo {index + 1} criado em {randomPosition}");
//    }

//    // Retorna quantos inimigos ainda estão vivos
//    public int GetAliveEnemyCount()
//    {
//        // Remove nulos (inimigos destruídos)
//        spawnedEnemies.RemoveAll(enemy => enemy == null);
//        return spawnedEnemies.Count;
//    }

//    // Visualização da área de spawn no Editor
//    void OnDrawGizmos()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
//    }
//}

using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefab do Inimigo")]
    public GameObject enemyPrefab;

    [Header("Área de Spawn")]
    public Vector3 spawnAreaCenter = Vector3.zero;
    public Vector3 spawnAreaSize = new Vector3(50, 10, 50);

    [Header("Waypoints (Opcional)")]
    public Transform[] sharedWaypoints; // Waypoints que todos os inimigos vão usar

    [Header("Configurações")]
    public bool spawnOnStart = true;
    public float spawnDelay = 0.5f; // Delay entre cada spawn

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (spawnOnStart)
        {
            SpawnEnemies();
        }
    }

    public void SpawnEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab não está atribuído!");
            return;
        }

        // Limpa lista de inimigos anteriores
        spawnedEnemies.Clear();

        // Pega configurações de dificuldade
        int enemyCount = 5; // Padrão
        float enemyHealth = 100f;
        float enemySpeed = 10f;
        bool canShoot = false;

        if (GameSettings.Instance != null)
        {
            enemyCount = GameSettings.Instance.GetEnemyCount();
            enemyHealth = GameSettings.Instance.GetEnemyHealth();
            enemySpeed = GameSettings.Instance.GetEnemySpeed();
            canShoot = GameSettings.Instance.CanEnemiesShoot();

            Debug.Log($"Spawnando {enemyCount} inimigos - Dificuldade: {GameSettings.Instance.selectedDifficulty}");
        }
        else
        {
            Debug.LogWarning("GameSettings não encontrado! Usando valores padrão.");
        }

        // Spawna os inimigos
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnSingleEnemy(i, enemyHealth, enemySpeed, canShoot);
        }
    }

    void SpawnSingleEnemy(int index, float health, float speed, bool canShoot)
    {
        // Posição aleatória na área de spawn
        Vector3 randomPosition = spawnAreaCenter + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        // Rotação aleatória
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        // Cria o inimigo
        GameObject enemy = Instantiate(enemyPrefab, randomPosition, randomRotation);
        enemy.name = "Enemy_" + (index + 1);

        // Configura propriedades do inimigo
        EnemyShip enemyScript = enemy.GetComponent<EnemyShip>();
        if (enemyScript != null)
        {
            enemyScript.health = health;
            enemyScript.moveSpeed = speed;
            enemyScript.canShoot = canShoot;

            // Configura movimento aleatório
            enemyScript.useRandomMovement = true; // Ativa movimento aleatório
            enemyScript.changeDirectionInterval = Random.Range(2f, 5f); // Varia o tempo
            enemyScript.movementRadius = 40f; // Área de movimento

            // Se tiver waypoints compartilhados, usa waypoints ao invés de aleatório
            if (sharedWaypoints != null && sharedWaypoints.Length > 0)
            {
                enemyScript.useRandomMovement = false; // Desativa aleatório
                enemyScript.waypoints = sharedWaypoints; // Usa waypoints
            }
        }

        // Adiciona à lista
        spawnedEnemies.Add(enemy);

        Debug.Log($"Inimigo {index + 1} criado em {randomPosition}");
    }

    // Retorna quantos inimigos ainda estão vivos
    public int GetAliveEnemyCount()
    {
        // Remove nulos (inimigos destruídos)
        spawnedEnemies.RemoveAll(enemy => enemy == null);
        return spawnedEnemies.Count;
    }

    // Visualização da área de spawn no Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(spawnAreaCenter, spawnAreaSize);
    }
}