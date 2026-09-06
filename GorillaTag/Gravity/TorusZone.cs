using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001245 RID: 4677
	public class TorusZone : BasicGravityZone
	{
		// Token: 0x06007694 RID: 30356 RVA: 0x00267286 File Offset: 0x00265486
		protected override void Awake()
		{
			base.Awake();
			this.CalculateDependentVars();
		}

		// Token: 0x06007695 RID: 30357 RVA: 0x00267294 File Offset: 0x00265494
		private void CalculateDependentVars()
		{
			this.sqrDistance = this.rotationDistance * this.rotationDistance;
		}

		// Token: 0x06007696 RID: 30358 RVA: 0x002672AC File Offset: 0x002654AC
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			Vector3 up = base.transform.up;
			Vector3 vector = worldPosition - base.transform.position;
			Vector3 vector2 = vector - up * Vector3.Dot(vector, up);
			float sqrMagnitude = vector2.sqrMagnitude;
			Vector3 vector4;
			if (sqrMagnitude < 1E-10f)
			{
				Vector3 vector3 = Vector3.Cross(up, Vector3.right);
				if (vector3.sqrMagnitude < 1E-10f)
				{
					vector3 = Vector3.Cross(up, Vector3.forward);
				}
				vector4 = base.transform.position + vector3.normalized * this.majorRadius;
			}
			else
			{
				vector4 = base.transform.position + vector2 * (this.majorRadius / Mathf.Sqrt(sqrMagnitude));
			}
			return worldPosition - vector4;
		}

		// Token: 0x06007697 RID: 30359 RVA: 0x00267380 File Offset: 0x00265580
		protected override bool GetRotationIntent(in Vector3 offsetFromGravity)
		{
			if (this.alwaysRotate)
			{
				return true;
			}
			if (this.rotateTarget)
			{
				Vector3 vector = offsetFromGravity;
				return vector.sqrMagnitude < this.sqrDistance;
			}
			return false;
		}

		// Token: 0x06007698 RID: 30360 RVA: 0x002673B7 File Offset: 0x002655B7
		public void CopyProperties(TorusZoneSettings settings)
		{
			base.CopyProperties(settings);
			this.majorRadius = settings.majorRadius;
			this.rotationDistance = settings.rotationDistance;
			this.alwaysRotate = settings.alwaysRotate;
			this.CalculateDependentVars();
		}

		// Token: 0x04008611 RID: 34321
		[Tooltip("Major radius of the torus (distance from torus center to the centerline of the tube). Torus axis is transform.up.")]
		[SerializeField]
		protected float majorRadius = 5f;

		// Token: 0x04008612 RID: 34322
		[Tooltip("how close to the central ring of the torus to enable rotating the player")]
		[SerializeField]
		protected float rotationDistance;

		// Token: 0x04008613 RID: 34323
		[Tooltip("if enabled, always rotates the player")]
		[SerializeField]
		protected bool alwaysRotate = true;

		// Token: 0x04008614 RID: 34324
		private float sqrDistance;
	}
}
