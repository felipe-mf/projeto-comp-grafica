using UnityEngine;

public class DisableAsteroidGravity : MonoBehaviour
{
    [Header("Configuração")]
    public bool disableOnStart = true; // Desativa automaticamente ao iniciar
    public bool includeChildren = true; // Busca nos filhos também

    void Start()
    {
        if (disableOnStart)
        {
            DisableGravityOnAllAsteroids();
        }
    }

    // Método público que você pode chamar manualmente
    public void DisableGravityOnAllAsteroids()
    {
        // Busca TODOS os Rigidbodies na cena (incluindo filhos)
        Rigidbody[] allRigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        int count = 0;
        foreach (Rigidbody rb in allRigidbodies)
        {
            // Verifica se é um asteroide (por nome ou tag)
            if (IsAsteroid(rb.gameObject))
            {
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Melhora colisões
                count++;
                Debug.Log($"Gravidade desativada em: {rb.gameObject.name}");
            }
        }

        Debug.Log($"<color=green>Total de {count} Rigidbodies de asteroides atualizados!</color>");
    }

    bool IsAsteroid(GameObject obj)
    {
        // Verifica por tag
        if (obj.CompareTag("Asteroid"))
            return true;

        // Verifica por nome
        string name = obj.name.ToLower();
        if (name.Contains("rock") ||
            name.Contains("asteroid") ||
            name.Contains("cell"))
        {
            return true;
        }

        // Verifica se o pai tem "rock" ou "asteroid" no nome
        Transform parent = obj.transform.parent;
        while (parent != null)
        {
            string parentName = parent.name.ToLower();
            if (parentName.Contains("rock") ||
                parentName.Contains("asteroid") ||
                parentName.Contains("fracture"))
            {
                return true;
            }
            parent = parent.parent;
        }

        return false;
    }
}