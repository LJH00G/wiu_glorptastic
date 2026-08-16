using UnityEngine;

public class AllAxisPositionGyroscope : MonoBehaviour
{
    [Header("Position Gyro")]
    [field: SerializeField]
    public Vector3 AxisMultiplier { get; set; } = Vector3.one;
    [field: SerializeField]
    public Vector3 TargetPosition { get; set; }
    [field: SerializeField]
    public float MoveStrength { get; set; }
    [field: SerializeField]
    public float Damping { get; set; }
    [field: SerializeField]
    public ForceMode ForceMode { get; set; } = ForceMode.Acceleration;


    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        Vector3 neededDelta = TargetPosition - transform.position;

        Vector3 acceleration = neededDelta * MoveStrength - rb.linearVelocity * Damping;

        acceleration.x *= AxisMultiplier.x;
        acceleration.y *= AxisMultiplier.y;
        acceleration.z *= AxisMultiplier.z;

        rb.AddForce(acceleration, ForceMode);
    }


}
