using UnityEngine;

public class SlideControl : MonoBehaviour
{
	GameObject slide;
	static Renderer slideShader;
	static Object[] textureList;
	static int maxSlide;
	static int currentSlide = 0;

    void Start()
    {
		// get the slide object's material
		slide = GameObject.Find(Global.slideObj);
		if (!slide)
		{
			print("slide gameobject not found");
		}
		slideShader = slide.GetComponent<Renderer>();

		// fetch all slide imgs
		textureList = Resources.LoadAll(Global.slideTexture, typeof(Texture));
		maxSlide = textureList.Length;
    }

	void JumpToSlide(int index)
	{
		Debug.Assert(index >= 0 && index <= maxSlide - 1, "Slide index out of range");
		
		slideShader.materials[0].mainTexture = (Texture)textureList[index];
		currentSlide = index;
	}

	public static void NextSlide(int step = 1)
	{
		Debug.Assert((currentSlide+step) >= 0 && (currentSlide+step) <= maxSlide - 1, "Slide index out of range");
		slideShader.materials[0].mainTexture = (Texture)textureList[currentSlide + step];
		currentSlide += step;
	}

	public static void PreviousSlide(int step = 1)
	{
		Debug.Assert((currentSlide-step) >= 0 && (currentSlide-step) <= maxSlide - 1, "Slide index out of range");
		slideShader.materials[0].mainTexture = (Texture)textureList[currentSlide - step];
		currentSlide -= step;
	}
}
