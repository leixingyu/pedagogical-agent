using System.Collections;
using UnityEngine;

public class ExpressionControl : MonoBehaviour
{
	SkinnedMeshRenderer skinnedMesh;
	GameObject character;
	int blendShapeCount;
	int facialStartIndex = 4;  // index 0, 1, 2, 3 for visem and eyeblink

	void OnEnable()
    {
		character = GameObject.FindGameObjectWithTag("Player");
		GameObject blendMeshObject = null;

		if (character.name == Global.luna)
		{
			blendMeshObject = character.transform.Find(Global.lunaMesh).gameObject;
		}

		else if (character.name == Global.david)
		{
			blendMeshObject = character.transform.Find(Global.davidMesh).gameObject;
		}

		skinnedMesh = blendMeshObject.GetComponent<SkinnedMeshRenderer>();		
		Mesh characterMesh = skinnedMesh.sharedMesh;
		blendShapeCount = characterMesh.blendShapeCount;
	}

	public void ResetBlendShape()
	{
		for (int i = facialStartIndex; i < blendShapeCount; i++)
		{
			if (character.name != Global.luna || i != 17) // exlude reseting luna chest fix
				StartCoroutine(BlendWeight(i, 0));
		}
	}

	public IEnumerator BlendWeight(int index, float targetWeight, int frames = Setting.facialBlend)
	{
		float currentWeight = skinnedMesh.GetBlendShapeWeight(index);

		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			float resultWeight = Mathf.Lerp(currentWeight, targetWeight, (float)i / (float)frames);
			skinnedMesh.SetBlendShapeWeight(index, resultWeight);
		}
	}

	public IEnumerator BrowRaise() {
		int browIndex = 0;
		if (character.name == Global.david)	browIndex = 6;
		else if (character.name == Global.luna)	browIndex = 20;

		System.Random rnd = new System.Random();
		float randSec = Random.Range(0.75f, 1.25f);
		int targetWeight = rnd.Next(80, 100);
		StartCoroutine(BlendWeight(browIndex, targetWeight, Setting.browBlend));
		yield return new WaitForSeconds(randSec);
		StartCoroutine(BlendWeight(browIndex, 20, Setting.browBlend));
	}

	public void SetHappy(int strength)
	{
		ResetBlendShape();
		if (character.name == Global.david) { 
			StartCoroutine(BlendWeight(9, 0.2f * strength));
			StartCoroutine(BlendWeight(10, 0.4f * strength));
			StartCoroutine(BlendWeight(13, 0.6f * strength));	
		}
		else if (character.name == Global.luna)
		{
			StartCoroutine(BlendWeight(11, 0.7f * strength));
			StartCoroutine(BlendWeight(16, 0.6f * strength));
			StartCoroutine(BlendWeight(7, 0.15f * strength));
			StartCoroutine(BlendWeight(8, 0.15f * strength));
		}
	}

	public void SetBored(int strength) {
		ResetBlendShape();
		if (character.name == Global.david)
		{
			StartCoroutine(BlendWeight(4, 1.0f * strength));
			StartCoroutine(BlendWeight(6, 0.5f * strength));
			StartCoroutine(BlendWeight(9, 0.7f * strength));
		}
		else if (character.name == Global.luna)
		{
			StartCoroutine(BlendWeight(20, 1.0f * strength));
			StartCoroutine(BlendWeight(15, 1.0f * strength));
		}
	}

	public void SetAngry(int strength) {
		ResetBlendShape();
		if (character.name == Global.david)
		{
			StartCoroutine(BlendWeight(5, 1.0f * strength));
			StartCoroutine(BlendWeight(7, 1.0f * strength));
			StartCoroutine(BlendWeight(8, 1.0f * strength));
			StartCoroutine(BlendWeight(10, 1.0f * strength));
			StartCoroutine(BlendWeight(11, 1.0f * strength));
		}
		else if (character.name == Global.luna)
		{
			StartCoroutine(BlendWeight(4, 0.75f * strength));
			StartCoroutine(BlendWeight(5, 0.75f * strength));
			StartCoroutine(BlendWeight(15, 1.0f * strength));
			StartCoroutine(BlendWeight(26, 0.4f * strength));
		}
	}

	public void SetContent(int strength) {
		ResetBlendShape();
		if (character.name == Global.david)
		{
			StartCoroutine(BlendWeight(6, 1.0f * strength));
			StartCoroutine(BlendWeight(9, 0.4f * strength));
		}
		else if (character.name == Global.luna)
		{
			StartCoroutine(BlendWeight(11, 0.15f * strength));
			StartCoroutine(BlendWeight(21, 0.4f * strength));
		}
	}
}
