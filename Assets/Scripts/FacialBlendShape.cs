using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacialBlendShape : MonoBehaviour
{
	SkinnedMeshRenderer facialCtrl;
	Mesh characterMesh;
	int blendShapeCount;
	int facialStartIndex = 4;
	string character;

	const int defaultTransFrame = 12;
	const int browTransFrame = 6;

	// Start is called before the first frame update
	void OnEnable()
    {
		facialCtrl = GetComponent<SkinnedMeshRenderer>();	
		if(facialCtrl.name == "Mesh_Luna_Full")
			character = "Luna";

		else if (facialCtrl.name == "base")
			character = "David";

		characterMesh = facialCtrl.sharedMesh;
		blendShapeCount = characterMesh.blendShapeCount;
	}

	public IEnumerator blendToWeight(int index, float targetWeight, int frames = defaultTransFrame)
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
			if (character != "Luna" || i != 17) // exlude reseting luna chest fix
				StartCoroutine(blendToWeight(i, 0));
		}
	}

	public IEnumerator browRaise() {
		int browIndex = 0;
		if (character == "David")	browIndex = 6;
		else if (character == "Luna")	browIndex = 20;

		System.Random rnd = new System.Random();
		float randSec = Random.Range(0.75f, 1.25f);
		int targetWeight = rnd.Next(80, 100);
		StartCoroutine(blendToWeight(browIndex, targetWeight, browTransFrame));
		yield return new WaitForSeconds(randSec);
		StartCoroutine(blendToWeight(browIndex, 20, browTransFrame));
	}

	public void setHappy(int strength)
	{
		resetBlendShape();
		if (character == "David") { 
			StartCoroutine(blendToWeight(9, 0.2f * strength));
			StartCoroutine(blendToWeight(10, 0.4f * strength));
			StartCoroutine(blendToWeight(13, 0.6f * strength));	
		}
		else if (character == "Luna")
		{
			StartCoroutine(blendToWeight(11, 0.7f * strength));
			StartCoroutine(blendToWeight(16, 0.6f * strength));
			StartCoroutine(blendToWeight(7, 0.15f * strength));
			StartCoroutine(blendToWeight(8, 0.15f * strength));
		}
	}

	public void setBored(int strength) {
		resetBlendShape();
		if (character == "David")
		{
			StartCoroutine(blendToWeight(4, 1.0f * strength));
			StartCoroutine(blendToWeight(6, 0.5f * strength));
			StartCoroutine(blendToWeight(9, 0.7f * strength));
		}
		else if (character == "Luna")
		{
			StartCoroutine(blendToWeight(20, 1.0f * strength));
			StartCoroutine(blendToWeight(15, 1.0f * strength));
		}
	}

	public void setAngry(int strength) {
		resetBlendShape();
		if (character == "David")
		{
			StartCoroutine(blendToWeight(5, 1.0f * strength));
			StartCoroutine(blendToWeight(7, 0.3f * strength));
			StartCoroutine(blendToWeight(8, 1.0f * strength));
			StartCoroutine(blendToWeight(10, 0.5f * strength));
			StartCoroutine(blendToWeight(11, 1.0f * strength));
		}
		else if (character == "Luna")
		{
			StartCoroutine(blendToWeight(4, 0.75f * strength));
			StartCoroutine(blendToWeight(5, 0.75f * strength));
			StartCoroutine(blendToWeight(15, 1.0f * strength));
			StartCoroutine(blendToWeight(26, 0.4f * strength));
		}
	}

	public void setContent(int strength) {
		resetBlendShape();
		if (character == "David")
		{
			StartCoroutine(blendToWeight(6, 1.0f * strength));
			StartCoroutine(blendToWeight(9, 0.4f * strength));
		}
		else if (character == "Luna")
		{
			StartCoroutine(blendToWeight(11, 0.15f * strength));
			StartCoroutine(blendToWeight(21, 0.4f * strength));
		}
	}
}
