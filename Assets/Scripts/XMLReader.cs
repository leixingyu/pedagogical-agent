using UnityEngine;
using System.Xml;

/* Takes a BML and triggers series of event (controls)
 * Calls Global control functionality */

public class XMLReader : MonoBehaviour
{
	XmlDocument xml = new XmlDocument();
	XmlNode root;

	private MasterControl currentEvent;
	public TextAsset file;

	float startTime;
	float currentTime;
	float triggerTime;
	int action = 0;

	// initialize BML by reseting time, loading BML script
	void Start()
    {
		currentEvent = gameObject.GetComponent<MasterControl>();
		startTime = Time.time;
		xml.LoadXml(file.text);
		root = xml.FirstChild;
	}

    // BML event callback
    void Update()
    {
		currentTime = Time.time - startTime;

		if(action < root.ChildNodes.Count) { 
			triggerTime = float.Parse(root.ChildNodes.Item(action).Attributes["start"].Value);
			if (triggerTime < currentTime)
			{
				string currentTag = root.ChildNodes.Item(action).Name;
				TriggerEvent(currentTag);
				action++;
			}
		}	
    }

	// Trigger event based on event name and event attribute+value
	private void TriggerEvent(string eventName)
	{
		if (eventName == "gesture") {
			float speed = 1.0f, blend = 0.15f;
			int strength = 100;
			string type = "neutral";

			// Required
			int poseIndex = int.Parse(root.ChildNodes.Item(action).Attributes["pose"].Value);
			// Optional
			if(root.ChildNodes.Item(action).Attributes["speed"] != null)
				speed = float.Parse(root.ChildNodes.Item(action).Attributes["speed"].Value);
			if (root.ChildNodes.Item(action).Attributes["blend"] != null)
				blend = float.Parse(root.ChildNodes.Item(action).Attributes["blend"].Value);
			if (root.ChildNodes.Item(action).Attributes["offset"] != null)
				type = root.ChildNodes.Item(action).Attributes["offset"].Value;
			if (root.ChildNodes.Item(action).Attributes["strength"] != null)
				strength = int.Parse(root.ChildNodes.Item(action).Attributes["strength"].Value);

			//
			Global.BodyOffset offsetType = Global.BodyOffset.NEUTRAL;
			if (type == "forward")			offsetType = Global.BodyOffset.FORWARD;
			else if (type == "backward")	offsetType = Global.BodyOffset.BACKWARD;
			else if (type == "inward")		offsetType = Global.BodyOffset.INWARD;
			else if (type == "outward")		offsetType = Global.BodyOffset.OUTWARD;

			currentEvent.ChangePose(poseIndex, speed, blend);
			currentEvent.CharacterOffset(offsetType, strength);
		}

		if (eventName == "facial")
		{
			int strength = 100;

			// Required
			string emotion = root.ChildNodes.Item(action).Attributes["emotion"].Value;
			// Optional
			if(root.ChildNodes.Item(action).Attributes["strength"] != null)
				strength = int.Parse(root.ChildNodes.Item(action).Attributes["strength"].Value);

			//
			Global.Emotion emotionType = Global.Emotion.CONTENT;
			if (emotion == "angry")
				emotionType = Global.Emotion.ANGRY;
			else if (emotion == "bored")
				emotionType = Global.Emotion.BORED;
			else if (emotion == "content")
				emotionType = Global.Emotion.CONTENT;
			else if (emotion == "happy")
				emotionType = Global.Emotion.HAPPY;

			currentEvent.SetExpression(emotionType, strength);
		}

		if (eventName == "hand")
		{
			// Required
			string side = root.ChildNodes.Item(action).Attributes["side"].Value;
			string shape = root.ChildNodes.Item(action).Attributes["shape"].Value;

			//
			Global.Side sideType = Global.Side.BOTH;
			if (side == "L")
				sideType = Global.Side.LEFT;
			else if (side == "R")
				sideType = Global.Side.RIGHT;

			Global.HandPose handType = Global.HandPose.RELAX;
			if (shape == "relax")
				handType = Global.HandPose.RELAX;
			else if (shape == "palm")
				handType = Global.HandPose.PALM;
			else if (shape == "fist")
				handType = Global.HandPose.FIST;

			currentEvent.SetHandShape(sideType, handType);
		}

		if (eventName == "foot")
		{
			// Required
			string status = root.ChildNodes.Item(action).Attributes["status"].Value;
			//
			if (status == "lock")
				currentEvent.LockFoot(true);
			else
				currentEvent.LockFoot(false);
		}

		if (eventName == "request")
		{
			string message = "";
			// Optional
			if (root.ChildNodes.Item(action).Attributes["message"] != null)
				message = root.ChildNodes.Item(action).Attributes["message"].Value;

			currentEvent.RequestSignal(message);
		}

		if (eventName == "slide")
		{
			int step = 1;
			// Required
			string direction = root.ChildNodes.Item(action).Attributes["direction"].Value;
			// Optional
			if (root.ChildNodes.Item(action).Attributes["step"] != null)
				step = int.Parse(root.ChildNodes.Item(action).Attributes["step"].Value);

			//
			Global.Direction directionType = Global.Direction.NEXT;
			if (direction == "back")
				directionType = Global.Direction.BACK;

			currentEvent.ChangeSlide(directionType, step);
			currentEvent.ChangeAudio(directionType, step);
		}
	}
}
