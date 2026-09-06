using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020011B3 RID: 4531
	[NetworkBehaviourWeaved(1)]
	public class TraverseSpline : NetworkComponent
	{
		// Token: 0x0600724B RID: 29259 RVA: 0x002530C1 File Offset: 0x002512C1
		protected override void Awake()
		{
			base.Awake();
			this.progress = this.SplineProgressOffet % 1f;
		}

		// Token: 0x0600724C RID: 29260 RVA: 0x002530DC File Offset: 0x002512DC
		protected virtual void FixedUpdate()
		{
			if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f);
			}
			else
			{
				if (this.isHeldByLocalPlayer)
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, this.speedMultiplierWhileHeld, this.acceleration * Time.deltaTime);
				}
				else
				{
					this.currentSpeedMultiplier = Mathf.MoveTowards(this.currentSpeedMultiplier, 1f, this.deceleration * Time.deltaTime);
				}
				if (this.goingForward)
				{
					this.progress += Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress > 1f)
					{
						if (this.mode == SplineWalkerMode.Once)
						{
							this.progress = 1f;
						}
						else if (this.mode == SplineWalkerMode.Loop)
						{
							this.progress %= 1f;
						}
						else
						{
							this.progress = 2f - this.progress;
							this.goingForward = false;
						}
					}
				}
				else
				{
					this.progress -= Time.deltaTime * this.currentSpeedMultiplier / this.duration;
					if (this.progress < 0f)
					{
						this.progress = -this.progress;
						this.goingForward = true;
					}
				}
			}
			Vector3 point = this.spline.GetPoint(this.progress, this.constantVelocity);
			base.transform.position = point;
			if (this.lookForward)
			{
				base.transform.LookAt(base.transform.position + this.spline.GetDirection(this.progress, this.constantVelocity));
			}
		}

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x0600724D RID: 29261 RVA: 0x002532A5 File Offset: 0x002514A5
		// (set) Token: 0x0600724E RID: 29262 RVA: 0x002532CB File Offset: 0x002514CB
		[Networked]
		[NetworkedWeaved(0, 1)]
		public unsafe float Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(float*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing TraverseSpline.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(float*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x0600724F RID: 29263 RVA: 0x002532F2 File Offset: 0x002514F2
		public override void WriteDataFusion()
		{
			this.Data = this.progress + this.currentSpeedMultiplier * 1f / this.duration;
		}

		// Token: 0x06007250 RID: 29264 RVA: 0x00253314 File Offset: 0x00251514
		public override void ReadDataFusion()
		{
			this.progressLerpEnd = this.Data;
			this.ReadDataShared();
		}

		// Token: 0x06007251 RID: 29265 RVA: 0x00253328 File Offset: 0x00251528
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.progress + this.currentSpeedMultiplier * 1f / this.duration);
		}

		// Token: 0x06007252 RID: 29266 RVA: 0x0025334F File Offset: 0x0025154F
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			this.progressLerpEnd = (float)stream.ReceiveNext();
			this.ReadDataShared();
		}

		// Token: 0x06007253 RID: 29267 RVA: 0x00253368 File Offset: 0x00251568
		private void ReadDataShared()
		{
			if (float.IsNaN(this.progressLerpEnd) || float.IsInfinity(this.progressLerpEnd))
			{
				this.progressLerpEnd = 1f;
			}
			else
			{
				this.progressLerpEnd = Mathf.Abs(this.progressLerpEnd);
				if (this.progressLerpEnd > 1f)
				{
					this.progressLerpEnd = (float)((double)this.progressLerpEnd % 1.0);
				}
			}
			this.progressLerpStart = ((Mathf.Abs(this.progressLerpEnd - this.progress) > Mathf.Abs(this.progressLerpEnd - (this.progress - 1f))) ? (this.progress - 1f) : this.progress);
			this.progressLerpStartTime = Time.time;
		}

		// Token: 0x06007254 RID: 29268 RVA: 0x00253423 File Offset: 0x00251623
		protected float GetProgress()
		{
			return this.progress;
		}

		// Token: 0x06007255 RID: 29269 RVA: 0x0025342B File Offset: 0x0025162B
		public float GetCurrentSpeed()
		{
			return this.currentSpeedMultiplier;
		}

		// Token: 0x06007257 RID: 29271 RVA: 0x00253481 File Offset: 0x00251681
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06007258 RID: 29272 RVA: 0x00253499 File Offset: 0x00251699
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04008311 RID: 33553
		public BezierSpline spline;

		// Token: 0x04008312 RID: 33554
		public float duration = 30f;

		// Token: 0x04008313 RID: 33555
		public float speedMultiplierWhileHeld = 2f;

		// Token: 0x04008314 RID: 33556
		private float currentSpeedMultiplier;

		// Token: 0x04008315 RID: 33557
		public float acceleration = 1f;

		// Token: 0x04008316 RID: 33558
		public float deceleration = 1f;

		// Token: 0x04008317 RID: 33559
		private bool isHeldByLocalPlayer;

		// Token: 0x04008318 RID: 33560
		public bool lookForward = true;

		// Token: 0x04008319 RID: 33561
		public SplineWalkerMode mode;

		// Token: 0x0400831A RID: 33562
		[SerializeField]
		private float SplineProgressOffet;

		// Token: 0x0400831B RID: 33563
		private float progress;

		// Token: 0x0400831C RID: 33564
		private float progressLerpStart;

		// Token: 0x0400831D RID: 33565
		private float progressLerpEnd;

		// Token: 0x0400831E RID: 33566
		private const float progressLerpDuration = 1f;

		// Token: 0x0400831F RID: 33567
		private float progressLerpStartTime;

		// Token: 0x04008320 RID: 33568
		private bool goingForward = true;

		// Token: 0x04008321 RID: 33569
		[SerializeField]
		private bool constantVelocity;

		// Token: 0x04008322 RID: 33570
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private float _Data;
	}
}
