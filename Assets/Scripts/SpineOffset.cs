using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Setup Offset component for character spine
 * Needs to be called by the global control or in other places */

public class SpineOffset : MonoBehaviour
{
	public float offset = 0.0f;

	private float initialZRot;
	private string character;
	private List<string> spineList;
	public List<GameObject> spineJoints;  // this has to be public otherwise error

    void Start()
    {
		character = gameObject.name;
		if (character == Global.Luna)
			spineList = Global.LunaSpine;

		else if (character == Global.David)
			spineList = Global.DavidSpine;

		// add spine transform to list
		foreach (string spine in spineList) {
			GameObject spineJnt = GameObject.Find(spine);
			Debug.Assert(spineJnt, "spine joint not found for offset" );
			initialZRot = spineJnt.transform.eulerAngles[2];
			spineJoints.Add(spineJnt);
		}

    }

	void LateUpdate()
	{
		if(character == Global.Luna)
		{
			spineJoints[0].transform.eulerAngles = new Vector3(spineJoints[0].transform.eulerAngles[0],
										  spineJoints[0].transform.eulerAngles[1],
										  initialZRot + offset);

			spineJoints[1].transform.eulerAngles = new Vector3(spineJoints[1].transform.eulerAngles[0],
										  spineJoints[1].transform.eulerAngles[1],
										  initialZRot - offset);
		}
		else { 
			int index = 0;
			foreach (GameObject spine in spineJoints) {
				spine.transform.eulerAngles = new Vector3(spine.transform.eulerAngles[0],
														  spine.transform.eulerAngles[1],
														  initialZRot - offset);
				index++;
			}
		}
    }

	public IEnumerator spineOffset(Global.BodyOffset type, float strength = 100.0f, int frames = Setting.bodyOffsetBlend)
	{
		float previous = offset;
		float next;

		if (type == Global.BodyOffset.FORWARD)
			next = Setting.bodyLeanExtreme * strength / 100.0f;
		else if (type == Global.BodyOffset.BACKWARD)
			next = Setting.bodyLeanExtreme * strength / 100.0f;
		else
			next = 0.0f;

		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			float currentVal = Mathf.Lerp(previous, next, (float)i / (float)frames);
			offset = currentVal;
		}
	}
}
