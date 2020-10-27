using System.Collections.Generic;

public class Global
{
	// character object name
	public const string Luna = "Luna_defined";
	public const string David = "David_defined";

	// character mesh name
	public const string LunaMesh = "Mesh_Luna_Full";
	public const string DavidMesh = "base";

	public enum BodyOffset
	{
		NEUTRAL,
		FORWARD,
		BACKWARD,
		OUTWARD,
		INWARD
	}

	public enum HandPose
	{
		Relax,
		Palm,
		Fist,
		FistRelax,
	}

	// spine joint hierarchy used for controlling offset 
	public static List<string> LunaSpine = new List<string>{
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt",
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt/Spine_3_Jnt"
	};
	public static List<string> DavidSpine = new List<string>{
		"Root_M/RootPart1_M/Spine1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M/Spine2_M"
	};

	// controller location in respect of the project hierarchy
	public static string LunaController = "Controller/Luna_controller";
	public static string DavidController = "Controller/David_controller";
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
