using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global
{
	// character name
	public static string Luna = "Luna_defined";
	public const string David = "David_defined";

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

	public static List<string> LunaSpine = new List<string>{
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt",
		"Rig_Group/Root_Jnt/Pelvis_Jnt/Spine_2_Jnt/Spine_3_Jnt"
	};

	public static List<string> DavidSpine = new List<string>{
		"Root_M/RootPart1_M/Spine1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M",
		"Root_M/RootPart1_M/Spine1_M/Spine1Part1_M/Spine2_M"
	};
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
