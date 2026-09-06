using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011AA RID: 4522
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x040082D3 RID: 33491
		public float minSlidePitch = 0.5f;

		// Token: 0x040082D4 RID: 33492
		public float maxSlidePitch = 1f;

		// Token: 0x040082D5 RID: 33493
		public float minSlideVolume;

		// Token: 0x040082D6 RID: 33494
		public float maxSlideVolume = 0.2f;

		// Token: 0x040082D7 RID: 33495
		public float maxSpeed = 10f;

		// Token: 0x040082D8 RID: 33496
		public float gravityMulti = 1.1f;

		// Token: 0x040082D9 RID: 33497
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x040082DA RID: 33498
		public float maxFriction = 1f;

		// Token: 0x040082DB RID: 33499
		public float maxFrictionSpeed = 15f;
	}
}
