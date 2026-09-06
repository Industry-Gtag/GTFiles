using System;
using TMPro;
using UnityEngine;

// Token: 0x02000BD9 RID: 3033
public class KIDUI_MessageScreen : MonoBehaviour
{
	// Token: 0x06004C72 RID: 19570 RVA: 0x00197DB6 File Offset: 0x00195FB6
	public void Show(string errorMessage)
	{
		base.gameObject.SetActive(true);
		if (errorMessage != null && errorMessage.Length > 0)
		{
			this._errorTxt.text = errorMessage;
		}
	}

	// Token: 0x06004C73 RID: 19571 RVA: 0x00197DDC File Offset: 0x00195FDC
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.Pending);
	}

	// Token: 0x06004C74 RID: 19572 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005F82 RID: 24450
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005F83 RID: 24451
	[SerializeField]
	private TMP_Text _errorTxt;
}
