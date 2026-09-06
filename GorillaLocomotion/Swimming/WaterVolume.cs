using System;
using System.Collections.Generic;
using CjLib;
using GorillaLocomotion.Climbing;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200119D RID: 4509
	[RequireComponent(typeof(Collider))]
	public class WaterVolume : BaseGuidedRefTargetMono, ITickSystemTick
	{
		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x060071A2 RID: 29090 RVA: 0x0024F00C File Offset: 0x0024D20C
		// (set) Token: 0x060071A3 RID: 29091 RVA: 0x0024F014 File Offset: 0x0024D214
		public bool TickRunning { get; set; }

		// Token: 0x140000C6 RID: 198
		// (add) Token: 0x060071A4 RID: 29092 RVA: 0x0024F020 File Offset: 0x0024D220
		// (remove) Token: 0x060071A5 RID: 29093 RVA: 0x0024F058 File Offset: 0x0024D258
		public event WaterVolume.WaterVolumeEvent ColliderEnteredVolume;

		// Token: 0x140000C7 RID: 199
		// (add) Token: 0x060071A6 RID: 29094 RVA: 0x0024F090 File Offset: 0x0024D290
		// (remove) Token: 0x060071A7 RID: 29095 RVA: 0x0024F0C8 File Offset: 0x0024D2C8
		public event WaterVolume.WaterVolumeEvent ColliderExitedVolume;

		// Token: 0x140000C8 RID: 200
		// (add) Token: 0x060071A8 RID: 29096 RVA: 0x0024F100 File Offset: 0x0024D300
		// (remove) Token: 0x060071A9 RID: 29097 RVA: 0x0024F138 File Offset: 0x0024D338
		public event WaterVolume.WaterVolumeEvent ColliderEnteredWater;

		// Token: 0x140000C9 RID: 201
		// (add) Token: 0x060071AA RID: 29098 RVA: 0x0024F170 File Offset: 0x0024D370
		// (remove) Token: 0x060071AB RID: 29099 RVA: 0x0024F1A8 File Offset: 0x0024D3A8
		public event WaterVolume.WaterVolumeEvent ColliderExitedWater;

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x060071AC RID: 29100 RVA: 0x0024F1DD File Offset: 0x0024D3DD
		public GTPlayer.LiquidType LiquidType
		{
			get
			{
				return this.liquidType;
			}
		}

		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x060071AD RID: 29101 RVA: 0x0024F1E5 File Offset: 0x0024D3E5
		public WaterCurrent Current
		{
			get
			{
				return this.waterCurrent;
			}
		}

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x060071AE RID: 29102 RVA: 0x0024F1ED File Offset: 0x0024D3ED
		public WaterParameters Parameters
		{
			get
			{
				return this.waterParams;
			}
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x060071AF RID: 29103 RVA: 0x0024F1F8 File Offset: 0x0024D3F8
		private VRRig PlayerVRRig
		{
			get
			{
				if (this.playerVRRig == null)
				{
					GorillaTagger instance = GorillaTagger.Instance;
					if (instance != null)
					{
						this.playerVRRig = instance.offlineVRRig;
					}
				}
				return this.playerVRRig;
			}
		}

		// Token: 0x060071B0 RID: 29104 RVA: 0x0024F234 File Offset: 0x0024D434
		public bool GetSurfaceQueryForPoint(Vector3 point, out WaterVolume.SurfaceQuery result, bool debugDraw = false)
		{
			result = default(WaterVolume.SurfaceQuery);
			if (!this.isStationary)
			{
				float num = float.MinValue;
				float num2 = float.MaxValue;
				for (int i = 0; i < this.volumeColliders.Count; i++)
				{
					if (!(this.volumeColliders[i] == null))
					{
						float y = this.volumeColliders[i].bounds.max.y;
						float y2 = this.volumeColliders[i].bounds.min.y;
						if (y > num)
						{
							num = y;
						}
						if (y2 < num2)
						{
							num2 = y2;
						}
					}
				}
				this.volumeMaxHeight = num;
				this.volumeMinHeight = num2;
			}
			Vector3 vector = ((this.surfacePlane != null) ? this.surfacePlane.up : Vector3.up);
			Vector3 vector2 = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
			float num3 = float.MinValue;
			float num4 = float.MaxValue;
			for (int j = 0; j < this.volumeColliders.Count; j++)
			{
				if (!(this.volumeColliders[j] == null))
				{
					Bounds bounds = this.volumeColliders[j].bounds;
					float num5 = Vector3.Dot(bounds.center, vector);
					float num6 = Vector3.Dot(bounds.extents, vector2);
					float num7 = num5 + num6;
					float num8 = num5 - num6;
					if (num7 > num3)
					{
						num3 = num7;
					}
					if (num8 < num4)
					{
						num4 = num8;
					}
				}
			}
			float num9 = Vector3.Dot(point, vector);
			Ray ray = new Ray(point + vector * (num3 - num9), -vector);
			Ray ray2 = new Ray(point + vector * (num4 - num9), vector);
			float num10 = num3 - num4;
			float num11 = float.MinValue;
			float num12 = float.MaxValue;
			bool flag = false;
			bool flag2 = false;
			float num13 = 0f;
			for (int k = 0; k < this.surfaceColliders.Count; k++)
			{
				if (!(this.surfaceColliders[k] == null))
				{
					bool enabled = this.surfaceColliders[k].enabled;
					this.surfaceColliders[k].enabled = true;
					RaycastHit raycastHit;
					if (this.surfaceColliders[k].Raycast(ray, out raycastHit, num10))
					{
						float num14 = Vector3.Dot(raycastHit.point, vector);
						if (num14 > num11 && this.HitOutsideSurfaceOfMesh(ray.direction, this.surfaceColliders[k], raycastHit))
						{
							num11 = num14;
							flag = true;
							result.surfacePoint = raycastHit.point;
							result.surfaceNormal = raycastHit.normal;
						}
					}
					RaycastHit raycastHit2;
					if (this.surfaceColliders[k].Raycast(ray2, out raycastHit2, num10))
					{
						float num15 = Vector3.Dot(raycastHit2.point, vector);
						if (num15 < num12 && this.HitOutsideSurfaceOfMesh(ray2.direction, this.surfaceColliders[k], raycastHit2))
						{
							num12 = num15;
							flag2 = true;
							num13 = num15;
						}
					}
					this.surfaceColliders[k].enabled = enabled;
				}
			}
			if (!flag && this.surfacePlane != null)
			{
				flag = true;
				result.surfacePoint = point - Vector3.Dot(point - this.surfacePlane.position, this.surfacePlane.up) * this.surfacePlane.up;
				result.surfaceNormal = this.surfacePlane.up;
			}
			if (flag && flag2)
			{
				result.maxDepth = Vector3.Dot(result.surfacePoint, vector) - num13;
			}
			else if (flag)
			{
				result.maxDepth = Vector3.Dot(result.surfacePoint, vector) - num4;
			}
			else
			{
				result.maxDepth = num3 - num4;
			}
			if (debugDraw)
			{
				if (flag)
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num10, Color.green, false);
					DebugUtil.DrawSphere(result.surfacePoint, 0.001f, 12, 12, Color.green, false, DebugUtil.Style.SolidColor);
				}
				else
				{
					DebugUtil.DrawLine(ray.origin, ray.origin + ray.direction * num10, Color.red, false);
				}
				if (flag2)
				{
					DebugUtil.DrawLine(ray2.origin, ray2.origin + ray2.direction * num10, Color.yellow, false);
					DebugUtil.DrawSphere(result.surfacePoint + vector * (num13 - Vector3.Dot(result.surfacePoint, vector)), 0.001f, 12, 12, Color.yellow, false, DebugUtil.Style.SolidColor);
				}
			}
			return flag;
		}

		// Token: 0x060071B1 RID: 29105 RVA: 0x0024F6EC File Offset: 0x0024D8EC
		private bool HitOutsideSurfaceOfMesh(Vector3 castDir, MeshCollider meshCollider, RaycastHit hit)
		{
			if (!WaterVolume.meshTrianglesDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshTris))
			{
				this.sharedMeshTris = (int[])meshCollider.sharedMesh.triangles.Clone();
				WaterVolume.meshTrianglesDict.Add(meshCollider.sharedMesh, this.sharedMeshTris);
			}
			if (!WaterVolume.meshVertsDict.TryGetValue(meshCollider.sharedMesh, out this.sharedMeshVerts))
			{
				this.sharedMeshVerts = (Vector3[])meshCollider.sharedMesh.vertices.Clone();
				WaterVolume.meshVertsDict.Add(meshCollider.sharedMesh, this.sharedMeshVerts);
			}
			Vector3 vector = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3]];
			Vector3 vector2 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 1]];
			Vector3 vector3 = this.sharedMeshVerts[this.sharedMeshTris[hit.triangleIndex * 3 + 2]];
			Vector3 vector4 = meshCollider.transform.TransformDirection(Vector3.Cross(vector2 - vector, vector3 - vector).normalized);
			bool flag = Vector3.Dot(castDir, vector4) < 0f;
			if (this.debugDrawSurfaceCast)
			{
				Color color = (flag ? Color.blue : Color.red);
				DebugUtil.DrawLine(hit.point, hit.point + vector4 * 0.3f, color, false);
			}
			return flag;
		}

		// Token: 0x060071B2 RID: 29106 RVA: 0x0024F860 File Offset: 0x0024DA60
		private void DebugDrawMeshColliderHitTriangle(RaycastHit hit)
		{
			MeshCollider meshCollider = hit.collider as MeshCollider;
			if (meshCollider != null)
			{
				Mesh sharedMesh = meshCollider.sharedMesh;
				int[] triangles = sharedMesh.triangles;
				Vector3[] vertices = sharedMesh.vertices;
				Vector3 vector = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3]]);
				Vector3 vector2 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
				Vector3 vector3 = meshCollider.gameObject.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				float num = 0.2f;
				DebugUtil.DrawLine(vector, vector + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector2 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector3, vector3 + normalized * num, Color.blue, false);
				DebugUtil.DrawLine(vector, vector2, Color.blue, false);
				DebugUtil.DrawLine(vector, vector3, Color.blue, false);
				DebugUtil.DrawLine(vector2, vector3, Color.blue, false);
			}
		}

		// Token: 0x060071B3 RID: 29107 RVA: 0x0024F9AC File Offset: 0x0024DBAC
		public bool RaycastWater(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, int layerMask)
		{
			if (this.triggerCollider != null)
			{
				return Physics.Raycast(new Ray(origin, direction), out hit, distance, layerMask, QueryTriggerInteraction.Collide);
			}
			hit = default(RaycastHit);
			return false;
		}

		// Token: 0x060071B4 RID: 29108 RVA: 0x0024F9D8 File Offset: 0x0024DBD8
		public bool CheckColliderInVolume(Collider collider, out bool inWater, out bool surfaceDetected)
		{
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == collider)
				{
					inWater = this.persistentColliders[i].inWater;
					surfaceDetected = this.persistentColliders[i].surfaceDetected;
					return true;
				}
			}
			inWater = false;
			surfaceDetected = false;
			return false;
		}

		// Token: 0x060071B5 RID: 29109 RVA: 0x0024FA43 File Offset: 0x0024DC43
		protected override void Awake()
		{
			base.Awake();
			this.RefreshColliders();
		}

		// Token: 0x060071B6 RID: 29110 RVA: 0x0001A297 File Offset: 0x00018497
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x060071B7 RID: 29111 RVA: 0x0024FA54 File Offset: 0x0024DC54
		public void RefreshColliders()
		{
			this.triggerCollider = base.GetComponent<Collider>();
			if (this.volumeColliders == null || this.volumeColliders.Count < 1)
			{
				this.volumeColliders = new List<Collider>();
				this.volumeColliders.Add(base.gameObject.GetComponent<Collider>());
			}
			float num = float.MinValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.volumeColliders.Count; i++)
			{
				if (!(this.volumeColliders[i] == null))
				{
					float y = this.volumeColliders[i].bounds.max.y;
					float y2 = this.volumeColliders[i].bounds.min.y;
					if (y > num)
					{
						num = y;
					}
					if (y2 < num2)
					{
						num2 = y2;
					}
				}
			}
			this.volumeMaxHeight = num;
			this.volumeMinHeight = num2;
		}

		// Token: 0x060071B8 RID: 29112 RVA: 0x0024FB38 File Offset: 0x0024DD38
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				waterOverlappingCollider.inVolume = false;
				waterOverlappingCollider.playDripEffect = false;
				WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
				if (colliderExitedVolume != null)
				{
					colliderExitedVolume(this, waterOverlappingCollider.collider);
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
			this.RemoveCollidersOutsideVolume(Time.time);
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x060071B9 RID: 29113 RVA: 0x0024FBB8 File Offset: 0x0024DDB8
		public void Tick()
		{
			if (this.persistentColliders.Count < 1)
			{
				return;
			}
			float time = Time.time;
			this.RemoveCollidersOutsideVolume(time);
			if (!this.CanPlayerSwim())
			{
				return;
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				bool inWater = waterOverlappingCollider.inWater;
				if (waterOverlappingCollider.inVolume)
				{
					this.CheckColliderAgainstWater(ref waterOverlappingCollider, time);
				}
				else
				{
					waterOverlappingCollider.inWater = false;
				}
				this.TryRegisterOwnershipOfCollider(waterOverlappingCollider.collider, waterOverlappingCollider.inWater, waterOverlappingCollider.surfaceDetected);
				if (waterOverlappingCollider.inWater && !inWater)
				{
					this.OnWaterSurfaceEnter(ref waterOverlappingCollider);
				}
				else if (!waterOverlappingCollider.inWater && inWater)
				{
					this.OnWaterSurfaceExit(ref waterOverlappingCollider, time);
				}
				if (this.HasOwnershipOfCollider(waterOverlappingCollider.collider) && waterOverlappingCollider.surfaceDetected)
				{
					if (!waterOverlappingCollider.inWater)
					{
						this.ColliderOutOfWaterUpdate(ref waterOverlappingCollider, time);
					}
					else
					{
						this.ColliderInWaterUpdate(ref waterOverlappingCollider, time);
					}
				}
				this.persistentColliders[i] = waterOverlappingCollider;
			}
		}

		// Token: 0x060071BA RID: 29114 RVA: 0x0024FCB8 File Offset: 0x0024DEB8
		private void RemoveCollidersOutsideVolume(float currentTime)
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			for (int i = this.persistentColliders.Count - 1; i >= 0; i--)
			{
				WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
				if (waterOverlappingCollider.collider == null || !waterOverlappingCollider.collider.gameObject.activeInHierarchy || (!waterOverlappingCollider.inVolume && (!waterOverlappingCollider.playDripEffect || currentTime - waterOverlappingCollider.lastInWaterTime > this.waterParams.postExitDripDuration)) || !this.CanPlayerSwim())
				{
					this.UnregisterOwnershipOfCollider(waterOverlappingCollider.collider);
					GTPlayer instance = GTPlayer.Instance;
					if (waterOverlappingCollider.collider == instance.headCollider || waterOverlappingCollider.collider == instance.bodyCollider)
					{
						instance.OnExitWaterVolume(waterOverlappingCollider.collider, this);
					}
					this.persistentColliders.RemoveAt(i);
				}
			}
		}

		// Token: 0x060071BB RID: 29115 RVA: 0x0024FD98 File Offset: 0x0024DF98
		private void CheckColliderAgainstWater(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 position = persistentCollider.collider.transform.position;
			bool flag = true;
			if (persistentCollider.surfaceDetected && persistentCollider.scaleMultiplier > 0.99f && this.isStationary)
			{
				flag = (position - Vector3.Dot(position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) * persistentCollider.lastSurfaceQuery.surfaceNormal - persistentCollider.lastSurfaceQuery.surfacePoint).sqrMagnitude > this.waterParams.recomputeSurfaceForColliderDist * this.waterParams.recomputeSurfaceForColliderDist;
			}
			if (flag)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (this.GetSurfaceQueryForPoint(position, out surfaceQuery, this.debugDrawSurfaceCast))
				{
					persistentCollider.surfaceDetected = true;
					persistentCollider.lastSurfaceQuery = surfaceQuery;
				}
				else
				{
					persistentCollider.surfaceDetected = false;
					persistentCollider.lastSurfaceQuery = default(WaterVolume.SurfaceQuery);
				}
			}
			if (persistentCollider.surfaceDetected)
			{
				bool flag2 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.down * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.down * 10f)).y < persistentCollider.lastSurfaceQuery.surfacePoint.y;
				bool flag3 = ((persistentCollider.collider is MeshCollider) ? persistentCollider.collider.ClosestPointOnBounds(position + Vector3.up * 10f) : persistentCollider.collider.ClosestPoint(position + Vector3.up * 10f)).y > persistentCollider.lastSurfaceQuery.surfacePoint.y - persistentCollider.lastSurfaceQuery.maxDepth;
				persistentCollider.inWater = flag2 && flag3;
			}
			else
			{
				persistentCollider.inWater = false;
			}
			if (persistentCollider.inWater)
			{
				persistentCollider.lastInWaterTime = currentTime;
			}
		}

		// Token: 0x060071BC RID: 29116 RVA: 0x0024FF80 File Offset: 0x0024E180
		private Vector3 GetColliderVelocity(ref WaterOverlappingCollider persistentCollider)
		{
			GTPlayer instance = GTPlayer.Instance;
			Vector3 vector = Vector3.one * (this.waterParams.splashSpeedRequirement + 0.1f);
			if (persistentCollider.velocityTracker != null)
			{
				vector = persistentCollider.velocityTracker.GetAverageVelocity(true, 0.1f, false);
			}
			else if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				vector = instance.AveragedVelocity;
			}
			else if (persistentCollider.collider.attachedRigidbody != null && !persistentCollider.collider.attachedRigidbody.isKinematic)
			{
				vector = persistentCollider.collider.attachedRigidbody.linearVelocity;
			}
			return vector;
		}

		// Token: 0x060071BD RID: 29117 RVA: 0x00250038 File Offset: 0x0024E238
		private void OnWaterSurfaceEnter(ref WaterOverlappingCollider persistentCollider)
		{
			WaterVolume.WaterVolumeEvent colliderEnteredWater = this.ColliderEnteredWater;
			if (colliderEnteredWater != null)
			{
				colliderEnteredWater(this, persistentCollider.collider);
			}
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnEnterWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				Vector3 colliderVelocity = this.GetColliderVelocity(ref persistentCollider);
				bool flag = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = Vector3.Dot(colliderVelocity, -persistentCollider.lastSurfaceQuery.surfaceNormal) > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, true, this);
				}
			}
		}

		// Token: 0x060071BE RID: 29118 RVA: 0x00250184 File Offset: 0x0024E384
		private void OnWaterSurfaceExit(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			WaterVolume.WaterVolumeEvent colliderExitedWater = this.ColliderExitedWater;
			if (colliderExitedWater != null)
			{
				colliderExitedWater(this, persistentCollider.collider);
			}
			persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
			GTPlayer instance = GTPlayer.Instance;
			if (persistentCollider.collider == instance.headCollider || persistentCollider.collider == instance.bodyCollider)
			{
				instance.OnExitWaterVolume(persistentCollider.collider, this);
			}
			if (this.HasOwnershipOfCollider(persistentCollider.collider))
			{
				float num = Vector3.Dot(this.GetColliderVelocity(ref persistentCollider), persistentCollider.lastSurfaceQuery.surfaceNormal);
				bool flag = num > this.waterParams.splashSpeedRequirement * persistentCollider.scaleMultiplier;
				bool flag2 = num > this.waterParams.bigSplashSpeedRequirement * persistentCollider.scaleMultiplier;
				persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, Time.time, this);
				if (this.waterParams.playSplashEffect && flag && (flag2 || !persistentCollider.playBigSplash))
				{
					persistentCollider.PlaySplashEffect(this.waterParams.splashEffect, persistentCollider.lastRipplePosition, this.waterParams.splashEffectScale, persistentCollider.playBigSplash && flag2, false, this);
				}
			}
		}

		// Token: 0x060071BF RID: 29119 RVA: 0x002502F0 File Offset: 0x0024E4F0
		private void ColliderOutOfWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			if (currentTime < persistentCollider.lastInWaterTime + this.waterParams.postExitDripDuration && currentTime > persistentCollider.nextDripTime && persistentCollider.playDripEffect)
			{
				persistentCollider.nextDripTime = currentTime + this.waterParams.perDripTimeDelay + Random.Range(-this.waterParams.perDripTimeRandRange * 0.5f, this.waterParams.perDripTimeRandRange * 0.5f);
				float num = this.waterParams.rippleEffectScale * 2f * (this.waterParams.perDripDefaultRadius + Random.Range(-this.waterParams.perDripRadiusRandRange * 0.5f, this.waterParams.perDripRadiusRandRange * 0.5f));
				persistentCollider.PlayDripEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, num);
			}
		}

		// Token: 0x060071C0 RID: 29120 RVA: 0x002503D8 File Offset: 0x0024E5D8
		private void ColliderInWaterUpdate(ref WaterOverlappingCollider persistentCollider, float currentTime)
		{
			Vector3 vector = Vector3.ProjectOnPlane(persistentCollider.collider.transform.position - persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal) + persistentCollider.lastSurfaceQuery.surfacePoint;
			bool flag;
			if (persistentCollider.overrideBoundingRadius)
			{
				flag = (persistentCollider.collider.transform.position - vector).sqrMagnitude < persistentCollider.boundingRadiusOverride * persistentCollider.boundingRadiusOverride;
			}
			else
			{
				flag = (persistentCollider.collider.ClosestPointOnBounds(vector) - vector).sqrMagnitude < 0.001f;
			}
			if (flag)
			{
				float num = Mathf.Max(this.waterParams.minDistanceBetweenRipples, this.waterParams.defaultDistanceBetweenRipples * (persistentCollider.lastRippleScale / this.waterParams.rippleEffectScale));
				bool flag2 = (persistentCollider.lastRipplePosition - vector).sqrMagnitude > num * num;
				bool flag3 = currentTime - persistentCollider.lastRippleTime > this.waterParams.minTimeBetweenRipples;
				if (flag2 || flag3)
				{
					persistentCollider.PlayRippleEffect(this.waterParams.rippleEffect, persistentCollider.lastSurfaceQuery.surfacePoint, persistentCollider.lastSurfaceQuery.surfaceNormal, this.waterParams.rippleEffectScale, currentTime, this);
					return;
				}
			}
			else
			{
				persistentCollider.lastRippleTime = currentTime;
			}
		}

		// Token: 0x060071C1 RID: 29121 RVA: 0x00250528 File Offset: 0x0024E728
		private void TryRegisterOwnershipOfCollider(Collider collider, bool isInWater, bool isSurfaceDetected)
		{
			WaterVolume waterVolume;
			if (WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume))
			{
				if (waterVolume != this)
				{
					bool flag;
					bool flag2;
					waterVolume.CheckColliderInVolume(collider, out flag, out flag2);
					if ((isSurfaceDetected && !flag2) || (isInWater && !flag))
					{
						WaterVolume.sharedColliderRegistry.Remove(collider);
						WaterVolume.sharedColliderRegistry.Add(collider, this);
						return;
					}
				}
			}
			else
			{
				WaterVolume.sharedColliderRegistry.Add(collider, this);
			}
		}

		// Token: 0x060071C2 RID: 29122 RVA: 0x0025058A File Offset: 0x0024E78A
		private void UnregisterOwnershipOfCollider(Collider collider)
		{
			if (WaterVolume.sharedColliderRegistry.ContainsKey(collider))
			{
				WaterVolume.sharedColliderRegistry.Remove(collider);
			}
		}

		// Token: 0x060071C3 RID: 29123 RVA: 0x002505A8 File Offset: 0x0024E7A8
		private bool HasOwnershipOfCollider(Collider collider)
		{
			WaterVolume waterVolume;
			return WaterVolume.sharedColliderRegistry.TryGetValue(collider, out waterVolume) && waterVolume == this;
		}

		// Token: 0x060071C4 RID: 29124 RVA: 0x002505D0 File Offset: 0x0024E7D0
		protected virtual bool CanPlayerSwim()
		{
			if (this.isMonkeblock && this.PlayerVRRig != null)
			{
				if (this.PlayerVRRig.scaleFactor < 0.5f)
				{
					return true;
				}
				BuilderTable builderTable;
				if (BuilderTable.TryGetBuilderTableForZone(this.PlayerVRRig.zoneEntity.currentZone, out builderTable))
				{
					return !builderTable.isTableMutable;
				}
			}
			return true;
		}

		// Token: 0x060071C5 RID: 29125 RVA: 0x0025062C File Offset: 0x0024E82C
		public void OnTriggerEnter(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderEnteredVolume = this.ColliderEnteredVolume;
			if (colliderEnteredVolume != null)
			{
				colliderEnteredVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = true;
					this.persistentColliders[i] = waterOverlappingCollider;
					return;
				}
			}
			WaterOverlappingCollider waterOverlappingCollider2 = new WaterOverlappingCollider
			{
				collider = other
			};
			waterOverlappingCollider2.inVolume = true;
			waterOverlappingCollider2.lastInWaterTime = Time.time - this.waterParams.postExitDripDuration - 10f;
			WaterSplashOverride component2 = other.GetComponent<WaterSplashOverride>();
			if (component2 != null)
			{
				if (component2.suppressWaterEffects)
				{
					return;
				}
				waterOverlappingCollider2.playBigSplash = component2.playBigSplash;
				waterOverlappingCollider2.playDripEffect = component2.playDrippingEffect;
				waterOverlappingCollider2.overrideBoundingRadius = component2.overrideBoundingRadius;
				waterOverlappingCollider2.boundingRadiusOverride = component2.boundingRadiusOverride;
				waterOverlappingCollider2.scaleMultiplier = (component2.scaleByPlayersScale ? GTPlayer.Instance.scale : 1f);
			}
			else
			{
				if (other.GetComponent<BuilderPieceCollider>() != null)
				{
					return;
				}
				waterOverlappingCollider2.playDripEffect = true;
				waterOverlappingCollider2.overrideBoundingRadius = false;
				waterOverlappingCollider2.scaleMultiplier = 1f;
				waterOverlappingCollider2.playBigSplash = false;
			}
			GTPlayer instance = GTPlayer.Instance;
			if (component != null)
			{
				waterOverlappingCollider2.velocityTracker = instance.GetHandVelocityTracker(component.isLeftHand);
				waterOverlappingCollider2.scaleMultiplier = instance.scale;
			}
			else
			{
				waterOverlappingCollider2.velocityTracker = other.GetComponent<GorillaVelocityTracker>();
			}
			if (this.PlayerVRRig != null && this.waterParams.sendSplashEffectRPCs && (component != null || waterOverlappingCollider2.collider == instance.headCollider || waterOverlappingCollider2.collider == instance.bodyCollider))
			{
				waterOverlappingCollider2.photonViewForRPC = this.PlayerVRRig.netView;
			}
			this.persistentColliders.Add(waterOverlappingCollider2);
		}

		// Token: 0x060071C6 RID: 29126 RVA: 0x0025084C File Offset: 0x0024EA4C
		private void OnTriggerExit(Collider other)
		{
			if (!this.CanPlayerSwim())
			{
				return;
			}
			GorillaTriggerColliderHandIndicator component = other.GetComponent<GorillaTriggerColliderHandIndicator>();
			if (other.isTrigger && component == null)
			{
				return;
			}
			WaterVolume.WaterVolumeEvent colliderExitedVolume = this.ColliderExitedVolume;
			if (colliderExitedVolume != null)
			{
				colliderExitedVolume(this, other);
			}
			for (int i = 0; i < this.persistentColliders.Count; i++)
			{
				if (this.persistentColliders[i].collider == other)
				{
					WaterOverlappingCollider waterOverlappingCollider = this.persistentColliders[i];
					waterOverlappingCollider.inVolume = false;
					this.persistentColliders[i] = waterOverlappingCollider;
				}
			}
		}

		// Token: 0x060071C7 RID: 29127 RVA: 0x002508DF File Offset: 0x0024EADF
		public void SetPropertiesFromPlaceholder(WaterVolumeProperties properties, List<Collider> waterVolumeColliders, WaterParameters parameters)
		{
			this.surfacePlane = properties.surfacePlane;
			this.surfaceColliders = properties.surfaceColliders;
			this.volumeColliders = waterVolumeColliders;
			this.liquidType = (GTPlayer.LiquidType)Math.Clamp(properties.liquidType - CMSZoneShaderSettings.EZoneLiquidType.Water, 0, 1);
			this.waterParams = parameters;
		}

		// Token: 0x04008271 RID: 33393
		[SerializeField]
		public Transform surfacePlane;

		// Token: 0x04008272 RID: 33394
		[SerializeField]
		private List<MeshCollider> surfaceColliders = new List<MeshCollider>();

		// Token: 0x04008273 RID: 33395
		[SerializeField]
		public List<Collider> volumeColliders = new List<Collider>();

		// Token: 0x04008274 RID: 33396
		[SerializeField]
		private GTPlayer.LiquidType liquidType;

		// Token: 0x04008275 RID: 33397
		[SerializeField]
		private WaterCurrent waterCurrent;

		// Token: 0x04008276 RID: 33398
		[SerializeField]
		private WaterParameters waterParams;

		// Token: 0x04008277 RID: 33399
		[SerializeField]
		[Tooltip("The water volume be placed in the scene (not spawned) and not moved for this to be true")]
		public bool isStationary = true;

		// Token: 0x04008278 RID: 33400
		[SerializeField]
		[Tooltip("Check scale of monke entering")]
		public bool isMonkeblock;

		// Token: 0x0400827A RID: 33402
		public const string WaterSplashRPC = "RPC_PlaySplashEffect";

		// Token: 0x0400827B RID: 33403
		public static float[] splashRPCSendTimes = new float[4];

		// Token: 0x0400827C RID: 33404
		private static Dictionary<Collider, WaterVolume> sharedColliderRegistry = new Dictionary<Collider, WaterVolume>(16);

		// Token: 0x0400827D RID: 33405
		private static Dictionary<Mesh, int[]> meshTrianglesDict = new Dictionary<Mesh, int[]>(16);

		// Token: 0x0400827E RID: 33406
		private static Dictionary<Mesh, Vector3[]> meshVertsDict = new Dictionary<Mesh, Vector3[]>(16);

		// Token: 0x0400827F RID: 33407
		private int[] sharedMeshTris;

		// Token: 0x04008280 RID: 33408
		private Vector3[] sharedMeshVerts;

		// Token: 0x04008285 RID: 33413
		private VRRig playerVRRig;

		// Token: 0x04008286 RID: 33414
		private float volumeMaxHeight;

		// Token: 0x04008287 RID: 33415
		private float volumeMinHeight;

		// Token: 0x04008288 RID: 33416
		private bool debugDrawSurfaceCast;

		// Token: 0x04008289 RID: 33417
		private Collider triggerCollider;

		// Token: 0x0400828A RID: 33418
		private List<WaterOverlappingCollider> persistentColliders = new List<WaterOverlappingCollider>(16);

		// Token: 0x0400828B RID: 33419
		private GuidedRefTargetIdSO _guidedRefTargetId;

		// Token: 0x0400828C RID: 33420
		private Object _guidedRefTargetObject;

		// Token: 0x0200119E RID: 4510
		public struct SurfaceQuery
		{
			// Token: 0x17000B28 RID: 2856
			// (get) Token: 0x060071CA RID: 29130 RVA: 0x0025097F File Offset: 0x0024EB7F
			public Plane surfacePlane
			{
				get
				{
					return new Plane(this.surfaceNormal, this.surfacePoint);
				}
			}

			// Token: 0x0400828D RID: 33421
			public Vector3 surfacePoint;

			// Token: 0x0400828E RID: 33422
			public Vector3 surfaceNormal;

			// Token: 0x0400828F RID: 33423
			public float maxDepth;
		}

		// Token: 0x0200119F RID: 4511
		// (Invoke) Token: 0x060071CC RID: 29132
		public delegate void WaterVolumeEvent(WaterVolume volume, Collider collider);
	}
}
