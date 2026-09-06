using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Swimming;
using GorillaTag;
using GorillaTag.Rendering;
using UnityEngine;

// Token: 0x020009DF RID: 2527
public class InfectionLavaController : MonoBehaviour, ITickSystemPost
{
	// Token: 0x17000617 RID: 1559
	// (get) Token: 0x060040D1 RID: 16593 RVA: 0x00159105 File Offset: 0x00157305
	public static IReadOnlyList<InfectionLavaController> ActiveControllers
	{
		get
		{
			return InfectionLavaController.activeControllers;
		}
	}

	// Token: 0x060040D2 RID: 16594 RVA: 0x0015910C File Offset: 0x0015730C
	public static InfectionLavaController GetControllerForZone(GTZone zone)
	{
		for (int i = 0; i < InfectionLavaController.activeControllers.Count; i++)
		{
			if (InfectionLavaController.activeControllers[i].zone == zone)
			{
				return InfectionLavaController.activeControllers[i];
			}
		}
		return null;
	}

	// Token: 0x17000618 RID: 1560
	// (get) Token: 0x060040D3 RID: 16595 RVA: 0x0015914E File Offset: 0x0015734E
	public GTZone Zone
	{
		get
		{
			return this.zone;
		}
	}

	// Token: 0x17000619 RID: 1561
	// (get) Token: 0x060040D4 RID: 16596 RVA: 0x00159158 File Offset: 0x00157358
	private bool IsAuthority
	{
		get
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return true;
			}
			int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
			if (zoneAuthorityActorNumber == 2147483647)
			{
				return RoomSystem.AmITheHost;
			}
			return zoneAuthorityActorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x1700061A RID: 1562
	// (get) Token: 0x060040D5 RID: 16597 RVA: 0x0015919A File Offset: 0x0015739A
	public bool LavaCurrentlyActivated
	{
		get
		{
			return this.reliableState.state > InfectionLavaController.RisingLavaState.Drained;
		}
	}

	// Token: 0x1700061B RID: 1563
	// (get) Token: 0x060040D6 RID: 16598 RVA: 0x001591AA File Offset: 0x001573AA
	public Plane LavaPlane
	{
		get
		{
			return new Plane(this.lavaSurfacePlaneTransform.up, this.lavaSurfacePlaneTransform.position);
		}
	}

	// Token: 0x1700061C RID: 1564
	// (get) Token: 0x060040D7 RID: 16599 RVA: 0x001591C7 File Offset: 0x001573C7
	public Vector3 SurfaceCenter
	{
		get
		{
			return this.lavaSurfacePlaneTransform.position;
		}
	}

	// Token: 0x1700061D RID: 1565
	// (get) Token: 0x060040D8 RID: 16600 RVA: 0x001591D4 File Offset: 0x001573D4
	private int PlayerCount
	{
		get
		{
			int num = 1;
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null && instance.currentNetPlayerArray != null)
			{
				num = instance.currentNetPlayerArray.Length;
			}
			return num;
		}
	}

	// Token: 0x1700061E RID: 1566
	// (get) Token: 0x060040D9 RID: 16601 RVA: 0x00159204 File Offset: 0x00157404
	private bool InCompetitiveQueue
	{
		get
		{
			return NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString.Contains("COMPETITIVE");
		}
	}

	// Token: 0x060040DA RID: 16602 RVA: 0x00159228 File Offset: 0x00157428
	private void Awake()
	{
		this.lavaActivationMPB = new MaterialPropertyBlock();
		RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Combine(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	// Token: 0x060040DB RID: 16603 RVA: 0x001592B4 File Offset: 0x001574B4
	protected void OnEnable()
	{
		InfectionLavaController.activeControllers.Add(this);
		this.VerifyReferences();
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			if (this.volcanoEffects[i] != null)
			{
				this.volcanoEffects[i].PreloadAssets();
			}
		}
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater += this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit += this.OnActivationLavaProjectileHit;
		}
		if (this.localPlayerInZone && this.lavaZoneShaderSettings != null && this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			this.lavaZoneShaderSettings.BecomeActiveInstance(false);
		}
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x060040DC RID: 16604 RVA: 0x00159384 File Offset: 0x00157584
	protected void OnDisable()
	{
		InfectionLavaController.activeControllers.Remove(this);
		TickSystem<object>.RemovePostTickCallback(this);
		if (this.lavaVolume != null)
		{
			this.lavaVolume.ColliderEnteredWater -= this.OnColliderEnteredLava;
		}
		if (this.lavaActivationProjectileHitNotifier != null)
		{
			this.lavaActivationProjectileHitNotifier.OnProjectileHit -= this.OnActivationLavaProjectileHit;
		}
		this.ResetLavaState();
	}

	// Token: 0x060040DD RID: 16605 RVA: 0x001593F4 File Offset: 0x001575F4
	private void VerifyReferences()
	{
		this.IfNullThenLogAndDisableSelf(this.lavaMeshTransform, "lavaMeshTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaSurfacePlaneTransform, "lavaSurfacePlaneTransform", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaVolume, "lavaVolume", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationRenderer, "lavaActivationRenderer", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationStartPos, "lavaActivationStartPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationEndPos, "lavaActivationEndPos", -1);
		this.IfNullThenLogAndDisableSelf(this.lavaActivationProjectileHitNotifier, "lavaActivationProjectileHitNotifier", -1);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.IfNullThenLogAndDisableSelf(this.volcanoEffects[i], "volcanoEffects", i);
		}
	}

	// Token: 0x060040DE RID: 16606 RVA: 0x001594A8 File Offset: 0x001576A8
	private void IfNullThenLogAndDisableSelf(Object obj, string fieldName, int index = -1)
	{
		if (obj != null)
		{
			return;
		}
		fieldName = ((index != -1) ? string.Format("{0}[{1}]", fieldName, index) : fieldName);
		Debug.LogError("InfectionLavaController: Disabling self because reference `" + fieldName + "` is null.", this);
		base.enabled = false;
	}

	// Token: 0x060040DF RID: 16607 RVA: 0x001594F8 File Offset: 0x001576F8
	private void OnDestroy()
	{
		RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
		RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
		RoomSystem.OnLavaSyncReceived = (Action<RoomSystem.LavaSyncEventData>)Delegate.Remove(RoomSystem.OnLavaSyncReceived, new Action<RoomSystem.LavaSyncEventData>(this.OnLavaSyncReceived));
	}

	// Token: 0x060040E0 RID: 16608 RVA: 0x00159578 File Offset: 0x00157778
	private void ResetLavaState()
	{
		this.reliableState = default(InfectionLavaController.LavaSyncData);
		this.lavaProgressLinear = 0f;
		this.lavaProgressSmooth = 0f;
		this.localLagLavaProgressOffset = 0f;
		this.activationProgessSmooth = 0f;
		this.currentTime = 0.0;
		this.prevTime = 0.0;
		this.lastSyncSendTime = 0.0;
		this.residuePlaneY = this.GetMinLavaY();
		Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
		{
			this.lavaActivationVotePlayerIds[i] = 0;
		}
		this.lavaActivationVoteCount = 0;
		for (int j = 0; j < this.volcanoEffects.Length; j++)
		{
			VolcanoEffects volcanoEffects = this.volcanoEffects[j];
			if (volcanoEffects != null)
			{
				volcanoEffects.SetDrainedState();
			}
		}
		this.UpdateLava(0f);
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.localPlayerInZone && this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
	}

	// Token: 0x1700061F RID: 1567
	// (get) Token: 0x060040E1 RID: 16609 RVA: 0x00159680 File Offset: 0x00157880
	// (set) Token: 0x060040E2 RID: 16610 RVA: 0x00159688 File Offset: 0x00157888
	bool ITickSystemPost.PostTickRunning { get; set; }

	// Token: 0x060040E3 RID: 16611 RVA: 0x00159694 File Offset: 0x00157894
	void ITickSystemPost.PostTick()
	{
		this.prevTime = this.currentTime;
		this.currentTime = (NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble);
		bool flag = this.localPlayerInZone;
		this.localPlayerInZone = this.CheckLocalPlayerInZone();
		if (this.IsAuthority)
		{
			InfectionLavaController.RisingLavaState state = this.reliableState.state;
			this.UpdateReliableState(this.currentTime, ref this.reliableState);
			bool flag2 = this.reliableState.state != state;
			bool flag3 = this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.currentTime - this.lastSyncSendTime > 2.0;
			if (flag2 || flag3)
			{
				this.SendSyncEvent();
			}
		}
		else
		{
			this.AdvanceLavaPhaseByTime(this.currentTime, ref this.reliableState);
			this.DrainActivationProgressLocally();
		}
		this.UpdateLocalState(this.currentTime, this.reliableState);
		if (this.localPlayerInZone && !flag)
		{
			if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
			{
				if (this.lavaZoneShaderSettings != null)
				{
					this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				}
			}
			else if (this.baseZoneShaderSettings != null)
			{
				this.baseZoneShaderSettings.BecomeActiveInstance(false);
			}
		}
		else if (!this.localPlayerInZone && flag)
		{
			ZoneShaderSettings.ActivateDefaultSettings();
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, Vector4.zero);
		}
		this.localLagLavaProgressOffset = Mathf.MoveTowards(this.localLagLavaProgressOffset, 0f, this.lagResolutionLavaProgressPerSecond * Time.deltaTime);
		this.UpdateLava(this.lavaProgressSmooth + this.localLagLavaProgressOffset);
		this.UpdateResidueState();
		this.UpdateVolcanoActivationLava(this.reliableState.activationProgress);
		this.CheckLocalPlayerAgainstLava(this.currentTime);
	}

	// Token: 0x060040E4 RID: 16612 RVA: 0x00159844 File Offset: 0x00157A44
	private void JumpToState(InfectionLavaController.RisingLavaState state)
	{
		this.reliableState.state = state;
		switch (state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
		{
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.SetDrainedState();
				}
			}
			if (this.localPlayerInZone)
			{
				ZoneShaderSettings.ActivateDefaultSettings();
				if (this.baseZoneShaderSettings != null)
				{
					this.baseZoneShaderSettings.BecomeActiveInstance(false);
					return;
				}
			}
			break;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		{
			for (int j = 0; j < this.volcanoEffects.Length; j++)
			{
				VolcanoEffects volcanoEffects2 = this.volcanoEffects[j];
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.SetEruptingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		}
		case InfectionLavaController.RisingLavaState.Rising:
		{
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int k = 0; k < this.volcanoEffects.Length; k++)
			{
				VolcanoEffects volcanoEffects3 = this.volcanoEffects[k];
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.SetRisingState();
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Full:
		{
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			for (int l = 0; l < this.volcanoEffects.Length; l++)
			{
				VolcanoEffects volcanoEffects4 = this.volcanoEffects[l];
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.SetFullState();
				}
			}
			break;
		}
		case InfectionLavaController.RisingLavaState.Draining:
		{
			for (int m = 0; m < this.volcanoEffects.Length; m++)
			{
				VolcanoEffects volcanoEffects5 = this.volcanoEffects[m];
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.SetDrainingState();
				}
			}
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
				return;
			}
			break;
		}
		default:
			return;
		}
	}

	// Token: 0x060040E5 RID: 16613 RVA: 0x00159A00 File Offset: 0x00157C00
	private void UpdateReliableState(double currentTime, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - currentTime > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = currentTime;
		}
		switch (syncData.state)
		{
		default:
			if (syncData.activationProgress > 1f)
			{
				float playerCount = (float)this.PlayerCount;
				float num = (this.InCompetitiveQueue ? this.activationVotePercentageCompetitiveQueue : this.activationVotePercentageDefaultQueue);
				int num2 = Mathf.RoundToInt(playerCount * num);
				if (this.lavaActivationVoteCount >= num2)
				{
					for (int i = 0; i < this.lavaActivationVoteCount; i++)
					{
						this.lavaActivationVotePlayerIds[i] = 0;
					}
					this.lavaActivationVoteCount = 0;
					syncData.stateStartTime = currentTime + (double)this.latencyBuffer;
					syncData.activationProgress = 1f;
					this.JumpToState(InfectionLavaController.RisingLavaState.Erupting);
					return;
				}
			}
			else
			{
				float num3 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
				float activationProgress = syncData.activationProgress;
				syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num3);
				if (activationProgress > 0f && syncData.activationProgress <= 1E-45f)
				{
					VolcanoEffects[] array = this.volcanoEffects;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].OnVolcanoBellyEmpty();
					}
					return;
				}
			}
			break;
		case InfectionLavaController.RisingLavaState.Erupting:
			if (currentTime > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (currentTime > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (currentTime > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num4 = Mathf.Clamp((float)(currentTime - this.prevTime), 0f, 0.1f);
			syncData.activationProgress = Mathf.MoveTowards(syncData.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num4);
			if (currentTime > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		}
		}
	}

	// Token: 0x060040E6 RID: 16614 RVA: 0x00159C58 File Offset: 0x00157E58
	private void AdvanceLavaPhaseByTime(double time, ref InfectionLavaController.LavaSyncData syncData)
	{
		if (syncData.stateStartTime - time > (double)this.latencyBuffer + 1.0)
		{
			syncData.stateStartTime = time;
		}
		switch (syncData.state)
		{
		case InfectionLavaController.RisingLavaState.Erupting:
			if (time > syncData.stateStartTime + (double)this.eruptTime)
			{
				syncData.stateStartTime += (double)this.eruptTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Rising);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Rising:
			if (time > syncData.stateStartTime + (double)this.riseTime)
			{
				syncData.stateStartTime += (double)this.riseTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Full);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Full:
			if (time > syncData.stateStartTime + (double)this.fullTime)
			{
				syncData.stateStartTime += (double)this.fullTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Draining);
				return;
			}
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			if (time > syncData.stateStartTime + (double)this.drainTime)
			{
				syncData.stateStartTime += (double)this.drainTime;
				this.JumpToState(InfectionLavaController.RisingLavaState.Drained);
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060040E7 RID: 16615 RVA: 0x00159D54 File Offset: 0x00157F54
	private void DrainActivationProgressLocally()
	{
		if (this.reliableState.activationProgress <= 0f)
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained && this.reliableState.state != InfectionLavaController.RisingLavaState.Draining)
		{
			return;
		}
		float num = Mathf.Clamp((float)(this.currentTime - this.prevTime), 0f, 0.1f);
		float activationProgress = this.reliableState.activationProgress;
		this.reliableState.activationProgress = Mathf.MoveTowards(this.reliableState.activationProgress, 0f, this.lavaActivationDrainRateVsPlayerCount.Evaluate((float)this.PlayerCount) * num);
		if (activationProgress > 0f && this.reliableState.activationProgress <= 1E-45f)
		{
			for (int i = 0; i < this.volcanoEffects.Length; i++)
			{
				VolcanoEffects volcanoEffects = this.volcanoEffects[i];
				if (volcanoEffects != null)
				{
					volcanoEffects.OnVolcanoBellyEmpty();
				}
			}
		}
	}

	// Token: 0x060040E8 RID: 16616 RVA: 0x00159E2C File Offset: 0x0015802C
	private void UpdateLocalState(double currentTime, InfectionLavaController.LavaSyncData syncData)
	{
		switch (syncData.state)
		{
		default:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float num = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			foreach (VolcanoEffects volcanoEffects in this.volcanoEffects)
			{
				if (volcanoEffects != null)
				{
					volcanoEffects.UpdateDrainedState(num);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		{
			this.lavaProgressLinear = 0f;
			this.lavaProgressSmooth = 0f;
			float num2 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num3 = Mathf.Clamp01(num2 / this.eruptTime);
			foreach (VolcanoEffects volcanoEffects2 in this.volcanoEffects)
			{
				if (volcanoEffects2 != null)
				{
					volcanoEffects2.UpdateEruptingState(num2, this.eruptTime - num2, num3);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Rising:
		{
			float num4 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num5 = num4 / this.riseTime;
			this.lavaProgressLinear = Mathf.Clamp01(num5);
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects3 in this.volcanoEffects)
			{
				if (volcanoEffects3 != null)
				{
					volcanoEffects3.UpdateRisingState(num4, this.riseTime - num4, this.lavaProgressLinear);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Full:
		{
			this.lavaProgressLinear = 1f;
			this.lavaProgressSmooth = 1f;
			float num6 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num7 = Mathf.Clamp01(num6 / this.fullTime);
			foreach (VolcanoEffects volcanoEffects4 in this.volcanoEffects)
			{
				if (volcanoEffects4 != null)
				{
					volcanoEffects4.UpdateFullState(num6, this.fullTime - num6, num7);
				}
			}
			return;
		}
		case InfectionLavaController.RisingLavaState.Draining:
		{
			float num8 = Mathf.Max(0f, (float)(currentTime - syncData.stateStartTime));
			float num9 = Mathf.Clamp01(num8 / this.drainTime);
			this.lavaProgressLinear = 1f - num9;
			this.lavaProgressSmooth = this.lavaProgressAnimationCurve.Evaluate(this.lavaProgressLinear);
			foreach (VolcanoEffects volcanoEffects5 in this.volcanoEffects)
			{
				if (volcanoEffects5 != null)
				{
					volcanoEffects5.UpdateDrainingState(num8, this.riseTime - num8, num9);
				}
			}
			return;
		}
		}
	}

	// Token: 0x060040E9 RID: 16617 RVA: 0x0015A09C File Offset: 0x0015829C
	private void UpdateLava(float fillProgress)
	{
		this.lavaScale = Mathf.Lerp(this.lavaMeshMinScale, this.lavaMeshMaxScale, fillProgress);
		if (this.lavaMeshTransform != null)
		{
			this.lavaMeshTransform.localScale = new Vector3(this.lavaMeshTransform.localScale.x, this.lavaMeshTransform.localScale.y, this.lavaScale);
		}
	}

	// Token: 0x060040EA RID: 16618 RVA: 0x0015A108 File Offset: 0x00158308
	private float GetMinLavaY()
	{
		if (this.lavaSurfacePlaneTransform == null || this.lavaMeshTransform == null)
		{
			return 0f;
		}
		float z = this.lavaMeshTransform.localScale.z;
		if (z < 0.001f)
		{
			return this.lavaSurfacePlaneTransform.position.y;
		}
		float y = this.lavaMeshTransform.position.y;
		float num = (this.lavaSurfacePlaneTransform.position.y - y) * (this.lavaMeshMinScale / z);
		return y + num;
	}

	// Token: 0x060040EB RID: 16619 RVA: 0x0015A194 File Offset: 0x00158394
	private void UpdateResidueState()
	{
		float num = ((this.lavaSurfacePlaneTransform != null) ? this.lavaSurfacePlaneTransform.position.y : 0f);
		switch (this.reliableState.state)
		{
		case InfectionLavaController.RisingLavaState.Drained:
		{
			float minLavaY = this.GetMinLavaY();
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, minLavaY, this.residueDrainSpeed * Time.deltaTime);
			break;
		}
		case InfectionLavaController.RisingLavaState.Erupting:
		case InfectionLavaController.RisingLavaState.Rising:
		case InfectionLavaController.RisingLavaState.Full:
			this.residuePlaneY = num;
			break;
		case InfectionLavaController.RisingLavaState.Draining:
			this.residuePlaneY = Mathf.MoveTowards(this.residuePlaneY, num, this.residueDrainSpeed * Time.deltaTime);
			this.residuePlaneY = Mathf.Max(this.residuePlaneY, num);
			break;
		}
		if (this.localPlayerInZone)
		{
			float minLavaY2 = this.GetMinLavaY();
			float num2 = ((this.reliableState.state != InfectionLavaController.RisingLavaState.Drained || this.residuePlaneY > minLavaY2 + 0.01f) ? this.residueIntensity : 0f);
			Shader.SetGlobalVector(InfectionLavaController._shaderProp_GlobalLavaResidueParams, new Vector4(this.residuePlaneY + this.residueOffset, num2, this.residueUVScale, 0f));
		}
	}

	// Token: 0x060040EC RID: 16620 RVA: 0x0015A2B8 File Offset: 0x001584B8
	private void UpdateVolcanoActivationLava(float activationProgress)
	{
		if (this.lavaActivationRenderer == null)
		{
			return;
		}
		this.activationProgessSmooth = Mathf.MoveTowards(this.activationProgessSmooth, activationProgress, this.lavaActivationVisualMovementProgressPerSecond * Time.deltaTime);
		this.lavaActivationMPB.SetColor(ShaderProps._BaseColor, this.lavaActivationGradient.Evaluate(this.activationProgessSmooth));
		this.lavaActivationRenderer.SetPropertyBlock(this.lavaActivationMPB);
		this.lavaActivationRenderer.transform.position = Vector3.Lerp(this.lavaActivationStartPos.position, this.lavaActivationEndPos.position, this.activationProgessSmooth);
	}

	// Token: 0x060040ED RID: 16621 RVA: 0x0015A355 File Offset: 0x00158555
	private void CheckLocalPlayerAgainstLava(double currentTime)
	{
		if (GTPlayer.Instance.InWater && GTPlayer.Instance.CurrentWaterVolume == this.lavaVolume)
		{
			this.LocalPlayerInLava(currentTime, false);
		}
	}

	// Token: 0x060040EE RID: 16622 RVA: 0x0015A382 File Offset: 0x00158582
	private void OnColliderEnteredLava(WaterVolume volume, Collider collider)
	{
		if (collider == GTPlayer.Instance.bodyCollider)
		{
			this.LocalPlayerInLava(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, true);
		}
	}

	// Token: 0x060040EF RID: 16623 RVA: 0x0015A3BC File Offset: 0x001585BC
	private void LocalPlayerInLava(double currentTime, bool enteredLavaThisFrame)
	{
		GorillaGameManager instance = GorillaGameManager.instance;
		if (instance != null && instance.CanAffectPlayer(NetworkSystem.Instance.LocalPlayer, enteredLavaThisFrame) && (currentTime - this.lastTagSelfRPCTime > 0.5 || enteredLavaThisFrame))
		{
			this.lastTagSelfRPCTime = currentTime;
			GameMode.ReportHit();
		}
	}

	// Token: 0x060040F0 RID: 16624 RVA: 0x0015A410 File Offset: 0x00158610
	public void OnActivationLavaProjectileHit(SlingshotProjectile projectile, Collision collision)
	{
		if (!projectile.gameObject.CompareTag("LavaRockProjectile"))
		{
			return;
		}
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			return;
		}
		if (this.IsAuthority)
		{
			this.AddLavaRock(projectile.projectileOwner.ActorNumber);
			return;
		}
		this.reliableState.activationProgress = this.reliableState.activationProgress + this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
	}

	// Token: 0x060040F1 RID: 16625 RVA: 0x0015A4A4 File Offset: 0x001586A4
	private void AddLavaRock(int playerId)
	{
		float num = this.lavaActivationRockProgressVsPlayerCount.Evaluate((float)this.PlayerCount);
		this.reliableState.activationProgress = this.reliableState.activationProgress + num;
		this.AddVoteForVolcanoActivation(playerId);
		for (int i = 0; i < this.volcanoEffects.Length; i++)
		{
			this.volcanoEffects[i].OnStoneAccepted(this.reliableState.activationProgress);
		}
		this.SendSyncEvent();
	}

	// Token: 0x060040F2 RID: 16626 RVA: 0x0015A50C File Offset: 0x0015870C
	private void AddVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority && this.lavaActivationVoteCount < 20)
		{
			bool flag = false;
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount] = playerId;
				this.lavaActivationVoteCount++;
			}
		}
	}

	// Token: 0x060040F3 RID: 16627 RVA: 0x0015A56C File Offset: 0x0015876C
	private void RemoveVoteForVolcanoActivation(int playerId)
	{
		if (this.IsAuthority)
		{
			for (int i = 0; i < this.lavaActivationVoteCount; i++)
			{
				if (this.lavaActivationVotePlayerIds[i] == playerId)
				{
					this.lavaActivationVotePlayerIds[i] = this.lavaActivationVotePlayerIds[this.lavaActivationVoteCount - 1];
					this.lavaActivationVoteCount--;
					return;
				}
			}
		}
	}

	// Token: 0x060040F4 RID: 16628 RVA: 0x0015A5C4 File Offset: 0x001587C4
	private void SendSyncEvent()
	{
		this.lastSyncSendTime = this.currentTime;
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSync((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds);
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x0015A624 File Offset: 0x00158824
	private void SendSyncEventToPlayer(NetPlayer target)
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			return;
		}
		RoomSystem.SendLavaSyncToPlayer((byte)this.zone, (byte)this.reliableState.state, this.reliableState.stateStartTime, this.reliableState.activationProgress, this.lavaActivationVoteCount, this.lavaActivationVotePlayerIds, target);
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x0015A67C File Offset: 0x0015887C
	private unsafe void OnLavaSyncReceived(RoomSystem.LavaSyncEventData data)
	{
		if (data.zone != (byte)this.zone || this.IsAuthority)
		{
			return;
		}
		int zoneAuthorityActorNumber = this.GetZoneAuthorityActorNumber();
		if (zoneAuthorityActorNumber != 2147483647 && data.senderActorNumber != zoneAuthorityActorNumber)
		{
			return;
		}
		InfectionLavaController.RisingLavaState state = (InfectionLavaController.RisingLavaState)data.state;
		float num = this.lavaProgressSmooth;
		this.reliableState.stateStartTime = data.stateStartTime;
		this.reliableState.activationProgress = data.activationProgress;
		this.lavaActivationVoteCount = data.voteCount;
		for (int i = 0; i < 20; i++)
		{
			this.lavaActivationVotePlayerIds[i] = *((ref data.votes.FixedElementField) + (IntPtr)i * 4);
		}
		if (state != this.reliableState.state)
		{
			this.JumpToState(state);
		}
		this.UpdateLocalState(NetworkSystem.Instance.InRoom ? NetworkSystem.Instance.SimTime : Time.timeAsDouble, this.reliableState);
		this.localLagLavaProgressOffset = num - this.lavaProgressSmooth;
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x0015A769 File Offset: 0x00158969
	private void OnPlayerJoinedRoom(NetPlayer player)
	{
		if (!this.IsAuthority)
		{
			return;
		}
		this.SendSyncEventToPlayer(player);
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x0015A77C File Offset: 0x0015897C
	public void OnPlayerLeftRoom(NetPlayer otherNetPlayer)
	{
		this.RemoveVoteForVolcanoActivation(otherNetPlayer.ActorNumber);
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			if (this.localPlayerInZone && this.lavaZoneShaderSettings != null)
			{
				this.lavaZoneShaderSettings.BecomeActiveInstance(false);
			}
			if (this.IsAuthority)
			{
				this.SendSyncEvent();
			}
		}
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x0015A7D4 File Offset: 0x001589D4
	private void OnLeftRoom()
	{
		if (this.reliableState.state != InfectionLavaController.RisingLavaState.Drained)
		{
			double num = this.currentTime - this.reliableState.stateStartTime;
			double timeAsDouble = Time.timeAsDouble;
			this.reliableState.stateStartTime = timeAsDouble - num;
			this.currentTime = timeAsDouble;
			this.prevTime = timeAsDouble;
			this.lastSyncSendTime = 0.0;
			for (int i = 0; i < this.lavaActivationVotePlayerIds.Length; i++)
			{
				this.lavaActivationVotePlayerIds[i] = 0;
			}
			this.lavaActivationVoteCount = 0;
			return;
		}
		ZoneShaderSettings.ActivateDefaultSettings();
		if (this.baseZoneShaderSettings != null)
		{
			this.baseZoneShaderSettings.BecomeActiveInstance(false);
		}
		this.ResetLavaState();
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x0015A87C File Offset: 0x00158A7C
	private int CountRigsInZone()
	{
		int num = 0;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].zoneEntity.currentZone == this.zone)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060040FB RID: 16635 RVA: 0x0015A8D0 File Offset: 0x00158AD0
	private bool CheckLocalPlayerInZone()
	{
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			if (activeRigs[i] != null && activeRigs[i].isLocal)
			{
				return activeRigs[i].zoneEntity.currentZone == this.zone;
			}
		}
		return false;
	}

	// Token: 0x060040FC RID: 16636 RVA: 0x0015A92C File Offset: 0x00158B2C
	private int GetZoneAuthorityActorNumber()
	{
		int num = int.MaxValue;
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			VRRig vrrig = activeRigs[i];
			if (!(vrrig == null) && vrrig.zoneEntity.currentZone == this.zone)
			{
				int num2;
				if (vrrig.isLocal)
				{
					num2 = NetworkSystem.Instance.LocalPlayer.ActorNumber;
				}
				else
				{
					NetPlayer creator = vrrig.Creator;
					if (creator == null)
					{
						goto IL_006C;
					}
					num2 = creator.ActorNumber;
				}
				if (num2 < num)
				{
					num = num2;
				}
			}
			IL_006C:;
		}
		return num;
	}

	// Token: 0x04005154 RID: 20820
	[OnEnterPlay_SetNew]
	private static readonly List<InfectionLavaController> activeControllers = new List<InfectionLavaController>();

	// Token: 0x04005155 RID: 20821
	[SerializeField]
	private GTZone zone;

	// Token: 0x04005156 RID: 20822
	[SerializeField]
	private float lavaMeshMinScale = 3.17f;

	// Token: 0x04005157 RID: 20823
	[Tooltip("If you throw rocks into the volcano quickly enough, then it will raise to this height.")]
	[SerializeField]
	private float lavaMeshMaxScale = 8.941086f;

	// Token: 0x04005158 RID: 20824
	[SerializeField]
	private float eruptTime = 3f;

	// Token: 0x04005159 RID: 20825
	[SerializeField]
	private float riseTime = 10f;

	// Token: 0x0400515A RID: 20826
	[SerializeField]
	private float fullTime = 240f;

	// Token: 0x0400515B RID: 20827
	[SerializeField]
	private float drainTime = 10f;

	// Token: 0x0400515C RID: 20828
	[Tooltip("Delay added when starting the eruption cycle so the sync event has time to reach other clients before visuals begin.")]
	[SerializeField]
	private float latencyBuffer = 0.5f;

	// Token: 0x0400515D RID: 20829
	[SerializeField]
	private float lagResolutionLavaProgressPerSecond = 0.2f;

	// Token: 0x0400515E RID: 20830
	[SerializeField]
	private AnimationCurve lavaProgressAnimationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400515F RID: 20831
	[Header("Volcano Activation")]
	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageDefaultQueue = 0.42f;

	// Token: 0x04005160 RID: 20832
	[SerializeField]
	[Range(0f, 1f)]
	private float activationVotePercentageCompetitiveQueue = 0.6f;

	// Token: 0x04005161 RID: 20833
	[SerializeField]
	private Gradient lavaActivationGradient;

	// Token: 0x04005162 RID: 20834
	[SerializeField]
	private AnimationCurve lavaActivationRockProgressVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005163 RID: 20835
	[SerializeField]
	private AnimationCurve lavaActivationDrainRateVsPlayerCount = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04005164 RID: 20836
	[SerializeField]
	private float lavaActivationVisualMovementProgressPerSecond = 1f;

	// Token: 0x04005165 RID: 20837
	[SerializeField]
	private bool debugLavaActivationVotes;

	// Token: 0x04005166 RID: 20838
	[Header("Scene References")]
	[SerializeField]
	private Transform lavaMeshTransform;

	// Token: 0x04005167 RID: 20839
	[SerializeField]
	private Transform lavaSurfacePlaneTransform;

	// Token: 0x04005168 RID: 20840
	[SerializeField]
	private WaterVolume lavaVolume;

	// Token: 0x04005169 RID: 20841
	[SerializeField]
	private MeshRenderer lavaActivationRenderer;

	// Token: 0x0400516A RID: 20842
	[SerializeField]
	private Transform lavaActivationStartPos;

	// Token: 0x0400516B RID: 20843
	[SerializeField]
	private Transform lavaActivationEndPos;

	// Token: 0x0400516C RID: 20844
	[SerializeField]
	private SlingshotProjectileHitNotifier lavaActivationProjectileHitNotifier;

	// Token: 0x0400516D RID: 20845
	[SerializeField]
	private VolcanoEffects[] volcanoEffects;

	// Token: 0x0400516E RID: 20846
	[SerializeField]
	private ZoneShaderSettings lavaZoneShaderSettings;

	// Token: 0x0400516F RID: 20847
	[SerializeField]
	private ZoneShaderSettings baseZoneShaderSettings;

	// Token: 0x04005170 RID: 20848
	[DebugReadout]
	private InfectionLavaController.LavaSyncData reliableState;

	// Token: 0x04005171 RID: 20849
	private readonly int[] lavaActivationVotePlayerIds = new int[20];

	// Token: 0x04005172 RID: 20850
	private int lavaActivationVoteCount;

	// Token: 0x04005173 RID: 20851
	private float localLagLavaProgressOffset;

	// Token: 0x04005174 RID: 20852
	[DebugReadout]
	private float lavaProgressLinear;

	// Token: 0x04005175 RID: 20853
	[DebugReadout]
	private float lavaProgressSmooth;

	// Token: 0x04005176 RID: 20854
	private double lastTagSelfRPCTime;

	// Token: 0x04005177 RID: 20855
	private const string lavaRockProjectileTag = "LavaRockProjectile";

	// Token: 0x04005178 RID: 20856
	private double currentTime;

	// Token: 0x04005179 RID: 20857
	private double prevTime;

	// Token: 0x0400517A RID: 20858
	private float activationProgessSmooth;

	// Token: 0x0400517B RID: 20859
	private float lavaScale;

	// Token: 0x0400517C RID: 20860
	private MaterialPropertyBlock lavaActivationMPB;

	// Token: 0x0400517D RID: 20861
	private double lastSyncSendTime;

	// Token: 0x0400517E RID: 20862
	private const double syncInterval = 2.0;

	// Token: 0x0400517F RID: 20863
	private bool localPlayerInZone;

	// Token: 0x04005180 RID: 20864
	private static readonly int _shaderProp_GlobalMainWaterSurfacePlane = Shader.PropertyToID("_GlobalMainWaterSurfacePlane");

	// Token: 0x04005181 RID: 20865
	private static readonly int _shaderProp_GlobalLavaResidueParams = Shader.PropertyToID("_GlobalLavaResidueParams");

	// Token: 0x04005182 RID: 20866
	[Header("Lava Residue")]
	[SerializeField]
	[Range(0f, 1f)]
	private float residueIntensity = 0.85f;

	// Token: 0x04005183 RID: 20867
	[Tooltip("How fast the residue plane trails behind the lava when draining (world units/sec).")]
	[SerializeField]
	private float residueDrainSpeed = 1.5f;

	// Token: 0x04005184 RID: 20868
	[Tooltip("UV scale for the residue texture in world space.")]
	[SerializeField]
	private float residueUVScale = 0.25f;

	// Token: 0x04005185 RID: 20869
	[SerializeField]
	private float residueOffset = 2f;

	// Token: 0x04005186 RID: 20870
	private float residuePlaneY;

	// Token: 0x020009E0 RID: 2528
	public enum RisingLavaState
	{
		// Token: 0x04005189 RID: 20873
		Drained,
		// Token: 0x0400518A RID: 20874
		Erupting,
		// Token: 0x0400518B RID: 20875
		Rising,
		// Token: 0x0400518C RID: 20876
		Full,
		// Token: 0x0400518D RID: 20877
		Draining
	}

	// Token: 0x020009E1 RID: 2529
	private struct LavaSyncData
	{
		// Token: 0x0400518E RID: 20878
		public InfectionLavaController.RisingLavaState state;

		// Token: 0x0400518F RID: 20879
		public double stateStartTime;

		// Token: 0x04005190 RID: 20880
		public float activationProgress;
	}
}
