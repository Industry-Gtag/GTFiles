using System;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x020005F9 RID: 1529
public class AutomaticAdjustIPD : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002609 RID: 9737 RVA: 0x00019260 File Offset: 0x00017460
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600260A RID: 9738 RVA: 0x00019269 File Offset: 0x00017469
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600260B RID: 9739 RVA: 0x000C94B4 File Offset: 0x000C76B4
	public void SliceUpdate()
	{
		if (!this.headset.isValid)
		{
			this.headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);
		}
		if (this.headset.isValid && this.headset.TryGetFeatureValue(CommonUsages.leftEyePosition, out this.leftEyePosition) && this.headset.TryGetFeatureValue(CommonUsages.rightEyePosition, out this.rightEyePosition))
		{
			this.currentIPD = (this.rightEyePosition - this.leftEyePosition).magnitude;
			if (Mathf.Abs(this.lastIPD - this.currentIPD) < 0.01f)
			{
				return;
			}
			this.lastIPD = this.currentIPD;
			for (int i = 0; i < this.adjustXScaleObjects.Length; i++)
			{
				Transform transform = this.adjustXScaleObjects[i];
				if (!transform)
				{
					return;
				}
				transform.localScale = new Vector3(Mathf.LerpUnclamped(1f, 1.12f, (this.currentIPD - 0.058f) / 0.0050000027f), 1f, 1f);
			}
		}
	}

	// Token: 0x04003195 RID: 12693
	public InputDevice headset;

	// Token: 0x04003196 RID: 12694
	public float currentIPD;

	// Token: 0x04003197 RID: 12695
	public Vector3 leftEyePosition;

	// Token: 0x04003198 RID: 12696
	public Vector3 rightEyePosition;

	// Token: 0x04003199 RID: 12697
	public bool testOverride;

	// Token: 0x0400319A RID: 12698
	public Transform[] adjustXScaleObjects;

	// Token: 0x0400319B RID: 12699
	public float sizeAt58mm = 1f;

	// Token: 0x0400319C RID: 12700
	public float sizeAt63mm = 1.12f;

	// Token: 0x0400319D RID: 12701
	public float lastIPD;
}
