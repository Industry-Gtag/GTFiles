using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020011B8 RID: 4536
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x06007270 RID: 29296 RVA: 0x00254B7F File Offset: 0x00252D7F
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x04008342 RID: 33602
		public bool snapX;

		// Token: 0x04008343 RID: 33603
		public bool snapY;

		// Token: 0x04008344 RID: 33604
		public bool snapZ;

		// Token: 0x04008345 RID: 33605
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04008346 RID: 33606
		public AudioClip clip;

		// Token: 0x04008347 RID: 33607
		public AudioClip clipOnFullRelease;

		// Token: 0x04008348 RID: 33608
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x04008349 RID: 33609
		public bool climbOnlyWhileSmall;

		// Token: 0x0400834A RID: 33610
		public bool IsPlayerAttached;

		// Token: 0x0400834B RID: 33611
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x0400834C RID: 33612
		[NonSerialized]
		public Collider colliderCache;
	}
}
