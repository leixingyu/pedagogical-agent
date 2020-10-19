using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Setup Foot IK control by accessing final ik foot component (pre-set)
 * Needs to be called by the global control or in other places */

public class LegIKControl : MonoBehaviour
{
	public RootMotion.FinalIK.FullBodyBipedIK ikSystem;

	public void footLock() {
		ikSystem.solver.leftFootEffector.positionWeight = 1.0f;
		ikSystem.solver.leftFootEffector.rotationWeight = 1.0f;

		ikSystem.solver.rightFootEffector.positionWeight = 1.0f;
		ikSystem.solver.rightFootEffector.rotationWeight = 1.0f;
	}

	public void footUnlock() {
		ikSystem.solver.leftFootEffector.positionWeight = 0.0f;
		ikSystem.solver.leftFootEffector.rotationWeight = 0.0f;

		ikSystem.solver.rightFootEffector.positionWeight = 0.0f;
		ikSystem.solver.rightFootEffector.rotationWeight = 0.0f;
	}
}
