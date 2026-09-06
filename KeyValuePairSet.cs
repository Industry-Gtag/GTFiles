using System;
using UnityEngine;

// Token: 0x020000B9 RID: 185
[CreateAssetMenu(fileName = "New KeyValuePairSet", menuName = "Data/KeyValuePairSet", order = 0)]
public class KeyValuePairSet : ScriptableObject
{
	// Token: 0x1700005A RID: 90
	// (get) Token: 0x06000484 RID: 1156 RVA: 0x00019CFD File Offset: 0x00017EFD
	public KeyValueStringPair[] Entries
	{
		get
		{
			return this.m_entries;
		}
	}

	// Token: 0x040004E4 RID: 1252
	[SerializeField]
	private KeyValueStringPair[] m_entries;
}
