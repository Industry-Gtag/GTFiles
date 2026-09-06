using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001240 RID: 4672
	[Serializable]
	public struct GravityInfo
	{
		// Token: 0x04008600 RID: 34304
		public Vector3 gravityUpDirection;

		// Token: 0x04008601 RID: 34305
		public Vector3 rotationDirection;

		// Token: 0x04008602 RID: 34306
		public float rotationSpeed;

		// Token: 0x04008603 RID: 34307
		public float gravityStrength;

		// Token: 0x04008604 RID: 34308
		public bool rotate;
	}
}
