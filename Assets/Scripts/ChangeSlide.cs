using UnityEngine;

public class ChangeSlide : MonoBehaviour
{
	GameObject slide;
	Renderer slideShader;
	Object[] textureList;
	int maxSlide;
	int currentSlide = 0;

    void Start()
    {
		// get the slide object's material
		slide = GameObject.Find(Global.Slide);
		if (!slide)
		{
			print("slide gameobject not found");
		}
		slideShader = slide.GetComponent<Renderer>();

		// fetch all slide imgs
		textureList = Resources.LoadAll(Global.SlideTexture, typeof(Texture));
		maxSlide = textureList.Length;
    }

	void JumpToSlide(int index)
	{
		Debug.Assert(index >= 0 && index <= maxSlide - 1, "Slide index out of range");
		
		slideShader.materials[0].mainTexture = (Texture)textureList[index];
		currentSlide = index;
		
	}

	public void NextSlide(int step = 1)
	{
		Debug.Assert((currentSlide+step) >= 0 && (currentSlide+step) <= maxSlide - 1, "Slide index out of range");
		slideShader.materials[0].mainTexture = (Texture)textureList[currentSlide + step];
		currentSlide += step;
	}

	public void PreviousSlide(int step = 1)
	{
		Debug.Assert((currentSlide-step) >= 0 && (currentSlide-step) <= maxSlide - 1, "Slide index out of range");
		slideShader.materials[0].mainTexture = (Texture)textureList[currentSlide - step];
		currentSlide -= step;
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown(KeyCode.A))
		{
			NextSlide();
		}
    }
}
