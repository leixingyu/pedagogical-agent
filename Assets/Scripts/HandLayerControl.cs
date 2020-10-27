using UnityEngine;

/* Setup Hand control by accessing pre-set animator components
 * Needs to be called by the global control or in other places */

public class HandLayerControl : MonoBehaviour
{
	static Animator anim;

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
