using System;

// Token: 0x02000DCB RID: 3531
public struct EnterPlayID
{
	// Token: 0x060056A2 RID: 22178 RVA: 0x001C456F File Offset: 0x001C276F
	[OnEnterPlay_Run]
	private static void NextID()
	{
		EnterPlayID.currentID++;
	}

	// Token: 0x060056A3 RID: 22179 RVA: 0x001C4580 File Offset: 0x001C2780
	public static EnterPlayID GetCurrent()
	{
		return new EnterPlayID
		{
			id = EnterPlayID.currentID
		};
	}

	// Token: 0x17000846 RID: 2118
	// (get) Token: 0x060056A4 RID: 22180 RVA: 0x001C45A2 File Offset: 0x001C27A2
	public bool IsCurrent
	{
		get
		{
			return this.id == EnterPlayID.currentID;
		}
	}

	// Token: 0x04006796 RID: 26518
	private static int currentID = 1;

	// Token: 0x04006797 RID: 26519
	private int id;
}
