using System;
using Photon.Realtime;

// Token: 0x0200046B RID: 1131
public class NetEventOptions
{
	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001B79 RID: 7033 RVA: 0x0009584A File Offset: 0x00093A4A
	public bool HasWebHooks
	{
		get
		{
			return this.Flags != WebFlags.Default;
		}
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x0009585C File Offset: 0x00093A5C
	public NetEventOptions()
	{
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x0009586F File Offset: 0x00093A6F
	public NetEventOptions(int reciever, int[] actors, byte flags)
	{
		this.Reciever = (NetEventOptions.RecieverTarget)reciever;
		this.TargetActors = actors;
		this.Flags = new WebFlags(flags);
	}

	// Token: 0x040025AF RID: 9647
	public NetEventOptions.RecieverTarget Reciever;

	// Token: 0x040025B0 RID: 9648
	public int[] TargetActors;

	// Token: 0x040025B1 RID: 9649
	public WebFlags Flags = WebFlags.Default;

	// Token: 0x0200046C RID: 1132
	public enum RecieverTarget
	{
		// Token: 0x040025B3 RID: 9651
		others,
		// Token: 0x040025B4 RID: 9652
		all,
		// Token: 0x040025B5 RID: 9653
		master
	}
}
