using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadEye_control : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform headBone;
    [SerializeField] float headTrackingSpeed = 3.0f; // damping speed
    [SerializeField] float headMaxTurnAngle = 60.0f;

    [SerializeField] Transform leftEyeBone;
    [SerializeField] Transform rightEyeBone;

    [SerializeField] float eyeTrackingSpeed;
    [SerializeField] float eyeLeftMaxRotation;
    [SerializeField] float eyeLeftMinRotation;
    [SerializeField] float eyeRightMaxRotation;
    [SerializeField] float eyeRightMinRotation;

    [SerializeField] LegStepper leftStepper;
    [SerializeField] LegStepper rightStepper;


    private void Awake()
    {
        StartCoroutine(LegUpdateCoroutine());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // update head first since the eye is children of head
        HeadTrackingUpdate();
        //EyeTrackingUpdate();
    }

    void HeadTrackingUpdate()
    {
        //store head's local rotation and initialize local rotation to 0
        //Quaternion currentLocalRotation = headBone.localRotation;
        //headBone.localRotation = Quaternion.identity;

        //get the world space look direction and convert to local space
        Vector3 targetWorldLookDir = target.position - headBone.position;
        //Vector3 targetLocalLookDir = headBone.InverseTransformDirection(targetWorldLookDir);

        // limit the local space look directon
        //targetLocalLookDir = Vector3.RotateTowards(Vector3.forward, targetLocalLookDir, Mathf.Deg2Rad * headMaxTurnAngle, 0);

        // transform the head local rotation to local space look direction, with damping
        headBone.rotation = Quaternion.LookRotation(targetWorldLookDir, transform.up)* Quaternion.Euler(0, 90, -70);

        //headBone.localRotation = Quaternion.Slerp(currentLocalRotation, targetLocalRotation, 1 - Mathf.Exp(-headTrackingSpeed * Time.deltaTime));
    }

    void EyeTrackingUpdate()
    {
        Vector3 lookDir = target.position - headBone.position;
        Quaternion targetEyeRotation = Quaternion.LookRotation(lookDir, transform.up);

        leftEyeBone.rotation = Quaternion.Slerp(leftEyeBone.rotation, targetEyeRotation, 1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));
        rightEyeBone.rotation = Quaternion.Slerp(rightEyeBone.rotation, targetEyeRotation, 1 - Mathf.Exp(-eyeTrackingSpeed * Time.deltaTime));

        float leftEyeCurrentYRotation = leftEyeBone.localEulerAngles.y;
        float rightEyeCurrentYRotation = rightEyeBone.localEulerAngles.y;

        if (leftEyeCurrentYRotation > 180)
        {
            leftEyeCurrentYRotation -= 360;
        }
        if (leftEyeCurrentYRotation < -180)
        {
            leftEyeCurrentYRotation += 360;
        }
        if (rightEyeCurrentYRotation > 180)
        {
            rightEyeCurrentYRotation -= 360;
        }
        if (rightEyeCurrentYRotation < -180)
        {
            rightEyeCurrentYRotation += 360;
        }

        float leftEyeClampedYRotation = Mathf.Clamp(leftEyeCurrentYRotation, eyeLeftMinRotation, eyeLeftMaxRotation);
        float rightEyeClampedYRotation = Mathf.Clamp(rightEyeCurrentYRotation, eyeRightMinRotation, eyeRightMaxRotation);

        leftEyeBone.localEulerAngles = new Vector3(leftEyeBone.localEulerAngles.x, leftEyeClampedYRotation, leftEyeBone.localEulerAngles.z);
        rightEyeBone.localEulerAngles = new Vector3(rightEyeBone.localEulerAngles.x, rightEyeClampedYRotation, rightEyeBone.localEulerAngles.z);
    }

    IEnumerator LegUpdateCoroutine() {
        while (true) {
            do
            {
                leftStepper.TryMove();
                yield return null;
            } while (leftStepper.Moving);

            do
            {
                rightStepper.TryMove();
                yield return null;
            } while (rightStepper.Moving);
        }
    }
}
