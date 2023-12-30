using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorTrigger : MonoBehaviour
{
    public string property;

    public Animator animator;

    public void SetPropertyName(string property) {
        this.property = property;
    }
    
    public void SetBool(bool value) {
        animator?.SetBool(property, value);
    }
    public void SetFloat(float value) {
        animator?.SetFloat(property, value);
    }
    public void SetInteger(int value) {
        animator?.SetInteger(property, value);
    }
}
