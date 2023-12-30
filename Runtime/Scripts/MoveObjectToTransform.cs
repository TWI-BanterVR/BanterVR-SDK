using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectToTransform : MonoBehaviour {
    public bool isLerped;
    public bool isRigidBody;
    public float lerpTime = 1;
    public float rotateLerpTime = 1;
    public Transform targetObject;
    private float startedPosTime = -1;
    private float startedRotTime = -1;
    private Vector3 targetPos;
    private Quaternion targetRot;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Rigidbody body;
    public void SetObjectPosition(Transform _targetObject) {
        if(isRigidBody && body == null) {
            body = GetComponent<Rigidbody>();
        }
        if(isLerped) {
            targetPos = this.targetObject != null ? this.targetObject.position : _targetObject.position;
            initialPos = transform.position;
            startedPosTime = Time.fixedUnscaledTime;
        }else{
            if(isRigidBody && body != null) {
                body.position = this.targetObject != null ? this.targetObject.position : _targetObject.position;
            }else{
                transform.position = this.targetObject != null ? this.targetObject.position : _targetObject.position;
            }
        }
    }

    public void SetObjectRotation(Transform _targetObject) {
        if(isRigidBody && body == null) {
            body = GetComponent<Rigidbody>();
        }
        if(isLerped) {
            targetRot = this.targetObject != null ? this.targetObject.rotation : _targetObject.rotation;
            initialRot = transform.rotation;
            startedRotTime = Time.fixedUnscaledTime;
        }else{
            if(isRigidBody && body != null) {
                body.rotation = this.targetObject != null ? this.targetObject.rotation : _targetObject.rotation;
            }else{
                transform.rotation = this.targetObject != null ? this.targetObject.rotation : _targetObject.rotation;
            }
        }
    }

    public void SetObjectPositionAndRotation(Transform targetObject) {
        SetObjectPosition(targetObject);
        SetObjectRotation(targetObject);
    }

    void FixedUpdate() {
        var timePos = Time.fixedUnscaledTime - startedPosTime;
        var timeRot = Time.fixedUnscaledTime - startedRotTime;
        if(isLerped) {
            if(startedPosTime > -1) {
                var pos = Vector3.Lerp(initialPos, targetPos, timePos / lerpTime);
                if(isRigidBody && body != null) {
                    body.position = pos;
                }else{
                    transform.position = pos;
                }
            }
            if(startedRotTime > -1) {
                var rot = Quaternion.Slerp(initialRot, targetRot, timeRot / rotateLerpTime);
                if(isRigidBody && body != null) {
                    body.rotation = rot;
                }else{
                    transform.rotation = rot;
                }
            }
            if(timePos > lerpTime && startedPosTime > -1) {
                startedPosTime = -1;
            }
            if(timeRot > rotateLerpTime && startedRotTime > -1) {
                startedRotTime = -1;
            }
        }
    }
}