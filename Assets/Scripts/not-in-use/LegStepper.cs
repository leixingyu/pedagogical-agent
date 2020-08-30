using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegStepper : MonoBehaviour
{
    [SerializeField] Transform homeTransform;
    [SerializeField] float wantStepAtDistance;
    [SerializeField] float moveDuration;
    [SerializeField] float stepOvershootFraction;

    public bool Moving;

    IEnumerator MoveToHome() {
        Moving = true;

        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;

        Quaternion endRot = homeTransform.rotation;

        Vector3 towardHome = (homeTransform.position - transform.position);

        float overShootDistance = wantStepAtDistance * stepOvershootFraction;
        Vector3 overShootVector = towardHome * overShootDistance;
        overShootVector = Vector3.ProjectOnPlane(overShootVector, Vector3.up);

        Vector3 endPos = homeTransform.position + overShootVector;
        Vector3 centerPos = (startPos + endPos) / 2;
        centerPos += homeTransform.up * Vector3.Distance(startPos, endPos) / 2f;

        float timeElapsed = 0;

        do
        {
            timeElapsed += Time.deltaTime;

            float normalizedTime = timeElapsed / moveDuration;
            normalizedTime = Easing.Cubic.InOut(normalizedTime);

            transform.position =
                Vector3.Lerp(
                    Vector3.Lerp(startPos, centerPos, normalizedTime),
                    Vector3.Lerp(centerPos, endPos, normalizedTime),
                    normalizedTime
                );

            transform.rotation = Quaternion.Slerp(startRot, endRot, normalizedTime);

            yield return null;
        }
        while (timeElapsed < moveDuration);

        Moving = false;
    }

    public void TryMove()
    {
        if (Moving) return;

        float distFromHome = Vector3.Distance(transform.position, homeTransform.position);

        if (distFromHome > wantStepAtDistance)
        {
            StartCoroutine(MoveToHome());
        }
    }
}
