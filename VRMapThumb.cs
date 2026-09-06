using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004FB RID: 1275
[Serializable]
public class VRMapThumb : VRMap
{
	// Token: 0x06001FE9 RID: 8169 RVA: 0x000AC39C File Offset: 0x000AA59C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000AC3F0 File Offset: 0x000AA5F0
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		if (this.vrTargetNode == XRNode.LeftHand)
		{
			this.primaryButtonPress = ControllerInputPoller.instance.leftControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.leftControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.leftControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.leftControllerSecondaryButtonTouch;
		}
		else
		{
			this.primaryButtonPress = ControllerInputPoller.instance.rightControllerPrimaryButton;
			this.primaryButtonTouch = ControllerInputPoller.instance.rightControllerPrimaryButtonTouch;
			this.secondaryButtonPress = ControllerInputPoller.instance.rightControllerSecondaryButton;
			this.secondaryButtonTouch = ControllerInputPoller.instance.rightControllerSecondaryButtonTouch;
		}
		if (this.primaryButtonPress || this.secondaryButtonPress)
		{
			this.calcT = 1f;
		}
		else if (this.primaryButtonTouch || this.secondaryButtonTouch)
		{
			this.calcT = 0.1f;
		}
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000AC4E4 File Offset: 0x000AA6E4
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.myTempInt = (int)(this.currentAngle1 * 10.1f);
			if (this.myTempInt != this.lastAngle1)
			{
				this.lastAngle1 = this.myTempInt;
				this.fingerBone1.localRotation = this.angle1Table[this.lastAngle1];
			}
			this.myTempInt = (int)(this.currentAngle2 * 10.1f);
			if (this.myTempInt != this.lastAngle2)
			{
				this.lastAngle2 = this.myTempInt;
				this.fingerBone2.localRotation = this.angle2Table[this.lastAngle2];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x04002AA4 RID: 10916
	public InputFeatureUsage inputAxis;

	// Token: 0x04002AA5 RID: 10917
	public bool primaryButtonTouch;

	// Token: 0x04002AA6 RID: 10918
	public bool primaryButtonPress;

	// Token: 0x04002AA7 RID: 10919
	public bool secondaryButtonTouch;

	// Token: 0x04002AA8 RID: 10920
	public bool secondaryButtonPress;

	// Token: 0x04002AA9 RID: 10921
	public Transform fingerBone1;

	// Token: 0x04002AAA RID: 10922
	public Transform fingerBone2;

	// Token: 0x04002AAB RID: 10923
	public Vector3 closedAngle1;

	// Token: 0x04002AAC RID: 10924
	public Vector3 closedAngle2;

	// Token: 0x04002AAD RID: 10925
	public Vector3 startingAngle1;

	// Token: 0x04002AAE RID: 10926
	public Vector3 startingAngle2;

	// Token: 0x04002AAF RID: 10927
	public Quaternion closedAngle1Quat;

	// Token: 0x04002AB0 RID: 10928
	public Quaternion closedAngle2Quat;

	// Token: 0x04002AB1 RID: 10929
	public Quaternion startingAngle1Quat;

	// Token: 0x04002AB2 RID: 10930
	public Quaternion startingAngle2Quat;

	// Token: 0x04002AB3 RID: 10931
	public Quaternion[] angle1Table;

	// Token: 0x04002AB4 RID: 10932
	public Quaternion[] angle2Table;

	// Token: 0x04002AB5 RID: 10933
	private float currentAngle1;

	// Token: 0x04002AB6 RID: 10934
	private float currentAngle2;

	// Token: 0x04002AB7 RID: 10935
	private int lastAngle1;

	// Token: 0x04002AB8 RID: 10936
	private int lastAngle2;

	// Token: 0x04002AB9 RID: 10937
	private InputDevice tempDevice;

	// Token: 0x04002ABA RID: 10938
	private int myTempInt;
}
