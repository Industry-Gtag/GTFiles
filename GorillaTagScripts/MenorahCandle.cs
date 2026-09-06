using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000FA0 RID: 4000
	public class MenorahCandle : MonoBehaviourPun
	{
		// Token: 0x0600638F RID: 25487 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void Awake()
		{
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x002005A8 File Offset: 0x001FE7A8
		private void Start()
		{
			this.EnableCandle(false);
			this.EnableFlame(false);
			this.litDate = new DateTime(this.year, this.month, this.day);
			this.currentDate = DateTime.Now;
			this.EnableCandle(this.CandleShouldBeVisible());
			this.EnableFlame(false);
			GorillaComputer instance = GorillaComputer.instance;
			instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x00200626 File Offset: 0x001FE826
		private void UpdateMenorah()
		{
			this.EnableCandle(this.CandleShouldBeVisible());
			if (this.ShouldLightCandle())
			{
				this.EnableFlame(true);
				return;
			}
			if (this.ShouldSnuffCandle())
			{
				this.EnableFlame(false);
			}
		}

		// Token: 0x06006392 RID: 25490 RVA: 0x00200653 File Offset: 0x001FE853
		private void OnTimeChanged()
		{
			this.currentDate = GorillaComputer.instance.GetServerTime();
			this.UpdateMenorah();
		}

		// Token: 0x06006393 RID: 25491 RVA: 0x0020066D File Offset: 0x001FE86D
		public void OnTimeEventStart()
		{
			this.activeTimeEventDay = true;
			this.UpdateMenorah();
		}

		// Token: 0x06006394 RID: 25492 RVA: 0x0020067C File Offset: 0x001FE87C
		public void OnTimeEventEnd()
		{
			this.activeTimeEventDay = false;
			this.UpdateMenorah();
		}

		// Token: 0x06006395 RID: 25493 RVA: 0x0020068B File Offset: 0x001FE88B
		private void EnableCandle(bool enable)
		{
			if (this.candle)
			{
				this.candle.SetActive(enable);
			}
		}

		// Token: 0x06006396 RID: 25494 RVA: 0x002006A6 File Offset: 0x001FE8A6
		private bool CandleShouldBeVisible()
		{
			return this.currentDate >= this.litDate;
		}

		// Token: 0x06006397 RID: 25495 RVA: 0x002006B9 File Offset: 0x001FE8B9
		private void EnableFlame(bool enable)
		{
			if (this.flame)
			{
				this.flame.SetActive(enable);
			}
		}

		// Token: 0x06006398 RID: 25496 RVA: 0x002006D4 File Offset: 0x001FE8D4
		private bool ShouldLightCandle()
		{
			return !this.activeTimeEventDay && this.CandleShouldBeVisible() && !this.flame.activeSelf;
		}

		// Token: 0x06006399 RID: 25497 RVA: 0x002006F6 File Offset: 0x001FE8F6
		private bool ShouldSnuffCandle()
		{
			return this.activeTimeEventDay && this.flame.activeSelf;
		}

		// Token: 0x0600639A RID: 25498 RVA: 0x0020070D File Offset: 0x001FE90D
		private void OnDestroy()
		{
			if (GorillaComputer.instance)
			{
				GorillaComputer instance = GorillaComputer.instance;
				instance.OnServerTimeUpdated = (Action)Delegate.Remove(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
			}
		}

		// Token: 0x04007245 RID: 29253
		public int day;

		// Token: 0x04007246 RID: 29254
		public int month;

		// Token: 0x04007247 RID: 29255
		public int year;

		// Token: 0x04007248 RID: 29256
		public GameObject flame;

		// Token: 0x04007249 RID: 29257
		public GameObject candle;

		// Token: 0x0400724A RID: 29258
		private DateTime litDate;

		// Token: 0x0400724B RID: 29259
		private bool activeTimeEventDay;

		// Token: 0x0400724C RID: 29260
		private DateTime currentDate;
	}
}
