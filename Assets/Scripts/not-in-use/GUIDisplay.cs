using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIDisplay : MonoBehaviour
{
	private MecanimControl mecanimControl;
	private LegIKControl legIkControl;
	private bool mirror;
	private float animSpeed = 1f;
	private float currentRotation = 0.0f;
	private float blendDuration = 0.15f;

	void Start()
	{
		mecanimControl = gameObject.GetComponent<MecanimControl>();
	}

	void OnGUI()
	{
		GUILayout.BeginVertical();

		GUILayout.Label("Speed (" + animSpeed.ToString("0.00") + ")");
		animSpeed = GUILayout.HorizontalSlider(animSpeed, 0.0f, 2.0f);
		setClipSpeed(animSpeed);

		GUILayout.Label("Rotation (" + currentRotation.ToString("000") + ")");
		currentRotation = GUILayout.HorizontalSlider(currentRotation, 0.0f, 360.0f);
		transform.localEulerAngles = new Vector3(0.0f, currentRotation, 0.0f);

		GUILayout.Label("Blending (" + blendDuration.ToString("0.00") + ")");
		blendDuration = GUILayout.HorizontalSlider(blendDuration, 0.0f, 1.0f);
		setBlendTime(blendDuration);

		GUILayout.Space(10);

		if (GUILayout.Button("Mirror " + mirror))
		{
			mirror = !mirror;
			mecanimControl.SetMirror(mirror);
		}

		if (GUILayout.Button("Debug " + mecanimControl.debugMode))
		{
			mecanimControl.debugMode = !mecanimControl.debugMode;
		}

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		int count = 0;
		foreach (AnimationData animationData in mecanimControl.animations)
		{
			GUIStyle style = new GUIStyle(GUI.skin.button);
			style.fixedHeight = 20;
			style.fixedWidth = 40;
			if (GUILayout.Button(animationData.clipName, style))
			{
				//mecanimControl.Play(animationData, mirror);
				setCurrentClip(count);
			}
			count++;
			GUILayout.Space(5);
			if (count % 10 == 0) {
				GUILayout.EndVertical();
				GUILayout.BeginVertical();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}

	public void setClipSpeed(float speed) {
		speed = Mathf.Clamp(speed, 0.0f, 2.0f);
		mecanimControl.SetSpeed(speed);
	}

	public void setBlendTime(float time) {
		time = Mathf.Clamp(time, 0.05f, 0.5f);
		mecanimControl.defaultTransitionDuration = time;
	}

	public void setCurrentClip(int index, bool mirror = false) {
		if (index < mecanimControl.animations.Length)
			mecanimControl.Play(mecanimControl.animations[index], mirror);
		else print("index out of range");
	}
}
