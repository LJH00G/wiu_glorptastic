
using UnityEngine;


public class AllAxisRotationGyroscope : MonoBehaviour
{

    [Header("Rotation Gyro")]
    [field: SerializeField]
    public Quaternion TargetRotation { get; set; }
    [field: SerializeField]
    public float RotateStrength { get; set; }
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
        Quaternion neededDelta = TargetRotation * Quaternion.Inverse(rb.rotation);
        neededDelta.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f)
            angle -= 360f;

        Vector3 torque = axis * angle * RotateStrength - rb.angularVelocity * Damping;
        
        rb.AddTorque(torque, ForceMode);

    }


}
