using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000FF0 RID: 4080
	public class VirtualStumpTeleportingHUD : MonoBehaviour
	{
		// Token: 0x0600656D RID: 25965 RVA: 0x0020ABF4 File Offset: 0x00208DF4
		public void Initialize(bool isEntering)
		{
			this.isEnteringVirtualStump = isEntering;
			if (isEntering)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_ENTERING", out text, this.enteringVirtualStumpString))
				{
					Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_ENTERING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
				}
				this.teleportingStatusText.text = text;
				this.teleportingStatusText.gameObject.SetActive(true);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_LEAVING", out text2, this.leavingVirtualStumpString))
			{
				Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_LEAVING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
			}
			this.teleportingStatusText.text = text2;
			this.teleportingStatusText.gameObject.SetActive(true);
		}

		// Token: 0x0600656E RID: 25966 RVA: 0x0020ACAC File Offset: 0x00208EAC
		private void Update()
		{
			if (Time.time - this.lastTextUpdateTime > this.textUpdateInterval)
			{
				this.lastTextUpdateTime = Time.time;
				this.IncrementProgressDots();
				this.teleportingStatusText.text = (this.isEnteringVirtualStump ? this.enteringVirtualStumpString : this.leavingVirtualStumpString);
				for (int i = 0; i < this.numProgressDots; i++)
				{
					TMP_Text tmp_Text = this.teleportingStatusText;
					tmp_Text.text += ".";
				}
			}
		}

		// Token: 0x0600656F RID: 25967 RVA: 0x0020AD2B File Offset: 0x00208F2B
		private void IncrementProgressDots()
		{
			this.numProgressDots++;
			if (this.numProgressDots > this.maxNumProgressDots)
			{
				this.numProgressDots = 0;
			}
		}

		// Token: 0x0400746A RID: 29802
		private const string VIRT_STUMP_HUD_ENTERING_KEY = "VIRT_STUMP_HUD_ENTERING";

		// Token: 0x0400746B RID: 29803
		private const string VIRT_STUMP_HUD_LEAVING_KEY = "VIRT_STUMP_HUD_LEAVING";

		// Token: 0x0400746C RID: 29804
		[SerializeField]
		private string enteringVirtualStumpString = "Now Entering the Virtual Stump";

		// Token: 0x0400746D RID: 29805
		[SerializeField]
		private string leavingVirtualStumpString = "Now Leaving the Virtual Stump";

		// Token: 0x0400746E RID: 29806
		[SerializeField]
		private TMP_Text teleportingStatusText;

		// Token: 0x0400746F RID: 29807
		[SerializeField]
		private int maxNumProgressDots = 3;

		// Token: 0x04007470 RID: 29808
		[SerializeField]
		private float textUpdateInterval = 0.5f;

		// Token: 0x04007471 RID: 29809
		private float lastTextUpdateTime;

		// Token: 0x04007472 RID: 29810
		private int numProgressDots;

		// Token: 0x04007473 RID: 29811
		private bool isEnteringVirtualStump;
	}
}
