using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011A7 RID: 4519
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x040082C1 RID: 33473
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x040082C2 RID: 33474
		public float frictionWhenNotHeld = 0.25f;
	}
}
