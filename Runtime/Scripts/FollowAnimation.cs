using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FollowAnimation : MonoBehaviour
{
    public Transform ghostTransform; // Assign your Ghost object's Transform in the Inspector
    private Rigidbody _rb;
    public float positionLerpSpeed = 3.0f; // Speed of movement
    public float rotationLerpSpeed = 15.0f; // Speed of rotation

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetPosition = ghostTransform.position;
        Quaternion targetRotation = ghostTransform.rotation;

        // Interpolate position and rotation between current and target values
        Vector3 newPosition = Vector3.Lerp(_rb.position, targetPosition, Time.deltaTime * positionLerpSpeed);
        Quaternion newRotation = Quaternion.Lerp(_rb.rotation, targetRotation, Time.deltaTime * rotationLerpSpeed);

        _rb.MovePosition(newPosition);
        _rb.MoveRotation(newRotation);
    }
}