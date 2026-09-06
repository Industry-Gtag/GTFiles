using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000220 RID: 544
public struct AnimatedBee
{
	// Token: 0x06000E51 RID: 3665 RVA: 0x0004E718 File Offset: 0x0004C918
	public void UpdateVisual(float syncTime, BeeSwarmManager manager)
	{
		if (this.destinationCache == null)
		{
			return;
		}
		syncTime %= this.loopDuration;
		if (syncTime < this.oldSyncTime)
		{
			this.InitRouteTimestamps();
		}
		Vector3 vector;
		Vector3 vector2;
		this.GetPositionAndDestinationAtTime(syncTime, out vector, out vector2);
		Vector3 vector3 = (vector2 - this.oldPosition).normalized * this.speed;
		this.velocity = Vector3.MoveTowards(this.velocity * manager.BeeJitterDamping, vector3, manager.BeeAcceleration * Time.deltaTime);
		if ((this.oldPosition - vector2).IsLongerThan(manager.BeeNearDestinationRadius))
		{
			this.velocity += Random.insideUnitSphere * manager.BeeJitterStrength * Time.deltaTime;
		}
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
			this.visual.transform.rotation = Quaternion.LookRotation(Vector3.up, vector4 - vector2);
		}
		this.oldPosition = vector4;
		this.oldSyncTime = syncTime;
	}

	// Token: 0x06000E52 RID: 3666 RVA: 0x0004E978 File Offset: 0x0004CB78
	public void GetPositionAndDestinationAtTime(float syncTime, out Vector3 idealPosition, out Vector3 destination)
	{
		if (syncTime > this.destinationB.syncEndTime || syncTime < this.destinationA.syncTime)
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
					idealPosition = this.destinationCache[num3].destination.GetPoint();
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
		destination = this.destinationB.destination.GetPoint();
		idealPosition = Vector3.Lerp(this.destinationA.destination.GetPoint(), destination, num4);
	}

	// Token: 0x06000E53 RID: 3667 RVA: 0x0004EA9A File Offset: 0x0004CC9A
	public void InitVisual(MeshRenderer prefab, BeeSwarmManager manager)
	{
		this.visual = Object.Instantiate<MeshRenderer>(prefab, manager.transform);
	}

	// Token: 0x06000E54 RID: 3668 RVA: 0x0004EAB0 File Offset: 0x0004CCB0
	public void InitRouteTimestamps()
	{
		this.destinationB.syncEndTime = -1f;
		this.destinationCache.Clear();
		this.destinationCache.Add(new AnimatedBee.TimedDestination
		{
			syncTime = 0f,
			destination = this.route[0]
		});
		float num = 0f;
		for (int i = 1; i < this.route.Count; i++)
		{
			if (this.route[i].enabled)
			{
				float num2 = (this.route[i].transform.position - this.route[i - 1].transform.position).magnitude * this.speed;
				num2 = Mathf.Min(num2, this.maxTravelTime);
				num += num2;
				float num3 = this.holdTimes[i];
				this.destinationCache.Add(new AnimatedBee.TimedDestination
				{
					syncTime = num,
					syncEndTime = num + num3,
					destination = this.route[i]
				});
				num += num3;
			}
		}
		num += Mathf.Min((this.route[0].transform.position - this.route[this.route.Count - 1].transform.position).magnitude * this.speed, this.maxTravelTime);
		float num4 = this.holdTimes[0];
		this.destinationCache.Add(new AnimatedBee.TimedDestination
		{
			syncTime = num,
			syncEndTime = num + num4,
			destination = this.route[0]
		});
	}

	// Token: 0x06000E55 RID: 3669 RVA: 0x0004EC8C File Offset: 0x0004CE8C
	public void InitRoute(List<BeePerchPoint> route, List<float> holdTimes, BeeSwarmManager manager)
	{
		this.route = route;
		this.holdTimes = holdTimes;
		this.speed = manager.BeeSpeed;
		this.maxTravelTime = manager.BeeMaxTravelTime;
		this.destinationCache = new List<AnimatedBee.TimedDestination>(route.Count + 1);
		float num = 0f;
		for (int i = 1; i < route.Count; i++)
		{
			num += (route[i].transform.position - route[i - 1].transform.position).magnitude * manager.BeeSpeed + holdTimes[i];
		}
		this.loopDuration = num + (route[0].transform.position - route[route.Count - 1].transform.position).magnitude * manager.BeeSpeed + holdTimes[0];
	}

	// Token: 0x0400113C RID: 4412
	private List<AnimatedBee.TimedDestination> destinationCache;

	// Token: 0x0400113D RID: 4413
	private AnimatedBee.TimedDestination destinationA;

	// Token: 0x0400113E RID: 4414
	private AnimatedBee.TimedDestination destinationB;

	// Token: 0x0400113F RID: 4415
	private float loopDuration;

	// Token: 0x04001140 RID: 4416
	private Vector3 oldPosition;

	// Token: 0x04001141 RID: 4417
	private Vector3 velocity;

	// Token: 0x04001142 RID: 4418
	public MeshRenderer visual;

	// Token: 0x04001143 RID: 4419
	private float oldSyncTime;

	// Token: 0x04001144 RID: 4420
	private List<BeePerchPoint> route;

	// Token: 0x04001145 RID: 4421
	private List<float> holdTimes;

	// Token: 0x04001146 RID: 4422
	private float speed;

	// Token: 0x04001147 RID: 4423
	private float maxTravelTime;

	// Token: 0x02000221 RID: 545
	private struct TimedDestination
	{
		// Token: 0x04001148 RID: 4424
		public float syncTime;

		// Token: 0x04001149 RID: 4425
		public float syncEndTime;

		// Token: 0x0400114A RID: 4426
		public BeePerchPoint destination;
	}
}
