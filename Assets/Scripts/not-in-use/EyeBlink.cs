using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBlink : MonoBehaviour
{
	System.Random rnd = new System.Random();
	SkinnedMeshRenderer eyeblinkCtrl;

	// Start is called before the first frame update
	void OnEnable()
	{
		eyeblinkCtrl = GetComponent<SkinnedMeshRenderer>();
		StartCoroutine(rndBlink());
	}

	// blink once after random seconds
	IEnumerator rndBlink()
	{
		while (true)
		{
			int randSec = rnd.Next(2, 6);
			yield return new WaitForSeconds(randSec);
			StartCoroutine(eyeBlink());
		}
	}

	// frames between each blinks closing and opening
	public IEnumerator eyeBlink(int closeFrame = 10, int holdFrame = 6,
								int openFrame = 10, int blinkIndex = 3)
	{
		float blinkWeight = 0.0f;
		eyeblinkCtrl.SetBlendShapeWeight(blinkIndex, 0); //initialize the blendshape index

		//closing for certain frames
		for (int i = 0; i < closeFrame; i++)
		{
			yield return null;
			blinkWeight += 100.0f / closeFrame;
			eyeblinkCtrl.SetBlendShapeWeight(blinkIndex, blinkWeight);
		}

		//hold for certain frames
		for (int i = 0; i < holdFrame; i++)
		{
			yield return null;
		}

		//opening for certain frames
		for (int i = 0; i < openFrame; i++)
		{
			yield return null;
			blinkWeight -= 100.0f / openFrame;
			eyeblinkCtrl.SetBlendShapeWeight(blinkIndex, blinkWeight);
		}
	}

}
