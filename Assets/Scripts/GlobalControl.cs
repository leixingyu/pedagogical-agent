using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using System;

public class GlobalControl : MonoBehaviour
{
	public GameObject character;		// assign character
	public GameObject skinnedMesh;		// assign skinned mesh under character
	public bool Offset = false;
	public bool Interface = false;

	private EffectorOffset charOffsetCtrl;
	private SpineOffset spineOffsetCtrl;
	private MecanimControl mecanimControl;
	private LegIKControl legControl;
	private FacialBlendShape facialControl;

	private bool emitorFlag = false;

	private bool mirror = false;
	private float animSpeed = 1.0f;
	private float currentRotation = 0.0f;
	private float blendDuration = 0.15f;
	private float emitTime = 2.0f;
	private bool ikStatus = false;
	private int strength = 100;

	// Start is called before the first frame update
	void Start()
    {
		mecanimControl = character.GetComponent<MecanimControl>();
		legControl = gameObject.GetComponent<LegIKControl>();
		facialControl = skinnedMesh.GetComponent<FacialBlendShape>();
		charOffsetCtrl = character.GetComponent<EffectorOffset>();
		spineOffsetCtrl = character.GetComponent<SpineOffset>();
	}

	void OnGUI()
	{
		if (Interface)
		{	
			// mecanim control
			mecanimGUI();

			// ik lock
			if (GUI.Button(new Rect(500f, 20f, 100f, 20f), "Foot IK")){
				footLock(ikStatus);
				ikStatus = !ikStatus;
			}

			// facial expression preset
			strength = (int)GUI.HorizontalSlider(new Rect(800, 0, 100, 20), strength, 0f, 100f);

			if (GUI.Button(new Rect(800, 20, 100, 20), "Bored"))
				setFacialExpression("bored");
			if (GUI.Button(new Rect(800, 40, 100, 20), "Happy"))
				setFacialExpression("happy", strength);
			if (GUI.Button(new Rect(800, 60, 100, 20), "Angry"))
				setFacialExpression("angry");
			if (GUI.Button(new Rect(800, 80, 100, 20), "Content"))
				setFacialExpression("content");

			// hand shape
			int max = 3;
			Array handPose = Enum.GetValues(typeof(HandLayerControl.HandPose));
			for (int i = 0; i < max; i++)
			{
				if (GUI.Button(new Rect(250, 20 + 40 * i, 100, 20), handPose.GetValue(Convert.ToInt32(i)).ToString()))
					setHandShape("L", handPose.GetValue(Convert.ToInt32(i)).ToString());
				if (GUI.Button(new Rect(350, 20 + 40 * i, 100, 20), handPose.GetValue(Convert.ToInt32(i)).ToString()))
					setHandShape("R", handPose.GetValue(Convert.ToInt32(i)).ToString());
			}
		}
	}

	// avoid close transitioning
	IEnumerator emitor()
	{
		emitorFlag = true;
		yield return new WaitForSeconds(emitTime);
		emitorFlag = false;
	}

	// add offset transitioning between poses
	IEnumerator leftHandOffset(float nextValue, int frames = 20)
	{
		float previousVal = charOffsetCtrl.leftHandOffset[0];
		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			float currentVal = Mathf.Lerp(previousVal, nextValue, (float)i / (float)frames);
			charOffsetCtrl.leftHandOffset.Set(currentVal, 0.0f, 0.0f);
		}
	}

	IEnumerator spineOffset(float nextValue, int frames = 20)
	{
		float previousVal = spineOffsetCtrl.offset;
		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			float currentVal = Mathf.Lerp(previousVal, nextValue, (float)i / (float)frames);
			spineOffsetCtrl.offset = currentVal;
		}
	}

	public void changePose(int poseIndex, float speed=1.0f, float blend=0.15f)
	{
		if (!emitorFlag) { 
			// apply offset
			if(poseIndex == 2 && Offset)
			{
				StartCoroutine(leftHandOffset(-0.1f));
				StartCoroutine(spineOffset(10f));
			}
			else if(Offset)
			{
				StartCoroutine(leftHandOffset(0.0f));
				StartCoroutine(spineOffset(0f));
			}

			mecanimControl.Play(mecanimControl.animations[poseIndex], blend);
			mecanimControl.SetSpeed(speed);
			StartCoroutine(emitor());  // avoid frequent transition
		}
	}

	public void randomBeat()
	{
		//int[] beatList = { 8, 9, 10, 11, 12, 13, 14, 15, 22, 23, 25, 27, 28, 29, 30, 43};  Luna
		int[] beatList = { 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 21, 22, 23, 24, 25, 26, 27};
		System.Random rnd = new System.Random();
		int beatIndex = rnd.Next(0, beatList.Length);
		changePose(beatList[beatIndex], 0.9f, 0.1f);
	}

	public void randomIdle() {
		//int[] idleList = { 40 };  Luna
		int[] idleList = { 36 };
		System.Random rnd = new System.Random();
		int idleIndex = rnd.Next(0, idleList.Length);
		changePose(idleList[idleIndex], 0.9f, 0.15f);
	}

	public void footLock(bool status) {
		if (status)
			legControl.footLock();
		else
			legControl.footUnlock();
	}

	public void setFacialExpression(string emotion, int strength=100) {
		Debug.Assert(strength >= 0 && strength <= 100, "strength should be in range 0 to 100");
		if (emotion == "bored")
			facialControl.setBored(strength);
		else if (emotion == "angry")
			facialControl.setAngry(strength);
		else if (emotion == "happy")
			facialControl.setHappy(strength);
		else if (emotion == "content")
			facialControl.setContent(strength);
	}

	public void raiseBrow()
	{
		StartCoroutine(facialControl.browRaise());
	}

	public void setHandShape(string side, string shape) {
		if (side == "L") {
			if (shape == "Relax")
				HandLayerControl.setLeftHand(0);
			else if (shape == "Fist")
				HandLayerControl.setLeftHand(2);
			else if (shape == "Palm")
				HandLayerControl.setLeftHand(1);
		}
		else if (side == "R")
		{
			if (shape == "Relax")
				HandLayerControl.setRightHand(0);
			else if (shape == "Fist")
				HandLayerControl.setRightHand(2);
			else if (shape == "Palm")
				HandLayerControl.setRightHand(1);
		}
	}

	private void mecanimGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.Label("Speed (" + animSpeed.ToString("0.00") + ")");
		animSpeed = GUILayout.HorizontalSlider(animSpeed, 0.0f, 2.0f);
		mecanimControl.SetSpeed(animSpeed);

		GUILayout.Label("Rotation (" + currentRotation.ToString("000") + ")");
		currentRotation = GUILayout.HorizontalSlider(currentRotation, 0.0f, 360.0f);
		character.transform.localEulerAngles = new Vector3(0.0f, currentRotation, 0.0f);

		GUILayout.Label("Blending (" + blendDuration.ToString("0.00") + ")");
		blendDuration = GUILayout.HorizontalSlider(blendDuration, 0.0f, 1.0f);
		mecanimControl.defaultTransitionDuration = blendDuration;

		GUILayout.Space(10);

		if (GUILayout.Button("Mirror " + mirror))
		{
			mirror = !mirror;
			mecanimControl.SetMirror(mirror);
		}

		if (GUILayout.Button("Debug " + mecanimControl.debugMode))
			mecanimControl.debugMode = !mecanimControl.debugMode;

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();

		int index = 0;
		foreach (AnimationData animationData in mecanimControl.animations)
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fixedHeight = 20;
			style.fixedWidth = 40;
			if (GUILayout.Button(animationData.clipName, style))
			{
				if (index < mecanimControl.animations.Length)
					changePose(index);

				else print("index out of range");
			}
			index++;
			GUILayout.Space(5);
			if (index % 10 == 0)
			{
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
			}
		}

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
}
