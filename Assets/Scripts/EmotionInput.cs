using UnityEngine;
using UnityEngine.Audio;

public class EmotionInput : MonoBehaviour
{
	private MasterControl currentEvent;
	private AudioMixerGroup pitchShifter;

	public static string lastEmotion = "None";
	public static string currentEmotion = "None";
	public static bool reset = true;

	private void Start()
	{
		currentEvent = gameObject.GetComponent<MasterControl>();
		pitchShifter = Resources.Load<AudioMixerGroup>("VoiceMixer");
	}

	void Update()
	{
		if (currentEmotion != lastEmotion && reset)
		{
			print("Transitioning to: " + currentEmotion);
			currentEvent.MirrorEmotion(currentEmotion);
			reset = false;
		}
	}
}
