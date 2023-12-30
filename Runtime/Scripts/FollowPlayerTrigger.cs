using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(TriggerEvent))]
public class FollowPlayerTrigger : MonoBehaviour
{
    public NavMeshAgent agent;
    private TriggerEvent triggerEvent;

    private bool isTriggered;
    // Start is called before the first frame update
    void Start()
    {
        triggerEvent = GetComponent<TriggerEvent>();
    }
    public void SetDestination(Transform destination){
        triggerEvent.lastColliderEnter = destination;
        isTriggered = true;
    }
    // Update is called once per frame
    public void SetDestination(){
        isTriggered = true;
    }

    public void RemoveDestination(){
        isTriggered = false;
    }

    void Update() {
        if(agent != null && isTriggered && agent.isOnNavMesh && agent.isActiveAndEnabled && agent.gameObject.activeSelf) {
            agent.SetDestination(triggerEvent.lastColliderEnter.position);
        }
    }
}
