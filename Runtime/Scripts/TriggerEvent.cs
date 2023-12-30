using UnityEngine;
using UnityEngine.Events;

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
    PlayerHead,
    Respawn,
    Finish  // new PlayerType
}
[RequireComponent(typeof(UniqueObjectId))]
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("Defines whether the trigger works for local, remote or all players")]
    public TriggerType triggerType = TriggerType.LOCALONLY;

    [Tooltip("Defines the type of player, either normal player or player head")]
    public PlayerType playerType = PlayerType.Player;

    [Tooltip("Event to call when trigger enters")]
    public UnityEvent onTriggerEnterEvent;

    [Tooltip("Event to call when trigger exits")]
    public UnityEvent onTriggerExitEvent;

    [Tooltip("Whether this should be synced across the network")]
    public bool isSynced;
    public bool adminOnly;

    [HideInInspector]
    public Transform lastColliderExit;

    [HideInInspector]
    public Transform lastColliderEnter;

#if BANTER_EDITOR 
    TriggerIndex index;
    void Start() {
        var unique = GetComponent<UniqueObjectId>();
        unique.Gen();
        index = GameObject.FindGameObjectWithTag("TriggerIndex").GetComponent<TriggerIndex>();
        index?.AddTrigger(unique.Id, this);
    }
    void OnDestroy() {
        index?.RemoveTrigger(GetComponent<UniqueObjectId>().Id);
    }
#endif

    private bool CanActivate(Collider other, string type)
    {
        string tagToCheck = playerType switch
        {
            PlayerType.Player => "Player",
            PlayerType.PlayerHead => "PlayerHead",
            PlayerType.PlayerLocoBall => "PlayerLocoBall",
            PlayerType.Respawn => "Respawn",
            PlayerType.Finish => "Finish",
            _ => "Player"
        };

        try{
            if (triggerType == TriggerType.ALL && (other.gameObject.CompareTag(tagToCheck) || other.gameObject.CompareTag("RemotePlayer")))
            {
                if(type.Equals("sq-trigger-enter")) {
                    lastColliderEnter = other.transform;
                }else{
                    lastColliderExit = other.transform;
                }
                return true;
            }
            else if (triggerType == TriggerType.LOCALONLY && other.gameObject.CompareTag(tagToCheck))
            {
                if(playerType == PlayerType.Player && playerType == PlayerType.PlayerHead && playerType == PlayerType.PlayerLocoBall) {
                    var me = GameObject.FindGameObjectWithTag("MyRemoteAvatar");
                    if(type.Equals("sq-trigger-enter")) {
                        lastColliderEnter = me.transform;
                    }else{
                        lastColliderExit = me.transform;
                    }
                }else{
                    if(type.Equals("sq-trigger-enter")) {
                        lastColliderEnter = other.transform;
                    }else{
                        lastColliderExit = other.transform;
                    }
                }
#if BANTER_EDITOR 
                if(isSynced){
                    index?.SyncTriggerEvent(type, GetComponent<UniqueObjectId>().Id);
                }
#endif
                return true;
            }
            else if (triggerType == TriggerType.REMOTEONLY && other.gameObject.CompareTag("RemotePlayer"))
            {
                if(type.Equals("sq-trigger-enter")) {
                    lastColliderEnter = other.transform;
                }else{
                    lastColliderExit = other.transform;
                }
                return true;
            }
        }catch{
            Debug.Log("Error");
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
#if BANTER_EDITOR 
        if ((adminOnly && (index?.isAdmin??false) || !adminOnly) && CanActivate(other, "sq-trigger-enter"))
#else
        if (CanActivate(other, "sq-trigger-enter"))
#endif
        {
            onTriggerEnterEvent?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
#if BANTER_EDITOR 
        if ((adminOnly && (index?.isAdmin??false) || !adminOnly) && CanActivate(other, "sq-trigger-exit"))
#else
        if (CanActivate(other, "sq-trigger-exit"))
#endif
        {   
            onTriggerExitEvent?.Invoke();
        }
    }
}