using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [Tooltip("Manually set the target GameObject. If left null, the script will search for the GameObject tagged 'PlayerHead'.")]
    public GameObject targetObject;

    private Transform playerTransform; // The Transform component of the player

    void Start()
    {
        if (targetObject != null)
        {
            playerTransform = targetObject.transform; // Set the player's transform if the target GameObject is manually set
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerHead"); // Find the object with the tag "PlayerHead"

            if (playerObject != null)
            {
                playerTransform = playerObject.transform; // Set the player's transform if the target GameObject was not manually set
            }
            else
            {
                Debug.LogError("No GameObject with tag 'PlayerHead' found in the scene.");
            }
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform.position); // Make the object look at the player
        }
    }
}