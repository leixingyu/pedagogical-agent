using UnityEngine;

public class ViewerEmotion : MonoBehaviour
{
	private MasterControl currentEvent;

	public static string lastEmotion = "null";
	public static string currentEmotion = "null";

	public static bool reset = true;

	// Update is called once per frame
	void Update()
	{
		currentEvent = gameObject.GetComponent<MasterControl>();
		if (currentEmotion != lastEmotion && reset)
		{
			print("Transitioning to: " + currentEmotion);
			MirrorEmotion(currentEmotion);
			reset = false;
		}
	}

	void MirrorEmotion(string emotion)
	{
		if (emotion == "Angry")
		{
			currentEvent.setFacialExpression("Angry");
		}
		else if (emotion == "Bored")
		{
			currentEvent.setFacialExpression("Bored");
		}
		else if (emotion == "Content")
		{
			currentEvent.setFacialExpression("Content");
		}
		else if (emotion == "Happy")
		{
			currentEvent.setFacialExpression("Happy");
		}
	}
}
