using System;
using UnityEngine;

// Token: 0x020003E7 RID: 999
[Serializable]
public class IndexedAudioClip
{
	// Token: 0x060017BE RID: 6078 RVA: 0x0008866E File Offset: 0x0008686E
	public static implicit operator int(IndexedAudioClip a)
	{
		return a.intVal;
	}

	// Token: 0x060017BF RID: 6079 RVA: 0x00088676 File Offset: 0x00086876
	public static implicit operator IndexedAudioClip(int a)
	{
		return new IndexedAudioClip(a);
	}

	// Token: 0x060017C0 RID: 6080 RVA: 0x0008867E File Offset: 0x0008687E
	public IndexedAudioClip(int a)
	{
		this.intVal = a;
	}

	// Token: 0x04002306 RID: 8966
	[SerializeField]
	private int intVal = 67;
}
