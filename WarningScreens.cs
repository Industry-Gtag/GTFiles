using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

// Token: 0x02000BE2 RID: 3042
public class WarningScreens : MonoBehaviour
{
	// Token: 0x06004C90 RID: 19600 RVA: 0x001983D1 File Offset: 0x001965D1
	private void Awake()
	{
		if (WarningScreens._activeReference == null)
		{
			WarningScreens._activeReference = this;
			return;
		}
		Debug.LogError("[WARNINGS] WarningScreens already exists. Destroying this instance.");
		Object.Destroy(this);
	}

	// Token: 0x06004C91 RID: 19601 RVA: 0x001983F8 File Offset: 0x001965F8
	private async Task<WarningButtonResult> StartWarningScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens._closedMessageBox = false;
		WarningScreens._result = WarningButtonResult.CloseWarning;
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus = await WarningsServer.Instance.FetchPlayerData(cancellationToken);
		WarningButtonResult warningButtonResult;
		if (cancellationToken.IsCancellationRequested || playerAgeGateWarningStatus == null)
		{
			warningButtonResult = WarningButtonResult.None;
		}
		else
		{
			PlayerAgeGateWarningStatus value = playerAgeGateWarningStatus.Value;
			if (value.header.IsNullOrEmpty() || value.body.IsNullOrEmpty())
			{
				Debug.Log("[WARNINGS] Not showing warning screen.");
				warningButtonResult = value.noWarningResult;
			}
			else
			{
				this._messageBox.Header = value.header;
				this._messageBox.Body = value.body;
				this._messageBox.LeftButton = value.leftButtonText;
				this._messageBox.RightButton = value.rightButtonText;
				WarningScreens._leftButtonResult = value.leftButtonResult;
				WarningScreens._rightButtonResult = value.rightButtonResult;
				this._onLeftButtonPressedAction = value.onLeftButtonPressedAction;
				this._onRightButtonPressedAction = value.onRightButtonPressedAction;
				if (this._imageContainerAfter && this._withImageTextBefore && this._imageContainerBefore && this._withImageTextAfter && this._noImageText)
				{
					this._imageContainerAfter.SetActive(value.showImage == EImageVisibility.AfterBody);
					this._imageContainerBefore.SetActive(value.showImage == EImageVisibility.BeforeBody);
					this._withImageTextBefore.text = value.body;
					this._withImageTextBefore.gameObject.SetActive(value.showImage == EImageVisibility.AfterBody);
					this._withImageTextAfter.text = value.body;
					this._withImageTextAfter.gameObject.SetActive(value.showImage == EImageVisibility.BeforeBody);
					this._noImageText.gameObject.SetActive(value.showImage == EImageVisibility.None);
				}
				this._messageBox.gameObject.SetActive(true);
				GameObject canvas = this._messageBox.GetCanvas();
				PrivateUIRoom.AddUI(canvas.transform);
				HandRayController.Instance.EnableHandRays();
				await WarningScreens.WaitForResponse(cancellationToken);
				HandRayController.Instance.DisableHandRays();
				PrivateUIRoom.RemoveUI(canvas.transform);
				this._messageBox.gameObject.SetActive(false);
				warningButtonResult = WarningScreens._result;
			}
		}
		return warningButtonResult;
	}

	// Token: 0x06004C92 RID: 19602 RVA: 0x00198444 File Offset: 0x00196644
	private async Task<WarningButtonResult> StartOptInFollowUpScreenInternal(CancellationToken cancellationToken)
	{
		WarningScreens._closedMessageBox = false;
		WarningScreens._result = WarningButtonResult.CloseWarning;
		PlayerAgeGateWarningStatus? playerAgeGateWarningStatus = await WarningsServer.Instance.GetOptInFollowUpMessage(cancellationToken);
		WarningButtonResult warningButtonResult;
		if (cancellationToken.IsCancellationRequested || playerAgeGateWarningStatus == null)
		{
			warningButtonResult = WarningButtonResult.None;
		}
		else
		{
			Debug.Log("[KID::WARNING_SCREEN] Body: " + playerAgeGateWarningStatus.Value.body);
			this._messageBox.Header = playerAgeGateWarningStatus.Value.header;
			this._messageBox.Body = playerAgeGateWarningStatus.Value.body;
			this._messageBox.LeftButton = playerAgeGateWarningStatus.Value.leftButtonText;
			this._messageBox.RightButton = playerAgeGateWarningStatus.Value.rightButtonText;
			WarningScreens._leftButtonResult = playerAgeGateWarningStatus.Value.leftButtonResult;
			WarningScreens._rightButtonResult = playerAgeGateWarningStatus.Value.rightButtonResult;
			this._onLeftButtonPressedAction = playerAgeGateWarningStatus.Value.onLeftButtonPressedAction;
			this._onRightButtonPressedAction = playerAgeGateWarningStatus.Value.onRightButtonPressedAction;
			if (this._imageContainerAfter && this._withImageTextBefore && this._imageContainerBefore && this._withImageTextAfter && this._noImageText)
			{
				this._imageContainerAfter.SetActive(playerAgeGateWarningStatus.Value.showImage == EImageVisibility.AfterBody);
				this._imageContainerBefore.SetActive(playerAgeGateWarningStatus.Value.showImage == EImageVisibility.BeforeBody);
				this._withImageTextBefore.text = playerAgeGateWarningStatus.Value.body;
				this._withImageTextBefore.gameObject.SetActive(playerAgeGateWarningStatus.Value.showImage == EImageVisibility.AfterBody);
				this._withImageTextAfter.text = playerAgeGateWarningStatus.Value.body;
				this._withImageTextAfter.gameObject.SetActive(playerAgeGateWarningStatus.Value.showImage == EImageVisibility.BeforeBody);
				this._noImageText.gameObject.SetActive(playerAgeGateWarningStatus.Value.showImage == EImageVisibility.None);
			}
			this._messageBox.gameObject.SetActive(true);
			GameObject canvas = this._messageBox.GetCanvas();
			PrivateUIRoom.AddUI(canvas.transform);
			HandRayController.Instance.EnableHandRays();
			await WarningScreens.WaitForResponse(cancellationToken);
			HandRayController.Instance.DisableHandRays();
			PrivateUIRoom.RemoveUI(canvas.transform);
			this._messageBox.gameObject.SetActive(false);
			warningButtonResult = WarningScreens._result;
		}
		return warningButtonResult;
	}

	// Token: 0x06004C93 RID: 19603 RVA: 0x00198490 File Offset: 0x00196690
	public static async Task<WarningButtonResult> StartWarningScreen(CancellationToken cancellationToken)
	{
		return await WarningScreens._activeReference.StartWarningScreenInternal(cancellationToken);
	}

	// Token: 0x06004C94 RID: 19604 RVA: 0x001984D4 File Offset: 0x001966D4
	public static async Task<WarningButtonResult> StartOptInFollowUpScreen(CancellationToken cancellationToken)
	{
		return await WarningScreens._activeReference.StartOptInFollowUpScreenInternal(cancellationToken);
	}

	// Token: 0x06004C95 RID: 19605 RVA: 0x00198518 File Offset: 0x00196718
	private static async Task WaitForResponse(CancellationToken cancellationToken)
	{
		while (!WarningScreens._closedMessageBox)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			await Task.Yield();
		}
	}

	// Token: 0x06004C96 RID: 19606 RVA: 0x0018FAF0 File Offset: 0x0018DCF0
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x06004C97 RID: 19607 RVA: 0x0019855B File Offset: 0x0019675B
	public static void OnLeftButtonClicked()
	{
		WarningScreens._result = WarningScreens._leftButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onLeftButtonPressedAction = activeReference._onLeftButtonPressedAction;
		if (onLeftButtonPressedAction == null)
		{
			return;
		}
		onLeftButtonPressedAction();
	}

	// Token: 0x06004C98 RID: 19608 RVA: 0x00198586 File Offset: 0x00196786
	public static void OnRightButtonClicked()
	{
		WarningScreens._result = WarningScreens._rightButtonResult;
		WarningScreens._closedMessageBox = true;
		WarningScreens activeReference = WarningScreens._activeReference;
		if (activeReference == null)
		{
			return;
		}
		Action onRightButtonPressedAction = activeReference._onRightButtonPressedAction;
		if (onRightButtonPressedAction == null)
		{
			return;
		}
		onRightButtonPressedAction();
	}

	// Token: 0x04005F9E RID: 24478
	private static WarningScreens _activeReference;

	// Token: 0x04005F9F RID: 24479
	[SerializeField]
	private MessageBox _messageBox;

	// Token: 0x04005FA0 RID: 24480
	[SerializeField]
	private GameObject _imageContainerAfter;

	// Token: 0x04005FA1 RID: 24481
	[SerializeField]
	private GameObject _imageContainerBefore;

	// Token: 0x04005FA2 RID: 24482
	[SerializeField]
	private TMP_Text _withImageTextBefore;

	// Token: 0x04005FA3 RID: 24483
	[SerializeField]
	private TMP_Text _withImageTextAfter;

	// Token: 0x04005FA4 RID: 24484
	[SerializeField]
	private TMP_Text _noImageText;

	// Token: 0x04005FA5 RID: 24485
	private Action _onLeftButtonPressedAction;

	// Token: 0x04005FA6 RID: 24486
	private Action _onRightButtonPressedAction;

	// Token: 0x04005FA7 RID: 24487
	private static WarningButtonResult _result;

	// Token: 0x04005FA8 RID: 24488
	private static WarningButtonResult _leftButtonResult;

	// Token: 0x04005FA9 RID: 24489
	private static WarningButtonResult _rightButtonResult;

	// Token: 0x04005FAA RID: 24490
	private static bool _closedMessageBox;
}
