using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F9 RID: 1273
[Serializable]
public class VRMapIndex : VRMap
{
	// Token: 0x06001FE1 RID: 8161 RVA: 0x000ABE5C File Offset: 0x000AA05C
	public override void Initialize()
	{
		this.closedAngle1Quat = Quaternion.Euler(this.closedAngle1);
		this.closedAngle2Quat = Quaternion.Euler(this.closedAngle2);
		this.closedAngle3Quat = Quaternion.Euler(this.closedAngle3);
		this.startingAngle1Quat = Quaternion.Euler(this.startingAngle1);
		this.startingAngle2Quat = Quaternion.Euler(this.startingAngle2);
		this.startingAngle3Quat = Quaternion.Euler(this.startingAngle3);
	}

	// Token: 0x06001FE2 RID: 8162 RVA: 0x000ABED0 File Offset: 0x000AA0D0
	public override void MapMyFinger(float lerpValue)
	{
		this.calcT = 0f;
		this.triggerValue = ControllerInputPoller.TriggerFloat(this.vrTargetNode);
		this.triggerTouch = ControllerInputPoller.TriggerTouch(this.vrTargetNode);
		this.calcT = 0.1f * this.triggerTouch;
		this.calcT += 0.9f * this.triggerValue;
		this.LerpFinger(lerpValue, false);
	}

	// Token: 0x06001FE3 RID: 8163 RVA: 0x000ABF40 File Offset: 0x000AA140
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

	// Token: 0x04002A6B RID: 10859
	public InputFeatureUsage inputAxis;

	// Token: 0x04002A6C RID: 10860
	public float triggerTouch;

	// Token: 0x04002A6D RID: 10861
	public float triggerValue;

	// Token: 0x04002A6E RID: 10862
	public Transform fingerBone1;

	// Token: 0x04002A6F RID: 10863
	public Transform fingerBone2;

	// Token: 0x04002A70 RID: 10864
	public Transform fingerBone3;

	// Token: 0x04002A71 RID: 10865
	public Vector3 closedAngle1;

	// Token: 0x04002A72 RID: 10866
	public Vector3 closedAngle2;

	// Token: 0x04002A73 RID: 10867
	public Vector3 closedAngle3;

	// Token: 0x04002A74 RID: 10868
	public Vector3 startingAngle1;

	// Token: 0x04002A75 RID: 10869
	public Vector3 startingAngle2;

	// Token: 0x04002A76 RID: 10870
	public Vector3 startingAngle3;

	// Token: 0x04002A77 RID: 10871
	public Quaternion closedAngle1Quat;

	// Token: 0x04002A78 RID: 10872
	public Quaternion closedAngle2Quat;

	// Token: 0x04002A79 RID: 10873
	public Quaternion closedAngle3Quat;

	// Token: 0x04002A7A RID: 10874
	public Quaternion startingAngle1Quat;

	// Token: 0x04002A7B RID: 10875
	public Quaternion startingAngle2Quat;

	// Token: 0x04002A7C RID: 10876
	public Quaternion startingAngle3Quat;

	// Token: 0x04002A7D RID: 10877
	private int lastAngle1;

	// Token: 0x04002A7E RID: 10878
	private int lastAngle2;

	// Token: 0x04002A7F RID: 10879
	private int lastAngle3;

	// Token: 0x04002A80 RID: 10880
	private InputDevice myInputDevice;

	// Token: 0x04002A81 RID: 10881
	public Quaternion[] angle1Table;

	// Token: 0x04002A82 RID: 10882
	public Quaternion[] angle2Table;

	// Token: 0x04002A83 RID: 10883
	public Quaternion[] angle3Table;

	// Token: 0x04002A84 RID: 10884
	private float currentAngle1;

	// Token: 0x04002A85 RID: 10885
	private float currentAngle2;

	// Token: 0x04002A86 RID: 10886
	private float currentAngle3;

	// Token: 0x04002A87 RID: 10887
	private int myTempInt;
}
