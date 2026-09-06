using System;
using UnityEngine;

// Token: 0x02000DDC RID: 3548
public class BezierCurve : MonoBehaviour
{
	// Token: 0x060056EB RID: 22251 RVA: 0x001C5EF0 File Offset: 0x001C40F0
	public Vector3 GetPoint(float t)
	{
		Vector3 vector = ((this.points.Length == 3) ? Bezier.GetPoint(this.points[0], this.points[1], this.points[2], t) : Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t));
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector);
	}

	// Token: 0x060056EC RID: 22252 RVA: 0x001C5F88 File Offset: 0x001C4188
	public Vector3 GetVelocity(float t)
	{
		Vector3 vector = ((this.points.Length == 3) ? Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], t) : Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t));
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector) - this.referenceTransform.position;
	}

	// Token: 0x060056ED RID: 22253 RVA: 0x001C6030 File Offset: 0x001C4230
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x060056EE RID: 22254 RVA: 0x001C604C File Offset: 0x001C424C
	public void Reset()
	{
		this.referenceTransform = base.transform;
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x040067C5 RID: 26565
	public Transform referenceTransform;

	// Token: 0x040067C6 RID: 26566
	public Vector3[] points;
}
