using UnityEngine;
using System;

/* Optional */
// access class to the BeatDetection class for trigging behavior

public class BeatListener : MonoBehaviour
{
	void Start ()
	{
		BeatDetection processor = FindObjectOfType<BeatDetection>();
		processor.onBeat.AddListener (onOnbeatDetected);
	}

	//this event will be called every time a beat is detected.
	void onOnbeatDetected ()
	{
		Debug.Log ("Beat Detected");
	}
}
