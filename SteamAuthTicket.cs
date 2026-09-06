using System;
using Steamworks;
using UnityEngine;

// Token: 0x02000CDE RID: 3294
public class SteamAuthTicket : IDisposable
{
	// Token: 0x06005186 RID: 20870 RVA: 0x001AFC94 File Offset: 0x001ADE94
	private SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		this.m_hAuthTicket = hAuthTicket;
	}

	// Token: 0x06005187 RID: 20871 RVA: 0x001AFCA3 File Offset: 0x001ADEA3
	public static implicit operator SteamAuthTicket(HAuthTicket hAuthTicket)
	{
		return new SteamAuthTicket(hAuthTicket);
	}

	// Token: 0x06005188 RID: 20872 RVA: 0x001AFCAC File Offset: 0x001ADEAC
	~SteamAuthTicket()
	{
		this.Dispose();
	}

	// Token: 0x06005189 RID: 20873 RVA: 0x001AFCD8 File Offset: 0x001ADED8
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		if (this.m_hAuthTicket != HAuthTicket.Invalid)
		{
			try
			{
				SteamUser.CancelAuthTicket(this.m_hAuthTicket);
			}
			catch (InvalidOperationException)
			{
				Debug.LogWarning("Failed to invalidate a Steam auth ticket because the Steam API was shut down. Was it supposed to be disposed of sooner?");
			}
			this.m_hAuthTicket = HAuthTicket.Invalid;
		}
	}

	// Token: 0x04006397 RID: 25495
	private HAuthTicket m_hAuthTicket;
}
