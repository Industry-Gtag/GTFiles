using System;
using UnityEngine;

// Token: 0x020008EF RID: 2287
public class GTSignalEmitter : MonoBehaviour
{
	// Token: 0x06003BEE RID: 15342 RVA: 0x0014790B File Offset: 0x00145B0B
	public virtual void Emit()
	{
		GTSignal.Emit(this.emitMode, this.signal, Array.Empty<object>());
	}

	// Token: 0x06003BEF RID: 15343 RVA: 0x00147928 File Offset: 0x00145B28
	public virtual void Emit(int targetActor)
	{
		GTSignal.Emit(targetActor, this.signal, Array.Empty<object>());
	}

	// Token: 0x06003BF0 RID: 15344 RVA: 0x00147940 File Offset: 0x00145B40
	public virtual void Emit(params object[] data)
	{
		GTSignal.Emit(this.emitMode, this.signal, data);
	}

	// Token: 0x04004C73 RID: 19571
	[Space]
	public GTSignalID signal;

	// Token: 0x04004C74 RID: 19572
	public GTSignal.EmitMode emitMode;
}
