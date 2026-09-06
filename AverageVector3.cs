using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000D80 RID: 3456
public class AverageVector3
{
	// Token: 0x06005534 RID: 21812 RVA: 0x001BE679 File Offset: 0x001BC879
	public AverageVector3(float averagingWindow = 0.1f)
	{
		this.timeWindow = averagingWindow;
	}

	// Token: 0x06005535 RID: 21813 RVA: 0x001BE6A0 File Offset: 0x001BC8A0
	public void AddSample(Vector3 sample, float time)
	{
		this.samples.Add(new AverageVector3.Sample
		{
			timeStamp = time,
			value = sample
		});
		this.RefreshSamples();
	}

	// Token: 0x06005536 RID: 21814 RVA: 0x001BE6D8 File Offset: 0x001BC8D8
	public Vector3 GetAverage()
	{
		this.RefreshSamples();
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.samples.Count; i++)
		{
			vector += this.samples[i].value;
		}
		return vector / (float)this.samples.Count;
	}

	// Token: 0x06005537 RID: 21815 RVA: 0x001BE733 File Offset: 0x001BC933
	public void Clear()
	{
		this.samples.Clear();
	}

	// Token: 0x06005538 RID: 21816 RVA: 0x001BE740 File Offset: 0x001BC940
	private void RefreshSamples()
	{
		float num = Time.time - this.timeWindow;
		for (int i = this.samples.Count - 1; i >= 0; i--)
		{
			if (this.samples[i].timeStamp < num)
			{
				this.samples.RemoveAt(i);
			}
		}
	}

	// Token: 0x040066C8 RID: 26312
	private List<AverageVector3.Sample> samples = new List<AverageVector3.Sample>();

	// Token: 0x040066C9 RID: 26313
	private float timeWindow = 0.1f;

	// Token: 0x02000D81 RID: 3457
	public struct Sample
	{
		// Token: 0x040066CA RID: 26314
		public float timeStamp;

		// Token: 0x040066CB RID: 26315
		public Vector3 value;
	}
}
