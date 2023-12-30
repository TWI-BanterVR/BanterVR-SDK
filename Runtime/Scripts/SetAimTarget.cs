using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetAimTarget : MonoBehaviour
{
    public enum TriggerType
    {
        LOCALONLY,
        REMOTEONLY,
        ALL
    }

    public enum PlayerType
    {
        PlayerLocoBall,
        Player,
        PlayerHead
    }

    [Tooltip("Layer mask to specify which layers the Raycast can hit.")]
    public LayerMask raycastLayerMask;

    [Tooltip("Enable or disable setting the Follow of the active virtual camera.")]
    public bool enableFollow = true;


    [Tooltip("List of Cinemachine virtual cameras.")]
    public List<CinemachineVirtualCamera> cameraList;

    [Tooltip("If true, the script will disable itself after setting a new aim target.")]
    public bool disableAfterUse = true;

    [Tooltip("Custom camera to use instead of the default camera. Leave empty if you want to use the default camera.")]
    public Camera customCamera;

    [Tooltip("If true, the script will set the camera target on Trigger Enter.")]
    public bool useTrigger = false;

    [Tooltip("Defines whether the trigger works for local, remote or all players")]
    public TriggerType triggerType = TriggerType.LOCALONLY;

    [Tooltip("Defines the type of player, either normal player or player head")]
    public PlayerType playerType = PlayerType.Player;

    private CinemachineBrain cinemachineBrain;
    private Camera cinemachineCamera;
    private Coroutine raycastCoroutine;

    public void SetEnableFollow(bool enable)
    {
        enableFollow = enable;
    }


    public void SetTarget(GameObject target)
    {
            // Instead of finding all Cinemachine cameras, use the cameraList
            foreach (var virtualCamera in cameraList)
            {
                if (target != null)
                {
                    virtualCamera.LookAt = target.transform;
                    if (enableFollow)
                    {
                        virtualCamera.Follow = target.transform;
                    }
                }
            }
            if (disableAfterUse)
                this.enabled = false;
        }

    

    private void Awake()
    {
        if (useTrigger && customCamera != null)
        {
            cinemachineBrain = customCamera.GetComponent<CinemachineBrain>();
            cinemachineCamera = customCamera;
        }
        else
        {
            cinemachineBrain = GetComponent<CinemachineBrain>();
            cinemachineCamera = GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (useTrigger)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Camera activeCamera = customCamera == null ? cinemachineCamera : customCamera;

            if (activeCamera != null && cinemachineBrain != null)
            {
                Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);

                if (raycastCoroutine != null)
                {
                    StopCoroutine(raycastCoroutine);
                }
                raycastCoroutine = StartCoroutine(PerformRaycast(ray));
            }
        }
    }


    IEnumerator PerformRaycast(Ray ray)
    {
        yield return null; // Wait for next frame

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
        {
            SetTarget(hit.collider.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
        {
            string tagToCheck = playerType == PlayerType.Player ? "Player" : playerType == PlayerType.PlayerHead ? "PlayerHead" : "PlayerLocoBall";
            if (triggerType == TriggerType.ALL && (other.gameObject.CompareTag(tagToCheck) || other.gameObject.CompareTag("RemotePlayer")))
            {
                SetTarget(other.gameObject);
            }
            else if (triggerType == TriggerType.LOCALONLY && other.gameObject.CompareTag(tagToCheck))
            {
                SetTarget(other.gameObject);
            }
        }
    }
}