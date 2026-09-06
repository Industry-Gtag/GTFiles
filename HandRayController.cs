using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Token: 0x02000B9F RID: 2975
public class HandRayController : MonoBehaviour
{
	// Token: 0x1700073A RID: 1850
	// (get) Token: 0x06004B1E RID: 19230 RVA: 0x001915EE File Offset: 0x0018F7EE
	public static HandRayController Instance
	{
		get
		{
			if (HandRayController.instance == null)
			{
				HandRayController.instance = Object.FindAnyObjectByType<HandRayController>();
				if (HandRayController.instance == null)
				{
					Debug.LogErrorFormat("[KID::UI::HAND_RAY_CONTROLLER] Not found in scene", Array.Empty<object>());
				}
			}
			return HandRayController.instance;
		}
	}

	// Token: 0x06004B1F RID: 19231 RVA: 0x00191628 File Offset: 0x0018F828
	private void Awake()
	{
		if (HandRayController.instance != null && HandRayController.instance != this)
		{
			Debug.LogErrorFormat(base.gameObject, "[KID::UI::HAND_RAY_CONTROLLER] Duplicate instance of HandRayController", Array.Empty<object>());
			Object.DestroyImmediate(this);
			return;
		}
		HandRayController.instance = this;
	}

	// Token: 0x06004B20 RID: 19232 RVA: 0x00191668 File Offset: 0x0018F868
	private void Start()
	{
		this._leftHandRay.attachTransform = (this._leftHandRay.rayOriginTransform = KIDHandReference.LeftHand.transform);
		this._rightHandRay.attachTransform = (this._rightHandRay.rayOriginTransform = KIDHandReference.RightHand.transform);
		this.DisableHandRays();
		this._activationCounter = 0;
	}

	// Token: 0x06004B21 RID: 19233 RVA: 0x001916C8 File Offset: 0x0018F8C8
	private void OnDisable()
	{
		this.DisableHandRays();
	}

	// Token: 0x06004B22 RID: 19234 RVA: 0x001916D0 File Offset: 0x0018F8D0
	public void EnableHandRays()
	{
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction += this.PostUpdate;
			}
			this.ToggleHands();
		}
		this._activationCounter++;
	}

	// Token: 0x06004B23 RID: 19235 RVA: 0x00191710 File Offset: 0x0018F910
	public void DisableHandRays()
	{
		this._activationCounter--;
		if (this._activationCounter == 0)
		{
			if (ControllerBehaviour.Instance)
			{
				ControllerBehaviour.Instance.OnAction -= this.PostUpdate;
			}
			this.HideHands();
		}
	}

	// Token: 0x06004B24 RID: 19236 RVA: 0x00191750 File Offset: 0x0018F950
	public void PulseActiveHandray(float vibrationStrength, float vibrationDuration)
	{
		if (this._activeHandRay == null)
		{
			return;
		}
		this._activeHandRay.SendHapticImpulse(vibrationStrength, vibrationDuration);
	}

	// Token: 0x06004B25 RID: 19237 RVA: 0x0019176F File Offset: 0x0018F96F
	private void PostUpdate()
	{
		if (!this._hasInitialised)
		{
			return;
		}
		if (this.ActiveHand == HandRayController.HandSide.Left)
		{
			if (ControllerBehaviour.Instance.RightButtonDown)
			{
				this.ToggleHands();
			}
			return;
		}
		if (ControllerBehaviour.Instance.LeftButtonDown)
		{
			this.ToggleHands();
		}
	}

	// Token: 0x06004B26 RID: 19238 RVA: 0x001917A8 File Offset: 0x0018F9A8
	private void ToggleRightHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] RIGHT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._rightHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._rightHandRay;
		}
	}

	// Token: 0x06004B27 RID: 19239 RVA: 0x00191804 File Offset: 0x0018FA04
	private void ToggleLeftHandRay(bool enabled)
	{
		Debug.LogFormat(string.Format("[KID::UI::HAND_RAY_CONTROLLER] LEFT Hand is: {0}. Setting to: {1}", this._rightHandRay.gameObject.activeInHierarchy, enabled), Array.Empty<object>());
		this._leftHandRay.gameObject.SetActive(enabled);
		if (enabled)
		{
			this._activeHandRay = this._leftHandRay;
		}
	}

	// Token: 0x06004B28 RID: 19240 RVA: 0x00191860 File Offset: 0x0018FA60
	private void InitialiseHands()
	{
		Debug.Log("[KID::UI::HAND_RAY_CONTROLLER] Initialising Hands");
		this.ToggleRightHandRay(this.ActiveHand == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(this.ActiveHand == HandRayController.HandSide.Left);
		this._hasInitialised = true;
	}

	// Token: 0x06004B29 RID: 19241 RVA: 0x00191894 File Offset: 0x0018FA94
	private void ToggleHands()
	{
		if (!this._hasInitialised)
		{
			this.InitialiseHands();
			return;
		}
		HandRayController.HandSide handSide = ((this.ActiveHand == HandRayController.HandSide.Left) ? HandRayController.HandSide.Right : HandRayController.HandSide.Left);
		Debug.LogFormat(string.Concat(new string[]
		{
			"[KID::UI::HAND_RAY_CONTROLLER] Setting ActiveHand FROM: [",
			this.ActiveHand.ToString(),
			"] TO: [",
			handSide.ToString(),
			"]"
		}), Array.Empty<object>());
		this.ActiveHand = handSide;
		this.ToggleRightHandRay(handSide == HandRayController.HandSide.Right);
		this.ToggleLeftHandRay(handSide == HandRayController.HandSide.Left);
	}

	// Token: 0x06004B2A RID: 19242 RVA: 0x00191929 File Offset: 0x0018FB29
	private void HideHands()
	{
		this.ToggleRightHandRay(false);
		this.ToggleLeftHandRay(false);
		this._hasInitialised = false;
		this._activeHandRay = null;
	}

	// Token: 0x04005DFE RID: 24062
	[OnEnterPlay_SetNull]
	private static HandRayController instance;

	// Token: 0x04005DFF RID: 24063
	[SerializeField]
	private XRRayInteractor _leftHandRay;

	// Token: 0x04005E00 RID: 24064
	[SerializeField]
	private XRRayInteractor _rightHandRay;

	// Token: 0x04005E01 RID: 24065
	private bool _hasInitialised;

	// Token: 0x04005E02 RID: 24066
	private HandRayController.HandSide ActiveHand = HandRayController.HandSide.Right;

	// Token: 0x04005E03 RID: 24067
	private XRRayInteractor _activeHandRay;

	// Token: 0x04005E04 RID: 24068
	private int _activationCounter;

	// Token: 0x02000BA0 RID: 2976
	private enum HandSide
	{
		// Token: 0x04005E06 RID: 24070
		Left,
		// Token: 0x04005E07 RID: 24071
		Right
	}
}
