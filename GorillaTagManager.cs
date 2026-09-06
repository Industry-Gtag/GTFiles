using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using GorillaGameModes;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x020008D3 RID: 2259
public class GorillaTagManager : GorillaGameManager
{
	// Token: 0x06003B2A RID: 15146 RVA: 0x00142618 File Offset: 0x00140818
	public override void Awake()
	{
		base.Awake();
		this.currentInfectedArray = new int[20];
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
	}

	// Token: 0x06003B2B RID: 15147 RVA: 0x00142654 File Offset: 0x00140854
	public override void StartPlaying()
	{
		base.StartPlaying();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			for (int i = 0; i < this.currentInfected.Count; i++)
			{
				this.tempPlayer = this.currentInfected[i];
				if (this.tempPlayer == null || !this.tempPlayer.InRoom())
				{
					this.currentInfected.RemoveAt(i);
					i--;
				}
			}
			if (this.currentIt != null && !this.currentIt.InRoom())
			{
				this.currentIt = null;
			}
			if (this.lastInfectedPlayer != null && !this.lastInfectedPlayer.InRoom())
			{
				this.lastInfectedPlayer = null;
			}
			this.UpdateState();
		}
	}

	// Token: 0x06003B2C RID: 15148 RVA: 0x00142701 File Offset: 0x00140901
	public override void StopPlaying()
	{
		base.StopPlaying();
		base.StopAllCoroutines();
		this.lastTaggedActorNr.Clear();
	}

	// Token: 0x06003B2D RID: 15149 RVA: 0x0014271C File Offset: 0x0014091C
	public override void ResetGame()
	{
		base.ResetGame();
		for (int i = 0; i < this.currentInfectedArray.Length; i++)
		{
			this.currentInfectedArray[i] = -1;
		}
		this.currentInfected.Clear();
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.allInfected = false;
		this.isCurrentlyTag = false;
		this.waitingToStartNextInfectionGame = false;
		this.currentIt = null;
		this.lastInfectedPlayer = null;
	}

	// Token: 0x06003B2E RID: 15150 RVA: 0x00142798 File Offset: 0x00140998
	public virtual void UpdateState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 1)
			{
				this.isCurrentlyTag = true;
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.currentIt = null;
				return;
			}
			if (this.isCurrentlyTag && this.currentIt == null)
			{
				int num = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num], false);
				return;
			}
			if (this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.SetisCurrentlyTag(false);
				this.ClearInfectionState();
				int num2 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(global::GorillaGameModes.GameMode.ParticipatingPlayers[num2], true);
				this.lastInfectedPlayer = global::GorillaGameModes.GameMode.ParticipatingPlayers[num2];
				return;
			}
			if (!this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
			{
				this.ClearInfectionState();
				this.lastInfectedPlayer = null;
				this.SetisCurrentlyTag(true);
				int num3 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num3], false);
				return;
			}
			if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
			{
				int num4 = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
				this.AddInfectedPlayer(global::GorillaGameModes.GameMode.ParticipatingPlayers[num4], true);
				return;
			}
			if (!this.isCurrentlyTag)
			{
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06003B2F RID: 15151 RVA: 0x00142906 File Offset: 0x00140B06
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateState();
		}
		this.inspectorLocalPlayerSpeed = this.LocalPlayerSpeed();
	}

	// Token: 0x06003B30 RID: 15152 RVA: 0x0014292C File Offset: 0x00140B2C
	protected virtual IEnumerator InfectionRoundEndingCoroutine()
	{
		while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
		{
			this.InfectionRoundStart();
		}
		yield return null;
		yield break;
	}

	// Token: 0x06003B31 RID: 15153 RVA: 0x0014293C File Offset: 0x00140B3C
	protected virtual void InfectionRoundStart()
	{
		this.ClearInfectionState();
		global::GorillaGameModes.GameMode.RefreshPlayers();
		List<NetPlayer> participatingPlayers = global::GorillaGameModes.GameMode.ParticipatingPlayers;
		if (participatingPlayers.Count > 0)
		{
			int num = Random.Range(0, participatingPlayers.Count);
			int num2 = 0;
			while (num2 < 10 && participatingPlayers[num] == this.lastInfectedPlayer)
			{
				num = Random.Range(0, participatingPlayers.Count);
				num2++;
			}
			this.AddInfectedPlayer(participatingPlayers[num], true);
			this.lastInfectedPlayer = participatingPlayers[num];
			this.lastTag = (double)Time.time;
		}
	}

	// Token: 0x06003B32 RID: 15154 RVA: 0x001429C0 File Offset: 0x00140BC0
	public virtual void UpdateInfectionState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		this.allInfected = true;
		foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (!this.currentInfected.Contains(netPlayer))
			{
				this.allInfected = false;
				break;
			}
		}
		if (!this.isCurrentlyTag && !this.waitingToStartNextInfectionGame && this.allInfected)
		{
			this.InfectionRoundEnd();
		}
	}

	// Token: 0x06003B33 RID: 15155 RVA: 0x00142A54 File Offset: 0x00140C54
	public void UpdateTagState(bool withTagFreeze = true)
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
		{
			if (this.currentIt == netPlayer)
			{
				if (withTagFreeze)
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, netPlayer);
				}
				else
				{
					RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, netPlayer);
				}
				RoomSystem.SendSoundEffectOnOther(0, 0.25f, netPlayer, false);
				break;
			}
		}
	}

	// Token: 0x06003B34 RID: 15156 RVA: 0x00142AD8 File Offset: 0x00140CD8
	protected virtual void InfectionRoundEnd()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			foreach (NetPlayer netPlayer in global::GorillaGameModes.GameMode.ParticipatingPlayers)
			{
				RoomSystem.SendSoundEffectToPlayer(2, 0.25f, netPlayer, true);
			}
			PlayerGameEvents.GameModeCompleteRound();
			global::GorillaGameModes.GameMode.BroadcastRoundComplete();
			this.lastTaggedActorNr.Clear();
			this.waitingToStartNextInfectionGame = true;
			this.timeInfectedGameEnded = (double)Time.time;
			base.StartCoroutine(this.InfectionRoundEndingCoroutine());
		}
	}

	// Token: 0x06003B35 RID: 15157 RVA: 0x00142B74 File Offset: 0x00140D74
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		if (this.isCurrentlyTag)
		{
			return myPlayer == this.currentIt && myPlayer != otherPlayer;
		}
		return this.currentInfected.Contains(myPlayer) && !this.currentInfected.Contains(otherPlayer);
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x00142BB0 File Offset: 0x00140DB0
	public override bool LocalIsTagged(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x00142BD0 File Offset: 0x00140DD0
	public override void LocalTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool bodyHit, bool leftHand)
	{
		if (this.LocalCanTag(NetworkSystem.Instance.LocalPlayer, taggedPlayer) && (double)Time.time > this.lastQuestTagTime + (double)this.tagCoolDown)
		{
			PlayerGameEvents.MiscEvent("GameModeTag", 1);
			this.lastQuestTagTime = (double)Time.time;
			if (!this.isCurrentlyTag)
			{
				PlayerGameEvents.GameModeObjectiveTriggered();
			}
		}
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x00142C2C File Offset: 0x00140E2C
	protected float InterpolatedInfectedJumpMultiplier(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpMultiplier;
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x00142C80 File Offset: 0x00140E80
	protected float InterpolatedInfectedJumpSpeed(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.fastJumpLimit;
		}
		return (this.fastJumpLimit - this.slowJumpLimit) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - infectedCount) + this.slowJumpLimit;
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x00142CD4 File Offset: 0x00140ED4
	protected float InterpolatedNoobJumpMultiplier(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpMultiplier;
		}
		return (this.fastJumpMultiplier - this.slowJumpMultiplier) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpMultiplier;
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x00142D24 File Offset: 0x00140F24
	protected float InterpolatedNoobJumpSpeed(int infectedCount)
	{
		if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count < 2)
		{
			return this.slowJumpLimit;
		}
		return (this.fastJumpLimit - this.fastJumpLimit) / (float)(global::GorillaGameModes.GameMode.ParticipatingPlayers.Count - 1) * (float)(infectedCount - 1) * 0.9f + this.slowJumpLimit;
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x00142D74 File Offset: 0x00140F74
	public override void ReportTag(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggingRig = this.FindPlayerVRRig(taggingPlayer);
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggingRig == null || this.taggedRig == null)
			{
				return;
			}
			this.taggedRig.SetTaggedBy(this.taggingRig);
			if (this.isCurrentlyTag)
			{
				if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.ChangeCurrentIt(taggedPlayer, true);
					this.lastTag = (double)Time.time;
					this.HandleTagBroadcast(taggedPlayer, taggingPlayer);
					global::GorillaGameModes.GameMode.BroadcastTag(taggedPlayer, taggingPlayer);
					return;
				}
			}
			else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
			{
				if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
				{
					MonkeAgent.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
					return;
				}
				this.HandleTagBroadcast(taggedPlayer, taggingPlayer);
				global::GorillaGameModes.GameMode.BroadcastTag(taggedPlayer, taggingPlayer);
				base.AddLastTagged(taggedPlayer, taggingPlayer);
				this.AddInfectedPlayer(taggedPlayer, true);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06003B3D RID: 15165 RVA: 0x00142EFC File Offset: 0x001410FC
	public override void HitPlayer(NetPlayer taggedPlayer)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.taggedRig = this.FindPlayerVRRig(taggedPlayer);
			if (this.taggedRig == null || this.waitingToStartNextInfectionGame || (double)Time.time < this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown))
			{
				return;
			}
			if (this.isCurrentlyTag)
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.ChangeCurrentIt(taggedPlayer, false);
				return;
			}
			if (!this.currentInfected.Contains(taggedPlayer))
			{
				base.AddLastTagged(taggedPlayer, taggedPlayer);
				this.AddInfectedPlayer(taggedPlayer, false);
				int count = this.currentInfected.Count;
			}
		}
	}

	// Token: 0x06003B3E RID: 15166 RVA: 0x00142F9C File Offset: 0x0014119C
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt != player && thisFrame;
		}
		return !this.waitingToStartNextInfectionGame && (double)Time.time >= this.timeInfectedGameEnded + (double)(2f * this.tagCoolDown) && !this.currentInfected.Contains(player);
	}

	// Token: 0x06003B3F RID: 15167 RVA: 0x00142BB0 File Offset: 0x00140DB0
	public bool IsInfected(NetPlayer player)
	{
		if (this.isCurrentlyTag)
		{
			return this.currentIt == player;
		}
		return this.currentInfected.Contains(player);
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x00142FF5 File Offset: 0x001411F5
	public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
	{
		base.NewVRRig(player, vrrigPhotonViewID, didTutorial);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			bool flag = this.isCurrentlyTag;
			this.UpdateState();
			if (!flag && !this.isCurrentlyTag)
			{
				if (didTutorial)
				{
					this.AddInfectedPlayer(player, false);
				}
				this.UpdateInfectionState();
			}
		}
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x00143034 File Offset: 0x00141234
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			while (this.currentInfected.Contains(otherPlayer))
			{
				this.currentInfected.Remove(otherPlayer);
			}
			if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber))
			{
				if (global::GorillaGameModes.GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, global::GorillaGameModes.GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(global::GorillaGameModes.GameMode.ParticipatingPlayers[num], false);
				}
			}
			else if (!this.isCurrentlyTag && global::GorillaGameModes.GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
			{
				this.UpdateInfectionState();
			}
			this.UpdateState();
		}
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x001430F4 File Offset: 0x001412F4
	private void CopyInfectedListToArray()
	{
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			this.currentInfectedArray[this.iterator1] = -1;
			this.iterator1++;
		}
		this.iterator1 = this.currentInfected.Count - 1;
		while (this.iterator1 >= 0)
		{
			if (this.currentInfected[this.iterator1] == null)
			{
				this.currentInfected.RemoveAt(this.iterator1);
			}
			this.iterator1--;
		}
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfected.Count)
		{
			this.currentInfectedArray[this.iterator1] = this.currentInfected[this.iterator1].ActorNumber;
			this.iterator1++;
		}
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x001431D4 File Offset: 0x001413D4
	private void CopyInfectedArrayToList()
	{
		this.currentInfected.Clear();
		this.iterator1 = 0;
		while (this.iterator1 < this.currentInfectedArray.Length)
		{
			if (this.currentInfectedArray[this.iterator1] != -1)
			{
				this.tempPlayer = NetworkSystem.Instance.GetPlayer(this.currentInfectedArray[this.iterator1]);
				if (this.tempPlayer != null)
				{
					this.currentInfected.Add(this.tempPlayer);
				}
			}
			this.iterator1++;
		}
	}

	// Token: 0x06003B44 RID: 15172 RVA: 0x00143259 File Offset: 0x00141459
	protected virtual void ChangeCurrentIt(NetPlayer newCurrentIt, bool withTagFreeze = true)
	{
		this.lastTag = (double)Time.time;
		this.currentIt = newCurrentIt;
		this.UpdateTagState(withTagFreeze);
	}

	// Token: 0x06003B45 RID: 15173 RVA: 0x00143275 File Offset: 0x00141475
	public void SetisCurrentlyTag(bool newTagSetting)
	{
		if (newTagSetting)
		{
			this.isCurrentlyTag = true;
		}
		else
		{
			this.isCurrentlyTag = false;
		}
		RoomSystem.SendSoundEffectAll(2, 0.25f, false);
	}

	// Token: 0x06003B46 RID: 15174 RVA: 0x00143296 File Offset: 0x00141496
	public virtual void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.currentInfected.Add(infectedPlayer);
			if (!withTagStop)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.JoinedTaggedTime, infectedPlayer);
			}
			else
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, infectedPlayer);
			}
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, infectedPlayer, false);
			this.UpdateInfectionState();
		}
	}

	// Token: 0x06003B47 RID: 15175 RVA: 0x001432D6 File Offset: 0x001414D6
	public void ClearInfectionState()
	{
		this.currentInfected.Clear();
		this.waitingToStartNextInfectionGame = false;
	}

	// Token: 0x06003B48 RID: 15176 RVA: 0x001432EA File Offset: 0x001414EA
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		base.OnMasterClientSwitched(newMasterClient);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.CopyRoomDataToLocalData();
			this.UpdateState();
		}
	}

	// Token: 0x06003B49 RID: 15177 RVA: 0x0014330B File Offset: 0x0014150B
	public void CopyRoomDataToLocalData()
	{
		this.lastTag = 0.0;
		this.timeInfectedGameEnded = 0.0;
		this.waitingToStartNextInfectionGame = false;
		if (this.isCurrentlyTag)
		{
			this.UpdateTagState(true);
			return;
		}
		this.UpdateInfectionState();
	}

	// Token: 0x06003B4A RID: 15178 RVA: 0x00143348 File Offset: 0x00141548
	public override void OnSerializeRead(object newData)
	{
		TagData tagData = (TagData)newData;
		this.isCurrentlyTag = tagData.isCurrentlyTag;
		this.tempItInt = tagData.currentItID;
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		tagData.infectedPlayerList.CopyTo(this.currentInfectedArray, true);
		this.CopyInfectedArrayToList();
	}

	// Token: 0x06003B4B RID: 15179 RVA: 0x001433B8 File Offset: 0x001415B8
	public override object OnSerializeWrite()
	{
		this.CopyInfectedListToArray();
		TagData tagData = default(TagData);
		tagData.isCurrentlyTag = this.isCurrentlyTag;
		tagData.currentItID = ((this.currentIt != null) ? this.currentIt.ActorNumber : (-1));
		tagData.infectedPlayerList.CopyFrom(this.currentInfectedArray, 0, this.currentInfectedArray.Length);
		return tagData;
	}

	// Token: 0x06003B4C RID: 15180 RVA: 0x00143428 File Offset: 0x00141628
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyInfectedListToArray();
		stream.SendNext(this.isCurrentlyTag);
		stream.SendNext((this.currentIt != null) ? this.currentIt.ActorNumber : (-1));
		stream.SendNext(this.currentInfectedArray[0]);
		stream.SendNext(this.currentInfectedArray[1]);
		stream.SendNext(this.currentInfectedArray[2]);
		stream.SendNext(this.currentInfectedArray[3]);
		stream.SendNext(this.currentInfectedArray[4]);
		stream.SendNext(this.currentInfectedArray[5]);
		stream.SendNext(this.currentInfectedArray[6]);
		stream.SendNext(this.currentInfectedArray[7]);
		stream.SendNext(this.currentInfectedArray[8]);
		stream.SendNext(this.currentInfectedArray[9]);
		stream.SendNext(this.currentInfectedArray[10]);
		stream.SendNext(this.currentInfectedArray[11]);
		stream.SendNext(this.currentInfectedArray[12]);
		stream.SendNext(this.currentInfectedArray[13]);
		stream.SendNext(this.currentInfectedArray[14]);
		stream.SendNext(this.currentInfectedArray[15]);
		stream.SendNext(this.currentInfectedArray[16]);
		stream.SendNext(this.currentInfectedArray[17]);
		stream.SendNext(this.currentInfectedArray[18]);
		stream.SendNext(this.currentInfectedArray[19]);
		base.WriteLastTagged(stream);
	}

	// Token: 0x06003B4D RID: 15181 RVA: 0x001435FC File Offset: 0x001417FC
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		bool flag = this.currentIt == NetworkSystem.Instance.LocalPlayer;
		bool flag2 = this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer);
		this.isCurrentlyTag = (bool)stream.ReceiveNext();
		this.tempItInt = (int)stream.ReceiveNext();
		this.currentIt = ((this.tempItInt != -1) ? NetworkSystem.Instance.GetPlayer(this.tempItInt) : null);
		this.currentInfectedArray[0] = (int)stream.ReceiveNext();
		this.currentInfectedArray[1] = (int)stream.ReceiveNext();
		this.currentInfectedArray[2] = (int)stream.ReceiveNext();
		this.currentInfectedArray[3] = (int)stream.ReceiveNext();
		this.currentInfectedArray[4] = (int)stream.ReceiveNext();
		this.currentInfectedArray[5] = (int)stream.ReceiveNext();
		this.currentInfectedArray[6] = (int)stream.ReceiveNext();
		this.currentInfectedArray[7] = (int)stream.ReceiveNext();
		this.currentInfectedArray[8] = (int)stream.ReceiveNext();
		this.currentInfectedArray[9] = (int)stream.ReceiveNext();
		this.currentInfectedArray[10] = (int)stream.ReceiveNext();
		this.currentInfectedArray[11] = (int)stream.ReceiveNext();
		this.currentInfectedArray[12] = (int)stream.ReceiveNext();
		this.currentInfectedArray[13] = (int)stream.ReceiveNext();
		this.currentInfectedArray[14] = (int)stream.ReceiveNext();
		this.currentInfectedArray[15] = (int)stream.ReceiveNext();
		this.currentInfectedArray[16] = (int)stream.ReceiveNext();
		this.currentInfectedArray[17] = (int)stream.ReceiveNext();
		this.currentInfectedArray[18] = (int)stream.ReceiveNext();
		this.currentInfectedArray[19] = (int)stream.ReceiveNext();
		base.ReadLastTagged(stream);
		this.CopyInfectedArrayToList();
		if (this.isCurrentlyTag)
		{
			if (!flag && this.currentIt == NetworkSystem.Instance.LocalPlayer)
			{
				this.lastQuestTagTime = (double)Time.time;
				return;
			}
		}
		else if (!flag2 && this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
		{
			this.lastQuestTagTime = (double)Time.time;
		}
	}

	// Token: 0x06003B4E RID: 15182 RVA: 0x00023F0C File Offset: 0x0002210C
	public override GameModeType GameType()
	{
		return GameModeType.Infection;
	}

	// Token: 0x06003B4F RID: 15183 RVA: 0x0014386B File Offset: 0x00141A6B
	public override string GameModeName()
	{
		return "INFECTION";
	}

	// Token: 0x06003B50 RID: 15184 RVA: 0x00143874 File Offset: 0x00141A74
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_INFECTION_ROOM_LABEL", out text, "(INFECTION GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_INFECTION_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x06003B51 RID: 15185 RVA: 0x0014389F File Offset: 0x00141A9F
	public override void AddFusionDataBehaviour(NetworkObject netObject)
	{
		netObject.AddBehaviour<TagGameModeData>();
	}

	// Token: 0x06003B52 RID: 15186 RVA: 0x001438A8 File Offset: 0x00141AA8
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		if (this.isCurrentlyTag && forPlayer == this.currentIt)
		{
			return 1;
		}
		if (this.currentInfected.Contains(forPlayer))
		{
			return 2;
		}
		return 0;
	}

	// Token: 0x06003B53 RID: 15187 RVA: 0x001438D0 File Offset: 0x00141AD0
	public override float[] LocalPlayerSpeed()
	{
		if (this.isCurrentlyTag)
		{
			if (NetworkSystem.Instance.LocalPlayer == this.currentIt)
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.slowJumpLimit;
			this.playerSpeed[1] = this.slowJumpMultiplier;
			return this.playerSpeed;
		}
		else
		{
			if (this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer))
			{
				this.playerSpeed[0] = this.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = this.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
			this.playerSpeed[0] = this.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
			this.playerSpeed[1] = this.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
			return this.playerSpeed;
		}
	}

	// Token: 0x04004B8B RID: 19339
	public new const int k_defaultMatIndex = 0;

	// Token: 0x04004B8C RID: 19340
	public const int k_itMatIndex = 1;

	// Token: 0x04004B8D RID: 19341
	public const int k_infectedMatIndex = 2;

	// Token: 0x04004B8E RID: 19342
	public float tagCoolDown = 5f;

	// Token: 0x04004B8F RID: 19343
	public int infectedModeThreshold = 4;

	// Token: 0x04004B90 RID: 19344
	public const byte ReportTagEvent = 1;

	// Token: 0x04004B91 RID: 19345
	public const byte ReportInfectionTagEvent = 2;

	// Token: 0x04004B92 RID: 19346
	[NonSerialized]
	public List<NetPlayer> currentInfected = new List<NetPlayer>(20);

	// Token: 0x04004B93 RID: 19347
	[NonSerialized]
	public int[] currentInfectedArray;

	// Token: 0x04004B94 RID: 19348
	[NonSerialized]
	public NetPlayer currentIt;

	// Token: 0x04004B95 RID: 19349
	[NonSerialized]
	public NetPlayer lastInfectedPlayer;

	// Token: 0x04004B96 RID: 19350
	public double lastTag;

	// Token: 0x04004B97 RID: 19351
	public double timeInfectedGameEnded;

	// Token: 0x04004B98 RID: 19352
	public bool waitingToStartNextInfectionGame;

	// Token: 0x04004B99 RID: 19353
	public bool isCurrentlyTag;

	// Token: 0x04004B9A RID: 19354
	private int tempItInt;

	// Token: 0x04004B9B RID: 19355
	private int iterator1;

	// Token: 0x04004B9C RID: 19356
	private NetPlayer tempPlayer;

	// Token: 0x04004B9D RID: 19357
	private bool allInfected;

	// Token: 0x04004B9E RID: 19358
	public float[] inspectorLocalPlayerSpeed;

	// Token: 0x04004B9F RID: 19359
	private protected VRRig taggingRig;

	// Token: 0x04004BA0 RID: 19360
	private protected VRRig taggedRig;

	// Token: 0x04004BA1 RID: 19361
	private NetPlayer lastTaggedPlayer;

	// Token: 0x04004BA2 RID: 19362
	private double lastQuestTagTime;
}
