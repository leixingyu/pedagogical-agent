using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HandLayerControl : MonoBehaviour
{
	static Animator anim;

	public enum HandPose
	{
		Relax,
		Palm,
		Fist,
		FistRelax,
	}

	// Start is called before the first frame update
	void Start()
    {
		anim = GetComponent<Animator>();
	}

	static public void setLeftHand(int index) {
		anim.SetInteger("LHand", index);
	}

	static public void setRightHand(int index)
	{
		anim.SetInteger("RHand", index);
	}
}
