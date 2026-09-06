using System;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;

// Token: 0x020005B5 RID: 1461
public class DebugAutoNamePlayer : MonoBehaviour
{
	// Token: 0x0600250D RID: 9485 RVA: 0x00002C2D File Offset: 0x00000E2D
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnEnable()
	{
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void OnDisable()
	{
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000C6C38 File Offset: 0x000C4E38
	private void Update()
	{
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000C6C45 File Offset: 0x000C4E45
	private void OnRoomJoined()
	{
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.m_joinDelayTimer = 2f;
		this.ApplyAutoName();
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000C6C74 File Offset: 0x000C4E74
	private void OnPlayersChanged()
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.ApplyAutoName();
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000C6C84 File Offset: 0x000C4E84
	private void OnZoneChange(ZoneData[] zones)
	{
		if (!RoomSystem.JoinedRoom)
		{
			return;
		}
		this.m_lastZone = DebugAutoNamePlayer.GetPrimaryZone();
		this.m_lastIsZoneAuthority = DebugAutoNamePlayer.GetIsZoneAuthority(this.m_lastZone);
		this.ApplyAutoName();
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000C6CB0 File Offset: 0x000C4EB0
	private void ApplyAutoName()
	{
		string platformCode = DebugAutoNamePlayer.GetPlatformCode();
		int localPlayerID = NetworkSystem.Instance.LocalPlayerID;
		string text = (NetworkSystem.Instance.IsMasterClient ? "MC" : "C");
		GTZone primaryZone = DebugAutoNamePlayer.GetPrimaryZone();
		string text2 = (DebugAutoNamePlayer.GetIsZoneAuthority(primaryZone) ? "ZA" : "Z");
		string text3 = primaryZone.ToString().ToUpper();
		string text4 = string.Format("{0}_{1}_{2}_{3}_{4}", new object[] { platformCode, localPlayerID, text, text2, text3 });
		if (text4.Length > 20)
		{
			text4 = text4.Substring(0, 20);
		}
		NetworkSystem.Instance.SetMyNickName(text4);
		if (GorillaComputer.instance != null)
		{
			GorillaComputer.instance.currentName = text4;
			GorillaComputer.instance.savedName = text4;
			GorillaComputer.instance.SetLocalNameTagText(text4);
		}
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				PlayerPrefs.GetFloat("redValue", 0f),
				PlayerPrefs.GetFloat("greenValue", 0f),
				PlayerPrefs.GetFloat("blueValue", 0f)
			});
		}
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000C6E0C File Offset: 0x000C500C
	private static GTZone GetPrimaryZone()
	{
		ZoneManagement instance = ZoneManagement.instance;
		if (instance != null && instance.activeZones.Count > 0)
		{
			return instance.activeZones[0];
		}
		return GTZone.forest;
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000C6E44 File Offset: 0x000C5044
	private static bool GetIsZoneAuthority(GTZone zone)
	{
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(zone);
		if (managerForZone == null)
		{
			return false;
		}
		NetPlayer localPlayer = NetworkSystem.Instance.LocalPlayer;
		return localPlayer != null && managerForZone.IsAuthorityPlayer(localPlayer);
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000C6E7A File Offset: 0x000C507A
	private static string GetPlatformCode()
	{
		return "ST";
	}

	// Token: 0x0400308A RID: 12426
	private float m_authorityPollTimer;

	// Token: 0x0400308B RID: 12427
	private float m_joinDelayTimer;

	// Token: 0x0400308C RID: 12428
	private bool m_lastIsZoneAuthority;

	// Token: 0x0400308D RID: 12429
	private GTZone m_lastZone;
}
