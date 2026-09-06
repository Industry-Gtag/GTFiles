using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000235 RID: 565
public class GreyZoneManager : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x17000171 RID: 369
	// (get) Token: 0x06000EE9 RID: 3817 RVA: 0x00051E3A File Offset: 0x0005003A
	public bool GreyZoneActive
	{
		get
		{
			return this.greyZoneActive;
		}
	}

	// Token: 0x17000172 RID: 370
	// (get) Token: 0x06000EEA RID: 3818 RVA: 0x00051E44 File Offset: 0x00050044
	public bool GreyZoneAvailable
	{
		get
		{
			bool flag = false;
			if (GorillaComputer.instance != null)
			{
				flag = GorillaComputer.instance.GetServerTime().DayOfYear >= this.greyZoneAvailableDayOfYear;
			}
			return flag;
		}
	}

	// Token: 0x17000173 RID: 371
	// (get) Token: 0x06000EEB RID: 3819 RVA: 0x00051E83 File Offset: 0x00050083
	public int GravityFactorSelection
	{
		get
		{
			return this.gravityFactorOptionSelection;
		}
	}

	// Token: 0x17000174 RID: 372
	// (get) Token: 0x06000EEC RID: 3820 RVA: 0x00051E8B File Offset: 0x0005008B
	// (set) Token: 0x06000EED RID: 3821 RVA: 0x00051E93 File Offset: 0x00050093
	public bool TickRunning
	{
		get
		{
			return this._tickRunning;
		}
		set
		{
			this._tickRunning = value;
		}
	}

	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000EEE RID: 3822 RVA: 0x00051E9C File Offset: 0x0005009C
	public bool HasAuthority
	{
		get
		{
			return !PhotonNetwork.InRoom || base.photonView.IsMine;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000EEF RID: 3823 RVA: 0x00051EB2 File Offset: 0x000500B2
	public float SummoningProgress
	{
		get
		{
			return this.summoningProgress;
		}
	}

	// Token: 0x06000EF0 RID: 3824 RVA: 0x00051EBA File Offset: 0x000500BA
	public void RegisterSummoner(GreyZoneSummoner summoner)
	{
		if (!this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Add(summoner);
		}
	}

	// Token: 0x06000EF1 RID: 3825 RVA: 0x00051ED6 File Offset: 0x000500D6
	public void DeregisterSummoner(GreyZoneSummoner summoner)
	{
		if (this.activeSummoners.Contains(summoner))
		{
			this.activeSummoners.Remove(summoner);
		}
	}

	// Token: 0x06000EF2 RID: 3826 RVA: 0x00051EF3 File Offset: 0x000500F3
	public void RegisterMoon(MoonController moon)
	{
		this.moonController = moon;
	}

	// Token: 0x06000EF3 RID: 3827 RVA: 0x00051EFC File Offset: 0x000500FC
	public void UnregisterMoon(MoonController moon)
	{
		if (this.moonController == moon)
		{
			this.moonController = null;
		}
	}

	// Token: 0x06000EF4 RID: 3828 RVA: 0x00051F13 File Offset: 0x00050113
	public void RegisterArea(GreyZoneAreaEnable area)
	{
		if (!this.m_areas.Contains(area))
		{
			this.m_areas.Add(area);
		}
	}

	// Token: 0x06000EF5 RID: 3829 RVA: 0x00051F2F File Offset: 0x0005012F
	public void UnRegisterArea(GreyZoneAreaEnable area)
	{
		this.m_areas.Remove(area);
	}

	// Token: 0x06000EF6 RID: 3830 RVA: 0x00051F3E File Offset: 0x0005013E
	public void ActivateGreyZoneAuthority()
	{
		this.greyZoneActive = true;
		this.photonConnectedDuringActivation = PhotonNetwork.InRoom;
		this.greyZoneActivationTime = (this.photonConnectedDuringActivation ? PhotonNetwork.Time : ((double)Time.time));
		this.ActivateGreyZoneLocal();
	}

	// Token: 0x06000EF7 RID: 3831 RVA: 0x00051F74 File Offset: 0x00050174
	private void ActivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 1);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.GravityOverrideFunction));
			this.gravityOverrideSet = true;
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeOutMusic(2f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioIn(this.greyZoneAmbience, this.greyZoneAmbienceVolume, this.ambienceFadeTime));
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.GTPlay();
		}
		this.greyZoneParticles.gameObject.SetActive(true);
		this.summoningProgress = 1f;
		this.UpdateSummonerVisuals();
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].OnGreyZoneActivated();
		}
		if (this.OnGreyZoneActivated != null)
		{
			this.OnGreyZoneActivated();
		}
	}

	// Token: 0x06000EF8 RID: 3832 RVA: 0x00052088 File Offset: 0x00050288
	public void LocalSimpleActivation(bool onOff, float gravityFactor)
	{
		GTPlayer instance = GTPlayer.Instance;
		if (instance == null)
		{
			return;
		}
		if (!(PlayerPrefs.GetString("didTutorial", "nope") == "done"))
		{
			return;
		}
		this.simpleGravityFactor = Mathf.Clamp(gravityFactor, 0f, 5f);
		Shader.SetGlobalInt(this._GreyZoneActive, onOff ? 1 : 0);
		if (onOff)
		{
			instance.SetGravityOverride(this, new Action<GTPlayer>(this.SimpleGravityOverrideFunction));
		}
		else
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = onOff;
		this.greyZoneParticles.gameObject.SetActive(onOff);
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x00052128 File Offset: 0x00050328
	public void DeactivateGreyZoneAuthority()
	{
		this.greyZoneActive = false;
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			this.summoningPlayerProgress[keyValuePair.Key] = 0f;
		}
		this.DeactivateGreyZoneLocal();
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x00052198 File Offset: 0x00050398
	private void DeactivateGreyZoneLocal()
	{
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(4f);
		}
		if (this.audioFadeCoroutine != null)
		{
			base.StopCoroutine(this.audioFadeCoroutine);
		}
		this.audioFadeCoroutine = base.StartCoroutine(this.FadeAudioOut(this.greyZoneAmbience, this.ambienceFadeTime));
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x00052240 File Offset: 0x00050440
	public void ForceStopGreyZone()
	{
		this.greyZoneActive = false;
		Shader.SetGlobalInt(this._GreyZoneActive, 0);
		GTPlayer instance = GTPlayer.Instance;
		if (instance != null)
		{
			instance.UnsetGravityOverride(this);
		}
		this.gravityOverrideSet = false;
		if (this.moonController != null)
		{
			this.moonController.UpdateDistance(1f);
		}
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.FadeInMusic(0f);
		}
		if (this.greyZoneAmbience != null)
		{
			this.greyZoneAmbience.volume = 0f;
			this.greyZoneAmbience.GTStop();
		}
		this.greyZoneParticles.gameObject.SetActive(false);
		this.summoningProgress = 0f;
		this.UpdateSummonerVisuals();
		if (this.OnGreyZoneDeactivated != null)
		{
			this.OnGreyZoneDeactivated();
		}
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x00052320 File Offset: 0x00050520
	public void GravityOverrideFunction(GTPlayer player)
	{
		this.gravityReductionAmount = 0f;
		if (this.moonController != null)
		{
			this.gravityReductionAmount = Mathf.InverseLerp(1f - this.skyMonsterDistGravityRampBuffer, this.skyMonsterDistGravityRampBuffer, this.moonController.Distance);
		}
		float num = Mathf.Lerp(1f, this.gravityFactorOptions[this.gravityFactorOptionSelection], this.gravityReductionAmount);
		player.AddForce(Physics.gravity * num * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x000523A9 File Offset: 0x000505A9
	public void SimpleGravityOverrideFunction(GTPlayer player)
	{
		player.AddForce(Physics.gravity * this.simpleGravityFactor * player.scale, ForceMode.Acceleration);
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x000523CD File Offset: 0x000505CD
	private IEnumerator FadeAudioIn(AudioSource source, float maxVolume, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			source.GTPlay();
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, maxVolume, num);
				yield return null;
			}
			source.volume = maxVolume;
		}
		yield break;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x000523EA File Offset: 0x000505EA
	private IEnumerator FadeAudioOut(AudioSource source, float duration)
	{
		if (source != null)
		{
			float startingVolume = source.volume;
			float startTime = Time.time;
			for (float num = 0f; num < 1f; num = (Time.time - startTime) / duration)
			{
				source.volume = Mathf.Lerp(startingVolume, 0f, num);
				yield return null;
			}
			source.volume = 0f;
			source.Stop();
		}
		yield break;
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x00052400 File Offset: 0x00050600
	public void VRRigEnteredSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (!this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Add(rig.Creator.ActorNumber, new ValueTuple<VRRig, GreyZoneSummoner>(rig, summoner));
			this.summoningPlayerProgress.Add(rig.Creator.ActorNumber, 0f);
		}
	}

	// Token: 0x06000F01 RID: 3841 RVA: 0x00052460 File Offset: 0x00050660
	public void VRRigExitedSummonerProximity(VRRig rig, GreyZoneSummoner summoner)
	{
		if (this.summoningPlayers.ContainsKey(rig.Creator.ActorNumber))
		{
			this.summoningPlayers.Remove(rig.Creator.ActorNumber);
			this.summoningPlayerProgress.Remove(rig.Creator.ActorNumber);
		}
	}

	// Token: 0x06000F02 RID: 3842 RVA: 0x000524B4 File Offset: 0x000506B4
	private void UpdateSummonerVisuals()
	{
		bool greyZoneAvailable = this.GreyZoneAvailable;
		for (int i = 0; i < this.activeSummoners.Count; i++)
		{
			this.activeSummoners[i].UpdateProgressFeedback(greyZoneAvailable);
		}
	}

	// Token: 0x06000F03 RID: 3843 RVA: 0x000524F0 File Offset: 0x000506F0
	private void ValidateSummoningPlayers()
	{
		this.invalidSummoners.Clear();
		foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
		{
			VRRig item = keyValuePair.Value.Item1;
			GreyZoneSummoner item2 = keyValuePair.Value.Item2;
			if (item.Creator.ActorNumber != keyValuePair.Key || (item.head.rigTarget.position - item2.SummoningFocusPoint).sqrMagnitude > item2.SummonerMaxDistance * item2.SummonerMaxDistance)
			{
				this.invalidSummoners.Add(keyValuePair.Key);
			}
		}
		foreach (int num in this.invalidSummoners)
		{
			this.summoningPlayers.Remove(num);
			this.summoningPlayerProgress.Remove(num);
		}
	}

	// Token: 0x06000F04 RID: 3844 RVA: 0x00052618 File Offset: 0x00050818
	private int DayNightOverrideFunction(int inputIndex)
	{
		int num = 0;
		int num2 = 8;
		int num3 = inputIndex - num;
		int num4 = num2 - inputIndex;
		if (num3 <= 0 || num4 <= 0)
		{
			return inputIndex;
		}
		if (num4 > num3)
		{
			return num2;
		}
		return num;
	}

	// Token: 0x06000F05 RID: 3845 RVA: 0x00052642 File Offset: 0x00050842
	private void Awake()
	{
		if (GreyZoneManager.Instance == null)
		{
			GreyZoneManager.Instance = this;
			this.greyZoneAmbienceVolume = this.greyZoneAmbience.volume;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06000F06 RID: 3846 RVA: 0x00052678 File Offset: 0x00050878
	private void OnEnable()
	{
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.SetTimeIndexOverrideFunction(new Func<int, int>(this.DayNightOverrideFunction));
			}
		}
	}

	// Token: 0x06000F07 RID: 3847 RVA: 0x000526B0 File Offset: 0x000508B0
	private void OnDisable()
	{
		this.ForceStopGreyZone();
		if (this.forceTimeOfDayToNight)
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (instance != null)
			{
				instance.UnsetTimeIndexOverrideFunction();
			}
		}
	}

	// Token: 0x06000F08 RID: 3848 RVA: 0x000526E2 File Offset: 0x000508E2
	private void Update()
	{
		if (this.HasAuthority)
		{
			this.AuthorityUpdate();
		}
		this.SharedUpdate();
	}

	// Token: 0x06000F09 RID: 3849 RVA: 0x000526F8 File Offset: 0x000508F8
	private void AuthorityUpdate()
	{
		float deltaTime = Time.deltaTime;
		if (this.greyZoneActive)
		{
			this.summoningProgress = 1f;
			double num;
			if (this.photonConnectedDuringActivation && PhotonNetwork.InRoom)
			{
				num = PhotonNetwork.Time;
			}
			else if (!this.photonConnectedDuringActivation && !PhotonNetwork.InRoom)
			{
				num = (double)Time.time;
			}
			else
			{
				num = -100.0;
			}
			if (num > this.greyZoneActivationTime + (double)this.greyZoneActiveDuration || num < this.greyZoneActivationTime - 10.0)
			{
				this.DeactivateGreyZoneAuthority();
				return;
			}
		}
		else if (this.GreyZoneAvailable)
		{
			this.roomPlayerList = PhotonNetwork.PlayerList;
			int num2 = 1;
			if (this.roomPlayerList != null && this.roomPlayerList.Length != 0)
			{
				num2 = Mathf.Max((this.roomPlayerList.Length + 1) / 2, 1);
			}
			float num3 = 0f;
			float num4 = 1f / this.summoningActivationTime;
			foreach (KeyValuePair<int, ValueTuple<VRRig, GreyZoneSummoner>> keyValuePair in this.summoningPlayers)
			{
				VRRig item = keyValuePair.Value.Item1;
				GreyZoneSummoner item2 = keyValuePair.Value.Item2;
				float num5 = this.summoningPlayerProgress[keyValuePair.Key];
				Vector3 vector = item2.SummoningFocusPoint - item.leftHand.rigTarget.position;
				Vector3 vector2 = -item.leftHand.rigTarget.right;
				bool flag = Vector3.Dot(vector, vector2) > 0f;
				Vector3 vector3 = item2.SummoningFocusPoint - item.rightHand.rigTarget.position;
				Vector3 right = item.rightHand.rigTarget.right;
				bool flag2 = Vector3.Dot(vector3, right) > 0f;
				if (flag && flag2)
				{
					num5 = Mathf.MoveTowards(num5, 1f, num4 * deltaTime);
				}
				else
				{
					num5 = Mathf.MoveTowards(num5, 0f, num4 * deltaTime);
				}
				num3 += num5;
				this.summoningPlayerProgress[keyValuePair.Key] = num5;
			}
			float num6 = 0.95f;
			this.summoningProgress = Mathf.Clamp01(num3 / num6 / (float)num2);
			this.UpdateSummonerVisuals();
			if (this.summoningProgress > 0.99f)
			{
				this.ActivateGreyZoneAuthority();
			}
		}
	}

	// Token: 0x06000F0A RID: 3850 RVA: 0x00052950 File Offset: 0x00050B50
	private void SharedUpdate()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.greyZoneActive)
		{
			Vector3 vector = Vector3.ClampMagnitude(instance.InstantaneousVelocity * this.particlePredictiveSpawnVelocityFactor, this.particlePredictiveSpawnMaxDist);
			this.greyZoneParticles.transform.position = instance.HeadCenterPosition + Vector3.down * 0.5f + vector;
		}
		else if (this.gravityOverrideSet && this.gravityReductionAmount < 0.01f)
		{
			instance.UnsetGravityOverride(this);
			this.gravityOverrideSet = false;
		}
		float num = (this.greyZoneActive ? 0f : 1f);
		float num2 = (this.greyZoneActive ? this.skyMonsterMovementEnterTime : this.skyMonsterMovementExitTime);
		if (this.moonController != null && this.moonController.Distance != num)
		{
			float num3 = Mathf.SmoothDamp(this.moonController.Distance, num, ref this.skyMonsterMovementVelocity, num2);
			if ((double)Mathf.Abs(num3 - num) < 0.001)
			{
				num3 = num;
			}
			this.moonController.UpdateDistance(num3);
		}
	}

	// Token: 0x06000F0B RID: 3851 RVA: 0x00052A64 File Offset: 0x00050C64
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(this.greyZoneActive);
			stream.SendNext(this.greyZoneActivationTime);
			stream.SendNext(this.photonConnectedDuringActivation);
			stream.SendNext(this.gravityFactorOptionSelection);
			stream.SendNext(this.summoningProgress);
			return;
		}
		if (stream.IsReading && info.Sender.IsMasterClient)
		{
			bool flag = this.greyZoneActive;
			this.greyZoneActive = (bool)stream.ReceiveNext();
			this.greyZoneActivationTime = ((double)stream.ReceiveNext()).GetFinite();
			this.photonConnectedDuringActivation = (bool)stream.ReceiveNext();
			this.gravityFactorOptionSelection = (int)stream.ReceiveNext();
			this.gravityFactorOptionSelection = Mathf.Clamp(this.gravityFactorOptionSelection, 0, this.gravityFactorOptions.Length - 1);
			this.summoningProgress = ((float)stream.ReceiveNext()).ClampSafe(0f, 1f);
			this.UpdateSummonerVisuals();
			if (this.greyZoneActive && !flag)
			{
				if (this.m_areas.Count > 0)
				{
					this.ActivateGreyZoneLocal();
					return;
				}
				this.greyZoneActive = false;
				return;
			}
			else if (!this.greyZoneActive && flag)
			{
				this.DeactivateGreyZoneLocal();
			}
		}
	}

	// Token: 0x06000F0C RID: 3852 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x06000F0D RID: 3853 RVA: 0x00052BB7 File Offset: 0x00050DB7
	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x06000F0E RID: 3854 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06000F0F RID: 3855 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x06000F10 RID: 3856 RVA: 0x00052BB7 File Offset: 0x00050DB7
	public void OnMasterClientSwitched(Player newMasterClient)
	{
		this.ValidateSummoningPlayers();
	}

	// Token: 0x04001214 RID: 4628
	[OnEnterPlay_SetNull]
	public static volatile GreyZoneManager Instance;

	// Token: 0x04001215 RID: 4629
	[SerializeField]
	private float greyZoneActiveDuration = 90f;

	// Token: 0x04001216 RID: 4630
	[SerializeField]
	private float[] gravityFactorOptions = new float[] { 0.25f, 0.5f, 0.75f };

	// Token: 0x04001217 RID: 4631
	[SerializeField]
	private int gravityFactorOptionSelection = 1;

	// Token: 0x04001218 RID: 4632
	[SerializeField]
	private float summoningActivationTime = 3f;

	// Token: 0x04001219 RID: 4633
	[SerializeField]
	private AudioSource greyZoneAmbience;

	// Token: 0x0400121A RID: 4634
	[SerializeField]
	private float ambienceFadeTime = 4f;

	// Token: 0x0400121B RID: 4635
	[SerializeField]
	private bool forceTimeOfDayToNight;

	// Token: 0x0400121C RID: 4636
	[SerializeField]
	private float skyMonsterMovementEnterTime = 4.5f;

	// Token: 0x0400121D RID: 4637
	[SerializeField]
	private float skyMonsterMovementExitTime = 3.2f;

	// Token: 0x0400121E RID: 4638
	[SerializeField]
	private float skyMonsterDistGravityRampBuffer = 0.15f;

	// Token: 0x0400121F RID: 4639
	[SerializeField]
	[Range(0f, 1f)]
	private float gravityReductionAmount = 1f;

	// Token: 0x04001220 RID: 4640
	private float simpleGravityFactor = 1f;

	// Token: 0x04001221 RID: 4641
	[SerializeField]
	private ParticleSystem greyZoneParticles;

	// Token: 0x04001222 RID: 4642
	[SerializeField]
	private float particlePredictiveSpawnMaxDist = 4f;

	// Token: 0x04001223 RID: 4643
	[SerializeField]
	private float particlePredictiveSpawnVelocityFactor = 0.5f;

	// Token: 0x04001224 RID: 4644
	private bool photonConnectedDuringActivation;

	// Token: 0x04001225 RID: 4645
	private double greyZoneActivationTime;

	// Token: 0x04001226 RID: 4646
	private bool greyZoneActive;

	// Token: 0x04001227 RID: 4647
	private bool _tickRunning;

	// Token: 0x04001228 RID: 4648
	private float summoningProgress;

	// Token: 0x04001229 RID: 4649
	private List<GreyZoneSummoner> activeSummoners = new List<GreyZoneSummoner>();

	// Token: 0x0400122A RID: 4650
	private Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>> summoningPlayers = new Dictionary<int, ValueTuple<VRRig, GreyZoneSummoner>>();

	// Token: 0x0400122B RID: 4651
	private Dictionary<int, float> summoningPlayerProgress = new Dictionary<int, float>();

	// Token: 0x0400122C RID: 4652
	private HashSet<int> invalidSummoners = new HashSet<int>();

	// Token: 0x0400122D RID: 4653
	private List<GreyZoneAreaEnable> m_areas = new List<GreyZoneAreaEnable>();

	// Token: 0x0400122E RID: 4654
	private Coroutine audioFadeCoroutine;

	// Token: 0x0400122F RID: 4655
	private Player[] roomPlayerList;

	// Token: 0x04001230 RID: 4656
	private ShaderHashId _GreyZoneActive = new ShaderHashId("_GreyZoneActive");

	// Token: 0x04001231 RID: 4657
	private MoonController moonController;

	// Token: 0x04001232 RID: 4658
	private float skyMonsterMovementVelocity;

	// Token: 0x04001233 RID: 4659
	private bool gravityOverrideSet;

	// Token: 0x04001234 RID: 4660
	private float greyZoneAmbienceVolume = 0.15f;

	// Token: 0x04001235 RID: 4661
	private int greyZoneAvailableDayOfYear = new DateTime(2024, 10, 25).DayOfYear;

	// Token: 0x04001236 RID: 4662
	public Action OnGreyZoneActivated;

	// Token: 0x04001237 RID: 4663
	public Action OnGreyZoneDeactivated;
}
