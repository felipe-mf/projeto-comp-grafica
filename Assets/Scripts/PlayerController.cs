using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class SpaceshipController : MonoBehaviour
{
    [Header("Referências")]
    public Transform thisShip;
    public Rigidbody r;

    [Header("Configurações de movimento")]
    public float turnSpeed = 60f;
    public float thrustForce = 40f;
    public float maxSpeed = 50f;
    public float damping = 0.98f;

    [Header("Movimento Automático")]
    public bool autoThrust = true;
    public float autoThrustSpeed = 20f;

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
            // Aplica leve amortecimento apenas se nao tiver movimento automático
            r.linearVelocity *= damping;
        }
    }

    void LimitVelocity()
    {
        Vector3 localVel = transform.InverseTransformDirection(r.linearVelocity);

        localVel.x *= 0.9f;
        localVel.y *= 0.9f;

        localVel.z = Mathf.Clamp(localVel.z, -maxSpeed, maxSpeed);

        r.linearVelocity = transform.TransformDirection(localVel);
    }
}