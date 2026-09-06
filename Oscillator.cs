using System;
using UnityEngine;

// Token: 0x02000027 RID: 39
public class Oscillator : MonoBehaviour
{
	// Token: 0x06000096 RID: 150 RVA: 0x000050BD File Offset: 0x000032BD
	public void Init(Vector3 center, Vector3 radius, Vector3 frequency, Vector3 startPhase)
	{
		this.Center = center;
		this.Radius = radius;
		this.Frequency = frequency;
		this.Phase = startPhase;
	}

	// Token: 0x06000097 RID: 151 RVA: 0x000050DC File Offset: 0x000032DC
	private float SampleWave(float phase)
	{
		switch (this.WaveType)
		{
		case Oscillator.WaveTypeEnum.Sine:
			return Mathf.Sin(phase);
		case Oscillator.WaveTypeEnum.Square:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase >= 3.1415927f)
			{
				return -1f;
			}
			return 1f;
		case Oscillator.WaveTypeEnum.Triangle:
			phase = Mathf.Repeat(phase, 6.2831855f);
			if (phase < 1.5707964f)
			{
				return phase / 1.5707964f;
			}
			if (phase < 3.1415927f)
			{
				return 1f - (phase - 1.5707964f) / 1.5707964f;
			}
			if (phase < 4.712389f)
			{
				return (3.1415927f - phase) / 1.5707964f;
			}
			return (phase - 4.712389f) / 1.5707964f - 1f;
		default:
			return 0f;
		}
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00005197 File Offset: 0x00003397
	public void OnEnable()
	{
		this.m_initCenter = base.transform.position;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x000051AC File Offset: 0x000033AC
	public void Update()
	{
		this.Phase += this.Frequency * 2f * 3.1415927f * Time.deltaTime;
		Vector3 vector = (this.UseCenter ? this.Center : this.m_initCenter);
		vector.x += this.Radius.x * this.SampleWave(this.Phase.x);
		vector.y += this.Radius.y * this.SampleWave(this.Phase.y);
		vector.z += this.Radius.z * this.SampleWave(this.Phase.z);
		base.transform.position = vector;
	}

	// Token: 0x040000AF RID: 175
	public Oscillator.WaveTypeEnum WaveType;

	// Token: 0x040000B0 RID: 176
	private Vector3 m_initCenter;

	// Token: 0x040000B1 RID: 177
	public bool UseCenter;

	// Token: 0x040000B2 RID: 178
	public Vector3 Center;

	// Token: 0x040000B3 RID: 179
	public Vector3 Radius;

	// Token: 0x040000B4 RID: 180
	public Vector3 Frequency;

	// Token: 0x040000B5 RID: 181
	public Vector3 Phase;

	// Token: 0x02000028 RID: 40
	public enum WaveTypeEnum
	{
		// Token: 0x040000B7 RID: 183
		Sine,
		// Token: 0x040000B8 RID: 184
		Square,
		// Token: 0x040000B9 RID: 185
		Triangle
	}
}
