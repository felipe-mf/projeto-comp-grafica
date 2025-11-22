using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Singleton - só existe uma instância em todo o jogo
    public static GameSettings Instance { get; private set; }

    public enum Difficulty
    {
        Easy,
        Hard
    }

    public Difficulty selectedDifficulty = Difficulty.Easy;

    [System.Serializable]
    public class DifficultySettings
    {
        public int numberOfEnemies = 5;
        public float enemyHealth = 100f;
        public float enemySpeed = 10f;
        public bool enemiesCanShoot = false;
    }

    [Header("Dificuldade Fácil")]
    public DifficultySettings easySettings = new DifficultySettings
    {
        numberOfEnemies = 5,
        enemyHealth = 100f,
        enemySpeed = 8f,
        enemiesCanShoot = false
    };

    [Header("Dificuldade Difícil")]
    public DifficultySettings hardSettings = new DifficultySettings
    {
        numberOfEnemies = 8,
        enemyHealth = 150f,
        enemySpeed = 12f,
        enemiesCanShoot = true
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Não destrói ao trocar de cena
            Debug.Log("GameSettings criado e persistente");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        selectedDifficulty = difficulty;
        Debug.Log("Dificuldade definida: " + difficulty);
    }

    public DifficultySettings GetCurrentSettings()
    {
        if (selectedDifficulty == Difficulty.Easy)
        {
            return easySettings;
        }
        else
        {
            return hardSettings;
        }
    }

    public int GetEnemyCount()
    {
        return GetCurrentSettings().numberOfEnemies;
    }

    public float GetEnemyHealth()
    {
        return GetCurrentSettings().enemyHealth;
    }

    public float GetEnemySpeed()
    {
        return GetCurrentSettings().enemySpeed;
    }

    public bool CanEnemiesShoot()
    {
        return GetCurrentSettings().enemiesCanShoot;
    }
}