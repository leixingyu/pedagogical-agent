using System.Collections.Generic;

public class Global
{
	/* ------------SCENE HIERARCHY-------------------- */
	
	// character object name
	public const string luna  = "Luna_defined";
	public const string david = "David_defined";

	// character mesh name
	public const string lunaMesh  = "Mesh_Luna_Full";
	public const string davidMesh = "base";

	// spine joint for controlling offset 
	public static List<string> lunaSpineObj  = new List<string>{
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt",
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt/Spine_3_Jnt"
	};
	public static List<string> davidSpineObj = new List<string>{
		"Root_M/RootPart1_M/Spine1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M/Spine2_M"
	};

	// slide object for changing texture
	public static string slideObj = "Environment/Board/Slides/SlideBoard";

	/* -------------PROJECT HIERARCHY----------------- */

	// character animation clips location
	public static string lunaAnim = "MotionLibrary/Luna";
	public static string davidAnim = "MotionLibrary/David";

	// character controller location
	public static string lunaController  = "Controller/Luna_controller";
	public static string davidController = "Controller/David_controller";

	// slide texture location
	public static string slideTexture = "Slide";

	// subsequence audio location
	public static string lunaAudio = "Audio/Luna";
	public static string davidAudio = "Audio/David";

	// audio mixer
	public static string mixer = "VoiceMixer";

	// animator -> layer name
	public static string leftHandLayer  = "LHand";
	public static string rightHandLayer = "RHand";

	/* -------------OTHER----------------------------- */
	public enum BodyOffset
	{
		NEUTRAL,
		FORWARD,
		BACKWARD,
		OUTWARD,
		INWARD,
	}

	public enum HandPose
	{
		RELAX,
		PALM,
		FIST,
		MAX,
	}

	public enum Emotion
	{
		ANGRY,
		BORED,
		CONTENT,
		HAPPY,
	}

	public enum Direction
	{
		NEXT,
		BACK,
	}

	public enum Side
	{
		LEFT,
		RIGHT,
		BOTH,
	}

	public enum CameraView
	{
		CLOSE = 1,
		MID = 2,
		FULL = 3,
	}

	// identified default gesture index
	public static int davidDefaultGesture = 36;
	public static int lunaDefaultGesture  = 40;

	// identified beat gesture index
	public static int[] davidBeatGestures = { 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 21, 22, 23, 24, 25, 26, 27};
	public static int[] lunaBeatGestures  = { 8, 9, 10, 11, 12, 13, 14, 15, 22, 23, 25, 27, 28, 29, 30, 43};

	// identified idle gesture index
	public static int[] davidIdleGestures = { 36 };
	public static int[] lunaIdleGestures  = { 40 };

	// identified angry gesture index
	public static int[] davidAngryGestures = { 29 };
	public static int[] lunaAngryGestures  = { 46 };

	// identified bored gesture index
	public static int[] davidBoredGestures = { 37 };
	public static int[] lunaBoredGestures  = { 37 };

	// identified content gesture index
	public static int[] davidContentGestures = { 33 };
	public static int[] lunaContentGestures  = { 47 };

	// identified happy index - same from default idle
	public static int[] davidHappyGestures = { 36 };
	public static int[] lunaHappyGestures  = { 40 };
}

public class Setting
{
	public const int bodyOffsetBlend = 100;
	public const float bodyLeanExtreme = 20.0f; // the maximum angle for body lean

	public const float emitTime = 2.0f;  // least amount of time for next pose transition

	public const int handShape = 3;  // current number of hand shape configured

	public const int facialBlend = 12;  // frames to blend to next facial expression
	public const int browBlend = 6;		// frames to raise eye brows
}
