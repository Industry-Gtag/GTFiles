using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020012A3 RID: 4771
	public class PlanarSound : MonoBehaviour
	{
		// Token: 0x060077E7 RID: 30695 RVA: 0x0026D0D5 File Offset: 0x0026B2D5
		protected void OnEnable()
		{
			if (Camera.main != null)
			{
				this.cameraXform = Camera.main.transform;
				this.hasCamera = true;
			}
		}

		// Token: 0x060077E8 RID: 30696 RVA: 0x0026D0FC File Offset: 0x0026B2FC
		protected void LateUpdate()
		{
			if (!this.hasCamera)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 vector = transform.parent.InverseTransformPoint(this.cameraXform.position);
			vector.y = 0f;
			if (this.limitDistance && vector.sqrMagnitude > this.maxDistance * this.maxDistance)
			{
				vector = vector.normalized * this.maxDistance;
			}
			transform.localPosition = vector;
		}

		// Token: 0x0400881C RID: 34844
		private Transform cameraXform;

		// Token: 0x0400881D RID: 34845
		private bool hasCamera;

		// Token: 0x0400881E RID: 34846
		[SerializeField]
		private bool limitDistance;

		// Token: 0x0400881F RID: 34847
		[SerializeField]
		private float maxDistance = 1f;
	}
}
