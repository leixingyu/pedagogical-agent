using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Attach this to the camera slider */

public class CameraPosition : MonoBehaviour
{
	private Vector3 closeView = new Vector3(-0.149f, 0.96f, 0.813f);
	private Vector3 midView = new Vector3(-0.31f, 0.75f, 1.97f);
	private Vector3 fullView = new Vector3(-0.31f, 0.584f, 2.804f);

	public Camera mainCam;
	Slider mainSlider;

	enum CAMVIEW {
		CLOSE = 1,
		MID   = 2,
		FULL  = 3,
	}

	// Start is called before the first frame update
	void Start()
	{
		mainCam.transform.position = midView;
		mainSlider = GetComponent<Slider>();
		mainSlider.onValueChanged.AddListener(delegate { CameraChange(); });
		mainSlider.value = (float)CAMVIEW.MID;
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
}
