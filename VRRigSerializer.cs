using System;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.Audio;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200085E RID: 2142
[NetworkBehaviourWeaved(35)]
internal class VRRigSerializer : GorillaWrappedSerializer, IFXContextParems<HandTapArgs>, IFXContextParems<GeoSoundArg>
{
	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x06003752 RID: 14162 RVA: 0x0012FDA1 File Offset: 0x0012DFA1
	// (set) Token: 0x06003753 RID: 14163 RVA: 0x0012FDCB File Offset: 0x0012DFCB
	[Networked]
	[NetworkedWeaved(0, 17)]
	public unsafe NetworkString<_16> nickName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.nickName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x06003754 RID: 14164 RVA: 0x0012FDF6 File Offset: 0x0012DFF6
	// (set) Token: 0x06003755 RID: 14165 RVA: 0x0012FE24 File Offset: 0x0012E024
	[Networked]
	[NetworkedWeaved(17, 17)]
	public unsafe NetworkString<_16> defaultName
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(NetworkString<_16>*)(this.Ptr + 17);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.defaultName. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(NetworkString<_16>*)(this.Ptr + 17) = value;
		}
	}

	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x06003756 RID: 14166 RVA: 0x0012FE53 File Offset: 0x0012E053
	// (set) Token: 0x06003757 RID: 14167 RVA: 0x0012FE81 File Offset: 0x0012E081
	[Networked]
	[NetworkedWeaved(34, 1)]
	public bool tutorialComplete
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(this.Ptr + 34);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing VRRigSerializer.tutorialComplete. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(this.Ptr + 34, value);
		}
	}

	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x06003758 RID: 14168 RVA: 0x0012FEB0 File Offset: 0x0012E0B0
	private PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
	}

	// Token: 0x170004F2 RID: 1266
	// (get) Token: 0x06003759 RID: 14169 RVA: 0x0012FEB8 File Offset: 0x0012E0B8
	public VRRig VRRig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170004F3 RID: 1267
	// (get) Token: 0x0600375A RID: 14170 RVA: 0x0012FEC0 File Offset: 0x0012E0C0
	public FXSystemSettings settings
	{
		get
		{
			return this.vrrig.fxSettings;
		}
	}

	// Token: 0x170004F4 RID: 1268
	// (get) Token: 0x0600375B RID: 14171 RVA: 0x0012FECD File Offset: 0x0012E0CD
	// (set) Token: 0x0600375C RID: 14172 RVA: 0x0012FED5 File Offset: 0x0012E0D5
	public InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped> SuccesfullSpawnEvent { get; private set; } = new InDelegateListProcessor<RigContainer, PhotonMessageInfoWrapped>(2);

	// Token: 0x0600375D RID: 14173 RVA: 0x0012FEE0 File Offset: 0x0012E0E0
	protected override bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetObject = null;
		outTargetType = null;
		NetPlayer player = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
		if (this.netView.IsRoomView)
		{
			if (player != null)
			{
				MonkeAgent.instance.SendReport("creating rigs as room objects", player.UserId, player.NickName);
			}
			return false;
		}
		if (NetworkSystem.Instance.IsObjectRoomObject(base.gameObject))
		{
			NetPlayer player2 = NetworkSystem.Instance.GetPlayer(wrappedInfo.senderID);
			if (player2 != null)
			{
				MonkeAgent.instance.SendReport("creating rigs as room objects", player2.UserId, player2.NickName);
			}
			return false;
		}
		if (player != this.netView.Owner)
		{
			MonkeAgent.instance.SendReport("creating rigs for someone else", player.UserId, player.NickName);
			return false;
		}
		if (VRRigCache.Instance.TryGetVrrig(player, out this.rigContainer))
		{
			outTargetObject = this.rigContainer.gameObject;
			outTargetType = typeof(VRRig);
			this.vrrig = this.rigContainer.Rig;
			return true;
		}
		return false;
	}

	// Token: 0x0600375E RID: 14174 RVA: 0x0012FFE8 File Offset: 0x0012E1E8
	protected override void OnSuccesfullySpawned(PhotonMessageInfoWrapped info)
	{
		bool initialized = this.rigContainer.Initialized;
		this.rigContainer.InitializeNetwork(this.netView, this.Voice, this);
		this.networkSpeaker.SetParent(this.rigContainer.SpeakerHead, false);
		base.transform.SetParent(VRRigCache.Instance.NetworkParent, true);
		this.SetupLoudSpeakerNetwork(this.rigContainer);
		this.netView.GetView.AddCallbackTarget(this);
		if (!initialized)
		{
			object[] instantiationData = info.punInfo.photonView.InstantiationData;
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			if (instantiationData != null && instantiationData.Length == 3)
			{
				object obj = instantiationData[0];
				if (obj is float)
				{
					float num4 = (float)obj;
					obj = instantiationData[1];
					if (obj is float)
					{
						float num5 = (float)obj;
						obj = instantiationData[2];
						if (obj is float)
						{
							float num6 = (float)obj;
							num = num4.ClampSafe(0f, 1f);
							num2 = num5.ClampSafe(0f, 1f);
							num3 = num6.ClampSafe(0f, 1f);
						}
					}
				}
			}
			this.vrrig.InitializeNoobMaterialLocal(num, num2, num3);
		}
		this.SuccesfullSpawnEvent.InvokeSafe(in this.rigContainer, in info);
		NetworkSystem.Instance.IsObjectLocallyOwned(base.gameObject);
		if (VRRigCache.isInitialized)
		{
			VRRigCache.Instance.OnVrrigSerializerSuccesfullySpawned();
		}
	}

	// Token: 0x0600375F RID: 14175 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void OnFailedSpawn()
	{
	}

	// Token: 0x06003760 RID: 14176 RVA: 0x00130154 File Offset: 0x0012E354
	protected override void OnBeforeDespawn()
	{
		this.CleanUp(true);
	}

	// Token: 0x06003761 RID: 14177 RVA: 0x00130160 File Offset: 0x0012E360
	private void CleanUp(bool netDestroy)
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		this.successfullInstantiate = false;
		if (this.vrrig != null)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				if (this.vrrig.isOfflineVRRig)
				{
					this.vrrig.ChangeMaterialLocal(0);
				}
			}
			else if (this.vrrig.isOfflineVRRig)
			{
				NetworkSystem.Instance.NetDestroy(base.gameObject);
			}
			if (this.vrrig.netView == this.netView)
			{
				this.vrrig.netView = null;
			}
			if (this.vrrig.rigSerializer == this)
			{
				this.vrrig.rigSerializer = null;
			}
		}
		if (this.networkSpeaker != null)
		{
			this.CleanupLoudSpeakerNetwork();
			this.networkSpeaker.gameObject.SetActive(false);
			if (netDestroy)
			{
				this.networkSpeaker.SetParent(base.transform, false);
			}
			else
			{
				this.networkSpeaker.SetParent(null);
			}
		}
		this.vrrig = null;
	}

	// Token: 0x06003762 RID: 14178 RVA: 0x00130257 File Offset: 0x0012E457
	private void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		this.CleanUp(false);
	}

	// Token: 0x06003763 RID: 14179 RVA: 0x00130266 File Offset: 0x0012E466
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.networkSpeaker != null && this.networkSpeaker.parent != base.transform)
		{
			global::UnityEngine.Object.Destroy(this.networkSpeaker.gameObject);
		}
	}

	// Token: 0x06003764 RID: 14180 RVA: 0x001302A4 File Offset: 0x0012E4A4
	[PunRPC]
	public void RPC_InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfo info)
	{
		this.InitializeNoobMaterialShared(red, green, blue, info);
	}

	// Token: 0x06003765 RID: 14181 RVA: 0x001302B6 File Offset: 0x0012E4B6
	[PunRPC]
	public void RPC_RequestCosmetics(PhotonMessageInfo info)
	{
		this.RequestCosmeticsShared(info);
	}

	// Token: 0x06003766 RID: 14182 RVA: 0x001302C4 File Offset: 0x0012E4C4
	[PunRPC]
	public void RPC_PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfo info)
	{
		this.PlayDrumShared(drumIndex, drumVolume, info);
	}

	// Token: 0x06003767 RID: 14183 RVA: 0x001302D4 File Offset: 0x0012E4D4
	[PunRPC]
	public void RPC_PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfo info)
	{
		this.PlaySelfOnlyInstrumentShared(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x06003768 RID: 14184 RVA: 0x001302E6 File Offset: 0x0012E4E6
	[PunRPC]
	public void RPC_PlayHandTap(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.PlayHandTapShared(soundIndex, isLeftHand, tapVolume, info);
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x001302F8 File Offset: 0x0012E4F8
	public void RPC_UpdateNativeSize(float value, PhotonMessageInfo info = default(PhotonMessageInfo))
	{
		this.UpdateNativeSizeShared(value, info);
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void RPC_UpdateCosmetics(string[] currentItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void RPC_UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, PhotonMessageInfo info)
	{
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x00130307 File Offset: 0x0012E507
	[PunRPC]
	public void RPC_UpdateCosmeticsWithTryonPacked(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfo info)
	{
		this.UpdateCosmeticsWithTryonShared(currentItemsPacked, tryOnItemsPacked, playfx, info);
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x00130319 File Offset: 0x0012E519
	[PunRPC]
	public void RPC_UpdateCosmeticsWithCollectablesPacked(int[] data, PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithCollectables(data ?? Array.Empty<int>(), info);
	}

	// Token: 0x0600376E RID: 14190 RVA: 0x0013033B File Offset: 0x0012E53B
	[PunRPC]
	public void RPC_SetCollectionCycleIndex(int[] data, PhotonMessageInfo info)
	{
		if (data == null || data.Length != 3)
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.SetCollectionCycleIndex(data[0], data[1], data[2], info);
	}

	// Token: 0x0600376F RID: 14191 RVA: 0x00130366 File Offset: 0x0012E566
	[PunRPC]
	public void RPC_BroadcastSubCosmeticSignal(int[] data, PhotonMessageInfo info)
	{
		if (data == null || data.Length != 2)
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.BroadcastSubCosmeticSignal(data[0], data[1], info);
	}

	// Token: 0x06003770 RID: 14192 RVA: 0x0013038E File Offset: 0x0012E58E
	[PunRPC]
	public void RPC_HideAllCosmetics(PhotonMessageInfo info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.HideAllCosmetics(info);
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x001303A1 File Offset: 0x0012E5A1
	[PunRPC]
	public void RPC_PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfo info)
	{
		this.PlaySplashEffectShared(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x001303B9 File Offset: 0x0012E5B9
	[PunRPC]
	public void RPC_PlayGeodeEffect(Vector3 hitPosition, PhotonMessageInfo info)
	{
		this.PlayGeodeEffectShared(hitPosition, info);
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x001303C8 File Offset: 0x0012E5C8
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfo info)
	{
		this.EnableNonCosmeticHandItemShared(enable, isLeftHand, info);
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x001303D8 File Offset: 0x0012E5D8
	[PunRPC]
	public void OnHandTapRPC(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfo info)
	{
		this.OnHandTapRPCShared(audioClipIndex, isDownTap, isLeftHand, stiltID, handTapSpeed, packedDirFromHitToHand, info);
	}

	// Token: 0x06003775 RID: 14197 RVA: 0x001303F0 File Offset: 0x0012E5F0
	[PunRPC]
	public void RPC_UpdateQuestScore(int score, PhotonMessageInfo info)
	{
		this.UpdateQuestScore(score, info);
	}

	// Token: 0x06003776 RID: 14198 RVA: 0x001303FF File Offset: 0x0012E5FF
	[PunRPC]
	public void RPC_UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfo info)
	{
		this.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x06003777 RID: 14199 RVA: 0x00130414 File Offset: 0x0012E614
	private void SetupLoudSpeakerNetwork(RigContainer rigContainer)
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.AddSpeaker(component);
		}
	}

	// Token: 0x06003778 RID: 14200 RVA: 0x0013048C File Offset: 0x0012E68C
	private void CleanupLoudSpeakerNetwork()
	{
		if (this.networkSpeaker == null)
		{
			return;
		}
		Speaker component = this.networkSpeaker.GetComponent<Speaker>();
		if (component == null)
		{
			return;
		}
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in this.rigContainer.LoudSpeakerNetworks)
		{
			loudSpeakerNetwork.RemoveSpeaker(component);
		}
	}

	// Token: 0x06003779 RID: 14201 RVA: 0x00130508 File Offset: 0x0012E708
	public void BroadcastLoudSpeakerNetwork(bool toggleBroadcast, int actorNumber)
	{
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
		{
			return;
		}
		bool flag = actorNumber == NetworkSystem.Instance.LocalPlayer.ActorNumber;
		this.BroadcastLoudSpeakerNetworkShared(toggleBroadcast, rigContainer, actorNumber, flag);
	}

	// Token: 0x0600377A RID: 14202 RVA: 0x0013054C File Offset: 0x0012E74C
	private void BroadcastLoudSpeakerNetworkShared(bool toggleBroadcast, RigContainer rigContainer, int actorNumber, bool isLocal)
	{
		this.SetupLoudSpeakerNetwork(rigContainer);
		foreach (LoudSpeakerNetwork loudSpeakerNetwork in rigContainer.LoudSpeakerNetworks)
		{
			if (toggleBroadcast)
			{
				loudSpeakerNetwork.BroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
			else
			{
				loudSpeakerNetwork.StopBroadcastLoudSpeakerNetwork(actorNumber, isLocal);
			}
		}
	}

	// Token: 0x0600377B RID: 14203 RVA: 0x001305B8 File Offset: 0x0012E7B8
	[PunRPC]
	public void GrabbedByPlayer(bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand, PhotonMessageInfo info)
	{
		GorillaGuardianManager gorillaGuardianManager = global::GorillaGameModes.GameMode.ActiveGameMode as GorillaGuardianManager;
		if (gorillaGuardianManager == null || !gorillaGuardianManager.IsPlayerGuardian(info.Sender))
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			return;
		}
		this.vrrig.GrabbedByPlayer(rigContainer.Rig, grabbedBody, grabbedLeftHand, grabbedWithLeftHand);
	}

	// Token: 0x0600377C RID: 14204 RVA: 0x00130614 File Offset: 0x0012E814
	[PunRPC]
	public void DroppedByPlayer(Vector3 throwVelocity, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "DroppedByPlayer");
		RigContainer rigContainer;
		if (this.vrrig.isOfflineVRRig && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			float num = 10000f;
			if ((in throwVelocity).IsValid(in num))
			{
				this.vrrig.DroppedByPlayer(rigContainer.Rig, throwVelocity);
				return;
			}
		}
	}

	// Token: 0x0600377D RID: 14205 RVA: 0x00130671 File Offset: 0x0012E871
	void IFXContextParems<HandTapArgs>.OnPlayFX(HandTapArgs parems)
	{
		this.vrrig.PlayHandTapLocal(parems.soundIndex, parems.isLeftHand, parems.tapVolume);
	}

	// Token: 0x0600377E RID: 14206 RVA: 0x00130690 File Offset: 0x0012E890
	void IFXContextParems<GeoSoundArg>.OnPlayFX(GeoSoundArg parems)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayGeodeEffect(parems.position);
	}

	// Token: 0x0600377F RID: 14207 RVA: 0x001306A8 File Offset: 0x0012E8A8
	private void OnHandTapRPCShared(int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapSpeed, long packedDirFromHitToHand, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "OnHandTapRPCShared");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		if (audioClipIndex < 0 || audioClipIndex >= GTPlayer.Instance.materialData.Count)
		{
			return;
		}
		TakeMyHand_HandLink takeMyHand_HandLink = (isLeftHand ? this.vrrig.rightHandLink : this.vrrig.leftHandLink);
		NetPlayer grabbedPlayer = takeMyHand_HandLink.grabbedPlayer;
		if (grabbedPlayer != null && grabbedPlayer.IsLocal)
		{
			(takeMyHand_HandLink.grabbedHandIsLeft ? VRRig.LocalRig.leftHandLink : VRRig.LocalRig.rightHandLink).PlayVicariousTapHaptic();
		}
		Vector3 vector = Utils.UnpackVector3FromLong(packedDirFromHitToHand);
		if (!Mathf.Approximately(vector.sqrMagnitude, 1f))
		{
			vector.Normalize();
		}
		float num = GorillaTagger.Instance.DefaultHandTapVolume;
		GorillaAmbushManager gorillaAmbushManager = global::GorillaGameModes.GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(this.rigContainer.Creator))
		{
			num = gorillaAmbushManager.crawlingSpeedForMaxVolume;
		}
		OnHandTapFX onHandTapFX = new OnHandTapFX
		{
			rig = this.vrrig,
			surfaceIndex = audioClipIndex,
			isDownTap = isDownTap,
			isLeftHand = isLeftHand,
			stiltID = stiltID,
			volume = handTapSpeed.ClampSafe(0f, num),
			speed = handTapSpeed,
			tapDir = vector
		};
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority() && CrittersManager.instance.rigSetupByRig[this.vrrig].IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.vrrig].rigActors[isLeftHand ? 0 : 2].actorSet;
			if (crittersLoudNoise.IsNotNull())
			{
				crittersLoudNoise.PlayHandTapRemote(info.SentServerTime, isLeftHand);
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(GTZone.ghostReactor);
		if (managerForZone != null && managerForZone.ghostReactorManager != null)
		{
			Vector3 vector2 = (isLeftHand ? this.vrrig.leftHand.rigTarget.position : this.vrrig.rightHand.rigTarget.position);
			managerForZone.ghostReactorManager.OnSharedTap(this.vrrig, vector2, handTapSpeed);
		}
		FXSystem.PlayFXForRig<HandEffectContext>(FXType.OnHandTap, onHandTapFX, info);
	}

	// Token: 0x06003780 RID: 14208 RVA: 0x001308F4 File Offset: 0x0012EAF4
	private void PlayHandTapShared(int soundIndex, bool isLeftHand, float tapVolume, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		MonkeAgent.IncrementRPCCall(info, "PlayHandTapShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && float.IsFinite(tapVolume))
		{
			this.handTapArgs.soundIndex = soundIndex;
			this.handTapArgs.isLeftHand = isLeftHand;
			this.handTapArgs.tapVolume = Mathf.Clamp(tapVolume, 0f, 0.1f);
			FXSystem.PlayFX<HandTapArgs>(FXType.PlayHandTap, this, this.handTapArgs, info);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent hand tap", sender.UserId, sender.NickName);
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x00130994 File Offset: 0x0012EB94
	private void UpdateNativeSizeShared(float value, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		MonkeAgent.IncrementRPCCall(info, "UpdateNativeSizeShared");
		NetPlayer sender = info.Sender;
		if (info.Sender == this.netView.Owner && RPCUtil.SafeValue(value, 0.1f, 10f) && RPCUtil.NotSpam("UpdateNativeSizeShared", info, 1f))
		{
			if (this.vrrig != null)
			{
				this.vrrig.NativeScale = value;
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent native size", sender.UserId, sender.NickName);
		}
	}

	// Token: 0x06003782 RID: 14210 RVA: 0x00130A24 File Offset: 0x0012EC24
	private void PlayGeodeEffectShared(Vector3 hitPosition, PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "PlayGeodeEffectShared");
		if (info.Sender == this.netView.Owner)
		{
			float num = 10000f;
			if ((in hitPosition).IsValid(in num))
			{
				this.geoSoundArg.position = hitPosition;
				FXSystem.PlayFX<GeoSoundArg>(FXType.PlayHandTap, this, this.geoSoundArg, info);
				return;
			}
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent geode effect", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06003783 RID: 14211 RVA: 0x00130AA2 File Offset: 0x0012ECA2
	private void InitializeNoobMaterialShared(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.InitializeNoobMaterial(red, green, blue, info);
	}

	// Token: 0x06003784 RID: 14212 RVA: 0x00130AB9 File Offset: 0x0012ECB9
	private void RequestMaterialColorShared(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestMaterialColor(askingPlayerID, info);
	}

	// Token: 0x06003785 RID: 14213 RVA: 0x00130AD0 File Offset: 0x0012ECD0
	private void RequestCosmeticsShared(PhotonMessageInfoWrapped info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestCosmetics");
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer) || !rigContainer.Rig.fxSettings.callSettings[9].CallLimitSettings.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.RequestCosmetics(info);
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x00130B32 File Offset: 0x0012ED32
	private void PlayDrumShared(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlayDrum(drumIndex, drumVolume, info);
	}

	// Token: 0x06003787 RID: 14215 RVA: 0x00130B47 File Offset: 0x0012ED47
	private void PlaySelfOnlyInstrumentShared(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySelfOnlyInstrument(selfOnlyIndex, noteIndex, instrumentVol, info);
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x00130B5E File Offset: 0x0012ED5E
	private void UpdateCosmeticsWithTryonShared(int[] currentItems, int[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateCosmeticsWithTryon(currentItems, tryOnItems, playfx, info);
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x00130B75 File Offset: 0x0012ED75
	private void PlaySplashEffectShared(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.PlaySplashEffect(splashPosition, splashRotation, splashScale, boundingRadius, bigSplash, enteringWater, info);
	}

	// Token: 0x0600378A RID: 14218 RVA: 0x00130B92 File Offset: 0x0012ED92
	private void EnableNonCosmeticHandItemShared(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.EnableNonCosmeticHandItemRPC(enable, isLeftHand, info);
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x00130BA7 File Offset: 0x0012EDA7
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateQuestScore(score, info);
	}

	// Token: 0x0600378C RID: 14220 RVA: 0x00130BBB File Offset: 0x0012EDBB
	public void UpdateRankedInfo(float elo, int questRank, int PCRank, PhotonMessageInfoWrapped info)
	{
		VRRig vrrig = this.vrrig;
		if (vrrig == null)
		{
			return;
		}
		vrrig.UpdateRankedInfo(elo, questRank, PCRank, info);
	}

	// Token: 0x0600378E RID: 14222 RVA: 0x00130BFC File Offset: 0x0012EDFC
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.nickName = this._nickName;
		this.defaultName = this._defaultName;
		this.tutorialComplete = this._tutorialComplete;
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x00130C2C File Offset: 0x0012EE2C
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._nickName = this.nickName;
		this._defaultName = this.defaultName;
		this._tutorialComplete = this.tutorialComplete;
	}

	// Token: 0x040047AD RID: 18349
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("nickName", 0, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _nickName;

	// Token: 0x040047AE RID: 18350
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("defaultName", 17, 17)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private NetworkString<_16> _defaultName;

	// Token: 0x040047AF RID: 18351
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("tutorialComplete", 34, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private bool _tutorialComplete;

	// Token: 0x040047B0 RID: 18352
	[SerializeField]
	private PhotonVoiceView voiceView;

	// Token: 0x040047B1 RID: 18353
	public Transform networkSpeaker;

	// Token: 0x040047B2 RID: 18354
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x040047B3 RID: 18355
	private RigContainer rigContainer;

	// Token: 0x040047B4 RID: 18356
	private HandTapArgs handTapArgs = new HandTapArgs();

	// Token: 0x040047B5 RID: 18357
	private GeoSoundArg geoSoundArg = new GeoSoundArg();
}
