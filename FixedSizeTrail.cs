using System;
using UnityEngine;

// Token: 0x0200069A RID: 1690
[RequireComponent(typeof(LineRenderer))]
public class FixedSizeTrail : MonoBehaviour
{
	// Token: 0x1700043B RID: 1083
	// (get) Token: 0x06002A22 RID: 10786 RVA: 0x000E34F6 File Offset: 0x000E16F6
	public LineRenderer renderer
	{
		get
		{
			return this._lineRenderer;
		}
	}

	// Token: 0x1700043C RID: 1084
	// (get) Token: 0x06002A23 RID: 10787 RVA: 0x000E34FE File Offset: 0x000E16FE
	// (set) Token: 0x06002A24 RID: 10788 RVA: 0x000E3506 File Offset: 0x000E1706
	public float length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = Math.Clamp(value, 0f, 128f);
		}
	}

	// Token: 0x1700043D RID: 1085
	// (get) Token: 0x06002A25 RID: 10789 RVA: 0x000E351E File Offset: 0x000E171E
	public Vector3[] points
	{
		get
		{
			return this._points;
		}
	}

	// Token: 0x06002A26 RID: 10790 RVA: 0x000E3526 File Offset: 0x000E1726
	private void Reset()
	{
		this.Setup();
	}

	// Token: 0x06002A27 RID: 10791 RVA: 0x000E3526 File Offset: 0x000E1726
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06002A28 RID: 10792 RVA: 0x000E3526 File Offset: 0x000E1726
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06002A29 RID: 10793 RVA: 0x000E3530 File Offset: 0x000E1730
	public void Setup()
	{
		this._transform = base.transform;
		if (this._lineRenderer == null)
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
		}
		if (!this._lineRenderer)
		{
			return;
		}
		this._lineRenderer.useWorldSpace = true;
		Vector3 position = this._transform.position;
		Vector3 forward = this._transform.forward;
		int num = this._segments + 1;
		this._points = new Vector3[num];
		float num2 = this._length / (float)this._segments;
		for (int i = 0; i < num; i++)
		{
			this._points[i] = position - forward * num2 * (float)i;
		}
		this._lineRenderer.positionCount = num;
		this._lineRenderer.SetPositions(this._points);
		this.Update();
	}

	// Token: 0x06002A2A RID: 10794 RVA: 0x000E360E File Offset: 0x000E180E
	private void Update()
	{
		if (!this.manualUpdate)
		{
			this.Update(Time.deltaTime);
		}
	}

	// Token: 0x06002A2B RID: 10795 RVA: 0x000E3624 File Offset: 0x000E1824
	private void FixedUpdate()
	{
		if (!this.applyPhysics)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = this._points.Length - 1;
		float num2 = this._length / (float)num;
		for (int i = 1; i < num; i++)
		{
			float num3 = (float)(i - 1) / (float)num;
			float num4 = this.gravityCurve.Evaluate(num3);
			Vector3 vector = this.gravity * (num4 * deltaTime);
			this._points[i] += vector;
			this._points[i + 1] += vector;
		}
	}

	// Token: 0x06002A2C RID: 10796 RVA: 0x000E36C8 File Offset: 0x000E18C8
	public void Update(float dt)
	{
		float num = this._length / (float)(this._segments - 1);
		Vector3 position = this._transform.position;
		this._points[0] = position;
		float num2 = Vector3.Distance(this._points[0], this._points[1]);
		float num3 = num - num2;
		if (num2 > num)
		{
			Array.Copy(this._points, 0, this._points, 1, this._points.Length - 1);
		}
		for (int i = 0; i < this._points.Length - 1; i++)
		{
			Vector3 vector = this._points[i];
			Vector3 vector2 = this._points[i + 1] - vector;
			if (vector2.sqrMagnitude > num * num)
			{
				this._points[i + 1] = vector + vector2.normalized * num;
			}
		}
		if (num3 > 0f)
		{
			int num4 = this._points.Length - 1;
			int num5 = num4 - 1;
			Vector3 vector3 = this._points[num4] - this._points[num5];
			Vector3 vector4 = vector3.normalized;
			if (this.applyPhysics)
			{
				Vector3 normalized = (this._points[num5] - this._points[num5 - 1]).normalized;
				vector4 = Vector3.Lerp(vector4, normalized, 0.5f);
			}
			this._points[num4] = this._points[num5] + vector4 * Math.Min(vector3.magnitude, num3);
		}
		this._lineRenderer.SetPositions(this._points);
	}

	// Token: 0x06002A2D RID: 10797 RVA: 0x000E3880 File Offset: 0x000E1A80
	private static float CalcLength(in Vector3[] positions)
	{
		float num = 0f;
		for (int i = 0; i < positions.Length - 1; i++)
		{
			num += Vector3.Distance(positions[i], positions[i + 1]);
		}
		return num;
	}

	// Token: 0x040036E3 RID: 14051
	[SerializeField]
	private Transform _transform;

	// Token: 0x040036E4 RID: 14052
	[SerializeField]
	private LineRenderer _lineRenderer;

	// Token: 0x040036E5 RID: 14053
	[SerializeField]
	[Range(1f, 128f)]
	private int _segments = 8;

	// Token: 0x040036E6 RID: 14054
	[SerializeField]
	private float _length = 8f;

	// Token: 0x040036E7 RID: 14055
	public bool manualUpdate;

	// Token: 0x040036E8 RID: 14056
	[Space]
	public bool applyPhysics;

	// Token: 0x040036E9 RID: 14057
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x040036EA RID: 14058
	public AnimationCurve gravityCurve = AnimationCurves.EaseInCubic;

	// Token: 0x040036EB RID: 14059
	[Space]
	private Vector3[] _points = new Vector3[8];
}
