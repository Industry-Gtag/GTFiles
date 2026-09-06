using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using KID.Model;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using TagEffects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020004F3 RID: 1267
public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IGorillaSliceableSimple, ITickSystemPost, IEyeScannable
{
	// Token: 0x06001EC4 RID: 7876 RVA: 0x000A41B8 File Offset: 0x000A23B8
	private void CosmeticsV2_Awake()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000A41DA File Offset: 0x000A23DA
	internal void Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive(false);
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001EC6 RID: 7878 RVA: 0x000A4209 File Offset: 0x000A2409
	// (set) Token: 0x06001EC7 RID: 7879 RVA: 0x000A4216 File Offset: 0x000A2416
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x000A4224 File Offset: 0x000A2424
	public Material myDefaultSkinMaterialInstance
	{
		get
		{
			return this.bodyRenderer.myDefaultSkinMaterialInstance;
		}
	}

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001EC9 RID: 7881 RVA: 0x000A4231 File Offset: 0x000A2431
	public List<GameObject> cosmetics
	{
		get
		{
			return CosmeticsV2Spawner_Dirty.RigDataForRig(this).vrRig_cosmetics;
		}
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001ECA RID: 7882 RVA: 0x000A423E File Offset: 0x000A243E
	public List<GameObject> overrideCosmetics
	{
		get
		{
			return CosmeticsV2Spawner_Dirty.RigDataForRig(this).vrRig_override;
		}
	}

	// Token: 0x06001ECB RID: 7883 RVA: 0x000A424B File Offset: 0x000A244B
	internal void SetTaggedBy(VRRig taggingRig)
	{
		this.taggedById = taggingRig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000A425E File Offset: 0x000A245E
	public int CheckCosmeticAge(string pfID)
	{
		if (this._playerOwnedCosmeticsAge.ContainsKey(pfID))
		{
			return this._playerOwnedCosmeticsAge[pfID];
		}
		return 0;
	}

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001ECD RID: 7885 RVA: 0x000A427C File Offset: 0x000A247C
	public HashSet<string> TemporaryCosmetics
	{
		get
		{
			return this._temporaryCosmetics;
		}
	}

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001ECE RID: 7886 RVA: 0x000A4284 File Offset: 0x000A2484
	// (set) Token: 0x06001ECF RID: 7887 RVA: 0x000A428C File Offset: 0x000A248C
	internal bool InitializedCosmetics
	{
		get
		{
			return this.initializedCosmetics;
		}
		set
		{
			this.initializedCosmetics = value;
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001ED0 RID: 7888 RVA: 0x000A4295 File Offset: 0x000A2495
	// (set) Token: 0x06001ED1 RID: 7889 RVA: 0x000A429D File Offset: 0x000A249D
	public CosmeticRefRegistry cosmeticReferences { get; private set; }

	// Token: 0x06001ED2 RID: 7890 RVA: 0x000A42A6 File Offset: 0x000A24A6
	public void SetVoiceShiftCosmeticsDirty()
	{
		this.voiceShiftCosmeticsDirty = true;
	}

	// Token: 0x06001ED3 RID: 7891 RVA: 0x000A42AF File Offset: 0x000A24AF
	public void BreakHandLinks()
	{
		this.leftHandLink.BreakLink();
		this.rightHandLink.BreakLink();
	}

	// Token: 0x06001ED4 RID: 7892 RVA: 0x000A42C7 File Offset: 0x000A24C7
	public bool IsInHandHoldChainWithOtherPlayer(int otherPlayer)
	{
		return TakeMyHand_HandLink.IsHandInChainWithOtherPlayer(this.leftHandLink, otherPlayer) || TakeMyHand_HandLink.IsHandInChainWithOtherPlayer(this.rightHandLink, otherPlayer);
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x000A42E5 File Offset: 0x000A24E5
	// (set) Token: 0x06001ED6 RID: 7894 RVA: 0x000A42ED File Offset: 0x000A24ED
	public float LastTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001ED7 RID: 7895 RVA: 0x000A42F6 File Offset: 0x000A24F6
	// (set) Token: 0x06001ED8 RID: 7896 RVA: 0x000A42FE File Offset: 0x000A24FE
	public float LastHandTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001ED9 RID: 7897 RVA: 0x000A4307 File Offset: 0x000A2507
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	// Token: 0x06001EDA RID: 7898 RVA: 0x000A4314 File Offset: 0x000A2514
	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001EDB RID: 7899 RVA: 0x000A4321 File Offset: 0x000A2521
	// (set) Token: 0x06001EDC RID: 7900 RVA: 0x000A4329 File Offset: 0x000A2529
	public GorillaSkin CurrentCosmeticSkin { get; set; }

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001EDD RID: 7901 RVA: 0x000A4332 File Offset: 0x000A2532
	// (set) Token: 0x06001EDE RID: 7902 RVA: 0x000A433A File Offset: 0x000A253A
	public GorillaSkin CurrentModeSkin { get; set; }

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001EDF RID: 7903 RVA: 0x000A4343 File Offset: 0x000A2543
	// (set) Token: 0x06001EE0 RID: 7904 RVA: 0x000A434B File Offset: 0x000A254B
	public GorillaSkin TemporaryEffectSkin { get; set; }

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001EE1 RID: 7905 RVA: 0x000A4354 File Offset: 0x000A2554
	// (set) Token: 0x06001EE2 RID: 7906 RVA: 0x000A435C File Offset: 0x000A255C
	public bool PostTickRunning { get; set; }

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000A4365 File Offset: 0x000A2565
	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001EE4 RID: 7908 RVA: 0x000A4396 File Offset: 0x000A2596
	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000A43A4 File Offset: 0x000A25A4
	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000A43AD File Offset: 0x000A25AD
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000A43BC File Offset: 0x000A25BC
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000A43CB File Offset: 0x000A25CB
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000A43F6 File Offset: 0x000A25F6
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000A4405 File Offset: 0x000A2605
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000A4430 File Offset: 0x000A2630
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000A443F File Offset: 0x000A263F
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000A446A File Offset: 0x000A266A
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000A4495 File Offset: 0x000A2695
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001EEF RID: 7919 RVA: 0x000A44A4 File Offset: 0x000A26A4
	// (set) Token: 0x06001EF0 RID: 7920 RVA: 0x000A44B1 File Offset: 0x000A26B1
	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06001EF1 RID: 7921 RVA: 0x000A44D8 File Offset: 0x000A26D8
	// (set) Token: 0x06001EF2 RID: 7922 RVA: 0x000A44E5 File Offset: 0x000A26E5
	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06001EF3 RID: 7923 RVA: 0x000A450C File Offset: 0x000A270C
	// (set) Token: 0x06001EF4 RID: 7924 RVA: 0x000A4519 File Offset: 0x000A2719
	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001EF5 RID: 7925 RVA: 0x000A4540 File Offset: 0x000A2740
	// (set) Token: 0x06001EF6 RID: 7926 RVA: 0x000A454D File Offset: 0x000A274D
	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001EF7 RID: 7927 RVA: 0x000A4579 File Offset: 0x000A2779
	// (set) Token: 0x06001EF8 RID: 7928 RVA: 0x000A4586 File Offset: 0x000A2786
	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000A45B2 File Offset: 0x000A27B2
	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000A45C4 File Offset: 0x000A27C4
	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000A45D8 File Offset: 0x000A27D8
	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000A45E1 File Offset: 0x000A27E1
	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001EFD RID: 7933 RVA: 0x000A45E9 File Offset: 0x000A27E9
	// (set) Token: 0x06001EFE RID: 7934 RVA: 0x000A45F6 File Offset: 0x000A27F6
	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06001EFF RID: 7935 RVA: 0x000A461D File Offset: 0x000A281D
	// (set) Token: 0x06001F00 RID: 7936 RVA: 0x000A462A File Offset: 0x000A282A
	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06001F01 RID: 7937 RVA: 0x000A4651 File Offset: 0x000A2851
	// (set) Token: 0x06001F02 RID: 7938 RVA: 0x000A465E File Offset: 0x000A285E
	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06001F03 RID: 7939 RVA: 0x000A4685 File Offset: 0x000A2885
	public float scaleFactor
	{
		get
		{
			return this.scaleMultiplier * this.nativeScale;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06001F04 RID: 7940 RVA: 0x000A4694 File Offset: 0x000A2894
	// (set) Token: 0x06001F05 RID: 7941 RVA: 0x000A469C File Offset: 0x000A289C
	public float ScaleMultiplier
	{
		get
		{
			return this.scaleMultiplier;
		}
		set
		{
			this.scaleMultiplier = value;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06001F06 RID: 7942 RVA: 0x000A46A5 File Offset: 0x000A28A5
	// (set) Token: 0x06001F07 RID: 7943 RVA: 0x000A46AD File Offset: 0x000A28AD
	public float NativeScale
	{
		get
		{
			return this.nativeScale;
		}
		set
		{
			this.nativeScale = value;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06001F08 RID: 7944 RVA: 0x000A46B6 File Offset: 0x000A28B6
	public NetPlayer Creator
	{
		get
		{
			return this.creator;
		}
	}

	// Token: 0x17000353 RID: 851
	// (get) Token: 0x06001F09 RID: 7945 RVA: 0x000A46BE File Offset: 0x000A28BE
	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06001F0A RID: 7946 RVA: 0x000A46C6 File Offset: 0x000A28C6
	// (set) Token: 0x06001F0B RID: 7947 RVA: 0x000A46CE File Offset: 0x000A28CE
	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
		set
		{
			this.speakingLoudness = value;
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06001F0C RID: 7948 RVA: 0x000A46D7 File Offset: 0x000A28D7
	internal HandEffectContext LeftHandEffect
	{
		get
		{
			return this._leftHandEffect;
		}
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06001F0D RID: 7949 RVA: 0x000A46DF File Offset: 0x000A28DF
	internal HandEffectContext RightHandEffect
	{
		get
		{
			return this._rightHandEffect;
		}
	}

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x06001F0E RID: 7950 RVA: 0x000A46E7 File Offset: 0x000A28E7
	internal HandEffectContext ExtraLeftHandEffect
	{
		get
		{
			return this._extraLeftHandEffect;
		}
	}

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x06001F0F RID: 7951 RVA: 0x000A46EF File Offset: 0x000A28EF
	internal HandEffectContext ExtraRightHandEffect
	{
		get
		{
			return this._extraRightHandEffect;
		}
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06001F10 RID: 7952 RVA: 0x000A46F7 File Offset: 0x000A28F7
	public GamePlayer GamePlayerRef
	{
		get
		{
			if (this._gamePlayerRef == null)
			{
				this._gamePlayerRef = base.GetComponent<GamePlayer>();
			}
			return this._gamePlayerRef;
		}
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000A471C File Offset: 0x000A291C
	public void BuildInitialize()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		this.lastPosition = base.transform.position;
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
		XRaySkeleton component2 = base.GetComponent<XRaySkeleton>();
		if (component2 == null)
		{
			return;
		}
		component2.OnBuildInitialize();
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000A47D0 File Offset: 0x000A29D0
	private void Awake()
	{
		this.cosmeticsObjectRegistry = new CosmeticItemRegistry(this);
		this.CosmeticsV2_Awake();
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateNameSafeAccount));
		if (this.isOfflineVRRig)
		{
			VRRig.gLocalRig = this;
			this.BuildInitialize();
		}
		this.SharedStart();
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000A4834 File Offset: 0x000A2A34
	private void ApplyColorCode()
	{
		float num = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", num);
		float float2 = PlayerPrefs.GetFloat("greenValue", num);
		float float3 = PlayerPrefs.GetFloat("blueValue", num);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000A4878 File Offset: 0x000A2A78
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.lastScaleFactor = this.scaleFactor;
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.bodyRenderer.SharedStart();
		this.initialized = false;
		if (this.isOfflineVRRig)
		{
			if (CosmeticsController.hasInstance && CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			}
			if (Application.platform == RuntimePlatform.Android && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		GorillaSkin.ShowActiveSkin(this);
		base.Invoke("ApplyColorCode", 1f);
		List<Material> list = new List<Material>();
		this.mainSkin.GetSharedMaterials(list);
		this.layerChanger = base.GetComponent<LayerChanger>();
		if (this.layerChanger != null)
		{
			this.layerChanger.InitializeLayers(base.transform);
		}
		this.frozenEffectMinY = this.frozenEffect.transform.localScale.y;
		this.frozenEffectMinHorizontalScale = this.frozenEffect.transform.localScale.x;
		this.rightIndex.Initialize();
		this.rightMiddle.Initialize();
		this.rightThumb.Initialize();
		this.leftIndex.Initialize();
		this.leftMiddle.Initialize();
		this.leftThumb.Initialize();
		this.cachedRenderTransformPos = this.renderTransform.localPosition;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000A4A4C File Offset: 0x000A2C4C
	public void SliceUpdate()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && global::GorillaGameModes.GameMode.ActiveNetworkHandler.IsNull())
		{
			global::GorillaGameModes.GameMode.LoadGameModeFromProperty();
		}
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000A4AB8 File Offset: 0x000A2CB8
	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return NetworkSystem.Instance.InRoom && GorillaGameManager.instance is GorillaPaintbrawlManager;
		}
		if (BuilderSetManager.instance.GetStarterSetsConcat().Contains(itemName))
		{
			return true;
		}
		if (this._playerOwnedCosmetics.Contains(itemName) || PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(this, itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000A4B3D File Offset: 0x000A2D3D
	public void ApplyLocalTrajectoryOverride(Vector3 overrideVelocity)
	{
		this.LocalTrajectoryOverrideBlend = 1f;
		this.LocalTrajectoryOverridePosition = base.transform.position;
		this.LocalTrajectoryOverrideVelocity = overrideVelocity;
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000A4B62 File Offset: 0x000A2D62
	public bool IsLocalTrajectoryOverrideActive()
	{
		return this.LocalTrajectoryOverrideBlend > 0f;
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000A4B71 File Offset: 0x000A2D71
	public void ApplyLocalGrabOverride(bool isBody, bool isLeftHand, Transform grabbingHand)
	{
		this.localOverrideIsBody = isBody;
		this.localOverrideIsLeftHand = isLeftHand;
		this.localOverrideGrabbingHand = grabbingHand;
		this.localGrabOverrideBlend = 1f;
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000A4B93 File Offset: 0x000A2D93
	public void ClearLocalGrabOverride()
	{
		this.localGrabOverrideBlend = -1f;
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000A4BA0 File Offset: 0x000A2DA0
	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			this.ScaleUpdate();
		}
		if (this.voiceAudio != null)
		{
			float? num = null;
			float? num2 = null;
			if (this.IsHaunted)
			{
				num = new float?(this.HauntedVoicePitch);
			}
			else if (this.UsingHauntedRing)
			{
				num = new float?(this.HauntedRingVoicePitch);
			}
			else
			{
				if (this.voiceShiftCosmeticsDirty)
				{
					this.cosmeticPitchShift = 0f;
					this.cosmeticVolumeShift = 0f;
					this.anyShiftedVoiceCosmetic = false;
					int num3 = 0;
					int num4 = 0;
					for (int i = 0; i < this.VoiceShiftCosmetics.Count; i++)
					{
						VoiceShiftCosmetic voiceShiftCosmetic = this.VoiceShiftCosmetics[i];
						if (voiceShiftCosmetic.IsShifted)
						{
							this.anyShiftedVoiceCosmetic = true;
							if (voiceShiftCosmetic.ModifyPitch)
							{
								this.cosmeticPitchShift += voiceShiftCosmetic.Pitch;
								num3++;
							}
							if (voiceShiftCosmetic.ModifyVolume)
							{
								this.cosmeticVolumeShift += voiceShiftCosmetic.Volume;
								num4++;
							}
						}
					}
					this.cosmeticPitchActive = num3 > 0;
					this.cosmeticVolumeActive = num4 > 0;
					if (this.cosmeticPitchActive)
					{
						this.cosmeticPitchShift /= (float)num3;
					}
					if (this.cosmeticVolumeActive)
					{
						this.cosmeticVolumeShift /= (float)num4;
					}
					this.voiceShiftCosmeticsDirty = false;
				}
				if (this.anyShiftedVoiceCosmetic)
				{
					if (this.cosmeticPitchActive)
					{
						num = new float?(this.cosmeticPitchShift);
					}
					if (this.cosmeticVolumeActive)
					{
						num2 = new float?(this.cosmeticVolumeShift);
					}
				}
				else
				{
					float num5 = GorillaTagger.Instance.offlineVRRig.scaleFactor / this.scaleFactor;
					float num6 = this.voicePitchForRelativeScale.Evaluate(num5);
					if (float.IsNaN(num6) || num6 <= 0f)
					{
						Debug.LogError("Voice pitch curve is invalid, please fix!");
					}
					else
					{
						num = new float?(num6);
					}
				}
			}
			if (num != null && !Mathf.Approximately(this.voiceAudio.pitch, num.Value))
			{
				this.voiceAudio.pitch = num.Value;
			}
			if (num2 != null && !Mathf.Approximately(this.voiceAudio.volume, num2.Value))
			{
				this.voiceAudio.volume = num2.Value;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 vector;
				if (this.grabbedRopeIsLeft)
				{
					vector = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					vector = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, vector, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += vector;
				}
			}
			else if (this.currentHoldParent)
			{
				Transform transform;
				if (this.grabbedRopeIsBody)
				{
					transform = this.bodyTransform;
				}
				else if (this.grabbedRopeIsLeft)
				{
					transform = this.leftHandTransform;
				}
				else
				{
					transform = this.rightHandTransform;
				}
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - transform.position;
			}
			else if (this.mountedMonkeBlock || this.mountedMovingSurface)
			{
				Transform transform2 = (this.movingSurfaceIsMonkeBlock ? this.mountedMonkeBlock.transform : this.mountedMovingSurface.transform);
				Vector3 vector2 = Vector3.zero;
				Vector3 vector3 = this.jobPos - base.transform.position;
				Transform transform3;
				if (this.mountedMovingSurfaceIsBody)
				{
					transform3 = this.bodyTransform;
				}
				else if (this.mountedMovingSurfaceIsLeft)
				{
					transform3 = this.leftHandTransform;
				}
				else
				{
					transform3 = this.rightHandTransform;
				}
				vector2 = transform2.TransformPoint(this.mountedMonkeBlockOffset) - (transform3.position + vector3);
				if (this.shouldLerpToMovingSurface)
				{
					this.lastMountedSurfaceTimer += Time.deltaTime;
					this.jobPos += Vector3.Lerp(Vector3.zero, vector2, this.lastMountedSurfaceTimer * 4f);
					if (this.lastMountedSurfaceTimer * 4f >= 1f)
					{
						this.shouldLerpToMovingSurface = false;
					}
				}
				else
				{
					this.jobPos += vector2;
				}
			}
			else if (this.InOverrideSubscriptionZone)
			{
				this.jobPos = this.OverrideSubscriptionZoneLocation;
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		if (this.LocalTrajectoryOverrideBlend > 0f)
		{
			this.LocalTrajectoryOverrideBlend -= Time.deltaTime / this.LocalTrajectoryOverrideDuration;
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			Vector3 vector4;
			Vector3 vector5;
			if (this.LocalTestMovementCollision(this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideVelocity, out vector4, out vector5))
			{
				this.LocalTrajectoryOverrideVelocity = vector4;
				this.LocalTrajectoryOverridePosition = vector5;
			}
			else
			{
				this.LocalTrajectoryOverridePosition += this.LocalTrajectoryOverrideVelocity * Time.deltaTime;
			}
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			this.jobPos = Vector3.Lerp(this.jobPos, this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideBlend);
		}
		else if (this.localGrabOverrideBlend > 0f)
		{
			this.localGrabOverrideBlend -= Time.deltaTime / this.LocalGrabOverrideDuration;
			if (this.localOverrideGrabbingHand != null)
			{
				Transform transform4;
				if (this.localOverrideIsBody)
				{
					transform4 = this.bodyTransform;
				}
				else if (this.localOverrideIsLeftHand)
				{
					transform4 = this.leftHandTransform;
				}
				else
				{
					transform4 = this.rightHandTransform;
				}
				this.jobPos += this.localOverrideGrabbingHand.TransformPoint(this.grabbedRopeOffset) - transform4.position;
			}
		}
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
		}
		else
		{
			this.jobRotation = this.SanitizeQuaternion(this.syncRotation);
		}
		this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
		this.head.MapOther(this.lerpValueBody);
		this.rightHand.MapOther(this.lerpValueBody);
		this.leftHand.MapOther(this.lerpValueBody);
		this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
		this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
		this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
		this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
		this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
		this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
		this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
		this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000A53FC File Offset: 0x000A35FC
	private void ScaleUpdate()
	{
		this.frameScale = Mathf.MoveTowards(this.lastScaleFactor, this.scaleFactor, Time.deltaTime * 4f);
		base.transform.localScale = Vector3.one * this.frameScale;
		this.lastScaleFactor = this.frameScale;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000A5452 File Offset: 0x000A3652
	public void AddLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Add(in action);
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000A5461 File Offset: 0x000A3661
	public void RemoveLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Remove(in action);
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000A5474 File Offset: 0x000A3674
	public void PostTick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				instance.jumpMultiplier = this.speedArray[1];
				instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				instance.jumpMultiplier = 1.1f;
				instance.maxJumpSpeed = 6.5f;
			}
			this.nativeScale = instance.NativeScale;
			this.scaleMultiplier = instance.ScaleMultiplier;
			if (this.scaleFactor != this.lastScaleFactor)
			{
				this.ScaleUpdate();
			}
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.lastScaleFactor + base.transform.rotation * this.headBodyOffset * this.lastScaleFactor;
			base.transform.SetPositionAndRotation(this.syncPos, GTPlayerTransform.BodyRotation);
			this.head.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			bool flag = instance.IsGroundedHand || instance.IsThrusterActive;
			bool isGroundedButt = instance.IsGroundedButt;
			bool isLeftGrabbing = EquipmentInteractor.instance.isLeftGrabbing;
			bool flag2 = isLeftGrabbing && EquipmentInteractor.instance.CanGrabLeft();
			bool isRightGrabbing = EquipmentInteractor.instance.isRightGrabbing;
			bool flag3 = isRightGrabbing && EquipmentInteractor.instance.CanGrabRight();
			this.LastTouchedGroundAtNetworkTime = instance.LastTouchedGroundAtNetworkTime;
			this.LastHandTouchedGroundAtNetworkTime = instance.LastHandTouchedGroundAtNetworkTime;
			TakeMyHand_HandLink takeMyHand_HandLink = this.leftHandLink;
			if (takeMyHand_HandLink != null)
			{
				takeMyHand_HandLink.LocalUpdate(flag, isGroundedButt, isLeftGrabbing, flag2);
			}
			TakeMyHand_HandLink takeMyHand_HandLink2 = this.rightHandLink;
			if (takeMyHand_HandLink2 != null)
			{
				takeMyHand_HandLink2.LocalUpdate(flag, isGroundedButt, isRightGrabbing, flag3);
			}
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.bodyRenderer.ActiveBody.enabled = !instance.inOverlay;
			int i = this.loudnessCheckFrame - 1;
			this.loudnessCheckFrame = i;
			if (i < 0)
			{
				this.SpeakingLoudness = 0f;
				if (this.shouldSendSpeakingLoudness && this.netView)
				{
					PhotonVoiceView component = this.netView.GetComponent<PhotonVoiceView>();
					if (component && component.RecorderInUse)
					{
						MicWrapper micWrapper = component.RecorderInUse.InputSource as MicWrapper;
						if (micWrapper != null)
						{
							int num = this.replacementVoiceDetectionDelay;
							if (num > this.voiceSampleBuffer.Length)
							{
								Array.Resize<float>(ref this.voiceSampleBuffer, num);
							}
							float[] array = this.voiceSampleBuffer;
							if (micWrapper.Mic != null && micWrapper.Mic.samples >= num && micWrapper.Mic.GetData(array, micWrapper.Mic.samples - num))
							{
								float num2 = 0f;
								for (int j = 0; j < num; j++)
								{
									float num3 = Mathf.Sqrt(array[j]);
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
								this.SpeakingLoudness = num2;
							}
						}
					}
				}
				this.loudnessCheckFrame = 10;
			}
			if (PhotonNetwork.InRoom && Time.time > this.nextLocalVelocityStoreTimestamp)
			{
				this.AddVelocityToQueue(base.transform.position, PhotonNetwork.Time);
				this.nextLocalVelocityStoreTimestamp = Time.time + 0.1f;
			}
		}
		if (this.leftHandLink.IsLinkActive())
		{
			VRRig myRig = this.leftHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig.IsVisuallyDisplaced)
			{
				this.leftHandLink.BreakLink();
			}
			else
			{
				this.leftHandLink.VisuallySnapHandsTogether();
			}
		}
		if (this.rightHandLink.IsLinkActive())
		{
			VRRig myRig2 = this.rightHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig2.IsVisuallyDisplaced)
			{
				this.rightHandLink.BreakLink();
			}
			else
			{
				this.rightHandLink.VisuallySnapHandsTogether();
			}
		}
		if (this.creator != null)
		{
			if (GorillaGameManager.instance != null)
			{
				GorillaGameManager.instance.UpdatePlayerAppearance(this);
			}
			else if (this.setMatIndex != 0)
			{
				this.ChangeMaterialLocal(0);
				this.ForceResetFrozenEffect();
			}
		}
		if (!this.isLocal && (this.displacementZone != null || this.renderTransformDisplaced))
		{
			Vector3 vector = base.transform.position + this.cachedRenderTransformPos;
			Vector3 vector2 = ((this.displacementZone != null) ? this.displacementZone.GetDisplacementForRig(this, vector) : Vector3.zero);
			if (vector2 == Vector3.zero)
			{
				this.renderTransform.localPosition = this.cachedRenderTransformPos;
			}
			else
			{
				this.renderTransform.position = vector + vector2;
			}
			this.myIk.renderDisplacement = vector2;
			this.renderTransformDisplaced = vector2 != Vector3.zero;
		}
		if (this.frozenEffect.activeSelf)
		{
			GorillaFreezeTagManager gorillaFreezeTagManager = GorillaGameManager.instance as GorillaFreezeTagManager;
			if (gorillaFreezeTagManager != null)
			{
				this.UpdateFrozen(Time.deltaTime, gorillaFreezeTagManager.freezeDuration);
			}
		}
		if (this.TemporaryCosmeticEffects.Count > 0)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
			{
				if (Time.time - keyValuePair.Value.EffectStartedTime >= keyValuePair.Value.EffectDuration)
				{
					this.RemoveTemporaryCosmeticEffects(keyValuePair);
				}
			}
		}
		this.lateUpdateCallbacks.TryRunCallbacks();
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x000A5A9C File Offset: 0x000A3C9C
	public void UpdateFrozen(float dt, float freezeDuration)
	{
		Vector3 localScale = this.frozenEffect.transform.localScale;
		Vector3 vector = localScale;
		vector.y = Mathf.Lerp(this.frozenEffectMinY, this.frozenEffectMaxY, this.frozenTimeElapsed / freezeDuration);
		localScale = new Vector3(localScale.x, vector.y, localScale.z);
		this.frozenEffect.transform.localScale = localScale;
		this.frozenTimeElapsed += dt;
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000A5B14 File Offset: 0x000A3D14
	private void RemoveTemporaryCosmeticEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin)
		{
			bool flag;
			if (effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
		{
			this.DisableHitWithKnockBack(effect);
		}
		this.TemporaryCosmeticEffects.Remove(effect.Key);
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000A5B87 File Offset: 0x000A3D87
	public void SpawnSkinEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GorillaSkin.ApplyToRig(this, effect.Value.newSkin, GorillaSkin.SkinType.temporaryEffect);
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000A5BB6 File Offset: 0x000A3DB6
	public void EnableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000A5BD4 File Offset: 0x000A3DD4
	private void DisableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key) && effect.Value.knockbackVFX)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.knockbackVFX, base.transform.position, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(base.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000A5C60 File Offset: 0x000A3E60
	public void DisableHitWithKnockBack()
	{
		foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> keyValuePair in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
		{
			bool flag;
			if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
			{
				this.DisableHitWithKnockBack(keyValuePair);
				this.TemporaryCosmeticEffects.Remove(keyValuePair.Key);
			}
			else if (keyValuePair.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin && keyValuePair.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == keyValuePair.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
				this.TemporaryCosmeticEffects.Remove(keyValuePair.Key);
			}
		}
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x000A5BB6 File Offset: 0x000A3DB6
	public void ApplyInstanceKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000A5BB6 File Offset: 0x000A3DB6
	public void ActivateVOEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000A5D0E File Offset: 0x000A3F0E
	public bool TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE key, out CosmeticEffectsOnPlayers.CosmeticEffect value)
	{
		if (this.TemporaryCosmeticEffects == null)
		{
			value = null;
			return false;
		}
		return this.TemporaryCosmeticEffects.TryGetValue(key, out value);
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000A5D2C File Offset: 0x000A3F2C
	public void PlayCosmeticEffectSFX(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
		int num = global::UnityEngine.Random.Range(0, effect.Value.sfxAudioClip.Count);
		this.tagSound.PlayOneShot(effect.Value.sfxAudioClip[num]);
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000A5D88 File Offset: 0x000A3F88
	public void SpawnVFXEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.VFXGameObject, base.transform.position, true);
		if (gameObject != null)
		{
			gameObject.gameObject.transform.SetParent(base.transform);
			gameObject.gameObject.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06001F2B RID: 7979 RVA: 0x000A5DEC File Offset: 0x000A3FEC
	public bool IsPlayerMeshHidden
	{
		get
		{
			return !this.mainSkin.enabled;
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000A5DFC File Offset: 0x000A3FFC
	public void SetPlayerMeshHidden(bool hide)
	{
		this.mainSkin.enabled = !hide;
		this.faceSkin.enabled = !hide;
		this.nameTagAnchor.SetActive(!hide);
		this.UpdateMatParticles(-1);
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000A5E32 File Offset: 0x000A4032
	public void SetInvisibleToLocalPlayer(bool invisible)
	{
		if (this.IsInvisibleToLocalPlayer == invisible)
		{
			return;
		}
		this.IsInvisibleToLocalPlayer = invisible;
		this.nameTagAnchor.SetActive(!invisible);
		this.UpdateFriendshipBracelet();
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000A5E5A File Offset: 0x000A405A
	public void ChangeLayer(string layerName)
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.ChangeLayer(base.transform.parent, layerName);
		}
		GTPlayer.Instance.ChangeLayer(layerName);
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000A5E8C File Offset: 0x000A408C
	public void RestoreLayer()
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.RestoreOriginalLayers();
		}
		GTPlayer.Instance.RestoreLayer();
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000A5EB1 File Offset: 0x000A40B1
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000A5EC4 File Offset: 0x000A40C4
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000A5F8E File Offset: 0x000A418E
	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000A5FD0 File Offset: 0x000A41D0
	private InputStruct SerializeWriteShared()
	{
		if (this.myIk == null)
		{
			this.myIk = base.GetComponent<GorillaIK>();
		}
		InputStruct inputStruct = new InputStruct
		{
			headRotation = BitPackUtils.PackQuaternionForNetwork(this.head.rigTarget.localRotation),
			rotation = BitPackUtils.PackQuaternionForNetwork(base.transform.rotation),
			usingNewIK = this.ShouldUseNewIKMethod(this.myIk.usingUpdatedIK)
		};
		if (inputStruct.usingNewIK)
		{
			inputStruct.bodyRotation = BitPackUtils.PackQuaternionForNetwork(this.myIk.targetBodyRot);
			inputStruct.rightUpperArmRotation = (short)BitPackUtils.PackRelativePos16(this.myIk.rightElbowDirection, Vector3.zero, 1f);
			inputStruct.leftUpperArmRotation = (short)BitPackUtils.PackRelativePos16(this.myIk.leftElbowDirection, Vector3.zero, 1f);
		}
		inputStruct.rightHandLong = BitPackUtils.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation);
		inputStruct.leftHandLong = BitPackUtils.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation);
		inputStruct.position = BitPackUtils.PackWorldPosForNetwork(base.transform.position);
		inputStruct.handPosition = this.ReturnHandPosition();
		inputStruct.taggedById = (short)this.taggedById;
		int num = 0;
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.SpeakingLoudness) * 255f);
		bool flag = this.leftHandLink.IsLinkActive() || this.rightHandLink.IsLinkActive();
		GorillaGameManager activeGameMode = global::GorillaGameModes.GameMode.ActiveGameMode;
		bool flag2 = activeGameMode != null && activeGameMode.GameType() == GameModeType.PropHunt;
		int num3 = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex != -1) ? 1024 : 0) + (this.grabbedRopeIsPhotonView ? 2048 : 0) + (flag ? 4096 : 0) + (this.hoverboardVisual.IsHeld ? 8192 : 0) + (this.hoverboardVisual.IsLeftHanded ? 16384 : 0) + ((this.mountedMovingSurfaceId != -1) ? 32768 : 0) + (flag2 ? 65536 : 0) + (this.propHuntHandFollower.IsLeftHand ? 131072 : 0) + (this.leftHandLink.CanBeGrabbed() ? 262144 : 0) + (this.rightHandLink.CanBeGrabbed() ? 524288 : 0) + (this.leftHandLink.IsTentacleGrab ? 1048576 : 0) + (this.rightHandLink.IsTentacleGrab ? 2097152 : 0) + (this.ShowGoldNameTag ? 4194304 : 0) + (this.portalShenanigansBit ? 8388608 : 0) + (num2 << 24);
		inputStruct.packedFields = num3;
		inputStruct.packedCompetitiveData = this.PackCompetitiveData();
		if (this.grabbedRopeIndex != -1)
		{
			inputStruct.grabbedRopeIndex = this.grabbedRopeIndex;
			inputStruct.ropeBoneIndex = this.grabbedRopeBoneIndex;
			inputStruct.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			inputStruct.ropeGrabIsBody = this.grabbedRopeIsBody;
			inputStruct.ropeGrabOffset = this.grabbedRopeOffset;
		}
		if (this.grabbedRopeIndex == -1 && this.mountedMovingSurfaceId != -1)
		{
			inputStruct.grabbedRopeIndex = this.mountedMovingSurfaceId;
			inputStruct.ropeGrabIsLeft = this.mountedMovingSurfaceIsLeft;
			inputStruct.ropeGrabIsBody = this.mountedMovingSurfaceIsBody;
			inputStruct.ropeGrabOffset = this.mountedMonkeBlockOffset;
		}
		if (this.hoverboardVisual.IsHeld)
		{
			inputStruct.hoverboardPosRot = BitPackUtils.PackHandPosRotForNetwork(this.hoverboardVisual.NominalLocalPosition, this.hoverboardVisual.NominalLocalRotation);
			inputStruct.hoverboardColor = BitPackUtils.PackColorForNetwork(this.hoverboardVisual.boardColor);
		}
		if (flag2)
		{
			inputStruct.propHuntPosRot = this.propHuntHandFollower.GetRelativePosRotLong();
		}
		if (flag)
		{
			this.leftHandLink.Write(out inputStruct.isGroundedHand, out inputStruct.isGroundedButt, out inputStruct.leftHandGrabbedActorNumber, out inputStruct.leftGrabbedHandIsLeft);
			this.rightHandLink.Write(out inputStruct.isGroundedHand, out inputStruct.isGroundedButt, out inputStruct.rightHandGrabbedActorNumber, out inputStruct.rightGrabbedHandIsLeft);
			inputStruct.lastTouchedGroundAtTime = this.LastTouchedGroundAtNetworkTime;
			inputStruct.lastHandTouchedGroundAtTime = this.LastHandTouchedGroundAtNetworkTime;
		}
		inputStruct.packedGTPlayerStats = GTPlayerStats.GetPackedValues();
		inputStruct.gtPlayerStatsFlags = (int)GTPlayerStats.SystemPropertiesFlags;
		return inputStruct;
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000A6428 File Offset: 0x000A4628
	private void SerializeReadShared(InputStruct data)
	{
		if (this.myIk == null)
		{
			this.myIk = base.GetComponent<GorillaIK>();
		}
		VRMap vrmap = this.head;
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.headRotation);
		(ref vrmap.syncRotation).SetValueSafe(in quaternion);
		bool flag = this.ShouldUseNewIKMethod(data.usingNewIK);
		this.myIk.usingUpdatedIK = flag;
		if (this.myIk.usingUpdatedIK)
		{
			GorillaIK gorillaIK = this.myIk;
			quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.bodyRotation);
			(ref gorillaIK.targetBodyRot).SetValueSafe(in quaternion);
			GorillaIK gorillaIK2 = this.myIk;
			Vector3 vector = BitPackUtils.UnpackRelativePos16((ushort)data.leftUpperArmRotation, Vector3.zero, 1f, false);
			(ref gorillaIK2.leftElbowDirection).SetValueSafe(in vector);
			GorillaIK gorillaIK3 = this.myIk;
			vector = BitPackUtils.UnpackRelativePos16((ushort)data.rightUpperArmRotation, Vector3.zero, 1f, false);
			(ref gorillaIK3.rightElbowDirection).SetValueSafe(in vector);
		}
		BitPackUtils.UnpackHandPosRotFromNetwork(data.rightHandLong, out this.tempVec, out this.tempQuat);
		this.rightHand.syncPos = this.tempVec;
		(ref this.rightHand.syncRotation).SetValueSafe(in this.tempQuat);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.leftHandLong, out this.tempVec, out this.tempQuat);
		this.leftHand.syncPos = this.tempVec;
		(ref this.leftHand.syncRotation).SetValueSafe(in this.tempQuat);
		this.syncPos = BitPackUtils.UnpackWorldPosFromNetwork(data.position);
		this.handSync = data.handPosition;
		quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.rotation);
		(ref this.syncRotation).SetValueSafe(in quaternion);
		int packedFields = data.packedFields;
		this.remoteUseReplacementVoice = (packedFields & 512) != 0;
		this.portalShenanigansBit = (packedFields & 8388608) != 0;
		if ((packedFields & 4194304) != 0 && SubscriptionManager.GetSubscriptionDetails(this).active)
		{
			this.playerText1.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
		}
		else
		{
			this.playerText1.color = Color.white;
		}
		int num = (packedFields >> 24) & 255;
		this.SpeakingLoudness = (float)num / 255f;
		this.UpdateReplacementVoice();
		this.UnpackCompetitiveData(data.packedCompetitiveData);
		this.taggedById = (int)data.taggedById;
		bool flag2 = (packedFields & 1024) != 0;
		this.grabbedRopeIsPhotonView = (packedFields & 2048) != 0;
		if (flag2)
		{
			this.grabbedRopeIndex = data.grabbedRopeIndex;
			this.grabbedRopeBoneIndex = data.ropeBoneIndex;
			this.grabbedRopeIsLeft = data.ropeGrabIsLeft;
			this.grabbedRopeIsBody = data.ropeGrabIsBody;
			(ref this.grabbedRopeOffset).SetValueSafe(in data.ropeGrabOffset);
		}
		else
		{
			this.grabbedRopeIndex = -1;
		}
		bool flag3 = (packedFields & 32768) != 0;
		if (!flag2 && flag3)
		{
			this.mountedMovingSurfaceId = data.grabbedRopeIndex;
			this.mountedMovingSurfaceIsLeft = data.ropeGrabIsLeft;
			this.mountedMovingSurfaceIsBody = data.ropeGrabIsBody;
			(ref this.mountedMonkeBlockOffset).SetValueSafe(in data.ropeGrabOffset);
			this.movingSurfaceIsMonkeBlock = data.movingSurfaceIsMonkeBlock;
		}
		else
		{
			this.mountedMovingSurfaceId = -1;
		}
		bool flag4 = (packedFields & 8192) != 0;
		bool flag5 = (packedFields & 16384) != 0;
		if (flag4)
		{
			Vector3 vector2;
			Quaternion quaternion2;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.hoverboardPosRot, out vector2, out quaternion2);
			Color color = BitPackUtils.UnpackColorFromNetwork(data.hoverboardColor);
			if ((in quaternion2).IsValid())
			{
				this.hoverboardVisual.SetIsHeld(flag5, vector2.ClampMagnitudeSafe(1f), quaternion2, color);
			}
		}
		else if (this.hoverboardVisual.gameObject.activeSelf)
		{
			this.hoverboardVisual.SetNotHeld();
		}
		if ((packedFields & 65536) != 0)
		{
			bool flag6 = (packedFields & 131072) != 0;
			Vector3 vector3;
			Quaternion quaternion3;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.propHuntPosRot, out vector3, out quaternion3);
			this.propHuntHandFollower.SetProp(flag6, vector3, quaternion3);
		}
		if (this.grabbedRopeIsPhotonView)
		{
			this.localGrabOverrideBlend = -1f;
		}
		this.leftHandLink.Read(this.leftHand.syncPos, this.syncRotation, this.syncPos, data.isGroundedHand, data.isGroundedButt, (packedFields & 262144) != 0, (packedFields & 1048576) != 0, data.leftHandGrabbedActorNumber, data.leftGrabbedHandIsLeft);
		this.rightHandLink.Read(this.rightHand.syncPos, this.syncRotation, this.syncPos, data.isGroundedHand, data.isGroundedButt, (packedFields & 524288) != 0, (packedFields & 2097152) != 0, data.rightHandGrabbedActorNumber, data.rightGrabbedHandIsLeft);
		this.LastTouchedGroundAtNetworkTime = data.lastTouchedGroundAtTime;
		this.LastHandTouchedGroundAtNetworkTime = data.lastHandTouchedGroundAtTime;
		this.UpdateRopeData();
		this.UpdateMovingMonkeBlockData();
		this.rigContainer.PlayerStats = GTPlayerStats.UnPackValues(data.packedGTPlayerStats, data.gtPlayerStatsFlags);
		this.AddVelocityToQueue(this.syncPos, data.serverTimeStamp);
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000A68C9 File Offset: 0x000A4AC9
	private bool ShouldUseNewIKMethod(bool isReceivingNewIKData)
	{
		if (this.isOfflineVRRig)
		{
			return SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT) && this.myIk != null && this.myIk.usingUpdatedIK;
		}
		return isReceivingNewIKData;
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000A68F8 File Offset: 0x000A4AF8
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		stream.SendNext(inputStruct.headRotation);
		stream.SendNext(inputStruct.rotation);
		stream.SendNext(inputStruct.usingNewIK);
		if (inputStruct.usingNewIK)
		{
			stream.SendNext(inputStruct.bodyRotation);
			stream.SendNext(inputStruct.leftUpperArmRotation);
			stream.SendNext(inputStruct.rightUpperArmRotation);
		}
		stream.SendNext(inputStruct.rightHandLong);
		stream.SendNext(inputStruct.leftHandLong);
		stream.SendNext(inputStruct.position);
		stream.SendNext(inputStruct.handPosition);
		stream.SendNext(inputStruct.packedFields);
		stream.SendNext(inputStruct.packedCompetitiveData);
		if (this.grabbedRopeIndex != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeBoneIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
		}
		else if (this.mountedMovingSurfaceId != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
			stream.SendNext(inputStruct.movingSurfaceIsMonkeBlock);
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			stream.SendNext(inputStruct.hoverboardPosRot);
			stream.SendNext(inputStruct.hoverboardColor);
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			stream.SendNext(inputStruct.isGroundedHand);
			stream.SendNext(inputStruct.isGroundedButt);
			stream.SendNext(inputStruct.leftHandGrabbedActorNumber);
			stream.SendNext(inputStruct.leftGrabbedHandIsLeft);
			stream.SendNext(inputStruct.rightHandGrabbedActorNumber);
			stream.SendNext(inputStruct.rightGrabbedHandIsLeft);
			stream.SendNext(inputStruct.lastTouchedGroundAtTime);
			stream.SendNext(inputStruct.lastHandTouchedGroundAtTime);
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			stream.SendNext(inputStruct.propHuntPosRot);
		}
		stream.SendNext(inputStruct.packedGTPlayerStats);
		stream.SendNext(inputStruct.gtPlayerStatsFlags);
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000A6BA8 File Offset: 0x000A4DA8
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		double sentServerTime = info.SentServerTime;
		InputStruct inputStruct = new InputStruct
		{
			headRotation = (int)stream.ReceiveNext(),
			rotation = (int)stream.ReceiveNext(),
			usingNewIK = (bool)stream.ReceiveNext()
		};
		if (inputStruct.usingNewIK)
		{
			inputStruct.bodyRotation = (int)stream.ReceiveNext();
			inputStruct.leftUpperArmRotation = (short)stream.ReceiveNext();
			inputStruct.rightUpperArmRotation = (short)stream.ReceiveNext();
		}
		inputStruct.rightHandLong = (long)stream.ReceiveNext();
		inputStruct.leftHandLong = (long)stream.ReceiveNext();
		inputStruct.position = (long)stream.ReceiveNext();
		inputStruct.handPosition = (int)stream.ReceiveNext();
		inputStruct.packedFields = (int)stream.ReceiveNext();
		inputStruct.packedCompetitiveData = (short)stream.ReceiveNext();
		bool flag = (inputStruct.packedFields & 1024) != 0;
		bool flag2 = (inputStruct.packedFields & 32768) != 0;
		if (flag)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeBoneIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		else if (flag2)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			inputStruct.hoverboardPosRot = (long)stream.ReceiveNext();
			inputStruct.hoverboardColor = (short)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			inputStruct.isGroundedHand = (bool)stream.ReceiveNext();
			inputStruct.isGroundedButt = (bool)stream.ReceiveNext();
			inputStruct.leftHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.leftGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.rightHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.rightGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.lastTouchedGroundAtTime = (float)stream.ReceiveNext();
			inputStruct.lastHandTouchedGroundAtTime = (float)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			inputStruct.propHuntPosRot = (long)stream.ReceiveNext();
		}
		inputStruct.packedGTPlayerStats = (long)stream.ReceiveNext();
		inputStruct.gtPlayerStatsFlags = (int)stream.ReceiveNext();
		inputStruct.serverTimeStamp = info.SentServerTime;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000A6E9C File Offset: 0x000A509C
	public object OnSerializeWrite()
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		double num = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = num;
		return inputStruct;
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000A6ED8 File Offset: 0x000A50D8
	public void OnSerializeRead(object objectData)
	{
		InputStruct inputStruct = (InputStruct)objectData;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000A6EF4 File Offset: 0x000A50F4
	private void UpdateExtrapolationTarget()
	{
		float num = (float)(NetworkSystem.Instance.SimTime - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000A6FA0 File Offset: 0x000A51A0
	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft && this.previousGrabbedRopeWasBody == this.grabbedRopeIsBody)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex != -1)
		{
			GorillaRopeSwing gorillaRopeSwing;
			if (this.grabbedRopeIsPhotonView)
			{
				PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
				GorillaClimbable gorillaClimbable;
				HandHoldXSceneRef handHoldXSceneRef;
				VRRigSerializer vrrigSerializer;
				if (photonView.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
				{
					this.currentHoldParent = photonView.transform;
				}
				else if (photonView.TryGetComponent<HandHoldXSceneRef>(out handHoldXSceneRef))
				{
					GameObject targetObject = handHoldXSceneRef.targetObject;
					this.currentHoldParent = ((targetObject != null) ? targetObject.transform : null);
				}
				else if (photonView && photonView.TryGetComponent<VRRigSerializer>(out vrrigSerializer))
				{
					this.currentHoldParent = ((this.grabbedRopeBoneIndex == 1) ? vrrigSerializer.VRRig.leftHandHoldsPlayer.transform : vrrigSerializer.VRRig.rightHandHoldsPlayer.transform);
				}
			}
			else if (RopeSwingManager.instance.TryGetRope(this.grabbedRopeIndex, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
		}
		else if (this.previousGrabbedRope != -1)
		{
			PhotonView photonView2 = PhotonView.Find(this.previousGrabbedRope);
			VRRigSerializer vrrigSerializer2;
			if (photonView2 && photonView2.TryGetComponent<VRRigSerializer>(out vrrigSerializer2) && vrrigSerializer2.VRRig == VRRig.LocalRig)
			{
				EquipmentInteractor.instance.ForceDropEquipment(this.bodyHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.leftHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.rightHolds);
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
		this.previousGrabbedRopeWasBody = this.grabbedRopeIsBody;
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000A71E0 File Offset: 0x000A53E0
	private void UpdateMovingMonkeBlockData()
	{
		if (this.mountedMonkeBlockOffset.sqrMagnitude > 2f)
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.prevMovingSurfaceID == this.mountedMovingSurfaceId && this.movingSurfaceWasBody == this.mountedMovingSurfaceIsBody && this.movingSurfaceWasLeft == this.mountedMovingSurfaceIsLeft && this.movingSurfaceWasMonkeBlock == this.movingSurfaceIsMonkeBlock)
		{
			return;
		}
		if (this.mountedMovingSurfaceId == -1)
		{
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		else if (this.movingSurfaceIsMonkeBlock)
		{
			this.mountedMonkeBlock = null;
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.zoneEntity.currentZone, out builderTable))
			{
				this.mountedMonkeBlock = builderTable.GetPiece(this.mountedMovingSurfaceId);
			}
			if (this.mountedMonkeBlock == null)
			{
				this.mountedMovingSurfaceId = -1;
				this.mountedMovingSurfaceIsLeft = false;
				this.mountedMovingSurfaceIsBody = false;
				this.mountedMonkeBlock = null;
				this.mountedMovingSurface = null;
			}
		}
		else if (MovingSurfaceManager.instance == null || !MovingSurfaceManager.instance.TryGetMovingSurface(this.mountedMovingSurfaceId, out this.mountedMovingSurface))
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.mountedMovingSurfaceId != -1 && this.prevMovingSurfaceID == -1)
		{
			this.shouldLerpToMovingSurface = true;
			this.lastMountedSurfaceTimer = 0f;
		}
		this.prevMovingSurfaceID = this.mountedMovingSurfaceId;
		this.movingSurfaceWasLeft = this.mountedMovingSurfaceIsLeft;
		this.movingSurfaceWasBody = this.mountedMovingSurfaceIsBody;
		this.movingSurfaceWasMonkeBlock = this.movingSurfaceIsMonkeBlock;
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000A738C File Offset: 0x000A558C
	public static void AttachLocalPlayerToMovingSurface(int blockId, bool isLeft, bool isBody, Vector3 offset, bool isMonkeBlock)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = blockId;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsLeft = isLeft;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsBody = isBody;
			GorillaTagger.Instance.offlineVRRig.movingSurfaceIsMonkeBlock = isMonkeBlock;
			GorillaTagger.Instance.offlineVRRig.mountedMonkeBlockOffset = offset;
		}
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x000A7402 File Offset: 0x000A5602
	public static void DetachLocalPlayerFromMovingSurface()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = -1;
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000A742C File Offset: 0x000A562C
	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = xrNode == XRNode.LeftHand;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = true;
		}
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x000A7499 File Offset: 0x000A5699
	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000A74C4 File Offset: 0x000A56C4
	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000A751B File Offset: 0x000A571B
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000A7534 File Offset: 0x000A5734
	public void UpdateFrozenEffect(bool enable)
	{
		if (this.frozenEffect != null && ((!this.frozenEffect.activeSelf && enable) || (this.frozenEffect.activeSelf && !enable)))
		{
			this.frozenEffect.SetActive(enable);
			if (enable)
			{
				this.frozenTimeElapsed = 0f;
			}
			else
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				localScale = new Vector3(localScale.x, this.frozenEffectMinY, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
			}
		}
		if (this.iceCubeLeft != null && ((!this.iceCubeLeft.activeSelf && enable) || (this.iceCubeLeft.activeSelf && !enable)))
		{
			this.iceCubeLeft.SetActive(enable);
		}
		if (this.iceCubeRight != null && ((!this.iceCubeRight.activeSelf && enable) || (this.iceCubeRight.activeSelf && !enable)))
		{
			this.iceCubeRight.SetActive(enable);
		}
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000A7640 File Offset: 0x000A5840
	public void ForceResetFrozenEffect()
	{
		this.frozenEffect.SetActive(false);
		this.iceCubeRight.SetActive(false);
		this.iceCubeLeft.SetActive(false);
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000A7668 File Offset: 0x000A5868
	public void ChangeMaterialLocal(int materialIndex)
	{
		if (this.setMatIndex == materialIndex)
		{
			return;
		}
		int num = this.setMatIndex;
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.bodyRenderer.SetMaterialIndex(materialIndex);
		}
		this.UpdateMatParticles(materialIndex);
		if (materialIndex > 0 && VRRig.LocalRig != this)
		{
			this.PlayTaggedEffect();
		}
		Action<int, int> onMaterialIndexChanged = this.OnMaterialIndexChanged;
		if (onMaterialIndexChanged == null)
		{
			return;
		}
		onMaterialIndexChanged(num, this.setMatIndex);
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000A76E8 File Offset: 0x000A58E8
	public void PlayTaggedEffect()
	{
		TagEffectPack tagEffectPack = null;
		quaternion quaternion = base.transform.rotation;
		TagEffectsLibrary.EffectType effectType = ((VRRig.LocalRig == this) ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON);
		if (GorillaGameManager.instance != null && this.OwningNetPlayer != null)
		{
			GorillaGameManager.instance.lastTaggedActorNr.TryGetValue(this.OwningNetPlayer.ActorNumber, out this.taggedById);
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.taggedById);
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			tagEffectPack = rigContainer.Rig.CosmeticEffectPack;
			if (tagEffectPack && tagEffectPack.shouldFaceTagger && effectType == TagEffectsLibrary.EffectType.THIRD_PERSON)
			{
				quaternion = Quaternion.LookRotation((rigContainer.Rig.transform.position - base.transform.position).normalized);
			}
		}
		TagEffectsLibrary.PlayEffect(base.transform, false, this.scaleFactor, effectType, this.CosmeticEffectPack, tagEffectPack, quaternion);
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000A77EC File Offset: 0x000A59EC
	public void ToggleMatParticles(bool enabled)
	{
		if (this.lavaParticleSystem != null)
		{
			this.ToggleParticleSystem(this.lavaParticleSystem, enabled);
		}
		if (this.rockParticleSystem != null)
		{
			this.ToggleParticleSystem(this.rockParticleSystem, enabled);
		}
		if (this.iceParticleSystem != null)
		{
			this.ToggleParticleSystem(this.iceParticleSystem, enabled);
		}
		if (this.snowFlakeParticleSystem != null)
		{
			this.ToggleParticleSystem(this.snowFlakeParticleSystem, enabled);
		}
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000A7868 File Offset: 0x000A5A68
	private void ToggleParticleSystem(ParticleSystem ps, bool enabled)
	{
		ps.emission.enabled = enabled;
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000A7884 File Offset: 0x000A5A84
	public void UpdateMatParticles(int materialIndex)
	{
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
		if (this.snowFlakeParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 14 && this.snowFlakeParticleSystem.isStopped)
			{
				this.snowFlakeParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.snowFlakeParticleSystem.isPlaying)
			{
				this.snowFlakeParticleSystem.Stop();
			}
		}
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000A79E4 File Offset: 0x000A5BE4
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_InitializeNoobMaterial");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID)) || (this.initialized && CosmeticWardrobeProximityDetector.IsUserNearWardrobe(info.senderID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x000A7AC8 File Offset: 0x000A5CC8
	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		color.r = Mathf.Clamp(color.r, 0f, 1f);
		color.g = Mathf.Clamp(color.g, 0f, 1f);
		color.b = Mathf.Clamp(color.b, 0f, 1f);
		this.bodyRenderer.UpdateColor(color);
		this.SetColor(color);
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		this.UpdateName(flag);
	}

	// Token: 0x06001F4D RID: 8013 RVA: 0x000A7B54 File Offset: 0x000A5D54
	public void UpdateNameSafeAccount(bool isSafeAccount)
	{
		this.UpdateName(!isSafeAccount);
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x000A7B60 File Offset: 0x000A5D60
	public void UpdateName(bool isNamePermissionEnabled)
	{
		if (!this.isOfflineVRRig && this.creator != null)
		{
			string text = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? this.creator.NickName : this.creator.DefaultName);
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? NetworkSystem.Instance.GetMyNickName() : NetworkSystem.Instance.GetMyDefaultName());
		}
		this.SetNameTagText(this.playerNameVisible);
		if (this.creator != null)
		{
			this.creator.SanitizedNickName = this.playerNameVisible;
		}
		Action onPlayerNameVisibleChanged = this.OnPlayerNameVisibleChanged;
		if (onPlayerNameVisibleChanged == null)
		{
			return;
		}
		onPlayerNameVisibleChanged();
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x000A7C2E File Offset: 0x000A5E2E
	public void SetNameTagText(string name)
	{
		this.playerNameVisible = name;
		this.playerText1.text = name;
		Action<RigContainer> onNameChanged = this.OnNameChanged;
		if (onNameChanged == null)
		{
			return;
		}
		onNameChanged(this.rigContainer);
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x000A7C5C File Offset: 0x000A5E5C
	public void UpdateName()
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
		bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
		this.UpdateName(flag);
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000A7C98 File Offset: 0x000A5E98
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 12);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000A7D25 File Offset: 0x000A5F25
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GTPlayer.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000A7D32 File Offset: 0x000A5F32
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GTPlayer.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000A7D40 File Offset: 0x000A5F40
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
		if (this.netView.IsMine)
		{
			this.netView.GetView.RPC("RPC_InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x000A7DE8 File Offset: 0x000A5FE8
	public void RequestCosmetics(PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (this.netView.IsMine && CosmeticsController.hasInstance)
		{
			if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
			{
				this.netView.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return;
			}
			int[] array = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
			int[] array2 = CosmeticsController.instance.tryOnSet.ToPackedIDArray();
			this.netView.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", player, new object[] { array, array2, false });
			CosmeticCollectionDisplay.GetDisplaysForRig(GorillaTagger.Instance.offlineVRRig, this.scratchDisplayList);
			if (this.scratchDisplayList.Count > 0)
			{
				int num = this.scratchDisplayList.Count * 3;
				if (this.cycleStatesArray.Length != num)
				{
					this.cycleStatesArray = new int[num];
				}
				for (int i = 0; i < this.scratchDisplayList.Count; i++)
				{
					CosmeticCollectionDisplay cosmeticCollectionDisplay = this.scratchDisplayList[i];
					string parentPlayFabID = cosmeticCollectionDisplay.ParentPlayFabID;
					this.cycleStatesArray[i * 3] = (int)(parentPlayFabID[0] - 'A' + '\u001a' * (parentPlayFabID[1] - 'A' + '\u001a' * (parentPlayFabID[2] - 'A' + '\u001a' * (parentPlayFabID[3] - 'A' + '\u001a' * (parentPlayFabID[4] - 'A')))));
					CosmeticsController.CosmeticItem? activeCollectable = cosmeticCollectionDisplay.ActiveCollectable;
					this.cycleStatesArray[i * 3 + 1] = ((activeCollectable != null && CosmeticsController.hasInstance) ? CosmeticsController.instance.GetCanonicalCollectableIndex(parentPlayFabID, activeCollectable.Value.itemName) : cosmeticCollectionDisplay.ActiveIndex);
					this.cycleStatesArray[i * 3 + 2] = cosmeticCollectionDisplay.VisibleMask;
				}
				this.netView.SendRPC("RPC_UpdateCosmeticsWithCollectablesPacked", player, new object[] { this.cycleStatesArray });
			}
		}
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x000A7FE8 File Offset: 0x000A61E8
	public void PlayTagSoundLocal(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		if (stopCurrentAudio)
		{
			this.tagSound.Stop();
		}
		this.tagSound.GTPlayOneShot(this.clipToPlay[soundIndex], 1f);
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x000A8041 File Offset: 0x000A6241
	public void AssignDrumToMusicDrums(int drumIndex, AudioSource drum)
	{
		if (drumIndex >= 0 && drumIndex < this.musicDrums.Length && drum != null)
		{
			this.musicDrums[drumIndex] = drum;
		}
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000A8064 File Offset: 0x000A6264
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlayDrum");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.senderRig = rigContainer.Rig;
		}
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent drum", player.UserId, player.NickName);
			return;
		}
		AudioSource audioSource = (this.netView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex]);
		if (!audioSource.gameObject.activeInHierarchy)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.GTPlay();
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000A8198 File Offset: 0x000A6398
	public int AssignInstrumentToInstrumentSelfOnly(TransferrableObject instrument)
	{
		if (instrument == null)
		{
			return -1;
		}
		if (!this.instrumentSelfOnly.Contains(instrument))
		{
			this.instrumentSelfOnly.Add(instrument);
		}
		return this.instrumentSelfOnly.IndexOf(instrument);
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000A81CC File Offset: 0x000A63CC
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySelfOnlyInstrument");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Count && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				MonkeAgent.instance.SendReport("inappropriate tag data being sent self only instrument", player.UserId, player.NickName);
			}
		}
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x000A82A8 File Offset: 0x000A64A8
	public void PlayHandTapLocal(int audioClipIndex, bool isLeftHand, float tapVolume)
	{
		if (audioClipIndex > -1 && audioClipIndex < GTPlayer.Instance.materialData.Count)
		{
			GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[audioClipIndex];
			AudioSource audioSource = (isLeftHand ? this.leftHandPlayer : this.rightHandPlayer);
			audioSource.volume = tapVolume;
			AudioClip audioClip = (materialData.overrideAudio ? materialData.audio : GTPlayer.Instance.materialData[0].audio);
			audioSource.GTPlayOneShot(audioClip, 1f);
		}
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x000A8325 File Offset: 0x000A6525
	internal HandEffectContext GetHandEffect(bool isLeftHand, StiltID stiltID)
	{
		if (stiltID == StiltID.None)
		{
			if (!isLeftHand)
			{
				return this.RightHandEffect;
			}
			return this.LeftHandEffect;
		}
		else
		{
			if (!isLeftHand)
			{
				return this.ExtraRightHandEffect;
			}
			return this.ExtraLeftHandEffect;
		}
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x000A834C File Offset: 0x000A654C
	internal void SetHandEffectData(HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
	{
		VRMap vrmap = (isLeftHand ? this.leftHand : this.rightHand);
		Vector3 vector = dirFromHitToHand * this.tapPointDistance * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			Vector3 vector2 = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			Vector3 vector3 = ((stiltID != StiltID.None) ? GTPlayer.Instance.GetHandPosition(isLeftHand, stiltID) : (vrmap.rigTarget.position - vector2 + vector));
			effectContext.position = vector3;
			effectContext.handSoundSource.transform.position = vector3;
		}
		else
		{
			Quaternion quaternion = vrmap.rigTarget.parent.rotation * vrmap.syncRotation;
			Vector3 vector4 = this.netSyncPos.GetPredictedFuture() - base.transform.position;
			Vector3 vector2 = quaternion * vrmap.trackingPositionOffset * this.scaleFactor;
			effectContext.position = vrmap.rigTarget.parent.TransformPoint(vrmap.netSyncPos.GetPredictedFuture()) - vector2 + vector + vector4;
		}
		GTPlayer.MaterialData handSurfaceData = this.GetHandSurfaceData(audioClipIndex);
		HandTapOverrides handTapOverrides = (isDownTap ? effectContext.DownTapOverrides : effectContext.UpTapOverrides);
		List<int> prefabHashes = effectContext.prefabHashes;
		int num = 0;
		HashWrapper hashWrapper = (handTapOverrides.overrideSurfacePrefab ? handTapOverrides.surfaceTapPrefab : GTPlayer.Instance.materialDatasSO.surfaceEffects[handSurfaceData.surfaceEffectIndex]);
		prefabHashes[num] = in hashWrapper;
		effectContext.prefabHashes[1] = (ref handTapOverrides.overrideGamemodePrefab ? in handTapOverrides.gamemodeTapPrefab : (RoomSystem.JoinedRoom && global::GorillaGameModes.GameMode.ActiveGameMode.IsNotNull()) ? global::GorillaGameModes.GameMode.ActiveGameMode.SpecialHandFX(this.creator, this.rigContainer) : (-1));
		effectContext.soundFX = (handTapOverrides.overrideSound ? handTapOverrides.tapSound : handSurfaceData.audio);
		effectContext.isDownTap = isDownTap;
		effectContext.isLeftHand = isLeftHand;
		effectContext.soundVolume = handTapVolume * this.handSpeedToVolumeModifier;
		effectContext.soundPitch = 1f;
		effectContext.speed = handTapSpeed;
		effectContext.color = this.playerColor;
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000A8588 File Offset: 0x000A6788
	internal GTPlayer.MaterialData GetHandSurfaceData(int index)
	{
		List<GTPlayer.MaterialData> materialData = GTPlayer.Instance.materialData;
		GTPlayer.MaterialData materialData2;
		if (index >= 0 && index < materialData.Count)
		{
			materialData2 = materialData[index];
		}
		else
		{
			materialData2 = materialData[0];
		}
		if (!materialData2.overrideAudio)
		{
			materialData2 = materialData[0];
		}
		return materialData2;
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000A85D0 File Offset: 0x000A67D0
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySplashEffect");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner)
		{
			float num = 10000f;
			if ((in splashPosition).IsValid(in num) && (in splashRotation).IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
			{
				if ((base.transform.position - splashPosition).sqrMagnitude >= 9f)
				{
					return;
				}
				float time = Time.time;
				int num2 = -1;
				float num3 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num3)
					{
						num3 = this.splashEffectTimes[i];
						num2 = i;
					}
				}
				if (time - 0.5f > num3)
				{
					this.splashEffectTimes[num2] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
				return;
			}
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent splash effect", player.UserId, player.NickName);
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000A876C File Offset: 0x000A696C
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_EnableNonCosmeticHandItem(bool enable, bool isLeftHand, RpcInfo info = default(RpcInfo))
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.IncrementRPC(photonMessageInfoWrapped, "EnableNonCosmeticHandItem");
		if (photonMessageInfoWrapped.Sender == this.creator)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(photonMessageInfoWrapped.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
		}
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000A8828 File Offset: 0x000A6A28
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		NetPlayer sender = info.Sender;
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (sender == this.netView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000A88DC File Offset: 0x000A6ADC
	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000A8934 File Offset: 0x000A6B34
	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000A898C File Offset: 0x000A6B8C
	public bool IsMakingFiveLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) < 0.25f;
		}
		return this.leftIndex.calcT < 0.25f && this.leftMiddle.calcT < 0.25f;
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000A89E4 File Offset: 0x000A6BE4
	public bool IsMakingFiveRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) < 0.25f;
		}
		return this.rightIndex.calcT < 0.25f && this.rightMiddle.calcT < 0.25f;
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000A8A3C File Offset: 0x000A6C3C
	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000A8A68 File Offset: 0x000A6C68
	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.GTPlay();
		}
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000A8AB0 File Offset: 0x000A6CB0
	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.GTPlayOneShot(this.leftHandPlayer.clip, 1f);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.GTPlayOneShot(this.rightHandPlayer.clip, 1f);
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000A8B30 File Offset: 0x000A6D30
	public void HideAllCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "HideAllCosmetics");
		if (NetworkSystem.Instance.GetPlayer(info.Sender) == this.netView.Owner)
		{
			this.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000A8BA0 File Offset: 0x000A6DA0
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length == 16 && tryOnItems.Length == 16)
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet cosmeticSet2 = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmeticSet2, playfx);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000A8C34 File Offset: 0x000A6E34
	public void UpdateCosmeticsWithTryon(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && CosmeticsController.instance.ValidatePackedItems(currentItemsPacked) && CosmeticsController.instance.ValidatePackedItems(tryOnItemsPacked))
		{
			CosmeticsController.CosmeticSet cosmeticSet = new CosmeticsController.CosmeticSet(currentItemsPacked, CosmeticsController.instance);
			CosmeticsController.CosmeticSet cosmeticSet2 = new CosmeticsController.CosmeticSet(tryOnItemsPacked, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(cosmeticSet, cosmeticSet2, playfx);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000A8CD8 File Offset: 0x000A6ED8
	public void UpdateCosmeticsWithCollectables(int[] cycleStatesPacked, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithCollectablesPacked");
		if (info.Sender != this.netView.Owner || cycleStatesPacked == null || cycleStatesPacked.Length % 3 != 0 || cycleStatesPacked.Length > 96)
		{
			return;
		}
		int num = cycleStatesPacked.Length / 3;
		this.remoteCycleStates.Clear();
		char[] array = new char[] { '\0', '\0', '\0', '\0', '\0', '.' };
		for (int i = 0; i < num; i++)
		{
			int num2 = cycleStatesPacked[i * 3];
			int num3 = cycleStatesPacked[i * 3 + 1];
			int num4 = cycleStatesPacked[i * 3 + 2];
			if (num3 >= 0)
			{
				array[0] = (char)(65 + num2 % 26);
				array[1] = (char)(65 + num2 / 26 % 26);
				array[2] = (char)(65 + num2 / 676 % 26);
				array[3] = (char)(65 + num2 / 17576 % 26);
				array[4] = (char)(65 + num2 / 456976 % 26);
				string text = new string(array);
				CosmeticsController.CollectionState collectionState = new CosmeticsController.CollectionState
				{
					activeIndex = num3,
					visibleMask = num4
				};
				this.remoteCycleStates[text] = collectionState;
				CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.FindForRig(this, text);
				if (cosmeticCollectionDisplay != null)
				{
					cosmeticCollectionDisplay.SetVisibleMask(num4);
					cosmeticCollectionDisplay.SetActiveIndex(num3);
				}
			}
		}
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000A8E0C File Offset: 0x000A700C
	public void BroadcastSubCosmeticSignal(int packedParentID, int signal, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_BroadcastSubCosmeticSignal");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		char[] array = new char[] { '\0', '\0', '\0', '\0', '\0', '.' };
		array[0] = (ushort)(65 + packedParentID % 26);
		array[1] = (ushort)(65 + packedParentID / 26 % 26);
		array[2] = (ushort)(65 + packedParentID / 676 % 26);
		array[3] = (ushort)(65 + packedParentID / 17576 % 26);
		array[4] = (ushort)(65 + packedParentID / 456976 % 26);
		string text = new string(array);
		CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.FindForRig(this, text);
		if (cosmeticCollectionDisplay != null)
		{
			SubCosmeticCycleController subCosmeticCycleController = cosmeticCollectionDisplay.GetComponent<SubCosmeticCycleController>();
			if (subCosmeticCycleController == null)
			{
				subCosmeticCycleController = cosmeticCollectionDisplay.GetComponentInChildren<SubCosmeticCycleController>(true);
			}
			if (subCosmeticCycleController != null)
			{
				subCosmeticCycleController.ReceiveNetworkSignal(signal);
			}
		}
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000A8ED0 File Offset: 0x000A70D0
	public void SetCollectionCycleIndex(int packedParentID, int activeIndex, int visibleMask, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_SetCollectionCycleIndex");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		char[] array = new char[] { '\0', '\0', '\0', '\0', '\0', '.' };
		array[0] = (ushort)(65 + packedParentID % 26);
		array[1] = (ushort)(65 + packedParentID / 26 % 26);
		array[2] = (ushort)(65 + packedParentID / 676 % 26);
		array[3] = (ushort)(65 + packedParentID / 17576 % 26);
		array[4] = (ushort)(65 + packedParentID / 456976 % 26);
		string text = new string(array);
		if (this.remoteCycleStates.Count >= 64 && !this.remoteCycleStates.ContainsKey(text))
		{
			return;
		}
		CosmeticsController.CollectionState collectionState = new CosmeticsController.CollectionState
		{
			activeIndex = activeIndex,
			visibleMask = visibleMask
		};
		this.remoteCycleStates[text] = collectionState;
		CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.FindForRig(this, text);
		if (cosmeticCollectionDisplay != null)
		{
			cosmeticCollectionDisplay.SetVisibleMask(visibleMask);
			cosmeticCollectionDisplay.SetActiveIndex(activeIndex);
		}
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000A8FC0 File Offset: 0x000A71C0
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet, bool playfx)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive(playfx);
		}
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000A8FE0 File Offset: 0x000A71E0
	private void CheckForEarlyAccess()
	{
		CosmeticInfoV2 info = CosmeticsController.instance.EarlyAccessSupporterPackCosmeticSO.info;
		if (this._playerOwnedCosmetics.Contains(info.playFabID))
		{
			CosmeticSO[] setCosmetics = info.setCosmetics;
			for (int i = 0; i < setCosmetics.Length; i++)
			{
				CosmeticInfoV2 info2 = setCosmetics[i].info;
				this._playerOwnedCosmetics.Add(info2.playFabID);
			}
		}
		this.InitializedCosmetics = true;
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000A904C File Offset: 0x000A724C
	public void SetCosmeticsActive(bool playfx)
	{
		if (CosmeticsController.instance == null)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, this.cosmeticsObjectRegistry);
		if (!playfx)
		{
			return;
		}
		if (this.cosmeticsActivationPS != null)
		{
			this.cosmeticsActivationPS.Play();
		}
		if (this.cosmeticsActivationSBP != null)
		{
			this.cosmeticsActivationSBP.Play();
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000A90F2 File Offset: 0x000A72F2
	public void RefreshCosmetics()
	{
		this.mergedSet.ActivateCosmetics(this.mergedSet, this, this.myBodyDockPositions, this.cosmeticsObjectRegistry);
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000A9114 File Offset: 0x000A7314
	public void GetCosmeticsPlayFabCatalogData()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
					{
						if (itemInstance.PurchaseDate != null)
						{
							int num = (int)(DateTime.UtcNow - itemInstance.PurchaseDate.Value).TotalDays;
							this.AddCosmetic(itemInstance.ItemId, num);
						}
						else
						{
							this.AddCosmetic(itemInstance.ItemId, 0);
						}
					}
				}
			}, delegate(PlayFabError error)
			{
				this.initializedCosmetics = true;
			}, null, null);
		}
		this.AddCosmetic("Slingshot", 0);
		foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.StartPieceSets)
		{
			this.AddCosmetic(builderPieceSet.playfabID, 0);
		}
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000A91B4 File Offset: 0x000A73B4
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000A920C File Offset: 0x000A740C
	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			thumb.angle1Table[i] = Quaternion.Lerp(thumb.startingAngle1Quat, thumb.closedAngle1Quat, (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(thumb.startingAngle2Quat, thumb.closedAngle2Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000A929C File Offset: 0x000A749C
	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(index.startingAngle1Quat, index.closedAngle1Quat, (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(index.startingAngle2Quat, index.closedAngle2Quat, (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(index.startingAngle3Quat, index.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000A9364 File Offset: 0x000A7564
	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(middle.startingAngle1Quat, middle.closedAngle1Quat, (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(middle.startingAngle2Quat, middle.closedAngle2Quat, (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(middle.startingAngle3Quat, middle.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000A942C File Offset: 0x000A762C
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000A94A8 File Offset: 0x000A76A8
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 5000f);
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000A9514 File Offset: 0x000A7714
	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			MonkeAgent.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000A952A File Offset: 0x000A772A
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			MonkeAgent.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000A9540 File Offset: 0x000A7740
	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 vector = Vector3.zero;
		if (this.velocityHistoryList.Count > 0)
		{
			double num = Utils.CalculateNetworkDeltaTime(this.velocityHistoryList[0].time, serverTime);
			if (num == 0.0)
			{
				return;
			}
			vector = (position - this.lastPosition) / (float)num;
		}
		this.velocityHistoryList.Add(new VRRig.VelocityTime(vector, serverTime));
		this.lastPosition = position;
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000A95B4 File Offset: 0x000A77B4
	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	// Token: 0x06001F7E RID: 8062 RVA: 0x000A96C6 File Offset: 0x000A78C6
	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x000A96ED File Offset: 0x000A78ED
	public bool IsPositionInRange(Vector3 position, float range)
	{
		return (this.syncPos - position).IsShorterThan(range * this.scaleFactor);
	}

	// Token: 0x06001F80 RID: 8064 RVA: 0x000A9708 File Offset: 0x000A7908
	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 vector;
		Vector3 vector2;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out vector, out vector2);
		return Vector3.SqrMagnitude(vector - vector2) < max * max * this.scaleFactor;
	}

	// Token: 0x06001F81 RID: 8065 RVA: 0x000A9764 File Offset: 0x000A7964
	public Vector3 ClampVelocityRelativeToPlayerSafe(Vector3 inVel, float max, float teleportSpeedThreshold = 100f)
	{
		max *= this.scaleFactor;
		Vector3 vector = Vector3.zero;
		(ref vector).SetValueSafe(in inVel);
		Vector3 vector2 = ((this.velocityHistoryList.Count > 0) ? this.velocityHistoryList[0].vel : Vector3.zero);
		if (vector2.sqrMagnitude > teleportSpeedThreshold * teleportSpeedThreshold)
		{
			vector2 = Vector3.zero;
		}
		Vector3 vector3 = vector - vector2;
		vector3 = Vector3.ClampMagnitude(vector3, max);
		vector = vector2 + vector3;
		return vector;
	}

	// Token: 0x14000043 RID: 67
	// (add) Token: 0x06001F82 RID: 8066 RVA: 0x000A97DC File Offset: 0x000A79DC
	// (remove) Token: 0x06001F83 RID: 8067 RVA: 0x000A9814 File Offset: 0x000A7A14
	public event Action<Color> OnColorChanged;

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x06001F84 RID: 8068 RVA: 0x000A984C File Offset: 0x000A7A4C
	// (remove) Token: 0x06001F85 RID: 8069 RVA: 0x000A9884 File Offset: 0x000A7A84
	public event Action OnPlayerNameVisibleChanged;

	// Token: 0x06001F86 RID: 8070 RVA: 0x000A98BC File Offset: 0x000A7ABC
	public void SetColor(Color color)
	{
		Action<Color> onColorChanged = this.OnColorChanged;
		if (onColorChanged != null)
		{
			onColorChanged(color);
		}
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000A9933 File Offset: 0x000A7B33
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000A9961 File Offset: 0x000A7B61
	private void SendScoresToRoom()
	{
		if (this.netView != null && this._scoreUpdated)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[] { this.currentQuestScore });
		}
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000A99A0 File Offset: 0x000A7BA0
	private void SendScoresToGameModeRoom(GameModeType newGameModeType)
	{
		if (this.netView != null && this._rankedInfoUpdated && newGameModeType != GameModeType.InfectionCompetitive && !this.m_sentRankedScore)
		{
			this.m_sentRankedScore = true;
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[] { this.currentRankedELO, this.currentRankedSubTierQuest, this.currentRankedSubTierPC });
		}
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000A9A18 File Offset: 0x000A7C18
	private void SendScoresToNewPlayer(NetPlayer player)
	{
		if (this.netView != null)
		{
			if (this._scoreUpdated)
			{
				this.netView.SendRPC("RPC_UpdateQuestScore", player, new object[] { this.currentQuestScore });
			}
			if (this._rankedInfoUpdated && !this.IsInRankedMode())
			{
				this.netView.SendRPC("RPC_UpdateRankedInfo", player, new object[] { this.currentRankedELO, this.currentRankedSubTierQuest, this.currentRankedSubTierPC });
			}
		}
	}

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x06001F8B RID: 8075 RVA: 0x000A9AB4 File Offset: 0x000A7CB4
	// (remove) Token: 0x06001F8C RID: 8076 RVA: 0x000A9AEC File Offset: 0x000A7CEC
	public event Action<int> OnQuestScoreChanged;

	// Token: 0x06001F8D RID: 8077 RVA: 0x000A9B24 File Offset: 0x000A7D24
	public void SetQuestScore(int score)
	{
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged != null)
		{
			onQuestScoreChanged(this.currentQuestScore);
		}
		if (this.netView != null)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[] { this.currentQuestScore });
		}
	}

	// Token: 0x06001F8E RID: 8078 RVA: 0x000A9B82 File Offset: 0x000A7D82
	public int GetCurrentQuestScore()
	{
		if (!this._scoreUpdated)
		{
			this.SetQuestScoreLocal(ProgressionController.TotalPoints);
		}
		return this.currentQuestScore;
	}

	// Token: 0x06001F8F RID: 8079 RVA: 0x000A9B9D File Offset: 0x000A7D9D
	private void SetQuestScoreLocal(int score)
	{
		this.currentQuestScore = score;
		this._scoreUpdated = true;
	}

	// Token: 0x06001F90 RID: 8080 RVA: 0x000A9BB0 File Offset: 0x000A7DB0
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateQuestScore");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.senderID != this.creator.ActorNumber)
		{
			return;
		}
		if (!this.updateQuestCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		if (score < this.currentQuestScore)
		{
			return;
		}
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged == null)
		{
			return;
		}
		onQuestScoreChanged(this.currentQuestScore);
	}

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x06001F91 RID: 8081 RVA: 0x000A9C28 File Offset: 0x000A7E28
	// (remove) Token: 0x06001F92 RID: 8082 RVA: 0x000A9C60 File Offset: 0x000A7E60
	public event Action<int, int> OnRankedSubtierChanged;

	// Token: 0x06001F93 RID: 8083 RVA: 0x000A9C98 File Offset: 0x000A7E98
	public void SetRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, bool broadcastToOtherClients = true)
	{
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		if (this.netView != null && broadcastToOtherClients)
		{
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[] { this.currentRankedELO, this.currentRankedSubTierQuest, this.currentRankedSubTierPC });
		}
	}

	// Token: 0x06001F94 RID: 8084 RVA: 0x000A9D13 File Offset: 0x000A7F13
	public int GetCurrentRankedSubTier(bool getPC)
	{
		if (!this._rankedInfoUpdated)
		{
			return -1;
		}
		if (!getPC)
		{
			return this.currentRankedSubTierQuest;
		}
		return this.currentRankedSubTierPC;
	}

	// Token: 0x06001F95 RID: 8085 RVA: 0x000A9D2F File Offset: 0x000A7F2F
	private void SetRankedInfoLocal(float rankedELO, int rankedSubTierQuest, int rankedSubTierPC)
	{
		this.currentRankedELO = rankedELO;
		this.currentRankedSubTierQuest = rankedSubTierQuest;
		this.currentRankedSubTierPC = rankedSubTierPC;
		this._rankedInfoUpdated = true;
	}

	// Token: 0x06001F96 RID: 8086 RVA: 0x000A9D4D File Offset: 0x000A7F4D
	private bool IsInRankedMode()
	{
		return global::GorillaGameModes.GameMode.ActiveGameMode != null && global::GorillaGameModes.GameMode.ActiveGameMode.GameType() == GameModeType.InfectionCompetitive;
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000A9D6C File Offset: 0x000A7F6C
	public void UpdateRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateRankedInfo");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		if (!rigContainer.Rig.updateRankedInfoCallLimit.CheckCallTime(Time.time) || info.senderID != this.creator.ActorNumber || !float.IsFinite(rankedELO))
		{
			return;
		}
		if (this.IsInRankedMode())
		{
			return;
		}
		if (RankedProgressionManager.Instance == null || !RankedProgressionManager.Instance.AreValuesValid(rankedELO, rankedSubtierQuest, rankedSubtierPC))
		{
			return;
		}
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		RankedProgressionManager.Instance.HandlePlayerRankedInfoReceived(this.creator.ActorNumber, rankedELO, rankedSubtierPC);
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x000A9E3C File Offset: 0x000A803C
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnSubscriptionData));
		GorillaComputer.RegisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
		this.bodyRenderer.SetDefaults();
		this.SetInvisibleToLocalPlayer(false);
		this.ReactivateAllRenderers();
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride += this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride += this.HandHold_HandPositionReleaseOverride;
			global::GorillaGameModes.GameMode.OnStartGameMode += this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent += new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.RegisterVRRig(this);
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001F99 RID: 8089 RVA: 0x000A9F58 File Offset: 0x000A8158
	public void OnSubscriptionData()
	{
		if (!this.isOfflineVRRig)
		{
			return;
		}
		this.showGoldNameTag = SubscriptionManager.IsLocalSubscribed() && PlayerPrefs.GetInt(SubscriptionManager.GetSubsFeatureKey(SubscriptionManager.SubscriptionFeatures.GoldenName)) > 0;
		if (this.showGoldNameTag)
		{
			this.playerText1.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
			return;
		}
		this.playerText1.color = Color.white;
	}

	// Token: 0x06001F9A RID: 8090 RVA: 0x000A9FB8 File Offset: 0x000A81B8
	void IPreDisable.PreDisable()
	{
		try
		{
			this.ClearRopeData();
			if (this.currentRopeSwingTarget)
			{
				this.currentRopeSwingTarget.SetParent(base.transform);
			}
			this.EnableHuntWatch(false);
			this.EnablePaintbrawlCosmetics(false);
			this.EnableSuperInfectionHands(false);
			this.ClearPartyMemberStatus();
			this._playerOwnedCosmetics.Clear();
			this._playerOwnedCosmeticsAge.Clear();
			if (this.cosmeticSet != null)
			{
				this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
				this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
				this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
				this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
			}
			if (!this.isOfflineVRRig)
			{
				PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
				this.pendingCosmeticUpdate = true;
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.rightHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.rightHandLink);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001F9B RID: 8091 RVA: 0x000AA138 File Offset: 0x000A8338
	public void OnDisable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnSubscriptionData));
		try
		{
			GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.gameMode);
			this.ChangeMaterialLocal(0);
			GorillaComputer.UnregisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
			this.netView = null;
			this.voiceAudio = null;
			this.muted = false;
			this.initialized = false;
			this.initializedCosmetics = false;
			this.inTryOnRoom = false;
			this.inTempCosmSpace = false;
			this.timeSpawned = 0f;
			this.setMatIndex = 0;
			this.currentCosmeticTries = 0;
			this.velocityHistoryList.Clear();
			this.netSyncPos.Reset();
			this.rightHand.netSyncPos.Reset();
			this.leftHand.netSyncPos.Reset();
			this.ForceResetFrozenEffect();
			this.nativeScale = (this.frameScale = (this.lastScaleFactor = 1f));
			base.transform.localScale = Vector3.one;
			this.currentQuestScore = 0;
			this._scoreUpdated = false;
			this.currentRankedELO = 0f;
			this.currentRankedSubTierQuest = 0;
			this.currentRankedSubTierPC = 0;
			this._rankedInfoUpdated = false;
			this.TemporaryCosmeticEffects.Clear();
			this.m_sentRankedScore = false;
			this.displacementZone = null;
			this.renderTransformDisplaced = false;
			this.renderTransform.localPosition = this.cachedRenderTransformPos;
			if (this.myIk != null)
			{
				this.myIk.renderDisplacement = Vector3.zero;
			}
			this.portalShenanigansBit = false;
			try
			{
				CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
				for (int i = 0; i < callSettings.Length; i++)
				{
					callSettings[i].CallLimitSettings.Reset();
				}
			}
			catch
			{
				Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
			}
		}
		catch (Exception)
		{
		}
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride -= this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride -= this.HandHold_HandPositionReleaseOverride;
			global::GorillaGameModes.GameMode.OnStartGameMode -= this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent -= new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
		}
		EyeScannerMono.Unregister(this);
		this.creator = null;
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001F9C RID: 8092 RVA: 0x000AA3C4 File Offset: 0x000A85C4
	private void HandHold_HandPositionReleaseOverride(HandHold hh, bool leftHand)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = null;
			return;
		}
		this.rightHand.handholdOverrideTarget = null;
	}

	// Token: 0x06001F9D RID: 8093 RVA: 0x000AA3E2 File Offset: 0x000A85E2
	private void HandHold_HandPositionRequestOverride(HandHold hh, bool leftHand, Vector3 pos)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = hh.transform;
			this.leftHand.handholdOverrideTargetOffset = pos;
			return;
		}
		this.rightHand.handholdOverrideTarget = hh.transform;
		this.rightHand.handholdOverrideTargetOffset = pos;
	}

	// Token: 0x06001F9E RID: 8094 RVA: 0x000AA424 File Offset: 0x000A8624
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaPaintbrawlManager || instance.GameModeName() == "PAINTBRAWL")
				{
					this.EnablePaintbrawlCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("PAINTBRAWL"))
					{
						this.EnablePaintbrawlCosmetics(true);
					}
				}
			}
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
			}
		}
		if (this.netView != null)
		{
			base.transform.position = this.netView.gameObject.transform.position;
			base.transform.rotation = this.netView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
		}
	}

	// Token: 0x06001F9F RID: 8095 RVA: 0x000AA580 File Offset: 0x000A8780
	public void GrabbedByPlayer(VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		GorillaClimbable gorillaClimbable = (grabbedWithLeftHand ? grabbedByRig.leftHandHoldsPlayer : grabbedByRig.rightHandHoldsPlayer);
		GorillaHandClimber gorillaHandClimber;
		if (grabbedBody)
		{
			gorillaHandClimber = EquipmentInteractor.instance.BodyClimber;
		}
		else if (grabbedLeftHand)
		{
			gorillaHandClimber = EquipmentInteractor.instance.LeftClimber;
		}
		else
		{
			gorillaHandClimber = EquipmentInteractor.instance.RightClimber;
		}
		gorillaHandClimber.SetCanRelease(false);
		GTPlayer.Instance.BeginClimbing(gorillaClimbable, gorillaHandClimber, null);
		this.grabbedRopeIsBody = grabbedBody;
		this.grabbedRopeIsLeft = grabbedLeftHand;
		this.grabbedRopeIndex = grabbedByRig.netView.ViewID;
		this.grabbedRopeBoneIndex = (grabbedWithLeftHand ? 1 : 0);
		this.grabbedRopeOffset = Vector3.zero;
		this.grabbedRopeIsPhotonView = true;
	}

	// Token: 0x06001FA0 RID: 8096 RVA: 0x000AA624 File Offset: 0x000A8824
	public void DroppedByPlayer(VRRig grabbedByRig, Vector3 throwVelocity)
	{
		GorillaClimbable currentClimbable = GTPlayer.Instance.CurrentClimbable;
		if (GTPlayer.Instance.isClimbing && (currentClimbable == grabbedByRig.leftHandHoldsPlayer || currentClimbable == grabbedByRig.rightHandHoldsPlayer))
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 20f);
			GorillaHandClimber currentClimber = GTPlayer.Instance.CurrentClimber;
			GTPlayer.Instance.EndClimbing(currentClimber, false, false);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			this.grabbedRopeIsBody = false;
			this.grabbedRopeIsLeft = false;
			this.grabbedRopeIndex = -1;
			this.grabbedRopeBoneIndex = 0;
			this.grabbedRopeOffset = Vector3.zero;
			this.grabbedRopeIsPhotonView = false;
			return;
		}
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() && VRRig.LocalRig.leftHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.leftHandLink.BreakLink();
			VRRig.LocalRig.leftHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			return;
		}
		if (VRRig.LocalRig.rightHandLink.IsLinkActive() && VRRig.LocalRig.rightHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.rightHandLink.BreakLink();
			VRRig.LocalRig.rightHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
		}
	}

	// Token: 0x06001FA1 RID: 8097 RVA: 0x000AA794 File Offset: 0x000A8994
	public bool IsOnGround(float headCheckDistance, float handCheckDistance, out Vector3 groundNormal)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(position, Vector3.down * headCheckDistance * this.scaleFactor, instance.headCollider.radius * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		if (this.LocalCheckCollision(position2, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		if (this.LocalCheckCollision(position3, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		groundNormal = Vector3.up;
		return false;
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x000AA8A8 File Offset: 0x000A8AA8
	private bool LocalTestMovementCollision(Vector3 startPosition, Vector3 startVelocity, out Vector3 modifiedVelocity, out Vector3 finalPosition)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = startVelocity * Time.deltaTime;
		finalPosition = startPosition + vector;
		modifiedVelocity = startVelocity;
		Vector3 vector2;
		RaycastHit raycastHit;
		bool flag = this.LocalCheckCollision(startPosition, vector, instance.headCollider.radius * this.scaleFactor, out vector2, out raycastHit);
		if (flag)
		{
			finalPosition = vector2 - vector.normalized * 0.01f;
			modifiedVelocity = startVelocity - raycastHit.normal * Vector3.Dot(raycastHit.normal, startVelocity);
		}
		Vector3 position = this.leftHand.rigTarget.position;
		Vector3 vector3;
		RaycastHit raycastHit2;
		bool flag2 = this.LocalCheckCollision(position, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector3, out raycastHit2);
		if (flag2)
		{
			finalPosition = vector3 - (this.leftHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		Vector3 position2 = this.rightHand.rigTarget.position;
		Vector3 vector4;
		RaycastHit raycastHit3;
		bool flag3 = this.LocalCheckCollision(position2, vector, instance.minimumRaycastDistance * this.scaleFactor, out vector4, out raycastHit3);
		if (flag3)
		{
			finalPosition = vector4 - (this.rightHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		return flag || flag2 || flag3;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x000AAA38 File Offset: 0x000A8C38
	public void TrySweptMoveTo(Vector3 targetPosition, out bool handCollided, out bool buttCollided)
	{
		Vector3 position = base.transform.position;
		this.TrySweptOffsetMove(targetPosition - position, out handCollided, out buttCollided);
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x000AAA60 File Offset: 0x000A8C60
	public void TrySweptOffsetMove(Vector3 movement, out bool handCollided, out bool buttCollided)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector = position + movement;
		Vector3 vector2 = position;
		handCollided = false;
		buttCollided = false;
		Vector3 vector3;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(vector2, movement, instance.headCollider.radius * this.scaleFactor, out vector3, out raycastHit))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector3 - movement.normalized * 0.01f;
			}
			movement = vector - position;
			buttCollided = true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		Vector3 vector4;
		RaycastHit raycastHit2;
		if (this.LocalCheckCollision(position2, movement, instance.minimumRaycastDistance * this.scaleFactor, out vector4, out raycastHit2))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector4 - (this.leftHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		Vector3 vector5;
		RaycastHit raycastHit3;
		if (this.LocalCheckCollision(position3, movement, instance.minimumRaycastDistance * this.scaleFactor, out vector5, out raycastHit3))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = vector5 - (this.rightHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		base.transform.position = vector;
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x000AABF0 File Offset: 0x000A8DF0
	private bool LocalCheckCollision(Vector3 startPosition, Vector3 movement, float radius, out Vector3 finalPosition, out RaycastHit hit)
	{
		GTPlayer instance = GTPlayer.Instance;
		finalPosition = startPosition + movement;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		Vector3 normalized = movement.normalized;
		int num = Physics.SphereCastNonAlloc(startPosition, radius, normalized, this.rayCastNonAllocColliders, movement.magnitude, instance.locomotionEnabledLayers.value);
		if (num > 0)
		{
			raycastHit = this.rayCastNonAllocColliders[0];
			for (int i = 0; i < num; i++)
			{
				if (raycastHit.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < raycastHit.distance))
				{
					flag = true;
					raycastHit = this.rayCastNonAllocColliders[i];
				}
			}
		}
		hit = raycastHit;
		if (flag)
		{
			finalPosition = startPosition + normalized * (raycastHit.distance - 0.01f);
			return true;
		}
		return false;
	}

	// Token: 0x06001FA6 RID: 8102 RVA: 0x000AACD4 File Offset: 0x000A8ED4
	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf;
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = this.reliableState.HasBracelet == flag2;
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
		}
	}

	// Token: 0x06001FA7 RID: 8103 RVA: 0x000AAEA4 File Offset: 0x000A90A4
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
		if (this.builderResizeWatch != null)
		{
			MeshRenderer component = this.builderResizeWatch.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = !on;
			}
		}
	}

	// Token: 0x06001FA8 RID: 8104 RVA: 0x000AAEEA File Offset: 0x000A90EA
	public void EnablePaintbrawlCosmetics(bool on)
	{
		this.paintbrawlBalloons.gameObject.SetActive(on);
	}

	// Token: 0x06001FA9 RID: 8105 RVA: 0x000AAF00 File Offset: 0x000A9100
	public void EnableBuilderResizeWatch(bool on)
	{
		if (this.builderResizeWatch != null && this.builderResizeWatch.activeSelf != on)
		{
			this.builderResizeWatch.SetActive(on);
			if (this.builderArmShelfLeft != null)
			{
				this.builderArmShelfLeft.gameObject.SetActive(on);
			}
			if (this.builderArmShelfRight != null)
			{
				this.builderArmShelfRight.gameObject.SetActive(on);
			}
		}
		if (this.isOfflineVRRig)
		{
			bool flag = this.reliableState.isBuilderWatchEnabled != on;
			this.reliableState.isBuilderWatchEnabled = on;
			if (flag)
			{
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001FAA RID: 8106 RVA: 0x000AAFA5 File Offset: 0x000A91A5
	public void EnableGuardianEjectWatch(bool on)
	{
		if (this.guardianEjectWatch != null && this.guardianEjectWatch.activeSelf != on)
		{
			this.guardianEjectWatch.SetActive(on);
		}
	}

	// Token: 0x06001FAB RID: 8107 RVA: 0x000AAFCF File Offset: 0x000A91CF
	public void EnableVStumpReturnWatch(bool on)
	{
		if (this.vStumpReturnWatch != null && this.vStumpReturnWatch.activeSelf != on)
		{
			this.vStumpReturnWatch.SetActive(on);
		}
	}

	// Token: 0x06001FAC RID: 8108 RVA: 0x000AAFF9 File Offset: 0x000A91F9
	public void EnableRankedTimerWatch(bool on)
	{
		if (this.rankedTimerWatch != null && this.rankedTimerWatch.activeSelf != on)
		{
			this.rankedTimerWatch.SetActive(on);
		}
	}

	// Token: 0x06001FAD RID: 8109 RVA: 0x000AB023 File Offset: 0x000A9223
	public void EnableSuperInfectionHands(bool on)
	{
		if (this.superInfectionHand != null)
		{
			this.superInfectionHand.EnableHands(on);
		}
	}

	// Token: 0x06001FAE RID: 8110 RVA: 0x000AB040 File Offset: 0x000A9240
	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	// Token: 0x06001FAF RID: 8111 RVA: 0x000AB090 File Offset: 0x000A9290
	public bool ShouldPlayReplacementVoice()
	{
		return this.netView && !this.netView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.SpeakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06001FB0 RID: 8112 RVA: 0x000AB10B File Offset: 0x000A930B
	public bool IsInDisplacementZone
	{
		get
		{
			return this.displacementZone != null;
		}
	}

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06001FB1 RID: 8113 RVA: 0x000AB119 File Offset: 0x000A9319
	public bool IsVisuallyDisplaced
	{
		get
		{
			return this.displacementZone != null && this.displacementZone.IsDisplacingRig(this);
		}
	}

	// Token: 0x06001FB2 RID: 8114 RVA: 0x000AB137 File Offset: 0x000A9337
	public void SetDisplacementZone(RigDisplacementZone displacementZone)
	{
		this.displacementZone = displacementZone;
	}

	// Token: 0x06001FB3 RID: 8115 RVA: 0x000AB140 File Offset: 0x000A9340
	public void ClearDisplacementZone(RigDisplacementZone displacementZone)
	{
		if (this.displacementZone == displacementZone)
		{
			this.displacementZone = null;
		}
	}

	// Token: 0x06001FB4 RID: 8116 RVA: 0x000AB157 File Offset: 0x000A9357
	public void ResetTimeSpawned()
	{
		this.timeSpawned = Time.time;
	}

	// Token: 0x06001FB5 RID: 8117 RVA: 0x000AB164 File Offset: 0x000A9364
	public void SetGooParticleSystemStatus(bool isLeftHand, bool isEnabled)
	{
		if (isLeftHand)
		{
			if (this.leftHandGooParticleSystem.gameObject.activeSelf != isEnabled)
			{
				this.leftHandGooParticleSystem.gameObject.SetActive(isEnabled);
				return;
			}
		}
		else if (this.rightHandGooParticleSystem.gameObject.activeSelf != isEnabled)
		{
			this.rightHandGooParticleSystem.gameObject.SetActive(isEnabled);
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06001FB6 RID: 8118 RVA: 0x000AB1BD File Offset: 0x000A93BD
	// (set) Token: 0x06001FB7 RID: 8119 RVA: 0x000AB1C5 File Offset: 0x000A93C5
	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06001FB8 RID: 8120 RVA: 0x000AB1CE File Offset: 0x000A93CE
	// (set) Token: 0x06001FB9 RID: 8121 RVA: 0x000AB1D6 File Offset: 0x000A93D6
	public bool IsFrozen { get; set; }

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06001FBA RID: 8122 RVA: 0x000AB1DF File Offset: 0x000A93DF
	// (set) Token: 0x06001FBB RID: 8123 RVA: 0x000AB1E7 File Offset: 0x000A93E7
	public bool ShowGoldNameTag
	{
		get
		{
			return this.showGoldNameTag;
		}
		private set
		{
			this.showGoldNameTag = value;
		}
	}

	// Token: 0x06001FBC RID: 8124 RVA: 0x000AB1F0 File Offset: 0x000A93F0
	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmeticsString)
	{
		if (cosmeticsString == "BANNED")
		{
			this._playerOwnedCosmetics.Clear();
			this._playerOwnedCosmeticsAge.Clear();
			return true;
		}
		Dictionary<string, ItemInstance> dictionary;
		try
		{
			dictionary = JsonConvert.DeserializeObject<Dictionary<string, ItemInstance>>(cosmeticsString);
		}
		catch (Exception ex)
		{
			string text = "Failed to deserialize cosmetics for ";
			NetPlayer netPlayer = this.creator;
			Debug.LogError(text + ((netPlayer != null) ? netPlayer.NickName : null) + ": " + ex.Message);
			dictionary = null;
		}
		if (this.currentCosmeticTries < this.cosmeticRetries && (dictionary == null || this._playerOwnedCosmetics.SetEquals(dictionary.Keys)))
		{
			this.currentCosmeticTries++;
			return false;
		}
		if (dictionary == null)
		{
			dictionary = new Dictionary<string, ItemInstance>();
		}
		this.currentCosmeticTries = 0;
		this.SaveOwnedCosmetics(dictionary);
		this.InitializedCosmetics = true;
		this.SetCosmeticsActive(false);
		this.myBodyDockPositions.RefreshTransferrableItems();
		NetworkView networkView = this.netView;
		if (networkView != null)
		{
			networkView.SendRPC("RPC_RequestCosmetics", this.creator, Array.Empty<object>());
		}
		return true;
	}

	// Token: 0x06001FBD RID: 8125 RVA: 0x000AB2F4 File Offset: 0x000A94F4
	private void SaveOwnedCosmetics(Dictionary<string, ItemInstance> cosmetics)
	{
		if (cosmetics.Count == 0)
		{
			return;
		}
		this._playerOwnedCosmetics.Clear();
		this._playerOwnedCosmeticsAge.Clear();
		foreach (KeyValuePair<string, ItemInstance> keyValuePair in cosmetics)
		{
			string text;
			ItemInstance itemInstance;
			keyValuePair.Deconstruct(out text, out itemInstance);
			string text2 = text;
			ItemInstance itemInstance2 = itemInstance;
			this._playerOwnedCosmetics.Add(text2);
			if (itemInstance2 != null && itemInstance2.PurchaseDate != null)
			{
				this._playerOwnedCosmeticsAge[text2] = (int)(DateTime.UtcNow - itemInstance2.PurchaseDate.Value).TotalDays;
			}
		}
		this.CheckForEarlyAccess();
	}

	// Token: 0x06001FBE RID: 8126 RVA: 0x000AB3BC File Offset: 0x000A95BC
	internal void AddCosmetic(string cosmeticId, int daysOwned = 0)
	{
		bool flag = this._playerOwnedCosmetics.Add(cosmeticId);
		if (daysOwned < 1)
		{
			return;
		}
		int num;
		if (!flag && this._playerOwnedCosmeticsAge.TryGetValue(cosmeticId, out num))
		{
			int num2 = Mathf.Max(num, daysOwned);
			this._playerOwnedCosmeticsAge[cosmeticId] = num2;
			return;
		}
		this._playerOwnedCosmeticsAge[cosmeticId] = daysOwned;
	}

	// Token: 0x06001FBF RID: 8127 RVA: 0x000AB413 File Offset: 0x000A9613
	internal bool HasCosmetic(string cosmeticId)
	{
		return this._playerOwnedCosmetics.Contains(cosmeticId);
	}

	// Token: 0x06001FC0 RID: 8128 RVA: 0x000AB424 File Offset: 0x000A9624
	public void DeactivateAllRenderers()
	{
		Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].enabled && !componentsInChildren[i].forceRenderingOff)
			{
				componentsInChildren[i].forceRenderingOff = true;
				this.deactivatedRenderers.Add(componentsInChildren[i]);
			}
		}
	}

	// Token: 0x06001FC1 RID: 8129 RVA: 0x000AB474 File Offset: 0x000A9674
	public void ReactivateAllRenderers()
	{
		for (int i = 0; i < this.deactivatedRenderers.Count; i++)
		{
			Renderer renderer = this.deactivatedRenderers[i];
			if (renderer.IsNotNull())
			{
				renderer.forceRenderingOff = false;
			}
		}
		this.deactivatedRenderers.Clear();
	}

	// Token: 0x06001FC2 RID: 8130 RVA: 0x000AB4C0 File Offset: 0x000A96C0
	private short PackCompetitiveData()
	{
		if (!this.turningCompInitialized)
		{
			this.GorillaSnapTurningComp = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			this.turningCompInitialized = true;
		}
		this.fps = Mathf.Min(Mathf.RoundToInt(1f / Time.smoothDeltaTime), 255);
		int num = 0;
		if (this.GorillaSnapTurningComp != null)
		{
			this.turnFactor = this.GorillaSnapTurningComp.turnFactor;
			this.turnType = this.GorillaSnapTurningComp.turnType;
			string text = this.turnType;
			if (!(text == "SNAP"))
			{
				if (text == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += this.turnFactor;
		}
		return (short)(this.fps + (num << 8));
	}

	// Token: 0x06001FC3 RID: 8131 RVA: 0x000AB580 File Offset: 0x000A9780
	private void UnpackCompetitiveData(short packed)
	{
		int num = 255;
		this.fps = (int)packed & num;
		int num2 = 31;
		int num3 = (packed >> 8) & num2;
		this.turnFactor = num3 % 10;
		int num4 = num3 / 10;
		if (num4 == 1)
		{
			this.turnType = "SNAP";
			return;
		}
		if (num4 != 2)
		{
			this.turnType = "NONE";
			return;
		}
		this.turnType = "SMOOTH";
	}

	// Token: 0x06001FC4 RID: 8132 RVA: 0x000AB5E4 File Offset: 0x000A97E4
	private void OnKIDSessionUpdated(bool showCustomNames, Permission.ManagedByEnum managedBy)
	{
		bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
		GorillaComputer.instance.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[] { GorillaComputer.ComputerState.Name }, false);
		bool flag2 = PlayerPrefs.GetInt("nameTagsOn", -1) > 0;
		switch (managedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			flag = GorillaComputer.instance.NametagsEnabled;
			break;
		case Permission.ManagedByEnum.GUARDIAN:
			flag = showCustomNames && flag2;
			break;
		case Permission.ManagedByEnum.PROHIBITED:
			flag = false;
			break;
		}
		this.UpdateName(flag);
		Debug.Log("[KID] On Session Update - Custom Names Permission changed - Has enabled customNames? [" + flag.ToString() + "]");
	}

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x06001FC5 RID: 8133 RVA: 0x000AB680 File Offset: 0x000A9880
	public static VRRig LocalRig
	{
		get
		{
			return VRRig.gLocalRig;
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x06001FC6 RID: 8134 RVA: 0x000AB687 File Offset: 0x000A9887
	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x06001FC7 RID: 8135 RVA: 0x0001104F File Offset: 0x0000F24F
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x06001FC8 RID: 8136 RVA: 0x000AB694 File Offset: 0x000A9894
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x17000364 RID: 868
	// (get) Token: 0x06001FC9 RID: 8137 RVA: 0x000AB6A4 File Offset: 0x000A98A4
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return default(Bounds);
		}
	}

	// Token: 0x17000365 RID: 869
	// (get) Token: 0x06001FCA RID: 8138 RVA: 0x000AB6BA File Offset: 0x000A98BA
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.buildEntries();
		}
	}

	// Token: 0x06001FCB RID: 8139 RVA: 0x000AB6C4 File Offset: 0x000A98C4
	private IList<KeyValueStringPair> buildEntries()
	{
		return new KeyValueStringPair[]
		{
			new KeyValueStringPair("Name", this.playerNameVisible),
			new KeyValueStringPair("Color", string.Format("{0}, {1}, {2}", Mathf.RoundToInt(this.playerColor.r * 9f), Mathf.RoundToInt(this.playerColor.g * 9f), Mathf.RoundToInt(this.playerColor.b * 9f)))
		};
	}

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06001FCC RID: 8140 RVA: 0x000AB75C File Offset: 0x000A995C
	// (remove) Token: 0x06001FCD RID: 8141 RVA: 0x000AB794 File Offset: 0x000A9994
	public event Action OnDataChange;

	// Token: 0x0400290B RID: 10507
	private bool _isListeningFor_OnPostInstantiateAllPrefabs;

	// Token: 0x0400290C RID: 10508
	[OnEnterPlay_SetNull]
	public static Action newPlayerJoined;

	// Token: 0x0400290D RID: 10509
	public VRMap head;

	// Token: 0x0400290E RID: 10510
	public VRMap rightHand;

	// Token: 0x0400290F RID: 10511
	public VRMap leftHand;

	// Token: 0x04002910 RID: 10512
	public VRMapThumb leftThumb;

	// Token: 0x04002911 RID: 10513
	public VRMapIndex leftIndex;

	// Token: 0x04002912 RID: 10514
	public VRMapMiddle leftMiddle;

	// Token: 0x04002913 RID: 10515
	public VRMapThumb rightThumb;

	// Token: 0x04002914 RID: 10516
	public VRMapIndex rightIndex;

	// Token: 0x04002915 RID: 10517
	public VRMapMiddle rightMiddle;

	// Token: 0x04002916 RID: 10518
	public CrittersLoudNoise leftHandNoise;

	// Token: 0x04002917 RID: 10519
	public CrittersLoudNoise rightHandNoise;

	// Token: 0x04002918 RID: 10520
	public CrittersLoudNoise speakingNoise;

	// Token: 0x04002919 RID: 10521
	private int previousGrabbedRope = -1;

	// Token: 0x0400291A RID: 10522
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x0400291B RID: 10523
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x0400291C RID: 10524
	private bool previousGrabbedRopeWasBody;

	// Token: 0x0400291D RID: 10525
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x0400291E RID: 10526
	private Transform currentHoldParent;

	// Token: 0x0400291F RID: 10527
	private Transform currentRopeSwingTarget;

	// Token: 0x04002920 RID: 10528
	private float lastRopeGrabTimer;

	// Token: 0x04002921 RID: 10529
	private bool shouldLerpToRope;

	// Token: 0x04002922 RID: 10530
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x04002923 RID: 10531
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x04002924 RID: 10532
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x04002925 RID: 10533
	[NonSerialized]
	public bool grabbedRopeIsBody;

	// Token: 0x04002926 RID: 10534
	[NonSerialized]
	public bool grabbedRopeIsPhotonView;

	// Token: 0x04002927 RID: 10535
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x04002928 RID: 10536
	private int prevMovingSurfaceID = -1;

	// Token: 0x04002929 RID: 10537
	private bool movingSurfaceWasLeft;

	// Token: 0x0400292A RID: 10538
	private bool movingSurfaceWasBody;

	// Token: 0x0400292B RID: 10539
	private bool movingSurfaceWasMonkeBlock;

	// Token: 0x0400292C RID: 10540
	[NonSerialized]
	public int mountedMovingSurfaceId = -1;

	// Token: 0x0400292D RID: 10541
	[NonSerialized]
	private BuilderPiece mountedMonkeBlock;

	// Token: 0x0400292E RID: 10542
	[NonSerialized]
	private MovingSurface mountedMovingSurface;

	// Token: 0x0400292F RID: 10543
	[NonSerialized]
	public bool mountedMovingSurfaceIsLeft;

	// Token: 0x04002930 RID: 10544
	[NonSerialized]
	public bool mountedMovingSurfaceIsBody;

	// Token: 0x04002931 RID: 10545
	[NonSerialized]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x04002932 RID: 10546
	[NonSerialized]
	public Vector3 mountedMonkeBlockOffset = Vector3.zero;

	// Token: 0x04002933 RID: 10547
	[NonSerialized]
	public bool InOverrideSubscriptionZone;

	// Token: 0x04002934 RID: 10548
	[NonSerialized]
	public Vector3 OverrideSubscriptionZoneLocation = Vector3.zero;

	// Token: 0x04002935 RID: 10549
	private float lastMountedSurfaceTimer;

	// Token: 0x04002936 RID: 10550
	private bool shouldLerpToMovingSurface;

	// Token: 0x04002937 RID: 10551
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x04002938 RID: 10552
	public GameObject mainCamera;

	// Token: 0x04002939 RID: 10553
	public Transform playerOffsetTransform;

	// Token: 0x0400293A RID: 10554
	public int SDKIndex;

	// Token: 0x0400293B RID: 10555
	public bool isMyPlayer;

	// Token: 0x0400293C RID: 10556
	public AudioSource leftHandPlayer;

	// Token: 0x0400293D RID: 10557
	public AudioSource rightHandPlayer;

	// Token: 0x0400293E RID: 10558
	public AudioSource tagSound;

	// Token: 0x0400293F RID: 10559
	[SerializeField]
	private float ratio;

	// Token: 0x04002940 RID: 10560
	public Transform headConstraint;

	// Token: 0x04002941 RID: 10561
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x04002942 RID: 10562
	public GameObject headMesh;

	// Token: 0x04002943 RID: 10563
	private NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002944 RID: 10564
	public Vector3 jobPos;

	// Token: 0x04002945 RID: 10565
	public Quaternion syncRotation;

	// Token: 0x04002946 RID: 10566
	public Quaternion jobRotation;

	// Token: 0x04002947 RID: 10567
	public AudioClip[] clipToPlay;

	// Token: 0x04002948 RID: 10568
	public AudioClip[] handTapSound;

	// Token: 0x04002949 RID: 10569
	public int setMatIndex;

	// Token: 0x0400294A RID: 10570
	public float lerpValueFingers;

	// Token: 0x0400294B RID: 10571
	public float lerpValueBody;

	// Token: 0x0400294C RID: 10572
	public GameObject backpack;

	// Token: 0x0400294D RID: 10573
	public Transform leftHandTransform;

	// Token: 0x0400294E RID: 10574
	public Transform rightHandTransform;

	// Token: 0x0400294F RID: 10575
	public Transform bodyTransform;

	// Token: 0x04002950 RID: 10576
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x04002951 RID: 10577
	public GorillaSkin defaultSkin;

	// Token: 0x04002952 RID: 10578
	public MeshRenderer faceSkin;

	// Token: 0x04002953 RID: 10579
	public XRaySkeleton skeleton;

	// Token: 0x04002954 RID: 10580
	public GorillaBodyRenderer bodyRenderer;

	// Token: 0x04002955 RID: 10581
	public ZoneEntityBSP zoneEntity;

	// Token: 0x04002956 RID: 10582
	public Material scoreboardMaterial;

	// Token: 0x04002957 RID: 10583
	public GameObject spectatorSkin;

	// Token: 0x04002958 RID: 10584
	public int handSync;

	// Token: 0x04002959 RID: 10585
	public Material[] materialsToChangeTo;

	// Token: 0x0400295A RID: 10586
	public float red;

	// Token: 0x0400295B RID: 10587
	public float green;

	// Token: 0x0400295C RID: 10588
	public float blue;

	// Token: 0x0400295D RID: 10589
	public TextMeshPro playerText1;

	// Token: 0x0400295E RID: 10590
	public string playerNameVisible;

	// Token: 0x0400295F RID: 10591
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x04002960 RID: 10592
	public CosmeticItemRegistry cosmeticsObjectRegistry;

	// Token: 0x04002961 RID: 10593
	[NonSerialized]
	public PropHuntHandFollower propHuntHandFollower;

	// Token: 0x04002962 RID: 10594
	private int taggedById;

	// Token: 0x04002963 RID: 10595
	private readonly HashSet<string> _playerOwnedCosmetics = new HashSet<string>(50);

	// Token: 0x04002964 RID: 10596
	private readonly Dictionary<string, int> _playerOwnedCosmeticsAge = new Dictionary<string, int>(50);

	// Token: 0x04002965 RID: 10597
	private bool initializedCosmetics;

	// Token: 0x04002966 RID: 10598
	private readonly HashSet<string> _temporaryCosmetics = new HashSet<string>();

	// Token: 0x04002967 RID: 10599
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x04002968 RID: 10600
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x04002969 RID: 10601
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x0400296A RID: 10602
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x0400296B RID: 10603
	[NonSerialized]
	public readonly List<GameObject> activeCosmetics = new List<GameObject>(16);

	// Token: 0x0400296C RID: 10604
	private int cosmeticRetries = 2;

	// Token: 0x0400296D RID: 10605
	private int currentCosmeticTries;

	// Token: 0x0400296F RID: 10607
	public SizeManager sizeManager;

	// Token: 0x04002970 RID: 10608
	public float pitchScale = 0.3f;

	// Token: 0x04002971 RID: 10609
	public float pitchOffset = 1f;

	// Token: 0x04002972 RID: 10610
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x04002973 RID: 10611
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x04002974 RID: 10612
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x04002975 RID: 10613
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x04002976 RID: 10614
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x04002977 RID: 10615
	private float cosmeticPitchShift;

	// Token: 0x04002978 RID: 10616
	private float cosmeticVolumeShift;

	// Token: 0x04002979 RID: 10617
	private bool cosmeticPitchActive;

	// Token: 0x0400297A RID: 10618
	private bool cosmeticVolumeActive;

	// Token: 0x0400297B RID: 10619
	private bool anyShiftedVoiceCosmetic;

	// Token: 0x0400297C RID: 10620
	private bool voiceShiftCosmeticsDirty;

	// Token: 0x0400297D RID: 10621
	[NonSerialized]
	public List<VoiceShiftCosmetic> VoiceShiftCosmetics = new List<VoiceShiftCosmetic>();

	// Token: 0x0400297E RID: 10622
	public FriendshipBracelet friendshipBraceletLeftHand;

	// Token: 0x0400297F RID: 10623
	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	// Token: 0x04002980 RID: 10624
	public FriendshipBracelet friendshipBraceletRightHand;

	// Token: 0x04002981 RID: 10625
	public NonCosmeticHandItem nonCosmeticRightHandItem;

	// Token: 0x04002982 RID: 10626
	public HoverboardVisual hoverboardVisual;

	// Token: 0x04002983 RID: 10627
	private int hoverboardEnabledCount;

	// Token: 0x04002984 RID: 10628
	public HoldableHand bodyHolds;

	// Token: 0x04002985 RID: 10629
	public HoldableHand leftHolds;

	// Token: 0x04002986 RID: 10630
	public HoldableHand rightHolds;

	// Token: 0x04002987 RID: 10631
	public GorillaClimbable leftHandHoldsPlayer;

	// Token: 0x04002988 RID: 10632
	public GorillaClimbable rightHandHoldsPlayer;

	// Token: 0x04002989 RID: 10633
	public TakeMyHand_HandLink leftHandLink;

	// Token: 0x0400298A RID: 10634
	public TakeMyHand_HandLink rightHandLink;

	// Token: 0x0400298D RID: 10637
	public GameObject nameTagAnchor;

	// Token: 0x0400298E RID: 10638
	public GameObject frozenEffect;

	// Token: 0x0400298F RID: 10639
	public GameObject iceCubeLeft;

	// Token: 0x04002990 RID: 10640
	public GameObject iceCubeRight;

	// Token: 0x04002991 RID: 10641
	public float frozenEffectMaxY;

	// Token: 0x04002992 RID: 10642
	public float frozenEffectMaxHorizontalScale = 0.8f;

	// Token: 0x04002993 RID: 10643
	public GameObject FPVEffectsParent;

	// Token: 0x04002994 RID: 10644
	public Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> TemporaryCosmeticEffects = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

	// Token: 0x04002995 RID: 10645
	private float _nextUpdateTime = -1f;

	// Token: 0x04002996 RID: 10646
	public VRRigReliableState reliableState;

	// Token: 0x04002997 RID: 10647
	[SerializeField]
	private Transform MouthPosition;

	// Token: 0x0400299B RID: 10651
	[SerializeField]
	internal RigContainer rigContainer;

	// Token: 0x0400299C RID: 10652
	public Action<RigContainer> OnNameChanged;

	// Token: 0x0400299D RID: 10653
	private Vector3 remoteVelocity;

	// Token: 0x0400299E RID: 10654
	private double remoteLatestTimestamp;

	// Token: 0x0400299F RID: 10655
	private Vector3 remoteCorrectionNeeded;

	// Token: 0x040029A0 RID: 10656
	private const float REMOTE_CORRECTION_RATE = 5f;

	// Token: 0x040029A1 RID: 10657
	private const bool USE_NEW_NETCODE = false;

	// Token: 0x040029A2 RID: 10658
	private float stealthTimer;

	// Token: 0x040029A3 RID: 10659
	private GorillaAmbushManager stealthManager;

	// Token: 0x040029A4 RID: 10660
	private LayerChanger layerChanger;

	// Token: 0x040029A5 RID: 10661
	private float frozenEffectMinY;

	// Token: 0x040029A6 RID: 10662
	private float frozenEffectMinHorizontalScale;

	// Token: 0x040029A7 RID: 10663
	private float frozenTimeElapsed;

	// Token: 0x040029A8 RID: 10664
	public TagEffectPack CosmeticEffectPack;

	// Token: 0x040029A9 RID: 10665
	private GorillaSnapTurn GorillaSnapTurningComp;

	// Token: 0x040029AA RID: 10666
	private bool turningCompInitialized;

	// Token: 0x040029AB RID: 10667
	private string turnType = "NONE";

	// Token: 0x040029AC RID: 10668
	private int turnFactor;

	// Token: 0x040029AD RID: 10669
	private int fps;

	// Token: 0x040029AF RID: 10671
	private VRRig.PartyMemberStatus partyMemberStatus;

	// Token: 0x040029B0 RID: 10672
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2),
		new GTBitOps.BitWriteInfo(5, 2),
		new GTBitOps.BitWriteInfo(7, 2),
		new GTBitOps.BitWriteInfo(9, 2),
		new GTBitOps.BitWriteInfo(11, 1),
		new GTBitOps.BitWriteInfo(12, 1),
		new GTBitOps.BitWriteInfo(13, 1)
	};

	// Token: 0x040029B1 RID: 10673
	public bool inTryOnRoom;

	// Token: 0x040029B2 RID: 10674
	public bool inTempCosmSpace;

	// Token: 0x040029B3 RID: 10675
	[NonSerialized]
	public Dictionary<string, CosmeticsController.CollectionState> remoteCycleStates = new Dictionary<string, CosmeticsController.CollectionState>();

	// Token: 0x040029B4 RID: 10676
	private readonly List<CosmeticCollectionDisplay> scratchDisplayList = new List<CosmeticCollectionDisplay>();

	// Token: 0x040029B5 RID: 10677
	private int[] cycleStatesArray = Array.Empty<int>();

	// Token: 0x040029B6 RID: 10678
	public bool muted;

	// Token: 0x040029B7 RID: 10679
	private float lastScaleFactor = 1f;

	// Token: 0x040029B8 RID: 10680
	private float scaleMultiplier = 1f;

	// Token: 0x040029B9 RID: 10681
	private float nativeScale = 1f;

	// Token: 0x040029BA RID: 10682
	private float timeSpawned;

	// Token: 0x040029BB RID: 10683
	public float doNotLerpConstant = 1f;

	// Token: 0x040029BC RID: 10684
	public string tempString;

	// Token: 0x040029BD RID: 10685
	internal NetPlayer creator;

	// Token: 0x040029BE RID: 10686
	private float[] speedArray;

	// Token: 0x040029BF RID: 10687
	private double handLerpValues;

	// Token: 0x040029C0 RID: 10688
	private bool initialized;

	// Token: 0x040029C1 RID: 10689
	[FormerlySerializedAs("battleBalloons")]
	public PaintbrawlBalloons paintbrawlBalloons;

	// Token: 0x040029C2 RID: 10690
	private int tempInt;

	// Token: 0x040029C3 RID: 10691
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x040029C4 RID: 10692
	public ParticleSystem lavaParticleSystem;

	// Token: 0x040029C5 RID: 10693
	public ParticleSystem rockParticleSystem;

	// Token: 0x040029C6 RID: 10694
	public ParticleSystem iceParticleSystem;

	// Token: 0x040029C7 RID: 10695
	public ParticleSystem snowFlakeParticleSystem;

	// Token: 0x040029C8 RID: 10696
	public ParticleSystem leftHandGooParticleSystem;

	// Token: 0x040029C9 RID: 10697
	public ParticleSystem rightHandGooParticleSystem;

	// Token: 0x040029CA RID: 10698
	public string tempItemName;

	// Token: 0x040029CB RID: 10699
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x040029CC RID: 10700
	public string tempItemId;

	// Token: 0x040029CD RID: 10701
	public int tempItemCost;

	// Token: 0x040029CE RID: 10702
	public int leftHandHoldableStatus;

	// Token: 0x040029CF RID: 10703
	public int rightHandHoldableStatus;

	// Token: 0x040029D0 RID: 10704
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x040029D1 RID: 10705
	private List<TransferrableObject> instrumentSelfOnly = new List<TransferrableObject>();

	// Token: 0x040029D2 RID: 10706
	public AudioSource geodeCrackingSound;

	// Token: 0x040029D3 RID: 10707
	public float bonkTime;

	// Token: 0x040029D4 RID: 10708
	public float bonkCooldown = 2f;

	// Token: 0x040029D5 RID: 10709
	private VRRig tempVRRig;

	// Token: 0x040029D6 RID: 10710
	public GameObject huntComputer;

	// Token: 0x040029D7 RID: 10711
	public GameObject builderResizeWatch;

	// Token: 0x040029D8 RID: 10712
	public BuilderArmShelf builderArmShelfLeft;

	// Token: 0x040029D9 RID: 10713
	public BuilderArmShelf builderArmShelfRight;

	// Token: 0x040029DA RID: 10714
	public GameObject guardianEjectWatch;

	// Token: 0x040029DB RID: 10715
	public GameObject vStumpReturnWatch;

	// Token: 0x040029DC RID: 10716
	public GameObject rankedTimerWatch;

	// Token: 0x040029DD RID: 10717
	public SuperInfectionHandDisplay superInfectionHand;

	// Token: 0x040029DE RID: 10718
	public ProjectileWeapon projectileWeapon;

	// Token: 0x040029DF RID: 10719
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x040029E0 RID: 10720
	private VRRig senderRig;

	// Token: 0x040029E1 RID: 10721
	private bool isInitialized;

	// Token: 0x040029E2 RID: 10722
	private CircularBuffer<VRRig.VelocityTime> velocityHistoryList = new CircularBuffer<VRRig.VelocityTime>(200);

	// Token: 0x040029E3 RID: 10723
	public int velocityHistoryMaxLength = 200;

	// Token: 0x040029E4 RID: 10724
	private Vector3 lastPosition;

	// Token: 0x040029E5 RID: 10725
	public const int splashLimitCount = 4;

	// Token: 0x040029E6 RID: 10726
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x040029E7 RID: 10727
	private float[] splashEffectTimes = new float[4];

	// Token: 0x040029E8 RID: 10728
	internal AudioSource voiceAudio;

	// Token: 0x040029E9 RID: 10729
	public bool remoteUseReplacementVoice;

	// Token: 0x040029EA RID: 10730
	public bool localUseReplacementVoice;

	// Token: 0x040029EB RID: 10731
	private MicWrapper currentMicWrapper;

	// Token: 0x040029EC RID: 10732
	private IAudioDesc audioDesc;

	// Token: 0x040029ED RID: 10733
	private float speakingLoudness;

	// Token: 0x040029EE RID: 10734
	public bool shouldSendSpeakingLoudness = true;

	// Token: 0x040029EF RID: 10735
	public float replacementVoiceLoudnessThreshold = 0.05f;

	// Token: 0x040029F0 RID: 10736
	public int replacementVoiceDetectionDelay = 128;

	// Token: 0x040029F1 RID: 10737
	private GorillaMouthFlap myMouthFlap;

	// Token: 0x040029F2 RID: 10738
	private GorillaSpeakerLoudness mySpeakerLoudness;

	// Token: 0x040029F3 RID: 10739
	public ReplacementVoice myReplacementVoice;

	// Token: 0x040029F4 RID: 10740
	private GorillaEyeExpressions myEyeExpressions;

	// Token: 0x040029F5 RID: 10741
	[SerializeField]
	internal NetworkView netView;

	// Token: 0x040029F6 RID: 10742
	[SerializeField]
	internal VRRigSerializer rigSerializer;

	// Token: 0x040029F7 RID: 10743
	[Obsolete("Deprecated, this is unreliable, use Creator", false)]
	public NetPlayer OwningNetPlayer;

	// Token: 0x040029F8 RID: 10744
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x040029F9 RID: 10745
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x040029FA RID: 10746
	[SerializeField]
	private float tapPointDistance = 0.035f;

	// Token: 0x040029FB RID: 10747
	[SerializeField]
	private float handSpeedToVolumeModifier = 0.05f;

	// Token: 0x040029FC RID: 10748
	[SerializeField]
	private HandEffectContext _leftHandEffect;

	// Token: 0x040029FD RID: 10749
	[SerializeField]
	private HandEffectContext _rightHandEffect;

	// Token: 0x040029FE RID: 10750
	[SerializeField]
	private HandEffectContext _extraLeftHandEffect;

	// Token: 0x040029FF RID: 10751
	[SerializeField]
	private HandEffectContext _extraRightHandEffect;

	// Token: 0x04002A00 RID: 10752
	[SerializeField]
	private Transform renderTransform;

	// Token: 0x04002A01 RID: 10753
	private GamePlayer _gamePlayerRef;

	// Token: 0x04002A02 RID: 10754
	private bool playerWasHaunted;

	// Token: 0x04002A03 RID: 10755
	private float nonHauntedVolume;

	// Token: 0x04002A04 RID: 10756
	[SerializeField]
	private AnimationCurve voicePitchForRelativeScale;

	// Token: 0x04002A05 RID: 10757
	private Vector3 LocalTrajectoryOverridePosition;

	// Token: 0x04002A06 RID: 10758
	private Vector3 LocalTrajectoryOverrideVelocity;

	// Token: 0x04002A07 RID: 10759
	private float LocalTrajectoryOverrideBlend;

	// Token: 0x04002A08 RID: 10760
	[SerializeField]
	private float LocalTrajectoryOverrideDuration = 1f;

	// Token: 0x04002A09 RID: 10761
	private bool localOverrideIsBody;

	// Token: 0x04002A0A RID: 10762
	private bool localOverrideIsLeftHand;

	// Token: 0x04002A0B RID: 10763
	private Transform localOverrideGrabbingHand;

	// Token: 0x04002A0C RID: 10764
	private float localGrabOverrideBlend;

	// Token: 0x04002A0D RID: 10765
	[SerializeField]
	private float LocalGrabOverrideDuration = 0.25f;

	// Token: 0x04002A0E RID: 10766
	private float[] voiceSampleBuffer = new float[128];

	// Token: 0x04002A0F RID: 10767
	private const int CHECK_LOUDNESS_FREQ_FRAMES = 10;

	// Token: 0x04002A10 RID: 10768
	private CallbackContainer<ICallBack> lateUpdateCallbacks = new CallbackContainer<ICallBack>(5);

	// Token: 0x04002A11 RID: 10769
	private float nextLocalVelocityStoreTimestamp;

	// Token: 0x04002A12 RID: 10770
	private bool IsInvisibleToLocalPlayer;

	// Token: 0x04002A13 RID: 10771
	private const int remoteUseReplacementVoice_BIT = 512;

	// Token: 0x04002A14 RID: 10772
	private const int grabbedRope_BIT = 1024;

	// Token: 0x04002A15 RID: 10773
	private const int grabbedRopeIsPhotonView_BIT = 2048;

	// Token: 0x04002A16 RID: 10774
	private const int isHoldingHandsWithPlayer_BIT = 4096;

	// Token: 0x04002A17 RID: 10775
	private const int isHoldingHoverboard_BIT = 8192;

	// Token: 0x04002A18 RID: 10776
	private const int isHoverboardLeftHanded_BIT = 16384;

	// Token: 0x04002A19 RID: 10777
	private const int isOnMovingSurface_BIT = 32768;

	// Token: 0x04002A1A RID: 10778
	private const int isPropHunt_BIT = 65536;

	// Token: 0x04002A1B RID: 10779
	private const int propHuntLeftHand_BIT = 131072;

	// Token: 0x04002A1C RID: 10780
	private const int isLeftHandGrabbable_BIT = 262144;

	// Token: 0x04002A1D RID: 10781
	private const int isRightHandGrabbable_BIT = 524288;

	// Token: 0x04002A1E RID: 10782
	private const int isLeftHandTentacleHoldingHand_BIT = 1048576;

	// Token: 0x04002A1F RID: 10783
	private const int isRightHandTentacleHoldingHand_BIT = 2097152;

	// Token: 0x04002A20 RID: 10784
	private const int showSubscriber_BIT = 4194304;

	// Token: 0x04002A21 RID: 10785
	private const int portalShenanigans_BIT = 8388608;

	// Token: 0x04002A22 RID: 10786
	private const int speakingLoudnessVal_BITSHIFT = 24;

	// Token: 0x04002A23 RID: 10787
	private GorillaIK myIk;

	// Token: 0x04002A24 RID: 10788
	private Vector3 tempVec;

	// Token: 0x04002A25 RID: 10789
	private Quaternion tempQuat;

	// Token: 0x04002A26 RID: 10790
	public Action<int, int> OnMaterialIndexChanged;

	// Token: 0x04002A27 RID: 10791
	[SerializeField]
	private ParticleSystem cosmeticsActivationPS;

	// Token: 0x04002A28 RID: 10792
	[SerializeField]
	private SoundBankPlayer cosmeticsActivationSBP;

	// Token: 0x04002A29 RID: 10793
	public Color playerColor;

	// Token: 0x04002A2A RID: 10794
	public bool colorInitialized;

	// Token: 0x04002A2B RID: 10795
	private Action<Color> onColorInitialized;

	// Token: 0x04002A2E RID: 10798
	private bool m_sentRankedScore;

	// Token: 0x04002A30 RID: 10800
	private int currentQuestScore;

	// Token: 0x04002A31 RID: 10801
	private bool _scoreUpdated;

	// Token: 0x04002A32 RID: 10802
	private CallLimiter updateQuestCallLimit = new CallLimiter(1, 0.5f, 0.5f);

	// Token: 0x04002A34 RID: 10804
	private float currentRankedELO;

	// Token: 0x04002A35 RID: 10805
	private int currentRankedSubTierQuest;

	// Token: 0x04002A36 RID: 10806
	private int currentRankedSubTierPC;

	// Token: 0x04002A37 RID: 10807
	private bool _rankedInfoUpdated;

	// Token: 0x04002A38 RID: 10808
	internal CallLimiter updateRankedInfoCallLimit = new CallLimiter(2, 60f, 0.5f);

	// Token: 0x04002A39 RID: 10809
	public const float maxGuardianThrowVelocity = 20f;

	// Token: 0x04002A3A RID: 10810
	public const float maxRegularThrowVelocity = 3f;

	// Token: 0x04002A3B RID: 10811
	private RaycastHit[] rayCastNonAllocColliders = new RaycastHit[5];

	// Token: 0x04002A3C RID: 10812
	private RigDisplacementZone displacementZone;

	// Token: 0x04002A3D RID: 10813
	private bool renderTransformDisplaced;

	// Token: 0x04002A3E RID: 10814
	private Vector3 cachedRenderTransformPos = new Vector3(0f, -1.65f, 0f);

	// Token: 0x04002A3F RID: 10815
	[NonSerialized]
	public bool portalShenanigansBit;

	// Token: 0x04002A40 RID: 10816
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04002A42 RID: 10818
	[NonSerialized]
	private bool showGoldNameTag;

	// Token: 0x04002A43 RID: 10819
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Right = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002A44 RID: 10820
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Left = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002A45 RID: 10821
	private int loudnessCheckFrame;

	// Token: 0x04002A46 RID: 10822
	private float frameScale;

	// Token: 0x04002A47 RID: 10823
	private SubscriptionManager.SubscriptionDetails subDataCache;

	// Token: 0x04002A48 RID: 10824
	private List<Renderer> deactivatedRenderers = new List<Renderer>();

	// Token: 0x04002A49 RID: 10825
	private const bool SHOW_SCREENS = false;

	// Token: 0x04002A4A RID: 10826
	[OnEnterPlay_SetNull]
	private static VRRig gLocalRig;

	// Token: 0x020004F4 RID: 1268
	public enum PartyMemberStatus
	{
		// Token: 0x04002A4D RID: 10829
		NeedsUpdate,
		// Token: 0x04002A4E RID: 10830
		InLocalParty,
		// Token: 0x04002A4F RID: 10831
		NotInLocalParty
	}

	// Token: 0x020004F5 RID: 1269
	public enum WearablePackedStateSlots
	{
		// Token: 0x04002A51 RID: 10833
		Hat,
		// Token: 0x04002A52 RID: 10834
		LeftHand,
		// Token: 0x04002A53 RID: 10835
		RightHand,
		// Token: 0x04002A54 RID: 10836
		Face,
		// Token: 0x04002A55 RID: 10837
		Pants1,
		// Token: 0x04002A56 RID: 10838
		Pants2,
		// Token: 0x04002A57 RID: 10839
		Badge,
		// Token: 0x04002A58 RID: 10840
		Fur,
		// Token: 0x04002A59 RID: 10841
		Shirt
	}

	// Token: 0x020004F6 RID: 1270
	public struct VelocityTime
	{
		// Token: 0x06001FD2 RID: 8146 RVA: 0x000ABB8D File Offset: 0x000A9D8D
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x04002A5A RID: 10842
		public Vector3 vel;

		// Token: 0x04002A5B RID: 10843
		public double time;
	}
}
