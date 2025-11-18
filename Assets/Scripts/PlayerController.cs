using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class SpaceshipController : MonoBehaviour
{
    [Header("Referências")]
    public Transform thisShip; // Modelo visual (filho)
    public Rigidbody r;

    [Header("Configurações de movimento")]
    public float turnSpeed = 60f;
    public float thrustForce = 40f;
    public float maxSpeed = 50f;
    public float damping = 0.98f; // reduz deslizamento lateral

    [Header("Movimento Automático")]
    public bool autoThrust = true; // Movimento constante para frente
    public float autoThrustSpeed = 20f; // Velocidade automática

    private void Start()
    {
        r.useGravity = false;
        r.isKinematic = false;
        r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        r.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        Turn();
        Thrust();
        LimitVelocity();
    }

    void Turn()
    {
        // Rotação suave igual ao script original
        float yaw = turnSpeed * Time.fixedDeltaTime * Input.GetAxis("Horizontal"); // A/D
        float pitch = turnSpeed * Time.fixedDeltaTime * Input.GetAxis("Vertical"); // W/S
        float roll = turnSpeed * Time.fixedDeltaTime * Input.GetAxis("Rotate");   // Q/E

        Quaternion deltaRotation = Quaternion.Euler(pitch, yaw, roll);
        r.MoveRotation(r.rotation * deltaRotation);
    }

    void Thrust()
    {
        float throttle = Input.GetAxis("Throttle");

        // Movimento automático para frente
        if (autoThrust)
        {
            Vector3 forwardForce = -transform.forward * autoThrustSpeed;
            r.AddForce(forwardForce, ForceMode.Acceleration);
        }

        // Input manual adicional (acelerar/desacelerar)
        if (!Mathf.Approximately(throttle, 0f))
        {
            Vector3 manualForce = -transform.forward * thrustForce * throttle;
            r.AddForce(manualForce, ForceMode.Acceleration);
        }
        else if (!autoThrust)
        {
            // Aplica leve amortecimento apenas se NÃO tiver movimento automático
            r.linearVelocity *= damping;
        }
    }

    void LimitVelocity()
    {
        // Mantém velocidade máxima e corrige movimento "torto"
        Vector3 localVel = transform.InverseTransformDirection(r.linearVelocity);

        // Z = frente/trás, X/Y = lateral/vertical
        localVel.x *= 0.9f; // reduz deriva lateral
        localVel.y *= 0.9f;

        // limita a velocidade máxima
        localVel.z = Mathf.Clamp(localVel.z, -maxSpeed, maxSpeed);

        // aplica de volta
        r.linearVelocity = transform.TransformDirection(localVel);
    }
}