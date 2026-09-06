using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x020011BB RID: 4539
	public class GorillaVelocityTracker : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x17000B37 RID: 2871
		// (get) Token: 0x0600727D RID: 29309 RVA: 0x00254F84 File Offset: 0x00253184
		// (set) Token: 0x0600727E RID: 29310 RVA: 0x00254F8C File Offset: 0x0025318C
		public bool TickRunning { get; set; }

		// Token: 0x0600727F RID: 29311 RVA: 0x00254F98 File Offset: 0x00253198
		public void ResetState()
		{
			this.trans = base.transform;
			this.localSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.localSpaceData);
			this.worldSpaceData = new GorillaVelocityTracker.VelocityDataPoint[this.maxDataPoints];
			this.<ResetState>g__PopulateArray|20_0(this.worldSpaceData);
			this.isRelativeTo = this.relativeTo != null;
			this.lastLocalSpacePos = this.GetPosition(false);
			this.lastWorldSpacePos = this.GetPosition(true);
			this.wasAboveThreshold = false;
		}

		// Token: 0x06007280 RID: 29312 RVA: 0x0025501E File Offset: 0x0025321E
		private void Awake()
		{
			this.ResetState();
		}

		// Token: 0x06007281 RID: 29313 RVA: 0x0001A297 File Offset: 0x00018497
		private void OnEnable()
		{
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06007282 RID: 29314 RVA: 0x00255026 File Offset: 0x00253226
		private void OnDisable()
		{
			this.ResetState();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007283 RID: 29315 RVA: 0x00255034 File Offset: 0x00253234
		public void SetRelativeTo(Transform tf)
		{
			this.relativeTo = tf;
			this.isRelativeTo = tf != null;
		}

		// Token: 0x06007284 RID: 29316 RVA: 0x0025504A File Offset: 0x0025324A
		private Vector3 GetPosition(bool worldSpace)
		{
			if (worldSpace)
			{
				return this.trans.position;
			}
			if (this.isRelativeTo)
			{
				return this.relativeTo.InverseTransformPoint(this.trans.position);
			}
			return this.trans.localPosition;
		}

		// Token: 0x06007285 RID: 29317 RVA: 0x00255088 File Offset: 0x00253288
		public void Tick()
		{
			if (Time.frameCount <= this.lastTickedFrame)
			{
				return;
			}
			Vector3 position = this.GetPosition(false);
			Vector3 position2 = this.GetPosition(true);
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = this.localSpaceData[this.currentDataPointIndex];
			velocityDataPoint.delta = (position - this.lastLocalSpacePos) / Time.deltaTime;
			velocityDataPoint.time = Time.time;
			this.localSpaceData[this.currentDataPointIndex] = velocityDataPoint;
			GorillaVelocityTracker.VelocityDataPoint velocityDataPoint2 = this.worldSpaceData[this.currentDataPointIndex];
			velocityDataPoint2.delta = (position2 - this.lastWorldSpacePos) / Time.deltaTime;
			velocityDataPoint2.time = Time.time;
			this.worldSpaceData[this.currentDataPointIndex] = velocityDataPoint2;
			this.lastLocalSpacePos = position;
			this.lastWorldSpacePos = position2;
			this.currentDataPointIndex++;
			if (this.currentDataPointIndex >= this.maxDataPoints)
			{
				this.currentDataPointIndex = 0;
			}
			if (this.useVelocityEvents)
			{
				this.GetLatestVelocity(this.useWorldSpaceForEvents);
			}
			this.lastTickedFrame = Time.frameCount;
		}

		// Token: 0x06007286 RID: 29318 RVA: 0x0025518A File Offset: 0x0025338A
		private void AddToQueue(ref List<GorillaVelocityTracker.VelocityDataPoint> dataPoints, GorillaVelocityTracker.VelocityDataPoint newData)
		{
			dataPoints.Add(newData);
			if (dataPoints.Count >= this.maxDataPoints)
			{
				dataPoints.RemoveAt(0);
			}
		}

		// Token: 0x06007287 RID: 29319 RVA: 0x002551AC File Offset: 0x002533AC
		public Vector3 GetAverageVelocity(bool worldSpace = false, float maxTimeFromPast = 0.15f, bool doMagnitudeCheck = false)
		{
			float num = maxTimeFromPast / 2f;
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return Vector3.zero;
			}
			GorillaVelocityTracker.<>c__DisplayClass28_0 CS$<>8__locals1;
			CS$<>8__locals1.total = Vector3.zero;
			CS$<>8__locals1.totalMag = 0f;
			CS$<>8__locals1.added = 0;
			float num2 = Time.time - maxTimeFromPast;
			float num3 = Time.time - num;
			int i = 0;
			int num4 = this.currentDataPointIndex;
			while (i < this.maxDataPoints)
			{
				GorillaVelocityTracker.VelocityDataPoint velocityDataPoint = array[num4];
				if (doMagnitudeCheck && CS$<>8__locals1.added > 1 && velocityDataPoint.time >= num3)
				{
					if (velocityDataPoint.delta.magnitude >= CS$<>8__locals1.totalMag / (float)CS$<>8__locals1.added)
					{
						GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
					}
				}
				else if (velocityDataPoint.time >= num2)
				{
					GorillaVelocityTracker.<GetAverageVelocity>g__AddPoint|28_0(velocityDataPoint, ref CS$<>8__locals1);
				}
				num4++;
				if (num4 >= this.maxDataPoints)
				{
					num4 = 0;
				}
				i++;
			}
			if (CS$<>8__locals1.added > 0)
			{
				return CS$<>8__locals1.total / (float)CS$<>8__locals1.added;
			}
			return Vector3.zero;
		}

		// Token: 0x06007288 RID: 29320 RVA: 0x002552BC File Offset: 0x002534BC
		public Vector3 GetLatestVelocity(bool worldSpace = false)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array[this.currentDataPointIndex].delta.magnitude >= this.latestVelocityThreshold && !this.wasAboveThreshold)
			{
				UnityEvent onLatestAboveThreshold = this.OnLatestAboveThreshold;
				if (onLatestAboveThreshold != null)
				{
					onLatestAboveThreshold.Invoke();
				}
				this.wasAboveThreshold = true;
			}
			else if (array[this.currentDataPointIndex].delta.magnitude < this.latestVelocityThreshold && this.wasAboveThreshold)
			{
				UnityEvent onLatestBelowThreshold = this.OnLatestBelowThreshold;
				if (onLatestBelowThreshold != null)
				{
					onLatestBelowThreshold.Invoke();
				}
				this.wasAboveThreshold = false;
			}
			return array[this.currentDataPointIndex].delta;
		}

		// Token: 0x06007289 RID: 29321 RVA: 0x00255360 File Offset: 0x00253560
		public float GetAverageSpeedChangeMagnitudeInDirection(Vector3 dir, bool worldSpace = false, float maxTimeFromPast = 0.05f)
		{
			GorillaVelocityTracker.VelocityDataPoint[] array;
			if (worldSpace)
			{
				array = this.worldSpaceData;
			}
			else
			{
				array = this.localSpaceData;
			}
			if (array.Length <= 1)
			{
				return 0f;
			}
			float num = 0f;
			int num2 = 0;
			float num3 = Time.time - maxTimeFromPast;
			bool flag = false;
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].time >= num3)
				{
					if (!flag)
					{
						vector = array[i].delta;
						flag = true;
					}
					else
					{
						num += Mathf.Abs(Vector3.Dot(array[i].delta - vector, dir));
						num2++;
					}
				}
			}
			if (num2 <= 0)
			{
				return 0f;
			}
			return num / (float)num2;
		}

		// Token: 0x0600728B RID: 29323 RVA: 0x00255420 File Offset: 0x00253620
		[CompilerGenerated]
		private void <ResetState>g__PopulateArray|20_0(GorillaVelocityTracker.VelocityDataPoint[] array)
		{
			for (int i = 0; i < this.maxDataPoints; i++)
			{
				array[i] = new GorillaVelocityTracker.VelocityDataPoint();
			}
		}

		// Token: 0x0600728C RID: 29324 RVA: 0x00255448 File Offset: 0x00253648
		[CompilerGenerated]
		internal static void <GetAverageVelocity>g__AddPoint|28_0(GorillaVelocityTracker.VelocityDataPoint point, ref GorillaVelocityTracker.<>c__DisplayClass28_0 A_1)
		{
			A_1.total += point.delta;
			A_1.totalMag += point.delta.magnitude;
			int added = A_1.added;
			A_1.added = added + 1;
		}

		// Token: 0x0400835C RID: 33628
		[SerializeField]
		private int maxDataPoints = 20;

		// Token: 0x0400835D RID: 33629
		[SerializeField]
		private Transform relativeTo;

		// Token: 0x0400835E RID: 33630
		[Tooltip("Use in Editor to trigger events when above or higher than a desired latest velocity.")]
		[SerializeField]
		private bool useVelocityEvents;

		// Token: 0x0400835F RID: 33631
		[SerializeField]
		private float latestVelocityThreshold;

		// Token: 0x04008360 RID: 33632
		public UnityEvent OnLatestBelowThreshold;

		// Token: 0x04008361 RID: 33633
		public UnityEvent OnLatestAboveThreshold;

		// Token: 0x04008362 RID: 33634
		[SerializeField]
		private bool useWorldSpaceForEvents;

		// Token: 0x04008363 RID: 33635
		private bool wasAboveThreshold;

		// Token: 0x04008364 RID: 33636
		private int currentDataPointIndex;

		// Token: 0x04008365 RID: 33637
		private GorillaVelocityTracker.VelocityDataPoint[] localSpaceData;

		// Token: 0x04008366 RID: 33638
		private GorillaVelocityTracker.VelocityDataPoint[] worldSpaceData;

		// Token: 0x04008367 RID: 33639
		private Transform trans;

		// Token: 0x04008368 RID: 33640
		private Vector3 lastWorldSpacePos;

		// Token: 0x04008369 RID: 33641
		private Vector3 lastLocalSpacePos;

		// Token: 0x0400836A RID: 33642
		private bool isRelativeTo;

		// Token: 0x0400836B RID: 33643
		private int lastTickedFrame = -1;

		// Token: 0x020011BC RID: 4540
		public class VelocityDataPoint
		{
			// Token: 0x0400836D RID: 33645
			public Vector3 delta;

			// Token: 0x0400836E RID: 33646
			public float time = -1f;
		}
	}
}
