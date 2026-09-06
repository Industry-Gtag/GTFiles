using System;
using GorillaLocomotion.Climbing;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011A8 RID: 4520
	public class GorillaZipline : MonoBehaviour
	{
		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x06007200 RID: 29184 RVA: 0x00251A3E File Offset: 0x0024FC3E
		// (set) Token: 0x06007201 RID: 29185 RVA: 0x00251A46 File Offset: 0x0024FC46
		public float currentSpeed { get; private set; }

		// Token: 0x06007202 RID: 29186 RVA: 0x00251A50 File Offset: 0x0024FC50
		protected void FindTFromDistance(ref float t, float distance, int steps = 1000)
		{
			float num = distance / (float)steps;
			Vector3 vector = this.spline.GetPointLocal(t);
			float num2 = 0f;
			for (int i = 0; i < 1000; i++)
			{
				t += num;
				if (t >= 1f || t <= 0f)
				{
					break;
				}
				Vector3 pointLocal = this.spline.GetPointLocal(t);
				num2 += Vector3.Distance(pointLocal, vector);
				if (num2 >= Mathf.Abs(distance))
				{
					break;
				}
				vector = pointLocal;
			}
		}

		// Token: 0x06007203 RID: 29187 RVA: 0x00251AC4 File Offset: 0x0024FCC4
		private float FindSlideHelperSpot(Vector3 grabPoint)
		{
			int i = 0;
			int num = 200;
			float num2 = 0.001f;
			float num3 = 1f / (float)num;
			float3 @float = base.transform.InverseTransformPoint(grabPoint);
			float num4 = 0f;
			float num5 = float.PositiveInfinity;
			while (i < num)
			{
				float num6 = math.distancesq(this.spline.GetPointLocal(num2), @float);
				if (num6 < num5)
				{
					num5 = num6;
					num4 = num2;
				}
				num2 += num3;
				i++;
			}
			return num4;
		}

		// Token: 0x06007204 RID: 29188 RVA: 0x00251B40 File Offset: 0x0024FD40
		protected virtual void Start()
		{
			this.spline = base.GetComponent<BezierSpline>();
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06007205 RID: 29189 RVA: 0x00251B76 File Offset: 0x0024FD76
		private void OnDestroy()
		{
			GorillaClimbable gorillaClimbable = this.slideHelper;
			gorillaClimbable.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Remove(gorillaClimbable.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(this.OnBeforeClimb));
		}

		// Token: 0x06007206 RID: 29190 RVA: 0x00251BA0 File Offset: 0x0024FDA0
		public Vector3 GetCurrentDirection()
		{
			return this.spline.GetDirection(this.currentT);
		}

		// Token: 0x06007207 RID: 29191 RVA: 0x00251BB4 File Offset: 0x0024FDB4
		protected virtual void OnBeforeClimb(GorillaHandClimber hand, GorillaClimbableRef climbRef)
		{
			bool flag = this.currentClimber == null;
			this.currentClimber = hand;
			if (climbRef)
			{
				this.climbOffsetHelper.SetParent(climbRef.transform);
				this.climbOffsetHelper.position = hand.transform.position;
				this.climbOffsetHelper.localPosition = new Vector3(0f, 0f, this.climbOffsetHelper.localPosition.z);
			}
			this.currentT = this.FindSlideHelperSpot(this.climbOffsetHelper.position);
			this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
			if (flag)
			{
				Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
				float num = Vector3.Dot(averagedVelocity.normalized, this.spline.GetDirection(this.currentT));
				this.currentSpeed = averagedVelocity.magnitude * num * this.currentInheritVelocityMulti;
			}
		}

		// Token: 0x06007208 RID: 29192 RVA: 0x00251CA8 File Offset: 0x0024FEA8
		private void Update()
		{
			if (this.currentClimber)
			{
				Vector3 direction = this.spline.GetDirection(this.currentT);
				float num = GTPlayerTransform.GravityStrength * Vector3.Dot(GTPlayerTransform.PhysicsUp, direction) * this.settings.gravityMulti;
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, this.settings.maxSpeed, num * Time.deltaTime);
				float num2 = MathUtils.Linear(this.currentSpeed, 0f, this.settings.maxFrictionSpeed, this.settings.friction, this.settings.maxFriction);
				this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, num2 * Time.deltaTime);
				this.currentSpeed = Mathf.Min(this.currentSpeed, this.settings.maxSpeed);
				this.currentSpeed = Mathf.Max(this.currentSpeed, -this.settings.maxSpeed);
				float num3 = Mathf.Abs(this.currentSpeed);
				this.FindTFromDistance(ref this.currentT, this.currentSpeed * Time.deltaTime, 1000);
				this.slideHelper.transform.localPosition = this.spline.GetPointLocal(this.currentT);
				if (!this.audioSlide.gameObject.activeSelf)
				{
					this.audioSlide.gameObject.SetActive(true);
				}
				this.audioSlide.volume = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, this.settings.minSlideVolume, this.settings.maxSlideVolume);
				this.audioSlide.pitch = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, this.settings.minSlidePitch, this.settings.maxSlidePitch);
				if (!this.audioSlide.isPlaying)
				{
					this.audioSlide.GTPlay();
				}
				float num4 = MathUtils.Linear(num3, 0f, this.settings.maxSpeed, -0.1f, 0.75f);
				if (num4 > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.currentClimber.xrNode, num4, Time.deltaTime);
				}
				if (!this.spline.Loop)
				{
					if (this.currentT >= 1f || this.currentT <= 0f)
					{
						this.currentClimber.ForceStopClimbing(false, true);
					}
				}
				else if (this.currentT >= 1f)
				{
					this.currentT = 0f;
				}
				else if (this.currentT <= 0f)
				{
					this.currentT = 1f;
				}
				if (!this.slideHelper.isBeingClimbed)
				{
					this.Stop();
				}
			}
			if (this.currentInheritVelocityMulti < 1f)
			{
				this.currentInheritVelocityMulti += Time.deltaTime * 0.2f;
				this.currentInheritVelocityMulti = Mathf.Min(this.currentInheritVelocityMulti, 1f);
			}
		}

		// Token: 0x06007209 RID: 29193 RVA: 0x00251F96 File Offset: 0x00250196
		private void Stop()
		{
			this.currentClimber = null;
			this.audioSlide.GTStop();
			this.audioSlide.gameObject.SetActive(false);
			this.currentInheritVelocityMulti = 0.55f;
			this.currentSpeed = 0f;
		}

		// Token: 0x040082C3 RID: 33475
		[SerializeField]
		protected Transform segmentsRoot;

		// Token: 0x040082C4 RID: 33476
		[SerializeField]
		protected GameObject segmentPrefab;

		// Token: 0x040082C5 RID: 33477
		[SerializeField]
		protected GorillaClimbable slideHelper;

		// Token: 0x040082C6 RID: 33478
		[SerializeField]
		private AudioSource audioSlide;

		// Token: 0x040082C7 RID: 33479
		protected BezierSpline spline;

		// Token: 0x040082C8 RID: 33480
		[SerializeField]
		private Transform climbOffsetHelper;

		// Token: 0x040082C9 RID: 33481
		[SerializeField]
		private GorillaZiplineSettings settings;

		// Token: 0x040082CB RID: 33483
		[SerializeField]
		protected float ziplineDistance = 15f;

		// Token: 0x040082CC RID: 33484
		[SerializeField]
		protected float segmentDistance = 0.9f;

		// Token: 0x040082CD RID: 33485
		private GorillaHandClimber currentClimber;

		// Token: 0x040082CE RID: 33486
		private float currentT;

		// Token: 0x040082CF RID: 33487
		private const float inheritVelocityRechargeRate = 0.2f;

		// Token: 0x040082D0 RID: 33488
		private const float inheritVelocityValueOnRelease = 0.55f;

		// Token: 0x040082D1 RID: 33489
		private float currentInheritVelocityMulti = 1f;
	}
}
