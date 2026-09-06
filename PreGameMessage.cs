using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000B95 RID: 2965
public class PreGameMessage : MonoBehaviour
{
	// Token: 0x06004AED RID: 19181 RVA: 0x00190816 File Offset: 0x0018EA16
	private void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
	}

	// Token: 0x06004AEE RID: 19182 RVA: 0x0019083A File Offset: 0x0018EA3A
	private void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance != null)
		{
			instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
		}
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
	}

	// Token: 0x06004AEF RID: 19183 RVA: 0x00190870 File Offset: 0x0018EA70
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x06004AF0 RID: 19184 RVA: 0x00190914 File Offset: 0x0018EB14
	public void ShowMessage(string messageTitle, string messageBody, string messageConfirmationButton, string messageAlternativeButton, Action onConfirmationAction, Action onAlternativeAction, float bodyFontSize = 0.5f)
	{
		this._confirmButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageAlternativeConfirmationTxt.text = messageConfirmationButton;
		this._messageAlternativeButtonTxt.text = messageAlternativeButton;
		this._confirmationAction = onConfirmationAction;
		this._alternativeAction = onAlternativeAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null || this._alternativeAction == null)
		{
			Debug.LogError("[KID] Trying to show a mesasge with multiple buttons, but one or both callbacks are null");
			this._multiButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageAlternativeConfirmationTxt.text) && !string.IsNullOrEmpty(this._messageAlternativeButtonTxt.text))
		{
			this._multiButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
	}

	// Token: 0x06004AF1 RID: 19185 RVA: 0x001909EC File Offset: 0x0018EBEC
	public async Task ShowMessageWithAwait(string messageTitle, string messageBody, string messageConfirmation, Action onConfirmationAction, float bodyFontSize = 0.5f, float buttonHideTimer = 0f)
	{
		this._alternativeAction = null;
		this._multiButtonRoot.SetActive(false);
		this._messageTitleTxt.text = messageTitle;
		this._messageBodyTxt.text = messageBody;
		this._messageConfirmationTxt.text = messageConfirmation;
		this._confirmationAction = onConfirmationAction;
		this._messageBodyTxt.fontSize = bodyFontSize;
		this._hasCompleted = false;
		if (this._confirmationAction == null)
		{
			this._confirmButtonRoot.SetActive(false);
		}
		else if (!string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(true);
		}
		PrivateUIRoom.AddUI(this._uiParent.transform);
		await this.WaitForCompletion();
	}

	// Token: 0x06004AF2 RID: 19186 RVA: 0x00190A5C File Offset: 0x0018EC5C
	public void UpdateMessage(string newMessageBody, string newConfirmButton)
	{
		this._messageBodyTxt.text = newMessageBody;
		this._messageConfirmationTxt.text = newConfirmButton;
		if (string.IsNullOrEmpty(this._messageConfirmationTxt.text))
		{
			this._confirmButtonRoot.SetActive(false);
			return;
		}
		if (this._confirmationAction != null)
		{
			this._confirmButtonRoot.SetActive(true);
		}
	}

	// Token: 0x06004AF3 RID: 19187 RVA: 0x00190AB4 File Offset: 0x0018ECB4
	public void CloseMessage()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
	}

	// Token: 0x06004AF4 RID: 19188 RVA: 0x00190AC8 File Offset: 0x0018ECC8
	private async Task WaitForCompletion()
	{
		do
		{
			await Task.Yield();
		}
		while (!this._hasCompleted);
	}

	// Token: 0x06004AF5 RID: 19189 RVA: 0x00190B0C File Offset: 0x0018ED0C
	private void PostUpdate()
	{
		bool isLeftStick = ControllerBehaviour.Instance.IsLeftStick;
		bool isRightStick = ControllerBehaviour.Instance.IsRightStick;
		bool buttonDown = ControllerBehaviour.Instance.ButtonDown;
		if (this._multiButtonRoot.activeInHierarchy)
		{
			if (isLeftStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarR.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarR.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else if (isRightStick)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnAlternativePressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBarR.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.transform.localScale = new Vector3(0f, 1f, 1f);
				this.progressBarL.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
		if (this._confirmButtonRoot.activeInHierarchy)
		{
			if (buttonDown)
			{
				this.progress += Time.deltaTime / this.holdTime;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
				if (this.progress >= 1f)
				{
					this.OnConfirmedPressed();
					return;
				}
			}
			else
			{
				this.progress = 0f;
				this.progressBar.transform.localScale = new Vector3(Mathf.Clamp01(this.progress), 1f, 1f);
				this.progressBar.textureScale = new Vector2(Mathf.Clamp01(this.progress), -1f);
			}
			return;
		}
	}

	// Token: 0x06004AF6 RID: 19190 RVA: 0x00190DE3 File Offset: 0x0018EFE3
	private void OnConfirmedPressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action confirmationAction = this._confirmationAction;
		if (confirmationAction == null)
		{
			return;
		}
		confirmationAction();
	}

	// Token: 0x06004AF7 RID: 19191 RVA: 0x00190E0C File Offset: 0x0018F00C
	private void OnAlternativePressed()
	{
		PrivateUIRoom.RemoveUI(this._uiParent.transform);
		this._hasCompleted = true;
		Action alternativeAction = this._alternativeAction;
		if (alternativeAction == null)
		{
			return;
		}
		alternativeAction();
	}

	// Token: 0x04005DCA RID: 24010
	[SerializeField]
	private GameObject _uiParent;

	// Token: 0x04005DCB RID: 24011
	[SerializeField]
	private TMP_Text _messageTitleTxt;

	// Token: 0x04005DCC RID: 24012
	[SerializeField]
	private TMP_Text _messageBodyTxt;

	// Token: 0x04005DCD RID: 24013
	[SerializeField]
	private GameObject _confirmButtonRoot;

	// Token: 0x04005DCE RID: 24014
	[SerializeField]
	private GameObject _multiButtonRoot;

	// Token: 0x04005DCF RID: 24015
	[SerializeField]
	private TMP_Text _messageConfirmationTxt;

	// Token: 0x04005DD0 RID: 24016
	[SerializeField]
	private TMP_Text _messageAlternativeConfirmationTxt;

	// Token: 0x04005DD1 RID: 24017
	[SerializeField]
	private TMP_Text _messageAlternativeButtonTxt;

	// Token: 0x04005DD2 RID: 24018
	private Action _confirmationAction;

	// Token: 0x04005DD3 RID: 24019
	private Action _alternativeAction;

	// Token: 0x04005DD4 RID: 24020
	private bool _hasCompleted;

	// Token: 0x04005DD5 RID: 24021
	private float progress;

	// Token: 0x04005DD6 RID: 24022
	[SerializeField]
	private float holdTime;

	// Token: 0x04005DD7 RID: 24023
	[SerializeField]
	private LineRenderer progressBar;

	// Token: 0x04005DD8 RID: 24024
	[SerializeField]
	private LineRenderer progressBarL;

	// Token: 0x04005DD9 RID: 24025
	[SerializeField]
	private LineRenderer progressBarR;
}
