using System;
using UnityEngine;

// Token: 0x02000BDA RID: 3034
public class KIDUI_RestrictedAccessScreen : MonoBehaviour
{
	// Token: 0x06004C76 RID: 19574 RVA: 0x00197DF8 File Offset: 0x00195FF8
	public void ShowRestrictedAccessScreen(SessionStatus? sessionStatus)
	{
		base.gameObject.SetActive(true);
		this._pendingStatusIndicator.SetActive(false);
		this._prohibitedStatusIndicator.SetActive(false);
		if (sessionStatus == null)
		{
			return;
		}
		if (sessionStatus != null)
		{
			switch (sessionStatus.GetValueOrDefault())
			{
			case SessionStatus.PASS:
			case SessionStatus.CHALLENGE:
			case SessionStatus.CHALLENGE_SESSION_UPGRADE:
				break;
			case SessionStatus.PROHIBITED:
				this._prohibitedStatusIndicator.SetActive(true);
				return;
			case SessionStatus.PENDING_AGE_APPEAL:
				this._pendingStatusIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06004C77 RID: 19575 RVA: 0x00197E78 File Offset: 0x00196078
	public void OnChangeAgePressed()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		base.gameObject.SetActive(false);
		this._ageAppealScreen.ShowAgeAppealScreen();
	}

	// Token: 0x06004C78 RID: 19576 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005F84 RID: 24452
	[SerializeField]
	private KIDAgeAppeal _ageAppealScreen;

	// Token: 0x04005F85 RID: 24453
	[SerializeField]
	private GameObject _pendingStatusIndicator;

	// Token: 0x04005F86 RID: 24454
	[SerializeField]
	private GameObject _prohibitedStatusIndicator;
}
