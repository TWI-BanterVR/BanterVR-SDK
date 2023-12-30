using UnityEngine;
#if BANTER_EDITOR   
using HexabodyVR.PlayerController;
#endif

public class TeleportPlayer : MonoBehaviour
{
    public GameObject destination;
    public bool isTrigger = true;
    public bool stopVelocity = true;
    public bool alignToDestination = false;
#if BANTER_EDITOR   
    HexaBodyPlayer4 localHexa;
    void Start()
    {
        localHexa = GameObject.FindGameObjectWithTag("HexaBodyPlayer4").GetComponent<HexaBodyPlayer4>();
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (isTrigger)
        {
            Trigger(other);
        }
    }

    public void Trigger()
    {
        Trigger(null);
    }

    public void Trigger(Collider other = null)
    {
#if BANTER_EDITOR 
        HexaBodyPlayer4 playerRigidbody = null;
        if (other == null)
        {
            playerRigidbody = localHexa;
        }
        else
        {
            playerRigidbody = other.GetComponentInParent<HexaBodyPlayer4>();
        }
        if (playerRigidbody != null)
        {
            playerRigidbody.MoveToPosition(destination.transform.position,stopVelocity);

            if (alignToDestination)
            {
                var eulers = playerRigidbody.Pelvis.transform.eulerAngles;
                playerRigidbody.Pelvis.transform.eulerAngles = new Vector3(eulers.x, destination.transform.eulerAngles.y-Camera.main.transform.eulerAngles.y, eulers.z);
            }
        }
#endif
    }
}
