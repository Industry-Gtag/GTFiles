using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x0200054C RID: 1356
[Serializable]
public class SlotTransformOverride
{
	// Token: 0x170003A0 RID: 928
	// (get) Token: 0x06002236 RID: 8758 RVA: 0x000B73A7 File Offset: 0x000B55A7
	// (set) Token: 0x06002237 RID: 8759 RVA: 0x000B73B4 File Offset: 0x000B55B4
	private XformOffset _EdXformOffsetRepresenationOf_overrideTransformMatrix
	{
		get
		{
			return new XformOffset(this.overrideTransformMatrix);
		}
		set
		{
			this.overrideTransformMatrix = Matrix4x4.TRS(value.pos, value.rot, value.scale);
		}
	}

	// Token: 0x06002238 RID: 8760 RVA: 0x000B73D4 File Offset: 0x000B55D4
	public void Initialize(Component component, Transform anchor)
	{
		if (!this.useAdvancedGrab)
		{
			return;
		}
		this.AdvOriginLocalToParentAnchorLocal = anchor.worldToLocalMatrix * this.advancedGrabPointOrigin.localToWorldMatrix;
		this.AdvAnchorLocalToAdvOriginLocal = this.advancedGrabPointOrigin.worldToLocalMatrix * this.advancedGrabPointAnchor.localToWorldMatrix;
		foreach (SubGrabPoint subGrabPoint in this.multiPoints)
		{
			if (subGrabPoint == null)
			{
				break;
			}
			subGrabPoint.InitializePoints(anchor, this.advancedGrabPointAnchor, this.advancedGrabPointOrigin);
		}
	}

	// Token: 0x06002239 RID: 8761 RVA: 0x000B7480 File Offset: 0x000B5680
	public void AddLineButton()
	{
		this.multiPoints.Add(new SubLineGrabPoint());
	}

	// Token: 0x0600223A RID: 8762 RVA: 0x000B7494 File Offset: 0x000B5694
	public void AddSubGrabPoint(TransferrableObjectGripPosition togp)
	{
		SubGrabPoint subGrabPoint = togp.CreateSubGrabPoint(this);
		this.multiPoints.Add(subGrabPoint);
	}

	// Token: 0x04002D38 RID: 11576
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	public Transform overrideTransform;

	// Token: 0x04002D39 RID: 11577
	[Obsolete("(2024-08-20 MattO) Cosmetics use xformOffsets now which fills in the appropriate data for this component. If you are doing something weird then `overrideTransformMatrix` must be used instead. This will probably be removed after 2024-09-15.")]
	[Delayed]
	public string overrideTransform_path;

	// Token: 0x04002D3A RID: 11578
	public TransferrableObject.PositionState positionState;

	// Token: 0x04002D3B RID: 11579
	public bool useAdvancedGrab;

	// Token: 0x04002D3C RID: 11580
	public Matrix4x4 overrideTransformMatrix = Matrix4x4.identity;

	// Token: 0x04002D3D RID: 11581
	public Transform advancedGrabPointAnchor;

	// Token: 0x04002D3E RID: 11582
	public Transform advancedGrabPointOrigin;

	// Token: 0x04002D3F RID: 11583
	[SerializeReference]
	public List<SubGrabPoint> multiPoints = new List<SubGrabPoint>();

	// Token: 0x04002D40 RID: 11584
	public Matrix4x4 AdvOriginLocalToParentAnchorLocal;

	// Token: 0x04002D41 RID: 11585
	public Matrix4x4 AdvAnchorLocalToAdvOriginLocal;
}
