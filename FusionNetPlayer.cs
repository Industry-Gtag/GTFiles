using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

// Token: 0x02000435 RID: 1077
public class FusionNetPlayer : NetPlayer
{
	// Token: 0x1700027F RID: 639
	// (get) Token: 0x0600199A RID: 6554 RVA: 0x0008FCE1 File Offset: 0x0008DEE1
	// (set) Token: 0x0600199B RID: 6555 RVA: 0x0008FCE9 File Offset: 0x0008DEE9
	public PlayerRef PlayerRef { get; private set; }

	// Token: 0x0600199C RID: 6556 RVA: 0x0008FCF4 File Offset: 0x0008DEF4
	public FusionNetPlayer()
	{
		this.PlayerRef = default(PlayerRef);
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x0008FD16 File Offset: 0x0008DF16
	public FusionNetPlayer(PlayerRef playerRef)
	{
		this.PlayerRef = playerRef;
	}

	// Token: 0x17000280 RID: 640
	// (get) Token: 0x0600199E RID: 6558 RVA: 0x0008FD25 File Offset: 0x0008DF25
	private NetworkRunner runner
	{
		get
		{
			return ((NetworkSystemFusion)NetworkSystem.Instance).runner;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x0600199F RID: 6559 RVA: 0x0008FD38 File Offset: 0x0008DF38
	public override bool IsValid
	{
		get
		{
			return this.validPlayer && this.PlayerRef.IsRealPlayer;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x060019A0 RID: 6560 RVA: 0x0008FD60 File Offset: 0x0008DF60
	public override int ActorNumber
	{
		get
		{
			return this.PlayerRef.PlayerId;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x060019A1 RID: 6561 RVA: 0x0008FD7C File Offset: 0x0008DF7C
	public override string UserId
	{
		get
		{
			return NetworkSystem.Instance.GetUserID(this.PlayerRef.PlayerId);
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x060019A2 RID: 6562 RVA: 0x0008FDA4 File Offset: 0x0008DFA4
	public override bool IsMasterClient
	{
		get
		{
			if (!(this.runner == null))
			{
				return (this.IsLocal && this.runner.IsSharedModeMasterClient) || NetworkSystem.Instance.MasterClient == this;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x060019A3 RID: 6563 RVA: 0x0008FDF8 File Offset: 0x0008DFF8
	public override bool IsLocal
	{
		get
		{
			if (!(this.runner == null))
			{
				return this.PlayerRef == this.runner.LocalPlayer;
			}
			return this.PlayerRef == default(PlayerRef);
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x060019A4 RID: 6564 RVA: 0x0008FE3E File Offset: 0x0008E03E
	public override bool IsNull
	{
		get
		{
			PlayerRef playerRef = this.PlayerRef;
			return false;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x060019A5 RID: 6565 RVA: 0x0008FE48 File Offset: 0x0008E048
	public override string NickName
	{
		get
		{
			return NetworkSystem.Instance.GetNickName(this);
		}
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x060019A6 RID: 6566 RVA: 0x0008FE58 File Offset: 0x0008E058
	public override string DefaultName
	{
		get
		{
			if (string.IsNullOrEmpty(this._defaultName))
			{
				this._defaultName = "gorilla" + Random.Range(0, 9999).ToString().PadLeft(4, '0');
			}
			return this._defaultName;
		}
	}

	// Token: 0x17000289 RID: 649
	// (get) Token: 0x060019A7 RID: 6567 RVA: 0x0008FEA4 File Offset: 0x0008E0A4
	public override bool InRoom
	{
		get
		{
			using (IEnumerator<PlayerRef> enumerator = this.runner.ActivePlayers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == this.PlayerRef)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0008FF04 File Offset: 0x0008E104
	public override bool Equals(NetPlayer myPlayer, NetPlayer other)
	{
		return myPlayer != null && other != null && ((FusionNetPlayer)myPlayer).PlayerRef.Equals(((FusionNetPlayer)other).PlayerRef);
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x0008FF37 File Offset: 0x0008E137
	public void InitPlayer(PlayerRef player)
	{
		this.PlayerRef = player;
		this.validPlayer = true;
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x0008FF48 File Offset: 0x0008E148
	public override void OnReturned()
	{
		base.OnReturned();
		this.PlayerRef = default(PlayerRef);
		if (this.PlayerRef.PlayerId != -1)
		{
			Debug.LogError("Returned Player to pool but isnt -1, broken");
		}
	}

	// Token: 0x060019AB RID: 6571 RVA: 0x0008FF85 File Offset: 0x0008E185
	public override void OnTaken()
	{
		base.OnTaken();
	}

	// Token: 0x04002482 RID: 9346
	private string _defaultName;

	// Token: 0x04002483 RID: 9347
	private bool validPlayer;
}
