using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005CA RID: 1482
public sealed class CasualGameMode : GorillaGameManager
{
	// Token: 0x06002555 RID: 9557 RVA: 0x00002076 File Offset: 0x00000276
	public override int MyMatIndex(NetPlayer player)
	{
		return 0;
	}

	// Token: 0x06002556 RID: 9558 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06002557 RID: 9559 RVA: 0x00036275 File Offset: 0x00034475
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06002558 RID: 9560 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002559 RID: 9561 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600255A RID: 9562 RVA: 0x00002076 File Offset: 0x00000276
	public override GameModeType GameType()
	{
		return GameModeType.Casual;
	}

	// Token: 0x0600255B RID: 9563 RVA: 0x0003627C File Offset: 0x0003447C
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x0600255C RID: 9564 RVA: 0x000C7D78 File Offset: 0x000C5F78
	public override string GameModeName()
	{
		return "CASUAL";
	}

	// Token: 0x0600255D RID: 9565 RVA: 0x000C7D80 File Offset: 0x000C5F80
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_CASUAL_ROOM_LABEL", out text, "(CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_CASUAL_ROOM_LABEL]");
		}
		return text;
	}
}
