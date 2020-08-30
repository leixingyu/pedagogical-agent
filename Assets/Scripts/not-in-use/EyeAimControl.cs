using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeAimControl : MonoBehaviour
{
	public RootMotion.FinalIK.LookAtIK ikSystem;
	Rect toggleRect = new Rect(350f, 60f, 100f, 20f);
	bool worldSpace = false;
	GameObject worldAim;
	GameObject objectAim;

	// Start is called before the first frame update
	void Start()
    {
		worldAim = GameObject.Find("WorldSpace_Aim_IK");
		objectAim = GameObject.Find("ObjectSpace_Aim_IK");
		//ikSystem.solver.target = objectAim.transform;
		//setObjectAim();
	}

	private void OnGUI()
	{
		if (GUI.Button(toggleRect, "Toggle Look IK")){
			worldSpace = !worldSpace;
			if (worldSpace) setWorldAim();
			else setObjectAim();
		}
	}

	public void setWorldAim(){
		ikSystem.solver.target = worldAim.transform;
	}

	public void setObjectAim() {
		ikSystem.solver.target = objectAim.transform;
	}
}
