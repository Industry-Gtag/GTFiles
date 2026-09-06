using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000615 RID: 1557
public class MonkeBallTeamSelector : MonoBehaviour
{
	// Token: 0x060026E9 RID: 9961 RVA: 0x000CDFD2 File Offset: 0x000CC1D2
	public void Awake()
	{
		this._setTeamButton.onPressButton.AddListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x060026EA RID: 9962 RVA: 0x000CDFF0 File Offset: 0x000CC1F0
	public void OnDestroy()
	{
		this._setTeamButton.onPressButton.RemoveListener(new UnityAction(this.OnSelect));
	}

	// Token: 0x060026EB RID: 9963 RVA: 0x000CE00E File Offset: 0x000CC20E
	private void OnSelect()
	{
		MonkeBallGame.Instance.RequestSetTeam(this.teamId);
	}

	// Token: 0x04003260 RID: 12896
	public int teamId;

	// Token: 0x04003261 RID: 12897
	[SerializeField]
	private GorillaPressableButton _setTeamButton;
}
