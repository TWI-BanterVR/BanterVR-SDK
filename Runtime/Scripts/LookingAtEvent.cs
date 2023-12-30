using UnityEngine;
using UnityEngine.Events;

public class LookingAtEvent : MonoBehaviour
{
    public Transform target; // Target object to look at
    public float maxDistance = 10.0f; // Maximum distance to check
    public float minDistance = 1.0f; // Minimum distance to check
    public float lookAngle = 5.0f; // Angle within which we'll consider the player looking at the object

    [Tooltip("Event invoked when the object becomes visible")]
    public UnityEvent lookingAtEvent;

    [Tooltip("Event invoked when the object becomes invisible")]
    public UnityEvent stoppedLookingAtEvent;

    private bool isSeeing = false;
    void Update()
    {
        if (target == null)
        {
            target = Camera.main.transform;
            return;
        }
        // Compute the direction from the player to the target
        Vector3 targetDir = transform.position - target.position;

        // Compute the distance from the player to the target
        float distance = targetDir.magnitude;

        // Compute the angle between the player's forward vector and the target direction
        float angle = Vector3.Angle(targetDir, target.forward);

        // Check if the angle is within the specified threshold and the distance is within the specified range
        if (angle < lookAngle && distance >= minDistance && distance <= maxDistance)
        {
            if (!isSeeing)
            {
                Debug.Log("Player is looking at the target!");
                lookingAtEvent?.Invoke();
                isSeeing = true;
            }
        }
        else if (isSeeing)
        {
            Debug.Log("Player has stopped looking at the target!");
            stoppedLookingAtEvent?.Invoke();
            isSeeing = false;
        }
    }
}
