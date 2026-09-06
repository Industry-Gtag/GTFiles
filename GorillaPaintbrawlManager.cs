using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaGameModes;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200089B RID: 2203
public sealed class GorillaPaintbrawlManager : GorillaGameManager
{
	// Token: 0x06003964 RID: 14692 RVA: 0x00138C23 File Offset: 0x00136E23
	private void ActivatePaintbrawlBalloons(bool enable)
	{
		if (GorillaTagger.Instance.offlineVRRig != null)
		{
			GorillaTagger.Instance.offlineVRRig.paintbrawlBalloons.gameObject.SetActive(enable);
		}
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x00138C51 File Offset: 0x00136E51
	private bool HasFlag(GorillaPaintbrawlManager.PaintbrawlStatus state, GorillaPaintbrawlManager.PaintbrawlStatus statusFlag)
	{
		return (state & statusFlag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x00138C59 File Offset: 0x00136E59
	public override GameModeType GameType()
	{
		return GameModeType.Paintbrawl;
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x00138C5C File Offset: 0x00136E5C
	public override void AddFusionDataBehaviour(NetworkObject behaviour)
	{
		behaviour.AddBehaviour<BattleGameModeData>();
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x00138C65 File Offset: 0x00136E65
	public override string GameModeName()
	{
		return "PAINTBRAWL";
	}

	// Token: 0x06003969 RID: 14697 RVA: 0x00138C6C File Offset: 0x00136E6C
	public override string GameModeNameRoomLabel()
	{
		string text;
		if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_PAINTBRAWL_ROOM_LABEL", out text, "(PAINTBRAWL GAME)"))
		{
			Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_PAINTBRAWL_ROOM_LABEL]");
		}
		return text;
	}

	// Token: 0x0600396A RID: 14698 RVA: 0x00138C98 File Offset: 0x00136E98
	private void ActivateDefaultSlingShot()
	{
		if (this._isDefaultSlingshotSynced && !Slingshot.IsSlingShotEnabled())
		{
			this._isDefaultSlingshotSynced = false;
		}
		if (this._isDefaultSlingshotSynced)
		{
			return;
		}
		Object offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		bool flag = Slingshot.IsSlingShotEnabled();
		if (offlineVRRig != null && !flag)
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
			instance.currentWornSet.HasItem("Slingshot");
			instance.ApplyCosmeticItemToSet(instance.currentWornSet, itemFromDict, true, false);
			instance.UpdateWornCosmetics(true);
			bool flag2 = instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
			instance.currentWornSet.HasItem("Slingshot");
			this._isDefaultSlingshotSynced = flag2;
		}
	}

	// Token: 0x0600396B RID: 14699 RVA: 0x00138D4C File Offset: 0x00136F4C
	private void PreloadSlingshotForActiveRigs(string caller)
	{
		int count = CosmeticsV2Spawner_Dirty._gVRRigDatas.Count;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			CosmeticsV2Spawner_Dirty.VRRigData vrrigData = CosmeticsV2Spawner_Dirty._gVRRigDatas[i];
			if (!(vrrigData.vrRig == null) && !this._slingshotPreloadedRigs.Contains(vrrigData.vrRig))
			{
				CosmeticItemRegistry cosmeticsObjectRegistry = vrrigData.vrRig.cosmeticsObjectRegistry;
				if (cosmeticsObjectRegistry != null)
				{
					CosmeticsV2Spawner_Dirty.ProcessLoadOpInfos(vrrigData.vrRig, "Slingshot", cosmeticsObjectRegistry);
					this._slingshotPreloadedRigs.Add(vrrigData.vrRig);
					num++;
				}
			}
		}
	}

	// Token: 0x0600396C RID: 14700 RVA: 0x00138DD8 File Offset: 0x00136FD8
	public override void Awake()
	{
		base.Awake();
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x0600396D RID: 14701 RVA: 0x00138DF0 File Offset: 0x00136FF0
	public override void StartPlaying()
	{
		base.StartPlaying();
		this._isDefaultSlingshotSynced = false;
		this._slingshotPreloadedRigs.Clear();
		this.PreloadSlingshotForActiveRigs("StartPlaying");
		this.ActivatePaintbrawlBalloons(true);
		this.VerifyPlayersInDict<int>(this.playerLives);
		this.VerifyPlayersInDict<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		this.VerifyPlayersInDict<float>(this.playerHitTimes);
		this.VerifyPlayersInDict<float>(this.playerStunTimes);
		this.CopyBattleDictToArray();
		this.UpdateBattleState();
	}

	// Token: 0x0600396E RID: 14702 RVA: 0x00138E64 File Offset: 0x00137064
	public override void StopPlaying()
	{
		base.StopPlaying();
		this._isDefaultSlingshotSynced = false;
		PlayerPrefs.GetString("slot_Chest", "NOTHING");
		if (Slingshot.IsSlingShotEnabled())
		{
			CosmeticsController instance = CosmeticsController.instance;
			CosmeticsController.CosmeticItem itemFromDict = instance.GetItemFromDict("Slingshot");
			if (instance.currentWornSet.HasItem("Slingshot"))
			{
				instance.ApplyCosmeticItemToSet(instance.currentWornSet, itemFromDict, true, false);
				instance.UpdateWornCosmetics(true);
				instance.currentWornSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
				PlayerPrefs.GetString("slot_Chest", "NOTHING");
			}
		}
		this.ActivatePaintbrawlBalloons(false);
		base.StopAllCoroutines();
		this.coroutineRunning = false;
	}

	// Token: 0x0600396F RID: 14703 RVA: 0x00138F04 File Offset: 0x00137104
	public override void ResetGame()
	{
		base.ResetGame();
		this.playerLives.Clear();
		this.playerStatusDict.Clear();
		this.playerHitTimes.Clear();
		this.playerStunTimes.Clear();
		for (int i = 0; i < this.playerActorNumberArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
			this.playerStatusArray[i] = GorillaPaintbrawlManager.PaintbrawlStatus.None;
		}
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers;
	}

	// Token: 0x06003970 RID: 14704 RVA: 0x00138F78 File Offset: 0x00137178
	private int CopyDictKeysToBuffer<T>(Dictionary<int, T> dict)
	{
		int num = 0;
		foreach (KeyValuePair<int, T> keyValuePair in dict)
		{
			if (num >= this.reusableKeyBuffer.Length)
			{
				break;
			}
			this.reusableKeyBuffer[num++] = keyValuePair.Key;
		}
		return num;
	}

	// Token: 0x06003971 RID: 14705 RVA: 0x00138FE4 File Offset: 0x001371E4
	private void VerifyPlayersInDict<T>(Dictionary<int, T> dict)
	{
		if (dict.Count < 1)
		{
			return;
		}
		int num = this.CopyDictKeysToBuffer<T>(dict);
		for (int i = 0; i < num; i++)
		{
			if (!Utils.PlayerInRoom(this.reusableKeyBuffer[i]))
			{
				dict.Remove(this.reusableKeyBuffer[i]);
			}
		}
	}

	// Token: 0x06003972 RID: 14706 RVA: 0x0013902D File Offset: 0x0013722D
	internal override void NetworkLinkSetup(GameModeSerializer netSerializer)
	{
		base.NetworkLinkSetup(netSerializer);
		netSerializer.AddRPCComponent<PaintbrawlRPCs>();
	}

	// Token: 0x06003973 RID: 14707 RVA: 0x0013903D File Offset: 0x0013723D
	private void Transition(GorillaPaintbrawlManager.PaintbrawlState newState)
	{
		this.currentState = newState;
		Debug.Log("current state is: " + this.currentState.ToString());
	}

	// Token: 0x06003974 RID: 14708 RVA: 0x00139068 File Offset: 0x00137268
	public void UpdateBattleState()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			switch (this.currentState)
			{
			case GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers:
				if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEnd:
				if (this.EndBattleGame())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameEndWaiting:
				if (this.BattleEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.StartCountdown:
				if (this.teamBattle)
				{
					this.RandomizeTeams();
				}
				base.StartCoroutine(this.StartBattleCountdown());
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.CountingDownToStart:
				if (!this.coroutineRunning)
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.StartCountdown);
				}
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameStart:
				this.StartBattle();
				this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameRunning);
				break;
			case GorillaPaintbrawlManager.PaintbrawlState.GameRunning:
				if (this.CheckForGameEnd())
				{
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.GameEnd);
					PlayerGameEvents.GameModeCompleteRound();
					global::GorillaGameModes.GameMode.BroadcastRoundComplete();
				}
				if ((float)RoomSystem.PlayersInRoom.Count < this.playerMin)
				{
					this.InitializePlayerStatus();
					this.Transition(GorillaPaintbrawlManager.PaintbrawlState.NotEnoughPlayers);
				}
				break;
			}
			this.UpdatePlayerStatus();
		}
	}

	// Token: 0x06003975 RID: 14709 RVA: 0x0013917C File Offset: 0x0013737C
	private bool CheckForGameEnd()
	{
		int num = 0;
		this.bcount = 0;
		this.rcount = 0;
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			if (this.playerLives.TryGetValue(netPlayer.ActorNumber, out this.lives))
			{
				if (this.lives > 0)
				{
					num++;
					if (this.teamBattle && this.playerStatusDict.TryGetValue(netPlayer.ActorNumber, out this.tempStatus))
					{
						if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam))
						{
							this.rcount++;
						}
						else if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam))
						{
							this.bcount++;
						}
					}
				}
			}
			else
			{
				this.playerLives.Add(netPlayer.ActorNumber, 0);
			}
		}
		return (this.teamBattle && (this.bcount == 0 || this.rcount == 0)) || (!this.teamBattle && num <= 1);
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x001392A0 File Offset: 0x001374A0
	public IEnumerator StartBattleCountdown()
	{
		this.coroutineRunning = true;
		this.countDownTime = 5;
		while (this.countDownTime > 0)
		{
			try
			{
				RoomSystem.SendSoundEffectAll(6, 0.25f, false);
				foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
				{
					this.playerLives[netPlayer.ActorNumber] = 3;
				}
			}
			catch
			{
			}
			yield return new WaitForSeconds(1f);
			this.countDownTime--;
		}
		this.coroutineRunning = false;
		this.currentState = GorillaPaintbrawlManager.PaintbrawlState.GameStart;
		yield return null;
		yield break;
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x001392B0 File Offset: 0x001374B0
	public void StartBattle()
	{
		RoomSystem.SendSoundEffectAll(7, 0.5f, false);
		foreach (NetPlayer netPlayer in RoomSystem.PlayersInRoom)
		{
			this.playerLives[netPlayer.ActorNumber] = 3;
		}
	}

	// Token: 0x06003978 RID: 14712 RVA: 0x0013931C File Offset: 0x0013751C
	private bool EndBattleGame()
	{
		if ((float)RoomSystem.PlayersInRoom.Count >= this.playerMin)
		{
			RoomSystem.SendStatusEffectAll(RoomSystem.StatusEffects.TaggedTime);
			RoomSystem.SendSoundEffectAll(2, 0.25f, false);
			this.timeBattleEnded = Time.time;
			return true;
		}
		return false;
	}

	// Token: 0x06003979 RID: 14713 RVA: 0x00139351 File Offset: 0x00137551
	public bool BattleEnd()
	{
		return Time.time > this.timeBattleEnded + this.tagCoolDown;
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x00139367 File Offset: 0x00137567
	public bool SlingshotHit(NetPlayer myPlayer, Player otherPlayer)
	{
		return this.playerLives.TryGetValue(otherPlayer.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x00139390 File Offset: 0x00137590
	public void ReportSlingshotHit(NetPlayer taggedPlayer, Vector3 hitLocation, int projectileCount, PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		if (this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.OnSameTeam(taggedPlayer, player))
		{
			return;
		}
		if (this.GetPlayerLives(taggedPlayer) > 0 && this.GetPlayerLives(player) > 0 && !this.PlayerInHitCooldown(taggedPlayer))
		{
			if (!this.playerHitTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
			{
				this.playerHitTimes.Add(taggedPlayer.ActorNumber, Time.time);
			}
			else
			{
				this.playerHitTimes[taggedPlayer.ActorNumber] = Time.time;
			}
			Dictionary<int, int> dictionary = this.playerLives;
			int actorNumber = taggedPlayer.ActorNumber;
			int num = dictionary[actorNumber];
			dictionary[actorNumber] = num - 1;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, taggedPlayer, false);
			return;
		}
		if (this.GetPlayerLives(player) == 0 && this.GetPlayerLives(taggedPlayer) > 0)
		{
			this.tempStatus = this.GetPlayerStatus(taggedPlayer);
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal) && !this.PlayerInHitCooldown(taggedPlayer) && !this.PlayerInStunCooldown(taggedPlayer))
			{
				if (!this.playerStunTimes.TryGetValue(taggedPlayer.ActorNumber, out this.outHitTime))
				{
					this.playerStunTimes.Add(taggedPlayer.ActorNumber, Time.time);
				}
				else
				{
					this.playerStunTimes[taggedPlayer.ActorNumber] = Time.time;
				}
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.SetSlowedTime, taggedPlayer);
				RoomSystem.SendSoundEffectOnOther(5, 0.125f, taggedPlayer, false);
				RigContainer rigContainer;
				if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer))
				{
					this.tempView = rigContainer.Rig.netView;
				}
			}
		}
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x0013952C File Offset: 0x0013772C
	public override void HitPlayer(NetPlayer player)
	{
		if (!NetworkSystem.Instance.IsMasterClient || this.currentState != GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
		{
			return;
		}
		if (this.GetPlayerLives(player) > 0)
		{
			this.playerLives[player.ActorNumber] = 0;
			RoomSystem.SendSoundEffectOnOther(0, 0.25f, player, false);
		}
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x00139578 File Offset: 0x00137778
	public override bool CanAffectPlayer(NetPlayer player, bool thisFrame)
	{
		return this.playerLives.TryGetValue(player.ActorNumber, out this.lives) && this.lives > 0;
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x001395A0 File Offset: 0x001377A0
	public override void OnPlayerEnteredRoom(NetPlayer newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			if (this.currentState == GorillaPaintbrawlManager.PaintbrawlState.GameRunning)
			{
				this.playerLives.Add(newPlayer.ActorNumber, 0);
			}
			else
			{
				this.playerLives.Add(newPlayer.ActorNumber, 3);
			}
			this.playerStatusDict.Add(newPlayer.ActorNumber, GorillaPaintbrawlManager.PaintbrawlStatus.None);
			this.CopyBattleDictToArray();
			if (this.teamBattle)
			{
				this.AddPlayerToCorrectTeam(newPlayer);
			}
		}
	}

	// Token: 0x0600397F RID: 14719 RVA: 0x00139618 File Offset: 0x00137818
	public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
	{
		base.OnPlayerLeftRoom(otherPlayer);
		if (this.playerLives.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerLives.Remove(otherPlayer.ActorNumber);
		}
		if (this.playerStatusDict.ContainsKey(otherPlayer.ActorNumber))
		{
			this.playerStatusDict.Remove(otherPlayer.ActorNumber);
		}
	}

	// Token: 0x06003980 RID: 14720 RVA: 0x00139678 File Offset: 0x00137878
	public override void OnSerializeRead(object newData)
	{
		PaintbrawlData paintbrawlData = (PaintbrawlData)newData;
		paintbrawlData.playerActorNumberArray.CopyTo(this.playerActorNumberArray, true);
		paintbrawlData.playerLivesArray.CopyTo(this.playerLivesArray, true);
		paintbrawlData.playerStatusArray.CopyTo(this.playerStatusArray, true);
		this.currentState = paintbrawlData.currentPaintbrawlState;
		this.CopyArrayToBattleDict();
	}

	// Token: 0x06003981 RID: 14721 RVA: 0x001396E0 File Offset: 0x001378E0
	public override object OnSerializeWrite()
	{
		this.CopyBattleDictToArray();
		PaintbrawlData paintbrawlData = default(PaintbrawlData);
		paintbrawlData.playerActorNumberArray.CopyFrom(this.playerActorNumberArray, 0, this.playerActorNumberArray.Length);
		paintbrawlData.playerLivesArray.CopyFrom(this.playerLivesArray, 0, this.playerLivesArray.Length);
		paintbrawlData.playerStatusArray.CopyFrom(this.playerStatusArray, 0, this.playerStatusArray.Length);
		paintbrawlData.currentPaintbrawlState = this.currentState;
		return paintbrawlData;
	}

	// Token: 0x06003982 RID: 14722 RVA: 0x00139768 File Offset: 0x00137968
	public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyBattleDictToArray();
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			stream.SendNext(this.playerActorNumberArray[i]);
			stream.SendNext(this.playerLivesArray[i]);
			stream.SendNext(this.playerStatusArray[i]);
		}
		stream.SendNext((int)this.currentState);
	}

	// Token: 0x06003983 RID: 14723 RVA: 0x001397D8 File Offset: 0x001379D8
	public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		NetworkSystem.Instance.GetPlayer(info.Sender);
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerActorNumberArray[i] = (int)stream.ReceiveNext();
			this.playerLivesArray[i] = (int)stream.ReceiveNext();
			this.playerStatusArray[i] = (GorillaPaintbrawlManager.PaintbrawlStatus)stream.ReceiveNext();
		}
		this.currentState = (GorillaPaintbrawlManager.PaintbrawlState)stream.ReceiveNext();
		this.CopyArrayToBattleDict();
	}

	// Token: 0x06003984 RID: 14724 RVA: 0x0013985C File Offset: 0x00137A5C
	public override int MyMatIndex(NetPlayer forPlayer)
	{
		this.tempStatus = this.GetPlayerStatus(forPlayer);
		if (this.tempStatus != GorillaPaintbrawlManager.PaintbrawlStatus.None)
		{
			if (this.OnRedTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 8;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 9;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 10;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 11;
				}
			}
			else if (this.OnBlueTeam(this.tempStatus))
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 4;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 5;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 6;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 7;
				}
			}
			else
			{
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
				{
					return 0;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Hit))
				{
					return 1;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Grace))
				{
					return 17;
				}
				if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
				{
					return 16;
				}
			}
		}
		return 0;
	}

	// Token: 0x06003985 RID: 14725 RVA: 0x001399B0 File Offset: 0x00137BB0
	public override float[] LocalPlayerSpeed()
	{
		if (this.playerStatusDict.TryGetValue(NetworkSystem.Instance.LocalPlayerID, out this.tempStatus))
		{
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Normal))
			{
				this.playerSpeed[0] = 6.5f;
				this.playerSpeed[1] = 1.1f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Stunned))
			{
				this.playerSpeed[0] = 2f;
				this.playerSpeed[1] = 0.5f;
				return this.playerSpeed;
			}
			if (this.HasFlag(this.tempStatus, GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated))
			{
				this.playerSpeed[0] = this.fastJumpLimit;
				this.playerSpeed[1] = this.fastJumpMultiplier;
				return this.playerSpeed;
			}
		}
		this.playerSpeed[0] = 6.5f;
		this.playerSpeed[1] = 1.1f;
		return this.playerSpeed;
	}

	// Token: 0x06003986 RID: 14726 RVA: 0x00139A91 File Offset: 0x00137C91
	public override void Tick()
	{
		base.Tick();
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.UpdateBattleState();
		}
		this.PreloadSlingshotForActiveRigs(null);
		this.ActivateDefaultSlingShot();
	}

	// Token: 0x06003987 RID: 14727 RVA: 0x00139AB8 File Offset: 0x00137CB8
	public override void InfrequentUpdate()
	{
		base.InfrequentUpdate();
		foreach (int num in this.playerLives.Keys)
		{
			this.playerInList = false;
			using (List<NetPlayer>.Enumerator enumerator2 = RoomSystem.PlayersInRoom.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.ActorNumber == num)
					{
						this.playerInList = true;
					}
				}
			}
			if (!this.playerInList)
			{
				this.playerLives.Remove(num);
			}
		}
	}

	// Token: 0x06003988 RID: 14728 RVA: 0x00139B74 File Offset: 0x00137D74
	public int GetPlayerLives(NetPlayer player)
	{
		if (player == null)
		{
			return 0;
		}
		if (this.playerLives.TryGetValue(player.ActorNumber, out this.outLives))
		{
			return this.outLives;
		}
		return 0;
	}

	// Token: 0x06003989 RID: 14729 RVA: 0x00139B9C File Offset: 0x00137D9C
	public bool PlayerInHitCooldown(NetPlayer player)
	{
		float num;
		return this.playerHitTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown > Time.time;
	}

	// Token: 0x0600398A RID: 14730 RVA: 0x00139BD0 File Offset: 0x00137DD0
	public bool PlayerInStunCooldown(NetPlayer player)
	{
		float num;
		return this.playerStunTimes.TryGetValue(player.ActorNumber, out num) && num + this.hitCooldown + this.stunGracePeriod > Time.time;
	}

	// Token: 0x0600398B RID: 14731 RVA: 0x00139C0A File Offset: 0x00137E0A
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerStatus(NetPlayer player)
	{
		if (this.playerStatusDict.TryGetValue(player.ActorNumber, out this.tempStatus))
		{
			return this.tempStatus;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x0600398C RID: 14732 RVA: 0x00139C2D File Offset: 0x00137E2D
	public bool OnRedTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
	}

	// Token: 0x0600398D RID: 14733 RVA: 0x00139C38 File Offset: 0x00137E38
	public bool OnRedTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnRedTeam(playerStatus);
	}

	// Token: 0x0600398E RID: 14734 RVA: 0x00139C54 File Offset: 0x00137E54
	public bool OnBlueTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return this.HasFlag(status, GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
	}

	// Token: 0x0600398F RID: 14735 RVA: 0x00139C60 File Offset: 0x00137E60
	public bool OnBlueTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnBlueTeam(playerStatus);
	}

	// Token: 0x06003990 RID: 14736 RVA: 0x00139C7C File Offset: 0x00137E7C
	public bool OnNoTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		return !this.OnRedTeam(status) && !this.OnBlueTeam(status);
	}

	// Token: 0x06003991 RID: 14737 RVA: 0x00139C94 File Offset: 0x00137E94
	public bool OnNoTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.OnNoTeam(playerStatus);
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x00139CB0 File Offset: 0x00137EB0
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(GorillaPaintbrawlManager.PaintbrawlStatus status)
	{
		if (this.OnRedTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam;
		}
		if (this.OnBlueTeam(status))
		{
			return GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam;
		}
		return GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x00139CCC File Offset: 0x00137ECC
	public GorillaPaintbrawlManager.PaintbrawlStatus GetPlayerTeam(NetPlayer player)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(player);
		return this.GetPlayerTeam(playerStatus);
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x00002076 File Offset: 0x00000276
	public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		return false;
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x00139CE8 File Offset: 0x00137EE8
	public override bool LocalIsTagged(NetPlayer player)
	{
		return this.GetPlayerLives(player) == 0;
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x00139CF4 File Offset: 0x00137EF4
	public bool OnSameTeam(GorillaPaintbrawlManager.PaintbrawlStatus playerA, GorillaPaintbrawlManager.PaintbrawlStatus playerB)
	{
		bool flag = this.OnRedTeam(playerA) && this.OnRedTeam(playerB);
		bool flag2 = this.OnBlueTeam(playerA) && this.OnBlueTeam(playerB);
		return flag || flag2;
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x00139D2C File Offset: 0x00137F2C
	public bool OnSameTeam(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus = this.GetPlayerStatus(myPlayer);
		GorillaPaintbrawlManager.PaintbrawlStatus playerStatus2 = this.GetPlayerStatus(otherPlayer);
		return this.OnSameTeam(playerStatus, playerStatus2);
	}

	// Token: 0x06003998 RID: 14744 RVA: 0x00139D54 File Offset: 0x00137F54
	public bool LocalCanHit(NetPlayer myPlayer, NetPlayer otherPlayer)
	{
		bool flag = !this.OnSameTeam(myPlayer, otherPlayer);
		bool flag2 = this.GetPlayerLives(otherPlayer) != 0;
		return flag && flag2;
	}

	// Token: 0x06003999 RID: 14745 RVA: 0x00139D7C File Offset: 0x00137F7C
	private void CopyBattleDictToArray()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			this.playerLivesArray[i] = 0;
			this.playerActorNumberArray[i] = -1;
		}
		int num = 0;
		foreach (KeyValuePair<int, int> keyValuePair in this.playerLives)
		{
			if (num >= this.playerLivesArray.Length)
			{
				break;
			}
			this.playerActorNumberArray[num] = keyValuePair.Key;
			this.playerLivesArray[num] = keyValuePair.Value;
			this.playerStatusArray[num] = this.GetPlayerStatus(NetworkSystem.Instance.GetPlayer(keyValuePair.Key));
			num++;
		}
	}

	// Token: 0x0600399A RID: 14746 RVA: 0x00139E40 File Offset: 0x00138040
	private void CopyArrayToBattleDict()
	{
		for (int i = 0; i < this.playerLivesArray.Length; i++)
		{
			if (this.playerActorNumberArray[i] != -1 && Utils.PlayerInRoom(this.playerActorNumberArray[i]))
			{
				if (this.playerLives.TryGetValue(this.playerActorNumberArray[i], out this.outLives))
				{
					this.playerLives[this.playerActorNumberArray[i]] = this.playerLivesArray[i];
				}
				else
				{
					this.playerLives.Add(this.playerActorNumberArray[i], this.playerLivesArray[i]);
				}
				if (this.playerStatusDict.ContainsKey(this.playerActorNumberArray[i]))
				{
					this.playerStatusDict[this.playerActorNumberArray[i]] = this.playerStatusArray[i];
				}
				else
				{
					this.playerStatusDict.Add(this.playerActorNumberArray[i], this.playerStatusArray[i]);
				}
			}
		}
	}

	// Token: 0x0600399B RID: 14747 RVA: 0x00139F26 File Offset: 0x00138126
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState | flag;
	}

	// Token: 0x0600399C RID: 14748 RVA: 0x000FCC07 File Offset: 0x000FAE07
	private GorillaPaintbrawlManager.PaintbrawlStatus SetFlagExclusive(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return flag;
	}

	// Token: 0x0600399D RID: 14749 RVA: 0x00139F2B File Offset: 0x0013812B
	private GorillaPaintbrawlManager.PaintbrawlStatus ClearFlag(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return currState & ~flag;
	}

	// Token: 0x0600399E RID: 14750 RVA: 0x00138C51 File Offset: 0x00136E51
	private bool FlagIsSet(GorillaPaintbrawlManager.PaintbrawlStatus currState, GorillaPaintbrawlManager.PaintbrawlStatus flag)
	{
		return (currState & flag) > GorillaPaintbrawlManager.PaintbrawlStatus.None;
	}

	// Token: 0x0600399F RID: 14751 RVA: 0x00139F34 File Offset: 0x00138134
	public void RandomizeTeams()
	{
		int[] array = new int[RoomSystem.PlayersInRoom.Count];
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			array[i] = i;
		}
		Random rand = new Random();
		int[] array2 = array.OrderBy((int x) => rand.Next()).ToArray<int>();
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = ((rand.Next(0, 2) == 0) ? GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam : GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam);
		GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus2 = ((paintbrawlStatus == GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam : GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		for (int j = 0; j < RoomSystem.PlayersInRoom.Count; j++)
		{
			GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus3 = ((array2[j] % 2 == 0) ? paintbrawlStatus2 : paintbrawlStatus);
			this.playerStatusDict[RoomSystem.PlayersInRoom[j].ActorNumber] = paintbrawlStatus3;
		}
	}

	// Token: 0x060039A0 RID: 14752 RVA: 0x0013A000 File Offset: 0x00138200
	public void AddPlayerToCorrectTeam(NetPlayer newPlayer)
	{
		this.rcount = 0;
		for (int i = 0; i < RoomSystem.PlayersInRoom.Count; i++)
		{
			if (this.playerStatusDict.ContainsKey(RoomSystem.PlayersInRoom[i].ActorNumber))
			{
				GorillaPaintbrawlManager.PaintbrawlStatus paintbrawlStatus = this.playerStatusDict[RoomSystem.PlayersInRoom[i].ActorNumber];
				this.rcount = (this.HasFlag(paintbrawlStatus, GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) ? (this.rcount + 1) : this.rcount);
			}
		}
		if ((RoomSystem.PlayersInRoom.Count - 1) / 2 == this.rcount)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = ((Random.Range(0, 2) == 0) ? this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) : this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.BlueTeam));
			return;
		}
		if (this.rcount <= (RoomSystem.PlayersInRoom.Count - 1) / 2)
		{
			this.playerStatusDict[newPlayer.ActorNumber] = this.SetFlag(this.playerStatusDict[newPlayer.ActorNumber], GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam);
		}
	}

	// Token: 0x060039A1 RID: 14753 RVA: 0x0013A124 File Offset: 0x00138324
	private void InitializePlayerStatus()
	{
		int num = this.CopyDictKeysToBuffer<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		for (int i = 0; i < num; i++)
		{
			this.playerStatusDict[this.reusableKeyBuffer[i]] = GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
		}
	}

	// Token: 0x060039A2 RID: 14754 RVA: 0x0013A160 File Offset: 0x00138360
	private void UpdatePlayerStatus()
	{
		int num = this.CopyDictKeysToBuffer<GorillaPaintbrawlManager.PaintbrawlStatus>(this.playerStatusDict);
		for (int i = 0; i < num; i++)
		{
			int num2 = this.reusableKeyBuffer[i];
			GorillaPaintbrawlManager.PaintbrawlStatus playerTeam = this.GetPlayerTeam(this.playerStatusDict[num2]);
			if (this.playerLives.TryGetValue(num2, out this.outLives) && this.outLives == 0)
			{
				this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Eliminated;
			}
			else if (this.playerHitTimes.TryGetValue(num2, out this.outHitTime) && this.outHitTime + this.hitCooldown > Time.time)
			{
				this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Hit;
			}
			else if (this.playerStunTimes.TryGetValue(num2, out this.outHitTime))
			{
				if (this.outHitTime + this.hitCooldown > Time.time)
				{
					this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Stunned;
				}
				else if (this.outHitTime + this.hitCooldown + this.stunGracePeriod > Time.time)
				{
					this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Grace;
				}
				else
				{
					this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
				}
			}
			else
			{
				this.playerStatusDict[num2] = playerTeam | GorillaPaintbrawlManager.PaintbrawlStatus.Normal;
			}
		}
	}

	// Token: 0x0400498E RID: 18830
	private float playerMin = 2f;

	// Token: 0x0400498F RID: 18831
	public float tagCoolDown = 5f;

	// Token: 0x04004990 RID: 18832
	public Dictionary<int, int> playerLives = new Dictionary<int, int>();

	// Token: 0x04004991 RID: 18833
	public Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus> playerStatusDict = new Dictionary<int, GorillaPaintbrawlManager.PaintbrawlStatus>();

	// Token: 0x04004992 RID: 18834
	public Dictionary<int, float> playerHitTimes = new Dictionary<int, float>();

	// Token: 0x04004993 RID: 18835
	public Dictionary<int, float> playerStunTimes = new Dictionary<int, float>();

	// Token: 0x04004994 RID: 18836
	public int[] playerActorNumberArray = new int[] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

	// Token: 0x04004995 RID: 18837
	public int[] playerLivesArray = new int[10];

	// Token: 0x04004996 RID: 18838
	public GorillaPaintbrawlManager.PaintbrawlStatus[] playerStatusArray = new GorillaPaintbrawlManager.PaintbrawlStatus[10];

	// Token: 0x04004997 RID: 18839
	public bool teamBattle = true;

	// Token: 0x04004998 RID: 18840
	public int countDownTime;

	// Token: 0x04004999 RID: 18841
	private float timeBattleEnded;

	// Token: 0x0400499A RID: 18842
	public float hitCooldown = 3f;

	// Token: 0x0400499B RID: 18843
	public float stunGracePeriod = 2f;

	// Token: 0x0400499C RID: 18844
	public object objRef;

	// Token: 0x0400499D RID: 18845
	private bool playerInList;

	// Token: 0x0400499E RID: 18846
	private bool coroutineRunning;

	// Token: 0x0400499F RID: 18847
	private int lives;

	// Token: 0x040049A0 RID: 18848
	private int outLives;

	// Token: 0x040049A1 RID: 18849
	private int bcount;

	// Token: 0x040049A2 RID: 18850
	private int rcount;

	// Token: 0x040049A3 RID: 18851
	private int randInt;

	// Token: 0x040049A4 RID: 18852
	private float outHitTime;

	// Token: 0x040049A5 RID: 18853
	private NetworkView tempView;

	// Token: 0x040049A6 RID: 18854
	private int[] reusableKeyBuffer = new int[20];

	// Token: 0x040049A7 RID: 18855
	private GorillaPaintbrawlManager.PaintbrawlStatus tempStatus;

	// Token: 0x040049A8 RID: 18856
	private GorillaPaintbrawlManager.PaintbrawlState currentState;

	// Token: 0x040049A9 RID: 18857
	private bool _isDefaultSlingshotSynced;

	// Token: 0x040049AA RID: 18858
	private readonly HashSet<VRRig> _slingshotPreloadedRigs = new HashSet<VRRig>(20);

	// Token: 0x0200089C RID: 2204
	public enum PaintbrawlStatus
	{
		// Token: 0x040049AC RID: 18860
		RedTeam = 1,
		// Token: 0x040049AD RID: 18861
		BlueTeam,
		// Token: 0x040049AE RID: 18862
		Normal = 4,
		// Token: 0x040049AF RID: 18863
		Hit = 8,
		// Token: 0x040049B0 RID: 18864
		Stunned = 16,
		// Token: 0x040049B1 RID: 18865
		Grace = 32,
		// Token: 0x040049B2 RID: 18866
		Eliminated = 64,
		// Token: 0x040049B3 RID: 18867
		None = 0
	}

	// Token: 0x0200089D RID: 2205
	public enum PaintbrawlState
	{
		// Token: 0x040049B5 RID: 18869
		NotEnoughPlayers,
		// Token: 0x040049B6 RID: 18870
		GameEnd,
		// Token: 0x040049B7 RID: 18871
		GameEndWaiting,
		// Token: 0x040049B8 RID: 18872
		StartCountdown,
		// Token: 0x040049B9 RID: 18873
		CountingDownToStart,
		// Token: 0x040049BA RID: 18874
		GameStart,
		// Token: 0x040049BB RID: 18875
		GameRunning
	}
}
