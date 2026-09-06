using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x02001243 RID: 4675
	public class PlanetZone : BasicGravityZone
	{
		// Token: 0x0600768B RID: 30347 RVA: 0x00267136 File Offset: 0x00265336
		protected override void Awake()
		{
			base.Awake();
			this.CalculateDependentVars();
		}

		// Token: 0x0600768C RID: 30348 RVA: 0x00267144 File Offset: 0x00265344
		private void CalculateDependentVars()
		{
			this.sqrDistance = this.rotationDistance * this.rotationDistance;
		}

		// Token: 0x0600768D RID: 30349 RVA: 0x00267159 File Offset: 0x00265359
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return worldPosition - base.transform.position;
		}

		// Token: 0x0600768E RID: 30350 RVA: 0x00267174 File Offset: 0x00265374
		protected override float GetGravityStrength(in Vector3 offsetFromGravity)
		{
			if (!this.useGravityCurve)
			{
				return this.gravityStrength;
			}
			AnimationCurve animationCurve = this.gravityCurve;
			Vector3 vector = offsetFromGravity;
			return animationCurve.Evaluate(vector.magnitude);
		}

		// Token: 0x0600768F RID: 30351 RVA: 0x002671AC File Offset: 0x002653AC
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

		// Token: 0x06007690 RID: 30352 RVA: 0x002671E3 File Offset: 0x002653E3
		public void CopyProperties(PlanetZoneSettings settings)
		{
			base.CopyProperties(settings);
			this.rotationDistance = settings.rotationDistance;
			this.alwaysRotate = settings.alwaysRotate;
			this.useGravityCurve = settings.useGravityCurve;
			this.gravityCurve = settings.gravityCurve;
			this.CalculateDependentVars();
		}

		// Token: 0x0400860B RID: 34315
		[Tooltip("how close to the center of the zone to enable rotating the player")]
		[SerializeField]
		protected float rotationDistance;

		// Token: 0x0400860C RID: 34316
		[Tooltip("if enabled, always rotates the player")]
		[SerializeField]
		protected bool alwaysRotate = true;

		// Token: 0x0400860D RID: 34317
		[Tooltip("if enabled, gravity strength is read from the curve below using distance from the zone's center, instead of the constant gravityStrength")]
		[SerializeField]
		private bool useGravityCurve;

		// Token: 0x0400860E RID: 34318
		[Tooltip("Maps distance from the zone's center (x) to gravity strength (y). Negative y pulls toward center, positive y expels.")]
		[SerializeField]
		private AnimationCurve gravityCurve = AnimationCurve.Constant(0f, 1f, -9.81f);

		// Token: 0x0400860F RID: 34319
		private float sqrDistance;
	}
}
