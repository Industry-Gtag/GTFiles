using System;
using UnityEngine;

// Token: 0x02000E95 RID: 3733
[Serializable]
public struct SerializableBSPNode
{
	// Token: 0x170008A6 RID: 2214
	// (get) Token: 0x06005AB2 RID: 23218 RVA: 0x001D8961 File Offset: 0x001D6B61
	public int matrixIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x170008A7 RID: 2215
	// (get) Token: 0x06005AB3 RID: 23219 RVA: 0x001D8969 File Offset: 0x001D6B69
	public int outsideChildIndex
	{
		get
		{
			return (int)this.rightChildIndex;
		}
	}

	// Token: 0x170008A8 RID: 2216
	// (get) Token: 0x06005AB4 RID: 23220 RVA: 0x001D8961 File Offset: 0x001D6B61
	public int zoneIndex
	{
		get
		{
			return (int)this.leftChildIndex;
		}
	}

	// Token: 0x04006BB2 RID: 27570
	[SerializeField]
	public SerializableBSPNode.Axis axis;

	// Token: 0x04006BB3 RID: 27571
	[SerializeField]
	public float splitValue;

	// Token: 0x04006BB4 RID: 27572
	[SerializeField]
	public short leftChildIndex;

	// Token: 0x04006BB5 RID: 27573
	[SerializeField]
	public short rightChildIndex;

	// Token: 0x02000E96 RID: 3734
	public enum Axis
	{
		// Token: 0x04006BB7 RID: 27575
		X,
		// Token: 0x04006BB8 RID: 27576
		Y,
		// Token: 0x04006BB9 RID: 27577
		Z,
		// Token: 0x04006BBA RID: 27578
		MatrixChain,
		// Token: 0x04006BBB RID: 27579
		MatrixFinal,
		// Token: 0x04006BBC RID: 27580
		Zone
	}
}
