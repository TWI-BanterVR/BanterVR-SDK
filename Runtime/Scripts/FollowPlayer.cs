using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    public GameObject target;
    public bool isTrigger = true;
    public LayerMask triggerLayerMask;
    // if no target agent will follow player
    private NavMeshAgent navAgent;
    private GameObject player;
    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("PlayerHead");
        if(player == null) {
            player = GameObject.FindWithTag("Player");
        }
    }
    private void LateUpdate()
    {
        if (target != null) 
        {
            if(navAgent != null) {
                if(navAgent.isOnNavMesh) {
                    navAgent.SetDestination(target.transform.position);
                }
            }else{
                transform.position = target.transform.position;
            }
        }
        else if (player != null)
        {
            if(navAgent != null) {
                if(navAgent.isOnNavMesh) {
                    navAgent.SetDestination(player.transform.position);
                }
            }else{
                transform.position = player.transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(isTrigger && (triggerLayerMask.value & 1 << other.gameObject.layer) > 0)
        {
            if(navAgent != null) 
            {
                if(navAgent.isOnNavMesh) {
                    navAgent.SetDestination(other.transform.position);
                }
            }
            else
            {
                transform.position = other.transform.position;
            }
        }
    }
}