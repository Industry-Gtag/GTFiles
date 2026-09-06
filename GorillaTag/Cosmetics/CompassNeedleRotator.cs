using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012CE RID: 4814
	public class CompassNeedleRotator : MonoBehaviour
	{
		// Token: 0x060078A2 RID: 30882 RVA: 0x00272566 File Offset: 0x00270766
		protected void OnEnable()
		{
			this.currentVelocity = 0f;
			base.transform.localRotation = Quaternion.identity;
		}

		// Token: 0x060078A3 RID: 30883 RVA: 0x00272584 File Offset: 0x00270784
		protected void LateUpdate()
		{
			Transform transform = base.transform;
			Vector3 forward = transform.forward;
			forward.y = 0f;
			forward.Normalize();
			float num = Mathf.SmoothDamp(Vector3.SignedAngle(forward, Vector3.forward, Vector3.up), 0f, ref this.currentVelocity, 0.005f);
			transform.Rotate(transform.up, num, Space.World);
		}

		// Token: 0x0400890A RID: 35082
		private const float smoothTime = 0.005f;

		// Token: 0x0400890B RID: 35083
		private float currentVelocity;
	}
}
