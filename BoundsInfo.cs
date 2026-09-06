using System;
using MathGeoLib;
using UnityEngine;

// Token: 0x02000E8A RID: 3722
[Serializable]
public struct BoundsInfo
{
	// Token: 0x1700089D RID: 2205
	// (get) Token: 0x06005A63 RID: 23139 RVA: 0x001D587B File Offset: 0x001D3A7B
	public Vector3 sizeComputed
	{
		get
		{
			return Vector3.Scale(this.size, this.scale) * this.inflate;
		}
	}

	// Token: 0x1700089E RID: 2206
	// (get) Token: 0x06005A64 RID: 23140 RVA: 0x001D5899 File Offset: 0x001D3A99
	public Vector3 sizeComputedAA
	{
		get
		{
			return Vector3.Scale(this.sizeAA, this.scaleAA) * this.inflateAA;
		}
	}

	// Token: 0x06005A65 RID: 23141 RVA: 0x001D58B8 File Offset: 0x001D3AB8
	public static BoundsInfo ComputeBounds(Vector3[] vertices)
	{
		if (vertices.Length == 0)
		{
			return default(BoundsInfo);
		}
		OrientedBoundingBox orientedBoundingBox = OrientedBoundingBox.BruteEnclosing(vertices);
		Vector4 vector = orientedBoundingBox.Axis1;
		Vector4 vector2 = orientedBoundingBox.Axis2;
		Vector4 vector3 = orientedBoundingBox.Axis3;
		Vector4 vector4 = new Vector4(0f, 0f, 0f, 1f);
		BoundsInfo boundsInfo = default(BoundsInfo);
		boundsInfo.center = orientedBoundingBox.Center;
		boundsInfo.size = orientedBoundingBox.Extent * 2f;
		boundsInfo.rotation = new Matrix4x4(vector, vector2, vector3, vector4).rotation;
		boundsInfo.scale = Vector3.one;
		boundsInfo.inflate = 1f;
		Bounds bounds = GeometryUtility.CalculateBounds(vertices, Matrix4x4.identity);
		boundsInfo.centerAA = bounds.center;
		boundsInfo.sizeAA = bounds.size;
		boundsInfo.scaleAA = Vector3.one;
		boundsInfo.inflateAA = 1f;
		return boundsInfo;
	}

	// Token: 0x06005A66 RID: 23142 RVA: 0x001D59BC File Offset: 0x001D3BBC
	public static BoxCollider CreateBoxCollider(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int hashCode3 = bounds.rotation.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2, hashCode3);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.center;
		transform.rotation = bounds.rotation;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputed;
		return boxCollider;
	}

	// Token: 0x06005A67 RID: 23143 RVA: 0x001D5A68 File Offset: 0x001D3C68
	public static BoxCollider CreateBoxColliderAA(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.centerAA;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputedAA;
		return boxCollider;
	}

	// Token: 0x04006B85 RID: 27525
	public Vector3 center;

	// Token: 0x04006B86 RID: 27526
	public Vector3 size;

	// Token: 0x04006B87 RID: 27527
	public Quaternion rotation;

	// Token: 0x04006B88 RID: 27528
	public Vector3 scale;

	// Token: 0x04006B89 RID: 27529
	public float inflate;

	// Token: 0x04006B8A RID: 27530
	[Space]
	public Vector3 centerAA;

	// Token: 0x04006B8B RID: 27531
	public Vector3 sizeAA;

	// Token: 0x04006B8C RID: 27532
	public Vector3 scaleAA;

	// Token: 0x04006B8D RID: 27533
	public float inflateAA;
}
