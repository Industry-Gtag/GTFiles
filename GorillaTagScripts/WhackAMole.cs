using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000F44 RID: 3908
	[NetworkBehaviourWeaved(210)]
	public class WhackAMole : NetworkComponent
	{
		// Token: 0x06005FE9 RID: 24553 RVA: 0x001E6C04 File Offset: 0x001E4E04
		private void UpdateMeshRendererList()
		{
			List<MeshRenderer> list = new List<MeshRenderer>();
			ZoneBasedObject[] array = this.zoneBasedVisuals;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MeshRenderer meshRenderer in array[i].GetComponentsInChildren<MeshRenderer>(true))
				{
					if (meshRenderer.enabled)
					{
						list.Add(meshRenderer);
					}
				}
			}
			this.zoneBasedMeshRenderers = list.ToArray();
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x001E6C6C File Offset: 0x001E4E6C
		protected override void Awake()
		{
			base.Awake();
			if (this.molesContainerRight != null)
			{
				this.rightMolesList = new List<Mole>(this.molesContainerRight.GetComponentsInChildren<Mole>());
				if (this.rightMolesList.Count > 0)
				{
					this.molesList.AddRange(this.rightMolesList);
				}
			}
			if (this.molesContainerLeft != null)
			{
				this.leftMolesList = new List<Mole>(this.molesContainerLeft.GetComponentsInChildren<Mole>());
				if (this.leftMolesList.Count > 0)
				{
					this.molesList.AddRange(this.leftMolesList);
					foreach (Mole mole in this.leftMolesList)
					{
						mole.IsLeftSideMole = true;
					}
				}
			}
			this.currentLevelIndex = -1;
			foreach (Mole mole2 in this.molesList)
			{
				mole2.OnTapped += this.OnMoleTapped;
			}
			List<Mole> list = this.leftMolesList;
			bool flag;
			if (list != null && list.Count > 0)
			{
				list = this.rightMolesList;
				flag = list != null && list.Count > 0;
			}
			else
			{
				flag = false;
			}
			this.isMultiplayer = flag;
			this.welcomeUI.SetActive(false);
			this.ongoingGameUI.SetActive(false);
			this.levelEndedUI.SetActive(false);
			this.ContinuePressedUI.SetActive(false);
			this.multiplyareScoresUI.SetActive(false);
			this.bestScore = 0;
			this.bestScoreText.text = string.Empty;
			this.highScorePlayerName = string.Empty;
			this.victoryParticles = this.victoryFX.GetComponentsInChildren<ParticleSystem>();
		}

		// Token: 0x06005FEB RID: 24555 RVA: 0x001E6E3C File Offset: 0x001E503C
		protected override void Start()
		{
			base.Start();
			this.SwitchState(WhackAMole.GameState.Off);
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Register(this);
			}
		}

		// Token: 0x06005FEC RID: 24556 RVA: 0x001E6E64 File Offset: 0x001E5064
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (Mole mole in this.molesList)
			{
				mole.OnTapped -= this.OnMoleTapped;
			}
			if (WhackAMoleManager.instance)
			{
				WhackAMoleManager.instance.Unregister(this);
			}
			this.molesList.Clear();
		}

		// Token: 0x06005FED RID: 24557 RVA: 0x001E6EE8 File Offset: 0x001E50E8
		public void InvokeUpdate()
		{
			bool isMasterClient = NetworkSystem.Instance.IsMasterClient;
			bool flag = this.zoneBasedVisuals[0].IsLocalPlayerInZone();
			if (isMasterClient != this.wasMasterClient || flag != this.wasLocalPlayerInZone)
			{
				MeshRenderer[] array = this.zoneBasedMeshRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = flag;
				}
				bool flag2 = isMasterClient || flag;
				ZoneBasedObject[] array2 = this.zoneBasedVisuals;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(flag2);
				}
				this.wasMasterClient = isMasterClient;
				this.wasLocalPlayerInZone = flag;
			}
		}

		// Token: 0x06005FEE RID: 24558 RVA: 0x001E6F80 File Offset: 0x001E5180
		private void SwitchState(WhackAMole.GameState state)
		{
			this.lastState = this.currentState;
			this.currentState = state;
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.ResetGame();
				this.currentLevelIndex = -1;
				this.currentLevel = null;
				this.UpdateLevelUI(1);
				break;
			case WhackAMole.GameState.ContinuePressed:
				this.continuePressedTime = Time.time;
				this.audioSource.GTStop();
				this.audioSource.GTPlayOneShot(this.counterClip, 1f);
				if (base.IsMine)
				{
					this.pickedMolesIndex.Clear();
				}
				this.ResetGame();
				if (base.IsMine)
				{
					this.LoadNextLevel();
				}
				break;
			case WhackAMole.GameState.Ongoing:
				this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
				break;
			case WhackAMole.GameState.TimesUp:
				if (this.currentLevel != null)
				{
					foreach (Mole mole in this.molesList)
					{
						mole.HideMole(false);
					}
					this.curentGameResult = this.GetGameResult();
					this.UpdateResultUI(this.curentGameResult);
					this.levelEndedTotalScoreText.text = "SCORE " + this.totalScore.ToString();
					this.levelEndedCurrentScoreText.text = string.Format("{0}/{1}", this.currentScore, this.currentLevel.GetMinScore(this.isMultiplayer));
					if (this.totalScore > this.bestScore)
					{
						this.bestScore = this.totalScore;
						this.highScorePlayerName = this.playerName;
					}
					this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
					this.audioSource.GTStop();
					if (this.curentGameResult == WhackAMole.GameResult.LevelComplete)
					{
						this.audioSource.GTPlayOneShot(this.levelCompleteClip, 1f);
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					else if (this.curentGameResult == WhackAMole.GameResult.GameOver)
					{
						this.audioSource.GTPlayOneShot(this.gameOverClip, 1f);
					}
					else if (this.curentGameResult == WhackAMole.GameResult.Win)
					{
						this.audioSource.GTPlayOneShot(this.winClip, 1f);
						if (this.victoryFX)
						{
							ParticleSystem[] array = this.victoryParticles;
							for (int i = 0; i < array.Length; i++)
							{
								array[i].Play();
							}
						}
						if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
						{
							PlayerGameEvents.MiscEvent("WhackComplete" + this.currentLevel.levelNumber.ToString(), 1);
						}
					}
					int minScore = this.currentLevel.GetMinScore(this.isMultiplayer);
					if (this.levelGoodMolesPicked < minScore)
					{
						GTDev.LogError<string>(string.Format("[WAM] Lvl:{0} Only Picked {1}/{2} good moles!", this.currentLevel.levelNumber, this.levelGoodMolesPicked, minScore), null);
					}
					if (base.IsMine)
					{
						GorillaTelemetry.WamLevelEnd(this.playerId, this.gameId, this.machineId, this.currentLevel.levelNumber, this.levelGoodMolesPicked, this.levelHazardMolesPicked, minScore, this.currentScore, this.levelHazardMolesHit, this.curentGameResult.ToString());
					}
				}
				break;
			}
			this.UpdateScreenData();
		}

		// Token: 0x06005FEF RID: 24559 RVA: 0x001E7348 File Offset: 0x001E5548
		private void UpdateScreenData()
		{
			switch (this.currentState)
			{
			case WhackAMole.GameState.Off:
				this.welcomeUI.SetActive(true);
				this.ContinuePressedUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.levelEndedUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				return;
			case WhackAMole.GameState.ContinuePressed:
				this.levelEndedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.multiplyareScoresUI.SetActive(false);
				this.ContinuePressedUI.SetActive(true);
				break;
			case WhackAMole.GameState.Ongoing:
				this.ContinuePressedUI.SetActive(false);
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(true);
				this.levelEndedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
					return;
				}
				break;
			case WhackAMole.GameState.PickMoles:
				break;
			case WhackAMole.GameState.TimesUp:
				this.welcomeUI.SetActive(false);
				this.ongoingGameUI.SetActive(false);
				this.ContinuePressedUI.SetActive(false);
				if (this.isMultiplayer)
				{
					this.multiplyareScoresUI.SetActive(true);
				}
				this.levelEndedUI.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005FF0 RID: 24560 RVA: 0x001E7480 File Offset: 0x001E5680
		public static int CreateNewGameID()
		{
			int num = (int)((DateTime.Now - WhackAMole.epoch).TotalSeconds * 8.0 % 2147483646.0) + 1;
			if (num <= WhackAMole.lastAssignedID)
			{
				WhackAMole.lastAssignedID++;
				return WhackAMole.lastAssignedID;
			}
			WhackAMole.lastAssignedID = num;
			return num;
		}

		// Token: 0x06005FF1 RID: 24561 RVA: 0x001E74E0 File Offset: 0x001E56E0
		private void OnMoleTapped(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeftHand)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.Off || gameState == WhackAMole.GameState.TimesUp)
			{
				return;
			}
			AudioClip audioClip = (moleType.isHazard ? this.whackHazardClips[Random.Range(0, this.whackHazardClips.Length)] : this.whackMonkeClips[Random.Range(0, this.whackMonkeClips.Length)]);
			if (moleType.isHazard)
			{
				this.audioSource.GTPlayOneShot(audioClip, 1f);
				this.levelHazardMolesHit++;
			}
			else
			{
				this.audioSource.GTPlayOneShot(audioClip, 1f);
			}
			if (moleType.monkeMoleHitMaterial != null)
			{
				moleType.MeshRenderer.material = moleType.monkeMoleHitMaterial;
			}
			this.currentScore += moleType.scorePoint;
			this.totalScore += moleType.scorePoint;
			if (moleType.IsLeftSideMoleType)
			{
				this.leftPlayerScore += moleType.scorePoint;
			}
			else
			{
				this.rightPlayerScore += moleType.scorePoint;
			}
			this.UpdateScoreUI(this.currentScore, this.leftPlayerScore, this.rightPlayerScore);
			moleType.MoleContainerParent.HideMole(true);
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x001E7604 File Offset: 0x001E5804
		public void HandleOnTimerStopped()
		{
			this.gameEndedTime = Time.time;
			this.SwitchState(WhackAMole.GameState.TimesUp);
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x001E7618 File Offset: 0x001E5818
		private IEnumerator PlayHazardAudio(AudioClip clip)
		{
			this.audioSource.clip = clip;
			this.audioSource.GTPlay();
			yield return new WaitForSeconds(this.audioSource.clip.length);
			this.audioSource.clip = this.errorClip;
			this.audioSource.GTPlay();
			yield break;
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x001E7630 File Offset: 0x001E5830
		private bool PickMoles()
		{
			WhackAMole.<>c__DisplayClass85_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			this.pickedMolesIndex.Clear();
			float passedTime = this.timer.GetPassedTime();
			if (passedTime > this.currentLevel.levelDuration - this.currentLevel.showMoleDuration)
			{
				return true;
			}
			float num = passedTime / this.currentLevel.levelDuration;
			CS$<>8__locals1.minMoleCount = Mathf.Lerp(this.currentLevel.minimumMoleCount.x, this.currentLevel.minimumMoleCount.y, num);
			CS$<>8__locals1.maxMoleCount = Mathf.Lerp(this.currentLevel.maximumMoleCount.x, this.currentLevel.maximumMoleCount.y, num);
			this.curentTime = Time.time;
			CS$<>8__locals1.hazardMoleChance = Mathf.Lerp(this.currentLevel.hazardMoleChance.x, this.currentLevel.hazardMoleChance.y, num);
			if (this.isMultiplayer)
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.rightMolesList, ref CS$<>8__locals1);
				this.<PickMoles>g__PickMolesFrom|85_0(this.leftMolesList, ref CS$<>8__locals1);
			}
			else
			{
				this.<PickMoles>g__PickMolesFrom|85_0(this.molesList, ref CS$<>8__locals1);
			}
			return this.pickedMolesIndex.Count != 0;
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x001E775C File Offset: 0x001E595C
		private void LoadNextLevel()
		{
			if (this.currentLevel != null)
			{
				this.resetToFirstLevel = this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer);
				if (this.resetToFirstLevel)
				{
					this.currentLevelIndex = 0;
				}
				else
				{
					this.currentLevelIndex++;
				}
				if (this.currentLevelIndex >= this.allLevels.Length)
				{
					this.currentLevelIndex = 0;
				}
			}
			else
			{
				this.currentLevelIndex++;
			}
			this.currentLevel = this.allLevels[this.currentLevelIndex];
			this.timer.SetTimerDuration(this.currentLevel.levelDuration);
			this.timer.RestartTimer();
			this.curentTime = Time.time;
			this.currentScore = 0;
			this.leftPlayerScore = 0;
			this.rightPlayerScore = 0;
			this.levelGoodMolesPicked = (this.levelHazardMolesPicked = 0);
			this.levelHazardMolesHit = 0;
			if (this.currentLevelIndex == 0)
			{
				this.totalScore = 0;
			}
			if (this.currentLevelIndex == 0 && base.IsMine)
			{
				this.gameId = WhackAMole.CreateNewGameID();
				Debug.LogWarning("GAME ID" + this.gameId.ToString());
			}
		}

		// Token: 0x06005FF6 RID: 24566 RVA: 0x001E788C File Offset: 0x001E5A8C
		private bool PickSingleMole(int randomMoleIndex, float hazardMoleChance)
		{
			bool flag = hazardMoleChance > 0f && Random.value <= hazardMoleChance;
			int moleTypeIndex = this.molesList[randomMoleIndex].GetMoleTypeIndex(flag);
			this.molesList[randomMoleIndex].ShowMole(this.currentLevel.showMoleDuration, moleTypeIndex);
			this.pickedMolesIndex.Add(randomMoleIndex, moleTypeIndex);
			if (flag)
			{
				this.levelHazardMolesPicked++;
			}
			else
			{
				this.levelGoodMolesPicked++;
			}
			return flag;
		}

		// Token: 0x06005FF7 RID: 24567 RVA: 0x001E7910 File Offset: 0x001E5B10
		private void ResetGame()
		{
			foreach (Mole mole in this.molesList)
			{
				mole.ResetPosition();
			}
		}

		// Token: 0x06005FF8 RID: 24568 RVA: 0x001E7960 File Offset: 0x001E5B60
		private void UpdateScoreUI(int totalScore, int _leftPlayerScore, int _rightPlayerScore)
		{
			if (this.currentLevel != null)
			{
				this.scoreText.text = string.Format("SCORE\n{0}/{1}", totalScore, this.currentLevel.GetMinScore(this.isMultiplayer));
				this.leftPlayerScoreText.text = _leftPlayerScore.ToString();
				this.rightPlayerScoreText.text = _rightPlayerScore.ToString();
			}
		}

		// Token: 0x06005FF9 RID: 24569 RVA: 0x001E79D0 File Offset: 0x001E5BD0
		private void UpdateLevelUI(int levelNumber)
		{
			this.arrowTargetRotation = Quaternion.Euler(0f, 0f, (float)(18 * (levelNumber - 1)));
			this.arrowRotationNeedsUpdate = true;
		}

		// Token: 0x06005FFA RID: 24570 RVA: 0x001E79F8 File Offset: 0x001E5BF8
		private void UpdateArrowRotation()
		{
			Quaternion quaternion = Quaternion.Slerp(this.levelArrow.transform.localRotation, this.arrowTargetRotation, Time.deltaTime * 5f);
			if (Quaternion.Angle(quaternion, this.arrowTargetRotation) < 0.1f)
			{
				quaternion = this.arrowTargetRotation;
				this.arrowRotationNeedsUpdate = false;
			}
			this.levelArrow.transform.localRotation = quaternion;
		}

		// Token: 0x06005FFB RID: 24571 RVA: 0x001E7A5E File Offset: 0x001E5C5E
		private void UpdateTimerUI(int time)
		{
			if (time == this.previousTime)
			{
				return;
			}
			this.timeText.text = "TIME " + time.ToString();
			this.previousTime = time;
		}

		// Token: 0x06005FFC RID: 24572 RVA: 0x001E7A8D File Offset: 0x001E5C8D
		private void UpdateResultUI(WhackAMole.GameResult gameResult)
		{
			if (gameResult == WhackAMole.GameResult.LevelComplete)
			{
				this.resultText.text = "LEVEL COMPLETE";
				return;
			}
			if (gameResult == WhackAMole.GameResult.Win)
			{
				this.resultText.text = "YOU WIN!";
				return;
			}
			if (gameResult == WhackAMole.GameResult.GameOver)
			{
				this.resultText.text = "GAME OVER";
			}
		}

		// Token: 0x06005FFD RID: 24573 RVA: 0x001E7ACC File Offset: 0x001E5CCC
		public void OnStartButtonPressed()
		{
			WhackAMole.GameState gameState = this.currentState;
			if (gameState == WhackAMole.GameState.TimesUp || gameState == WhackAMole.GameState.Off)
			{
				base.GetView.RPC("WhackAMoleButtonPressed", RpcTarget.All, Array.Empty<object>());
			}
		}

		// Token: 0x06005FFE RID: 24574 RVA: 0x001E7AFD File Offset: 0x001E5CFD
		[PunRPC]
		private void WhackAMoleButtonPressed(PhotonMessageInfo info)
		{
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06005FFF RID: 24575 RVA: 0x001E7B0C File Offset: 0x001E5D0C
		[Rpc]
		private unsafe void RPC_WhackAMoleButtonPressed(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) == 0)
					{
						NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", base.Object, 7);
					}
					else
					{
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.WhackAMole::RPC_WhackAMoleButtonPressed(Fusion.RpcInfo)", num);
						}
						else
						{
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 7) != 0)
							{
								info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
								goto IL_0012;
							}
						}
					}
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			this.WhackAMoleButtonPressedShared(info);
		}

		// Token: 0x06006000 RID: 24576 RVA: 0x001E7C4C File Offset: 0x001E5E4C
		private void WhackAMoleButtonPressedShared(PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "WhackAMoleButtonPressedShared");
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
			if (vrrig)
			{
				this.playerName = vrrig.playerNameVisible;
				if (this.currentState == WhackAMole.GameState.Off)
				{
					this.playerId = info.Sender.UserId;
					if (NetworkSystem.Instance.LocalPlayer.UserId == this.playerId)
					{
						PlayerGameEvents.MiscEvent("PlayArcadeGame", 1);
					}
				}
			}
			this.SwitchState(WhackAMole.GameState.ContinuePressed);
		}

		// Token: 0x06006001 RID: 24577 RVA: 0x001E7CCB File Offset: 0x001E5ECB
		private WhackAMole.GameResult GetGameResult()
		{
			if (this.currentScore < this.currentLevel.GetMinScore(this.isMultiplayer))
			{
				return WhackAMole.GameResult.GameOver;
			}
			if (this.currentLevelIndex >= this.allLevels.Length - 1)
			{
				return WhackAMole.GameResult.Win;
			}
			return WhackAMole.GameResult.LevelComplete;
		}

		// Token: 0x06006002 RID: 24578 RVA: 0x001E7CFD File Offset: 0x001E5EFD
		public int GetCurrentLevel()
		{
			if (this.currentLevel != null)
			{
				return this.currentLevel.levelNumber;
			}
			return 0;
		}

		// Token: 0x06006003 RID: 24579 RVA: 0x001E7D1A File Offset: 0x001E5F1A
		public int GetTotalLevelNumbers()
		{
			if (this.allLevels != null)
			{
				return this.allLevels.Length;
			}
			return 0;
		}

		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x06006004 RID: 24580 RVA: 0x001E7D2E File Offset: 0x001E5F2E
		// (set) Token: 0x06006005 RID: 24581 RVA: 0x001E7D58 File Offset: 0x001E5F58
		[Networked]
		[NetworkedWeaved(0, 210)]
		public unsafe WhackAMole.WhackAMoleData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(WhackAMole.WhackAMoleData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing WhackAMole.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(WhackAMole.WhackAMoleData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06006006 RID: 24582 RVA: 0x001E7D84 File Offset: 0x001E5F84
		public override void WriteDataFusion()
		{
			this.Data = new WhackAMole.WhackAMoleData(this.currentState, this.currentLevelIndex, this.currentScore, this.totalScore, this.bestScore, this.rightPlayerScore, this.highScorePlayerName, this.timer.GetRemainingTime(), this.gameEndedTime, this.gameId, this.pickedMolesIndex);
			this.pickedMolesIndex.Clear();
		}

		// Token: 0x06006007 RID: 24583 RVA: 0x001E7DF0 File Offset: 0x001E5FF0
		public override void ReadDataFusion()
		{
			this.ReadDataShared(this.Data.CurrentState, this.Data.CurrentLevelIndex, this.Data.CurrentScore, this.Data.TotalScore, this.Data.BestScore, this.Data.RightPlayerScore, this.Data.HighScorePlayerName.Value, this.Data.RemainingTime, this.Data.GameEndedTime, this.Data.GameId);
			for (int i = 0; i < this.Data.PickedMolesIndexCount; i++)
			{
				int num = this.Data.PickedMolesIndex[i];
				if (i >= 0 && i < this.molesList.Count && this.currentLevel)
				{
					this.molesList[i].ShowMole(this.currentLevel.showMoleDuration, num);
				}
			}
		}

		// Token: 0x06006008 RID: 24584 RVA: 0x001E7F08 File Offset: 0x001E6108
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x06006009 RID: 24585 RVA: 0x001E7F18 File Offset: 0x001E6118
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
		}

		// Token: 0x0600600A RID: 24586 RVA: 0x001E7F28 File Offset: 0x001E6128
		private void ReadDataShared(WhackAMole.GameState _currentState, int _currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float _remainingTime, float endedTime, int _gameId)
		{
			WhackAMole.GameState gameState = this.currentState;
			if (_currentState != gameState)
			{
				this.SwitchState(_currentState);
			}
			this.currentLevelIndex = _currentLevelIndex;
			if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
			{
				this.currentLevel = this.allLevels[this.currentLevelIndex];
				this.UpdateLevelUI(this.currentLevel.levelNumber);
			}
			this.currentScore = cScore;
			this.totalScore = tScore;
			this.bestScore = bScore;
			this.rightPlayerScore = rPScore;
			this.leftPlayerScore = this.currentScore - this.rightPlayerScore;
			this.highScorePlayerName = hScorePName;
			this.bestScoreText.text = (this.isMultiplayer ? this.bestScore.ToString() : (this.highScorePlayerName + "  " + this.bestScore.ToString()));
			this.remainingTime = _remainingTime;
			if (float.IsFinite(this.remainingTime) && this.currentLevel)
			{
				this.remainingTime = this.remainingTime.ClampSafe(0f, this.currentLevel.levelDuration);
				this.UpdateTimerUI((int)this.remainingTime);
			}
			if (float.IsFinite(endedTime))
			{
				this.gameEndedTime = endedTime.ClampSafe(0f, Time.time);
			}
			this.gameId = _gameId;
		}

		// Token: 0x0600600B RID: 24587 RVA: 0x001E807C File Offset: 0x001E627C
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (NetworkSystem.Instance.IsMasterClient)
			{
				this.timer.RestartTimer();
				this.timer.SetTimerDuration(this.remainingTime);
				this.curentTime = Time.time;
				if (this.currentLevelIndex >= 0 && this.currentLevelIndex < this.allLevels.Length)
				{
					this.currentLevel = this.allLevels[this.currentLevelIndex];
				}
				this.SwitchState(this.currentState);
			}
		}

		// Token: 0x0600600E RID: 24590 RVA: 0x001E8180 File Offset: 0x001E6380
		[CompilerGenerated]
		private void <PickMoles>g__PickMolesFrom|85_0(List<Mole> moles, ref WhackAMole.<>c__DisplayClass85_0 A_2)
		{
			int num = Mathf.RoundToInt(Random.Range(A_2.minMoleCount, A_2.maxMoleCount));
			this.potentialMoles.Clear();
			foreach (Mole mole in moles)
			{
				if (mole.CanPickMole())
				{
					this.potentialMoles.Add(mole);
				}
			}
			int num2 = Mathf.Min(num, this.potentialMoles.Count);
			int num3 = Mathf.CeilToInt((float)num2 * A_2.hazardMoleChance);
			int num4 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num5 = Random.Range(0, this.potentialMoles.Count);
				if (this.PickSingleMole(this.molesList.IndexOf(this.potentialMoles[num5]), (num4 < num3) ? A_2.hazardMoleChance : 0f))
				{
					num4++;
				}
				this.potentialMoles.RemoveAt(num5);
			}
		}

		// Token: 0x0600600F RID: 24591 RVA: 0x001E828C File Offset: 0x001E648C
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06006010 RID: 24592 RVA: 0x001E82A4 File Offset: 0x001E64A4
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x06006011 RID: 24593 RVA: 0x001E82B8 File Offset: 0x001E64B8
		[NetworkRpcWeavedInvoker(1, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_WhackAMoleButtonPressed@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((WhackAMole)behaviour).RPC_WhackAMoleButtonPressed(rpcInfo);
		}

		// Token: 0x04006E67 RID: 28263
		public string machineId = "default";

		// Token: 0x04006E68 RID: 28264
		public GameObject molesContainerRight;

		// Token: 0x04006E69 RID: 28265
		[Tooltip("Only for co-op version")]
		public GameObject molesContainerLeft;

		// Token: 0x04006E6A RID: 28266
		public int betweenLevelPauseDuration = 3;

		// Token: 0x04006E6B RID: 28267
		public int countdownDuration = 5;

		// Token: 0x04006E6C RID: 28268
		public WhackAMoleLevelSO[] allLevels;

		// Token: 0x04006E6D RID: 28269
		[SerializeField]
		private GorillaTimer timer;

		// Token: 0x04006E6E RID: 28270
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x04006E6F RID: 28271
		public GameObject levelArrow;

		// Token: 0x04006E70 RID: 28272
		public GameObject victoryFX;

		// Token: 0x04006E71 RID: 28273
		public ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x04006E72 RID: 28274
		[SerializeField]
		private MeshRenderer[] zoneBasedMeshRenderers;

		// Token: 0x04006E73 RID: 28275
		[Space]
		public AudioClip backgroundLoop;

		// Token: 0x04006E74 RID: 28276
		public AudioClip errorClip;

		// Token: 0x04006E75 RID: 28277
		public AudioClip counterClip;

		// Token: 0x04006E76 RID: 28278
		public AudioClip levelCompleteClip;

		// Token: 0x04006E77 RID: 28279
		public AudioClip winClip;

		// Token: 0x04006E78 RID: 28280
		public AudioClip gameOverClip;

		// Token: 0x04006E79 RID: 28281
		public AudioClip[] whackHazardClips;

		// Token: 0x04006E7A RID: 28282
		public AudioClip[] whackMonkeClips;

		// Token: 0x04006E7B RID: 28283
		[Space]
		public GameObject welcomeUI;

		// Token: 0x04006E7C RID: 28284
		public GameObject ongoingGameUI;

		// Token: 0x04006E7D RID: 28285
		public GameObject levelEndedUI;

		// Token: 0x04006E7E RID: 28286
		public GameObject ContinuePressedUI;

		// Token: 0x04006E7F RID: 28287
		public GameObject multiplyareScoresUI;

		// Token: 0x04006E80 RID: 28288
		[Space]
		public TextMeshPro scoreText;

		// Token: 0x04006E81 RID: 28289
		public TextMeshPro bestScoreText;

		// Token: 0x04006E82 RID: 28290
		[Tooltip("Only for co-op version")]
		public TextMeshPro rightPlayerScoreText;

		// Token: 0x04006E83 RID: 28291
		[Tooltip("Only for co-op version")]
		public TextMeshPro leftPlayerScoreText;

		// Token: 0x04006E84 RID: 28292
		public TextMeshPro timeText;

		// Token: 0x04006E85 RID: 28293
		public TextMeshPro counterText;

		// Token: 0x04006E86 RID: 28294
		public TextMeshPro resultText;

		// Token: 0x04006E87 RID: 28295
		public TextMeshPro levelEndedOptionsText;

		// Token: 0x04006E88 RID: 28296
		public TextMeshPro levelEndedCountdownText;

		// Token: 0x04006E89 RID: 28297
		public TextMeshPro levelEndedTotalScoreText;

		// Token: 0x04006E8A RID: 28298
		public TextMeshPro levelEndedCurrentScoreText;

		// Token: 0x04006E8B RID: 28299
		private List<Mole> rightMolesList;

		// Token: 0x04006E8C RID: 28300
		private List<Mole> leftMolesList;

		// Token: 0x04006E8D RID: 28301
		private List<Mole> molesList = new List<Mole>();

		// Token: 0x04006E8E RID: 28302
		private WhackAMoleLevelSO currentLevel;

		// Token: 0x04006E8F RID: 28303
		private int currentScore;

		// Token: 0x04006E90 RID: 28304
		private int totalScore;

		// Token: 0x04006E91 RID: 28305
		private int leftPlayerScore;

		// Token: 0x04006E92 RID: 28306
		private int rightPlayerScore;

		// Token: 0x04006E93 RID: 28307
		private int bestScore;

		// Token: 0x04006E94 RID: 28308
		private float curentTime;

		// Token: 0x04006E95 RID: 28309
		private int currentLevelIndex;

		// Token: 0x04006E96 RID: 28310
		private float continuePressedTime;

		// Token: 0x04006E97 RID: 28311
		private bool resetToFirstLevel;

		// Token: 0x04006E98 RID: 28312
		private Quaternion arrowTargetRotation;

		// Token: 0x04006E99 RID: 28313
		private bool arrowRotationNeedsUpdate;

		// Token: 0x04006E9A RID: 28314
		private List<Mole> potentialMoles = new List<Mole>();

		// Token: 0x04006E9B RID: 28315
		private Dictionary<int, int> pickedMolesIndex = new Dictionary<int, int>();

		// Token: 0x04006E9C RID: 28316
		private WhackAMole.GameState currentState;

		// Token: 0x04006E9D RID: 28317
		private WhackAMole.GameState lastState;

		// Token: 0x04006E9E RID: 28318
		private float remainingTime;

		// Token: 0x04006E9F RID: 28319
		private int previousTime = -1;

		// Token: 0x04006EA0 RID: 28320
		private bool isMultiplayer;

		// Token: 0x04006EA1 RID: 28321
		private float gameEndedTime;

		// Token: 0x04006EA2 RID: 28322
		private WhackAMole.GameResult curentGameResult;

		// Token: 0x04006EA3 RID: 28323
		private string playerName = string.Empty;

		// Token: 0x04006EA4 RID: 28324
		private string highScorePlayerName = string.Empty;

		// Token: 0x04006EA5 RID: 28325
		private ParticleSystem[] victoryParticles;

		// Token: 0x04006EA6 RID: 28326
		private int levelHazardMolesPicked;

		// Token: 0x04006EA7 RID: 28327
		private int levelGoodMolesPicked;

		// Token: 0x04006EA8 RID: 28328
		private string playerId;

		// Token: 0x04006EA9 RID: 28329
		private int gameId;

		// Token: 0x04006EAA RID: 28330
		private int levelHazardMolesHit;

		// Token: 0x04006EAB RID: 28331
		private static DateTime epoch = new DateTime(2024, 1, 1);

		// Token: 0x04006EAC RID: 28332
		private static int lastAssignedID;

		// Token: 0x04006EAD RID: 28333
		private bool wasMasterClient;

		// Token: 0x04006EAE RID: 28334
		private bool wasLocalPlayerInZone = true;

		// Token: 0x04006EAF RID: 28335
		[WeaverGenerated]
		[SerializeField]
		[DefaultForProperty("Data", 0, 210)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private WhackAMole.WhackAMoleData _Data;

		// Token: 0x02000F45 RID: 3909
		public enum GameState
		{
			// Token: 0x04006EB1 RID: 28337
			Off,
			// Token: 0x04006EB2 RID: 28338
			ContinuePressed,
			// Token: 0x04006EB3 RID: 28339
			Ongoing,
			// Token: 0x04006EB4 RID: 28340
			PickMoles,
			// Token: 0x04006EB5 RID: 28341
			TimesUp,
			// Token: 0x04006EB6 RID: 28342
			LevelStarted
		}

		// Token: 0x02000F46 RID: 3910
		private enum GameResult
		{
			// Token: 0x04006EB8 RID: 28344
			GameOver,
			// Token: 0x04006EB9 RID: 28345
			Win,
			// Token: 0x04006EBA RID: 28346
			LevelComplete,
			// Token: 0x04006EBB RID: 28347
			Unknown
		}

		// Token: 0x02000F47 RID: 3911
		[NetworkStructWeaved(210)]
		[StructLayout(LayoutKind.Explicit, Size = 840)]
		public struct WhackAMoleData : INetworkStruct
		{
			// Token: 0x1700094E RID: 2382
			// (get) Token: 0x06006012 RID: 24594 RVA: 0x001E82FC File Offset: 0x001E64FC
			// (set) Token: 0x06006013 RID: 24595 RVA: 0x001E8304 File Offset: 0x001E6504
			public WhackAMole.GameState CurrentState { readonly get; set; }

			// Token: 0x1700094F RID: 2383
			// (get) Token: 0x06006014 RID: 24596 RVA: 0x001E830D File Offset: 0x001E650D
			// (set) Token: 0x06006015 RID: 24597 RVA: 0x001E8315 File Offset: 0x001E6515
			public int CurrentLevelIndex { readonly get; set; }

			// Token: 0x17000950 RID: 2384
			// (get) Token: 0x06006016 RID: 24598 RVA: 0x001E831E File Offset: 0x001E651E
			// (set) Token: 0x06006017 RID: 24599 RVA: 0x001E8326 File Offset: 0x001E6526
			public int CurrentScore { readonly get; set; }

			// Token: 0x17000951 RID: 2385
			// (get) Token: 0x06006018 RID: 24600 RVA: 0x001E832F File Offset: 0x001E652F
			// (set) Token: 0x06006019 RID: 24601 RVA: 0x001E8337 File Offset: 0x001E6537
			public int TotalScore { readonly get; set; }

			// Token: 0x17000952 RID: 2386
			// (get) Token: 0x0600601A RID: 24602 RVA: 0x001E8340 File Offset: 0x001E6540
			// (set) Token: 0x0600601B RID: 24603 RVA: 0x001E8348 File Offset: 0x001E6548
			public int BestScore { readonly get; set; }

			// Token: 0x17000953 RID: 2387
			// (get) Token: 0x0600601C RID: 24604 RVA: 0x001E8351 File Offset: 0x001E6551
			// (set) Token: 0x0600601D RID: 24605 RVA: 0x001E8359 File Offset: 0x001E6559
			public int RightPlayerScore { readonly get; set; }

			// Token: 0x17000954 RID: 2388
			// (get) Token: 0x0600601E RID: 24606 RVA: 0x001E8362 File Offset: 0x001E6562
			// (set) Token: 0x0600601F RID: 24607 RVA: 0x001E8374 File Offset: 0x001E6574
			[Networked]
			[NetworkedWeaved(6, 129)]
			public unsafe NetworkString<_128> HighScorePlayerName
			{
				readonly get
				{
					return *(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName);
				}
				set
				{
					*(NetworkString<_128>*)Native.ReferenceToPointer<FixedStorage@129>(ref this._HighScorePlayerName) = value;
				}
			}

			// Token: 0x17000955 RID: 2389
			// (get) Token: 0x06006020 RID: 24608 RVA: 0x001E8387 File Offset: 0x001E6587
			// (set) Token: 0x06006021 RID: 24609 RVA: 0x001E838F File Offset: 0x001E658F
			public float RemainingTime { readonly get; set; }

			// Token: 0x17000956 RID: 2390
			// (get) Token: 0x06006022 RID: 24610 RVA: 0x001E8398 File Offset: 0x001E6598
			// (set) Token: 0x06006023 RID: 24611 RVA: 0x001E83A0 File Offset: 0x001E65A0
			public float GameEndedTime { readonly get; set; }

			// Token: 0x17000957 RID: 2391
			// (get) Token: 0x06006024 RID: 24612 RVA: 0x001E83A9 File Offset: 0x001E65A9
			// (set) Token: 0x06006025 RID: 24613 RVA: 0x001E83B1 File Offset: 0x001E65B1
			public int GameId { readonly get; set; }

			// Token: 0x17000958 RID: 2392
			// (get) Token: 0x06006026 RID: 24614 RVA: 0x001E83BA File Offset: 0x001E65BA
			// (set) Token: 0x06006027 RID: 24615 RVA: 0x001E83C2 File Offset: 0x001E65C2
			public int PickedMolesIndexCount { readonly get; set; }

			// Token: 0x17000959 RID: 2393
			// (get) Token: 0x06006028 RID: 24616 RVA: 0x001E83CC File Offset: 0x001E65CC
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedDictionary(17, 1, 1, typeof(ElementReaderWriterInt32), typeof(ElementReaderWriterInt32))]
			[NetworkedWeaved(139, 71)]
			public unsafe NetworkDictionary<int, int> PickedMolesIndex
			{
				get
				{
					return new NetworkDictionary<int, int>((int*)Native.ReferenceToPointer<FixedStorage@71>(ref this._PickedMolesIndex), 17, ElementReaderWriterInt32.GetInstance(), ElementReaderWriterInt32.GetInstance());
				}
			}

			// Token: 0x06006029 RID: 24617 RVA: 0x001E83F8 File Offset: 0x001E65F8
			public WhackAMoleData(WhackAMole.GameState state, int currentLevelIndex, int cScore, int tScore, int bScore, int rPScore, string hScorePName, float remainingTime, float endedTime, int gameId, Dictionary<int, int> moleIndexs)
			{
				this.CurrentState = state;
				this.CurrentLevelIndex = currentLevelIndex;
				this.CurrentScore = cScore;
				this.TotalScore = tScore;
				this.BestScore = bScore;
				this.RightPlayerScore = rPScore;
				this.HighScorePlayerName = hScorePName;
				this.RemainingTime = remainingTime;
				this.GameEndedTime = endedTime;
				this.GameId = gameId;
				this.PickedMolesIndexCount = moleIndexs.Count;
				foreach (KeyValuePair<int, int> keyValuePair in moleIndexs)
				{
					this.PickedMolesIndex.Set(keyValuePair.Key, keyValuePair.Value);
				}
			}

			// Token: 0x04006EC2 RID: 28354
			[FixedBufferProperty(typeof(NetworkString<_128>), typeof(UnityValueSurrogate@ReaderWriter@Fusion_NetworkString), 0, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(24)]
			private FixedStorage@129 _HighScorePlayerName;

			// Token: 0x04006EC7 RID: 28359
			[FixedBufferProperty(typeof(NetworkDictionary<int, int>), typeof(UnityDictionarySurrogate@ElementReaderWriterInt32@ElementReaderWriterInt32), 17, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(556)]
			private FixedStorage@71 _PickedMolesIndex;
		}
	}
}
