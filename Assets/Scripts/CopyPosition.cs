using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    Transform transTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Copia as coordenadas de translação apenas
        transform.position = transTarget.position;
    }
}
