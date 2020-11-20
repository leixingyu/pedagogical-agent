using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasManage : MonoBehaviour
{
	MasterControl currentEvent;
	AudioSource audioSource;

	public Camera mainCam;
	public Slider mainSlider;
	public Button lunaBtn;
	public Button davidBtn;
	Button debugBtn, nextBtn, preBtn, reloadBtn, pausePlayBtn;
	Toggle XMLTog, BeatTog, EmotionTog;

	Vector3 closeView = new Vector3(-0.149f, 0.96f, 0.813f);
	Vector3 midView = new Vector3(-0.31f, 0.75f, 1.97f);
	Vector3 fullView = new Vector3(-0.31f, 0.584f, 2.804f);

	bool pause = false;

	void Start()
	{
		currentEvent = gameObject.GetComponent<MasterControl>();
		audioSource = gameObject.GetComponent<AudioSource>();

		mainCam.transform.position = midView;
		mainSlider.value = (float)Global.CameraView.MID;
		mainSlider.onValueChanged.AddListener(delegate { CameraChange(); });

		lunaBtn.onClick.AddListener(delegate { SwitchToLuna(); });
		davidBtn.onClick.AddListener(delegate { SwitchToDavid(); });

		preBtn = GameObject.Find("Canvas/Previous").GetComponent<Button>();
		preBtn.onClick.AddListener(delegate { Previous(); });

		nextBtn = GameObject.Find("Canvas/Next").GetComponent<Button>();
		nextBtn.onClick.AddListener(delegate { Next(); });

		debugBtn = GameObject.Find("Canvas/Debug").GetComponent<Button>();
		debugBtn.onClick.AddListener(delegate { ToggleDebugLog(); });

		reloadBtn = GameObject.Find("Canvas/Reload").GetComponent<Button>();
		reloadBtn.onClick.AddListener(delegate { ReloadScene(); });

		pausePlayBtn = GameObject.Find("Canvas/PausePlay").GetComponent<Button>();
		pausePlayBtn.onClick.AddListener(delegate { PausePlayModifier(); });

		XMLTog = GameObject.Find("Canvas/XMLReader").GetComponent<Toggle>();
		XMLTog.onValueChanged.AddListener(delegate { ToggleXMLReader(); });

		BeatTog = GameObject.Find("Canvas/BeatDetect").GetComponent<Toggle>();
		BeatTog.onValueChanged.AddListener(delegate { ToggleBeatDect(); });

		EmotionTog = GameObject.Find("Canvas/EmotionInput").GetComponent<Toggle>();
		EmotionTog.onValueChanged.AddListener(delegate { ToggleEmotionRec(); });
	}
 
	void CameraChange()
	{
		switch ((int)mainSlider.value)
		{
			case (int)Global.CameraView.CLOSE:
				mainCam.transform.position = closeView;
				break;
			case (int)Global.CameraView.MID:
				mainCam.transform.position = midView;
				break;
			case (int)Global.CameraView.FULL:
				mainCam.transform.position = fullView;
				break;
		}
	}

	void SwitchToDavid() {
		SceneManager.LoadScene("David");
	}

	void SwitchToLuna() {
		SceneManager.LoadScene("Luna");
	}

	void ToggleDebugLog()
	{
		currentEvent.RevertDebug();
	}

	void Next()
	{
		currentEvent.ChangeSlide(Global.Direction.NEXT);
		currentEvent.ChangeAudio(Global.Direction.NEXT);
	}

	void Previous()
	{
		currentEvent.ChangeSlide(Global.Direction.BACK);
		currentEvent.ChangeAudio(Global.Direction.BACK);
	}

	void ReloadScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}

	void PausePlayModifier()
	{
		if (pause)
		{
			Time.timeScale = 1;
			audioSource.Play();
		}

		else
		{
			Time.timeScale = 0;
			audioSource.Pause();
		}
			
		pause = !pause;
	}

	void ToggleXMLReader()
	{
		XMLReader obj = GetComponent<XMLReader>();
		if (XMLTog.isOn)
		{
			obj.enabled = true;
		}
		else
			obj.enabled = false;
	}

	void ToggleBeatDect()
	{
		BeatDetection obj = GetComponent<BeatDetection>();
		if (BeatTog.isOn)
		{
			obj.enabled = true;
		}
		else
			obj.enabled = false;
	}

	void ToggleEmotionRec()
	{
		EmotionInput obj = GetComponent<EmotionInput>();
		if (EmotionTog.isOn)
		{
			obj.enabled = true;
		}
		else
			obj.enabled = false;
	}
}
