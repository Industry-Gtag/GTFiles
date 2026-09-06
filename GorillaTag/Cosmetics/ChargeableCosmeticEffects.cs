using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x020012FE RID: 4862
	public class ChargeableCosmeticEffects : MonoBehaviour, ITickSystemTick
	{
		// Token: 0x060079FE RID: 31230 RVA: 0x0027CF8A File Offset: 0x0027B18A
		private bool HasFractionals()
		{
			return this.continuousProperties.Count > 0 || this.whileCharging.GetPersistentEventCount() > 0;
		}

		// Token: 0x060079FF RID: 31231 RVA: 0x0027CFAA File Offset: 0x0027B1AA
		private void Awake()
		{
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.hasFractionalsCached = this.HasFractionals();
		}

		// Token: 0x06007A00 RID: 31232 RVA: 0x0027CFCA File Offset: 0x0027B1CA
		public void SetMaxChargeSeconds(float s)
		{
			this.maxChargeSeconds = s;
			this.inverseMaxChargeSeconds = 1f / this.maxChargeSeconds;
			this.SetChargeTime(this.chargeTime);
		}

		// Token: 0x06007A01 RID: 31233 RVA: 0x0027CFF1 File Offset: 0x0027B1F1
		public void SetChargeState(bool state)
		{
			if (this.isCharging != state)
			{
				TickSystem<object>.AddTickCallback(this);
				this.isCharging = state;
			}
		}

		// Token: 0x06007A02 RID: 31234 RVA: 0x0027D009 File Offset: 0x0027B209
		public void StartCharging()
		{
			this.SetChargeState(true);
		}

		// Token: 0x06007A03 RID: 31235 RVA: 0x0027D012 File Offset: 0x0027B212
		public void StopCharging()
		{
			this.SetChargeState(false);
		}

		// Token: 0x06007A04 RID: 31236 RVA: 0x0027D01B File Offset: 0x0027B21B
		public void ToggleCharging()
		{
			this.SetChargeState(!this.isCharging);
		}

		// Token: 0x06007A05 RID: 31237 RVA: 0x0027D02C File Offset: 0x0027B22C
		public void SetChargeTime(float t)
		{
			if (t >= this.maxChargeSeconds)
			{
				if (this.chargeTime < this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
			}
			else if (t <= 0f)
			{
				if (this.chargeTime > 0f)
				{
					this.RunNoCharge();
					return;
				}
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
				this.chargeTime = t;
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x06007A06 RID: 31238 RVA: 0x0027D08E File Offset: 0x0027B28E
		public void SetChargeFrac(float f)
		{
			this.SetChargeTime(f * this.maxChargeSeconds);
		}

		// Token: 0x06007A07 RID: 31239 RVA: 0x0027D09E File Offset: 0x0027B29E
		public void EmptyCharge()
		{
			this.SetChargeTime(0f);
		}

		// Token: 0x06007A08 RID: 31240 RVA: 0x0027D0AB File Offset: 0x0027B2AB
		public void FillCharge()
		{
			this.SetChargeTime(this.maxChargeSeconds);
		}

		// Token: 0x06007A09 RID: 31241 RVA: 0x0027D0B9 File Offset: 0x0027B2B9
		public void EmptyAndStop()
		{
			this.isCharging = false;
			this.EmptyCharge();
		}

		// Token: 0x06007A0A RID: 31242 RVA: 0x0027D0C8 File Offset: 0x0027B2C8
		public void FillAndStop()
		{
			this.StopCharging();
			this.FillCharge();
		}

		// Token: 0x06007A0B RID: 31243 RVA: 0x0027D0D6 File Offset: 0x0027B2D6
		public void EmptyAndStart()
		{
			this.StartCharging();
			this.EmptyCharge();
		}

		// Token: 0x06007A0C RID: 31244 RVA: 0x0027D0E4 File Offset: 0x0027B2E4
		public void FillAndStart()
		{
			this.isCharging = true;
			this.FillCharge();
		}

		// Token: 0x06007A0D RID: 31245 RVA: 0x0027D0F4 File Offset: 0x0027B2F4
		private void OnEnable()
		{
			if ((this.chargeTime <= 0f && this.isCharging) || (this.chargeTime >= this.maxChargeSeconds && !this.isCharging) || (this.chargeTime > 0f && this.chargeTime < this.maxChargeSeconds))
			{
				TickSystem<object>.AddTickCallback(this);
			}
		}

		// Token: 0x06007A0E RID: 31246 RVA: 0x0001A29F File Offset: 0x0001849F
		private void OnDisable()
		{
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007A0F RID: 31247 RVA: 0x0027D150 File Offset: 0x0027B350
		private void RunMaxCharge()
		{
			if (this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = this.maxChargeSeconds;
			UnityEvent unityEvent = this.onMaxCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(1f);
			}
			this.continuousProperties.ApplyAll(1f);
		}

		// Token: 0x06007A10 RID: 31248 RVA: 0x0027D1B8 File Offset: 0x0027B3B8
		private void RunNoCharge()
		{
			if (!this.isCharging)
			{
				TickSystem<object>.RemoveTickCallback(this);
			}
			else
			{
				TickSystem<object>.AddTickCallback(this);
			}
			this.chargeTime = 0f;
			UnityEvent unityEvent = this.onNoCharge;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			UnityEvent<float> unityEvent2 = this.whileCharging;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(0f);
			}
			this.continuousProperties.ApplyAll(0f);
		}

		// Token: 0x06007A11 RID: 31249 RVA: 0x0027D220 File Offset: 0x0027B420
		private void RunChargeFrac()
		{
			float num = this.masterChargeRemapCurve.Evaluate(this.chargeTime * this.inverseMaxChargeSeconds);
			UnityEvent<float> unityEvent = this.whileCharging;
			if (unityEvent != null)
			{
				unityEvent.Invoke(num);
			}
			this.continuousProperties.ApplyAll(num);
		}

		// Token: 0x17000BD1 RID: 3025
		// (get) Token: 0x06007A12 RID: 31250 RVA: 0x0027D264 File Offset: 0x0027B464
		// (set) Token: 0x06007A13 RID: 31251 RVA: 0x0027D26C File Offset: 0x0027B46C
		public bool TickRunning { get; set; }

		// Token: 0x06007A14 RID: 31252 RVA: 0x0027D278 File Offset: 0x0027B478
		public void Tick()
		{
			if (this.isCharging && this.chargeTime < this.maxChargeSeconds)
			{
				this.chargeTime += Time.deltaTime * this.chargeGainSpeed;
				if (this.chargeTime >= this.maxChargeSeconds)
				{
					this.RunMaxCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
					return;
				}
			}
			else if (!this.isCharging && this.chargeTime > 0f)
			{
				this.chargeTime -= Time.deltaTime * this.chargeLossSpeed;
				if (this.chargeTime <= 0f)
				{
					this.RunNoCharge();
					return;
				}
				if (this.hasFractionalsCached)
				{
					this.RunChargeFrac();
				}
			}
		}

		// Token: 0x04008B68 RID: 35688
		[SerializeField]
		private float maxChargeSeconds = 1f;

		// Token: 0x04008B69 RID: 35689
		[SerializeField]
		private float chargeGainSpeed = 1f;

		// Token: 0x04008B6A RID: 35690
		[SerializeField]
		private float chargeLossSpeed = 1f;

		// Token: 0x04008B6B RID: 35691
		[Tooltip("This will remap the internal charge output to whatever you set. The remapped value will be output by 'whileCharging' and the 'continuousProperties' (keep in mind that the remapped value will then be used as an INPUT for the curves on each ContinuousProperty).\n\nIt should start at (0,0) and end at (1,1).\n\nDisabled if there are no ContinuousProperties and no whileCharging event callbacks.")]
		[SerializeField]
		private AnimationCurve masterChargeRemapCurve = AnimationCurves.Linear;

		// Token: 0x04008B6C RID: 35692
		[SerializeField]
		private bool isCharging;

		// Token: 0x04008B6D RID: 35693
		[SerializeField]
		private ContinuousPropertyArray continuousProperties;

		// Token: 0x04008B6E RID: 35694
		[SerializeField]
		private UnityEvent<float> whileCharging;

		// Token: 0x04008B6F RID: 35695
		[SerializeField]
		private UnityEvent onMaxCharge;

		// Token: 0x04008B70 RID: 35696
		[SerializeField]
		private UnityEvent onNoCharge;

		// Token: 0x04008B71 RID: 35697
		private float chargeTime;

		// Token: 0x04008B72 RID: 35698
		private float inverseMaxChargeSeconds;

		// Token: 0x04008B73 RID: 35699
		private bool hasFractionalsCached;
	}
}
