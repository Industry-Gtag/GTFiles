using System;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200017F RID: 383
public sealed class SuperCasualGame : GorillaGameManager
{
	// Token: 0x06000A14 RID: 2580 RVA: 0x00002076 File Offset: 0x00000276
	public override int MyMatIndex(NetPlayer player)
	{
		return 0;
	}

	// Token: 0x06000A15 RID: 2581 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(object newData)
	{
	}

	// Token: 0x06000A16 RID: 2582 RVA: 0x00036275 File Offset: 0x00034475
	public override object OnSerializeWrite()
	{
		return null;
	}

	// Token: 0x06000A17 RID: 2583 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000A18 RID: 2584 RVA: 0x00002C2D File Offset: 0x00000E2D
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000A19 RID: 2585 RVA: 0x00036278 File Offset: 0x00034478
	public override GameModeType GameType()
	{
		return GameModeType.SuperCasual;
	}

	// Token: 0x06000A1A RID: 2586 RVA: 0x0003627C File Offset: 0x0003447C
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<CasualGameModeData>();
	}

	// Token: 0x06000A1B RID: 2587 RVA: 0x00036285 File Offset: 0x00034485
	public override string GameModeName()
	{
		return "SUPER CASUAL";
	}

	// Token: 0x06000A1C RID: 2588 RVA: 0x0003628C File Offset: 0x0003448C
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_SUPER_CASUAL_ROOM_LABEL", out text, "(SUPER CASUAL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_SUPER_CASUAL_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x06000A1D RID: 2589 RVA: 0x000362B8 File Offset: 0x000344B8
	public override void StartPlaying()
	{
		base.StartPlaying();
		VRRig.LocalRig.EnableSuperInfectionHands(true);
		for (int i = 0; i < this.currentNetPlayerArray.Length; i++)
		{
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(this.currentNetPlayerArray[i], out rigContainer))
			{
				rigContainer.Rig.EnableSuperInfectionHands(true);
			}
		}
	}

	// Token: 0x06000A1E RID: 2590 RVA: 0x0003630B File Offset: 0x0003450B
	public override void StopPlaying()
	{
		base.StopPlaying();
		VRRig.LocalRig.EnableSuperInfectionHands(false);
	}

	// Token: 0x06000A1F RID: 2591 RVA: 0x00036320 File Offset: 0x00034520
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(newPlayer, out rigContainer))
		{
			rigContainer.Rig.EnableSuperInfectionHands(true);
		}
	}
}
