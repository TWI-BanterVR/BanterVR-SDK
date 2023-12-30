using UnityEngine;

public class ForceTrigger : MonoBehaviour
{
    [Tooltip("If true, the trigger will fire when the player ball enters it")]
    public bool isTrigger = true;

    [Tooltip("The strength of the impulse when the player ball enters the trigger")]
    public float triggerImpulseStrength = 10f;

    [Tooltip("If you want to move a different body than the player ball, set it here")]
    public Rigidbody otherBodyToMove;

    [Tooltip("If true, will do an impulse instead of a force")]
    public bool TriggerIsImpulse;
    private Rigidbody currentBody;

    public void SetImpulseOnPlayer(float strength) {
        var playerRigidbody = GameObject.FindGameObjectWithTag("PlayerLocoBall").GetComponent<Rigidbody>();
        if(playerRigidbody != null) {
            SetForce(strength, playerRigidbody);
        }
    }

    public void SetImpulseOnOtherBody(float strength) {
        if(otherBodyToMove != null) {
            SetForce(strength, otherBodyToMove);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("PlayerLocoBall") && isTrigger) {
            var playerRigidbody = otherBodyToMove == null ? other.gameObject.GetComponent<Rigidbody>() : otherBodyToMove;
            if(TriggerIsImpulse){
                SetForce(triggerImpulseStrength, playerRigidbody);
            }else{
                currentBody = playerRigidbody == null ? gameObject.GetComponent<Rigidbody>() : playerRigidbody;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("PlayerLocoBall") && isTrigger) {
            if(!TriggerIsImpulse && currentBody != null) {
                currentBody = null;
            }
        }
    }

    private void SetForce(float strength, Rigidbody playerRigidbody = null) {
        currentBody = playerRigidbody == null ? gameObject.GetComponent<Rigidbody>() : playerRigidbody;
        if (playerRigidbody != null) {
            playerRigidbody.AddForce(transform.up * strength, ForceMode.Impulse);
        }
    }

    void FixedUpdate() {
        if(!TriggerIsImpulse && currentBody != null) {
            currentBody.AddForce(transform.up * triggerImpulseStrength, ForceMode.Force);
        }
    }
}