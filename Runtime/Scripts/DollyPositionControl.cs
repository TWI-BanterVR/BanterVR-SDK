using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class DollyPositionControl : MonoBehaviour
{
    [Tooltip("Assign if you want to control the dolly position with a slider. Leave empty if not using a slider.")]
    public Slider positionSlider;

    [Tooltip("Assign if you want to control the dolly position with a scrollbar. Leave empty if not using a scrollbar.")]
    public Scrollbar positionScrollbar;

    [Tooltip("Toggle this to enable or disable changing FOV based on dolly position.")]
    public bool changeFOV = true;

    [Tooltip("The minimum FOV value when moving along the dolly track. Only used if 'Change FOV' is enabled.")]
    public float minFOV = 30f;

    [Tooltip("The maximum FOV value when moving along the dolly track. Only used if 'Change FOV' is enabled.")]
    public float maxFOV = 60f;

    [Tooltip("The speed at which the FOV changes. Higher values make the FOV change more quickly.")]
    public float fovChangeSpeed = 2f;

    private CinemachineVirtualCamera dollyCamera;
    private CinemachineTrackedDolly dolly;
    private CinemachineSmoothPath smoothPath;
    private float targetFOV;

    void Start()
    {
        // Get the CinemachineVirtualCamera component from the same GameObject
        dollyCamera = GetComponent<CinemachineVirtualCamera>();

        // Initialize dolly
        dolly = dollyCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

        // Get the CinemachineSmoothPath from the dolly
        smoothPath = dolly.m_Path as CinemachineSmoothPath;

        // Set the initial target FOV to the current camera's FOV
        targetFOV = dollyCamera.m_Lens.FieldOfView;

        // Setup slider or scrollbar
        if (positionSlider != null)
        {
            positionSlider.minValue = 0;  // The start of the path
            positionSlider.maxValue = smoothPath.m_Waypoints.Length - 1;  // The end of the path
            positionSlider.onValueChanged.AddListener(UpdateDollyPositionAndFOVSlider);
        }

        if (positionScrollbar != null)
        {
            positionScrollbar.onValueChanged.AddListener(UpdateDollyPositionAndFOVScrollbar);
        }
    }

    void Update()
    {
        if (changeFOV)
        {
            // Smoothly interpolate to the target FOV
            dollyCamera.m_Lens.FieldOfView = Mathf.Lerp(dollyCamera.m_Lens.FieldOfView, targetFOV, fovChangeSpeed * Time.deltaTime);
        }
    }

    public void UpdateDollyPositionAndFOVSlider(float newValue)
    {
        // Directly assign the newValue to the path position
        dolly.m_PathPosition = newValue;

        if (changeFOV)
        {
            // Compute the new target FOV
            float t = (newValue - positionSlider.minValue) / (positionSlider.maxValue - positionSlider.minValue);
            targetFOV = Mathf.Lerp(minFOV, maxFOV, t);
        }
    }

    public void UpdateDollyPositionAndFOVScrollbar(float newValue)
    {
        // Compute the new position based on the number of waypoints
        float newPosition = newValue * (smoothPath.m_Waypoints.Length - 1);

        // Assign the new position to the dolly path
        dolly.m_PathPosition = newPosition;

        if (changeFOV)
        {
            // Compute the new target FOV
            targetFOV = Mathf.Lerp(minFOV, maxFOV, newValue);
        }
    }
}