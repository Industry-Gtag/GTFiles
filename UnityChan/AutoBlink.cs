using System;
using System.Collections;
using UnityEngine;

namespace UnityChan
{
	// Token: 0x02001417 RID: 5143
	public class AutoBlink : MonoBehaviour
	{
		// Token: 0x060081BF RID: 33215 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void Awake()
		{
		}

		// Token: 0x060081C0 RID: 33216 RVA: 0x002A63D6 File Offset: 0x002A45D6
		private void Start()
		{
			this.ResetTimer();
			base.StartCoroutine("RandomChange");
		}

		// Token: 0x060081C1 RID: 33217 RVA: 0x002A63EA File Offset: 0x002A45EA
		private void ResetTimer()
		{
			this.timeRemining = this.timeBlink;
			this.timerStarted = false;
		}

		// Token: 0x060081C2 RID: 33218 RVA: 0x002A6400 File Offset: 0x002A4600
		private void Update()
		{
			if (!this.timerStarted)
			{
				this.eyeStatus = AutoBlink.Status.Close;
				this.timerStarted = true;
			}
			if (this.timerStarted)
			{
				this.timeRemining -= Time.deltaTime;
				if (this.timeRemining <= 0f)
				{
					this.eyeStatus = AutoBlink.Status.Open;
					this.ResetTimer();
					return;
				}
				if (this.timeRemining <= this.timeBlink * 0.3f)
				{
					this.eyeStatus = AutoBlink.Status.HalfClose;
				}
			}
		}

		// Token: 0x060081C3 RID: 33219 RVA: 0x002A6474 File Offset: 0x002A4674
		private void LateUpdate()
		{
			if (this.isActive && this.isBlink)
			{
				switch (this.eyeStatus)
				{
				case AutoBlink.Status.Close:
					this.SetCloseEyes();
					return;
				case AutoBlink.Status.HalfClose:
					this.SetHalfCloseEyes();
					return;
				case AutoBlink.Status.Open:
					this.SetOpenEyes();
					this.isBlink = false;
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x060081C4 RID: 33220 RVA: 0x002A64C6 File Offset: 0x002A46C6
		private void SetCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Close);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Close);
		}

		// Token: 0x060081C5 RID: 33221 RVA: 0x002A64EC File Offset: 0x002A46EC
		private void SetHalfCloseEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_HalfClose);
		}

		// Token: 0x060081C6 RID: 33222 RVA: 0x002A6512 File Offset: 0x002A4712
		private void SetOpenEyes()
		{
			this.ref_SMR_EYE_DEF.SetBlendShapeWeight(6, this.ratio_Open);
			this.ref_SMR_EL_DEF.SetBlendShapeWeight(6, this.ratio_Open);
		}

		// Token: 0x060081C7 RID: 33223 RVA: 0x002A6538 File Offset: 0x002A4738
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				float num = Random.Range(0f, 1f);
				if (!this.isBlink && num > this.threshold)
				{
					this.isBlink = true;
				}
				yield return new WaitForSeconds(this.interval);
			}
			yield break;
		}

		// Token: 0x04009278 RID: 37496
		public bool isActive = true;

		// Token: 0x04009279 RID: 37497
		public SkinnedMeshRenderer ref_SMR_EYE_DEF;

		// Token: 0x0400927A RID: 37498
		public SkinnedMeshRenderer ref_SMR_EL_DEF;

		// Token: 0x0400927B RID: 37499
		public float ratio_Close = 85f;

		// Token: 0x0400927C RID: 37500
		public float ratio_HalfClose = 20f;

		// Token: 0x0400927D RID: 37501
		[HideInInspector]
		public float ratio_Open;

		// Token: 0x0400927E RID: 37502
		private bool timerStarted;

		// Token: 0x0400927F RID: 37503
		private bool isBlink;

		// Token: 0x04009280 RID: 37504
		public float timeBlink = 0.4f;

		// Token: 0x04009281 RID: 37505
		private float timeRemining;

		// Token: 0x04009282 RID: 37506
		public float threshold = 0.3f;

		// Token: 0x04009283 RID: 37507
		public float interval = 3f;

		// Token: 0x04009284 RID: 37508
		private AutoBlink.Status eyeStatus;

		// Token: 0x02001418 RID: 5144
		private enum Status
		{
			// Token: 0x04009286 RID: 37510
			Close,
			// Token: 0x04009287 RID: 37511
			HalfClose,
			// Token: 0x04009288 RID: 37512
			Open
		}
	}
}
