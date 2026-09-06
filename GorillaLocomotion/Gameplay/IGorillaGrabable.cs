using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011AB RID: 4523
	internal interface IGorillaGrabable
	{
		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x0600720D RID: 29197
		string name { get; }

		// Token: 0x0600720E RID: 29198
		bool MomentaryGrabOnly();

		// Token: 0x0600720F RID: 29199
		bool CanBeGrabbed(GorillaGrabber grabber);

		// Token: 0x06007210 RID: 29200
		void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition);

		// Token: 0x06007211 RID: 29201
		void OnGrabReleased(GorillaGrabber grabber);
	}
}
