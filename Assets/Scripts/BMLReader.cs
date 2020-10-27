using UnityEngine;
using System.Xml;

/* Takes a BML and triggers series of event (controls)
 * Calls Global control functionality */

public class BMLReader : MonoBehaviour
{
	XmlDocument xml = new XmlDocument();
	XmlNode root;

	public MasterControl currentEvent;
	public TextAsset file;

	float startTime;
	float currentTime;
	float triggerTime;
	int action = 0;

	// initialize BML by reseting time, loading BML script
	void Start()
    {
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
			int poseIndex = int.Parse(root.ChildNodes.Item(action).Attributes["pose"].Value);

			float speed = 1.0f, blend = 0.15f;
			string type = "neutral";
			int strength = 100;

			if(root.ChildNodes.Item(action).Attributes["speed"] != null)
				speed = float.Parse(root.ChildNodes.Item(action).Attributes["speed"].Value);
			if (root.ChildNodes.Item(action).Attributes["blend"] != null)
				blend = float.Parse(root.ChildNodes.Item(action).Attributes["blend"].Value);
			currentEvent.changePose(poseIndex, speed, blend);

			if (root.ChildNodes.Item(action).Attributes["offset"] != null)
				type = root.ChildNodes.Item(action).Attributes["offset"].Value;
			if (root.ChildNodes.Item(action).Attributes["strength"] != null)
				blend = int.Parse(root.ChildNodes.Item(action).Attributes["blend"].Value);
			currentEvent.characterOffset(type, strength);
		}

		if (eventName == "facial")
		{
			int strength = 100;
			string emotion = root.ChildNodes.Item(action).Attributes["emotion"].Value;
			if(root.ChildNodes.Item(action).Attributes["strength"] != null)
				strength = int.Parse(root.ChildNodes.Item(action).Attributes["strength"].Value);
			currentEvent.setFacialExpression(emotion, strength);
		}

		if (eventName == "hand")
		{
			string side = root.ChildNodes.Item(action).Attributes["side"].Value;
			string shape = root.ChildNodes.Item(action).Attributes["shape"].Value;
			currentEvent.setHandShape(side, shape);
		}

		if (eventName == "foot")
		{
			string status = root.ChildNodes.Item(action).Attributes["status"].Value;
			if (status == "lock")
				currentEvent.footLock(true);
			else
				currentEvent.footLock(false);
		}

		if (eventName == "request")
		{
			string message = root.ChildNodes.Item(action).Attributes["message"].Value;
			currentEvent.requestSignal(message);
		}
	}
}
