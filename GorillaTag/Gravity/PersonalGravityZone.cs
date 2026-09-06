using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001242 RID: 4674
	public class PersonalGravityZone : BasicGravityZone
	{
		// Token: 0x06007684 RID: 30340 RVA: 0x001B3601 File Offset: 0x001B1801
		public void SetLocalPlayerGravityDirection(Vector3 direction)
		{
			GTPlayerTransform.Instance.SetPersonalGravityDirection(direction);
		}

		// Token: 0x06007685 RID: 30341 RVA: 0x001B360E File Offset: 0x001B180E
		public void SetLocalPlayerGravityDirection(Transform referenceDir)
		{
			GTPlayerTransform.Instance.SetPersonalGravityDirection(referenceDir);
		}

		// Token: 0x06007686 RID: 30342 RVA: 0x002670FE File Offset: 0x002652FE
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return controller.PersonalGravityDirection;
		}

		// Token: 0x06007687 RID: 30343 RVA: 0x00267107 File Offset: 0x00265307
		private void ResetLocalPlayerIfMatch(MonkeGravityController controller)
		{
			if (controller == GTPlayerTransform.Instance)
			{
				GTPlayerTransform.Instance.SetPersonalGravityDirection(Vector3.up);
			}
		}

		// Token: 0x06007688 RID: 30344 RVA: 0x00267125 File Offset: 0x00265325
		protected override void OnTargetExited(MonkeGravityController target)
		{
			this.ResetLocalPlayerIfMatch(target);
		}

		// Token: 0x06007689 RID: 30345 RVA: 0x00267125 File Offset: 0x00265325
		protected override void OnTargetFilteredOut(MonkeGravityController target)
		{
			this.ResetLocalPlayerIfMatch(target);
		}
	}
}
