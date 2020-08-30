using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpineOffset : MonoBehaviour
{
	public float offset;
	private List<Vector3> initialRotation = new List<Vector3>();

	private string character;
	private List<string> spineList;
	public List<GameObject> spineJoints;


    // Start is called before the first frame update
    void Start()
    {
		character = gameObject.name;
		if (character == "Luna_defined")
			spineList = new List<string>{ "Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt",
				"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt/Spine_3_Jnt" };
		else if(character == "David_defined")
		{

		}

		foreach (string spine in spineList) {
			GameObject spineJnt = GameObject.Find(spine);
			initialRotation.Add(spineJnt.transform.eulerAngles);
			spineJoints.Add(spineJnt);
		}
    }

	
	// Update is called once per frame
	void LateUpdate()
	{
		if(character == "Luna_defined")
		{
			spineJoints[0].transform.eulerAngles = initialRotation[0] + new Vector3(0, 0, offset);
			spineJoints[1].transform.eulerAngles = initialRotation[1] - new Vector3(0, 0, offset);
		}
		else { 
			int index = 0;
			foreach (GameObject spine in spineJoints) {
				spine.transform.eulerAngles = initialRotation[index] + new Vector3(0, 0, offset);
				index++;
			}
		}
    }
	
}
