using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [Header("Efeitos")]
    public float rotationSpeed = 50f;
    public AudioClip portalSound;

    [Header("Partículas (Opcional)")]
    public ParticleSystem portalParticles;

    private VictoryManager victoryManager;

    void Start()
    {
        victoryManager = FindFirstObjectByType<VictoryManager>();

        if (portalParticles != null)
        {
            portalParticles.Play();
        }

        if (portalSound != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = portalSound;
            audioSource.loop = true;
            audioSource.spatialBlend = 1f;
            audioSource.Play();
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entrou no portal!");

            if (victoryManager != null)
            {
                victoryManager.TriggerVictory();
            }

            StartCoroutine(SuckPlayerIntoPortal(other.gameObject));
        }
    }

    System.Collections.IEnumerator SuckPlayerIntoPortal(GameObject player)
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            player.transform.position = Vector3.Lerp(startPos, targetPos, t);
            player.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t);

            yield return null;
        }

        player.SetActive(false);
    }
}