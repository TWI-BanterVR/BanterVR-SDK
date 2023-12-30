using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // For ExecuteEvents

[RequireComponent(typeof(UniqueObjectId))]
public class ButtonTriggerSync : MonoBehaviour
{
    Selectable selectable;
    public bool adminOnly;

#if BANTER_EDITOR
    TriggerIndex index;
    bool hasRecievedClick = false;

    void Start()
    {
        index = GameObject.FindGameObjectWithTag("TriggerIndex").GetComponent<TriggerIndex>();
        index?.AddButtonTrigger(GetComponent<UniqueObjectId>().Id, this);

        selectable = GetComponent<Selectable>();

        if (selectable is Button button)
        {
            button.onClick.AddListener(SendClick);
        }
        else if (selectable is Toggle toggle)
        {
            toggle.onValueChanged.AddListener(delegate { SendClick(); });
        }
    }

    void OnDestroy()
    {
        index?.RemoveButtonTrigger(GetComponent<UniqueObjectId>().Id);
    }
#endif

    public void RemoteClickButton()
    {
        if (selectable is Button button)
        {
            button.onClick.RemoveListener(SendClick);
            button.OnSubmit(null);
            button.onClick.AddListener(SendClick);
        }
        else if (selectable is Toggle toggle)
        {
            toggle.onValueChanged.RemoveListener(delegate { SendClick(); });
            toggle.isOn = !toggle.isOn;
            toggle.onValueChanged.AddListener(delegate { SendClick(); });
        }
    }

    private void SendClick()
    {
#if BANTER_EDITOR
        if (adminOnly && (index?.isAdmin ?? false) || !adminOnly)
        {
            index?.SyncTriggerEvent("sq-button-click", GetComponent<UniqueObjectId>().Id);
        }
#endif
    }
}