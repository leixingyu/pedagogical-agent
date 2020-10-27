using System.Collections;
using UnityEngine;

/* Setup Facial Expression component for character of different emotion
 * Needs to be called by the global control or in other places */

public class FacialBlendShape : MonoBehaviour
{
	SkinnedMeshRenderer facialCtrl;
	Mesh characterMesh;

	private int blendShapeCount;
	private int facialStartIndex = 4;  // index 0, 1, 2, 3 for visem and eyeblink
	private string character;

	// Start is called before the first frame update
	void OnEnable()
    {
		facialCtrl = GetComponent<SkinnedMeshRenderer>();	
		if(facialCtrl.name == Global.LunaMesh)
			character = Global.Luna;

		else if (facialCtrl.name == Global.DavidMesh)
			character = Global.David;

		characterMesh = facialCtrl.sharedMesh;
		blendShapeCount = characterMesh.blendShapeCount;
	}

	public IEnumerator blendToWeight(int index, float targetWeight, int frames = Setting.facialBlend)
	{
		float currentWeight = facialCtrl.GetBlendShapeWeight(index);

		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			float resultWeight = Mathf.Lerp(currentWeight, targetWeight, (float)i / (float)frames);
			facialCtrl.SetBlendShapeWeight(index, resultWeight);
		}
	}

	public void resetBlendShape() {
		for (int i = facialStartIndex; i < blendShapeCount; i++)
		{
			if (character != Global.Luna || i != 17) // exlude reseting luna chest fix
				StartCoroutine(blendToWeight(i, 0));
		}
	}

	public IEnumerator browRaise() {
		int browIndex = 0;
		if (character == Global.David)	browIndex = 6;
		else if (character == Global.Luna)	browIndex = 20;

		System.Random rnd = new System.Random();
		float randSec = Random.Range(0.75f, 1.25f);
		int targetWeight = rnd.Next(80, 100);
		StartCoroutine(blendToWeight(browIndex, targetWeight, Setting.browBlend));
		yield return new WaitForSeconds(randSec);
		StartCoroutine(blendToWeight(browIndex, 20, Setting.browBlend));
	}

	public void setHappy(int strength)
	{
		resetBlendShape();
		if (character == Global.David) { 
			StartCoroutine(blendToWeight(9, 0.2f * strength));
			StartCoroutine(blendToWeight(10, 0.4f * strength));
			StartCoroutine(blendToWeight(13, 0.6f * strength));	
		}
		else if (character == Global.Luna)
		{
			StartCoroutine(blendToWeight(11, 0.7f * strength));
			StartCoroutine(blendToWeight(16, 0.6f * strength));
			StartCoroutine(blendToWeight(7, 0.15f * strength));
			StartCoroutine(blendToWeight(8, 0.15f * strength));
		}
	}

	public void setBored(int strength) {
		resetBlendShape();
		if (character == Global.David)
		{
			StartCoroutine(blendToWeight(4, 1.0f * strength));
			StartCoroutine(blendToWeight(6, 0.5f * strength));
			StartCoroutine(blendToWeight(9, 0.7f * strength));
		}
		else if (character == Global.Luna)
		{
			StartCoroutine(blendToWeight(20, 1.0f * strength));
			StartCoroutine(blendToWeight(15, 1.0f * strength));
		}
	}

	public void setAngry(int strength) {
		resetBlendShape();
		if (character == Global.David)
		{
			StartCoroutine(blendToWeight(5, 1.0f * strength));
			StartCoroutine(blendToWeight(7, 0.3f * strength));
			StartCoroutine(blendToWeight(8, 1.0f * strength));
			StartCoroutine(blendToWeight(10, 0.5f * strength));
			StartCoroutine(blendToWeight(11, 1.0f * strength));
		}
		else if (character == Global.Luna)
		{
			StartCoroutine(blendToWeight(4, 0.75f * strength));
			StartCoroutine(blendToWeight(5, 0.75f * strength));
			StartCoroutine(blendToWeight(15, 1.0f * strength));
			StartCoroutine(blendToWeight(26, 0.4f * strength));
		}
	}

	public void setContent(int strength) {
		resetBlendShape();
		if (character == Global.David)
		{
			StartCoroutine(blendToWeight(6, 1.0f * strength));
			StartCoroutine(blendToWeight(9, 0.4f * strength));
		}
		else if (character == Global.Luna)
		{
			StartCoroutine(blendToWeight(11, 0.15f * strength));
			StartCoroutine(blendToWeight(21, 0.4f * strength));
		}
	}
}
