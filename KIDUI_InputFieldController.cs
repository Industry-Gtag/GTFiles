using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using Valve.VR;

// Token: 0x02000BD2 RID: 3026
public class KIDUI_InputFieldController : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	// Token: 0x1700074C RID: 1868
	// (get) Token: 0x06004C38 RID: 19512 RVA: 0x00088695 File Offset: 0x00086895
	private XRUIInputModule InputModule
	{
		get
		{
			return EventSystem.current.currentInputModule as XRUIInputModule;
		}
	}

	// Token: 0x06004C39 RID: 19513 RVA: 0x001962DC File Offset: 0x001944DC
	protected void OnEnable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction += this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Listen(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Listen(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06004C3A RID: 19514 RVA: 0x00196344 File Offset: 0x00194544
	protected void OnDisable()
	{
		if (ControllerBehaviour.Instance)
		{
			ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
		}
		SteamVR_Events.System(EVREventType.VREvent_KeyboardClosed).Remove(new UnityAction<VREvent_t>(this.OnKeyboardClosed));
		SteamVR_Events.System(EVREventType.VREvent_KeyboardCharInput).Remove(new UnityAction<VREvent_t>(this.OnChar));
	}

	// Token: 0x06004C3B RID: 19515 RVA: 0x001963AC File Offset: 0x001945AC
	private void Update()
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		SteamVR.instance.overlay.GetKeyboardText(this._inputStringBuilder, 1024U);
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] String BUilder Says: [" + this._inputStringBuilder.ToString() + "]");
		this._inputField.text = this._inputBuffer;
		this._inputField.stringPosition = this._inputBuffer.Length;
	}

	// Token: 0x06004C3C RID: 19516 RVA: 0x00196424 File Offset: 0x00194624
	private void PostUpdate()
	{
		if (!this._inputField.interactable || !this.inside)
		{
			return;
		}
		if (ControllerBehaviour.Instance && ControllerBehaviour.Instance.TriggerDown)
		{
			string text = string.Concat(new string[]
			{
				"[",
				base.transform.parent.parent.parent.name,
				".",
				base.transform.parent.parent.name,
				".",
				base.transform.parent.name,
				".",
				base.transform.name,
				"]"
			});
			Debug.Log(string.Concat(new string[]
			{
				"[KID::UIBUTTON::DEBUG] ",
				text,
				" - STEAM - OnClick is pressed. Time: [",
				Time.time.ToString(),
				"]"
			}), this);
			this.OnClickedInputField("");
		}
	}

	// Token: 0x06004C3D RID: 19517 RVA: 0x00196538 File Offset: 0x00194738
	public void OnPointerEnter(PointerEventData eventData)
	{
		this.inside = true;
		if (!this._inputField.IsInteractable() || !this._inputField.IsActive())
		{
			return;
		}
		XRRayInteractor xrrayInteractor = this.InputModule.GetInteractor(eventData.pointerId) as XRRayInteractor;
		if (!xrrayInteractor)
		{
			return;
		}
		xrrayInteractor.xrController.SendHapticImpulse(this._highlightedVibrationStrength, this._highlightedVibrationDuration);
	}

	// Token: 0x06004C3E RID: 19518 RVA: 0x0019659F File Offset: 0x0019479F
	public void OnPointerExit(PointerEventData eventData)
	{
		this.inside = false;
	}

	// Token: 0x06004C3F RID: 19519 RVA: 0x001965A8 File Offset: 0x001947A8
	private void OnClickedInputField(string _ = "")
	{
		if (this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUT_FIELD_CONTROLLER] Selecting and Activating Input Field");
		EVROverlayError evroverlayError = OpenVR.Overlay.ShowKeyboard(0, 0, 1U, "Enter Email", 1024U, this._inputField.text ?? "", 0UL);
		if (evroverlayError != EVROverlayError.None)
		{
			Debug.LogError("[KID::INPUT_FIELD_CONTROLLER] Failed to open keyboard. Resulted with error: [" + evroverlayError.ToString() + "]");
			return;
		}
		this._inputBuffer = this._inputField.text ?? "";
		this.keyboardShowing = true;
		HandRayController.Instance.DisableHandRays();
	}

	// Token: 0x06004C40 RID: 19520 RVA: 0x00196648 File Offset: 0x00194848
	private void OnChar(VREvent_t ev)
	{
		if (!this.keyboardShowing)
		{
			return;
		}
		char c = ev.data.keyboard.cNewInput[0];
		if (c == '\b')
		{
			this._inputBuffer = this._inputBuffer.Remove(this._inputBuffer.Length - 1, 1);
			return;
		}
		if (this.IsIllegalChar(c))
		{
			return;
		}
		this._inputBuffer += c.ToString();
	}

	// Token: 0x06004C41 RID: 19521 RVA: 0x001966BC File Offset: 0x001948BC
	private void OnKeyboardClosed(VREvent_t ev)
	{
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Trying to close Keyboard");
		if (!this.keyboardShowing)
		{
			return;
		}
		Debug.Log("[KID::INPUTFIELD_CONTROLLER] Closing Keyboard");
		OpenVR.Overlay.HideKeyboard();
		this._inputField.text = this._inputBuffer;
		this._inputField.DeactivateInputField(false);
		HandRayController.Instance.EnableHandRays();
		this.keyboardShowing = false;
	}

	// Token: 0x06004C42 RID: 19522 RVA: 0x0019671E File Offset: 0x0019491E
	private bool IsIllegalChar(char c)
	{
		return c == '\t' || c == '\n';
	}

	// Token: 0x04005F36 RID: 24374
	[Header("Haptics")]
	[SerializeField]
	private float _highlightedVibrationStrength = 0.1f;

	// Token: 0x04005F37 RID: 24375
	[SerializeField]
	private float _highlightedVibrationDuration = 0.1f;

	// Token: 0x04005F38 RID: 24376
	[Header("Steam Settings")]
	[SerializeField]
	private TMP_InputField _inputField;

	// Token: 0x04005F39 RID: 24377
	[SerializeField]
	private UXSettings _cbUXSettings;

	// Token: 0x04005F3A RID: 24378
	public bool testMinimal;

	// Token: 0x04005F3B RID: 24379
	public bool minimalMode;

	// Token: 0x04005F3C RID: 24380
	private bool inside;

	// Token: 0x04005F3D RID: 24381
	private bool keyboardShowing;

	// Token: 0x04005F3E RID: 24382
	private bool _canTrigger = true;

	// Token: 0x04005F3F RID: 24383
	private string _testStr = string.Empty;

	// Token: 0x04005F40 RID: 24384
	private string previousStr = string.Empty;

	// Token: 0x04005F41 RID: 24385
	private StringBuilder _inputStringBuilder = new StringBuilder(1024);

	// Token: 0x04005F42 RID: 24386
	private string _inputBuffer = "";
}
