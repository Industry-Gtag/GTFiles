using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B8 RID: 184
public interface IEyeScannable
{
	// Token: 0x17000056 RID: 86
	// (get) Token: 0x0600047C RID: 1148
	int scannableId { get; }

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x0600047D RID: 1149
	Vector3 Position { get; }

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x0600047E RID: 1150
	Bounds Bounds { get; }

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x0600047F RID: 1151
	IList<KeyValueStringPair> Entries { get; }

	// Token: 0x06000480 RID: 1152
	void OnEnable();

	// Token: 0x06000481 RID: 1153
	void OnDisable();

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x06000482 RID: 1154
	// (remove) Token: 0x06000483 RID: 1155
	event Action OnDataChange;
}
