using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Audio;

/* Master (Main) control wrapper of character behavior: 
 *		[offset ctrl] + [mecanim pose ctrl] + [legIK ctrl] + [facial expression ctrl] + [hand shape ctrl]
 * Create GUI and allow control
 * Allow BML reader to acess all the control */

public class MasterControl : MonoBehaviour
{
	GameObject character;       // assigned character
	MainOffset mainOffsetCtrl;
	SpineOffset spineOffsetCtrl;
	MecanimControl mecanimControl;
	ExpressionControl facialControl;
	SignalRequester requester;

	System.Random rnd;
	AudioMixerGroup pitchShifter;

	int[] beatList, idleList;
	bool emitorFlag = false;
	bool ikStatus = false;
	//int strength = 100;
	bool debug = false;

	float animSpeed = 1.0f;
	float currentRotation = 0.0f;
	float blendDuration = 0.15f;

	void Start()
    {
		character = GameObject.FindGameObjectWithTag("Player");
		CharacterInitialize();
		rnd = new System.Random();
		pitchShifter = Resources.Load<AudioMixerGroup>(Global.mixer);

		// Components added during runtime
		mecanimControl = character.AddComponent<MecanimControl>();
		facialControl = character.AddComponent<ExpressionControl>();
		mainOffsetCtrl = character.AddComponent<MainOffset>();
		spineOffsetCtrl = character.AddComponent<SpineOffset>();

		// Static method can be called doesn't have StartCoroutine()
		character.AddComponent<HandControl>();
		character.AddComponent<FootControl>();
		gameObject.AddComponent<SlideControl>();
		gameObject.AddComponent<AudioControl>();
	}

	/*---------------------------------------------------------------*/

	IEnumerator Emitor()
	{
		emitorFlag = true;
		yield return new WaitForSeconds(Setting.emitTime);   // avoid rapid transitioning
		emitorFlag = false;
	}

	private void CharacterInitialize()
	{
		switch (character.name)
		{
			case Global.david:
				beatList = Global.davidBeatGestures;
				idleList = Global.davidIdleGestures;
				break;
			case Global.luna:
				beatList = Global.lunaBeatGestures;
				idleList = Global.lunaIdleGestures;
				break;
			default:
				beatList = null;
				idleList = null;
				break;
		}
	}

	public void ChangeIdle(Global.Emotion emotion)
	{
		switch (character.name)
		{
			case Global.david:
				if (emotion == Global.Emotion.ANGRY)		idleList = Global.davidAngryGestures;
				else if (emotion == Global.Emotion.BORED)   idleList = Global.davidBoredGestures;
				else if (emotion == Global.Emotion.CONTENT) idleList = Global.davidContentGestures;
				else if (emotion == Global.Emotion.HAPPY)   idleList = Global.davidHappyGestures;
				break;
			case Global.luna:
				if (emotion == Global.Emotion.ANGRY)		idleList = Global.lunaAngryGestures;
				else if (emotion == Global.Emotion.BORED)   idleList = Global.lunaBoredGestures;
				else if (emotion == Global.Emotion.CONTENT) idleList = Global.lunaContentGestures;
				else if (emotion == Global.Emotion.HAPPY)   idleList = Global.lunaHappyGestures;
				break;
			default:
				idleList = null;
				break;
		}
	}

	public void RandomBeat()
	{
		int beatIndex = rnd.Next(0, beatList.Length);
		ChangePose(beatList[beatIndex], 0.9f, 0.1f);
	}

	public void RandomIdle()
	{
		int idleIndex = rnd.Next(0, idleList.Length);
		ChangePose(idleList[idleIndex], 0.9f, 0.15f);
	}

	public void RaiseBrow()
	{
		StartCoroutine(facialControl.BrowRaise());
	}

	/*---------------------------------------------------------------*/

	public void ChangePose(int poseIndex, float speed = 1.0f, float blend = 0.15f)
	{
		if (!emitorFlag)
		{
			mecanimControl.Play(mecanimControl.animations[poseIndex], blend);
			mecanimControl.SetSpeed(speed);
			StartCoroutine(Emitor());  // avoid frequent transition
		}
	}

	public void LockFoot(bool status) {
		if (status)
			FootControl.LockFoot();
		else
			FootControl.UnlockFoot();
	}

	public void SetExpression(Global.Emotion type, int strength = 100) {
		Debug.Assert(strength >= 0 && strength <= 100, "strength should be in range 0 to 100");
		if (type == Global.Emotion.ANGRY)
			facialControl.SetAngry(strength);
		else if (type == Global.Emotion.BORED)
			facialControl.SetBored(strength);
		else if (type == Global.Emotion.CONTENT)
			facialControl.SetContent(strength);
		else if (type == Global.Emotion.HAPPY)
			facialControl.SetHappy(strength);
	}

	public void SetHandShape(Global.Side side, Global.HandPose handPose) {
		if (side == Global.Side.LEFT)
			HandControl.SetLeftHand((int)handPose);
		else if (side == Global.Side.RIGHT)
			HandControl.SetRightHand((int)handPose);
	}

	public void CharacterOffset(Global.BodyOffset type, int strength) {
		StartCoroutine(spineOffsetCtrl.OffsetSpine(type, strength));
		StartCoroutine(mainOffsetCtrl.OffsetMain(type, strength));
	}

	/*---------------------------------------------------------------*/

	public void RequestSignal(string message)
	{
		EmotionInput.lastEmotion = EmotionInput.currentEmotion;
		EmotionInput.reset = true;
		requester = new SignalRequester();
		requester.message = message;
		requester.Start();
	}

	void OnDestory()
	{
		requester.Stop();
	}

	/*---------------------------------------------------------------*/

	public void ChangeSlide(Global.Direction direction, int step = 1)
	{
		if (direction == Global.Direction.NEXT)
			SlideControl.NextSlide(step);
		else if(direction == Global.Direction.BACK)
			SlideControl.PreviousSlide(step);
	}

	public void ChangeAudio(Global.Direction direction, int step = 1)
	{
		if (direction == Global.Direction.NEXT)
			AudioControl.NextAudio(step);
		else if (direction == Global.Direction.BACK)
			AudioControl.PreviousAudio(step);
	}

	/*---------------------------------------------------------------*/

	public void MirrorEmotion(string emotion)
	{
		if (emotion == "u-a")
		{
			SetExpression(Global.Emotion.ANGRY);

			ChangeIdle(Global.Emotion.ANGRY);
			RandomIdle();
			pitchShifter.audioMixer.SetFloat("pitchblend", 0.9f);
		}
		else if (emotion == "u-i")
		{
			SetExpression(Global.Emotion.BORED);

			ChangeIdle(Global.Emotion.BORED);
			RandomIdle();
			pitchShifter.audioMixer.SetFloat("pitchblend", 0.94f);
		}
		else if (emotion == "p-i")
		{
			SetExpression(Global.Emotion.CONTENT);

			ChangeIdle(Global.Emotion.CONTENT);
			RandomIdle();
		}
		else if (emotion == "p-a")
		{
			SetExpression(Global.Emotion.HAPPY);

			ChangeIdle(Global.Emotion.HAPPY);
			RandomIdle();
			pitchShifter.audioMixer.SetFloat("pitchblend", 1.03f);
		}
		else
		{
			print("No Emotion/Face Detected");
		}
	}

	public void RevertDebug()
	{
		debug = !debug;
	}

	void OnGUI()
	{
		float width = 100.0f, height = 20.0f, hGap = 20.0f, vGap = 10.0f;
		int row = 10, column = 5;

		if (debug)
		{
			int clipCount = mecanimControl.animations.GetLength(0);

			GUIStyle myStyle = new GUIStyle("box");
			myStyle.fontSize = 14;

			GUI.Box(new Rect(0, 0, column * width + (column + 1) * hGap, Screen.height), "", myStyle);

			// Facial Expression
			//strength = (int)GUI.HorizontalSlider(new Rect(800, 0, 100, 20), strength, 0f, 100f);

			if (GUI.Button(new Rect(hGap, height + vGap, width, 20), "Angry"))
			{
				//SetExpression("angry");
				MirrorEmotion("u-a");
			}

			if (GUI.Button(new Rect(hGap, 2 * (height + vGap), width, 20), "Bored"))
			{
				//SetExpression("bored");
				MirrorEmotion("u-i");
			}

			if (GUI.Button(new Rect(hGap, 3 * (height + vGap), width, 20), "Content"))
			{
				//SetExpression("content");
				MirrorEmotion("p-i");
			}

			if (GUI.Button(new Rect(hGap, 4 * (height + vGap), width, 20), "Happy"))
			{
				//SetExpression("happy", strength);
				MirrorEmotion("p-a");
			}


			// Hand shape
			Array handPose = Enum.GetValues(typeof(Global.HandPose));
			for (int i = 0; i < (int)Global.HandPose.MAX; i++)
			{
				if (GUI.Button(new Rect(hGap + (width + hGap) * 2, (i + 1) * (height + vGap), width, 20), handPose.GetValue(Convert.ToInt32(i)).ToString()))
					SetHandShape(Global.Side.LEFT, (Global.HandPose)i);
				if (GUI.Button(new Rect(hGap + (width + hGap) * 3, (i + 1) * (height + vGap), width, 20), handPose.GetValue(Convert.ToInt32(i)).ToString()))
					SetHandShape(Global.Side.RIGHT, (Global.HandPose)i);
			}

			// IK foot lock
			if (GUI.Button(new Rect(hGap + width + hGap, height + vGap, 100f, 20f), "Foot IK"))
			{
				LockFoot(ikStatus);
				ikStatus = !ikStatus;
			}

			// MECANIM
			GUI.Label(new Rect(hGap, 5 * (height + vGap), 100, height), "Speed (" + animSpeed.ToString("0.00") + ")");
			animSpeed = GUI.HorizontalSlider(new Rect(hGap + 100, 5 * (height + vGap), 200, height), animSpeed, 0.0f, 2.0f);
			mecanimControl.SetSpeed(animSpeed);

			GUI.Label(new Rect(hGap, 6 * (height + vGap), 100, height), "Rotation (" + currentRotation.ToString("0.00") + ")");
			currentRotation = GUI.HorizontalSlider(new Rect(hGap + 100, 6 * (height + vGap), 200, height), currentRotation, 0.0f, 360.0f);
			character.transform.localEulerAngles = new Vector3(0.0f, currentRotation, 0.0f);

			GUI.Label(new Rect(hGap, 7 * (height + vGap), 100, height), "Blending (" + blendDuration.ToString("0.00") + ")");
			blendDuration = GUI.HorizontalSlider(new Rect(hGap + 100, 7 * (height + vGap), 200, height), blendDuration, 0.05f, 0.2f);
			mecanimControl.defaultTransitionDuration = blendDuration;

			for (int i = 0; i < clipCount; i++)
			{
				if (GUI.Button(new Rect(i / row * (width + hGap) + hGap, (i % row + 8) * (height + vGap), width, height), mecanimControl.animations[i].clipName))
					mecanimControl.Play(mecanimControl.animations[i], false);
			}
		}
	}
}
