using System;
using System.Collections.Generic;
using System.Text;
using GorillaLocomotion;
using TMPro;
using UnityEngine;

// Token: 0x02000C17 RID: 3095
public class ConsentScreen : MonoBehaviour
{
	// Token: 0x06004D6E RID: 19822 RVA: 0x0019CC3C File Offset: 0x0019AE3C
	private void Awake()
	{
		if (ConsentScreen._activeReference == null)
		{
			ConsentScreen._activeReference = this;
		}
		else if (ConsentScreen._activeReference != this)
		{
			return;
		}
		if (this.popupRoot == null || this.promptText == null || this.resultText == null || this.yesButton == null || this.noButton == null)
		{
			Debug.LogError("[ConsentScreen] Popup references are not wired on the prefab; consent will be denied by default");
			this.popupRoot = null;
			return;
		}
		this.yesButton.HoldComplete += delegate
		{
			this.OnChoice(true);
		};
		this.noButton.HoldComplete += delegate
		{
			this.OnChoice(false);
		};
		this.popupRoot.SetActive(false);
	}

	// Token: 0x06004D6F RID: 19823 RVA: 0x0019CCFE File Offset: 0x0019AEFE
	private void OnDestroy()
	{
		if (ConsentScreen._activeReference == this)
		{
			ConsentScreen._activeReference = null;
		}
	}

	// Token: 0x06004D70 RID: 19824 RVA: 0x0019CD14 File Offset: 0x0019AF14
	public static void StartConsentFlow(string itemDisplayName, List<ConsentScreen.ConsentCost> costs, Action<bool, Action<string>> OnConsentChosen)
	{
		if (ConsentScreen._activeReference == null || ConsentScreen._activeReference.popupRoot == null)
		{
			Debug.LogError("[ConsentScreen] No active consent screen instance; denying consent by default");
			if (OnConsentChosen != null)
			{
				OnConsentChosen(false, delegate(string _)
				{
				});
			}
			return;
		}
		ConsentScreen._activeReference.ShowPrompt(itemDisplayName, costs, OnConsentChosen);
	}

	// Token: 0x06004D71 RID: 19825 RVA: 0x0019CD84 File Offset: 0x0019AF84
	private void ShowPrompt(string itemDisplayName, List<ConsentScreen.ConsentCost> costs, Action<bool, Action<string>> onConsentChosen)
	{
		this.pendingCallback = onConsentChosen;
		this.promptText.text = ConsentScreen.ComposePrompt(itemDisplayName, costs);
		this.promptText.gameObject.SetActive(true);
		this.resultText.gameObject.SetActive(false);
		this.SetButtonsVisible(true);
		this.state = ConsentScreen.PopupState.Prompt;
		this.popupRoot.SetActive(true);
		this.UpdatePopupTransform();
		Transform transform = ConsentScreen.ResolveHead();
		this.hasPromptOrigin = transform != null;
		this.promptOrigin = (this.hasPromptOrigin ? transform.position : Vector3.zero);
		this.PlayAppearCue();
	}

	// Token: 0x06004D72 RID: 19826 RVA: 0x0019CE20 File Offset: 0x0019B020
	private void PlayAppearCue()
	{
		if (this.appearSound != null)
		{
			this.appearSound.Play();
		}
		if (GorillaTagger.Instance != null)
		{
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * this.appearHapticScale, this.appearHapticDuration);
		}
	}

	// Token: 0x06004D73 RID: 19827 RVA: 0x0019CE78 File Offset: 0x0019B078
	private static string ComposePrompt(string itemDisplayName, List<ConsentScreen.ConsentCost> costs)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (!string.IsNullOrEmpty(itemDisplayName))
		{
			stringBuilder.Append("<b>Purchase " + itemDisplayName + "?</b>\n\n");
		}
		if (costs != null)
		{
			foreach (ConsentScreen.ConsentCost consentCost in costs)
			{
				string text = (string.IsNullOrEmpty(consentCost.DisplayName) ? "credits" : consentCost.DisplayName);
				stringBuilder.Append(string.Format("<b>You are about to spend {0} {1}.</b>\n\n", consentCost.Amount, text));
				if (consentCost.HasBalance)
				{
					stringBuilder.Append(string.Format("This will leave you with {0} {1}.\n\n", consentCost.CurrentBalance - consentCost.Amount, text));
				}
			}
		}
		stringBuilder.Append("<b>Are you sure?</b>");
		return stringBuilder.ToString();
	}

	// Token: 0x06004D74 RID: 19828 RVA: 0x0019CF60 File Offset: 0x0019B160
	private void OnChoice(bool consented)
	{
		if (this.state != ConsentScreen.PopupState.Prompt)
		{
			return;
		}
		Action<bool, Action<string>> action = this.pendingCallback;
		this.pendingCallback = null;
		if (consented)
		{
			this.SetButtonsVisible(false);
			this.ShowResultText(this.processingText);
			this.state = ConsentScreen.PopupState.Processing;
		}
		else
		{
			this.Hide();
		}
		if (action == null)
		{
			return;
		}
		action(consented, new Action<string>(this.OnAsyncWorkComplete));
	}

	// Token: 0x06004D75 RID: 19829 RVA: 0x0019CFC0 File Offset: 0x0019B1C0
	private async void OnAsyncWorkComplete(string result)
	{
		await Awaitable.MainThreadAsync();
		if (!(this == null) && !(this.popupRoot == null))
		{
			if (string.IsNullOrEmpty(result))
			{
				if (this.state == ConsentScreen.PopupState.Processing)
				{
					this.Hide();
				}
			}
			else if (this.state == ConsentScreen.PopupState.Processing)
			{
				this.SetButtonsVisible(false);
				this.ShowResultText(result);
				this.resultHideAt = Time.time + this.resultDisplaySeconds;
				this.state = ConsentScreen.PopupState.Result;
				this.popupRoot.SetActive(true);
			}
		}
	}

	// Token: 0x06004D76 RID: 19830 RVA: 0x0019CFFF File Offset: 0x0019B1FF
	private void Hide()
	{
		this.state = ConsentScreen.PopupState.Hidden;
		this.pendingCallback = null;
		this.popupRoot.SetActive(false);
	}

	// Token: 0x06004D77 RID: 19831 RVA: 0x0019D01B File Offset: 0x0019B21B
	private void SetButtonsVisible(bool visible)
	{
		this.yesButton.gameObject.SetActive(visible);
		this.noButton.gameObject.SetActive(visible);
		if (visible)
		{
			this.yesButton.ResetHold();
			this.noButton.ResetHold();
		}
	}

	// Token: 0x06004D78 RID: 19832 RVA: 0x0019D058 File Offset: 0x0019B258
	private void ShowResultText(string message)
	{
		this.promptText.gameObject.SetActive(false);
		this.resultText.gameObject.SetActive(true);
		this.resultText.text = message;
	}

	// Token: 0x06004D79 RID: 19833 RVA: 0x0019D088 File Offset: 0x0019B288
	private void Update()
	{
		ConsentScreen.PopupState popupState = this.state;
		if (popupState != ConsentScreen.PopupState.Prompt)
		{
			if (popupState != ConsentScreen.PopupState.Result)
			{
				return;
			}
			if (Time.time >= this.resultHideAt)
			{
				this.Hide();
			}
		}
		else
		{
			Transform transform = ConsentScreen.ResolveHead();
			if (transform != null)
			{
				if (!this.hasPromptOrigin)
				{
					this.promptOrigin = transform.position;
					this.hasPromptOrigin = true;
					return;
				}
				if (Vector3.Distance(transform.position, this.promptOrigin) >= this.dismissDistanceMeters)
				{
					this.OnChoice(false);
					return;
				}
			}
		}
	}

	// Token: 0x06004D7A RID: 19834 RVA: 0x0019D104 File Offset: 0x0019B304
	private void LateUpdate()
	{
		if (this.state == ConsentScreen.PopupState.Hidden)
		{
			return;
		}
		this.UpdatePopupTransform();
	}

	// Token: 0x06004D7B RID: 19835 RVA: 0x0019D118 File Offset: 0x0019B318
	private void UpdatePopupTransform()
	{
		Transform transform = ConsentScreen.ResolveHead();
		Transform transform2 = this.ResolveWatchAnchor();
		float num = ConsentScreen.ResolvePlayerScale();
		Vector3 vector;
		if (transform2 != null)
		{
			vector = transform2.position + Vector3.up * (this.hoverHeightMeters * num);
			if (transform != null)
			{
				Vector3 vector2 = transform.position - vector;
				if (vector2.sqrMagnitude > 0.0001f)
				{
					vector += vector2.normalized * (this.faceOffsetMeters * num);
				}
			}
		}
		else
		{
			if (!(transform != null))
			{
				return;
			}
			vector = transform.position + transform.forward * (this.fallbackForwardMeters * num) - Vector3.up * (this.fallbackDownMeters * num);
		}
		this.popupRoot.transform.position = vector;
		this.popupRoot.transform.localScale = Vector3.one * num;
		if (transform != null)
		{
			Vector3 vector3 = vector - transform.position;
			if (vector3.sqrMagnitude > 0.0001f)
			{
				this.popupRoot.transform.rotation = Quaternion.LookRotation(vector3, Vector3.up);
			}
		}
	}

	// Token: 0x06004D7C RID: 19836 RVA: 0x0019D250 File Offset: 0x0019B450
	private static Transform ResolveHead()
	{
		if (GorillaTagger.Instance != null && GorillaTagger.Instance.mainCamera != null)
		{
			return GorillaTagger.Instance.mainCamera.transform;
		}
		return null;
	}

	// Token: 0x06004D7D RID: 19837 RVA: 0x0019D284 File Offset: 0x0019B484
	private Transform ResolveWatchAnchor()
	{
		if (this.watchAnchor != null)
		{
			return this.watchAnchor;
		}
		if (VRRig.LocalRig != null && VRRig.LocalRig.vStumpReturnWatch != null)
		{
			this.watchAnchor = VRRig.LocalRig.vStumpReturnWatch.transform;
		}
		return this.watchAnchor;
	}

	// Token: 0x06004D7E RID: 19838 RVA: 0x0019D2E0 File Offset: 0x0019B4E0
	private static float ResolvePlayerScale()
	{
		if (VRRig.LocalRig != null)
		{
			return VRRig.LocalRig.scaleFactor;
		}
		if (GTPlayer.Instance != null)
		{
			return GTPlayer.Instance.scale;
		}
		return 1f;
	}

	// Token: 0x040060CE RID: 24782
	[Header("Wiring")]
	[SerializeField]
	private GameObject popupRoot;

	// Token: 0x040060CF RID: 24783
	[SerializeField]
	private TextMeshProUGUI promptText;

	// Token: 0x040060D0 RID: 24784
	[SerializeField]
	private TextMeshProUGUI resultText;

	// Token: 0x040060D1 RID: 24785
	[SerializeField]
	private ConsentHoldButton yesButton;

	// Token: 0x040060D2 RID: 24786
	[SerializeField]
	private ConsentHoldButton noButton;

	// Token: 0x040060D3 RID: 24787
	[Header("Timing")]
	[SerializeField]
	private float resultDisplaySeconds = 3f;

	// Token: 0x040060D4 RID: 24788
	[Header("Dismissal")]
	[Tooltip("Prompt auto-dismisses (counts as NO) when the player moves this many meters from where it appeared.")]
	[SerializeField]
	private float dismissDistanceMeters = 5f;

	// Token: 0x040060D5 RID: 24789
	[Header("Popup Cue")]
	[SerializeField]
	private AudioSource appearSound;

	// Token: 0x040060D6 RID: 24790
	[Tooltip("Haptic buzz on the watch hand when the prompt appears")]
	[SerializeField]
	private float appearHapticScale = 1f;

	// Token: 0x040060D7 RID: 24791
	[SerializeField]
	private float appearHapticDuration = 0.15f;

	// Token: 0x040060D8 RID: 24792
	[Header("Placement")]
	[Tooltip("How far above the VStump watch the popup floats, in meters at 1x player scale.")]
	[SerializeField]
	private float hoverHeightMeters = 0.15f;

	// Token: 0x040060D9 RID: 24793
	[SerializeField]
	private float faceOffsetMeters = 0.08f;

	// Token: 0x040060DA RID: 24794
	[Tooltip("Used only when the watch can't be found: popup floats this far in front of the face.")]
	[SerializeField]
	private float fallbackForwardMeters = 0.45f;

	// Token: 0x040060DB RID: 24795
	[Tooltip("Used only when the watch can't be found: popup sits this far below eye level.")]
	[SerializeField]
	private float fallbackDownMeters = 0.05f;

	// Token: 0x040060DC RID: 24796
	[Header("Text")]
	[SerializeField]
	private string processingText = "Processing...";

	// Token: 0x040060DD RID: 24797
	private static ConsentScreen _activeReference;

	// Token: 0x040060DE RID: 24798
	private ConsentScreen.PopupState state;

	// Token: 0x040060DF RID: 24799
	private Action<bool, Action<string>> pendingCallback;

	// Token: 0x040060E0 RID: 24800
	private Vector3 promptOrigin;

	// Token: 0x040060E1 RID: 24801
	private bool hasPromptOrigin;

	// Token: 0x040060E2 RID: 24802
	private float resultHideAt;

	// Token: 0x040060E3 RID: 24803
	private Transform watchAnchor;

	// Token: 0x02000C18 RID: 3096
	public struct ConsentCost
	{
		// Token: 0x040060E4 RID: 24804
		public string DisplayName;

		// Token: 0x040060E5 RID: 24805
		public int Amount;

		// Token: 0x040060E6 RID: 24806
		public int CurrentBalance;

		// Token: 0x040060E7 RID: 24807
		public bool HasBalance;
	}

	// Token: 0x02000C19 RID: 3097
	private enum PopupState
	{
		// Token: 0x040060E9 RID: 24809
		Hidden,
		// Token: 0x040060EA RID: 24810
		Prompt,
		// Token: 0x040060EB RID: 24811
		Processing,
		// Token: 0x040060EC RID: 24812
		Result
	}
}
