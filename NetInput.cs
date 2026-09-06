using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200043E RID: 1086
public static class NetInput
{
	// Token: 0x17000293 RID: 659
	// (get) Token: 0x060019EA RID: 6634 RVA: 0x000906D4 File Offset: 0x0008E8D4
	public static VRRig LocalPlayerVRRig
	{
		get
		{
			if (NetInput._localPlayerVRRig == null)
			{
				NetInput._localPlayerVRRig = GameObject.Find("Local VRRig").GetComponentInChildren<VRRig>();
			}
			return NetInput._localPlayerVRRig;
		}
	}

	// Token: 0x060019EB RID: 6635 RVA: 0x000906FC File Offset: 0x0008E8FC
	public static NetworkedInput GetInput()
	{
		NetworkedInput networkedInput = default(NetworkedInput);
		if (NetInput.LocalPlayerVRRig == null)
		{
			return networkedInput;
		}
		networkedInput.headRot_LS = NetInput.LocalPlayerVRRig.head.rigTarget.localRotation;
		networkedInput.rightHandPos_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localPosition;
		networkedInput.rightHandRot_LS = NetInput.LocalPlayerVRRig.rightHand.rigTarget.localRotation;
		networkedInput.leftHandPos_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localPosition;
		networkedInput.leftHandRot_LS = NetInput.LocalPlayerVRRig.leftHand.rigTarget.localRotation;
		networkedInput.handPoseData = NetInput.LocalPlayerVRRig.ReturnHandPosition();
		networkedInput.rootPosition = NetInput.LocalPlayerVRRig.transform.position;
		networkedInput.rootRotation = NetInput.LocalPlayerVRRig.transform.rotation;
		networkedInput.leftThumbTouch = ControllerInputPoller.PrimaryButtonTouch(XRNode.LeftHand) || ControllerInputPoller.SecondaryButtonTouch(XRNode.LeftHand);
		networkedInput.leftThumbPress = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
		networkedInput.leftIndexValue = ControllerInputPoller.TriggerFloat(XRNode.LeftHand);
		networkedInput.leftMiddleValue = ControllerInputPoller.GripFloat(XRNode.LeftHand);
		networkedInput.rightThumbTouch = ControllerInputPoller.PrimaryButtonTouch(XRNode.RightHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		networkedInput.rightThumbPress = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand) || ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
		networkedInput.rightIndexValue = ControllerInputPoller.TriggerFloat(XRNode.RightHand);
		networkedInput.rightMiddleValue = ControllerInputPoller.GripFloat(XRNode.RightHand);
		networkedInput.scale = NetInput.LocalPlayerVRRig.scaleFactor;
		return networkedInput;
	}

	// Token: 0x040024A9 RID: 9385
	private static VRRig _localPlayerVRRig;
}
