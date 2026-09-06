using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200141E RID: 5150
	public class BoingBehavior : BoingBase
	{
		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x060081E3 RID: 33251 RVA: 0x002A712F File Offset: 0x002A532F
		// (set) Token: 0x060081E4 RID: 33252 RVA: 0x002A7141 File Offset: 0x002A5341
		public Vector3Spring PositionSpring
		{
			get
			{
				return this.Params.Instance.PositionSpring;
			}
			set
			{
				this.Params.Instance.PositionSpring = value;
				this.PositionSpringDirty = true;
			}
		}

		// Token: 0x17000C77 RID: 3191
		// (get) Token: 0x060081E5 RID: 33253 RVA: 0x002A715B File Offset: 0x002A535B
		// (set) Token: 0x060081E6 RID: 33254 RVA: 0x002A716D File Offset: 0x002A536D
		public QuaternionSpring RotationSpring
		{
			get
			{
				return this.Params.Instance.RotationSpring;
			}
			set
			{
				this.Params.Instance.RotationSpring = value;
				this.RotationSpringDirty = true;
			}
		}

		// Token: 0x17000C78 RID: 3192
		// (get) Token: 0x060081E7 RID: 33255 RVA: 0x002A7187 File Offset: 0x002A5387
		// (set) Token: 0x060081E8 RID: 33256 RVA: 0x002A7199 File Offset: 0x002A5399
		public Vector3Spring ScaleSpring
		{
			get
			{
				return this.Params.Instance.ScaleSpring;
			}
			set
			{
				this.Params.Instance.ScaleSpring = value;
				this.ScaleSpringDirty = true;
			}
		}

		// Token: 0x060081E9 RID: 33257 RVA: 0x002A71B3 File Offset: 0x002A53B3
		public BoingBehavior()
		{
			this.Params.Init();
		}

		// Token: 0x060081EA RID: 33258 RVA: 0x002A71DC File Offset: 0x002A53DC
		public virtual void Reboot()
		{
			this.Params.Instance.PositionSpring.Reset(base.transform.position);
			this.Params.Instance.RotationSpring.Reset(base.transform.rotation);
			this.Params.Instance.ScaleSpring.Reset(base.transform.localScale);
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedPositionWs = base.transform.position;
			this.CachedRotationWs = base.transform.rotation;
			this.CachedScaleLs = base.transform.localScale;
			this.CachedTransformValid = true;
		}

		// Token: 0x060081EB RID: 33259 RVA: 0x002A72A5 File Offset: 0x002A54A5
		public virtual void OnEnable()
		{
			this.CachedTransformValid = false;
			this.InitRebooted = false;
			this.Register();
		}

		// Token: 0x060081EC RID: 33260 RVA: 0x002A72BB File Offset: 0x002A54BB
		public void Start()
		{
			this.InitRebooted = false;
		}

		// Token: 0x060081ED RID: 33261 RVA: 0x002A72C4 File Offset: 0x002A54C4
		public virtual void OnDisable()
		{
			this.Unregister();
		}

		// Token: 0x060081EE RID: 33262 RVA: 0x002A72CC File Offset: 0x002A54CC
		protected virtual void Register()
		{
			BoingManager.Register(this);
		}

		// Token: 0x060081EF RID: 33263 RVA: 0x002A72D4 File Offset: 0x002A54D4
		protected virtual void Unregister()
		{
			BoingManager.Unregister(this);
		}

		// Token: 0x060081F0 RID: 33264 RVA: 0x002A72DC File Offset: 0x002A54DC
		public void UpdateFlags()
		{
			this.Params.Bits.SetBit(0, this.TwoDDistanceCheck);
			this.Params.Bits.SetBit(1, this.TwoDPositionInfluence);
			this.Params.Bits.SetBit(2, this.TwoDRotationInfluence);
			this.Params.Bits.SetBit(3, this.EnablePositionEffect);
			this.Params.Bits.SetBit(4, this.EnableRotationEffect);
			this.Params.Bits.SetBit(5, this.EnableScaleEffect);
			this.Params.Bits.SetBit(6, this.GlobalReactionUpVector);
			this.Params.Bits.SetBit(9, this.UpdateMode == BoingManager.UpdateMode.FixedUpdate);
			this.Params.Bits.SetBit(10, this.UpdateMode == BoingManager.UpdateMode.EarlyUpdate);
			this.Params.Bits.SetBit(11, this.UpdateMode == BoingManager.UpdateMode.LateUpdate);
		}

		// Token: 0x060081F1 RID: 33265 RVA: 0x002A73DB File Offset: 0x002A55DB
		public virtual void PrepareExecute()
		{
			this.PrepareExecute(false);
		}

		// Token: 0x060081F2 RID: 33266 RVA: 0x002A73E4 File Offset: 0x002A55E4
		protected void PrepareExecute(bool accumulateEffectors)
		{
			if (this.SharedParams != null)
			{
				BoingWork.Params.Copy(ref this.SharedParams.Params, ref this.Params);
			}
			this.UpdateFlags();
			this.Params.InstanceID = base.GetInstanceID();
			this.Params.Instance.PrepareExecute(ref this.Params, this.CachedPositionWs, this.CachedRotationWs, base.transform.localScale, accumulateEffectors);
		}

		// Token: 0x060081F3 RID: 33267 RVA: 0x002A745A File Offset: 0x002A565A
		public void Execute(float dt)
		{
			this.Params.Execute(dt);
		}

		// Token: 0x060081F4 RID: 33268 RVA: 0x002A7468 File Offset: 0x002A5668
		public void PullResults()
		{
			this.PullResults(ref this.Params);
		}

		// Token: 0x060081F5 RID: 33269 RVA: 0x002A7478 File Offset: 0x002A5678
		public void GatherOutput(ref BoingWork.Output o)
		{
			if (!BoingManager.UseAsynchronousJobs)
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
				this.Params.Instance.RotationSpring = o.RotationSpring;
				this.Params.Instance.ScaleSpring = o.ScaleSpring;
				return;
			}
			if (this.PositionSpringDirty)
			{
				this.PositionSpringDirty = false;
			}
			else
			{
				this.Params.Instance.PositionSpring = o.PositionSpring;
			}
			if (this.RotationSpringDirty)
			{
				this.RotationSpringDirty = false;
			}
			else
			{
				this.Params.Instance.RotationSpring = o.RotationSpring;
			}
			if (this.ScaleSpringDirty)
			{
				this.ScaleSpringDirty = false;
				return;
			}
			this.Params.Instance.ScaleSpring = o.ScaleSpring;
		}

		// Token: 0x060081F6 RID: 33270 RVA: 0x002A7544 File Offset: 0x002A5744
		private void PullResults(ref BoingWork.Params p)
		{
			this.CachedPositionLs = base.transform.localPosition;
			this.CachedPositionWs = base.transform.position;
			this.RenderPositionWs = BoingWork.ComputeTranslationalResults(base.transform, base.transform.position, p.Instance.PositionSpring.Value, this);
			base.transform.position = this.RenderPositionWs;
			this.CachedRotationLs = base.transform.localRotation;
			this.CachedRotationWs = base.transform.rotation;
			this.RenderRotationWs = p.Instance.RotationSpring.ValueQuat;
			base.transform.rotation = this.RenderRotationWs;
			this.CachedScaleLs = base.transform.localScale;
			this.RenderScaleLs = p.Instance.ScaleSpring.Value;
			base.transform.localScale = this.RenderScaleLs;
			this.CachedTransformValid = true;
		}

		// Token: 0x060081F7 RID: 33271 RVA: 0x002A763C File Offset: 0x002A583C
		public virtual void Restore()
		{
			if (!this.CachedTransformValid)
			{
				return;
			}
			if (Application.isEditor)
			{
				if ((base.transform.position - this.RenderPositionWs).sqrMagnitude < 0.0001f)
				{
					base.transform.localPosition = this.CachedPositionLs;
				}
				if (QuaternionUtil.GetAngle(base.transform.rotation * Quaternion.Inverse(this.RenderRotationWs)) < 0.01f)
				{
					base.transform.localRotation = this.CachedRotationLs;
				}
				if ((base.transform.localScale - this.RenderScaleLs).sqrMagnitude < 0.0001f)
				{
					base.transform.localScale = this.CachedScaleLs;
					return;
				}
			}
			else
			{
				base.transform.localPosition = this.CachedPositionLs;
				base.transform.localRotation = this.CachedRotationLs;
				base.transform.localScale = this.CachedScaleLs;
			}
		}

		// Token: 0x040092B7 RID: 37559
		public BoingManager.UpdateMode UpdateMode = BoingManager.UpdateMode.LateUpdate;

		// Token: 0x040092B8 RID: 37560
		public bool TwoDDistanceCheck;

		// Token: 0x040092B9 RID: 37561
		public bool TwoDPositionInfluence;

		// Token: 0x040092BA RID: 37562
		public bool TwoDRotationInfluence;

		// Token: 0x040092BB RID: 37563
		public bool EnablePositionEffect = true;

		// Token: 0x040092BC RID: 37564
		public bool EnableRotationEffect = true;

		// Token: 0x040092BD RID: 37565
		public bool EnableScaleEffect;

		// Token: 0x040092BE RID: 37566
		public bool GlobalReactionUpVector;

		// Token: 0x040092BF RID: 37567
		public BoingManager.TranslationLockSpace TranslationLockSpace;

		// Token: 0x040092C0 RID: 37568
		public bool LockTranslationX;

		// Token: 0x040092C1 RID: 37569
		public bool LockTranslationY;

		// Token: 0x040092C2 RID: 37570
		public bool LockTranslationZ;

		// Token: 0x040092C3 RID: 37571
		public BoingWork.Params Params;

		// Token: 0x040092C4 RID: 37572
		public SharedBoingParams SharedParams;

		// Token: 0x040092C5 RID: 37573
		internal bool PositionSpringDirty;

		// Token: 0x040092C6 RID: 37574
		internal bool RotationSpringDirty;

		// Token: 0x040092C7 RID: 37575
		internal bool ScaleSpringDirty;

		// Token: 0x040092C8 RID: 37576
		internal bool CachedTransformValid;

		// Token: 0x040092C9 RID: 37577
		internal Vector3 CachedPositionLs;

		// Token: 0x040092CA RID: 37578
		internal Vector3 CachedPositionWs;

		// Token: 0x040092CB RID: 37579
		internal Vector3 RenderPositionWs;

		// Token: 0x040092CC RID: 37580
		internal Quaternion CachedRotationLs;

		// Token: 0x040092CD RID: 37581
		internal Quaternion CachedRotationWs;

		// Token: 0x040092CE RID: 37582
		internal Quaternion RenderRotationWs;

		// Token: 0x040092CF RID: 37583
		internal Vector3 CachedScaleLs;

		// Token: 0x040092D0 RID: 37584
		internal Vector3 RenderScaleLs;

		// Token: 0x040092D1 RID: 37585
		internal bool InitRebooted;
	}
}
