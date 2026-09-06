using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000AD5 RID: 2773
internal class VirtualStumpTeleporterSerializer : GorillaSerializer
{
	// Token: 0x0600473C RID: 18236 RVA: 0x001806C3 File Offset: 0x0017E8C3
	public void NotifyPlayerTeleporting(short teleporterIdx, AudioSource localPlayerTeleporterAudioSource)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[] { false, teleporterIdx });
		}
	}

	// Token: 0x0600473D RID: 18237 RVA: 0x00180700 File Offset: 0x0017E900
	public void NotifyPlayerReturning(short teleporterIdx)
	{
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		Debug.Log(string.Format("[VRTeleporterSerializer::NotifyPlayerReturning] Sending RPC to activate VFX at idx: {0}", teleporterIdx));
		if (PhotonNetwork.InRoom)
		{
			base.SendRPC("ActivateTeleportVFX", true, new object[] { true, teleporterIdx });
		}
	}

	// Token: 0x0600473E RID: 18238 RVA: 0x0018075C File Offset: 0x0017E95C
	[PunRPC]
	private void ActivateTeleportVFX(bool returning, short teleporterIdx, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "ActivateTeleportVFX");
		if ((int)teleporterIdx >= this.teleporters.Count)
		{
			return;
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[13].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VirtualStumpTeleporter virtualStumpTeleporter = this.teleporters[(int)teleporterIdx];
		if (virtualStumpTeleporter.IsNotNull())
		{
			virtualStumpTeleporter.PlayTeleportEffects(false, !returning, null, false);
		}
	}

	// Token: 0x0600473F RID: 18239 RVA: 0x001807EC File Offset: 0x0017E9EC
	public short GetTeleporterIndex(VirtualStumpTeleporter teleporter)
	{
		short num = 0;
		while ((int)num < this.teleporters.Count)
		{
			if (this.teleporters[(int)num] == teleporter)
			{
				return num;
			}
			num += 1;
		}
		return -1;
	}

	// Token: 0x040059BE RID: 22974
	[SerializeField]
	public List<VirtualStumpTeleporter> teleporters = new List<VirtualStumpTeleporter>();

	// Token: 0x040059BF RID: 22975
	[SerializeField]
	public List<ParticleSystem> teleporterVFX = new List<ParticleSystem>();

	// Token: 0x040059C0 RID: 22976
	[SerializeField]
	public List<ParticleSystem> returnVFX = new List<ParticleSystem>();

	// Token: 0x040059C1 RID: 22977
	[SerializeField]
	public List<AudioSource> teleportAudioSource = new List<AudioSource>();

	// Token: 0x040059C2 RID: 22978
	[SerializeField]
	public List<AudioClip> teleportingPlayerSoundClips = new List<AudioClip>();

	// Token: 0x040059C3 RID: 22979
	[SerializeField]
	public List<AudioClip> observerSoundClips = new List<AudioClip>();
}
