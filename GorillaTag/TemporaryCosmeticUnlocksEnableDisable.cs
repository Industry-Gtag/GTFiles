using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200120C RID: 4620
	public class TemporaryCosmeticUnlocksEnableDisable : MonoBehaviour
	{
		// Token: 0x06007510 RID: 29968 RVA: 0x002602C8 File Offset: 0x0025E4C8
		private void Awake()
		{
			if (this.m_wardrobe.IsNull() || this.m_cosmeticAreaTrigger.IsNull())
			{
				Debug.LogError("TemporaryCosmeticUnlocksEnableDisable: reference is null, disabling self");
				base.enabled = false;
			}
			if (CosmeticsController.instance.IsNull() || !this.m_wardrobe.WardrobeButtonsInitialized())
			{
				base.enabled = false;
				this.m_timer = new TickSystemTimer(0.05f, new Action(this.CheckWardrobeRady));
				this.m_timer.Start();
			}
		}

		// Token: 0x06007511 RID: 29969 RVA: 0x0026034C File Offset: 0x0025E54C
		private void OnEnable()
		{
			bool tempUnlocksEnabled = PlayerCosmeticsSystem.TempUnlocksEnabled;
			this.m_wardrobe.UseTemporarySet = tempUnlocksEnabled;
			this.m_cosmeticAreaTrigger.SetActive(tempUnlocksEnabled);
		}

		// Token: 0x06007512 RID: 29970 RVA: 0x00260378 File Offset: 0x0025E578
		private void CheckWardrobeRady()
		{
			if (CosmeticsController.instance.IsNotNull() && this.m_wardrobe.WardrobeButtonsInitialized())
			{
				this.m_timer.Stop();
				this.m_timer = null;
				base.enabled = true;
				return;
			}
			this.m_timer.Start();
		}

		// Token: 0x040084C5 RID: 33989
		[SerializeField]
		private CosmeticWardrobe m_wardrobe;

		// Token: 0x040084C6 RID: 33990
		[SerializeField]
		private GameObject m_cosmeticAreaTrigger;

		// Token: 0x040084C7 RID: 33991
		private TickSystemTimer m_timer;
	}
}
