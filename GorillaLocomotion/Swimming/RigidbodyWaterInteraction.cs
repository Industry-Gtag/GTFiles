using System;
using System.Collections.Generic;
using AA;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02001194 RID: 4500
	[RequireComponent(typeof(Rigidbody))]
	public class RigidbodyWaterInteraction : MonoBehaviour
	{
		// Token: 0x06007174 RID: 29044 RVA: 0x0024CFA6 File Offset: 0x0024B1A6
		protected void Awake()
		{
			this.rb = base.GetComponent<Rigidbody>();
			this.baseAngularDrag = this.rb.angularDamping;
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06007175 RID: 29045 RVA: 0x0024CFCB File Offset: 0x0024B1CB
		protected void OnEnable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.RegisterRBWI(this);
		}

		// Token: 0x06007176 RID: 29046 RVA: 0x0024CFDE File Offset: 0x0024B1DE
		protected void OnDisable()
		{
			this.overlappingWaterVolumes.Clear();
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x06007177 RID: 29047 RVA: 0x0024CFF1 File Offset: 0x0024B1F1
		private void OnDestroy()
		{
			RigidbodyWaterInteractionManager.UnregisterRBWI(this);
		}

		// Token: 0x06007178 RID: 29048 RVA: 0x0024CFFC File Offset: 0x0024B1FC
		public void InvokeFixedUpdate()
		{
			if (this.rb.isKinematic)
			{
				return;
			}
			bool flag = this.overlappingWaterVolumes.Count > 0;
			WaterVolume.SurfaceQuery surfaceQuery = default(WaterVolume.SurfaceQuery);
			float num = float.MinValue;
			if (flag && this.enablePreciseWaterCollision)
			{
				Vector3 vector = base.transform.position + GTPlayerTransform.PhysicsDown * 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
				bool flag2 = false;
				this.activeWaterCurrents.Clear();
				for (int i = 0; i < this.overlappingWaterVolumes.Count; i++)
				{
					WaterVolume.SurfaceQuery surfaceQuery2;
					if (this.overlappingWaterVolumes[i] == null)
					{
						this.overlappingWaterVolumes.RemoveAt(i);
						i--;
					}
					else if (this.overlappingWaterVolumes[i].GetSurfaceQueryForPoint(vector, out surfaceQuery2, false))
					{
						float num2 = Vector3.Dot(surfaceQuery2.surfacePoint - vector, surfaceQuery2.surfaceNormal);
						if (num2 > num)
						{
							num = num2;
							surfaceQuery = surfaceQuery2;
							flag2 = true;
						}
						WaterCurrent waterCurrent = this.overlappingWaterVolumes[i].Current;
						if (this.applyWaterCurrents && waterCurrent != null && num2 > 0f && !this.activeWaterCurrents.Contains(waterCurrent))
						{
							this.activeWaterCurrents.Add(waterCurrent);
						}
					}
				}
				if (flag2)
				{
					bool flag3 = num > -(1f - this.buoyancyEquilibrium) * 2f * this.objectRadiusForWaterCollision;
					float num3 = (this.enablePreciseWaterCollision ? this.objectRadiusForWaterCollision : 0f);
					Vector3 vector2 = surfaceQuery.surfacePoint - surfaceQuery.surfaceNormal * surfaceQuery.maxDepth;
					bool flag4 = Vector3.Dot(base.transform.position + surfaceQuery.surfaceNormal * num3 - vector2, surfaceQuery.surfaceNormal) > 0f;
					flag = flag3 && flag4;
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				float fixedDeltaTime = Time.fixedDeltaTime;
				Vector3 vector3 = this.rb.linearVelocity;
				Vector3 vector4 = Vector3.zero;
				if (this.applyWaterCurrents)
				{
					Vector3 vector5 = Vector3.zero;
					for (int j = 0; j < this.activeWaterCurrents.Count; j++)
					{
						WaterCurrent waterCurrent2 = this.activeWaterCurrents[j];
						Vector3 vector6 = vector3 + vector4;
						Vector3 vector7;
						Vector3 vector8;
						if (waterCurrent2.GetCurrentAtPoint(base.transform.position, vector6, fixedDeltaTime, out vector7, out vector8))
						{
							vector5 += vector7;
							vector4 += vector8;
						}
					}
					if (this.enablePreciseWaterCollision)
					{
						Vector3 vector9 = (surfaceQuery.surfacePoint + (base.transform.position + GTPlayerTransform.PhysicsDown * this.objectRadiusForWaterCollision)) * 0.5f;
						this.rb.AddForceAtPosition(vector4 * this.rb.mass, vector9, ForceMode.Impulse);
					}
					else
					{
						vector3 += vector4;
					}
				}
				if (this.applyBuoyancyForce)
				{
					Vector3 vector10 = Vector3.zero;
					if (this.enablePreciseWaterCollision)
					{
						float num4 = 2f * this.objectRadiusForWaterCollision * this.buoyancyEquilibrium;
						float num5 = Mathf.InverseLerp(0f, num4, num);
						vector10 = GTPlayerTransform.PhysicsUp * Physics.gravity.magnitude * this.underWaterBuoyancyFactor * num5 * fixedDeltaTime;
					}
					else
					{
						vector10 = GTPlayerTransform.PhysicsUp * Physics.gravity.magnitude * this.underWaterBuoyancyFactor * fixedDeltaTime;
					}
					if (vector4.sqrMagnitude > 0.001f)
					{
						float magnitude = vector4.magnitude;
						Vector3 vector11 = vector4 / magnitude;
						float num6 = Vector3.Dot(vector10, vector11);
						if (num6 < 0f)
						{
							vector10 -= num6 * vector11;
						}
					}
					vector3 += vector10;
				}
				float magnitude2 = vector3.magnitude;
				if (magnitude2 > 0.001f && this.applyDamping)
				{
					Vector3 vector12 = vector3 / magnitude2;
					float num7 = Spring.DamperDecayExact(magnitude2, this.underWaterDampingHalfLife, fixedDeltaTime, 1E-05f);
					if (this.enablePreciseWaterCollision)
					{
						float num8 = Spring.DamperDecayExact(magnitude2, this.waterSurfaceDampingHalfLife, fixedDeltaTime, 1E-05f);
						float num9 = Mathf.Clamp(-Vector3.Dot(base.transform.position - surfaceQuery.surfacePoint, surfaceQuery.surfaceNormal) / this.objectRadiusForWaterCollision, -1f, 1f) * 0.5f + 0.5f;
						vector3 = Mathf.Lerp(num8, num7, num9) * vector12;
					}
					else
					{
						vector3 = num7 * vector12;
					}
				}
				if (this.applySurfaceTorque && this.enablePreciseWaterCollision)
				{
					float num10 = Vector3.Dot(base.transform.position - surfaceQuery.surfacePoint, surfaceQuery.surfaceNormal);
					if (num10 < this.objectRadiusForWaterCollision && num10 > 0f)
					{
						Vector3 vector13 = vector3 - Vector3.Dot(vector3, surfaceQuery.surfaceNormal) * surfaceQuery.surfaceNormal;
						Vector3 normalized = Vector3.Cross(surfaceQuery.surfaceNormal, vector13).normalized;
						float num11 = Vector3.Dot(this.rb.angularVelocity, normalized);
						float num12 = vector13.magnitude / this.objectRadiusForWaterCollision - num11;
						if (num12 > 0f)
						{
							this.rb.AddTorque(this.surfaceTorqueAmount * num12 * normalized, ForceMode.Acceleration);
						}
					}
				}
				this.rb.linearVelocity = vector3;
				this.rb.angularDamping = this.angularDrag;
				return;
			}
			this.rb.angularDamping = this.baseAngularDrag;
		}

		// Token: 0x06007179 RID: 29049 RVA: 0x0024D5B8 File Offset: 0x0024B7B8
		protected void OnTriggerEnter(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && !this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Add(component);
			}
		}

		// Token: 0x0600717A RID: 29050 RVA: 0x0024D5F0 File Offset: 0x0024B7F0
		protected void OnTriggerExit(Collider other)
		{
			WaterVolume component = other.GetComponent<WaterVolume>();
			if (component != null && this.overlappingWaterVolumes.Contains(component))
			{
				this.overlappingWaterVolumes.Remove(component);
			}
		}

		// Token: 0x04008203 RID: 33283
		public bool applyDamping = true;

		// Token: 0x04008204 RID: 33284
		public bool applyBuoyancyForce = true;

		// Token: 0x04008205 RID: 33285
		public bool applyAngularDrag = true;

		// Token: 0x04008206 RID: 33286
		public bool applyWaterCurrents = true;

		// Token: 0x04008207 RID: 33287
		public bool applySurfaceTorque = true;

		// Token: 0x04008208 RID: 33288
		public float underWaterDampingHalfLife = 0.25f;

		// Token: 0x04008209 RID: 33289
		public float waterSurfaceDampingHalfLife = 1f;

		// Token: 0x0400820A RID: 33290
		public float underWaterBuoyancyFactor = 0.5f;

		// Token: 0x0400820B RID: 33291
		public float angularDrag = 0.5f;

		// Token: 0x0400820C RID: 33292
		public float surfaceTorqueAmount = 0.5f;

		// Token: 0x0400820D RID: 33293
		public bool enablePreciseWaterCollision;

		// Token: 0x0400820E RID: 33294
		public float objectRadiusForWaterCollision = 0.25f;

		// Token: 0x0400820F RID: 33295
		[Range(0f, 1f)]
		public float buoyancyEquilibrium = 0.8f;

		// Token: 0x04008210 RID: 33296
		private Rigidbody rb;

		// Token: 0x04008211 RID: 33297
		private List<WaterVolume> overlappingWaterVolumes = new List<WaterVolume>();

		// Token: 0x04008212 RID: 33298
		private List<WaterCurrent> activeWaterCurrents = new List<WaterCurrent>(16);

		// Token: 0x04008213 RID: 33299
		private float baseAngularDrag = 0.05f;
	}
}
