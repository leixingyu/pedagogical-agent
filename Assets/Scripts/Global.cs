using System.Collections.Generic;

public class Global
{
	/* ------------SCENE HIERARCHY-------------------- */
	
	// character object name
	public const string Luna  = "Luna_defined";
	public const string David = "David_defined";

	// character mesh name
	public const string LunaMesh  = "Mesh_Luna_Full";
	public const string DavidMesh = "base";

	// spine joint for controlling offset 
	public static List<string> LunaSpine  = new List<string>{
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt",
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt/Spine_3_Jnt"
	};
	public static List<string> DavidSpine = new List<string>{
		"Root_M/RootPart1_M/Spine1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M/Spine2_M"
	};

	// slide object for changing texture
	public static string Slide = "Environment/Board/Slides/SlideBoard";

	/* -------------PROJECT HIERARCHY----------------- */

	// character controller location
	public static string LunaController  = "Controller/Luna_controller";
	public static string DavidController = "Controller/David_controller";

	// slide texture location
	public static string SlideTexture = "Slide";

	/* -------------OTHER----------------------------- */
	public enum BodyOffset
	{
		Neutral,
		Forward,
		Backward,
		Outward,
		Inward,
	}

	public enum HandPose
	{
		Relax,
		Palm,
		Fist,
		FistRelax,
	}

	public enum Emotion
	{
		Angry,
		Bored,
		Content,
		Happy,
	}

	// identified default gesture index
	public static int DavidDefaultGesture = 36;
	public static int LunaDefaultGesture  = 40;

	// identified beat gesture index
	public static int[] DavidBeatGestures = { 5, 6, 7, 8, 9, 10, 11, 12, 19, 20, 21, 22, 23, 24, 25, 26, 27};
	public static int[] LunaBeatGestures  = { 8, 9, 10, 11, 12, 13, 14, 15, 22, 23, 25, 27, 28, 29, 30, 43};

	// identified idle gesture index
	public static int[] DavidIdleGestures = { 36 };
	public static int[] LunaIdleGestures  = { 40 };
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
