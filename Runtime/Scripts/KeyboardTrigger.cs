using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(UniqueObjectId))]
public class KeyboardTrigger : MonoBehaviour
{
    public KeyCode key = KeyCode.Space;
    public UnityEvent onKeyPress;
    public bool isSynced;
    public bool adminOnly;
    private Button button;
    [SerializeField] private GameObject _lobby;
#if BANTER_EDITOR 
    TriggerIndex index;
#endif
    void Start() {
        button = GetComponent<Button>();
#if BANTER_EDITOR 
        var unique = GetComponent<UniqueObjectId>();
        unique.Gen();
        index = GameObject.FindGameObjectWithTag("TriggerIndex").GetComponent<TriggerIndex>();
        index?.AddKeyboardTrigger(unique.Id, this);
#endif
    }
#if BANTER_EDITOR 
    void OnDestroy() {
        index?.RemoveTrigger(GetComponent<UniqueObjectId>().Id);
    }
#endif

    void Update() {
        if(Input.GetKeyDown(key)) {
            Trigger(true);
        }
    }

    public void Trigger(bool isLocal = false) {
#if BANTER_EDITOR 
        if(isSynced && isLocal && ((adminOnly && ((index?.isAdmin??false) || _lobby.activeSelf)) || !adminOnly)){
            index?.SyncTriggerEvent("sq-keyboard-key", GetComponent<UniqueObjectId>().Id);
        }
        if((adminOnly && ((index?.isAdmin??false) || _lobby.activeSelf)) || !adminOnly) {
#endif
            if(button != null) {
                button.OnSubmit(null);
            }else{
                onKeyPress?.Invoke();
            }
#if BANTER_EDITOR 
        }
#endif
    }
}
