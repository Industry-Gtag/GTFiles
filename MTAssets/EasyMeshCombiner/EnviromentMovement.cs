using System;
using UnityEngine;

namespace MTAssets.EasyMeshCombiner
{
	// Token: 0x0200115F RID: 4447
	public class EnviromentMovement : MonoBehaviour
	{
		// Token: 0x06006FAA RID: 28586 RVA: 0x0023F9AF File Offset: 0x0023DBAF
		private void Start()
		{
			this.thisTransform = base.gameObject.GetComponent<Transform>();
			this.nextPosition = this.pos1;
		}

		// Token: 0x06006FAB RID: 28587 RVA: 0x0023F9D0 File Offset: 0x0023DBD0
		private void Update()
		{
			if (Vector3.Distance(this.thisTransform.position, this.nextPosition) > 0.5f)
			{
				base.transform.position = Vector3.Lerp(this.thisTransform.position, this.nextPosition, 2f * Time.deltaTime);
				return;
			}
			if (this.nextPosition == this.pos1)
			{
				this.nextPosition = this.pos2;
				return;
			}
			if (this.nextPosition == this.pos2)
			{
				this.nextPosition = this.pos1;
				return;
			}
		}

		// Token: 0x04007F8C RID: 32652
		private Vector3 nextPosition = Vector3.zero;

		// Token: 0x04007F8D RID: 32653
		private Transform thisTransform;

		// Token: 0x04007F8E RID: 32654
		public Vector3 pos1;

		// Token: 0x04007F8F RID: 32655
		public Vector3 pos2;
	}
}
