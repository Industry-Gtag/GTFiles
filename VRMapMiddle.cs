using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004FA RID: 1274
[Serializable]
public class VRMapMiddle : VRMap
{
	// Token: 0x06001FE5 RID: 8165 RVA: 0x000AC11C File Offset: 0x000AA31C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000AC18F File Offset: 0x000AA38F
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.gripValue = ControllerInputPoller.GripFloat(this.vrTargetNode);
		this.calcT = 1f * this.gripValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000AC1C8 File Offset: 0x000AA3C8
	public override void LerpFinger(float lerpValue, bool isOther)
	{
		if (isOther)
		{
			this.currentAngle1 = Mathf.Lerp(this.currentAngle1, this.calcT, lerpValue);
			this.currentAngle2 = Mathf.Lerp(this.currentAngle2, this.calcT, lerpValue);
			this.currentAngle3 = Mathf.Lerp(this.currentAngle3, this.calcT, lerpValue);
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
			}
			this.myTempInt = (int)(this.currentAngle3 * 10.1f);
			if (this.myTempInt != this.lastAngle3)
			{
				this.lastAngle3 = this.myTempInt;
				this.fingerBone3.localRotation = this.angle3Table[this.lastAngle3];
				return;
			}
		}
		else
		{
			this.fingerBone1.localRotation = Quaternion.Lerp(this.fingerBone1.localRotation, Quaternion.Lerp(this.startingAngle1Quat, this.closedAngle1Quat, this.calcT), lerpValue);
			this.fingerBone2.localRotation = Quaternion.Lerp(this.fingerBone2.localRotation, Quaternion.Lerp(this.startingAngle2Quat, this.closedAngle2Quat, this.calcT), lerpValue);
			this.fingerBone3.localRotation = Quaternion.Lerp(this.fingerBone3.localRotation, Quaternion.Lerp(this.startingAngle3Quat, this.closedAngle3Quat, this.calcT), lerpValue);
		}
	}

	// Token: 0x04002A88 RID: 10888
	public InputFeatureUsage inputAxis;

	// Token: 0x04002A89 RID: 10889
	public float gripValue;

	// Token: 0x04002A8A RID: 10890
	public Transform fingerBone1;

	// Token: 0x04002A8B RID: 10891
	public Transform fingerBone2;

	// Token: 0x04002A8C RID: 10892
	public Transform fingerBone3;

	// Token: 0x04002A8D RID: 10893
	public Vector3 closedAngle1;

	// Token: 0x04002A8E RID: 10894
	public Vector3 closedAngle2;

	// Token: 0x04002A8F RID: 10895
	public Vector3 closedAngle3;

	// Token: 0x04002A90 RID: 10896
	public Vector3 startingAngle1;

	// Token: 0x04002A91 RID: 10897
	public Vector3 startingAngle2;

	// Token: 0x04002A92 RID: 10898
	public Vector3 startingAngle3;

	// Token: 0x04002A93 RID: 10899
	public Quaternion closedAngle1Quat;

	// Token: 0x04002A94 RID: 10900
	public Quaternion closedAngle2Quat;

	// Token: 0x04002A95 RID: 10901
	public Quaternion closedAngle3Quat;

	// Token: 0x04002A96 RID: 10902
	public Quaternion startingAngle1Quat;

	// Token: 0x04002A97 RID: 10903
	public Quaternion startingAngle2Quat;

	// Token: 0x04002A98 RID: 10904
	public Quaternion startingAngle3Quat;

	// Token: 0x04002A99 RID: 10905
	public Quaternion[] angle1Table;

	// Token: 0x04002A9A RID: 10906
	public Quaternion[] angle2Table;

	// Token: 0x04002A9B RID: 10907
	public Quaternion[] angle3Table;

	// Token: 0x04002A9C RID: 10908
	private int lastAngle1;

	// Token: 0x04002A9D RID: 10909
	private int lastAngle2;

	// Token: 0x04002A9E RID: 10910
	private int lastAngle3;

	// Token: 0x04002A9F RID: 10911
	private float currentAngle1;

	// Token: 0x04002AA0 RID: 10912
	private float currentAngle2;

	// Token: 0x04002AA1 RID: 10913
	private float currentAngle3;

	// Token: 0x04002AA2 RID: 10914
	private InputDevice tempDevice;

	// Token: 0x04002AA3 RID: 10915
	private int myTempInt;
}
