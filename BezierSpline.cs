using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000DDD RID: 3549
public class BezierSpline : MonoBehaviour
{
	// Token: 0x060056F0 RID: 22256 RVA: 0x001C60E0 File Offset: 0x001C42E0
	private void Awake()
	{
		float num = 0f;
		for (int i = 1; i < this.points.Length; i++)
		{
			num += (this.points[i] - this.points[i - 1]).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / 0.1f);
		this.buildTimesLenghtsTables(num2);
	}

	// Token: 0x060056F1 RID: 22257 RVA: 0x001C6144 File Offset: 0x001C4344
	private void buildTimesLenghtsTables(int subdivisions)
	{
		this._totalArcLength = 0f;
		float num = 1f / (float)subdivisions;
		this._timesTable = new float[subdivisions];
		this._lengthsTable = new float[subdivisions];
		Vector3 vector = this.GetPoint(0f);
		for (int i = 1; i <= subdivisions; i++)
		{
			float num2 = num * (float)i;
			Vector3 point = this.GetPoint(num2);
			this._totalArcLength += Vector3.Distance(point, vector);
			vector = point;
			this._timesTable[i - 1] = num2;
			this._lengthsTable[i - 1] = this._totalArcLength;
		}
	}

	// Token: 0x060056F2 RID: 22258 RVA: 0x001C61DC File Offset: 0x001C43DC
	private float getPathFromTime(float t)
	{
		if (float.IsNaN(this._totalArcLength) || this._totalArcLength == 0f)
		{
			return t;
		}
		if (t > 0f && t < 1f)
		{
			float num = this._totalArcLength * t;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = this._lengthsTable.Length;
			int i = 0;
			while (i < num6)
			{
				if (this._lengthsTable[i] > num)
				{
					num4 = this._timesTable[i];
					num5 = this._lengthsTable[i];
					if (i > 0)
					{
						num3 = this._lengthsTable[i - 1];
						break;
					}
					break;
				}
				else
				{
					num2 = this._timesTable[i];
					i++;
				}
			}
			t = num2 + (num - num3) / (num5 - num3) * (num4 - num2);
		}
		if (t > 1f)
		{
			t = 1f;
		}
		else if (t < 0f)
		{
			t = 0f;
		}
		return t;
	}

	// Token: 0x060056F3 RID: 22259 RVA: 0x001C62C8 File Offset: 0x001C44C8
	public void BuildSplineFromPoints(Vector3[] newPoints, BezierControlPointMode[] newModes, bool isLoop)
	{
		this.points = newPoints;
		this.modes = newModes;
		this.loop = isLoop;
		float num = 0f;
		for (int i = 1; i < this.points.Length; i++)
		{
			num += (this.points[i] - this.points[i - 1]).magnitude;
		}
		int num2 = Mathf.RoundToInt(num / 0.1f);
		this.buildTimesLenghtsTables(num2);
	}

	// Token: 0x1700084D RID: 2125
	// (get) Token: 0x060056F4 RID: 22260 RVA: 0x001C6341 File Offset: 0x001C4541
	// (set) Token: 0x060056F5 RID: 22261 RVA: 0x001C6349 File Offset: 0x001C4549
	public bool Loop
	{
		get
		{
			return this.loop;
		}
		set
		{
			this.loop = value;
			if (value)
			{
				this.modes[this.modes.Length - 1] = this.modes[0];
				this.SetControlPoint(0, this.points[0]);
			}
		}
	}

	// Token: 0x1700084E RID: 2126
	// (get) Token: 0x060056F6 RID: 22262 RVA: 0x001C6381 File Offset: 0x001C4581
	public int ControlPointCount
	{
		get
		{
			return this.points.Length;
		}
	}

	// Token: 0x060056F7 RID: 22263 RVA: 0x001C638B File Offset: 0x001C458B
	public Vector3 GetControlPoint(int index)
	{
		return this.points[index];
	}

	// Token: 0x060056F8 RID: 22264 RVA: 0x001C639C File Offset: 0x001C459C
	public void SetControlPoint(int index, Vector3 point)
	{
		if (index % 3 == 0)
		{
			Vector3 vector = point - this.points[index];
			if (this.loop)
			{
				if (index == 0)
				{
					this.points[1] += vector;
					this.points[this.points.Length - 2] += vector;
					this.points[this.points.Length - 1] = point;
				}
				else if (index == this.points.Length - 1)
				{
					this.points[0] = point;
					this.points[1] += vector;
					this.points[index - 1] += vector;
				}
				else
				{
					this.points[index - 1] += vector;
					this.points[index + 1] += vector;
				}
			}
			else
			{
				if (index > 0)
				{
					this.points[index - 1] += vector;
				}
				if (index + 1 < this.points.Length)
				{
					this.points[index + 1] += vector;
				}
			}
		}
		this.points[index] = point;
		this.EnforceMode(index);
	}

	// Token: 0x060056F9 RID: 22265 RVA: 0x001C652E File Offset: 0x001C472E
	public BezierControlPointMode GetControlPointMode(int index)
	{
		return this.modes[(index + 1) / 3];
	}

	// Token: 0x060056FA RID: 22266 RVA: 0x001C653C File Offset: 0x001C473C
	public void SetControlPointMode(int index, BezierControlPointMode mode)
	{
		int num = (index + 1) / 3;
		this.modes[num] = mode;
		if (this.loop)
		{
			if (num == 0)
			{
				this.modes[this.modes.Length - 1] = mode;
			}
			else if (num == this.modes.Length - 1)
			{
				this.modes[0] = mode;
			}
		}
		this.EnforceMode(index);
	}

	// Token: 0x060056FB RID: 22267 RVA: 0x001C6594 File Offset: 0x001C4794
	private void EnforceMode(int index)
	{
		int num = (index + 1) / 3;
		BezierControlPointMode bezierControlPointMode = this.modes[num];
		if (bezierControlPointMode == BezierControlPointMode.Free || (!this.loop && (num == 0 || num == this.modes.Length - 1)))
		{
			return;
		}
		int num2 = num * 3;
		int num3;
		int num4;
		if (index <= num2)
		{
			num3 = num2 - 1;
			if (num3 < 0)
			{
				num3 = this.points.Length - 2;
			}
			num4 = num2 + 1;
			if (num4 >= this.points.Length)
			{
				num4 = 1;
			}
		}
		else
		{
			num3 = num2 + 1;
			if (num3 >= this.points.Length)
			{
				num3 = 1;
			}
			num4 = num2 - 1;
			if (num4 < 0)
			{
				num4 = this.points.Length - 2;
			}
		}
		Vector3 vector = this.points[num2];
		Vector3 vector2 = vector - this.points[num3];
		if (bezierControlPointMode == BezierControlPointMode.Aligned)
		{
			vector2 = vector2.normalized * Vector3.Distance(vector, this.points[num4]);
		}
		this.points[num4] = vector + vector2;
	}

	// Token: 0x1700084F RID: 2127
	// (get) Token: 0x060056FC RID: 22268 RVA: 0x001C6683 File Offset: 0x001C4883
	public int CurveCount
	{
		get
		{
			return (this.points.Length - 1) / 3;
		}
	}

	// Token: 0x060056FD RID: 22269 RVA: 0x001C6691 File Offset: 0x001C4891
	public Vector3 GetPoint(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetPoint(this.getPathFromTime(t));
		}
		return this.GetPoint(t);
	}

	// Token: 0x060056FE RID: 22270 RVA: 0x001C66AC File Offset: 0x001C48AC
	public Vector3 GetPoint(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t));
	}

	// Token: 0x060056FF RID: 22271 RVA: 0x001C673C File Offset: 0x001C493C
	public Vector3 GetPointLocal(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return Bezier.GetPoint(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t);
	}

	// Token: 0x06005700 RID: 22272 RVA: 0x001C67C0 File Offset: 0x001C49C0
	public Vector3 GetVelocity(float t)
	{
		int num;
		if (t >= 1f)
		{
			t = 1f;
			num = this.points.Length - 4;
		}
		else
		{
			t = Mathf.Clamp01(t) * (float)this.CurveCount;
			num = (int)t;
			t -= (float)num;
			num *= 3;
		}
		return base.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[num], this.points[num + 1], this.points[num + 2], this.points[num + 3], t)) - base.transform.position;
	}

	// Token: 0x06005701 RID: 22273 RVA: 0x001C685D File Offset: 0x001C4A5D
	public Vector3 GetDirection(float t, bool ConstantVelocity)
	{
		if (ConstantVelocity)
		{
			return this.GetDirection(this.getPathFromTime(t));
		}
		return this.GetDirection(t);
	}

	// Token: 0x06005702 RID: 22274 RVA: 0x001C6878 File Offset: 0x001C4A78
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x06005703 RID: 22275 RVA: 0x001C6894 File Offset: 0x001C4A94
	public void AddCurve()
	{
		Vector3 vector = this.points[this.points.Length - 1];
		Array.Resize<Vector3>(ref this.points, this.points.Length + 3);
		vector.x += 1f;
		this.points[this.points.Length - 3] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 2] = vector;
		vector.x += 1f;
		this.points[this.points.Length - 1] = vector;
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length + 1);
		this.modes[this.modes.Length - 1] = this.modes[this.modes.Length - 2];
		this.EnforceMode(this.points.Length - 4);
		if (this.loop)
		{
			this.points[this.points.Length - 1] = this.points[0];
			this.modes[this.modes.Length - 1] = this.modes[0];
			this.EnforceMode(0);
		}
	}

	// Token: 0x06005704 RID: 22276 RVA: 0x001C69CE File Offset: 0x001C4BCE
	public void RemoveLastCurve()
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		Array.Resize<Vector3>(ref this.points, this.points.Length - 3);
		Array.Resize<BezierControlPointMode>(ref this.modes, this.modes.Length - 1);
	}

	// Token: 0x06005705 RID: 22277 RVA: 0x001C6A08 File Offset: 0x001C4C08
	public void RemoveCurve(int index)
	{
		if (this.points.Length <= 4)
		{
			return;
		}
		List<Vector3> list = this.points.ToList<Vector3>();
		int num = 4;
		while (num < this.points.Length && index - 3 > num)
		{
			num += 3;
		}
		for (int i = 0; i < 3; i++)
		{
			list.RemoveAt(num);
		}
		this.points = list.ToArray();
		int num2 = (num - 4) / 3;
		List<BezierControlPointMode> list2 = this.modes.ToList<BezierControlPointMode>();
		list2.RemoveAt(num2);
		this.modes = list2.ToArray();
	}

	// Token: 0x06005706 RID: 22278 RVA: 0x001C6A90 File Offset: 0x001C4C90
	public void Reset()
	{
		this.points = new Vector3[]
		{
			new Vector3(0f, -1f, 0f),
			new Vector3(0f, -1f, 2f),
			new Vector3(0f, -1f, 4f),
			new Vector3(0f, -1f, 6f)
		};
		this.modes = new BezierControlPointMode[2];
	}

	// Token: 0x040067C7 RID: 26567
	[SerializeField]
	private Vector3[] points;

	// Token: 0x040067C8 RID: 26568
	[SerializeField]
	private BezierControlPointMode[] modes;

	// Token: 0x040067C9 RID: 26569
	[SerializeField]
	private bool loop;

	// Token: 0x040067CA RID: 26570
	private float _totalArcLength;

	// Token: 0x040067CB RID: 26571
	private float[] _timesTable;

	// Token: 0x040067CC RID: 26572
	private float[] _lengthsTable;
}
