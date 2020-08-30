using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchCharacter : MonoBehaviour
{
	public void SwitchToDavid() {
		SceneManager.LoadScene("David");
	}

	public void SwitchToLuna() {
		SceneManager.LoadScene("Luna");
	}

	public void SwitchToDana()
	{
		SceneManager.LoadScene("Dana");
	}
}
