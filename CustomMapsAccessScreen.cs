using System;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x02000AAD RID: 2733
public class CustomMapsAccessScreen : CustomMapsTerminalScreen
{
	// Token: 0x060045E5 RID: 17893 RVA: 0x00177788 File Offset: 0x00175988
	private void LateUpdate()
	{
		if (CustomMapsTerminal.GetDriverID() == -2)
		{
			return;
		}
		if (CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (GorillaComputer.instance == null)
		{
			return;
		}
		if (this.useNametags == GorillaComputer.instance.NametagsEnabled)
		{
			return;
		}
		this.useNametags = GorillaComputer.instance.NametagsEnabled;
		this.SetDriverName();
	}

	// Token: 0x060045E6 RID: 17894 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void Initialize()
	{
	}

	// Token: 0x060045E7 RID: 17895 RVA: 0x001777E4 File Offset: 0x001759E4
	public override void Show()
	{
		base.Show();
		if (this.displayedText == string.Empty)
		{
			this.displayedText = this.defaultText;
		}
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x060045E8 RID: 17896 RVA: 0x00177848 File Offset: 0x00175A48
	public override void Hide()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(false);
		base.Hide();
	}

	// Token: 0x060045E9 RID: 17897 RVA: 0x00177872 File Offset: 0x00175A72
	public void Reset()
	{
		this.errorText.gameObject.SetActive(false);
		this.terminalControlPromptText.gameObject.SetActive(true);
		this.displayedText = this.defaultText;
	}

	// Token: 0x060045EA RID: 17898 RVA: 0x001778A2 File Offset: 0x00175AA2
	public void SetDetailsScreenForDriver()
	{
		this.displayedText = this.detailsScreenText;
	}

	// Token: 0x060045EB RID: 17899 RVA: 0x001778B0 File Offset: 0x00175AB0
	public void SetDriverName()
	{
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		string text;
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.GetDriverID());
			text = netPlayerByID.DefaultName;
			if (this.useNametags && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			text = ((this.useNametags && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		this.displayedText = "TERMINAL CONTROLLED BY: " + text;
		if (!this.isControlScreen)
		{
			this.displayedText += this.detailsScreenText;
		}
		this.terminalControlPromptText.text = this.displayedText;
	}

	// Token: 0x060045EC RID: 17900 RVA: 0x0017798B File Offset: 0x00175B8B
	public void DisplayError(string errorMessage)
	{
		this.terminalControlPromptText.gameObject.SetActive(false);
		this.errorText.text = errorMessage;
		this.errorText.gameObject.SetActive(true);
	}

	// Token: 0x0400582C RID: 22572
	[SerializeField]
	private TMP_Text errorText;

	// Token: 0x0400582D RID: 22573
	[SerializeField]
	private TMP_Text terminalControlPromptText;

	// Token: 0x0400582E RID: 22574
	[SerializeField]
	private bool isControlScreen = true;

	// Token: 0x0400582F RID: 22575
	[SerializeField]
	private string defaultText = "PRESS THE 'TERMINAL AVAILABLE' BUTTON TO PROCEED.";

	// Token: 0x04005830 RID: 22576
	private string detailsScreenText = "\nMAP DETAILS WILL APPEAR HERE WHEN A MAP IS SELECTED.";

	// Token: 0x04005831 RID: 22577
	private string displayedText = string.Empty;

	// Token: 0x04005832 RID: 22578
	private bool useNametags;
}
