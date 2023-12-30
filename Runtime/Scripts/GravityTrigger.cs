using UnityEngine;

public class GravityTrigger : MonoBehaviour
{
    // Public variable for gravity strength
    public float gravityStrength = 9.81f;

    public bool isTrigger = true;

    public void SetGravityStrength(float strength) {
        // Get the local z-axis of the trigger collider
        Vector3 gravityDirection = transform.TransformDirection(Vector3.up);

        // Set the scene gravity to the local z-axis direction multiplied by strength
        Physics.gravity = gravityDirection * strength;
    }

    public void ResetGravity() {
        Physics.gravity = new Vector3(0f, -9.81f, 0f);
    }
    // Set gravity when player enters trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SetGravityStrength(gravityStrength);
        }
    }

    // Reset gravity when player exits trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Reset the scene gravity to the default (downwards)
            ResetGravity();
        }
    }

}
