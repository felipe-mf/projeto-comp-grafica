using UnityEngine;

public class DisableAsteroidGravity : MonoBehaviour
{
    [Header("Configuração")]
    public bool disableOnStart = true;
    public bool includeChildren = true;

    void Start()
    {
        if (disableOnStart)
        {
            DisableGravityOnAllAsteroids();
        }
    }

    public void DisableGravityOnAllAsteroids()
    {
        Rigidbody[] allRigidbodies = FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        int count = 0;
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (IsAsteroid(rb.gameObject))
            {
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                count++;
                Debug.Log($"Gravidade desativada em: {rb.gameObject.name}");
            }
        }

        Debug.Log($"<color=green>Total de {count} Rigidbodies de asteroides atualizados!</color>");
    }

    bool IsAsteroid(GameObject obj)
    {
        if (obj.CompareTag("Asteroid"))
            return true;

        string name = obj.name.ToLower();
        if (name.Contains("rock") ||
            name.Contains("asteroid") ||
            name.Contains("cell"))
        {
            return true;
        }

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