using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000882 RID: 2178
public sealed class GorillaHuntManager : GorillaGameManager
{
	// Token: 0x060038AD RID: 14509 RVA: 0x000133F3 File Offset: 0x000115F3
	public override GameModeType GameType()
	{
		return GameModeType.HuntDown;
	}

	// Token: 0x060038AE RID: 14510 RVA: 0x00134FAB File Offset: 0x001331AB
	public override string GameModeName()
	{
		return "HUNTDOWN";
	}

	// Token: 0x060038AF RID: 14511 RVA: 0x00134FB4 File Offset: 0x001331B4
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_HUNT_ROOM_LABEL", out text, "(HUNTDOWN GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_HUNT_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x060038B0 RID: 14512 RVA: 0x00134FDF File Offset: 0x001331DF
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<HuntGameModeData>();
	}

	// Token: 0x060038B1 RID: 14513 RVA: 0x00134FE8 File Offset: 0x001331E8
	public override void StartPlaying()
	{
		base.StartPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(true);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentHunted.Count; i++)
			{
				this.tempPlayer = this.currentHunted[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentHunted.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < this.currentTarget.Count; i++)
			{
				this.tempPlayer = this.currentTarget[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentTarget.RemoveAt(i);
					i--;
				}
			}
			this.UpdateState();
		}
	}

	// Token: 0x060038B2 RID: 14514 RVA: 0x001350BF File Offset: 0x001332BF
	public override void StopPlaying()
	{
		base.StopPlaying();
		GorillaTagger.Instance.offlineVRRig.huntComputer.SetActive(false);
		base.StopAllCoroutines();
	}

	// Token: 0x060038B3 RID: 14515 RVA: 0x001350E4 File Offset: 0x001332E4
	public override void ResetGame()
	{
		base.ResetGame();
		this.currentHunted.Clear();
		this.currentTarget.Clear();
		for (int i = 0; i < this.currentHuntedArray.Length; i++)
		{
			this.currentHuntedArray[i] = -1;
			this.currentTargetArray[i] = -1;
		}
		this.huntStarted = false;
		this.waitingToStartNextHuntGame = false;
		this.inStartCountdown = false;
		this.timeHuntGameEnded = 0.0;
		this.countDownTime = 0;
		this.timeLastSlowTagged = 0f;
	}

	// Token: 0x060038B4 RID: 14516 RVA: 0x00135168 File Offset: 0x00133368
	public void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (NetworkSystem.Instance.RoomPlayerCount <= 3)
			{
				this.CleanUpHunt();
				this.huntStarted = false;
				this.waitingToStartNextHuntGame = false;
				this.iterator1 = 0;
				while (this.iterator1 < RoomSystem.PlayersInRoom.Count)
				{
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, RoomSystem.PlayersInRoom[this.iterator1], false);
					this.iterator1++;
				}
				return;
			}
			if (NetworkSystem.Instance.RoomPlayerCount > 3 && !this.huntStarted && !this.waitingToStartNextHuntGame && !this.inStartCountdown)
			{
				Utils.Log("<color=red> there are enough players</color>", this);
				base.StartCoroutine(this.StartHuntCountdown());
				return;
			}
			this.UpdateHuntState();
		}
	}

	// Token: 0x060038B5 RID: 14517 RVA: 0x0013522F File Offset: 0x0013342F
	public void CleanUpHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.currentHunted.Clear();
			this.currentTarget.Clear();
		}
	}

	// Token: 0x060038B6 RID: 14518 RVA: 0x00135253 File Offset: 0x00133453
	public IEnumerator StartHuntCountdown()
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.inStartCountdown)
		{
			this.inStartCountdown = true;
			this.countDownTime = 5;
			this.CleanUpHunt();
			while (this.countDownTime > 0)
			{
				yield return new WaitForSeconds(1f);
				this.countDownTime--;
			}
			this.StartHunt();
		}
		yield return null;
		yield break;
	}

	// Token: 0x060038B7 RID: 14519 RVA: 0x00135264 File Offset: 0x00133464
	public void StartHunt()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.huntStarted = true;
			this.waitingToStartNextHuntGame = false;
			this.countDownTime = 0;
			this.inStartCountdown = false;
			this.CleanUpHunt();
			this.iterator1 = 0;
			while (this.iterator1 < NetworkSystem.Instance.AllNetPlayers.Count<NetPlayer>())
			{
				if (this.currentTarget.Count < 10)
				{
					this.currentTarget.Add(NetworkSystem.Instance.AllNetPlayers[this.iterator1]);
					RoomSystem.SendSoundEffectToPlayer(0, 0.25f, NetworkSystem.Instance.AllNetPlayers[this.iterator1], false);
				}
				this.iterator1++;
			}
			this.RandomizePlayerList(ref this.currentTarget);
		}
	}

	// Token: 0x060038B8 RID: 14520 RVA: 0x00135324 File Offset: 0x00133524
	public void RandomizePlayerList(ref List<NetPlayer> listToRandomize)
	{
		for (int i = 0; i < listToRandomize.Count - 1; i++)
		{
			this.tempRandIndex = Random.Range(i, listToRandomize.Count);
			this.tempRandPlayer = listToRandomize[i];
			listToRandomize[i] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandPlayer;
		}
	}

	// Token: 0x060038B9 RID: 14521 RVA: 0x0013538E File Offset: 0x0013358E
	public IEnumerator HuntEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while ((double)Time.time < this.timeHuntGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.waitingToStartNextHuntGame)
			{
				base.StartCoroutine(this.StartHuntCountdown());
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	// Token: 0x060038BA RID: 14522 RVA: 0x001353A0 File Offset: 0x001335A0
	public void UpdateHuntState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.notHuntedCount = 0;
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				if (this.currentTarget.Contains(netPlayer) && !this.currentHunted.Contains(netPlayer))
				{
					this.notHuntedCount++;
				}
			}
			if (this.notHuntedCount <= 2 && this.huntStarted)
			{
				this.EndHuntGame();
			}
		}
	}

	// Token: 0x060038BB RID: 14523 RVA: 0x00135440 File Offset: 0x00133640
	private void EndHuntGame()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, false);
			}
			this.huntStarted = false;
			this.timeHuntGameEnded = (double)Time.time;
			this.waitingToStartNextHuntGame = true;
			base.StartCoroutine(this.HuntEnd());
		}
	}

	// Token: 0x060038BC RID: 14524 RVA: 0x001354D4 File Offset: 0x001336D4
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.waitingToStartNextHuntGame || this.countDownTime > 0 || GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Frozen)
		{
			return false;
		}
		if (this.currentHunted.Contains(myPlayer) && !this.currentHunted.Contains(otherPlayer) && Time.time > this.timeLastSlowTagged + 1f)
		{
			this.timeLastSlowTagged = Time.time;
			return true;
		}
		return this.IsTargetOf(myPlayer, otherPlayer);
	}

	// Token: 0x060038BD RID: 14525 RVA: 0x0013554A File Offset: 0x0013374A
	public override bool LocalIsTagged(NetPlayer player)
	{
		return !this.waitingToStartNextHuntGame && this.countDownTime <= 0 && this.currentHunted.Contains(player);
	}

	// Token: 0x060038BE RID: 14526 RVA: 0x0013556C File Offset: 0x0013376C
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame)
		{
			if ((this.currentHunted.Contains(taggingPlayer) || !this.currentTarget.Contains(taggingPlayer)) && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				return;
			}
			if (this.IsTargetOf(taggingPlayer, taggedPlayer))
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
				this.currentHunted.Add(taggedPlayer);
				this.UpdateHuntState();
			}
		}
	}

	// Token: 0x060038BF RID: 14527 RVA: 0x00135610 File Offset: 0x00133810
	public bool IsTargetOf(NetPlayer huntingPlayer, NetPlayer huntedPlayer)
	{
		return !this.currentHunted.Contains(huntingPlayer) && !this.currentHunted.Contains(huntedPlayer) && this.currentTarget.Contains(huntingPlayer) && this.currentTarget.Contains(huntedPlayer) && huntedPlayer == this.GetTargetOf(huntingPlayer);
	}

	// Token: 0x060038C0 RID: 14528 RVA: 0x00135664 File Offset: 0x00133864
	public NetPlayer GetTargetOf(NetPlayer netPlayer)
	{
		if (this.currentHunted.Contains(netPlayer) || !this.currentTarget.Contains(netPlayer))
		{
			return null;
		}
		this.tempTargetIndex = this.currentTarget.IndexOf(netPlayer);
		for (int num = (this.tempTargetIndex + 1) % this.currentTarget.Count; num != this.tempTargetIndex; num = (num + 1) % this.currentTarget.Count)
		{
			if (this.currentTarget[num] == netPlayer)
			{
				return null;
			}
			if (!this.currentHunted.Contains(this.currentTarget[num]) && this.currentTarget[num] != null)
			{
				return this.currentTarget[num];
			}
		}
		return null;
	}

	// Token: 0x060038C1 RID: 14529 RVA: 0x00135718 File Offset: 0x00133918
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient && !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(taggedPlayer) && this.currentTarget.Contains(taggedPlayer))
		{
			RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, taggedPlayer);
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			this.currentHunted.Add(taggedPlayer);
			this.UpdateHuntState();
		}
	}

	// Token: 0x060038C2 RID: 14530 RVA: 0x0013577B File Offset: 0x0013397B
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return !this.waitingToStartNextHuntGame && !this.currentHunted.Contains(player) && this.currentTarget.Contains(player);
	}

	// Token: 0x060038C3 RID: 14531 RVA: 0x001357A1 File Offset: 0x001339A1
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
	}

	// Token: 0x060038C4 RID: 14532 RVA: 0x001357B5 File Offset: 0x001339B5
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (!this.waitingToStartNextHuntGame && this.huntStarted)
			{
				this.currentHunted.Add(player);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060038C5 RID: 14533 RVA: 0x001357F0 File Offset: 0x001339F0
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentTarget.Contains(otherPlayer))
			{
				this.currentTarget.Remove(otherPlayer);
			}
			if (this.currentHunted.Contains(otherPlayer))
			{
				this.currentHunted.Remove(otherPlayer);
			}
			this.UpdateState();
		}
	}

	// Token: 0x060038C6 RID: 14534 RVA: 0x0013584C File Offset: 0x00133A4C
	private void CopyHuntDataListToArray()
	{
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < 10)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = -1;
			this.currentTargetArray[this.copyListToArrayIndex] = -1;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = this.currentHunted.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentHunted[this.copyListToArrayIndex] == null)
			{
				this.currentHunted.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = this.currentTarget.Count - 1;
		while (this.copyListToArrayIndex >= 0)
		{
			if (this.currentTarget[this.copyListToArrayIndex] == null)
			{
				this.currentTarget.RemoveAt(this.copyListToArrayIndex);
			}
			this.copyListToArrayIndex--;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentHunted.Count)
		{
			this.currentHuntedArray[this.copyListToArrayIndex] = this.currentHunted[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
		this.copyListToArrayIndex = 0;
		while (this.copyListToArrayIndex < this.currentTarget.Count)
		{
			this.currentTargetArray[this.copyListToArrayIndex] = this.currentTarget[this.copyListToArrayIndex].ActorNumber;
			this.copyListToArrayIndex++;
		}
	}

	// Token: 0x060038C7 RID: 14535 RVA: 0x001359D0 File Offset: 0x00133BD0
	private void CopyHuntDataArrayToList()
	{
		this.currentTarget.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentTargetArray.Length)
		{
			if (this.currentTargetArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentTargetArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentTarget.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
		this.currentHunted.Clear();
		this.copyArrayToListIndex = 0;
		while (this.copyArrayToListIndex < this.currentHuntedArray.Length)
		{
			if (this.currentHuntedArray[this.copyArrayToListIndex] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentHuntedArray[this.copyArrayToListIndex]);
				if (this.tempPlayer != null)
				{
					this.currentHunted.Add(this.tempPlayer);
				}
			}
			this.copyArrayToListIndex++;
		}
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x00135ACD File Offset: 0x00133CCD
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x00135AEE File Offset: 0x00133CEE
	public void CopyRoomDataToLocalData()
	{
		this.waitingToStartNextHuntGame = false;
		this.UpdateHuntState();
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x00135B00 File Offset: 0x00133D00
	public override void OnSerializeRead(object newData)
	{
		HuntData huntData = (HuntData)newData;
		huntData.currentHuntedArray.CopyTo(this.currentHuntedArray, true);
		huntData.currentTargetArray.CopyTo(this.currentTargetArray, true);
		this.huntStarted = huntData.huntStarted;
		this.waitingToStartNextHuntGame = huntData.waitingToStartNextHuntGame;
		this.countDownTime = huntData.countDownTime;
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x00135B74 File Offset: 0x00133D74
	public override object OnSerializeWrite()
	{
		this.CopyHuntDataListToArray();
		HuntData huntData = default(HuntData);
		huntData.currentHuntedArray.CopyFrom(this.currentHuntedArray, 0, this.currentHuntedArray.Length);
		huntData.currentTargetArray.CopyFrom(this.currentTargetArray, 0, this.currentTargetArray.Length);
		huntData.huntStarted = this.huntStarted;
		huntData.waitingToStartNextHuntGame = this.waitingToStartNextHuntGame;
		huntData.countDownTime = this.countDownTime;
		return huntData;
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x00135C04 File Offset: 0x00133E04
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyHuntDataListToArray();
		stream.SendNext(this.currentHuntedArray[0]);
		stream.SendNext(this.currentHuntedArray[1]);
		stream.SendNext(this.currentHuntedArray[2]);
		stream.SendNext(this.currentHuntedArray[3]);
		stream.SendNext(this.currentHuntedArray[4]);
		stream.SendNext(this.currentHuntedArray[5]);
		stream.SendNext(this.currentHuntedArray[6]);
		stream.SendNext(this.currentHuntedArray[7]);
		stream.SendNext(this.currentHuntedArray[8]);
		stream.SendNext(this.currentHuntedArray[9]);
		stream.SendNext(this.currentTargetArray[0]);
		stream.SendNext(this.currentTargetArray[1]);
		stream.SendNext(this.currentTargetArray[2]);
		stream.SendNext(this.currentTargetArray[3]);
		stream.SendNext(this.currentTargetArray[4]);
		stream.SendNext(this.currentTargetArray[5]);
		stream.SendNext(this.currentTargetArray[6]);
		stream.SendNext(this.currentTargetArray[7]);
		stream.SendNext(this.currentTargetArray[8]);
		stream.SendNext(this.currentTargetArray[9]);
		stream.SendNext(this.huntStarted);
		stream.SendNext(this.waitingToStartNextHuntGame);
		stream.SendNext(this.countDownTime);
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x00135DC8 File Offset: 0x00133FC8
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		this.currentHuntedArray[0] = (int)stream.ReceiveNext();
		this.currentHuntedArray[1] = (int)stream.ReceiveNext();
		this.currentHuntedArray[2] = (int)stream.ReceiveNext();
		this.currentHuntedArray[3] = (int)stream.ReceiveNext();
		this.currentHuntedArray[4] = (int)stream.ReceiveNext();
		this.currentHuntedArray[5] = (int)stream.ReceiveNext();
		this.currentHuntedArray[6] = (int)stream.ReceiveNext();
		this.currentHuntedArray[7] = (int)stream.ReceiveNext();
		this.currentHuntedArray[8] = (int)stream.ReceiveNext();
		this.currentHuntedArray[9] = (int)stream.ReceiveNext();
		this.currentTargetArray[0] = (int)stream.ReceiveNext();
		this.currentTargetArray[1] = (int)stream.ReceiveNext();
		this.currentTargetArray[2] = (int)stream.ReceiveNext();
		this.currentTargetArray[3] = (int)stream.ReceiveNext();
		this.currentTargetArray[4] = (int)stream.ReceiveNext();
		this.currentTargetArray[5] = (int)stream.ReceiveNext();
		this.currentTargetArray[6] = (int)stream.ReceiveNext();
		this.currentTargetArray[7] = (int)stream.ReceiveNext();
		this.currentTargetArray[8] = (int)stream.ReceiveNext();
		this.currentTargetArray[9] = (int)stream.ReceiveNext();
		this.huntStarted = (bool)stream.ReceiveNext();
		this.waitingToStartNextHuntGame = (bool)stream.ReceiveNext();
		this.countDownTime = (int)stream.ReceiveNext();
		this.CopyHuntDataArrayToList();
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x00135F8C File Offset: 0x0013418C
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		NetPlayer targetOf = this.GetTargetOf(forPlayer);
		if (this.currentHunted.Contains(forPlayer) || (this.huntStarted && targetOf == null))
		{
			return 3;
		}
		return 0;
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x00135FC0 File Offset: 0x001341C0
	public override float[] LocalPlayerSpeed()
	{
		if (this.currentHunted.Contains(NetworkSystem.Instance.LocalPlayer) || (this.huntStarted && this.GetTargetOf(NetworkSystem.Instance.LocalPlayer) == null))
		{
			return new float[] { 8.5f, 1.3f };
		}
		if (GorillaTagger.Instance.currentStatus == GorillaTagger.StatusEffect.Slowed)
		{
			return new float[] { 5.5f, 0.9f };
		}
		return new float[] { 6.5f, 1.1f };
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x0013604F File Offset: 0x0013424F
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
	}

	// Token: 0x040048BB RID: 18619
	public float tagCoolDown = 5f;

	// Token: 0x040048BC RID: 18620
	public int[] currentHuntedArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x040048BD RID: 18621
	public List<NetPlayer> currentHunted = new List<NetPlayer>(10);

	// Token: 0x040048BE RID: 18622
	public int[] currentTargetArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x040048BF RID: 18623
	public List<NetPlayer> currentTarget = new List<NetPlayer>(10);

	// Token: 0x040048C0 RID: 18624
	public bool huntStarted;

	// Token: 0x040048C1 RID: 18625
	public bool waitingToStartNextHuntGame;

	// Token: 0x040048C2 RID: 18626
	public bool inStartCountdown;

	// Token: 0x040048C3 RID: 18627
	public int countDownTime;

	// Token: 0x040048C4 RID: 18628
	public double timeHuntGameEnded;

	// Token: 0x040048C5 RID: 18629
	public float timeLastSlowTagged;

	// Token: 0x040048C6 RID: 18630
	public object objRef;

	// Token: 0x040048C7 RID: 18631
	private int iterator1;

	// Token: 0x040048C8 RID: 18632
	private NetPlayer tempRandPlayer;

	// Token: 0x040048C9 RID: 18633
	private int tempRandIndex;

	// Token: 0x040048CA RID: 18634
	private int notHuntedCount;

	// Token: 0x040048CB RID: 18635
	private int tempTargetIndex;

	// Token: 0x040048CC RID: 18636
	private NetPlayer tempPlayer;

	// Token: 0x040048CD RID: 18637
	private int copyListToArrayIndex;

	// Token: 0x040048CE RID: 18638
	private int copyArrayToListIndex;
}
