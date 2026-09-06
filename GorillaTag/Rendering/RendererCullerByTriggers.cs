using System;
using UnityEngine;

namespace GorillaTag.Rendering
{
	// Token: 0x020012B2 RID: 4786
	public class RendererCullerByTriggers : MonoBehaviour, IBuildValidation
	{
		// Token: 0x0600783E RID: 30782 RVA: 0x0026EC08 File Offset: 0x0026CE08
		protected void OnEnable()
		{
			this.camWasTouching = false;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = false;
				}
			}
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
		}

		// Token: 0x0600783F RID: 30783 RVA: 0x0026EC64 File Offset: 0x0026CE64
		protected void LateUpdate()
		{
			if (this.mainCameraTransform == null)
			{
				this.mainCameraTransform = Camera.main.transform;
			}
			Vector3 position = this.mainCameraTransform.position;
			bool flag = false;
			foreach (Collider collider in this.colliders)
			{
				if (!(collider == null) && (collider.ClosestPoint(position) - position).sqrMagnitude < 0.010000001f)
				{
					flag = true;
					break;
				}
			}
			if (this.camWasTouching == flag)
			{
				return;
			}
			this.camWasTouching = flag;
			foreach (Renderer renderer in this.renderers)
			{
				if (renderer != null)
				{
					renderer.enabled = flag;
				}
			}
		}

		// Token: 0x06007840 RID: 30784 RVA: 0x00023F0C File Offset: 0x0002210C
		public bool BuildValidationCheck()
		{
			return true;
		}

		// Token: 0x04008865 RID: 34917
		[Tooltip("These renderers will be enabled/disabled depending on if the main camera is the colliders.")]
		public Renderer[] renderers;

		// Token: 0x04008866 RID: 34918
		public Collider[] colliders;

		// Token: 0x04008867 RID: 34919
		private bool camWasTouching;

		// Token: 0x04008868 RID: 34920
		private const float cameraRadiusSq = 0.010000001f;

		// Token: 0x04008869 RID: 34921
		private Transform mainCameraTransform;
	}
}
