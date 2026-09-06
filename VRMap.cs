using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020004F8 RID: 1272
[Serializable]
public class VRMap
{
	// Token: 0x17000366 RID: 870
	// (get) Token: 0x06001FD7 RID: 8151 RVA: 0x000ABBB1 File Offset: 0x000A9DB1
	// (set) Token: 0x06001FD8 RID: 8152 RVA: 0x000ABBBE File Offset: 0x000A9DBE
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x06001FD9 RID: 8153 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void Initialize()
	{
	}

	// Token: 0x06001FDA RID: 8154 RVA: 0x000ABBCC File Offset: 0x000A9DCC
	public void MapOther(float lerpValue)
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetLocalPositionAndRotation(out vector, out quaternion);
		this.rigTarget.SetLocalPositionAndRotation(Vector3.Lerp(vector, this.syncPos, lerpValue), Quaternion.Lerp(quaternion, this.syncRotation, lerpValue));
	}

	// Token: 0x06001FDB RID: 8155 RVA: 0x000ABC10 File Offset: 0x000A9E10
	public void MapMine(float ratio, Transform playerOffsetTransform)
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetPositionAndRotation(out vector, out quaternion);
		if (this.overrideTarget != null)
		{
			Vector3 vector2;
			Quaternion quaternion2;
			this.overrideTarget.GetPositionAndRotation(out vector2, out quaternion2);
			this.rigTarget.SetPositionAndRotation(vector2 + quaternion * this.trackingPositionOffset * ratio, quaternion2 * Quaternion.Euler(this.trackingRotationOffset));
		}
		else
		{
			if (!this.hasInputDevice && ConnectedControllerHandler.Instance.GetValidForXRNode(this.vrTargetNode))
			{
				this.myInputDevice = InputDevices.GetDeviceAtXRNode(this.vrTargetNode);
				this.hasInputDevice = true;
				if (this.vrTargetNode != XRNode.LeftHand && this.vrTargetNode != XRNode.RightHand)
				{
					this.hasInputDevice = this.myInputDevice.isValid;
				}
			}
			Quaternion quaternion3;
			Vector3 vector3;
			if (this.hasInputDevice && this.myInputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out quaternion3) && this.myInputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out vector3))
			{
				Quaternion quaternion4 = Quaternion.identity;
				Transform parent = playerOffsetTransform.parent;
				if (parent.IsNotNull())
				{
					quaternion4 = parent.rotation;
				}
				this.rigTarget.SetPositionAndRotation(vector3 + quaternion * this.trackingPositionOffset * ratio + playerOffsetTransform.position, quaternion4 * quaternion3 * Quaternion.Euler(this.trackingRotationOffset));
				this.rigTarget.RotateAround(playerOffsetTransform.position, playerOffsetTransform.up, playerOffsetTransform.localEulerAngles.y);
			}
		}
		if (this.handholdOverrideTarget != null)
		{
			this.rigTarget.position = Vector3.MoveTowards(vector, this.handholdOverrideTarget.position - this.handholdOverrideTargetOffset + quaternion * this.trackingPositionOffset * ratio, Time.deltaTime * 2f);
		}
	}

	// Token: 0x06001FDC RID: 8156 RVA: 0x000ABDF4 File Offset: 0x000A9FF4
	public Vector3 GetExtrapolatedControllerPosition()
	{
		Vector3 vector;
		Quaternion quaternion;
		this.rigTarget.GetPositionAndRotation(out vector, out quaternion);
		return vector - quaternion * this.trackingPositionOffset * this.rigTarget.lossyScale.x;
	}

	// Token: 0x06001FDD RID: 8157 RVA: 0x000ABE37 File Offset: 0x000AA037
	public virtual void MapOtherFinger(float handSync, float lerpValue)
	{
		this.calcT = handSync;
		this.LerpFinger(lerpValue, true);
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void MapMyFinger(float lerpValue)
	{
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void LerpFinger(float lerpValue, bool isOther)
	{
	}

	// Token: 0x04002A5F RID: 10847
	public XRNode vrTargetNode;

	// Token: 0x04002A60 RID: 10848
	public Transform overrideTarget;

	// Token: 0x04002A61 RID: 10849
	public Transform rigTarget;

	// Token: 0x04002A62 RID: 10850
	public Vector3 trackingPositionOffset;

	// Token: 0x04002A63 RID: 10851
	public Vector3 trackingRotationOffset;

	// Token: 0x04002A64 RID: 10852
	internal NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002A65 RID: 10853
	public Quaternion syncRotation;

	// Token: 0x04002A66 RID: 10854
	public float calcT;

	// Token: 0x04002A67 RID: 10855
	private InputDevice myInputDevice;

	// Token: 0x04002A68 RID: 10856
	private bool hasInputDevice;

	// Token: 0x04002A69 RID: 10857
	public Transform handholdOverrideTarget;

	// Token: 0x04002A6A RID: 10858
	public Vector3 handholdOverrideTargetOffset;
}
