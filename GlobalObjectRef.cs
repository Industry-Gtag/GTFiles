using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x02000AF8 RID: 2808
[Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct GlobalObjectRef
{
	// Token: 0x060047EB RID: 18411 RVA: 0x00184374 File Offset: 0x00182574
	public static GlobalObjectRef ObjectToRefSlow(Object target)
	{
		return default(GlobalObjectRef);
	}

	// Token: 0x060047EC RID: 18412 RVA: 0x00036275 File Offset: 0x00034475
	public static Object RefToObjectSlow(GlobalObjectRef @ref)
	{
		return null;
	}

	// Token: 0x04005A7D RID: 23165
	[FieldOffset(0)]
	public ulong targetObjectId;

	// Token: 0x04005A7E RID: 23166
	[FieldOffset(8)]
	public ulong targetPrefabId;

	// Token: 0x04005A7F RID: 23167
	[FieldOffset(16)]
	public Guid assetGUID;

	// Token: 0x04005A80 RID: 23168
	[HideInInspector]
	[FieldOffset(32)]
	public int identifierType;

	// Token: 0x04005A81 RID: 23169
	[NonSerialized]
	[FieldOffset(32)]
	private GlobalObjectRefType refType;
}
