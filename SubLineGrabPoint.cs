using System;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;

// Token: 0x0200054A RID: 1354
[Serializable]
public class SubLineGrabPoint : SubGrabPoint
{
	// Token: 0x0600222A RID: 8746 RVA: 0x000B7084 File Offset: 0x000B5284
	public override Matrix4x4 GetTransformation_GripPointLocalToAdvOriginLocal(AdvancedItemState.PreData advancedItemState, SlotTransformOverride slotTransformOverride)
	{
		float distAlongLine = advancedItemState.distAlongLine;
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), distAlongLine);
		Quaternion quaternion = Quaternion.Slerp(this.startPointRelativeTransformToGrabPointOrigin.rotation, this.endPointRelativeTransformToGrabPointOrigin.rotation, distAlongLine);
		return Matrix4x4.TRS(vector, quaternion, Vector3.one);
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x000B70DC File Offset: 0x000B52DC
	public override void InitializePoints(Transform anchor, Transform grabPointAnchor, Transform advancedGrabPointOrigin)
	{
		base.InitializePoints(anchor, grabPointAnchor, advancedGrabPointOrigin);
		if (this.startPoint == null || this.endPoint == null)
		{
			return;
		}
		this.startPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.startPoint.position);
		this.endPointRelativeToGrabPointOrigin = advancedGrabPointOrigin.InverseTransformPoint(this.endPoint.position);
		this.endPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.endPoint.localToWorldMatrix;
		this.startPointRelativeTransformToGrabPointOrigin = advancedGrabPointOrigin.worldToLocalMatrix * this.startPoint.localToWorldMatrix;
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x000B7175 File Offset: 0x000B5375
	public override AdvancedItemState.PreData GetPreData(Transform objectTransform, Transform handTransform, Transform targetDock, SlotTransformOverride slotTransformOverride)
	{
		return new AdvancedItemState.PreData
		{
			distAlongLine = SubLineGrabPoint.<GetPreData>g__FindNearestFractionOnLine|8_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position),
			pointType = AdvancedItemState.PointType.DistanceBased
		};
	}

	// Token: 0x0600222D RID: 8749 RVA: 0x000B71AC File Offset: 0x000B53AC
	public override float EvaluateScore(Transform objectTransform, Transform handTransform, Transform targetDock)
	{
		float num = SubLineGrabPoint.<EvaluateScore>g__FindNearestFractionOnLine|9_0(objectTransform.TransformPoint(this.startPointRelativeToGrabPointOrigin), objectTransform.TransformPoint(this.endPointRelativeToGrabPointOrigin), handTransform.position);
		Vector3 vector = Vector3.Lerp(this.startPointRelativeTransformToGrabPointOrigin.Position(), this.endPointRelativeTransformToGrabPointOrigin.Position(), num);
		Vector3 vector2 = objectTransform.InverseTransformPoint(handTransform.position);
		return Vector3.SqrMagnitude(vector - vector2);
	}

	// Token: 0x0600222F RID: 8751 RVA: 0x000B721C File Offset: 0x000B541C
	[CompilerGenerated]
	internal static float <GetPreData>g__FindNearestFractionOnLine|8_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x06002230 RID: 8752 RVA: 0x000B7258 File Offset: 0x000B5458
	[CompilerGenerated]
	internal static float <EvaluateScore>g__FindNearestFractionOnLine|9_0(Vector3 origin, Vector3 end, Vector3 point)
	{
		Vector3 vector = end - origin;
		float magnitude = vector.magnitude;
		vector /= magnitude;
		return Mathf.Clamp01(Vector3.Dot(point - origin, vector) / magnitude);
	}

	// Token: 0x04002D2F RID: 11567
	public Transform startPoint;

	// Token: 0x04002D30 RID: 11568
	public Transform endPoint;

	// Token: 0x04002D31 RID: 11569
	public Vector3 startPointRelativeToGrabPointOrigin;

	// Token: 0x04002D32 RID: 11570
	public Vector3 endPointRelativeToGrabPointOrigin;

	// Token: 0x04002D33 RID: 11571
	public Matrix4x4 startPointRelativeTransformToGrabPointOrigin;

	// Token: 0x04002D34 RID: 11572
	public Matrix4x4 endPointRelativeTransformToGrabPointOrigin;
}
