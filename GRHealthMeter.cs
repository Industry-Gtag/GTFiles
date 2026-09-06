using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007BE RID: 1982
public class GRHealthMeter : MonoBehaviour
{
	// Token: 0x060032A8 RID: 12968 RVA: 0x00115C5F File Offset: 0x00113E5F
	public void Setup(int maxHP)
	{
		this.maxHP = maxHP;
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x00115C68 File Offset: 0x00113E68
	public void SetHP(int hp)
	{
		int num = Mathf.CeilToInt((float)hp / (float)this.maxHP * (float)this.nodes.Count);
		for (int i = 0; i < this.nodes.Count; i++)
		{
			this.nodes[i].SetEmpty(i >= num);
		}
	}

	// Token: 0x040041AB RID: 16811
	public List<GRHealthMeterNode> nodes;

	// Token: 0x040041AC RID: 16812
	private int maxHP;
}
