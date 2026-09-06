using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CjLib;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag
{
	// Token: 0x0200120E RID: 4622
	[NetworkBehaviourWeaved(76)]
	public class ScienceExperimentManager : NetworkComponent, ITickSystemTick
	{
		// Token: 0x17000B57 RID: 2903
		// (get) Token: 0x06007518 RID: 29976 RVA: 0x002605B4 File Offset: 0x0025E7B4
		private bool RefreshWaterAvailable
		{
			get
			{
				return this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Erupting || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising && this.riseProgress < this.lavaProgressToDisableRefreshWater) || (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining && this.riseProgress < this.lavaProgressToEnableRefreshWater);
			}
		}

		// Token: 0x17000B58 RID: 2904
		// (get) Token: 0x06007519 RID: 29977 RVA: 0x00260618 File Offset: 0x0025E818
		public ScienceExperimentManager.RisingLiquidState GameState
		{
			get
			{
				return this.reliableState.state;
			}
		}

		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x0600751A RID: 29978 RVA: 0x00260625 File Offset: 0x0025E825
		public float RiseProgress
		{
			get
			{
				return this.riseProgress;
			}
		}

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x0600751B RID: 29979 RVA: 0x0026062D File Offset: 0x0025E82D
		public float RiseProgressLinear
		{
			get
			{
				return this.riseProgressLinear;
			}
		}

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x0600751C RID: 29980 RVA: 0x00260638 File Offset: 0x0025E838
		private int PlayerCount
		{
			get
			{
				int num = 1;
				GorillaGameManager gorillaGameManager = GorillaGameManager.instance;
				if (gorillaGameManager != null && gorillaGameManager.currentNetPlayerArray != null)
				{
					num = gorillaGameManager.currentNetPlayerArray.Length;
				}
				return num;
			}
		}

		// Token: 0x0600751D RID: 29981 RVA: 0x00260668 File Offset: 0x0025E868
		protected override void Awake()
		{
			base.Awake();
			if (ScienceExperimentManager.instance == null)
			{
				ScienceExperimentManager.instance = this;
				NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
				this.riseTimeLookup = new float[] { this.riseTimeFast, this.riseTimeMedium, this.riseTimeSlow, this.riseTimeExtraSlow };
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
				GorillaGameManager.OnTouch += this.OnPlayerTagged;
				RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
				RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
				this.rotatingRings = new ScienceExperimentManager.RotatingRingState[this.ringParent.childCount];
				for (int i = 0; i < this.rotatingRings.Length; i++)
				{
					this.rotatingRings[i].ringTransform = this.ringParent.GetChild(i);
					this.rotatingRings[i].initialAngle = 0f;
					this.rotatingRings[i].resultingAngle = 0f;
				}
				this.gameAreaTriggerNotifier.CompositeTriggerEnter += this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit += this.OnColliderExitedVolume;
				this.liquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				this.entryLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				if (this.bottleLiquidVolume != null)
				{
					this.bottleLiquidVolume.ColliderEnteredWater += this.OnColliderEnteredSoda;
					this.bottleLiquidVolume.ColliderExitedWater += this.OnColliderExitedSoda;
				}
				if (this.refreshWaterVolume != null)
				{
					this.refreshWaterVolume.ColliderEnteredWater += this.OnColliderEnteredRefreshWater;
					this.refreshWaterVolume.ColliderExitedWater += this.OnColliderExitedRefreshWater;
				}
				if (this.sodaWaterProjectileTriggerNotifier != null)
				{
					this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter += this.OnProjectileEnteredSodaWater;
				}
				float num = Vector3.Distance(this.drainBlockerClosedPosition.position, this.drainBlockerOpenPosition.position);
				this.drainBlockerSlideSpeed = num / this.drainBlockerSlideTime;
				return;
			}
			global::UnityEngine.Object.Destroy(this);
		}

		// Token: 0x0600751E RID: 29982 RVA: 0x0000C135 File Offset: 0x0000A335
		internal override void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x0600751F RID: 29983 RVA: 0x0000C149 File Offset: 0x0000A349
		internal override void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06007520 RID: 29984 RVA: 0x00260908 File Offset: 0x0025EB08
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			GorillaGameManager.OnTouch -= this.OnPlayerTagged;
			if (this.gameAreaTriggerNotifier != null)
			{
				this.gameAreaTriggerNotifier.CompositeTriggerEnter -= this.OnColliderEnteredVolume;
				this.gameAreaTriggerNotifier.CompositeTriggerExit -= this.OnColliderExitedVolume;
			}
			if (this.liquidVolume != null)
			{
				this.liquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.liquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.entryLiquidVolume != null)
			{
				this.entryLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.entryLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.bottleLiquidVolume != null)
			{
				this.bottleLiquidVolume.ColliderEnteredWater -= this.OnColliderEnteredSoda;
				this.bottleLiquidVolume.ColliderExitedWater -= this.OnColliderExitedSoda;
			}
			if (this.refreshWaterVolume != null)
			{
				this.refreshWaterVolume.ColliderEnteredWater -= this.OnColliderEnteredRefreshWater;
				this.refreshWaterVolume.ColliderExitedWater -= this.OnColliderExitedRefreshWater;
			}
			if (this.sodaWaterProjectileTriggerNotifier != null)
			{
				this.sodaWaterProjectileTriggerNotifier.OnProjectileTriggerEnter -= this.OnProjectileEnteredSodaWater;
			}
		}

		// Token: 0x06007521 RID: 29985 RVA: 0x00260A80 File Offset: 0x0025EC80
		public void InitElements(ScienceExperimentSceneElements elements)
		{
			this.elements = elements;
			this.fizzParticleEmission = elements.sodaFizzParticles.emission;
			elements.sodaFizzParticles.gameObject.SetActive(false);
			elements.sodaEruptionParticles.gameObject.SetActive(false);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		}

		// Token: 0x06007522 RID: 29986 RVA: 0x00260AE2 File Offset: 0x0025ECE2
		public void DeInitElements()
		{
			this.elements = null;
		}

		// Token: 0x06007523 RID: 29987 RVA: 0x00260AEC File Offset: 0x0025ECEC
		public Transform GetElement(ScienceExperimentElementID elementID)
		{
			switch (elementID)
			{
			case ScienceExperimentElementID.Platform1:
				return this.rotatingRings[0].ringTransform;
			case ScienceExperimentElementID.Platform2:
				return this.rotatingRings[1].ringTransform;
			case ScienceExperimentElementID.Platform3:
				return this.rotatingRings[2].ringTransform;
			case ScienceExperimentElementID.Platform4:
				return this.rotatingRings[3].ringTransform;
			case ScienceExperimentElementID.Platform5:
				return this.rotatingRings[4].ringTransform;
			case ScienceExperimentElementID.LiquidMesh:
				return this.liquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberLiquidMesh:
				return this.entryWayLiquidMeshTransform;
			case ScienceExperimentElementID.EntryChamberBridgeQuad:
				return this.entryWayBridgeQuadTransform;
			case ScienceExperimentElementID.DrainBlocker:
				return this.drainBlocker;
			default:
				Debug.LogError(string.Format("Unhandled ScienceExperiment element ID! {0}", elementID));
				return null;
			}
		}

		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x06007524 RID: 29988 RVA: 0x00260BB1 File Offset: 0x0025EDB1
		// (set) Token: 0x06007525 RID: 29989 RVA: 0x00260BB9 File Offset: 0x0025EDB9
		bool ITickSystemTick.TickRunning { get; set; }

		// Token: 0x06007526 RID: 29990 RVA: 0x00260BC4 File Offset: 0x0025EDC4
		void ITickSystemTick.Tick()
		{
			this.prevTime = this.currentTime;
			this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.unscaledTimeAsDouble);
			this.lastInfrequentUpdateTime = ((this.lastInfrequentUpdateTime > this.currentTime) ? this.currentTime : this.lastInfrequentUpdateTime);
			if (this.currentTime > this.lastInfrequentUpdateTime + (double)this.infrequentUpdatePeriod)
			{
				this.InfrequentUpdate();
				this.lastInfrequentUpdateTime = (double)((float)this.currentTime);
			}
			if (base.IsMine)
			{
				this.UpdateReliableState(this.currentTime, ref this.reliableState);
			}
			this.UpdateLocalState(this.currentTime, this.reliableState);
			this.localLagRiseProgressOffset = Mathf.MoveTowards(this.localLagRiseProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
			this.UpdateLiquid(this.riseProgress + this.localLagRiseProgressOffset);
			this.UpdateRotatingRings(this.ringRotationProgress);
			this.UpdateRefreshWater();
			this.UpdateDrainBlocker(this.currentTime);
			this.DisableObjectsInContactWithLava(this.liquidMeshTransform.localScale.z);
			this.UpdateEffects();
			if (this.debugDrawPlayerGameState)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					NetPlayer netPlayer = null;
					if (NetworkSystem.Instance.InRoom)
					{
						netPlayer = NetworkSystem.Instance.GetPlayer(this.inGamePlayerStates[i].playerId);
					}
					else if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
					{
						netPlayer = NetworkSystem.Instance.LocalPlayer;
					}
					RigContainer rigContainer;
					if (netPlayer != null && VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer) && rigContainer.Rig != null)
					{
						float num = 0.03f;
						DebugUtil.DrawSphere(rigContainer.Rig.transform.position + Vector3.up * 0.5f * num, 0.16f * num, 12, 12, this.inGamePlayerStates[i].touchedLiquid ? Color.red : Color.green, true, DebugUtil.Style.SolidColor);
					}
				}
			}
		}

		// Token: 0x06007527 RID: 29991 RVA: 0x00260DE4 File Offset: 0x0025EFE4
		private void InfrequentUpdate()
		{
			this.allPlayersInRoom = RoomSystem.PlayersInRoom.ToArray();
			if (base.IsMine)
			{
				for (int i = this.inGamePlayerCount - 1; i >= 0; i--)
				{
					int playerId = this.inGamePlayerStates[i].playerId;
					bool flag = false;
					for (int j = 0; j < this.allPlayersInRoom.Length; j++)
					{
						if (this.allPlayersInRoom[j].ActorNumber == playerId)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (i < this.inGamePlayerCount - 1)
						{
							this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						}
						this.inGamePlayerStates[this.inGamePlayerCount - 1] = default(ScienceExperimentManager.PlayerGameState);
						this.inGamePlayerCount--;
					}
				}
			}
			if (this.optPlayersOutOfRoomGameMode)
			{
				for (int k = 0; k < this.allPlayersInRoom.Length; k++)
				{
					bool flag2 = false;
					for (int l = 0; l < this.inGamePlayerCount; l++)
					{
						if (this.allPlayersInRoom[k].ActorNumber == this.inGamePlayerStates[l].playerId)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						global::GorillaGameModes.GameMode.OptOut(this.allPlayersInRoom[k]);
					}
					else
					{
						global::GorillaGameModes.GameMode.OptIn(this.allPlayersInRoom[k]);
					}
				}
			}
		}

		// Token: 0x06007528 RID: 29992 RVA: 0x00260F30 File Offset: 0x0025F130
		private bool PlayerInGame(Player player)
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == player.ActorNumber)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06007529 RID: 29993 RVA: 0x00260F6C File Offset: 0x0025F16C
		private void UpdateReliableState(double currentTime, ref ScienceExperimentManager.SyncData syncData)
		{
			if (currentTime < syncData.stateStartTime)
			{
				syncData.stateStartTime = currentTime;
			}
			switch (syncData.state)
			{
			default:
			{
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() > 0 && syncData.activationProgress > 1.0)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Erupting;
					syncData.stateStartTime = currentTime;
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.activationProgress = 1.0;
					return;
				}
				float num = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				syncData.activationProgress = (double)Mathf.MoveTowards((float)syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (currentTime > syncData.stateStartTime + (double)this.fullyDrainedWaitTime)
				{
					this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Rising;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = Mathf.Clamp01((float)((currentTime - syncData.stateStartTime) / (double)this.riseTime));
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				if (currentTime > syncData.stateStartTime + (double)this.riseTime)
				{
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Full;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Full:
				if (this.<UpdateReliableState>g__GetAlivePlayerCount|105_0() <= 0 || currentTime > syncData.stateStartTime + (double)this.maxFullTime)
				{
					this.UpdateWinner();
					syncData.stateStartLiquidProgressLinear = 1f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.PreDrainDelay;
					syncData.stateStartTime = currentTime;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				if (currentTime > syncData.stateStartTime + (double)this.preDrainWaitTime)
				{
					syncData.state = ScienceExperimentManager.RisingLiquidState.Draining;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
					for (int i = 0; i < this.rotatingRings.Length; i++)
					{
						float num2 = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
						float num3 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
						float num4 = ((Random.Range(0f, 1f) > 0.5f) ? 1f : (-1f));
						this.rotatingRings[i].initialAngle = num2;
						this.rotatingRings[i].resultingAngle = num2 + num4 * num3;
					}
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num5 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				if (currentTime + num5 > syncData.stateStartTime + (double)this.drainTime)
				{
					syncData.stateStartLiquidProgressLinear = 0f;
					syncData.state = ScienceExperimentManager.RisingLiquidState.Drained;
					syncData.stateStartTime = currentTime;
					syncData.activationProgress = 0.0;
				}
				break;
			}
			}
		}

		// Token: 0x0600752A RID: 29994 RVA: 0x00261248 File Offset: 0x0025F448
		private void UpdateLocalState(double currentTime, ScienceExperimentManager.SyncData syncData)
		{
			switch (syncData.state)
			{
			default:
				this.riseProgressLinear = 0f;
				this.riseProgress = 0f;
				if (!this.debugRandomizingRings)
				{
					this.ringRotationProgress = 1f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
			{
				double num = (currentTime - syncData.stateStartTime) / (double)this.riseTime;
				this.riseProgressLinear = Mathf.Clamp01((float)num);
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			}
			case ScienceExperimentManager.RisingLiquidState.Full:
				this.riseProgressLinear = 1f;
				this.riseProgress = 1f;
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.PreDrainDelay:
				this.riseProgressLinear = syncData.stateStartLiquidProgressLinear;
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = 1f;
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
			{
				double num2 = (1.0 - (double)syncData.stateStartLiquidProgressLinear) * (double)this.drainTime;
				double num3 = (currentTime + num2 - syncData.stateStartTime) / (double)this.drainTime;
				this.riseProgressLinear = Mathf.Clamp01((float)(1.0 - num3));
				this.riseProgress = this.animationCurve.Evaluate(this.riseProgressLinear);
				this.ringRotationProgress = (float)(currentTime - syncData.stateStartTime) / (this.drainTime * syncData.stateStartLiquidProgressLinear);
				break;
			}
			}
		}

		// Token: 0x0600752B RID: 29995 RVA: 0x002613B4 File Offset: 0x0025F5B4
		private void UpdateLiquid(float fillProgress)
		{
			float num = Mathf.Lerp(this.minScale, this.maxScale, fillProgress);
			this.liquidMeshTransform.localScale = new Vector3(1f, 1f, num);
			bool flag = this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Full || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.PreDrainDelay || this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining;
			this.liquidMeshTransform.gameObject.SetActive(flag);
			if (this.entryWayLiquidMeshTransform != null)
			{
				float num2 = 0f;
				float num3;
				float num4;
				if (num < this.entryLiquidScaleSyncOpeningBottom.y)
				{
					num3 = this.entryLiquidScaleSyncOpeningBottom.x;
					num4 = this.entryBridgeQuadMinMaxZHeight.x;
				}
				else if (num < this.entryLiquidScaleSyncOpeningTop.y)
				{
					float num5 = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningBottom.y, this.entryLiquidScaleSyncOpeningTop.y, num);
					num3 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningBottom.x, this.entryLiquidScaleSyncOpeningTop.x, num5);
					num4 = Mathf.Lerp(this.entryBridgeQuadMinMaxZHeight.x, this.entryBridgeQuadMinMaxZHeight.y, num5);
					num2 = this.entryBridgeQuadMaxScaleY * Mathf.Sin(num5 * 3.1415927f);
				}
				else
				{
					float num6 = Mathf.InverseLerp(this.entryLiquidScaleSyncOpeningTop.y, 0.6f * this.maxScale, num);
					num3 = Mathf.Lerp(this.entryLiquidScaleSyncOpeningTop.x, this.entryLiquidMaxScale, num6);
					num4 = this.entryBridgeQuadMinMaxZHeight.y;
				}
				this.entryWayLiquidMeshTransform.localScale = new Vector3(this.entryWayLiquidMeshTransform.localScale.x, this.entryWayLiquidMeshTransform.localScale.y, num3);
				this.entryWayBridgeQuadTransform.localScale = new Vector3(this.entryWayBridgeQuadTransform.localScale.x, num2, this.entryWayBridgeQuadTransform.localScale.z);
				this.entryWayBridgeQuadTransform.localPosition = new Vector3(this.entryWayBridgeQuadTransform.localPosition.x, this.entryWayBridgeQuadTransform.localPosition.y, num4);
			}
		}

		// Token: 0x0600752C RID: 29996 RVA: 0x002615D8 File Offset: 0x0025F7D8
		private void UpdateRotatingRings(float rotationProgress)
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float num = Mathf.Lerp(this.rotatingRings[i].initialAngle, this.rotatingRings[i].resultingAngle, rotationProgress);
				this.rotatingRings[i].ringTransform.rotation = Quaternion.AngleAxis(num, Vector3.up);
			}
		}

		// Token: 0x0600752D RID: 29997 RVA: 0x00261644 File Offset: 0x0025F844
		private void UpdateDrainBlocker(double currentTime)
		{
			if (this.reliableState.state != ScienceExperimentManager.RisingLiquidState.Draining)
			{
				this.drainBlocker.position = this.drainBlockerClosedPosition.position;
				return;
			}
			float num = (float)(currentTime - this.reliableState.stateStartTime);
			float num2 = (1f - this.reliableState.stateStartLiquidProgressLinear) * this.drainTime;
			if (this.drainTime - (num + num2) < this.drainBlockerSlideTime)
			{
				this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerClosedPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
				return;
			}
			this.drainBlocker.position = Vector3.MoveTowards(this.drainBlocker.position, this.drainBlockerOpenPosition.position, this.drainBlockerSlideSpeed * Time.deltaTime);
		}

		// Token: 0x0600752E RID: 29998 RVA: 0x00261718 File Offset: 0x0025F918
		private void UpdateEffects()
		{
			switch (this.reliableState.state)
			{
			case ScienceExperimentManager.RisingLiquidState.Drained:
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.elements.sodaFizzParticles.gameObject.SetActive(true);
					if (this.reliableState.activationProgress > 0.0010000000474974513)
					{
						this.fizzParticleEmission.rateOverTimeMultiplier = Mathf.Lerp(this.sodaFizzParticleEmissionMinMax.x, this.sodaFizzParticleEmissionMinMax.y, (float)this.reliableState.activationProgress);
						return;
					}
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Erupting:
				if (!this.hasPlayedEruptionEffects)
				{
					this.eruptionAudioSource.loop = true;
					this.eruptionAudioSource.GTPlay();
					this.hasPlayedEruptionEffects = true;
					if (this.elements != null)
					{
						this.elements.sodaEruptionParticles.gameObject.SetActive(true);
						this.fizzParticleEmission.rateOverTimeMultiplier = this.sodaFizzParticleEmissionMinMax.y;
						return;
					}
				}
				break;
			case ScienceExperimentManager.RisingLiquidState.Rising:
				if (this.elements != null)
				{
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
					return;
				}
				break;
			default:
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				this.hasPlayedEruptionEffects = false;
				this.hasPlayedDrainEffects = false;
				this.eruptionAudioSource.GTStop();
				this.drainAudioSource.GTStop();
				this.rotatingRingsAudioSource.GTStop();
				return;
			case ScienceExperimentManager.RisingLiquidState.Draining:
				this.hasPlayedEruptionEffects = false;
				this.eruptionAudioSource.GTStop();
				if (this.elements != null)
				{
					this.elements.sodaFizzParticles.gameObject.SetActive(false);
					this.elements.sodaEruptionParticles.gameObject.SetActive(false);
					this.fizzParticleEmission.rateOverTimeMultiplier = 0f;
				}
				if (!this.hasPlayedDrainEffects)
				{
					this.drainAudioSource.loop = true;
					this.drainAudioSource.GTPlay();
					this.rotatingRingsAudioSource.loop = true;
					this.rotatingRingsAudioSource.GTPlay();
					this.hasPlayedDrainEffects = true;
				}
				break;
			}
		}

		// Token: 0x0600752F RID: 29999 RVA: 0x002619B4 File Offset: 0x0025FBB4
		private void DisableObjectsInContactWithLava(float lavaScale)
		{
			if (this.elements == null)
			{
				return;
			}
			Plane plane = new Plane(this.liquidSurfacePlane.up, this.liquidSurfacePlane.position);
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Rising)
			{
				for (int i = 0; i < this.elements.disableByLiquidList.Count; i++)
				{
					if (!plane.GetSide(this.elements.disableByLiquidList[i].target.position + this.elements.disableByLiquidList[i].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[i].target.gameObject.SetActive(false);
					}
				}
				return;
			}
			if (this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Draining)
			{
				for (int j = 0; j < this.elements.disableByLiquidList.Count; j++)
				{
					if (plane.GetSide(this.elements.disableByLiquidList[j].target.position + this.elements.disableByLiquidList[j].heightOffset * Vector3.up))
					{
						this.elements.disableByLiquidList[j].target.gameObject.SetActive(true);
					}
				}
			}
		}

		// Token: 0x06007530 RID: 30000 RVA: 0x00261B20 File Offset: 0x0025FD20
		private void UpdateWinner()
		{
			float num = -1f;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
					break;
				}
				if (this.inGamePlayerStates[i].touchedLiquidAtProgress > num)
				{
					num = this.inGamePlayerStates[i].touchedLiquidAtProgress;
					this.lastWinnerId = this.inGamePlayerStates[i].playerId;
				}
			}
			this.RefreshWinnerName();
		}

		// Token: 0x06007531 RID: 30001 RVA: 0x00261BB4 File Offset: 0x0025FDB4
		private void RefreshWinnerName()
		{
			NetPlayer playerFromId = this.GetPlayerFromId(this.lastWinnerId);
			if (playerFromId != null)
			{
				this.lastWinnerName = playerFromId.NickName;
				return;
			}
			this.lastWinnerName = "None";
		}

		// Token: 0x06007532 RID: 30002 RVA: 0x00261BE9 File Offset: 0x0025FDE9
		private NetPlayer GetPlayerFromId(int id)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				return NetworkSystem.Instance.GetPlayer(id);
			}
			if (id == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				return NetworkSystem.Instance.LocalPlayer;
			}
			return null;
		}

		// Token: 0x06007533 RID: 30003 RVA: 0x00261C24 File Offset: 0x0025FE24
		private void UpdateRefreshWater()
		{
			if (this.refreshWaterVolume != null)
			{
				if (this.RefreshWaterAvailable && !this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(true);
					return;
				}
				if (!this.RefreshWaterAvailable && this.refreshWaterVolume.gameObject.activeSelf)
				{
					this.refreshWaterVolume.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x06007534 RID: 30004 RVA: 0x00261C98 File Offset: 0x0025FE98
		private void ResetGame()
		{
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
				playerGameState.touchedLiquid = false;
				playerGameState.touchedLiquidAtProgress = -1f;
				this.inGamePlayerStates[i] = playerGameState;
			}
		}

		// Token: 0x06007535 RID: 30005 RVA: 0x00261CE4 File Offset: 0x0025FEE4
		public void RestartGame()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
				this.ResetGame();
			}
		}

		// Token: 0x06007536 RID: 30006 RVA: 0x00261D6C File Offset: 0x0025FF6C
		public void DebugErupt()
		{
			if (base.IsMine)
			{
				this.riseTime = this.riseTimeLookup[(int)this.nextRoundRiseSpeed];
				this.reliableState.state = ScienceExperimentManager.RisingLiquidState.Erupting;
				this.reliableState.stateStartTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : ((double)Time.time));
				this.reliableState.stateStartLiquidProgressLinear = 0f;
				this.reliableState.activationProgress = 1.0;
			}
		}

		// Token: 0x06007537 RID: 30007 RVA: 0x00261DF0 File Offset: 0x0025FFF0
		public void RandomizeRings()
		{
			for (int i = 0; i < this.rotatingRings.Length; i++)
			{
				float num = Mathf.Repeat(this.rotatingRings[i].resultingAngle, 360f);
				float num2 = Random.Range(this.rotatingRingRandomAngleRange.x, this.rotatingRingRandomAngleRange.y);
				float num3 = ((Random.Range(0f, 1f) > 0.5f) ? 1f : (-1f));
				this.rotatingRings[i].initialAngle = num;
				float num4 = num + num3 * num2;
				if (this.rotatingRingQuantizeAngles)
				{
					num4 = Mathf.Round(num4 / this.rotatingRingAngleSnapDegrees) * this.rotatingRingAngleSnapDegrees;
				}
				this.rotatingRings[i].resultingAngle = num4;
			}
			if (this.rotateRingsCoroutine != null)
			{
				base.StopCoroutine(this.rotateRingsCoroutine);
			}
			this.rotateRingsCoroutine = base.StartCoroutine(this.RotateRingsCoroutine());
		}

		// Token: 0x06007538 RID: 30008 RVA: 0x00261EE2 File Offset: 0x002600E2
		private IEnumerator RotateRingsCoroutine()
		{
			if (this.debugRotateRingsTime > 0.01f)
			{
				float routineStartTime = Time.time;
				this.ringRotationProgress = 0f;
				this.debugRandomizingRings = true;
				while (this.ringRotationProgress < 1f)
				{
					this.ringRotationProgress = (Time.time - routineStartTime) / this.debugRotateRingsTime;
					yield return null;
				}
			}
			this.debugRandomizingRings = false;
			this.ringRotationProgress = 1f;
			yield break;
		}

		// Token: 0x06007539 RID: 30009 RVA: 0x00261EF4 File Offset: 0x002600F4
		public bool GetMaterialIfPlayerInGame(int playerActorNumber, out int materialIndex)
		{
			int i = 0;
			while (i < this.inGamePlayerCount)
			{
				if (this.inGamePlayerStates[i].playerId == playerActorNumber)
				{
					if (this.inGamePlayerStates[i].touchedLiquid)
					{
						materialIndex = 12;
						return true;
					}
					materialIndex = 0;
					return true;
				}
				else
				{
					i++;
				}
			}
			materialIndex = 0;
			return false;
		}

		// Token: 0x0600753A RID: 30010 RVA: 0x00261F48 File Offset: 0x00260148
		private void OnPlayerTagged(NetPlayer taggedPlayer, NetPlayer taggingPlayer)
		{
			if (base.IsMine)
			{
				int num = -1;
				int num2 = -1;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == taggedPlayer.ActorNumber)
					{
						num = i;
					}
					else if (this.inGamePlayerStates[i].playerId == taggingPlayer.ActorNumber)
					{
						num2 = i;
					}
					if (num != -1 && num2 != -1)
					{
						break;
					}
				}
				if (num == -1 || num2 == -1)
				{
					return;
				}
				switch (this.tagBehavior)
				{
				case ScienceExperimentManager.TagBehavior.None:
					break;
				case ScienceExperimentManager.TagBehavior.Infect:
					if (this.inGamePlayerStates[num2].touchedLiquid && !this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[num];
						playerGameState.touchedLiquid = true;
						playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						this.inGamePlayerStates[num] = playerGameState;
						return;
					}
					break;
				case ScienceExperimentManager.TagBehavior.Revive:
					if (!this.inGamePlayerStates[num2].touchedLiquid && this.inGamePlayerStates[num].touchedLiquid)
					{
						ScienceExperimentManager.PlayerGameState playerGameState2 = this.inGamePlayerStates[num];
						playerGameState2.touchedLiquid = false;
						playerGameState2.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[num] = playerGameState2;
					}
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600753B RID: 30011 RVA: 0x0026208C File Offset: 0x0026028C
		private void OnColliderEnteredVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerEnteredGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x0600753C RID: 30012 RVA: 0x002620CC File Offset: 0x002602CC
		private void OnColliderExitedVolume(Collider collider)
		{
			VRRig component = collider.attachedRigidbody.gameObject.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				this.PlayerExitedGameArea(component.creator.ActorNumber);
			}
		}

		// Token: 0x0600753D RID: 30013 RVA: 0x0026210C File Offset: 0x0026030C
		private void OnColliderEnteredSoda(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedLava(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedLavaRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x0600753E RID: 30014 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnColliderExitedSoda(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x0600753F RID: 30015 RVA: 0x00262160 File Offset: 0x00260360
		private void OnColliderEnteredRefreshWater(WaterVolume volume, Collider collider)
		{
			if (collider == GTPlayer.Instance.bodyCollider)
			{
				if (base.IsMine)
				{
					this.PlayerTouchedRefreshWater(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerTouchedRefreshWaterRPC", RpcTarget.MasterClient, Array.Empty<object>());
			}
		}

		// Token: 0x06007540 RID: 30016 RVA: 0x00002C2D File Offset: 0x00000E2D
		private void OnColliderExitedRefreshWater(WaterVolume volume, Collider collider)
		{
		}

		// Token: 0x06007541 RID: 30017 RVA: 0x002621B3 File Offset: 0x002603B3
		private void OnProjectileEnteredSodaWater(SlingshotProjectile projectile, Collider collider)
		{
			if (projectile.gameObject.CompareTag(this.mentoProjectileTag))
			{
				this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			}
		}

		// Token: 0x06007542 RID: 30018 RVA: 0x002621DC File Offset: 0x002603DC
		private void AddLavaRock(int playerId)
		{
			if (base.IsMine && this.reliableState.state == ScienceExperimentManager.RisingLiquidState.Drained)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (!this.inGamePlayerStates[i].touchedLiquid)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
					this.reliableState.activationProgress = this.reliableState.activationProgress + (double)num;
				}
			}
		}

		// Token: 0x06007543 RID: 30019 RVA: 0x00262250 File Offset: 0x00260450
		public void OnWaterBalloonHitPlayer(NetPlayer hitPlayer)
		{
			bool flag = false;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == hitPlayer.ActorNumber)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (hitPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.ValidateLocalPlayerWaterBalloonHit(hitPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("ValidateLocalPlayerWaterBalloonHitRPC", RpcTarget.Others, new object[] { hitPlayer.ActorNumber });
			}
		}

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x06007544 RID: 30020 RVA: 0x002622CD File Offset: 0x002604CD
		// (set) Token: 0x06007545 RID: 30021 RVA: 0x002622F7 File Offset: 0x002604F7
		[Networked]
		[NetworkedWeaved(0, 76)]
		private unsafe ScienceExperimentManager.ScienceManagerData Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ScienceExperimentManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(ScienceExperimentManager.ScienceManagerData*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06007546 RID: 30022 RVA: 0x00262324 File Offset: 0x00260524
		public override void WriteDataFusion()
		{
			ScienceExperimentManager.ScienceManagerData scienceManagerData = new ScienceExperimentManager.ScienceManagerData((int)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.stateStartLiquidProgressLinear, this.reliableState.activationProgress, (int)this.nextRoundRiseSpeed, this.riseTime, this.lastWinnerId, this.inGamePlayerCount, this.inGamePlayerStates, this.rotatingRings);
			this.Data = scienceManagerData;
		}

		// Token: 0x06007547 RID: 30023 RVA: 0x00262390 File Offset: 0x00260590
		public override void ReadDataFusion()
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)this.Data.reliableState;
			this.reliableState.stateStartTime = this.Data.stateStartTime;
			this.reliableState.stateStartLiquidProgressLinear = this.Data.stateStartLiquidProgressLinear.ClampSafe(0f, 1f);
			this.reliableState.activationProgress = this.Data.activationProgress.GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)this.Data.nextRoundRiseSpeed;
			this.riseTime = this.Data.riseTime.GetFinite();
			this.lastWinnerId = this.Data.lastWinnerId;
			this.inGamePlayerCount = Mathf.Clamp(this.Data.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = this.Data.playerIdArray[i];
				this.inGamePlayerStates[i].touchedLiquid = this.Data.touchedLiquidArray[i];
				this.inGamePlayerStates[i].touchedLiquidAtProgress = this.Data.touchedLiquidAtProgressArray[i].ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = this.Data.initialAngleArray[j].GetFinite();
				this.rotatingRings[j].resultingAngle = this.Data.resultingAngleArray[j].GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x06007548 RID: 30024 RVA: 0x002625B8 File Offset: 0x002607B8
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext((int)this.reliableState.state);
			stream.SendNext(this.reliableState.stateStartTime);
			stream.SendNext(this.reliableState.stateStartLiquidProgressLinear);
			stream.SendNext(this.reliableState.activationProgress);
			stream.SendNext((int)this.nextRoundRiseSpeed);
			stream.SendNext(this.riseTime);
			stream.SendNext(this.lastWinnerId);
			stream.SendNext(this.inGamePlayerCount);
			for (int i = 0; i < 10; i++)
			{
				stream.SendNext(this.inGamePlayerStates[i].playerId);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquid);
				stream.SendNext(this.inGamePlayerStates[i].touchedLiquidAtProgress);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				stream.SendNext(this.rotatingRings[j].initialAngle);
				stream.SendNext(this.rotatingRings[j].resultingAngle);
			}
		}

		// Token: 0x06007549 RID: 30025 RVA: 0x00262710 File Offset: 0x00260910
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			int num = this.lastWinnerId;
			ScienceExperimentManager.RiseSpeed riseSpeed = this.nextRoundRiseSpeed;
			this.reliableState.state = (ScienceExperimentManager.RisingLiquidState)((int)stream.ReceiveNext());
			this.reliableState.stateStartTime = ((double)stream.ReceiveNext()).GetFinite();
			this.reliableState.stateStartLiquidProgressLinear = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.reliableState.activationProgress = ((double)stream.ReceiveNext()).GetFinite();
			this.nextRoundRiseSpeed = (ScienceExperimentManager.RiseSpeed)((int)stream.ReceiveNext());
			this.riseTime = ((float)stream.ReceiveNext()).GetFinite();
			this.lastWinnerId = (int)stream.ReceiveNext();
			this.inGamePlayerCount = (int)stream.ReceiveNext();
			this.inGamePlayerCount = Mathf.Clamp(this.inGamePlayerCount, 0, 10);
			for (int i = 0; i < 10; i++)
			{
				this.inGamePlayerStates[i].playerId = (int)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquid = (bool)stream.ReceiveNext();
				this.inGamePlayerStates[i].touchedLiquidAtProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			}
			for (int j = 0; j < this.rotatingRings.Length; j++)
			{
				this.rotatingRings[j].initialAngle = ((float)stream.ReceiveNext()).GetFinite();
				this.rotatingRings[j].resultingAngle = ((float)stream.ReceiveNext()).GetFinite();
			}
			float num2 = this.riseProgress;
			this.UpdateLocalState(NetworkSystem.Instance.SimTime, this.reliableState);
			this.localLagRiseProgressOffset = num2 - this.riseProgress;
			if (num != this.lastWinnerId)
			{
				this.RefreshWinnerName();
			}
		}

		// Token: 0x0600754A RID: 30026 RVA: 0x002628F8 File Offset: 0x00260AF8
		private void PlayerEnteredGameArea(int pId)
		{
			if (base.IsMine)
			{
				bool flag = false;
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == pId)
					{
						flag = true;
						break;
					}
				}
				if (!flag && this.inGamePlayerCount < 10)
				{
					bool flag2 = false;
					this.inGamePlayerStates[this.inGamePlayerCount] = new ScienceExperimentManager.PlayerGameState
					{
						playerId = pId,
						touchedLiquid = flag2,
						touchedLiquidAtProgress = -1f
					};
					this.inGamePlayerCount++;
					if (this.optPlayersOutOfRoomGameMode)
					{
						global::GorillaGameModes.GameMode.OptOut(pId);
					}
				}
			}
		}

		// Token: 0x0600754B RID: 30027 RVA: 0x0026299C File Offset: 0x00260B9C
		private void PlayerExitedGameArea(int playerId)
		{
			if (base.IsMine)
			{
				int i = 0;
				while (i < this.inGamePlayerCount)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
						this.inGamePlayerCount--;
						if (this.optPlayersOutOfRoomGameMode)
						{
							global::GorillaGameModes.GameMode.OptIn(playerId);
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}
		}

		// Token: 0x0600754C RID: 30028 RVA: 0x00262A12 File Offset: 0x00260C12
		[PunRPC]
		public void PlayerTouchedLavaRPC(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(info.Sender.ActorNumber);
		}

		// Token: 0x0600754D RID: 30029 RVA: 0x00262A30 File Offset: 0x00260C30
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		public unsafe void RPC_PlayerTouchedLava(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 1);
								int num2 = 8;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedLava(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			MonkeAgent.IncrementRPCCall(photonMessageInfoWrapped, "PlayerTouchedLavaRPC");
			this.PlayerTouchedLava(photonMessageInfoWrapped.Sender.ActorNumber);
		}

		// Token: 0x0600754E RID: 30030 RVA: 0x00262B94 File Offset: 0x00260D94
		private void PlayerTouchedLava(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						if (!playerGameState.touchedLiquid)
						{
							playerGameState.touchedLiquidAtProgress = this.riseProgressLinear;
						}
						playerGameState.touchedLiquid = true;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x0600754F RID: 30031 RVA: 0x00262C06 File Offset: 0x00260E06
		[PunRPC]
		private void PlayerTouchedRefreshWaterRPC(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(info.Sender.ActorNumber);
		}

		// Token: 0x06007550 RID: 30032 RVA: 0x00262C24 File Offset: 0x00260E24
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RPC_PlayerTouchedRefreshWater(RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 2);
								int num2 = 8;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerTouchedRefreshWater(Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
			MonkeAgent.IncrementRPCCall(photonMessageInfoWrapped, "PlayerTouchedRefreshWaterRPC");
			this.PlayerTouchedRefreshWater(photonMessageInfoWrapped.Sender.ActorNumber);
		}

		// Token: 0x06007551 RID: 30033 RVA: 0x00262D88 File Offset: 0x00260F88
		private void PlayerTouchedRefreshWater(int playerId)
		{
			if (base.IsMine && this.RefreshWaterAvailable)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06007552 RID: 30034 RVA: 0x00262DF9 File Offset: 0x00260FF9
		[PunRPC]
		private void ValidateLocalPlayerWaterBalloonHitRPC(int playerId, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "ValidateLocalPlayerWaterBalloonHitRPC");
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.ValidateLocalPlayerWaterBalloonHit(playerId);
			}
		}

		// Token: 0x06007553 RID: 30035 RVA: 0x00262E20 File Offset: 0x00261020
		[Rpc(InvokeLocal = false)]
		private unsafe void RPC_ValidateLocalPlayerWaterBalloonHit(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (this.InvokeRpc)
			{
				this.InvokeRpc = false;
				MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "ValidateLocalPlayerWaterBalloonHitRPC");
				if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.ValidateLocalPlayerWaterBalloonHit(playerId);
				}
				return;
			}
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (base.Runner.Stage != SimulationStages.Resimulate)
			{
				int localAuthorityMask = base.Object.GetLocalAuthorityMask();
				if ((localAuthorityMask & 7) == 0)
				{
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				else
				{
					int num = 8;
					num += 4;
					if (!SimulationMessage.CanAllocateUserPayload(num))
					{
						NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_ValidateLocalPlayerWaterBalloonHit(System.Int32,Fusion.RpcInfo)", num);
					}
					else if (base.Runner.HasAnyActiveConnections())
					{
						SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
						byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
						*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 3);
						int num2 = 8;
						*(int*)(ptr2 + num2) = playerId;
						num2 += 4;
						ptr->Offset = num2 * 8;
						base.Runner.SendRpc(ptr);
					}
				}
			}
		}

		// Token: 0x06007554 RID: 30036 RVA: 0x00262F7C File Offset: 0x0026117C
		private void ValidateLocalPlayerWaterBalloonHit(int playerId)
		{
			if (playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber && !GTPlayer.Instance.InWater)
			{
				if (base.IsMine)
				{
					this.PlayerHitByWaterBalloon(NetworkSystem.Instance.LocalPlayer.ActorNumber);
					return;
				}
				base.GetView.RPC("PlayerHitByWaterBalloonRPC", RpcTarget.MasterClient, new object[] { PhotonNetwork.LocalPlayer.ActorNumber });
			}
		}

		// Token: 0x06007555 RID: 30037 RVA: 0x00262FEE File Offset: 0x002611EE
		[PunRPC]
		private void PlayerHitByWaterBalloonRPC(int playerId, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x06007556 RID: 30038 RVA: 0x00263004 File Offset: 0x00261204
		[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
		private unsafe void RPC_PlayerHitByWaterBalloon(int playerId, RpcInfo info = default(RpcInfo))
		{
			if (!this.InvokeRpc)
			{
				NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
				if (base.Runner.Stage != SimulationStages.Resimulate)
				{
					int localAuthorityMask = base.Object.GetLocalAuthorityMask();
					if ((localAuthorityMask & 7) != 0)
					{
						if ((localAuthorityMask & 1) != 1)
						{
							int num = 8;
							num += 4;
							if (!SimulationMessage.CanAllocateUserPayload(num))
							{
								NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", num);
								return;
							}
							if (base.Runner.HasAnyActiveConnections())
							{
								SimulationMessage* ptr = SimulationMessage.Allocate(base.Runner.Simulation, num);
								byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
								*(RpcHeader*)ptr2 = RpcHeader.Create(base.Object.Id, this.ObjectIndex, 4);
								int num2 = 8;
								*(int*)(ptr2 + num2) = playerId;
								num2 += 4;
								ptr->Offset = num2 * 8;
								base.Runner.SendRpc(ptr);
							}
							if ((localAuthorityMask & 1) == 0)
							{
								return;
							}
						}
						info = RpcInfo.FromLocal(base.Runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_0012;
					}
					NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void GorillaTag.ScienceExperimentManager::RPC_PlayerHitByWaterBalloon(System.Int32,Fusion.RpcInfo)", base.Object, 7);
				}
				return;
			}
			this.InvokeRpc = false;
			IL_0012:
			MonkeAgent.IncrementRPCCall(new PhotonMessageInfoWrapped(info), "PlayerHitByWaterBalloonRPC");
			this.PlayerHitByWaterBalloon(playerId);
		}

		// Token: 0x06007557 RID: 30039 RVA: 0x0026317C File Offset: 0x0026137C
		private void PlayerHitByWaterBalloon(int playerId)
		{
			if (base.IsMine)
			{
				for (int i = 0; i < this.inGamePlayerCount; i++)
				{
					if (this.inGamePlayerStates[i].playerId == playerId)
					{
						ScienceExperimentManager.PlayerGameState playerGameState = this.inGamePlayerStates[i];
						playerGameState.touchedLiquid = false;
						playerGameState.touchedLiquidAtProgress = -1f;
						this.inGamePlayerStates[i] = playerGameState;
						return;
					}
				}
			}
		}

		// Token: 0x06007558 RID: 30040 RVA: 0x002631E5 File Offset: 0x002613E5
		public void OnPlayerLeftRoom(NetPlayer otherPlayer)
		{
			this.PlayerExitedGameArea(otherPlayer.ActorNumber);
		}

		// Token: 0x06007559 RID: 30041 RVA: 0x002631F4 File Offset: 0x002613F4
		public void OnLeftRoom()
		{
			this.inGamePlayerCount = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (this.inGamePlayerStates[i].playerId == NetworkSystem.Instance.LocalPlayer.ActorNumber)
				{
					this.inGamePlayerStates[0] = this.inGamePlayerStates[i];
					this.inGamePlayerCount = 1;
					return;
				}
			}
		}

		// Token: 0x0600755A RID: 30042 RVA: 0x0026325C File Offset: 0x0026145C
		protected override void OnOwnerSwitched(NetPlayer newOwningPlayer)
		{
			base.OnOwnerSwitched(newOwningPlayer);
			if (!NetworkSystem.Instance.IsMasterClient)
			{
				return;
			}
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!Utils.PlayerInRoom(this.inGamePlayerStates[i].playerId))
				{
					this.inGamePlayerStates[i] = this.inGamePlayerStates[this.inGamePlayerCount - 1];
					this.inGamePlayerCount--;
					i--;
				}
			}
		}

		// Token: 0x0600755C RID: 30044 RVA: 0x002634D8 File Offset: 0x002616D8
		[CompilerGenerated]
		private int <UpdateReliableState>g__GetAlivePlayerCount|105_0()
		{
			int num = 0;
			for (int i = 0; i < this.inGamePlayerCount; i++)
			{
				if (!this.inGamePlayerStates[i].touchedLiquid)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x0600755D RID: 30045 RVA: 0x00263510 File Offset: 0x00261710
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x0600755E RID: 30046 RVA: 0x00263528 File Offset: 0x00261728
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x0600755F RID: 30047 RVA: 0x0026353C File Offset: 0x0026173C
		[NetworkRpcWeavedInvoker(1, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedLava@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedLava(rpcInfo);
		}

		// Token: 0x06007560 RID: 30048 RVA: 0x00263580 File Offset: 0x00261780
		[NetworkRpcWeavedInvoker(2, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerTouchedRefreshWater@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerTouchedRefreshWater(rpcInfo);
		}

		// Token: 0x06007561 RID: 30049 RVA: 0x002635C4 File Offset: 0x002617C4
		[NetworkRpcWeavedInvoker(3, 7, 7)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_ValidateLocalPlayerWaterBalloonHit@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int num3 = num2;
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_ValidateLocalPlayerWaterBalloonHit(num3, rpcInfo);
		}

		// Token: 0x06007562 RID: 30050 RVA: 0x00263624 File Offset: 0x00261824
		[NetworkRpcWeavedInvoker(4, 7, 1)]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerHitByWaterBalloon@Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int num3 = num2;
			RpcInfo rpcInfo = RpcInfo.FromMessage(behaviour.Runner, message, RpcHostMode.SourceIsServer);
			behaviour.InvokeRpc = true;
			((ScienceExperimentManager)behaviour).RPC_PlayerHitByWaterBalloon(num3, rpcInfo);
		}

		// Token: 0x040084CE RID: 33998
		public static volatile ScienceExperimentManager instance;

		// Token: 0x040084CF RID: 33999
		[SerializeField]
		private ScienceExperimentManager.TagBehavior tagBehavior = ScienceExperimentManager.TagBehavior.Infect;

		// Token: 0x040084D0 RID: 34000
		[SerializeField]
		private float minScale = 1f;

		// Token: 0x040084D1 RID: 34001
		[SerializeField]
		private float maxScale = 10f;

		// Token: 0x040084D2 RID: 34002
		[SerializeField]
		private float riseTimeFast = 30f;

		// Token: 0x040084D3 RID: 34003
		[SerializeField]
		private float riseTimeMedium = 60f;

		// Token: 0x040084D4 RID: 34004
		[SerializeField]
		private float riseTimeSlow = 120f;

		// Token: 0x040084D5 RID: 34005
		[SerializeField]
		private float riseTimeExtraSlow = 240f;

		// Token: 0x040084D6 RID: 34006
		[SerializeField]
		private float preDrainWaitTime = 3f;

		// Token: 0x040084D7 RID: 34007
		[SerializeField]
		private float maxFullTime = 5f;

		// Token: 0x040084D8 RID: 34008
		[SerializeField]
		private float drainTime = 10f;

		// Token: 0x040084D9 RID: 34009
		[SerializeField]
		private float fullyDrainedWaitTime = 3f;

		// Token: 0x040084DA RID: 34010
		[SerializeField]
		private float lagResolutionLavaProgressPerSecond = 0.2f;

		// Token: 0x040084DB RID: 34011
		[SerializeField]
		private AnimationCurve animationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040084DC RID: 34012
		[SerializeField]
		private float lavaProgressToDisableRefreshWater = 0.18f;

		// Token: 0x040084DD RID: 34013
		[SerializeField]
		private float lavaProgressToEnableRefreshWater = 0.08f;

		// Token: 0x040084DE RID: 34014
		[SerializeField]
		private float entryLiquidMaxScale = 5f;

		// Token: 0x040084DF RID: 34015
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningTop = Vector2.zero;

		// Token: 0x040084E0 RID: 34016
		[SerializeField]
		private Vector2 entryLiquidScaleSyncOpeningBottom = Vector2.zero;

		// Token: 0x040084E1 RID: 34017
		[SerializeField]
		private float entryBridgeQuadMaxScaleY = 0.0915f;

		// Token: 0x040084E2 RID: 34018
		[SerializeField]
		private Vector2 entryBridgeQuadMinMaxZHeight = new Vector2(0.245f, 0.337f);

		// Token: 0x040084E3 RID: 34019
		[SerializeField]
		private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040084E4 RID: 34020
		[SerializeField]
		private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		// Token: 0x040084E5 RID: 34021
		[SerializeField]
		public GameObject waterBalloonPrefab;

		// Token: 0x040084E6 RID: 34022
		[SerializeField]
		private Vector2 rotatingRingRandomAngleRange = Vector2.zero;

		// Token: 0x040084E7 RID: 34023
		[SerializeField]
		private bool rotatingRingQuantizeAngles;

		// Token: 0x040084E8 RID: 34024
		[SerializeField]
		private float rotatingRingAngleSnapDegrees = 9f;

		// Token: 0x040084E9 RID: 34025
		[SerializeField]
		private float drainBlockerSlideTime = 4f;

		// Token: 0x040084EA RID: 34026
		[SerializeField]
		private Vector2 sodaFizzParticleEmissionMinMax = new Vector2(30f, 100f);

		// Token: 0x040084EB RID: 34027
		[SerializeField]
		private float infrequentUpdatePeriod = 3f;

		// Token: 0x040084EC RID: 34028
		[SerializeField]
		private bool optPlayersOutOfRoomGameMode;

		// Token: 0x040084ED RID: 34029
		[SerializeField]
		private bool debugDrawPlayerGameState;

		// Token: 0x040084EE RID: 34030
		private ScienceExperimentSceneElements elements;

		// Token: 0x040084EF RID: 34031
		private NetPlayer[] allPlayersInRoom;

		// Token: 0x040084F0 RID: 34032
		private ScienceExperimentManager.RotatingRingState[] rotatingRings = new ScienceExperimentManager.RotatingRingState[0];

		// Token: 0x040084F1 RID: 34033
		private const int maxPlayerCount = 10;

		// Token: 0x040084F2 RID: 34034
		private ScienceExperimentManager.PlayerGameState[] inGamePlayerStates = new ScienceExperimentManager.PlayerGameState[10];

		// Token: 0x040084F3 RID: 34035
		private int inGamePlayerCount;

		// Token: 0x040084F4 RID: 34036
		private int lastWinnerId = -1;

		// Token: 0x040084F5 RID: 34037
		private string lastWinnerName = "None";

		// Token: 0x040084F6 RID: 34038
		private List<ScienceExperimentManager.PlayerGameState> sortedPlayerStates = new List<ScienceExperimentManager.PlayerGameState>();

		// Token: 0x040084F7 RID: 34039
		private ScienceExperimentManager.SyncData reliableState;

		// Token: 0x040084F8 RID: 34040
		private ScienceExperimentManager.RiseSpeed nextRoundRiseSpeed = ScienceExperimentManager.RiseSpeed.Slow;

		// Token: 0x040084F9 RID: 34041
		private float riseTime = 120f;

		// Token: 0x040084FA RID: 34042
		private float riseProgress;

		// Token: 0x040084FB RID: 34043
		private float riseProgressLinear;

		// Token: 0x040084FC RID: 34044
		private float localLagRiseProgressOffset;

		// Token: 0x040084FD RID: 34045
		private double lastInfrequentUpdateTime = -10.0;

		// Token: 0x040084FE RID: 34046
		private string mentoProjectileTag = "ScienceCandyProjectile";

		// Token: 0x040084FF RID: 34047
		private double currentTime;

		// Token: 0x04008500 RID: 34048
		private double prevTime;

		// Token: 0x04008501 RID: 34049
		private float ringRotationProgress = 1f;

		// Token: 0x04008502 RID: 34050
		private float drainBlockerSlideSpeed;

		// Token: 0x04008503 RID: 34051
		private float[] riseTimeLookup;

		// Token: 0x04008504 RID: 34052
		[Header("Scene References")]
		public Transform ringParent;

		// Token: 0x04008505 RID: 34053
		public Transform liquidMeshTransform;

		// Token: 0x04008506 RID: 34054
		public Transform liquidSurfacePlane;

		// Token: 0x04008507 RID: 34055
		public Transform entryWayLiquidMeshTransform;

		// Token: 0x04008508 RID: 34056
		public Transform entryWayBridgeQuadTransform;

		// Token: 0x04008509 RID: 34057
		public Transform drainBlocker;

		// Token: 0x0400850A RID: 34058
		public Transform drainBlockerClosedPosition;

		// Token: 0x0400850B RID: 34059
		public Transform drainBlockerOpenPosition;

		// Token: 0x0400850C RID: 34060
		public WaterVolume liquidVolume;

		// Token: 0x0400850D RID: 34061
		public WaterVolume entryLiquidVolume;

		// Token: 0x0400850E RID: 34062
		public WaterVolume bottleLiquidVolume;

		// Token: 0x0400850F RID: 34063
		public WaterVolume refreshWaterVolume;

		// Token: 0x04008510 RID: 34064
		public CompositeTriggerEvents gameAreaTriggerNotifier;

		// Token: 0x04008511 RID: 34065
		public SlingshotProjectileHitNotifier sodaWaterProjectileTriggerNotifier;

		// Token: 0x04008512 RID: 34066
		public AudioSource eruptionAudioSource;

		// Token: 0x04008513 RID: 34067
		public AudioSource drainAudioSource;

		// Token: 0x04008514 RID: 34068
		public AudioSource rotatingRingsAudioSource;

		// Token: 0x04008515 RID: 34069
		private ParticleSystem.EmissionModule fizzParticleEmission;

		// Token: 0x04008516 RID: 34070
		private bool hasPlayedEruptionEffects;

		// Token: 0x04008517 RID: 34071
		private bool hasPlayedDrainEffects;

		// Token: 0x04008519 RID: 34073
		[SerializeField]
		private float debugRotateRingsTime = 10f;

		// Token: 0x0400851A RID: 34074
		private Coroutine rotateRingsCoroutine;

		// Token: 0x0400851B RID: 34075
		private bool debugRandomizingRings;

		// Token: 0x0400851C RID: 34076
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 76)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private ScienceExperimentManager.ScienceManagerData _Data;

		// Token: 0x0200120F RID: 4623
		public enum RisingLiquidState
		{
			// Token: 0x0400851E RID: 34078
			Drained,
			// Token: 0x0400851F RID: 34079
			Erupting,
			// Token: 0x04008520 RID: 34080
			Rising,
			// Token: 0x04008521 RID: 34081
			Full,
			// Token: 0x04008522 RID: 34082
			PreDrainDelay,
			// Token: 0x04008523 RID: 34083
			Draining
		}

		// Token: 0x02001210 RID: 4624
		private enum RiseSpeed
		{
			// Token: 0x04008525 RID: 34085
			Fast,
			// Token: 0x04008526 RID: 34086
			Medium,
			// Token: 0x04008527 RID: 34087
			Slow,
			// Token: 0x04008528 RID: 34088
			ExtraSlow
		}

		// Token: 0x02001211 RID: 4625
		private enum TagBehavior
		{
			// Token: 0x0400852A RID: 34090
			None,
			// Token: 0x0400852B RID: 34091
			Infect,
			// Token: 0x0400852C RID: 34092
			Revive
		}

		// Token: 0x02001212 RID: 4626
		[Serializable]
		public struct PlayerGameState
		{
			// Token: 0x0400852D RID: 34093
			public int playerId;

			// Token: 0x0400852E RID: 34094
			public bool touchedLiquid;

			// Token: 0x0400852F RID: 34095
			public float touchedLiquidAtProgress;
		}

		// Token: 0x02001213 RID: 4627
		private struct SyncData
		{
			// Token: 0x04008530 RID: 34096
			public ScienceExperimentManager.RisingLiquidState state;

			// Token: 0x04008531 RID: 34097
			public double stateStartTime;

			// Token: 0x04008532 RID: 34098
			public float stateStartLiquidProgressLinear;

			// Token: 0x04008533 RID: 34099
			public double activationProgress;
		}

		// Token: 0x02001214 RID: 4628
		private struct RotatingRingState
		{
			// Token: 0x04008534 RID: 34100
			public Transform ringTransform;

			// Token: 0x04008535 RID: 34101
			public float initialAngle;

			// Token: 0x04008536 RID: 34102
			public float resultingAngle;
		}

		// Token: 0x02001215 RID: 4629
		[Serializable]
		private struct DisableByLiquidData
		{
			// Token: 0x04008537 RID: 34103
			public Transform target;

			// Token: 0x04008538 RID: 34104
			public float heightOffset;
		}

		// Token: 0x02001216 RID: 4630
		[NetworkStructWeaved(76)]
		[StructLayout(LayoutKind.Explicit, Size = 304)]
		private struct ScienceManagerData : INetworkStruct
		{
			// Token: 0x17000B5E RID: 2910
			// (get) Token: 0x06007563 RID: 30051 RVA: 0x00263684 File Offset: 0x00261884
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterInt32))]
			[NetworkedWeaved(10, 10)]
			public NetworkArray<int> playerIdArray
			{
				get
				{
					return new NetworkArray<int>(Native.ReferenceToPointer<FixedStorage@10>(ref this._playerIdArray), 10, ElementReaderWriterInt32.GetInstance());
				}
			}

			// Token: 0x17000B5F RID: 2911
			// (get) Token: 0x06007564 RID: 30052 RVA: 0x002636AC File Offset: 0x002618AC
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterBoolean))]
			[NetworkedWeaved(20, 10)]
			public NetworkArray<bool> touchedLiquidArray
			{
				get
				{
					return new NetworkArray<bool>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidArray), 10, ElementReaderWriterBoolean.GetInstance());
				}
			}

			// Token: 0x17000B60 RID: 2912
			// (get) Token: 0x06007565 RID: 30053 RVA: 0x002636D4 File Offset: 0x002618D4
			[Networked]
			[Capacity(10)]
			[NetworkedWeavedArray(10, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(30, 10)]
			public NetworkArray<float> touchedLiquidAtProgressArray
			{
				get
				{
					return new NetworkArray<float>(Native.ReferenceToPointer<FixedStorage@10>(ref this._touchedLiquidAtProgressArray), 10, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x17000B61 RID: 2913
			// (get) Token: 0x06007566 RID: 30054 RVA: 0x002636FC File Offset: 0x002618FC
			[Networked]
			[Capacity(5)]
			[NetworkedWeavedLinkedList(5, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(40, 18)]
			public NetworkLinkedList<float> initialAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._initialAngleArray), 5, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x17000B62 RID: 2914
			// (get) Token: 0x06007567 RID: 30055 RVA: 0x00263720 File Offset: 0x00261920
			[Networked]
			[Capacity(5)]
			[NetworkedWeavedLinkedList(5, 1, typeof(ElementReaderWriterSingle))]
			[NetworkedWeaved(58, 18)]
			public NetworkLinkedList<float> resultingAngleArray
			{
				get
				{
					return new NetworkLinkedList<float>(Native.ReferenceToPointer<FixedStorage@18>(ref this._resultingAngleArray), 5, ElementReaderWriterSingle.GetInstance());
				}
			}

			// Token: 0x06007568 RID: 30056 RVA: 0x00263744 File Offset: 0x00261944
			public ScienceManagerData(int reliableState, double stateStartTime, float stateStartLiquidProgressLinear, double activationProgress, int nextRoundRiseSpeed, float riseTime, int lastWinnerId, int inGamePlayerCount, ScienceExperimentManager.PlayerGameState[] playerStates, ScienceExperimentManager.RotatingRingState[] rings)
			{
				this.reliableState = reliableState;
				this.stateStartTime = stateStartTime;
				this.stateStartLiquidProgressLinear = stateStartLiquidProgressLinear;
				this.activationProgress = activationProgress;
				this.nextRoundRiseSpeed = nextRoundRiseSpeed;
				this.riseTime = riseTime;
				this.lastWinnerId = lastWinnerId;
				this.inGamePlayerCount = inGamePlayerCount;
				foreach (ScienceExperimentManager.RotatingRingState rotatingRingState in rings)
				{
					this.initialAngleArray.Add(rotatingRingState.initialAngle);
					this.resultingAngleArray.Add(rotatingRingState.resultingAngle);
				}
				int[] array = new int[10];
				bool[] array2 = new bool[10];
				float[] array3 = new float[10];
				for (int j = 0; j < 10; j++)
				{
					array[j] = playerStates[j].playerId;
					array2[j] = playerStates[j].touchedLiquid;
					array3[j] = playerStates[j].touchedLiquidAtProgress;
				}
				this.playerIdArray.CopyFrom(array, 0, array.Length);
				this.touchedLiquidArray.CopyFrom(array2, 0, array2.Length);
				this.touchedLiquidAtProgressArray.CopyFrom(array3, 0, array3.Length);
			}

			// Token: 0x04008539 RID: 34105
			[FieldOffset(0)]
			public int reliableState;

			// Token: 0x0400853A RID: 34106
			[FieldOffset(4)]
			public double stateStartTime;

			// Token: 0x0400853B RID: 34107
			[FieldOffset(12)]
			public float stateStartLiquidProgressLinear;

			// Token: 0x0400853C RID: 34108
			[FieldOffset(16)]
			public double activationProgress;

			// Token: 0x0400853D RID: 34109
			[FieldOffset(24)]
			public int nextRoundRiseSpeed;

			// Token: 0x0400853E RID: 34110
			[FieldOffset(28)]
			public float riseTime;

			// Token: 0x0400853F RID: 34111
			[FieldOffset(32)]
			public int lastWinnerId;

			// Token: 0x04008540 RID: 34112
			[FieldOffset(36)]
			public int inGamePlayerCount;

			// Token: 0x04008541 RID: 34113
			[FixedBufferProperty(typeof(NetworkArray<int>), typeof(UnityArraySurrogate@ElementReaderWriterInt32), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(40)]
			private FixedStorage@10 _playerIdArray;

			// Token: 0x04008542 RID: 34114
			[FixedBufferProperty(typeof(NetworkArray<bool>), typeof(UnityArraySurrogate@ElementReaderWriterBoolean), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(80)]
			private FixedStorage@10 _touchedLiquidArray;

			// Token: 0x04008543 RID: 34115
			[FixedBufferProperty(typeof(NetworkArray<float>), typeof(UnityArraySurrogate@ElementReaderWriterSingle), 10, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(120)]
			private FixedStorage@10 _touchedLiquidAtProgressArray;

			// Token: 0x04008544 RID: 34116
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ElementReaderWriterSingle), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(160)]
			private FixedStorage@18 _initialAngleArray;

			// Token: 0x04008545 RID: 34117
			[FixedBufferProperty(typeof(NetworkLinkedList<float>), typeof(UnityLinkedListSurrogate@ElementReaderWriterSingle), 5, order = -2147483647)]
			[WeaverGenerated]
			[SerializeField]
			[FieldOffset(232)]
			private FixedStorage@18 _resultingAngleArray;
		}
	}
}
