using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009F0 RID: 2544
public class SnapXformToLine : MonoBehaviour
{
	// Token: 0x17000629 RID: 1577
	// (get) Token: 0x0600414D RID: 16717 RVA: 0x0015BBEB File Offset: 0x00159DEB
	public Vector3 linePoint
	{
		get
		{
			return this._closest;
		}
	}

	// Token: 0x1700062A RID: 1578
	// (get) Token: 0x0600414E RID: 16718 RVA: 0x0015BBF3 File Offset: 0x00159DF3
	public float linearDistance
	{
		get
		{
			return this._linear;
		}
	}

	// Token: 0x0600414F RID: 16719 RVA: 0x0015BBFB File Offset: 0x00159DFB
	public void SnapTarget(bool applyToXform = true)
	{
		this.Snap(this.target, true);
	}

	// Token: 0x06004150 RID: 16720 RVA: 0x0015BC0A File Offset: 0x00159E0A
	public void SnapTarget(Vector3 point)
	{
		if (this.target)
		{
			this.target.position = this.GetSnappedPoint(this.target.position);
		}
	}

	// Token: 0x06004151 RID: 16721 RVA: 0x0015BC38 File Offset: 0x00159E38
	public void SnapTargetLinear(float t)
	{
		if (this.target && this.from && this.to)
		{
			this.target.position = Vector3.Lerp(this.from.position, this.to.position, t);
		}
	}

	// Token: 0x06004152 RID: 16722 RVA: 0x0015BC93 File Offset: 0x00159E93
	public Vector3 GetSnappedPoint(Transform t)
	{
		return this.GetSnappedPoint(t.position);
	}

	// Token: 0x06004153 RID: 16723 RVA: 0x0015BCA4 File Offset: 0x00159EA4
	public Vector3 GetSnappedPoint(Vector3 point)
	{
		if (!this.apply)
		{
			return point;
		}
		if (!this.from || !this.to)
		{
			return point;
		}
		return SnapXformToLine.GetClosestPointOnLine(point, this.from.position, this.to.position);
	}

	// Token: 0x06004154 RID: 16724 RVA: 0x0015BCF4 File Offset: 0x00159EF4
	public void Snap(Transform xform, bool applyToXform = true)
	{
		if (!this.apply || !xform || !this.from || !this.to)
		{
			return;
		}
		Vector3 position = xform.position;
		Vector3 position2 = this.from.position;
		Vector3 position3 = this.to.position;
		Vector3 closestPointOnLine = SnapXformToLine.GetClosestPointOnLine(position, position2, position3);
		float num = Vector3.Distance(position2, position3);
		float num2 = Vector3.Distance(closestPointOnLine, position2);
		Vector3 closest = this._closest;
		Vector3 vector = closestPointOnLine;
		float linear = this._linear;
		float num3 = (Mathf.Approximately(num, 0f) ? 0f : (num2 / (num + Mathf.Epsilon)));
		this._closest = vector;
		this._linear = num3;
		if (this.output)
		{
			IRangedVariable<float> asT = this.output.AsT;
			asT.Set(asT.Min + this._linear * asT.Range);
		}
		if (applyToXform)
		{
			xform.position = this._closest;
			if (!Mathf.Approximately(closest.x, vector.x) || !Mathf.Approximately(closest.y, vector.y) || !Mathf.Approximately(closest.z, vector.z))
			{
				UnityEvent<Vector3> unityEvent = this.onPositionChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._closest);
				}
			}
			if (!Mathf.Approximately(linear, num3))
			{
				UnityEvent<float> unityEvent2 = this.onLinearDistanceChanged;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this._linear);
				}
			}
			if (this.snapOrientation)
			{
				xform.forward = (position3 - position2).normalized;
				xform.up = Vector3.Lerp(this.from.up.normalized, this.to.up.normalized, this._linear);
			}
		}
	}

	// Token: 0x06004155 RID: 16725 RVA: 0x0015BEBA File Offset: 0x0015A0BA
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.SnapTargetLinear(0f);
		}
	}

	// Token: 0x06004156 RID: 16726 RVA: 0x0015BECF File Offset: 0x0015A0CF
	private void LateUpdate()
	{
		this.SnapTarget(true);
	}

	// Token: 0x06004157 RID: 16727 RVA: 0x0015BED8 File Offset: 0x0015A0D8
	private static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 vector = p - a;
		Vector3 vector2 = b - a;
		float sqrMagnitude = vector2.sqrMagnitude;
		float num = Mathf.Clamp(Vector3.Dot(vector, vector2) / sqrMagnitude, 0f, 1f);
		return a + vector2 * num;
	}

	// Token: 0x040051ED RID: 20973
	public bool apply = true;

	// Token: 0x040051EE RID: 20974
	public bool snapOrientation = true;

	// Token: 0x040051EF RID: 20975
	public bool resetOnDisable = true;

	// Token: 0x040051F0 RID: 20976
	[Space]
	public Transform target;

	// Token: 0x040051F1 RID: 20977
	[Space]
	public Transform from;

	// Token: 0x040051F2 RID: 20978
	public Transform to;

	// Token: 0x040051F3 RID: 20979
	private Vector3 _closest;

	// Token: 0x040051F4 RID: 20980
	private float _linear;

	// Token: 0x040051F5 RID: 20981
	public Ref<IRangedVariable<float>> output;

	// Token: 0x040051F6 RID: 20982
	public UnityEvent<float> onLinearDistanceChanged;

	// Token: 0x040051F7 RID: 20983
	public UnityEvent<Vector3> onPositionChanged;
}
