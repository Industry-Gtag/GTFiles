using System;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000E19 RID: 3609
[Obsolete("DEPRECATED! Use SubscriptionKiosk instead")]
public class SubscriptionStation : MonoBehaviour
{
	// Token: 0x0600586A RID: 22634 RVA: 0x001CB5DC File Offset: 0x001C97DC
	private void Awake()
	{
		this.formatString = this.screenText.text;
		this.screenText.text = string.Format(this.formatString, new object[] { "*", "*", "*", "*" });
	}

	// Token: 0x0600586B RID: 22635 RVA: 0x001CB638 File Offset: 0x001C9838
	private void UpdateScreen()
	{
		Debug.Log(":::SubscriptionStation::UpdateScreen");
		bool flag = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).tier > 0;
		int daysAccrued = SubscriptionManager.GetSubscriptionDetails(VRRig.LocalRig).daysAccrued;
		bool subsOnlyMatchmaking = SubscriptionManager.SubsOnlyMatchmaking;
		bool showGoldNameTag = VRRig.LocalRig.ShowGoldNameTag;
		if (flag)
		{
			this.screenText.text = string.Format(this.formatString, new object[]
			{
				"Y",
				subsOnlyMatchmaking ? "Y" : "N",
				showGoldNameTag ? "Y" : "N",
				daysAccrued
			});
			return;
		}
		this.screenText.text = string.Format(this.formatString, new object[] { "N", "*", "*", "*" });
	}

	// Token: 0x0600586C RID: 22636 RVA: 0x001CB712 File Offset: 0x001C9912
	public void ToggleSubscriptionStatus()
	{
		SubscriptionManager.ForceRecheck();
		this.UpdateScreen();
	}

	// Token: 0x0600586D RID: 22637 RVA: 0x001CB71F File Offset: 0x001C991F
	public void ToggleSubsOnly()
	{
		SubscriptionManager.SubsOnlyMatchmaking = !SubscriptionManager.SubsOnlyMatchmaking;
		this.UpdateScreen();
	}

	// Token: 0x0600586E RID: 22638 RVA: 0x001CB734 File Offset: 0x001C9934
	public void ToggleSubsDecoration()
	{
		this.UpdateScreen();
	}

	// Token: 0x0400688A RID: 26762
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x0400688B RID: 26763
	private string formatString;
}
