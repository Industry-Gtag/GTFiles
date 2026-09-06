using System;
using System.Collections.Generic;
using AA;
using CjLib;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02001199 RID: 4505
	public class WaterCurrent : MonoBehaviour
	{
		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x06007192 RID: 29074 RVA: 0x0024E68D File Offset: 0x0024C88D
		public float Speed
		{
			get
			{
				return this.currentSpeed;
			}
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x06007193 RID: 29075 RVA: 0x0024E695 File Offset: 0x0024C895
		public float Accel
		{
			get
			{
				return this.currentAccel;
			}
		}

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x06007194 RID: 29076 RVA: 0x0024E69D File Offset: 0x0024C89D
		public float InwardSpeed
		{
			get
			{
				return this.inwardCurrentSpeed;
			}
		}

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x06007195 RID: 29077 RVA: 0x0024E6A5 File Offset: 0x0024C8A5
		public float InwardAccel
		{
			get
			{
				return this.inwardCurrentAccel;
			}
		}

		// Token: 0x06007196 RID: 29078 RVA: 0x0024E6B0 File Offset: 0x0024C8B0
		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (this.fullEffectDistance + this.fadeDistance) * (this.fullEffectDistance + this.fadeDistance);
			bool flag = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 vector = startingVelocity / magnitude;
				float num3 = Spring.DamperDecayExact(magnitude, this.dampingHalfLife, dt, 1E-05f);
				Vector3 vector2 = vector * num3;
				velocityChange += vector2 - startingVelocity;
			}
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector3;
				float closestEvaluationOnSpline = catmullRomSpline.GetClosestEvaluationOnSpline(worldPoint, out vector3);
				Vector3 vector4 = catmullRomSpline.Evaluate(closestEvaluationOnSpline);
				Vector3 vector5 = vector4 - worldPoint;
				if (vector5.sqrMagnitude < num)
				{
					flag = true;
					float magnitude2 = vector5.magnitude;
					float num4 = ((magnitude2 > this.fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - this.fullEffectDistance) / this.fadeDistance)) : 1f);
					float num5 = Mathf.Clamp01(closestEvaluationOnSpline + this.velocityAnticipationAdjustment);
					Vector3 forwardTangent = catmullRomSpline.GetForwardTangent(num5, 0.01f);
					if (this.currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num4 * this.currentSpeed)
					{
						velocityChange += forwardTangent * (this.currentAccel * dt);
					}
					else if (this.currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num4 * this.currentSpeed)
					{
						velocityChange -= forwardTangent * (this.currentAccel * dt);
					}
					currentVelocity += forwardTangent * num4 * this.currentSpeed;
					float num6 = Mathf.InverseLerp(this.inwardCurrentNoEffectRadius, this.inwardCurrentFullEffectRadius, magnitude2);
					if (num6 > num2)
					{
						vector3 = Vector3.ProjectOnPlane(vector5, forwardTangent);
						Vector3 normalized = vector3.normalized;
						if (this.inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num6 * this.inwardCurrentSpeed)
						{
							velocityChange += normalized * (this.InwardAccel * dt);
						}
						else if (this.inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num6 * this.inwardCurrentSpeed)
						{
							velocityChange -= normalized * (this.InwardAccel * dt);
						}
					}
					this.debugSplinePoint = vector4;
				}
			}
			this.debugCurrentVelocity = velocityChange.normalized;
			return flag;
		}

		// Token: 0x06007197 RID: 29079 RVA: 0x0024E964 File Offset: 0x0024CB64
		private void Update()
		{
			if (this.debugDrawCurrentQueries)
			{
				DebugUtil.DrawSphere(this.debugSplinePoint, 0.15f, 12, 12, Color.green, false, DebugUtil.Style.Wireframe);
				DebugUtil.DrawArrow(this.debugSplinePoint, this.debugSplinePoint + this.debugCurrentVelocity, 0.1f, 0.1f, 12, 0.1f, Color.yellow, false, DebugUtil.Style.Wireframe);
			}
		}

		// Token: 0x06007198 RID: 29080 RVA: 0x0024E9C8 File Offset: 0x0024CBC8
		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < this.splines.Count; i++)
			{
				CatmullRomSpline catmullRomSpline = this.splines[i];
				Vector3 vector = catmullRomSpline.Evaluate(0f);
				for (int j = 1; j <= num; j++)
				{
					float num2 = (float)j / (float)num;
					Vector3 vector2 = catmullRomSpline.Evaluate(num2);
					vector2 - vector;
					Quaternion quaternion = Quaternion.LookRotation(catmullRomSpline.GetForwardTangent(num2, 0.01f), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					this.DrawGizmoCircle(vector2, quaternion, this.fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					this.DrawGizmoCircle(vector2, quaternion, this.fullEffectDistance + this.fadeDistance);
				}
			}
		}

		// Token: 0x06007199 RID: 29081 RVA: 0x0024EAB0 File Offset: 0x0024CCB0
		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 vector = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float num2 = (float)i / (float)num * 2f * 3.1415927f;
				Vector3 vector2 = new Vector3(Mathf.Cos(num2), Mathf.Sin(num2), 0f) * radius;
				Gizmos.DrawLine(center + rotation * vector, center + rotation * vector2);
				vector = vector2;
			}
		}

		// Token: 0x04008235 RID: 33333
		[SerializeField]
		private List<CatmullRomSpline> splines = new List<CatmullRomSpline>();

		// Token: 0x04008236 RID: 33334
		[SerializeField]
		private float fullEffectDistance = 1f;

		// Token: 0x04008237 RID: 33335
		[SerializeField]
		private float fadeDistance = 0.5f;

		// Token: 0x04008238 RID: 33336
		[SerializeField]
		private float currentSpeed = 1f;

		// Token: 0x04008239 RID: 33337
		[SerializeField]
		private float currentAccel = 10f;

		// Token: 0x0400823A RID: 33338
		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		// Token: 0x0400823B RID: 33339
		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		// Token: 0x0400823C RID: 33340
		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		// Token: 0x0400823D RID: 33341
		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		// Token: 0x0400823E RID: 33342
		[SerializeField]
		private float inwardCurrentAccel = 10f;

		// Token: 0x0400823F RID: 33343
		[SerializeField]
		private float dampingHalfLife = 0.25f;

		// Token: 0x04008240 RID: 33344
		[SerializeField]
		private bool debugDrawCurrentQueries;

		// Token: 0x04008241 RID: 33345
		private Vector3 debugCurrentVelocity = Vector3.zero;

		// Token: 0x04008242 RID: 33346
		private Vector3 debugSplinePoint = Vector3.zero;
	}
}
