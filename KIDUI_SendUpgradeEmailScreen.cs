using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KID.Model;
using UnityEngine;

// Token: 0x02000BDB RID: 3035
public class KIDUI_SendUpgradeEmailScreen : MonoBehaviour
{
	// Token: 0x06004C7A RID: 19578 RVA: 0x00197E9C File Offset: 0x0019609C
	public async Task SendUpgradeEmail(List<string> requestedPermissions)
	{
		if (requestedPermissions.Count == 0)
		{
			Debug.Log("[KID] Tried requesting 0 permissions. Skipping upgrade email flow.");
			this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
		}
		else
		{
			base.gameObject.SetActive(true);
			this._animatedEllipsis.StartAnimation();
			UpgradeSessionData upgradeSessionData = await KIDManager.TryUpgradeSession(requestedPermissions);
			if (upgradeSessionData == null)
			{
				this.OnFailure("We couldn't get to your information. Please contact Customer Support");
				Debug.LogError("[KID] UpgradeSessionData response was null. Maybe banned.");
			}
			else if (upgradeSessionData.status == SessionStatus.PASS)
			{
				this.OnSuccess();
			}
			else if (upgradeSessionData.status == SessionStatus.CHALLENGE_SESSION_UPGRADE)
			{
				if (KIDManager.CurrentSession.ManagedBy == Session.ManagedByEnum.PLAYER)
				{
					base.gameObject.SetActive(false);
				}
				else
				{
					ValueTuple<bool, string> valueTuple = await KIDManager.TrySendUpgradeSessionChallengeEmail();
					bool item = valueTuple.Item1;
					string item2 = valueTuple.Item2;
					if (item)
					{
						this.OnSuccess();
					}
					else
					{
						this.OnFailure(item2);
					}
				}
			}
			else
			{
				Debug.LogError("[KID] Unexpected session status when upgrading session: " + upgradeSessionData.status.ToString());
				this.OnFailure(null);
			}
		}
	}

	// Token: 0x06004C7B RID: 19579 RVA: 0x00197EE7 File Offset: 0x001960E7
	public void OnCancel()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06004C7C RID: 19580 RVA: 0x00197F01 File Offset: 0x00196101
	private void OnSuccess()
	{
		base.gameObject.SetActive(false);
		this._successScreen.Show(null);
	}

	// Token: 0x06004C7D RID: 19581 RVA: 0x00197F1B File Offset: 0x0019611B
	private void OnFailure(string errorMessage)
	{
		base.gameObject.SetActive(false);
		this._errorScreen.Show(errorMessage);
	}

	// Token: 0x04005F87 RID: 24455
	[SerializeField]
	private KIDUI_AnimatedEllipsis _animatedEllipsis;

	// Token: 0x04005F88 RID: 24456
	[SerializeField]
	private KIDUI_MessageScreen _successScreen;

	// Token: 0x04005F89 RID: 24457
	[SerializeField]
	private KIDUI_MessageScreen _errorScreen;

	// Token: 0x04005F8A RID: 24458
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;
}
