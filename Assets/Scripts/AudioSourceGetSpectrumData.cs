using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceGetSpectrumData : MonoBehaviour
{
	AudioSource audioclip;
	float maxScale = 1000;

	private void Start()
	{
		audioclip = GetComponent<AudioSource>();
		print(audioclip.name);
	}

	void Update()
	{
		float[] spectrum = new float[256];

		audioclip.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

		for (int i = 1; i < spectrum.Length - 1; i++)
		{
			//Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
			//Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
			//Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
			//Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);

			Debug.DrawLine(new Vector3(i-1 + 20, spectrum[i-1]* maxScale + 200, 0), new Vector3(i + 20, spectrum[i]*maxScale + 200, 0), Color.blue);
		}
	}
}
