using System;

// Token: 0x02000487 RID: 1159
public class StaticRPCEntry
{
	// Token: 0x06001C50 RID: 7248 RVA: 0x000999A0 File Offset: 0x00097BA0
	public StaticRPCEntry(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		this.placeholder = placeholder;
		this.code = code;
		this.lookupMethod = lookupMethod;
	}

	// Token: 0x0400266A RID: 9834
	public NetworkSystem.StaticRPCPlaceholder placeholder;

	// Token: 0x0400266B RID: 9835
	public byte code;

	// Token: 0x0400266C RID: 9836
	public NetworkSystem.StaticRPC lookupMethod;
}
