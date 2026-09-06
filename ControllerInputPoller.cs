using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaTag;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

// Token: 0x02000677 RID: 1655
public class ControllerInputPoller : MonoBehaviour
{
	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x06002942 RID: 10562 RVA: 0x000DFCF2 File Offset: 0x000DDEF2
	public bool LeftHandValid
	{
		get
		{
			return this.leftControllerIsValid || this.handTrackingActive;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x06002943 RID: 10563 RVA: 0x000DFD04 File Offset: 0x000DDF04
	public bool RightHandValid
	{
		get
		{
			return this.rightControllerIsValid || this.handTrackingActive;
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x06002944 RID: 10564 RVA: 0x000DFD16 File Offset: 0x000DDF16
	[DebugReadout]
	public bool leftIndexPressed
	{
		get
		{
			return this._leftIndexPressed;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002945 RID: 10565 RVA: 0x000DFD1E File Offset: 0x000DDF1E
	[DebugReadout]
	public bool leftIndexReleased
	{
		get
		{
			return this._leftIndexReleased;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002946 RID: 10566 RVA: 0x000DFD26 File Offset: 0x000DDF26
	[DebugReadout]
	public bool rightIndexPressed
	{
		get
		{
			return this._rightIndexPressed;
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002947 RID: 10567 RVA: 0x000DFD2E File Offset: 0x000DDF2E
	[DebugReadout]
	public bool rightIndexReleased
	{
		get
		{
			return this._rightIndexReleased;
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002948 RID: 10568 RVA: 0x000DFD36 File Offset: 0x000DDF36
	[DebugReadout]
	public bool leftIndexPressedThisFrame
	{
		get
		{
			return this._leftIndexPressedThisFrame;
		}
	}

	// Token: 0x17000422 RID: 1058
	// (get) Token: 0x06002949 RID: 10569 RVA: 0x000DFD3E File Offset: 0x000DDF3E
	[DebugReadout]
	public bool leftIndexReleasedThisFrame
	{
		get
		{
			return this._leftIndexReleasedThisFrame;
		}
	}

	// Token: 0x17000423 RID: 1059
	// (get) Token: 0x0600294A RID: 10570 RVA: 0x000DFD46 File Offset: 0x000DDF46
	[DebugReadout]
	public bool rightIndexPressedThisFrame
	{
		get
		{
			return this._rightIndexPressedThisFrame;
		}
	}

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x0600294B RID: 10571 RVA: 0x000DFD4E File Offset: 0x000DDF4E
	[DebugReadout]
	public bool rightIndexReleasedThisFrame
	{
		get
		{
			return this._rightIndexReleasedThisFrame;
		}
	}

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x0600294C RID: 10572 RVA: 0x000DFD56 File Offset: 0x000DDF56
	[DebugReadout]
	public Vector3 leftVelocity
	{
		get
		{
			return this._leftVelocity;
		}
	}

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x0600294D RID: 10573 RVA: 0x000DFD5E File Offset: 0x000DDF5E
	[DebugReadout]
	public Vector3 rightVelocity
	{
		get
		{
			return this._rightVelocity;
		}
	}

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x0600294E RID: 10574 RVA: 0x000DFD66 File Offset: 0x000DDF66
	[DebugReadout]
	public Vector3 leftAngularVelocity
	{
		get
		{
			return this._leftAngularVelocity;
		}
	}

	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600294F RID: 10575 RVA: 0x000DFD6E File Offset: 0x000DDF6E
	[DebugReadout]
	public Vector3 rightAngularVelocity
	{
		get
		{
			return this._rightAngularVelocity;
		}
	}

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x06002950 RID: 10576 RVA: 0x000DFD76 File Offset: 0x000DDF76
	// (set) Token: 0x06002951 RID: 10577 RVA: 0x000DFD7E File Offset: 0x000DDF7E
	public GorillaControllerType controllerType { get; private set; }

	// Token: 0x06002952 RID: 10578 RVA: 0x000DFD87 File Offset: 0x000DDF87
	private void Awake()
	{
		if (ControllerInputPoller.instance == null)
		{
			ControllerInputPoller.instance = this;
			return;
		}
		if (ControllerInputPoller.instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06002953 RID: 10579 RVA: 0x000DFDBC File Offset: 0x000DDFBC
	public static void AddUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Add(callback);
	}

	// Token: 0x06002954 RID: 10580 RVA: 0x000DFE24 File Offset: 0x000DE024
	public static void RemoveUpdateCallback(Action callback)
	{
		if (!ControllerInputPoller.instance.didModifyOnUpdate)
		{
			ControllerInputPoller.instance.onUpdateNext.Clear();
			ControllerInputPoller.instance.onUpdateNext.AddRange(ControllerInputPoller.instance.onUpdate);
			ControllerInputPoller.instance.didModifyOnUpdate = true;
		}
		ControllerInputPoller.instance.onUpdateNext.Remove(callback);
	}

	// Token: 0x06002955 RID: 10581 RVA: 0x000DFE90 File Offset: 0x000DE090
	public void LateUpdate()
	{
		this.leftControllerIsValid = this.leftControllerDevice.isValid;
		if (!this.leftControllerIsValid)
		{
			this.leftControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
			this.leftControllerIsValid = this.leftControllerDevice.isValid;
			if (this.leftControllerIsValid)
			{
				this.controllerType = GorillaControllerType.OCULUS_DEFAULT;
				if (this.leftControllerDevice.name.ToLower().Contains("knuckles"))
				{
					this.controllerType = GorillaControllerType.INDEX;
				}
				Debug.Log(string.Format("Found left controller: {0} ControllerType: {1}", this.leftControllerDevice.name, this.controllerType));
			}
		}
		this.rightControllerIsValid = this.rightControllerDevice.isValid;
		if (!this.rightControllerIsValid)
		{
			this.rightControllerDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		}
		if (!this.headDevice.isValid)
		{
			this.headDevice = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
		}
		InputDevice inputDevice = this.leftControllerDevice;
		InputDevice inputDevice2 = this.rightControllerDevice;
		InputDevice inputDevice3 = this.headDevice;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.leftControllerPrimaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.leftControllerSecondaryButton);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.leftControllerPrimaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.leftControllerSecondaryButtonTouch);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.leftControllerGripFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.leftControllerIndexFloat);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.leftControllerPosition);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.leftControllerRotation);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.leftControllerPrimary2DAxis);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.leftControllerTriggerButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out this.rightControllerPrimaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out this.rightControllerSecondaryButton);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out this.rightControllerPrimaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out this.rightControllerSecondaryButtonTouch);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.grip, out this.rightControllerGripFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.trigger, out this.rightControllerIndexFloat);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.rightControllerPosition);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.rightControllerRotation);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out this.rightControllerPrimary2DAxis);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.triggerButton, out this.rightControllerTriggerButton);
		this.leftControllerPrimaryButton = SteamVR_Actions.gorillaTag_LeftPrimaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButton = SteamVR_Actions.gorillaTag_LeftSecondaryClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_LeftPrimaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_LeftSecondaryTouch.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerGripFloat = SteamVR_Actions.gorillaTag_LeftGripFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerIndexFloat = SteamVR_Actions.gorillaTag_LeftTriggerFloat.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.leftControllerTriggerButton = SteamVR_Actions.gorillaTag_LeftTriggerClick.GetState(SteamVR_Input_Sources.LeftHand);
		this.leftControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_LeftJoystick2DAxis.GetAxis(SteamVR_Input_Sources.LeftHand);
		this.rightControllerPrimaryButton = SteamVR_Actions.gorillaTag_RightPrimaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButton = SteamVR_Actions.gorillaTag_RightSecondaryClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimaryButtonTouch = SteamVR_Actions.gorillaTag_RightPrimaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerSecondaryButtonTouch = SteamVR_Actions.gorillaTag_RightSecondaryTouch.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerGripFloat = SteamVR_Actions.gorillaTag_RightGripFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerIndexFloat = SteamVR_Actions.gorillaTag_RightTriggerFloat.GetAxis(SteamVR_Input_Sources.RightHand);
		this.rightControllerTriggerButton = SteamVR_Actions.gorillaTag_RightTriggerClick.GetState(SteamVR_Input_Sources.RightHand);
		this.rightControllerPrimary2DAxis = SteamVR_Actions.gorillaTag_RightJoystick2DAxis.GetAxis(SteamVR_Input_Sources.RightHand);
		this.headDevice.TryGetFeatureValue(CommonUsages.devicePosition, out this.headPosition);
		this.headDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out this.headRotation);
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
		}
		else if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
		this.handTrackingActive = false;
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out this._leftVelocity);
		this.leftControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out this._leftAngularVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceVelocity, out this._rightVelocity);
		this.rightControllerDevice.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out this._rightAngularVelocity);
		this._UpdatePressFlags();
		if (this.didModifyOnUpdate)
		{
			List<Action> list = this.onUpdateNext;
			List<Action> list2 = this.onUpdate;
			this.onUpdate = list;
			this.onUpdateNext = list2;
			this.didModifyOnUpdate = false;
		}
		foreach (Action action in this.onUpdate)
		{
			action();
		}
	}

	// Token: 0x06002956 RID: 10582 RVA: 0x000E0488 File Offset: 0x000DE688
	private void CalculateGrabState(float grabValue, ref bool grab, ref bool grabRelease, out bool grabMomentary, out bool grabReleaseMomentary, float grabThreshold, float grabReleaseThreshold)
	{
		bool flag = grabValue >= grabThreshold;
		bool flag2 = grabValue <= grabReleaseThreshold;
		grabMomentary = flag && !grab;
		grabReleaseMomentary = flag2 && !grabRelease;
		grab = flag;
		grabRelease = flag2;
	}

	// Token: 0x06002957 RID: 10583 RVA: 0x000E04CC File Offset: 0x000DE6CC
	public void RecalculateGrabState()
	{
		this.CalculateGrabState(this.leftControllerIndexFloat, ref this._leftIndexPressed, ref this._leftIndexReleased, out this._leftIndexPressedThisFrame, out this._leftIndexReleasedThisFrame, 0.75f, 0.65f);
		this.CalculateGrabState(this.rightControllerIndexFloat, ref this._rightIndexPressed, ref this._rightIndexReleased, out this._rightIndexPressedThisFrame, out this._rightIndexReleasedThisFrame, 0.75f, 0.65f);
		if (this.controllerType == GorillaControllerType.OCULUS_DEFAULT)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.75f, 0.65f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.75f, 0.65f);
			return;
		}
		if (this.controllerType == GorillaControllerType.INDEX)
		{
			this.CalculateGrabState(this.leftControllerGripFloat, ref this.leftGrab, ref this.leftGrabRelease, out this.leftGrabMomentary, out this.leftGrabReleaseMomentary, 0.1f, 0.01f);
			this.CalculateGrabState(this.rightControllerGripFloat, ref this.rightGrab, ref this.rightGrabRelease, out this.rightGrabMomentary, out this.rightGrabReleaseMomentary, 0.1f, 0.01f);
		}
	}

	// Token: 0x06002958 RID: 10584 RVA: 0x000E05FF File Offset: 0x000DE7FF
	public static bool HandTrackingActive()
	{
		return ControllerInputPoller.instance.handTrackingActive;
	}

	// Token: 0x06002959 RID: 10585 RVA: 0x000E060D File Offset: 0x000DE80D
	public static bool GetIndexPressed(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.rightIndexPressed;
		}
		return ControllerInputPoller.instance.leftIndexPressed;
	}

	// Token: 0x0600295A RID: 10586 RVA: 0x000E0632 File Offset: 0x000DE832
	public static bool GetIndexReleased(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.rightIndexReleased;
		}
		return ControllerInputPoller.instance.leftIndexReleased;
	}

	// Token: 0x0600295B RID: 10587 RVA: 0x000E0657 File Offset: 0x000DE857
	public static bool GetIndexPressedThisFrame(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.leftIndexPressedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexPressedThisFrame;
	}

	// Token: 0x0600295C RID: 10588 RVA: 0x000E067C File Offset: 0x000DE87C
	public static bool GetIndexReleasedThisFrame(XRNode node)
	{
		if (node != XRNode.LeftHand)
		{
			return node == XRNode.RightHand && ControllerInputPoller.instance.leftIndexReleasedThisFrame;
		}
		return ControllerInputPoller.instance.leftIndexReleasedThisFrame;
	}

	// Token: 0x0600295D RID: 10589 RVA: 0x000E06A1 File Offset: 0x000DE8A1
	public static bool GetGrab(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrab;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrab;
	}

	// Token: 0x0600295E RID: 10590 RVA: 0x000E06C6 File Offset: 0x000DE8C6
	public static bool GetGrabRelease(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabRelease;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabRelease;
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x000E06EB File Offset: 0x000DE8EB
	public static bool GetGrabMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabMomentary;
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x000E0710 File Offset: 0x000DE910
	public static bool GetGrabReleaseMomentary(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftGrabReleaseMomentary;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightGrabReleaseMomentary;
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x000E0735 File Offset: 0x000DE935
	public static Vector2 Primary2DAxis(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimary2DAxis;
		}
		return ControllerInputPoller.instance.rightControllerPrimary2DAxis;
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x000E0754 File Offset: 0x000DE954
	public static bool PrimaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButton;
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x000E0779 File Offset: 0x000DE979
	public static bool SecondaryButtonPress(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButton;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButton;
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000E079E File Offset: 0x000DE99E
	public static bool PrimaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000E07C3 File Offset: 0x000DE9C3
	public static bool SecondaryButtonTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000E07E8 File Offset: 0x000DE9E8
	public static float GripFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerGripFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerGripFloat;
		}
		return 0f;
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000E0811 File Offset: 0x000DEA11
	public static float TriggerFloat(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexFloat;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexFloat;
		}
		return 0f;
	}

	// Token: 0x06002968 RID: 10600 RVA: 0x000E083A File Offset: 0x000DEA3A
	public static float TriggerTouch(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerIndexTouch;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerIndexTouch;
		}
		return 0f;
	}

	// Token: 0x06002969 RID: 10601 RVA: 0x000E0863 File Offset: 0x000DEA63
	public static Vector3 DevicePosition(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headPosition;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerPosition;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerPosition;
		}
		return Vector3.zero;
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x000E089D File Offset: 0x000DEA9D
	public static Quaternion DeviceRotation(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headRotation;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerRotation;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightControllerRotation;
		}
		return Quaternion.identity;
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x000E08D7 File Offset: 0x000DEAD7
	public static Vector3 DeviceVelocity(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftVelocity;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x000E0900 File Offset: 0x000DEB00
	public static Vector3 DeviceAngularVelocity(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftAngularVelocity;
		}
		if (node == XRNode.RightHand)
		{
			return ControllerInputPoller.instance.rightAngularVelocity;
		}
		return Vector3.zero;
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x000E092C File Offset: 0x000DEB2C
	public static bool PositionValid(XRNode node)
	{
		if (node == XRNode.Head)
		{
			return ControllerInputPoller.instance.headDevice.isValid;
		}
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftControllerDevice.isValid;
		}
		return node == XRNode.RightHand && ControllerInputPoller.instance.rightControllerDevice.isValid;
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x000E097C File Offset: 0x000DEB7C
	public static bool HasPressFlags(XRNode node, EControllerInputPressFlags inputStateFlags)
	{
		EControllerInputPressFlags inputStateFlags2 = ControllerInputPoller.GetInputStateFlags(node);
		return inputStateFlags != EControllerInputPressFlags.None && (inputStateFlags2 & inputStateFlags) == inputStateFlags;
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x0600296F RID: 10607 RVA: 0x000E099B File Offset: 0x000DEB9B
	// (set) Token: 0x06002970 RID: 10608 RVA: 0x000E09A3 File Offset: 0x000DEBA3
	public EControllerInputPressFlags leftPressFlags { get; private set; }

	// Token: 0x1700042B RID: 1067
	// (get) Token: 0x06002971 RID: 10609 RVA: 0x000E09AC File Offset: 0x000DEBAC
	// (set) Token: 0x06002972 RID: 10610 RVA: 0x000E09B4 File Offset: 0x000DEBB4
	public EControllerInputPressFlags rightPressFlags { get; private set; }

	// Token: 0x1700042C RID: 1068
	// (get) Token: 0x06002973 RID: 10611 RVA: 0x000E09BD File Offset: 0x000DEBBD
	// (set) Token: 0x06002974 RID: 10612 RVA: 0x000E09C5 File Offset: 0x000DEBC5
	public EControllerInputPressFlags leftPressFlagsLastFrame { get; private set; }

	// Token: 0x1700042D RID: 1069
	// (get) Token: 0x06002975 RID: 10613 RVA: 0x000E09CE File Offset: 0x000DEBCE
	// (set) Token: 0x06002976 RID: 10614 RVA: 0x000E09D6 File Offset: 0x000DEBD6
	public EControllerInputPressFlags rightPressFlagsLastFrame { get; private set; }

	// Token: 0x06002977 RID: 10615 RVA: 0x000E09DF File Offset: 0x000DEBDF
	public static EControllerInputPressFlags GetInputStateFlags(XRNode node)
	{
		if (node == XRNode.LeftHand)
		{
			return ControllerInputPoller.instance.leftPressFlags;
		}
		if (node != XRNode.RightHand)
		{
			return EControllerInputPressFlags.None;
		}
		return ControllerInputPoller.instance.rightPressFlags;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x000E0A04 File Offset: 0x000DEC04
	public static void AddCallbackOnPressStart(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, flags, callback);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x000E0A12 File Offset: 0x000DEC12
	public static void AddCallbackOnPressEnd(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, flags, callback);
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x000E0A20 File Offset: 0x000DEC20
	public static void AddCallbackOnPressUpdate(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._AddInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, flags, callback);
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000E0A30 File Offset: 0x000DEC30
	private static void _AddInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
	{
		if (callback == null || flags == EControllerInputPressFlags.None)
		{
			return;
		}
		if (ref_callbacksInfo.list.Capacity <= ref_callbacksInfo.list.Count)
		{
			ref_callbacksInfo.list.Capacity = ref_callbacksInfo.list.Count * 2;
		}
		ref_callbacksInfo.list.Add(new ControllerInputPoller._InputCallback(flags, callback));
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000E0A86 File Offset: 0x000DEC86
	public static void RemoveCallbackOnPressStart(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressStart, callback);
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000E0A93 File Offset: 0x000DEC93
	public static void RemoveCallbackOnPressEnd(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressEnd, callback);
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x000E0AA0 File Offset: 0x000DECA0
	public static void RemoveCallbackOnPressUpdate(Action<EHandednessFlags> callback)
	{
		ControllerInputPoller._RemoveInputStateCallback(ref ControllerInputPoller._g_callbacks_onPressUpdate, callback);
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x000E0AB0 File Offset: 0x000DECB0
	private static void _RemoveInputStateCallback(ref ControllerInputPoller._InputCallbacksCadenceInfo ref_callbacksInfo, Action<EHandednessFlags> callback)
	{
		if (callback == null)
		{
			return;
		}
		ref_callbacksInfo.list.RemoveAll((ControllerInputPoller._InputCallback sub) => sub.callback == callback);
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x000E0AEC File Offset: 0x000DECEC
	private void _UpdatePressFlags()
	{
		this.leftPressFlagsLastFrame = this.leftPressFlags;
		this.leftPressFlags = (this.leftIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.leftGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.leftControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.leftControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None);
		this.rightPressFlagsLastFrame = this.rightPressFlags;
		this.rightPressFlags = (this.rightIndexPressed ? EControllerInputPressFlags.Index : EControllerInputPressFlags.None) | (this.rightGrab ? EControllerInputPressFlags.Grip : EControllerInputPressFlags.None) | (this.rightControllerPrimaryButton ? EControllerInputPressFlags.Primary : EControllerInputPressFlags.None) | (this.rightControllerSecondaryButton ? EControllerInputPressFlags.Secondary : EControllerInputPressFlags.None);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressStart, ControllerInputPoller._EPressCadence.Start, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressEnd, ControllerInputPoller._EPressCadence.End, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
		ControllerInputPoller._UpdatePressFlags_Callbacks(ref ControllerInputPoller._g_callbacks_onPressUpdate, ControllerInputPoller._EPressCadence.Held, this.leftPressFlags, this.leftPressFlagsLastFrame, this.rightPressFlags, this.rightPressFlagsLastFrame);
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x000E0BEC File Offset: 0x000DEDEC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void _UpdatePressFlags_Callbacks(ref ControllerInputPoller._InputCallbacksCadenceInfo callbacksInfo, ControllerInputPoller._EPressCadence cadence, EControllerInputPressFlags lFlags_now, EControllerInputPressFlags lFlags_old, EControllerInputPressFlags rFlags_now, EControllerInputPressFlags rFlags_old)
	{
		for (int i = 0; i < callbacksInfo.list.Count; i++)
		{
			EControllerInputPressFlags flags = callbacksInfo.list[i].flags;
			Action<EHandednessFlags> callback = callbacksInfo.list[i].callback;
			EHandednessFlags ehandednessFlags = ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Left, cadence, flags, lFlags_now, lFlags_old) | ControllerInputPoller._IsHandContributingToPressCadence(EHandednessFlags.Right, cadence, flags, rFlags_now, rFlags_old);
			if (ehandednessFlags != EHandednessFlags.None && callback != null)
			{
				try
				{
					callbacksInfo.list[i].callback(ehandednessFlags);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
		}
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x000E0C84 File Offset: 0x000DEE84
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static EHandednessFlags _IsHandContributingToPressCadence(EHandednessFlags hand, ControllerInputPoller._EPressCadence pressCadence, EControllerInputPressFlags cbFlags, EControllerInputPressFlags flags_now, EControllerInputPressFlags flags_old)
	{
		if ((pressCadence != ControllerInputPoller._EPressCadence.Held || (cbFlags & flags_now) != cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.Start || (cbFlags & flags_now) != cbFlags || (cbFlags & flags_old) == cbFlags) && (pressCadence != ControllerInputPoller._EPressCadence.End || (cbFlags & flags_now) == cbFlags || (cbFlags & flags_old) != cbFlags))
		{
			return EHandednessFlags.None;
		}
		return hand;
	}

	// Token: 0x040035D5 RID: 13781
	public const int k_defaultExecutionOrder = -400;

	// Token: 0x040035D6 RID: 13782
	[OnEnterPlay_SetNull]
	public static volatile ControllerInputPoller instance;

	// Token: 0x040035D7 RID: 13783
	public float leftControllerIndexFloat;

	// Token: 0x040035D8 RID: 13784
	public float leftControllerGripFloat;

	// Token: 0x040035D9 RID: 13785
	public float rightControllerIndexFloat;

	// Token: 0x040035DA RID: 13786
	public float rightControllerGripFloat;

	// Token: 0x040035DB RID: 13787
	public float leftControllerIndexTouch;

	// Token: 0x040035DC RID: 13788
	public float rightControllerIndexTouch;

	// Token: 0x040035DD RID: 13789
	public float rightStickLRFloat;

	// Token: 0x040035DE RID: 13790
	public Vector3 leftControllerPosition;

	// Token: 0x040035DF RID: 13791
	public Vector3 rightControllerPosition;

	// Token: 0x040035E0 RID: 13792
	public Vector3 headPosition;

	// Token: 0x040035E1 RID: 13793
	public Quaternion leftControllerRotation;

	// Token: 0x040035E2 RID: 13794
	public Quaternion rightControllerRotation;

	// Token: 0x040035E3 RID: 13795
	public Quaternion headRotation;

	// Token: 0x040035E4 RID: 13796
	public InputDevice leftControllerDevice;

	// Token: 0x040035E5 RID: 13797
	public InputDevice rightControllerDevice;

	// Token: 0x040035E6 RID: 13798
	public InputDevice headDevice;

	// Token: 0x040035E7 RID: 13799
	public bool leftControllerIsValid;

	// Token: 0x040035E8 RID: 13800
	public bool rightControllerIsValid;

	// Token: 0x040035E9 RID: 13801
	public bool handTrackingActive;

	// Token: 0x040035EA RID: 13802
	public bool leftControllerPrimaryButton;

	// Token: 0x040035EB RID: 13803
	public bool leftControllerSecondaryButton;

	// Token: 0x040035EC RID: 13804
	public bool rightControllerPrimaryButton;

	// Token: 0x040035ED RID: 13805
	public bool rightControllerSecondaryButton;

	// Token: 0x040035EE RID: 13806
	public bool leftControllerPrimaryButtonTouch;

	// Token: 0x040035EF RID: 13807
	public bool leftControllerSecondaryButtonTouch;

	// Token: 0x040035F0 RID: 13808
	public bool rightControllerPrimaryButtonTouch;

	// Token: 0x040035F1 RID: 13809
	public bool rightControllerSecondaryButtonTouch;

	// Token: 0x040035F2 RID: 13810
	public bool leftControllerTriggerButton;

	// Token: 0x040035F3 RID: 13811
	public bool rightControllerTriggerButton;

	// Token: 0x040035F4 RID: 13812
	public bool leftGrab;

	// Token: 0x040035F5 RID: 13813
	public bool leftGrabRelease;

	// Token: 0x040035F6 RID: 13814
	public bool rightGrab;

	// Token: 0x040035F7 RID: 13815
	public bool rightGrabRelease;

	// Token: 0x040035F8 RID: 13816
	public bool leftGrabMomentary;

	// Token: 0x040035F9 RID: 13817
	public bool leftGrabReleaseMomentary;

	// Token: 0x040035FA RID: 13818
	public bool rightGrabMomentary;

	// Token: 0x040035FB RID: 13819
	public bool rightGrabReleaseMomentary;

	// Token: 0x040035FC RID: 13820
	private bool _leftIndexPressed;

	// Token: 0x040035FD RID: 13821
	private bool _leftIndexReleased;

	// Token: 0x040035FE RID: 13822
	private bool _rightIndexPressed;

	// Token: 0x040035FF RID: 13823
	private bool _rightIndexReleased;

	// Token: 0x04003600 RID: 13824
	private bool _leftIndexPressedThisFrame;

	// Token: 0x04003601 RID: 13825
	private bool _leftIndexReleasedThisFrame;

	// Token: 0x04003602 RID: 13826
	private bool _rightIndexPressedThisFrame;

	// Token: 0x04003603 RID: 13827
	private bool _rightIndexReleasedThisFrame;

	// Token: 0x04003604 RID: 13828
	private Vector3 _leftVelocity;

	// Token: 0x04003605 RID: 13829
	private Vector3 _rightVelocity;

	// Token: 0x04003606 RID: 13830
	private Vector3 _leftAngularVelocity;

	// Token: 0x04003607 RID: 13831
	private Vector3 _rightAngularVelocity;

	// Token: 0x04003609 RID: 13833
	public Vector2 leftControllerPrimary2DAxis;

	// Token: 0x0400360A RID: 13834
	public Vector2 rightControllerPrimary2DAxis;

	// Token: 0x0400360B RID: 13835
	public AnimationCurve handTriggerCurve;

	// Token: 0x0400360C RID: 13836
	public AnimationCurve handGripCurve;

	// Token: 0x0400360D RID: 13837
	private List<Action> onUpdate = new List<Action>();

	// Token: 0x0400360E RID: 13838
	private List<Action> onUpdateNext = new List<Action>();

	// Token: 0x0400360F RID: 13839
	private bool didModifyOnUpdate;

	// Token: 0x04003610 RID: 13840
	public Vector3 leftHandOffset = new Vector3(0.01f, -0.16f, 0f);

	// Token: 0x04003611 RID: 13841
	public Quaternion leftHandRotation = Quaternion.Euler(89f, 6f, 11f);

	// Token: 0x04003612 RID: 13842
	public Vector3 rightHandOffset = new Vector3(-0.01f, -0.16f, 0f);

	// Token: 0x04003613 RID: 13843
	public Quaternion rightHandRotation = Quaternion.Euler(89f, 6f, 11f);

	// Token: 0x04003618 RID: 13848
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressStart = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x04003619 RID: 13849
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressEnd = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x0400361A RID: 13850
	private static ControllerInputPoller._InputCallbacksCadenceInfo _g_callbacks_onPressUpdate = new ControllerInputPoller._InputCallbacksCadenceInfo(32);

	// Token: 0x02000678 RID: 1656
	private enum _EPressCadence
	{
		// Token: 0x0400361C RID: 13852
		Start,
		// Token: 0x0400361D RID: 13853
		End,
		// Token: 0x0400361E RID: 13854
		Held
	}

	// Token: 0x02000679 RID: 1657
	private struct _InputCallback
	{
		// Token: 0x06002985 RID: 10629 RVA: 0x000E0D6B File Offset: 0x000DEF6B
		public _InputCallback(EControllerInputPressFlags flags, Action<EHandednessFlags> callback)
		{
			this.flags = flags;
			this.callback = callback;
		}

		// Token: 0x0400361F RID: 13855
		public readonly EControllerInputPressFlags flags;

		// Token: 0x04003620 RID: 13856
		public readonly Action<EHandednessFlags> callback;
	}

	// Token: 0x0200067A RID: 1658
	private struct _InputCallbacksCadenceInfo
	{
		// Token: 0x06002986 RID: 10630 RVA: 0x000E0D7B File Offset: 0x000DEF7B
		public _InputCallbacksCadenceInfo(int initialCapacity)
		{
			this.list = new List<ControllerInputPoller._InputCallback>(initialCapacity);
		}

		// Token: 0x04003621 RID: 13857
		public readonly List<ControllerInputPoller._InputCallback> list;
	}
}
