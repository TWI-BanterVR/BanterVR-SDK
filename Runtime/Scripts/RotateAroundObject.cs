using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public Transform targetObject; // The object around which this object will rotate
    public float rotationAngle = 90f; // The amount of rotation in degrees
    public AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1); // The interpolation curve

    private bool isRotating = false;
    private float elapsedTime = 0f;
    private float lastCurveValue = 0f;

    private float RotationDuration
    {
        get
        {
            if (rotationCurve.keys.Length > 0)
                return rotationCurve.keys[rotationCurve.keys.Length - 1].time;
            else
                return 0f;
        }
    }

    private void Update()
    {
        if (isRotating)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= RotationDuration)
            {
                isRotating = false;
                elapsedTime = RotationDuration;
            }

            // Use the animation curve to determine the interpolation amount
            float currentCurveValue = rotationCurve.Evaluate(elapsedTime);
            float currentAngleDifference = rotationAngle * (currentCurveValue - lastCurveValue);
            RotateAround(targetObject.position, targetObject.right, currentAngleDifference); // Using targetObject's x-axis

            lastCurveValue = currentCurveValue;
        }
    }

    // This function will be called to start the rotation
    public void StartRotation()
    {
        isRotating = true;
        elapsedTime = 0f;
        lastCurveValue = 0f;
    }

    public void RotateAround(Vector3 point, Vector3 axis, float angle)
    {
        // This function will rotate the object around the given point, on the given axis, by the given angle
        transform.RotateAround(point, axis, angle);
    }
}
