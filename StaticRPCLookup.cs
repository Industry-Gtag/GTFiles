using System;
using System.Collections.Generic;

// Token: 0x02000488 RID: 1160
public class StaticRPCLookup
{
	// Token: 0x06001C51 RID: 7249 RVA: 0x000999C0 File Offset: 0x00097BC0
	public void Add(NetworkSystem.StaticRPCPlaceholder placeholder, byte code, NetworkSystem.StaticRPC lookupMethod)
	{
		int count = this.entries.Count;
		this.entries.Add(new StaticRPCEntry(placeholder, code, lookupMethod));
		this.eventCodeEntryLookup.Add(code, count);
		this.placeholderEntryLookup.Add(placeholder, count);
	}

	// Token: 0x06001C52 RID: 7250 RVA: 0x00099A06 File Offset: 0x00097C06
	public NetworkSystem.StaticRPC CodeToMethod(byte code)
	{
		return this.entries[this.eventCodeEntryLookup[code]].lookupMethod;
	}

	// Token: 0x06001C53 RID: 7251 RVA: 0x00099A24 File Offset: 0x00097C24
	public byte PlaceholderToCode(NetworkSystem.StaticRPCPlaceholder placeholder)
	{
		return this.entries[this.placeholderEntryLookup[placeholder]].code;
	}

	// Token: 0x0400266D RID: 9837
	public List<StaticRPCEntry> entries = new List<StaticRPCEntry>();

	// Token: 0x0400266E RID: 9838
	private Dictionary<byte, int> eventCodeEntryLookup = new Dictionary<byte, int>();

	// Token: 0x0400266F RID: 9839
	private Dictionary<NetworkSystem.StaticRPCPlaceholder, int> placeholderEntryLookup = new Dictionary<NetworkSystem.StaticRPCPlaceholder, int>();
}
