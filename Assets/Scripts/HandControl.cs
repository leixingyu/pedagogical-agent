using UnityEngine;

public class HandControl : MonoBehaviour
{
	static Animator anim;

	void Start()
    {
		anim = GetComponent<Animator>();
	}

	static public void SetLeftHand(int index)
	{
		anim.SetInteger(Global.leftHandLayer, index);
	}

	static public void SetRightHand(int index)
	{
		anim.SetInteger(Global.rightHandLayer, index);
	}
}
