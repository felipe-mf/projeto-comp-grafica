using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    public Transform thisShip;
    public Rigidbody r;


    //speed
    public float turnSpeed = 60f;
    public float boostSpeed = 45f;

    // future variables

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        r = GetComponent<Rigidbody>();
        r.useGravity = false;
    }

    private void FixedUpdate()
    {
        Turn();
        Thrust();
    }

    void Turn()
    {
        float yaw = turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        float pitch = turnSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        float roll = turnSpeed * Time.deltaTime * Input.GetAxis("Rotate");
        thisShip.Rotate(pitch, yaw, roll);
    }

    void Thrust()
    {
        thisShip.position -= thisShip.forward * boostSpeed * Time.deltaTime * Input.GetAxis("Throttle");
    }
}