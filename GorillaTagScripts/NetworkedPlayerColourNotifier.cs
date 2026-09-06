using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F8C RID: 3980
	public static class NetworkedPlayerColourNotifier
	{
		// Token: 0x060062EA RID: 25322 RVA: 0x001FD9F3 File Offset: 0x001FBBF3
		static NetworkedPlayerColourNotifier()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(NetworkedPlayerColourNotifier.OnPlayerJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(NetworkedPlayerColourNotifier.OnJoinedRoom);
		}

		// Token: 0x060062EB RID: 25323 RVA: 0x001FDA2B File Offset: 0x001FBC2B
		public static void SetLocalRigReference(RigContainer rig)
		{
			NetworkedPlayerColourNotifier.m_localRigContainer = rig;
			NetworkedPlayerColourNotifier.m_localRig = rig.Rig;
			NetworkedPlayerColourNotifier.m_localRig.OnColorChanged += NetworkedPlayerColourNotifier.OnLocalColourChanged;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		// Token: 0x060062EC RID: 25324 RVA: 0x001FDA5C File Offset: 0x001FBC5C
		public static void NotifyOthers()
		{
			if (!RoomSystem.JoinedRoom || NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNull())
			{
				return;
			}
			Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			float r = playerColor.r;
			float g = playerColor.g;
			float b = playerColor.b;
			NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.Others, new object[] { r, g, b });
		}

		// Token: 0x060062ED RID: 25325 RVA: 0x001FDAD6 File Offset: 0x001FBCD6
		private static void OnLocalColourChanged(Color color)
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			NetworkedPlayerColourNotifier.m_netColourDirty = NetworkedPlayerColourNotifier.m_initialNetColour != color;
		}

		// Token: 0x060062EE RID: 25326 RVA: 0x001FDAF0 File Offset: 0x001FBCF0
		private static void OnPlayerJoinedRoom(NetPlayer player)
		{
			if (NetworkedPlayerColourNotifier.m_netColourDirty && NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNotNull())
			{
				Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
				float r = playerColor.r;
				float g = playerColor.g;
				float b = playerColor.b;
				NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", player, new object[] { r, g, b });
			}
		}

		// Token: 0x060062EF RID: 25327 RVA: 0x001FDB69 File Offset: 0x001FBD69
		private static void OnJoinedRoom()
		{
			NetworkedPlayerColourNotifier.m_initialNetColour = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		// Token: 0x0400719E RID: 29086
		private static RigContainer m_localRigContainer;

		// Token: 0x0400719F RID: 29087
		private static VRRig m_localRig;

		// Token: 0x040071A0 RID: 29088
		private static Color m_initialNetColour;

		// Token: 0x040071A1 RID: 29089
		private static bool m_netColourDirty;
	}
}
