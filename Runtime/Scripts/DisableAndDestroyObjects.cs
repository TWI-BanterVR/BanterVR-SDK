using UnityEngine;

public class DisableAndDestroyObjects : MonoBehaviour
{
    // Define a list of objects to disable in the inspector
    public GameObject[] objectsToDisable;

    // Define a list of objects to destroy in the inspector
    public GameObject[] objectsToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        // Loop through the list and disable each object
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }

        // Loop through the list and destroy each object
        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }
}