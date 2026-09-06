using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000BA1 RID: 2977
public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	// Token: 0x06004B2C RID: 19244 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void Awake()
	{
	}

	// Token: 0x06004B2D RID: 19245 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x06004B2E RID: 19246 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004B2F RID: 19247 RVA: 0x00191956 File Offset: 0x0018FB56
	public void ShowRestrictedAccessScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004B30 RID: 19248 RVA: 0x00044B04 File Offset: 0x00042D04
	public void OnChangeAgePressed()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04005E08 RID: 24072
	[SerializeField]
	private KIDUIButton _changeAgeButton;

	// Token: 0x04005E09 RID: 24073
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x04005E0A RID: 24074
	private string _submittedEmailAddress;

	// Token: 0x04005E0B RID: 24075
	private CancellationTokenSource _cancellationTokenSource;
}
