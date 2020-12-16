using UnityEngine;

public class AudioControl : MonoBehaviour
{
	static Object[] audioList;
	static AudioSource audioPlayer;
	static int maxAudio;
	static int currentAudio;
	GameObject character;

	void Start()
    {
		character = GameObject.FindGameObjectWithTag("Player");
		audioPlayer = gameObject.GetComponent<AudioSource>();
		currentAudio = 0;

		if (character.name == Global.luna)
		{
			audioList = Resources.LoadAll(Global.lunaAudio, typeof(AudioClip));
		}
		else if(character.name == Global.david)
		{
			audioList = Resources.LoadAll(Global.davidAudio, typeof(AudioClip));
		}
		audioPlayer.clip = (AudioClip)audioList[0];
		maxAudio = audioList.Length;
		audioPlayer.Play();
	}

	void JumpToAudio(int index)
	{
		Debug.Assert(index >= 0 && index <= maxAudio - 1, "Audio index out of range");
		audioPlayer.clip = (AudioClip)audioList[index];
		audioPlayer.Play();
		currentAudio = index;
	}

	public static void NextAudio(int step = 1)
	{
		Debug.Assert((currentAudio + step) >= 0 && (currentAudio + step) <= maxAudio - 1, "Audio index out of range");
		audioPlayer.clip = (AudioClip)audioList[currentAudio + step];
		audioPlayer.Play();
		currentAudio += step;
	}

	public static void PreviousAudio(int step = 1)
	{
		Debug.Assert((currentAudio + step) >= 0 && (currentAudio + step) <= maxAudio - 1, "Audio index out of range");
		audioPlayer.clip = (AudioClip)audioList[currentAudio - step];
		audioPlayer.Play();
		currentAudio -= step;
	}

}
