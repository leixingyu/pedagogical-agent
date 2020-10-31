using System.Collections;
using UnityEngine;
using RootMotion.FinalIK;

public class MainOffset : MonoBehaviour
{
	private BodyEffectorSetup effector;

	// Forward
	Vector3 lHandForward = new Vector3(0, 0.02f, 0.1f);
	Vector3 rHandForward = new Vector3(0, 0.02f, 0.1f);
	Vector3 lShoulderForward = new Vector3(0, 0, 0.05f);
	Vector3 rShoulderForward = new Vector3(0, 0, 0.05f);

	// Backward
	Vector3 lHandBackward = new Vector3(0, -0.02f, -0.1f);
	Vector3 rHandBackward = new Vector3(0, -0.02f, -0.1f);
	Vector3 lShoulderBackward = new Vector3(0, 0, -0.05f);
	Vector3 rShoulderBackward = new Vector3(0, 0, -0.05f);

	Vector3 lHandPre, rHandPre, lShoulderPre, rShoulderPre, lHandNext, rHandNext, lShoulderNext, rShoulderNext;

	private void Awake()
	{
		effector = gameObject.AddComponent<BodyEffectorSetup>();
	}

	public IEnumerator bodyOffset(Global.BodyOffset type, float strength = 100.0f, int frames = Setting.bodyOffsetBlend)
	{
		// store previous value
		lHandPre = effector.leftHandOffset;
		rHandPre = effector.rightHandOffset;
		lShoulderPre = effector.leftHandOffset;
		rShoulderPre = effector.leftHandOffset;

		// determine the next value based on offset type and strength
		if(type == Global.BodyOffset.Backward)
		{
			lHandNext = lHandBackward * strength / 100.0f;
			rHandNext = rHandBackward * strength / 100.0f;
			lShoulderNext = lShoulderBackward * strength / 100.0f;
			rShoulderNext = rShoulderBackward * strength / 100.0f;
		}
		else if (type == Global.BodyOffset.Forward)
		{
			lHandNext = lHandForward * strength / 100.0f;
			rHandNext = rHandForward * strength / 100.0f;
			lShoulderNext = lShoulderForward * strength / 100.0f;
			rShoulderNext = rShoulderForward * strength / 100.0f;
		}
		else
		{
			lHandNext = new Vector3(0.0f, 0.0f, 0.0f);
			rHandNext = new Vector3(0.0f, 0.0f, 0.0f);
			lShoulderNext = new Vector3(0.0f, 0.0f, 0.0f);
			rShoulderNext = new Vector3(0.0f, 0.0f, 0.0f);
		}

		for (int i = 0; i <= frames; i++)
		{
			yield return null;
			Vector3 lHandCurrent = Vector3.Lerp(lHandPre, lHandNext, (float)i / (float)frames);
			Vector3 rHandCurrent = Vector3.Lerp(rHandPre, rHandNext, (float)i / (float)frames);
			Vector3 lShoulderCurrent = Vector3.Lerp(lShoulderPre, lShoulderNext, (float)i / (float)frames);
			Vector3 rShoulderCurrent = Vector3.Lerp(rShoulderPre, rShoulderNext, (float)i / (float)frames);
			effector.leftHandOffset.Set(lHandCurrent.x, lHandCurrent.y, lHandCurrent.z);
			effector.rightHandOffset.Set(rHandCurrent.x, rHandCurrent.y, rHandCurrent.z);
			effector.leftShoulderOffset.Set(lShoulderCurrent.x, lShoulderCurrent.y, lShoulderCurrent.z);
			effector.rightShoulderOffset.Set(rShoulderCurrent.x, rShoulderCurrent.y, rShoulderCurrent.z);
		}
	}
}

public class BodyEffectorSetup : OffsetModifier
{
	// If 1, The hand effectors will maintain their position relative to their parent triangle's rotation {root node, left shoulder, right shoulder} 
	[Range(0f, 1f)]
	public float handsMaintainRelativePositionWeight;

	// The offset vectors for each effector
	//public Vector3 bodyOffset;
	public Vector3 leftHandOffset;
	public Vector3 rightHandOffset;
	public Vector3 leftShoulderOffset;
	public Vector3 rightShoulderOffset;

	private void Awake()
	{
		ik = gameObject.GetComponent<FullBodyBipedIK>();
	}

	protected override void OnModifyOffset()
	{
		// How much will the hand effectors maintain their position relative to their parent triangle's rotation {root node, left shoulder, right shoulder} ?
		ik.solver.leftHandEffector.maintainRelativePositionWeight = handsMaintainRelativePositionWeight;
		ik.solver.rightHandEffector.maintainRelativePositionWeight = handsMaintainRelativePositionWeight;

		// Apply position offsets relative to this GameObject's rotation.
		//ik.solver.bodyEffector.positionOffset += transform.rotation * bodyOffset * weight;
		ik.solver.leftShoulderEffector.positionOffset += transform.rotation * leftShoulderOffset * weight;
		ik.solver.rightShoulderEffector.positionOffset += transform.rotation * rightShoulderOffset * weight;
		ik.solver.leftHandEffector.positionOffset += transform.rotation * leftHandOffset * weight;
		ik.solver.rightHandEffector.positionOffset += transform.rotation * rightHandOffset * weight;
	}
}
