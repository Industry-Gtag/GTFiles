using System;
using UnityEngine;

// Token: 0x02000B1C RID: 2844
[Serializable]
public class VelocityHelper
{
	// Token: 0x0600491C RID: 18716 RVA: 0x001872F5 File Offset: 0x001854F5
	public VelocityHelper(int historySize = 12)
	{
		this._size = historySize;
		this._samples = new float[historySize * 4];
	}

	// Token: 0x0600491D RID: 18717 RVA: 0x00187314 File Offset: 0x00185514
	public void SamplePosition(Transform target, float dt)
	{
		Vector3 position = target.position;
		if (!this._initialized)
		{
			this._InitSamples(position, dt);
		}
		this._SetSample(this._latest, position, dt);
		this._latest = (this._latest + 1) % this._size;
	}

	// Token: 0x0600491E RID: 18718 RVA: 0x0018735C File Offset: 0x0018555C
	private void _InitSamples(Vector3 position, float dt)
	{
		for (int i = 0; i < this._size; i++)
		{
			this._SetSample(i, position, dt);
		}
		this._initialized = true;
	}

	// Token: 0x0600491F RID: 18719 RVA: 0x0018738A File Offset: 0x0018558A
	private void _SetSample(int i, Vector3 position, float dt)
	{
		this._samples[i] = position.x;
		this._samples[i + 1] = position.y;
		this._samples[i + 2] = position.z;
		this._samples[i + 3] = dt;
	}

	// Token: 0x04005B3B RID: 23355
	private float[] _samples;

	// Token: 0x04005B3C RID: 23356
	private int _latest;

	// Token: 0x04005B3D RID: 23357
	private int _size;

	// Token: 0x04005B3E RID: 23358
	private bool _initialized;
}
