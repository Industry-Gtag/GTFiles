using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000222 RID: 546
public struct AnimatedButterfly
{
	// Token: 0x06000E56 RID: 3670 RVA: 0x0004ED7C File Offset: 0x0004CF7C
	public void UpdateVisual(float syncTime, ButterflySwarmManager manager)
	{
		if (this.destinationCache == null)
		{
			return;
		}
		syncTime %= this.loopDuration;
		Vector3 vector;
		Vector3 vector2;
		this.GetPositionAndDestinationAtTime(syncTime, out vector, out vector2);
		Vector3 vector3 = (vector2 - this.oldPosition).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * manager.BeeJitterDamping, vector3, manager.BeeAcceleration * Time.deltaTime);
		float sqrMagnitude = (this.oldPosition - vector2).sqrMagnitude;
		if (sqrMagnitude < manager.BeeNearDestinationRadius * manager.BeeNearDestinationRadius)
		{
			this.visual.transform.position = Vector3.MoveTowards(this.visual.transform.position, vector2, Time.deltaTime);
			this.visual.transform.rotation = this.destinationB.destination.transform.rotation;
			if (sqrMagnitude < 1E-07f && !this.wasPerched)
			{
				this.material.SetFloat(ShaderProps._VertexFlapSpeed, manager.PerchedFlapSpeed);
				this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, manager.PerchedFlapPhase);
				this.wasPerched = true;
			}
		}
		else
		{
			if (this.wasPerched)
			{
				this.material.SetFloat(ShaderProps._VertexFlapSpeed, this.baseFlapSpeed);
				this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, 0f);
				this.wasPerched = false;
			}
			this.velocity += Random.insideUnitSphere * manager.BeeJitterStrength * Time.deltaTime;
			Vector3 vector4 = this.oldPosition + this.velocity * Time.deltaTime;
			if ((vector4 - vector).IsLongerThan(manager.BeeMaxJitterRadius))
			{
				vector4 = vector + (vector4 - vector).normalized * manager.BeeMaxJitterRadius;
				this.velocity = (vector4 - this.oldPosition) / Time.deltaTime;
			}
			foreach (GameObject gameObject in BeeSwarmManager.avoidPoints)
			{
				Vector3 position = gameObject.transform.position;
				if ((vector4 - position).IsShorterThan(manager.AvoidPointRadius))
				{
					Vector3 normalized = Vector3.Cross(position - vector4, vector2 - vector4).normalized;
					Vector3 normalized2 = (vector2 - position).normalized;
					float num = Vector3.Dot(vector4 - position, normalized);
					Vector3 vector5 = (manager.AvoidPointRadius - num) * normalized;
					vector4 += vector5;
					this.velocity += vector5;
				}
			}
			this.visual.transform.position = vector4;
			if ((vector2 - vector4).IsLongerThan(0.01f))
			{
				this.visual.transform.rotation = Quaternion.LookRotation(vector2 - vector4) * this.travellingLocalRotation;
			}
		}
		this.oldPosition = this.visual.transform.position;
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x0004F0D4 File Offset: 0x0004D2D4
	public void GetPositionAndDestinationAtTime(float syncTime, out Vector3 idealPosition, out Vector3 destination)
	{
		if (syncTime > this.destinationB.syncEndTime || syncTime < this.destinationA.syncTime || this.destinationA.destination == null || this.destinationB.destination == null)
		{
			int num = 0;
			int num2 = this.destinationCache.Count - 1;
			while (num + 1 < num2)
			{
				int num3 = (num + num2) / 2;
				float syncTime2 = this.destinationCache[num3].syncTime;
				float syncEndTime = this.destinationCache[num3].syncEndTime;
				if (syncTime2 <= syncTime && syncEndTime >= syncTime)
				{
					idealPosition = this.destinationCache[num3].destination.transform.position;
					destination = idealPosition;
				}
				if (syncEndTime < syncTime)
				{
					num = num3;
				}
				else
				{
					num2 = num3;
				}
			}
			this.destinationA = this.destinationCache[num];
			this.destinationB = this.destinationCache[num2];
		}
		float num4 = Mathf.InverseLerp(this.destinationA.syncEndTime, this.destinationB.syncTime, syncTime);
		destination = this.destinationB.destination.transform.position;
		idealPosition = Vector3.Lerp(this.destinationA.destination.transform.position, destination, num4);
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x0004F21F File Offset: 0x0004D41F
	public void InitVisual(MeshRenderer prefab, ButterflySwarmManager manager)
	{
		this.visual = Object.Instantiate<MeshRenderer>(prefab, manager.transform);
		this.material = this.visual.material;
		this.material.SetFloat(ShaderProps._VertexFlapPhaseOffset, 0f);
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0004F259 File Offset: 0x0004D459
	public void SetColor(Color color)
	{
		this.material.SetColor(ShaderProps._BaseColor, color);
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x0004F26C File Offset: 0x0004D46C
	public void SetFlapSpeed(float flapSpeed)
	{
		this.material.SetFloat(ShaderProps._VertexFlapSpeed, flapSpeed);
		this.baseFlapSpeed = flapSpeed;
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x0004F288 File Offset: 0x0004D488
	public void InitRoute(List<GameObject> route, List<float> holdTimes, ButterflySwarmManager manager)
	{
		this.speed = manager.BeeSpeed;
		this.maxTravelTime = manager.BeeMaxTravelTime;
		this.travellingLocalRotation = manager.TravellingLocalRotation;
		this.destinationCache = new List<AnimatedButterfly.TimedDestination>(route.Count + 1);
		this.destinationCache.Clear();
		this.destinationCache.Add(new AnimatedButterfly.TimedDestination
		{
			syncTime = 0f,
			syncEndTime = 0f,
			destination = route[0]
		});
		float num = 0f;
		for (int i = 1; i < route.Count; i++)
		{
			float num2 = (route[i].transform.position - route[i - 1].transform.position).magnitude / this.speed;
			num2 = Mathf.Min(num2, this.maxTravelTime);
			num += num2;
			float num3 = holdTimes[i];
			this.destinationCache.Add(new AnimatedButterfly.TimedDestination
			{
				syncTime = num,
				syncEndTime = num + num3,
				destination = route[i]
			});
			num += num3;
		}
		num += Mathf.Min((route[0].transform.position - route[route.Count - 1].transform.position).magnitude / this.speed, this.maxTravelTime);
		float num4 = holdTimes[0];
		this.destinationCache.Add(new AnimatedButterfly.TimedDestination
		{
			syncTime = num,
			syncEndTime = num + num4,
			destination = route[0]
		});
		this.loopDuration = num + (route[0].transform.position - route[route.Count - 1].transform.position).magnitude * manager.BeeSpeed + holdTimes[0];
	}

	// Token: 0x0400114B RID: 4427
	private List<AnimatedButterfly.TimedDestination> destinationCache;

	// Token: 0x0400114C RID: 4428
	private AnimatedButterfly.TimedDestination destinationA;

	// Token: 0x0400114D RID: 4429
	private AnimatedButterfly.TimedDestination destinationB;

	// Token: 0x0400114E RID: 4430
	private float loopDuration;

	// Token: 0x0400114F RID: 4431
	private Vector3 oldPosition;

	// Token: 0x04001150 RID: 4432
	private Vector3 velocity;

	// Token: 0x04001151 RID: 4433
	public MeshRenderer visual;

	// Token: 0x04001152 RID: 4434
	private Material material;

	// Token: 0x04001153 RID: 4435
	private float speed;

	// Token: 0x04001154 RID: 4436
	private float maxTravelTime;

	// Token: 0x04001155 RID: 4437
	private Quaternion travellingLocalRotation;

	// Token: 0x04001156 RID: 4438
	private float baseFlapSpeed;

	// Token: 0x04001157 RID: 4439
	private bool wasPerched;

	// Token: 0x02000223 RID: 547
	private struct TimedDestination
	{
		// Token: 0x04001158 RID: 4440
		public float syncTime;

		// Token: 0x04001159 RID: 4441
		public float syncEndTime;

		// Token: 0x0400115A RID: 4442
		public GameObject destination;
	}
}
