using UnityEngine;
using UnityEngine.Audio;

// used for triggering event when detecting emotion shift

public class EmotionInput : MonoBehaviour
{
	private MasterControl currentEvent;
	private AudioMixerGroup pitchShifter;

	public static string lastEmotion = "None";
	public static string currentEmotion = "None";
	public static string activeEmotion = "None";
	public static bool reset = true;

	private void Start()
	{
		gameObject.AddComponent<EmotionUpdate>();
		currentEvent = gameObject.GetComponent<MasterControl>();
		pitchShifter = Resources.Load<AudioMixerGroup>("VoiceMixer");
	}

	void Update()
	{
		if (currentEmotion != lastEmotion && reset && currentEmotion != "None")
		{
			print("Transitioning to: " + currentEmotion);
			currentEvent.MirrorEmotion(currentEmotion);
			reset = false;
			lastEmotion = currentEmotion;
		}
	}
}
