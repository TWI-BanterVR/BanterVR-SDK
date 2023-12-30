using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyForAdmin : MonoBehaviour
{
    [Tooltip("Objects that will only be visible to admins")]
    public GameObject[] objects;

#if BANTER_EDITOR 
    TriggerIndex index;
    bool wasAdmin;
    void Start() {
        index = GameObject.FindGameObjectWithTag("TriggerIndex").GetComponent<TriggerIndex>();
        
    }

    private void OnEnable()
    {
        if(index?.isAdmin??false) {
            HideShowObjects(true);
        }else{
            HideShowObjects(false);
        }
    }

    void Update() {
        if(index.isAdmin != wasAdmin) {
            if(index.isAdmin) {
                HideShowObjects(true);
            }else{
                HideShowObjects(false);
            }
            wasAdmin = index.isAdmin;
        }
    }

    void HideShowObjects(bool show) {
        foreach(var obj in objects) {
            obj.SetActive(show);
        }
    }
#endif
}
