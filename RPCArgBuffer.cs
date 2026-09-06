using System;
using System.Runtime.InteropServices;

// Token: 0x02000452 RID: 1106
public struct RPCArgBuffer<T> where T : struct
{
	// Token: 0x06001A72 RID: 6770 RVA: 0x000942EE File Offset: 0x000924EE
	public RPCArgBuffer(T argStruct)
	{
		this.DataLength = Marshal.SizeOf(typeof(T));
		this.Data = new byte[this.DataLength];
		this.Args = argStruct;
	}

	// Token: 0x0400252D RID: 9517
	public T Args;

	// Token: 0x0400252E RID: 9518
	public byte[] Data;

	// Token: 0x0400252F RID: 9519
	public int DataLength;
}
