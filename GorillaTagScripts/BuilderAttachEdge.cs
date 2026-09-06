using System;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F53 RID: 3923
	public class BuilderAttachEdge : MonoBehaviour
	{
		// Token: 0x0600606F RID: 24687 RVA: 0x001E9092 File Offset: 0x001E7292
		private void Awake()
		{
			if (this.center == null)
			{
				this.center = base.transform;
			}
		}

		// Token: 0x06006070 RID: 24688 RVA: 0x001E90B0 File Offset: 0x001E72B0
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Transform transform = this.center;
			if (transform == null)
			{
				transform = base.transform;
			}
			Vector3 vector = transform.rotation * Vector3.right;
			Gizmos.DrawLine(transform.position - vector * this.length * 0.5f, transform.position + vector * this.length * 0.5f);
		}

		// Token: 0x04006F05 RID: 28421
		public Transform center;

		// Token: 0x04006F06 RID: 28422
		public float length;
	}
}
