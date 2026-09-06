using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E92 RID: 3730
public class BSPZoneData : MonoBehaviour
{
	// Token: 0x170008A4 RID: 2212
	// (get) Token: 0x06005AAF RID: 23215 RVA: 0x001D8959 File Offset: 0x001D6B59
	public int Priority
	{
		get
		{
			return this.priority;
		}
	}

	// Token: 0x170008A5 RID: 2213
	// (get) Token: 0x06005AB0 RID: 23216 RVA: 0x00017814 File Offset: 0x00015A14
	public string ZoneName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	// Token: 0x04006BAC RID: 27564
	[SerializeField]
	private int priority;

	// Token: 0x04006BAD RID: 27565
	[NonSerialized]
	public List<BoxCollider> boxList;
}
