using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Switch character and adjust Camera Position
 * works standalone */

public class SceneAndCameraManager : MonoBehaviour
{
	public Camera mainCam;
	public Slider mainSlider;
	public Button lunaBtn;
	public Button davidBtn;

	Vector3 closeView = new Vector3(-0.149f, 0.96f, 0.813f);
	Vector3 midView = new Vector3(-0.31f, 0.75f, 1.97f);
	Vector3 fullView = new Vector3(-0.31f, 0.584f, 2.804f);

	enum CAMVIEW
	{
		CLOSE = 1,
		MID = 2,
		FULL = 3,
	}

	void Start()
	{
		mainCam.transform.position = midView;
		mainSlider.value = (float)CAMVIEW.MID;
		mainSlider.onValueChanged.AddListener(delegate { CameraChange(); });

		lunaBtn.onClick.AddListener(delegate { SwitchToLuna(); });
		davidBtn.onClick.AddListener(delegate { SwitchToDavid(); });
	}
 
	public void CameraChange()
	{
		switch ((int)mainSlider.value)
		{
			case (int)CAMVIEW.CLOSE:
				mainCam.transform.position = closeView;
				break;
			case (int)CAMVIEW.MID:
				mainCam.transform.position = midView;
				break;
			case (int)CAMVIEW.FULL:
				mainCam.transform.position = fullView;
				break;
		}
	}

	public void SwitchToDavid() {
		SceneManager.LoadScene("David");
	}

	public void SwitchToLuna() {
		SceneManager.LoadScene("Luna");
	}
}
