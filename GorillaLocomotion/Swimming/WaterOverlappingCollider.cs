using System;
using GorillaLocomotion.Climbing;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x0200119A RID: 4506
	public struct WaterOverlappingCollider
	{
		// Token: 0x0600719B RID: 29083 RVA: 0x0024EBD0 File Offset: 0x0024CDD0
		public void PlayRippleEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float defaultRippleScale, float currentTime, WaterVolume volume)
		{
			this.lastRipplePosition = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			this.lastBoundingRadius = this.GetBoundingRadiusOnSurface(surfaceNormal);
			this.lastRippleScale = defaultRippleScale * this.lastBoundingRadius * 2f * this.scaleMultiplier;
			this.lastRippleTime = currentTime;
			ObjectPools.instance.Instantiate(rippleEffectPrefab, this.lastRipplePosition, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), this.lastRippleScale, true).GetComponent<WaterRippleEffect>().PlayEffect(volume);
		}

		// Token: 0x0600719C RID: 29084 RVA: 0x0024EC6C File Offset: 0x0024CE6C
		public void PlaySplashEffect(GameObject splashEffectPrefab, Vector3 splashPosition, float splashScale, bool bigSplash, bool enteringWater, WaterVolume volume)
		{
			Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right);
			ObjectPools.instance.Instantiate(splashEffectPrefab, splashPosition, quaternion, splashScale * this.scaleMultiplier, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, this.scaleMultiplier, volume);
			if (this.photonViewForRPC != null)
			{
				float time = Time.time;
				int num = -1;
				float num2 = time + 10f;
				for (int i = 0; i < WaterVolume.splashRPCSendTimes.Length; i++)
				{
					if (WaterVolume.splashRPCSendTimes[i] < num2)
					{
						num2 = WaterVolume.splashRPCSendTimes[i];
						num = i;
					}
				}
				if (time - 0.5f > num2)
				{
					WaterVolume.splashRPCSendTimes[num] = time;
					this.photonViewForRPC.SendRPC("RPC_PlaySplashEffect", RpcTarget.Others, new object[]
					{
						splashPosition,
						quaternion,
						splashScale * this.scaleMultiplier,
						this.lastBoundingRadius,
						bigSplash,
						enteringWater
					});
				}
			}
		}

		// Token: 0x0600719D RID: 29085 RVA: 0x0024ED8C File Offset: 0x0024CF8C
		public void PlayDripEffect(GameObject rippleEffectPrefab, Vector3 surfacePoint, Vector3 surfaceNormal, float dripScale)
		{
			Vector3 closestPositionOnSurface = this.GetClosestPositionOnSurface(surfacePoint, surfaceNormal);
			float num = (this.overrideBoundingRadius ? this.boundingRadiusOverride : this.lastBoundingRadius);
			Vector3 vector = Vector3.ProjectOnPlane(Random.onUnitSphere * num * 0.5f, surfaceNormal);
			ObjectPools.instance.Instantiate(rippleEffectPrefab, closestPositionOnSurface + vector, Quaternion.FromToRotation(Vector3.up, this.lastSurfaceQuery.surfaceNormal) * Quaternion.AngleAxis(-90f, Vector3.right), dripScale * this.scaleMultiplier, true);
		}

		// Token: 0x0600719E RID: 29086 RVA: 0x0024EE1B File Offset: 0x0024D01B
		public Vector3 GetClosestPositionOnSurface(Vector3 surfacePoint, Vector3 surfaceNormal)
		{
			return Vector3.ProjectOnPlane(this.collider.transform.position - surfacePoint, surfaceNormal) + surfacePoint;
		}

		// Token: 0x0600719F RID: 29087 RVA: 0x0024EE40 File Offset: 0x0024D040
		private float GetBoundingRadiusOnSurface(Vector3 surfaceNormal)
		{
			if (this.overrideBoundingRadius)
			{
				this.lastBoundingRadius = this.boundingRadiusOverride;
				return this.boundingRadiusOverride;
			}
			Vector3 extents = this.collider.bounds.extents;
			Vector3 vector = Vector3.ProjectOnPlane(this.collider.transform.right * extents.x, surfaceNormal);
			Vector3 vector2 = Vector3.ProjectOnPlane(this.collider.transform.up * extents.y, surfaceNormal);
			Vector3 vector3 = Vector3.ProjectOnPlane(this.collider.transform.forward * extents.z, surfaceNormal);
			float sqrMagnitude = vector.sqrMagnitude;
			float sqrMagnitude2 = vector2.sqrMagnitude;
			float sqrMagnitude3 = vector3.sqrMagnitude;
			if (sqrMagnitude >= sqrMagnitude2 && sqrMagnitude >= sqrMagnitude3)
			{
				return vector.magnitude;
			}
			if (sqrMagnitude2 >= sqrMagnitude && sqrMagnitude2 >= sqrMagnitude3)
			{
				return vector2.magnitude;
			}
			return vector3.magnitude;
		}

		// Token: 0x04008243 RID: 33347
		public bool playBigSplash;

		// Token: 0x04008244 RID: 33348
		public bool playDripEffect;

		// Token: 0x04008245 RID: 33349
		public bool overrideBoundingRadius;

		// Token: 0x04008246 RID: 33350
		public float boundingRadiusOverride;

		// Token: 0x04008247 RID: 33351
		public float scaleMultiplier;

		// Token: 0x04008248 RID: 33352
		public Collider collider;

		// Token: 0x04008249 RID: 33353
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x0400824A RID: 33354
		public WaterVolume.SurfaceQuery lastSurfaceQuery;

		// Token: 0x0400824B RID: 33355
		public NetworkView photonViewForRPC;

		// Token: 0x0400824C RID: 33356
		public bool surfaceDetected;

		// Token: 0x0400824D RID: 33357
		public bool inWater;

		// Token: 0x0400824E RID: 33358
		public bool inVolume;

		// Token: 0x0400824F RID: 33359
		public float lastBoundingRadius;

		// Token: 0x04008250 RID: 33360
		public Vector3 lastRipplePosition;

		// Token: 0x04008251 RID: 33361
		public float lastRippleScale;

		// Token: 0x04008252 RID: 33362
		public float lastRippleTime;

		// Token: 0x04008253 RID: 33363
		public float lastInWaterTime;

		// Token: 0x04008254 RID: 33364
		public float nextDripTime;
	}
}
