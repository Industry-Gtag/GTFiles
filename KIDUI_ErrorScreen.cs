using System;
using TMPro;
using UnityEngine;

// Token: 0x02000BD1 RID: 3025
public class KIDUI_ErrorScreen : MonoBehaviour
{
	// Token: 0x06004C33 RID: 19507 RVA: 0x00196275 File Offset: 0x00194475
	public void ShowErrorScreen(string title, string email, string errorMessage)
	{
		this._titleTxt.text = title;
		this._emailTxt.text = email;
		this._errorTxt.text = errorMessage;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004C34 RID: 19508 RVA: 0x001962A7 File Offset: 0x001944A7
	public void OnClose()
	{
		base.gameObject.SetActive(false);
		this._mainScreen.ShowMainScreen(EMainScreenStatus.None);
	}

	// Token: 0x06004C35 RID: 19509 RVA: 0x00193E22 File Offset: 0x00192022
	public void OnQuitGame()
	{
		Application.Quit();
	}

	// Token: 0x06004C36 RID: 19510 RVA: 0x001962C1 File Offset: 0x001944C1
	public void OnBack()
	{
		base.gameObject.SetActive(false);
		this._setupScreen.OnStartSetup();
	}

	// Token: 0x04005F31 RID: 24369
	[SerializeField]
	private TMP_Text _titleTxt;

	// Token: 0x04005F32 RID: 24370
	[SerializeField]
	private TMP_Text _emailTxt;

	// Token: 0x04005F33 RID: 24371
	[SerializeField]
	private TMP_Text _errorTxt;

	// Token: 0x04005F34 RID: 24372
	[SerializeField]
	private KIDUI_MainScreen _mainScreen;

	// Token: 0x04005F35 RID: 24373
	[SerializeField]
	private KIDUI_SetupScreen _setupScreen;
}
