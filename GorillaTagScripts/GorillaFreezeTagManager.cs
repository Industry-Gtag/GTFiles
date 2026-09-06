using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaGameModes;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F91 RID: 3985
	public sealed class GorillaFreezeTagManager : GorillaTagManager
	{
		// Token: 0x06006308 RID: 25352 RVA: 0x001B524B File Offset: 0x001B344B
		public override GameModeType GameType()
		{
			return GameModeType.FreezeTag;
		}

		// Token: 0x06006309 RID: 25353 RVA: 0x001FDEA6 File Offset: 0x001FC0A6
		public override string GameModeName()
		{
			return "FREEZE TAG";
		}

		// Token: 0x0600630A RID: 25354 RVA: 0x001FDEB0 File Offset: 0x001FC0B0
		public override string GameModeNameRoomLabel()
		{
			string text;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("GAME_MODE_FREEZE_TAG_ROOM_LABEL", out text, "(FREEZE TAG GAME)"))
			{
				Debug.LogError("[LOCALIZATION::GORILLA_GAME_MANAGER] Failed to get key for Game Mode [GAME_MODE_FREEZE_TAG_ROOM_LABEL]");
			}
			return text;
		}

		// Token: 0x0600630B RID: 25355 RVA: 0x001FDEDB File Offset: 0x001FC0DB
		public override void Awake()
		{
			base.Awake();
			this.fastJumpLimitCached = this.fastJumpLimit;
			this.fastJumpMultiplierCached = this.fastJumpMultiplier;
			this.slowJumpLimitCached = this.slowJumpLimit;
			this.slowJumpMultiplierCached = this.slowJumpMultiplier;
		}

		// Token: 0x0600630C RID: 25356 RVA: 0x001FDF14 File Offset: 0x001FC114
		public override void UpdateState()
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen.ToList<KeyValuePair<NetPlayer, float>>())
				{
					if (Time.time - keyValuePair.Value >= this.freezeDuration)
					{
						this.currentFrozen.Remove(keyValuePair.Key);
						this.AddInfectedPlayer(keyValuePair.Key, false);
						RoomSystem.SendSoundEffectAll(11, 0.25f, false);
					}
				}
				if (GameMode.ParticipatingPlayers.Count < 1)
				{
					this.ResetGame();
					base.SetisCurrentlyTag(true);
					return;
				}
				if (this.isCurrentlyTag && this.currentIt == null)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				else if (this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count >= this.infectedModeThreshold)
				{
					this.ResetGame();
					int num2 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num2], true);
				}
				else if (!this.isCurrentlyTag && GameMode.ParticipatingPlayers.Count < this.infectedModeThreshold)
				{
					this.ResetGame();
					base.SetisCurrentlyTag(true);
					int num3 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num3], false);
				}
				else if (!this.isCurrentlyTag && this.currentInfected.Count == 0)
				{
					int num4 = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.AddInfectedPlayer(GameMode.ParticipatingPlayers[num4], true);
				}
				bool flag = true;
				foreach (NetPlayer netPlayer in GameMode.ParticipatingPlayers)
				{
					if (!this.IsFrozen(netPlayer) && !base.IsInfected(netPlayer))
					{
						flag = false;
						break;
					}
				}
				if (flag && !this.isCurrentlyTag)
				{
					this.InfectionRoundEnd();
				}
			}
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x001FE144 File Offset: 0x001FC344
		public override void Tick()
		{
			base.Tick();
			if (this.localVRRig)
			{
				this.localVRRig.IsFrozen = this.IsFrozen(NetworkSystem.Instance.LocalPlayer);
			}
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x001FE174 File Offset: 0x001FC374
		public override void StartPlaying()
		{
			base.StartPlaying();
			this.localVRRig = this.FindPlayerVRRig(NetworkSystem.Instance.LocalPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				foreach (NetPlayer netPlayer in this.lastRoundInfectedPlayers.ToArray())
				{
					if (netPlayer != null && !netPlayer.InRoom)
					{
						this.lastRoundInfectedPlayers.Remove(netPlayer);
					}
				}
				foreach (NetPlayer netPlayer2 in this.currentRoundInfectedPlayers.ToArray())
				{
					if (netPlayer2 != null && !netPlayer2.InRoom)
					{
						this.currentRoundInfectedPlayers.Remove(netPlayer2);
					}
				}
			}
		}

		// Token: 0x0600630F RID: 25359 RVA: 0x001FE218 File Offset: 0x001FC418
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
				Debug.LogWarning("Report TAG - tagged " + this.taggedRig.playerNameVisible + ", tagging " + this.taggingRig.playerNameVisible);
				if (this.isCurrentlyTag)
				{
					if (taggingPlayer == this.currentIt && taggingPlayer != taggedPlayer && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
					{
						base.AddLastTagged(taggedPlayer, taggingPlayer);
						this.ChangeCurrentIt(taggedPlayer, false);
						this.lastTag = (double)Time.time;
						return;
					}
				}
				else if (this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && !this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						MonkeAgent.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					base.AddLastTagged(taggedPlayer, taggingPlayer);
					this.AddFrozenPlayer(taggedPlayer);
					return;
				}
				else if (!this.currentInfected.Contains(taggingPlayer) && !this.currentInfected.Contains(taggedPlayer) && this.currentFrozen.ContainsKey(taggedPlayer) && (double)Time.time > this.lastTag + (double)this.tagCoolDown)
				{
					if (!this.taggingRig.IsPositionInRange(this.taggedRig.transform.position, 6f) && !this.taggingRig.CheckTagDistanceRollback(this.taggedRig, 6f, 0.2f))
					{
						MonkeAgent.instance.SendReport("extremely far tag", taggingPlayer.UserId, taggingPlayer.NickName);
						return;
					}
					this.UnfreezePlayer(taggedPlayer);
				}
			}
		}

		// Token: 0x06006310 RID: 25360 RVA: 0x001FE448 File Offset: 0x001FC648
		public override bool LocalCanTag(NetPlayer myPlayer, NetPlayer otherPlayer)
		{
			if (this.isCurrentlyTag)
			{
				return myPlayer == this.currentIt && myPlayer != otherPlayer;
			}
			return (this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(otherPlayer) && !this.currentInfected.Contains(otherPlayer)) || (!this.currentInfected.Contains(myPlayer) && !this.currentFrozen.ContainsKey(myPlayer) && (this.currentInfected.Contains(otherPlayer) || this.currentFrozen.ContainsKey(otherPlayer)));
		}

		// Token: 0x06006311 RID: 25361 RVA: 0x001FE4D7 File Offset: 0x001FC6D7
		public override bool LocalIsTagged(NetPlayer player)
		{
			if (this.isCurrentlyTag)
			{
				return this.currentIt == player;
			}
			return this.currentInfected.Contains(player) || this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x001FE507 File Offset: 0x001FC707
		public override void NewVRRig(NetPlayer player, int vrrigPhotonViewID, bool didTutorial)
		{
			if (NetworkSystem.Instance.IsMasterClient)
			{
				GameMode.RefreshPlayers();
				if (!this.isCurrentlyTag && !base.IsInfected(player))
				{
					this.AddInfectedPlayer(player, true);
					this.currentRoundInfectedPlayers.Add(player);
				}
				this.UpdateInfectionState();
			}
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x001FE545 File Offset: 0x001FC745
		protected override IEnumerator InfectionRoundEndingCoroutine()
		{
			while ((double)Time.time < this.timeInfectedGameEnded + (double)this.tagCoolDown)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (!this.isCurrentlyTag && this.waitingToStartNextInfectionGame)
			{
				base.ClearInfectionState();
				this.currentFrozen.Clear();
				GameMode.RefreshPlayers();
				this.lastRoundInfectedPlayers.Clear();
				this.lastRoundInfectedPlayers.AddRange(this.currentRoundInfectedPlayers);
				this.currentRoundInfectedPlayers.Clear();
				List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
				int num = 0;
				if (participatingPlayers.Count > 0 && participatingPlayers.Count < this.infectMorePlayerLowerThreshold)
				{
					num = 1;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerLowerThreshold && participatingPlayers.Count < this.infectMorePlayerUpperThreshold)
				{
					num = 2;
				}
				else if (participatingPlayers.Count >= this.infectMorePlayerUpperThreshold)
				{
					num = 3;
				}
				for (int i = 0; i < num; i++)
				{
					this.TryAddNewInfectedPlayer();
				}
				this.lastTag = (double)Time.time;
			}
			yield return null;
			yield break;
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x001FE554 File Offset: 0x001FC754
		public override void ResetGame()
		{
			base.ResetGame();
			this.currentFrozen.Clear();
			this.currentRoundInfectedPlayers.Clear();
			this.lastRoundInfectedPlayers.Clear();
		}

		// Token: 0x06006315 RID: 25365 RVA: 0x00143296 File Offset: 0x00141496
		private new void AddInfectedPlayer(NetPlayer infectedPlayer, bool withTagStop = true)
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

		// Token: 0x06006316 RID: 25366 RVA: 0x001FE580 File Offset: 0x001FC780
		private void TryAddNewInfectedPlayer()
		{
			List<NetPlayer> participatingPlayers = GameMode.ParticipatingPlayers;
			int num = Random.Range(0, participatingPlayers.Count);
			int num2 = 0;
			while (num2 < 10 && this.lastRoundInfectedPlayers.Contains(participatingPlayers[num]))
			{
				num = Random.Range(0, participatingPlayers.Count);
				num2++;
			}
			this.AddInfectedPlayer(participatingPlayers[num], true);
			this.currentRoundInfectedPlayers.Add(participatingPlayers[num]);
		}

		// Token: 0x06006317 RID: 25367 RVA: 0x001FE5EE File Offset: 0x001FC7EE
		public override int MyMatIndex(NetPlayer forPlayer)
		{
			if (this.isCurrentlyTag && forPlayer == this.currentIt)
			{
				return 14;
			}
			if (this.currentInfected.Contains(forPlayer))
			{
				return 14;
			}
			return 0;
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x001FE618 File Offset: 0x001FC818
		public override void UpdatePlayerAppearance(VRRig rig)
		{
			NetPlayer netPlayer = (rig.isOfflineVRRig ? NetworkSystem.Instance.LocalPlayer : rig.creator);
			rig.UpdateFrozenEffect(this.IsFrozen(netPlayer));
			int num = this.MyMatIndex(netPlayer);
			rig.ChangeMaterialLocal(num);
		}

		// Token: 0x06006319 RID: 25369 RVA: 0x001FE65C File Offset: 0x001FC85C
		private void UnfreezePlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Remove(taggedPlayer);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.UnTagged, taggedPlayer);
				RoomSystem.SendSoundEffectAll(10, 0.25f, true);
			}
		}

		// Token: 0x0600631A RID: 25370 RVA: 0x001FE69C File Offset: 0x001FC89C
		private void AddFrozenPlayer(NetPlayer taggedPlayer)
		{
			if (NetworkSystem.Instance.IsMasterClient && !this.currentFrozen.ContainsKey(taggedPlayer))
			{
				this.currentFrozen.Add(taggedPlayer, Time.time);
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.FrozenTime, taggedPlayer);
				RoomSystem.SendSoundEffectAll(9, 0.25f, false);
				RoomSystem.SendSoundEffectToPlayer(12, 0.05f, taggedPlayer, false);
			}
		}

		// Token: 0x0600631B RID: 25371 RVA: 0x001FE6F6 File Offset: 0x001FC8F6
		public bool IsFrozen(NetPlayer player)
		{
			return this.currentFrozen.ContainsKey(player);
		}

		// Token: 0x0600631C RID: 25372 RVA: 0x001FE704 File Offset: 0x001FC904
		public override float[] LocalPlayerSpeed()
		{
			this.fastJumpLimit = this.fastJumpLimitCached;
			this.fastJumpMultiplier = this.fastJumpMultiplierCached;
			this.slowJumpLimit = this.slowJumpLimitCached;
			this.slowJumpMultiplier = this.slowJumpMultiplierCached;
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
				if (!this.currentInfected.Contains(NetworkSystem.Instance.LocalPlayer) && !this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.playerSpeed[0] = base.InterpolatedNoobJumpSpeed(this.currentInfected.Count);
					this.playerSpeed[1] = base.InterpolatedNoobJumpMultiplier(this.currentInfected.Count);
					return this.playerSpeed;
				}
				if (this.currentFrozen.ContainsKey(NetworkSystem.Instance.LocalPlayer))
				{
					this.fastJumpLimit = this.frozenPlayerFastJumpLimit;
					this.fastJumpMultiplier = this.frozenPlayerFastJumpMultiplier;
					this.slowJumpLimit = this.frozenPlayerSlowJumpLimit;
					this.slowJumpMultiplier = this.frozenPlayerSlowJumpMultiplier;
				}
				this.playerSpeed[0] = base.InterpolatedInfectedJumpSpeed(this.currentInfected.Count);
				this.playerSpeed[1] = base.InterpolatedInfectedJumpMultiplier(this.currentInfected.Count);
				return this.playerSpeed;
			}
		}

		// Token: 0x0600631D RID: 25373 RVA: 0x001FE888 File Offset: 0x001FCA88
		public int GetFrozenHandTapAudioIndex()
		{
			int num = Random.Range(0, this.frozenHandTapIndices.Length);
			return this.frozenHandTapIndices[num];
		}

		// Token: 0x0600631E RID: 25374 RVA: 0x001FE8AC File Offset: 0x001FCAAC
		public override void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			base.OnPlayerLeftRoom(otherPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				if (this.isCurrentlyTag && ((otherPlayer != null && otherPlayer == this.currentIt) || this.currentIt.ActorNumber == otherPlayer.ActorNumber) && GameMode.ParticipatingPlayers.Count > 0)
				{
					int num = Random.Range(0, GameMode.ParticipatingPlayers.Count);
					this.ChangeCurrentIt(GameMode.ParticipatingPlayers[num], false);
				}
				if (this.currentInfected.Contains(otherPlayer))
				{
					this.currentInfected.Remove(otherPlayer);
				}
				if (this.currentFrozen.ContainsKey(otherPlayer))
				{
					this.currentFrozen.Remove(otherPlayer);
				}
				this.UpdateState();
			}
		}

		// Token: 0x0600631F RID: 25375 RVA: 0x001FE964 File Offset: 0x001FCB64
		public override void StopPlaying()
		{
			base.StopPlaying();
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				vrrig.ForceResetFrozenEffect();
			}
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x001FE9B4 File Offset: 0x001FCBB4
		public override void OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeRead(stream, info);
			this.currentFrozen.Clear();
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)stream.ReceiveNext();
				float num3 = (float)stream.ReceiveNext();
				NetPlayer player = NetworkSystem.Instance.GetPlayer(num2);
				this.currentFrozen.Add(player, num3);
			}
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x001FEA20 File Offset: 0x001FCC20
		public override void OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
		{
			base.OnSerializeWrite(stream, info);
			stream.SendNext(this.currentFrozen.Count);
			foreach (KeyValuePair<NetPlayer, float> keyValuePair in this.currentFrozen)
			{
				stream.SendNext(keyValuePair.Key.ActorNumber);
				stream.SendNext(keyValuePair.Value);
			}
		}

		// Token: 0x040071CA RID: 29130
		public Dictionary<NetPlayer, float> currentFrozen = new Dictionary<NetPlayer, float>(10);

		// Token: 0x040071CB RID: 29131
		public float freezeDuration;

		// Token: 0x040071CC RID: 29132
		public int infectMorePlayerLowerThreshold = 6;

		// Token: 0x040071CD RID: 29133
		public int infectMorePlayerUpperThreshold = 10;

		// Token: 0x040071CE RID: 29134
		[Space]
		[Header("Frozen player jump settings")]
		public float frozenPlayerFastJumpLimit;

		// Token: 0x040071CF RID: 29135
		public float frozenPlayerFastJumpMultiplier;

		// Token: 0x040071D0 RID: 29136
		public float frozenPlayerSlowJumpLimit;

		// Token: 0x040071D1 RID: 29137
		public float frozenPlayerSlowJumpMultiplier;

		// Token: 0x040071D2 RID: 29138
		[GorillaSoundLookup]
		public int[] frozenHandTapIndices;

		// Token: 0x040071D3 RID: 29139
		private float fastJumpLimitCached;

		// Token: 0x040071D4 RID: 29140
		private float fastJumpMultiplierCached;

		// Token: 0x040071D5 RID: 29141
		private float slowJumpLimitCached;

		// Token: 0x040071D6 RID: 29142
		private float slowJumpMultiplierCached;

		// Token: 0x040071D7 RID: 29143
		private VRRig localVRRig;

		// Token: 0x040071D8 RID: 29144
		private int hapticStrength;

		// Token: 0x040071D9 RID: 29145
		private List<NetPlayer> currentRoundInfectedPlayers = new List<NetPlayer>(10);

		// Token: 0x040071DA RID: 29146
		private List<NetPlayer> lastRoundInfectedPlayers = new List<NetPlayer>(10);
	}
}
