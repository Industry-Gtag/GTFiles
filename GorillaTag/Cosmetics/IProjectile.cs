using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001347 RID: 4935
	public interface IProjectile
	{
		// Token: 0x06007BBB RID: 31675
		void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep = -1);
	}
}
