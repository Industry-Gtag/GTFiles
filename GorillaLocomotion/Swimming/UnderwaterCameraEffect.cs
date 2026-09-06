using System;
using CjLib;
using GorillaTag;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x02001196 RID: 4502
	[ExecuteAlways]
	public class UnderwaterCameraEffect : MonoBehaviour
	{
		// Token: 0x06007184 RID: 29060 RVA: 0x0024D7B4 File Offset: 0x0024B9B4
		private void SetOffScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 0f, 1f);
			base.transform.localPosition = new Vector3(0f, -(this.frustumPlaneExtents.y + 0.04f), this.distanceFromCamera);
		}

		// Token: 0x06007185 RID: 29061 RVA: 0x0024D820 File Offset: 0x0024BA20
		private void SetFullScreenPosition()
		{
			base.transform.localScale = new Vector3(2f * (this.frustumPlaneExtents.x + 0.04f), 2f * (this.frustumPlaneExtents.y + 0.04f), 1f);
			base.transform.localPosition = new Vector3(0f, 0f, this.distanceFromCamera);
		}

		// Token: 0x06007186 RID: 29062 RVA: 0x0024D890 File Offset: 0x0024BA90
		private void OnEnable()
		{
			if (this.targetCamera == null)
			{
				this.targetCamera = Camera.main;
			}
			this.hasTargetCamera = this.targetCamera != null;
			this.InitializeShaderProperties();
		}

		// Token: 0x06007187 RID: 29063 RVA: 0x0024D8C4 File Offset: 0x0024BAC4
		private void Start()
		{
			this.player = GTPlayer.Instance;
			this.cachedAspectRatio = this.targetCamera.aspect;
			this.cachedFov = this.targetCamera.fieldOfView;
			this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			this.SetOffScreenPosition();
		}

		// Token: 0x06007188 RID: 29064 RVA: 0x0024D918 File Offset: 0x0024BB18
		private void LateUpdate()
		{
			if (!this.hasTargetCamera)
			{
				return;
			}
			if (!this.player)
			{
				return;
			}
			if (this.player.HeadOverlappingWaterVolumes.Count < 1)
			{
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				if (this.planeRenderer.enabled)
				{
					this.planeRenderer.enabled = false;
					this.SetOffScreenPosition();
				}
				if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
				{
					this.underwaterParticleEffect.UpdateParticleEffect(false, ref this.waterSurface);
				}
				return;
			}
			if (this.targetCamera.aspect != this.cachedAspectRatio || this.targetCamera.fieldOfView != this.cachedFov)
			{
				this.cachedAspectRatio = this.targetCamera.aspect;
				this.cachedFov = this.targetCamera.fieldOfView;
				this.CalculateFrustumPlaneBounds(this.cachedFov, this.cachedAspectRatio);
			}
			bool flag = false;
			float num = float.MinValue;
			Vector3 position = this.targetCamera.transform.position;
			for (int i = 0; i < this.player.HeadOverlappingWaterVolumes.Count; i++)
			{
				WaterVolume.SurfaceQuery surfaceQuery;
				if (!(this.player.HeadOverlappingWaterVolumes[i] == null) && this.player.HeadOverlappingWaterVolumes[i].GetSurfaceQueryForPoint(position, out surfaceQuery, false))
				{
					WaterParameters parameters = this.player.HeadOverlappingWaterVolumes[i].Parameters;
					if (parameters == null || parameters.allowBubblesInVolume)
					{
						float num2 = Vector3.Dot(surfaceQuery.surfacePoint - position, surfaceQuery.surfaceNormal);
						if (num2 > num)
						{
							flag = true;
							num = num2;
							this.waterSurface = surfaceQuery;
						}
					}
				}
			}
			if (flag)
			{
				Vector3 vector = this.targetCamera.transform.InverseTransformPoint(this.waterSurface.surfacePoint);
				Vector3 vector2 = this.targetCamera.transform.InverseTransformDirection(this.waterSurface.surfaceNormal);
				Plane plane = new Plane(vector2, vector);
				Plane plane2 = new Plane(Vector3.forward, -this.distanceFromCamera);
				Vector3 vector3;
				Vector3 vector4;
				if (this.IntersectPlanes(plane2, plane, out vector3, out vector4))
				{
					Vector3 normalized = Vector3.Cross(vector4, Vector3.forward).normalized;
					float num3 = Vector3.Dot(new Vector3(vector3.x, vector3.y, 0f), normalized);
					if (num3 > this.frustumPlaneExtents.y + 0.04f)
					{
						this.SetFullScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
					}
					else if (num3 < -(this.frustumPlaneExtents.y + 0.04f))
					{
						this.SetOffScreenPosition();
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
					}
					else
					{
						float num4 = num3;
						num4 += this.GetFrustumCoverageDistance(-normalized) + 0.04f;
						float num5 = this.GetFrustumCoverageDistance(vector4) + 0.04f;
						num5 += this.GetFrustumCoverageDistance(-vector4) + 0.04f;
						base.transform.localScale = new Vector3(num5, num4, 1f);
						base.transform.localPosition = normalized * (num3 - num4 * 0.5f) + new Vector3(0f, 0f, this.distanceFromCamera);
						float num6 = Vector3.SignedAngle(Vector3.up, normalized, Vector3.forward);
						base.transform.localRotation = Quaternion.AngleAxis(num6, Vector3.forward);
						this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged);
					}
					if (this.debugDraw)
					{
						Vector3 vector5 = this.targetCamera.transform.TransformPoint(vector3);
						Vector3 vector6 = this.targetCamera.transform.TransformDirection(vector4);
						DebugUtil.DrawLine(vector5 - 2f * this.frustumPlaneExtents.x * vector6, vector5 + 2f * this.frustumPlaneExtents.x * vector6, Color.white, false);
					}
				}
				else if (new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint).GetSide(this.targetCamera.transform.position))
				{
					this.SetFullScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged);
				}
				else
				{
					this.SetOffScreenPosition();
					this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
				}
			}
			else
			{
				this.SetOffScreenPosition();
				this.SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater);
			}
			if (this.underwaterParticleEffect != null && this.underwaterParticleEffect.gameObject.activeInHierarchy)
			{
				this.underwaterParticleEffect.UpdateParticleEffect(flag, ref this.waterSurface);
			}
		}

		// Token: 0x06007189 RID: 29065 RVA: 0x0024DD90 File Offset: 0x0024BF90
		[DebugOption]
		private void InitializeShaderProperties()
		{
			Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
			Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
			float num = -Vector3.Dot(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
			Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(this.waterSurface.surfaceNormal.x, this.waterSurface.surfaceNormal.y, this.waterSurface.surfaceNormal.z, num));
		}

		// Token: 0x0600718A RID: 29066 RVA: 0x0024DE10 File Offset: 0x0024C010
		private void SetCameraOverlapState(UnderwaterCameraEffect.CameraOverlapWaterState state)
		{
			if (state != this.cameraOverlapWaterState || state == UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized)
			{
				this.cameraOverlapWaterState = state;
				switch (this.cameraOverlapWaterState)
				{
				case UnderwaterCameraEffect.CameraOverlapWaterState.Uninitialized:
				case UnderwaterCameraEffect.CameraOverlapWaterState.OutOfWater:
					Shader.DisableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.DisableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				case UnderwaterCameraEffect.CameraOverlapWaterState.FullySubmerged:
					Shader.EnableKeyword("_GLOBAL_CAMERA_TOUCHING_WATER");
					Shader.EnableKeyword("_GLOBAL_CAMERA_FULLY_UNDERWATER");
					break;
				}
			}
			if (this.cameraOverlapWaterState == UnderwaterCameraEffect.CameraOverlapWaterState.PartiallySubmerged)
			{
				Plane plane = new Plane(this.waterSurface.surfaceNormal, this.waterSurface.surfacePoint);
				Shader.SetGlobalVector(this.shaderParam_GlobalCameraOverlapWaterSurfacePlane, new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance));
			}
		}

		// Token: 0x0600718B RID: 29067 RVA: 0x0024DEF0 File Offset: 0x0024C0F0
		private void CalculateFrustumPlaneBounds(float fieldOfView, float aspectRatio)
		{
			float num = Mathf.Tan(0.017453292f * fieldOfView * 0.5f) * this.distanceFromCamera;
			float num2 = aspectRatio * num + 0.04f;
			float num3 = 1f / aspectRatio * num + 0.04f;
			this.frustumPlaneExtents = new Vector2(num2, num3);
			this.frustumPlaneCornersLocal[0] = new Vector3(-num2, -num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[1] = new Vector3(-num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[2] = new Vector3(num2, num3, this.distanceFromCamera);
			this.frustumPlaneCornersLocal[3] = new Vector3(num2, -num3, this.distanceFromCamera);
		}

		// Token: 0x0600718C RID: 29068 RVA: 0x0024DFA8 File Offset: 0x0024C1A8
		private bool IntersectPlanes(Plane p1, Plane p2, out Vector3 point, out Vector3 direction)
		{
			direction = Vector3.Cross(p1.normal, p2.normal);
			float num = Vector3.Dot(direction, direction);
			if (num < Mathf.Epsilon)
			{
				point = Vector3.zero;
				return false;
			}
			point = Vector3.Cross(direction, p1.distance * p2.normal - p2.distance * p1.normal) / num;
			return true;
		}

		// Token: 0x0600718D RID: 29069 RVA: 0x0024E03C File Offset: 0x0024C23C
		private float GetFrustumCoverageDistance(Vector3 localDirection)
		{
			float num = float.MinValue;
			for (int i = 0; i < this.frustumPlaneCornersLocal.Length; i++)
			{
				float num2 = Vector3.Dot(this.frustumPlaneCornersLocal[i], localDirection);
				if (num2 > num)
				{
					num = num2;
				}
			}
			return num;
		}

		// Token: 0x04008217 RID: 33303
		private const float edgeBuffer = 0.04f;

		// Token: 0x04008218 RID: 33304
		[SerializeField]
		private Camera targetCamera;

		// Token: 0x04008219 RID: 33305
		[SerializeField]
		private MeshRenderer planeRenderer;

		// Token: 0x0400821A RID: 33306
		[SerializeField]
		private UnderwaterParticleEffects underwaterParticleEffect;

		// Token: 0x0400821B RID: 33307
		[SerializeField]
		private float distanceFromCamera = 0.02f;

		// Token: 0x0400821C RID: 33308
		[SerializeField]
		[DebugOption]
		private bool debugDraw;

		// Token: 0x0400821D RID: 33309
		private float cachedAspectRatio = 1f;

		// Token: 0x0400821E RID: 33310
		private float cachedFov = 90f;

		// Token: 0x0400821F RID: 33311
		private readonly Vector3[] frustumPlaneCornersLocal = new Vector3[4];

		// Token: 0x04008220 RID: 33312
		private Vector2 frustumPlaneExtents;

		// Token: 0x04008221 RID: 33313
		private GTPlayer player;

		// Token: 0x04008222 RID: 33314
		private WaterVolume.SurfaceQuery waterSurface;

		// Token: 0x04008223 RID: 33315
		private const string kShaderKeyword_GlobalCameraTouchingWater = "_GLOBAL_CAMERA_TOUCHING_WATER";

		// Token: 0x04008224 RID: 33316
		private const string kShaderKeyword_GlobalCameraFullyUnderwater = "_GLOBAL_CAMERA_FULLY_UNDERWATER";

		// Token: 0x04008225 RID: 33317
		private int shaderParam_GlobalCameraOverlapWaterSurfacePlane = Shader.PropertyToID("_GlobalCameraOverlapWaterSurfacePlane");

		// Token: 0x04008226 RID: 33318
		private bool hasTargetCamera;

		// Token: 0x04008227 RID: 33319
		[DebugReadout]
		private UnderwaterCameraEffect.CameraOverlapWaterState cameraOverlapWaterState;

		// Token: 0x02001197 RID: 4503
		private enum CameraOverlapWaterState
		{
			// Token: 0x04008229 RID: 33321
			Uninitialized,
			// Token: 0x0400822A RID: 33322
			OutOfWater,
			// Token: 0x0400822B RID: 33323
			PartiallySubmerged,
			// Token: 0x0400822C RID: 33324
			FullySubmerged
		}
	}
}
