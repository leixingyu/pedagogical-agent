using UnityEngine;

public class ViewerEmotion : MonoBehaviour
{
	public static string lastEmotion = "null";
	public static string currentEmotion = "null";

	public static bool reset = true;

	// Update is called once per frame
	void Update()
	{
		if (currentEmotion != lastEmotion && reset)
		{
			print("Transitioning to: " + currentEmotion);
			EmotionState(currentEmotion);
			reset = false;
		}
	}

	void EmotionState(string emotion)
	{
		return;
	}
}
