using System;
using GorillaTagScripts.Builder;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F52 RID: 3922
	public class SurfaceMover : MonoBehaviour
	{
		// Token: 0x06006060 RID: 24672 RVA: 0x001E8B31 File Offset: 0x001E6D31
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterSurfaceMover(this);
		}

		// Token: 0x06006061 RID: 24673 RVA: 0x001E8B4A File Offset: 0x001E6D4A
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterSurfaceMover(this);
			}
		}

		// Token: 0x06006062 RID: 24674 RVA: 0x001E8B64 File Offset: 0x001E6D64
		public void InitMovingSurface()
		{
			if (this.moveType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				this.distance = Vector3.Distance(this.endXf.position, this.startXf.position);
				float num = this.distance / this.velocity;
				this.cycleDuration = num + this.cycleDelay;
			}
			else
			{
				if (this.rotationRelativeToStarting)
				{
					this.startingRotation = base.transform.localRotation.eulerAngles;
				}
				this.cycleDuration = this.rotationAmount / 360f / this.velocity;
				this.cycleDuration += this.cycleDelay;
			}
			float num2 = this.cycleDelay / this.cycleDuration;
			Vector2 vector = new Vector2(num2 / 2f, 0f);
			Vector2 vector2 = new Vector2(1f - num2 / 2f, 1f);
			float num3 = (vector2.y - vector.y) / (vector2.x - vector.x);
			this.lerpAlpha = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(num2 / 2f, 0f, 0f, num3),
				new Keyframe(1f - num2 / 2f, 1f, num3, 0f)
			});
			this.currT = this.startPercentage;
			uint num4 = (uint)(this.cycleDuration * 1000f);
			if (num4 == 0U)
			{
				num4 = 1U;
			}
			uint num5 = 2147483648U % num4;
			uint num6 = (uint)(this.startPercentage * num4);
			if (num6 >= num5)
			{
				this.startPercentageCycleOffset = num6 - num5;
				return;
			}
			this.startPercentageCycleOffset = num6 + num4 + num4 - num5;
		}

		// Token: 0x06006063 RID: 24675 RVA: 0x001E8D0E File Offset: 0x001E6F0E
		private long NetworkTimeMs()
		{
			if (PhotonNetwork.InRoom)
			{
				return (long)((ulong)(PhotonNetwork.ServerTimestamp + (int)this.startPercentageCycleOffset + int.MinValue));
			}
			return (long)(Time.time * 1000f);
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x001E8D37 File Offset: 0x001E6F37
		private long CycleLengthMs()
		{
			return (long)(this.cycleDuration * 1000f);
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001E8D48 File Offset: 0x001E6F48
		public double PlatformTime()
		{
			long num = this.NetworkTimeMs();
			long num2 = this.CycleLengthMs();
			return (double)(num - num / num2 * num2) / 1000.0;
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x001E8D73 File Offset: 0x001E6F73
		public int CycleCount()
		{
			return (int)(this.NetworkTimeMs() / this.CycleLengthMs());
		}

		// Token: 0x06006067 RID: 24679 RVA: 0x001E8D83 File Offset: 0x001E6F83
		public float CycleCompletionPercent()
		{
			return Mathf.Clamp((float)(this.PlatformTime() / (double)this.cycleDuration), 0f, 1f);
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x001E8DA3 File Offset: 0x001E6FA3
		public bool IsEvenCycle()
		{
			return this.CycleCount() % 2 == 0;
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x001E8DB0 File Offset: 0x001E6FB0
		public void Move()
		{
			this.Progress();
			BuilderMovingPart.BuilderMovingPartType builderMovingPartType = this.moveType;
			if (builderMovingPartType == BuilderMovingPart.BuilderMovingPartType.Translation)
			{
				base.transform.localPosition = this.UpdatePointToPoint(this.percent);
				return;
			}
			if (builderMovingPartType != BuilderMovingPart.BuilderMovingPartType.Rotation)
			{
				return;
			}
			this.UpdateRotation(this.percent);
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x001E8DF8 File Offset: 0x001E6FF8
		private Vector3 UpdatePointToPoint(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc);
			return Vector3.Lerp(this.startXf.localPosition, this.endXf.localPosition, num);
		}

		// Token: 0x0600606B RID: 24683 RVA: 0x001E8E30 File Offset: 0x001E7030
		private void UpdateRotation(float perc)
		{
			float num = this.lerpAlpha.Evaluate(perc) * this.rotationAmount;
			if (this.rotationRelativeToStarting)
			{
				Vector3 vector = this.startingRotation;
				switch (this.rotationAxis)
				{
				case RotationAxis.X:
					vector.x += num;
					break;
				case RotationAxis.Y:
					vector.y += num;
					break;
				case RotationAxis.Z:
					vector.z += num;
					break;
				}
				base.transform.localRotation = Quaternion.Euler(vector);
				return;
			}
			switch (this.rotationAxis)
			{
			case RotationAxis.X:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.right);
				return;
			case RotationAxis.Y:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.up);
				return;
			case RotationAxis.Z:
				base.transform.localRotation = Quaternion.AngleAxis(num, Vector3.forward);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600606C RID: 24684 RVA: 0x001E8F14 File Offset: 0x001E7114
		private void Progress()
		{
			this.currT = this.CycleCompletionPercent();
			this.currForward = this.IsEvenCycle();
			this.percent = this.currT;
			if (this.reverseDirOnCycle)
			{
				this.percent = (this.currForward ? this.currT : (1f - this.currT));
			}
			if (this.reverseDir)
			{
				this.percent = 1f - this.percent;
			}
		}

		// Token: 0x0600606D RID: 24685 RVA: 0x001E8F8C File Offset: 0x001E718C
		public void CopySettings(SurfaceMoverSettings settings)
		{
			this.moveType = (BuilderMovingPart.BuilderMovingPartType)settings.moveType;
			this.startPercentage = 0f;
			this.velocity = Math.Clamp(settings.velocity, 0.001f, Math.Abs(settings.velocity));
			this.reverseDirOnCycle = settings.reverseDirOnCycle;
			this.reverseDir = settings.reverseDir;
			this.cycleDelay = Math.Clamp(settings.cycleDelay, 0f, Math.Abs(settings.cycleDelay));
			this.startXf = settings.start;
			this.endXf = settings.end;
			this.rotationAxis = (RotationAxis)settings.rotationAxis;
			this.rotationAmount = Math.Clamp(settings.rotationAmount, 0.001f, Math.Abs(settings.rotationAmount));
			this.rotationRelativeToStarting = settings.rotationRelativeToStarting;
		}

		// Token: 0x04006EEE RID: 28398
		[SerializeField]
		private BuilderMovingPart.BuilderMovingPartType moveType;

		// Token: 0x04006EEF RID: 28399
		[SerializeField]
		private float startPercentage = 0.5f;

		// Token: 0x04006EF0 RID: 28400
		[SerializeField]
		private float velocity;

		// Token: 0x04006EF1 RID: 28401
		[SerializeField]
		private bool reverseDirOnCycle = true;

		// Token: 0x04006EF2 RID: 28402
		[SerializeField]
		private bool reverseDir;

		// Token: 0x04006EF3 RID: 28403
		[SerializeField]
		private float cycleDelay = 0.25f;

		// Token: 0x04006EF4 RID: 28404
		[SerializeField]
		protected Transform startXf;

		// Token: 0x04006EF5 RID: 28405
		[SerializeField]
		protected Transform endXf;

		// Token: 0x04006EF6 RID: 28406
		[SerializeField]
		public RotationAxis rotationAxis = RotationAxis.Y;

		// Token: 0x04006EF7 RID: 28407
		[SerializeField]
		public float rotationAmount = 360f;

		// Token: 0x04006EF8 RID: 28408
		[SerializeField]
		public bool rotationRelativeToStarting;

		// Token: 0x04006EF9 RID: 28409
		private AnimationCurve lerpAlpha;

		// Token: 0x04006EFA RID: 28410
		private float cycleDuration;

		// Token: 0x04006EFB RID: 28411
		private float distance;

		// Token: 0x04006EFC RID: 28412
		private Vector3 startingRotation;

		// Token: 0x04006EFD RID: 28413
		private float currT;

		// Token: 0x04006EFE RID: 28414
		private float percent;

		// Token: 0x04006EFF RID: 28415
		private bool currForward;

		// Token: 0x04006F00 RID: 28416
		private float dtSinceServerUpdate;

		// Token: 0x04006F01 RID: 28417
		private int lastServerTimeStamp;

		// Token: 0x04006F02 RID: 28418
		private float rotateStartAmt;

		// Token: 0x04006F03 RID: 28419
		private float rotateAmt;

		// Token: 0x04006F04 RID: 28420
		private uint startPercentageCycleOffset;
	}
}
