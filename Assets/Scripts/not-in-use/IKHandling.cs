using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandling : MonoBehaviour
{

    Animator anim;

    public float ikWeight;

    public Transform leftIKTarget;
    public Transform rightIKTarget;

    public Transform hintLeft;
    public Transform hintRight;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKTarget.position);

        anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, ikWeight);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, ikWeight);

        anim.SetIKHintPosition(AvatarIKHint.LeftKnee, hintLeft.position);
        anim.SetIKHintPosition(AvatarIKHint.RightKnee, hintRight.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, ikWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, ikWeight);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftIKTarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightIKTarget.rotation);
    }

}
