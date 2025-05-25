using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Smoothing")]
    public float moveSmoothness;
    public float rotSmoothness;

    [Header("Offsets")]
    public Vector3 moveOffset;
    public Vector3 rotOffset;

    [Header("Target")]
    public Transform carTarget;

    [Header("Wall Avoidance")]
    public float cameraRadius = 0.3f;
    public LayerMask obstacleLayers;

    void FixedUpdate()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 desiredPos = carTarget.TransformPoint(moveOffset);

        Vector3 direction = desiredPos - carTarget.position;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.SphereCast(carTarget.position, cameraRadius, direction, out hit, distance, obstacleLayers))
        {
            desiredPos = hit.point - direction * cameraRadius;
        }


        transform.position = Vector3.Lerp(transform.position, desiredPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        Vector3 direction = carTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotSmoothness * Time.deltaTime);
    }
}
