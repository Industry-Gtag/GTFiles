using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054B RID: 1355
[Serializable]
public class SubSplineGrabPoint : SubLineGrabPoint
{
	// Token: 0x06002231 RID: 8753 RVA: 0x000B7291 File Offset: 0x000B5491
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		return CatmullRomSpline.Evaluate(this.controlPointsTransformsRelativeToGrabOrigin, advancedItemState.distAlongLine);
	}

	// Token: 0x06002232 RID: 8754 RVA: 0x000B72A4 File Offset: 0x000B54A4
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		this.controlPointsRelativeToGrabOrigin = new List<Vector3>();
		foreach (Transform transform in this.spline.controlPointTransforms)
		{
			this.controlPointsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.InverseTransformPoint(transform.position));
			this.controlPointsTransformsRelativeToGrabOrigin.Add(advancedGrabPointOrigin.worldToLocalMatrix * transform.localToWorldMatrix);
		}
	}

	// Token: 0x06002233 RID: 8755 RVA: 0x000B7318 File Offset: 0x000B5518
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector2;
		return new AdvancedItemState.PreData
		{
			distAlongLine = CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out vector2),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x06002234 RID: 8756 RVA: 0x000B7354 File Offset: 0x000B5554
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		Vector3 vector = objectTransform.InverseTransformPoint(handTransform.position);
		Vector3 vector2;
		CatmullRomSpline.GetClosestEvaluationOnSpline(this.controlPointsRelativeToGrabOrigin, vector, out vector2);
		return Vector3.SqrMagnitude(vector2 - vector);
	}

	// Token: 0x04002D35 RID: 11573
	public CatmullRomSpline spline;

	// Token: 0x04002D36 RID: 11574
	public List<Vector3> controlPointsRelativeToGrabOrigin = new List<Vector3>();

	// Token: 0x04002D37 RID: 11575
	public List<Matrix4x4> controlPointsTransformsRelativeToGrabOrigin = new List<Matrix4x4>();
}
