using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used for constant update the most frequent emotion

public class EmotionUpdate : MonoBehaviour
{
	float startTime;
	float currentTime;

	SignalRequester requester;
	Queue emotionList = new Queue();
	public static string freqEmotion = "None";

	void Start()
    {
		startTime = Time.time;

		InvokeRepeating("RequestSignal", 0.0f, Setting.timeInterval);
		for(int count=0; count < Setting.maxQueueSize; count++)
		{
			emotionList.Enqueue("None");
		}
	}

	void OnDestory()
	{
		requester.Stop();
	}

	void Update()
    {
		currentTime = Time.time - startTime;
	}

	public void RequestSignal()
	{
		requester = new SignalRequester();
		requester.message = "runtime: " + currentTime + "\nagent emotion: " + EmotionInput.currentEmotion;

		requester.Start();
		emotionList.Dequeue();
		emotionList.Enqueue(EmotionInput.activeEmotion);

		PrintValues(emotionList);
		freqEmotion = mostFrequent(emotionList.ToArray(), Setting.maxQueueSize);
	}

	public static void PrintValues(IEnumerable myCollection)
	{
		string text = "";
		foreach (System.Object obj in myCollection) { 
			text += "    " + obj;
		}
		Debug.Log(text);
	}

	static string mostFrequent(object[] arr, int n)
	{
		// Insert all elements in hash 
		Dictionary<string, int> hp = new Dictionary<string, int>();

		for (int i = 0; i < n; i++)
		{
			string key = (string)arr[i];
			if (hp.ContainsKey(key))
			{
				int freq = hp[key];
				freq++;
				hp[key] = freq;
			}
			else
				hp.Add(key, 1);
		}

		// find max frequency. 
		int min_count = 0; 
		string res = "None";

		foreach (KeyValuePair<string, int> pair in hp)
		{
			if (min_count < pair.Value)
			{
				res = pair.Key;
				min_count = pair.Value;
			}
		}
		return res;
	}

}
