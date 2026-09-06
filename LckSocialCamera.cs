using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.Cosmetics;
using Liv.Lck.GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200040A RID: 1034
[NetworkBehaviourWeaved(1)]
public class LckSocialCamera : NetworkComponent, IGorillaSliceableSimple
{
	// Token: 0x17000269 RID: 617
	// (get) Token: 0x06001874 RID: 6260 RVA: 0x0008B2C1 File Offset: 0x000894C1
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe ref LckSocialCamera.CameraData _networkedData
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing LckSocialCamera._networkedData. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ref *(LckSocialCamera.CameraData*)(this.Ptr + 0);
		}
	}

	// Token: 0x1700026A RID: 618
	// (get) Token: 0x06001875 RID: 6261 RVA: 0x0008B2E6 File Offset: 0x000894E6
	public VRRig VrRig
	{
		get
		{
			return this._vrrig;
		}
	}

	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001876 RID: 6262 RVA: 0x0008B2EE File Offset: 0x000894EE
	// (set) Token: 0x06001877 RID: 6263 RVA: 0x0008B2F6 File Offset: 0x000894F6
	public LCKSocialCameraFollower SocialCameraFollower { get; private set; }

	// Token: 0x06001878 RID: 6264 RVA: 0x0008B300 File Offset: 0x00089500
	public override void OnSpawned()
	{
		if (base.IsLocallyOwned)
		{
			this._localOwnedState = LckSocialCamera.CameraState.Empty;
			this.visible = false;
			this.recording = false;
			this.IsOnNeck = false;
			return;
		}
		if (base.Runner != null)
		{
			LckSocialCamera.CameraState currentState = this._networkedData.currentState;
			this.ApplyVisualState(currentState);
			this._networkOwnedState = currentState;
		}
	}

	// Token: 0x06001879 RID: 6265 RVA: 0x0008B35A File Offset: 0x0008955A
	public unsafe override void WriteDataFusion()
	{
		*this._networkedData = new LckSocialCamera.CameraData(this._localOwnedState);
	}

	// Token: 0x0600187A RID: 6266 RVA: 0x0008B372 File Offset: 0x00089572
	public override void ReadDataFusion()
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		this.ReadDataShared(this._networkedData.currentState);
	}

	// Token: 0x0600187B RID: 6267 RVA: 0x0008B38E File Offset: 0x0008958E
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		stream.SendNext(this._localOwnedState);
	}

	// Token: 0x0600187C RID: 6268 RVA: 0x0008B3A4 File Offset: 0x000895A4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != info.photonView.Owner || this.m_isCorrupted)
		{
			return;
		}
		LckSocialCamera.CameraState cameraState = (LckSocialCamera.CameraState)stream.ReceiveNext();
		this.ReadDataShared(cameraState);
	}

	// Token: 0x0600187D RID: 6269 RVA: 0x0008B3E0 File Offset: 0x000895E0
	private void ReadDataShared(LckSocialCamera.CameraState newState)
	{
		if (newState != this._networkOwnedState)
		{
			this.ApplyVisualState(newState);
			this._networkOwnedState = newState;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x0600187E RID: 6270 RVA: 0x0008B3F9 File Offset: 0x000895F9
	// (set) Token: 0x0600187F RID: 6271 RVA: 0x0008B417 File Offset: 0x00089617
	public bool IsOnNeck
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.OnNeck);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.OnNeck, value);
		}
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001880 RID: 6272 RVA: 0x0008B435 File Offset: 0x00089635
	// (set) Token: 0x06001881 RID: 6273 RVA: 0x0008B453 File Offset: 0x00089653
	public bool visible
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.Visible);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.Visible, value);
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001882 RID: 6274 RVA: 0x0008B471 File Offset: 0x00089671
	// (set) Token: 0x06001883 RID: 6275 RVA: 0x0008B48F File Offset: 0x0008968F
	public bool recording
	{
		get
		{
			return LckSocialCamera.GetFlag(base.IsLocallyOwned ? this._localOwnedState : this._networkOwnedState, LckSocialCamera.CameraState.Recording);
		}
		set
		{
			if (!base.IsLocallyOwned)
			{
				return;
			}
			this._localOwnedState = LckSocialCamera.SetFlag(this._localOwnedState, LckSocialCamera.CameraState.Recording, value);
		}
	}

	// Token: 0x06001884 RID: 6276 RVA: 0x0008B4B0 File Offset: 0x000896B0
	private void ApplyVisualState(LckSocialCamera.CameraState newState)
	{
		if (this.m_isCorrupted)
		{
			return;
		}
		bool flag = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Visible);
		bool flag2 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.Recording);
		bool flag3 = LckSocialCamera.GetFlag(newState, LckSocialCamera.CameraState.OnNeck);
		if (!base.IsLocallyOwned)
		{
			IGtCameraVisuals cameraVisuals = this.m_CameraVisuals;
			if (cameraVisuals != null)
			{
				cameraVisuals.SetNetworkedVisualsActive(flag);
			}
			IGtCameraVisuals cameraVisuals2 = this.m_CameraVisuals;
			if (cameraVisuals2 != null)
			{
				cameraVisuals2.SetRecordingState(flag2);
			}
			if (this.m_cameraType == LckSocialCamera.CameraType.Tablet)
			{
				if (flag3)
				{
					this.SocialCameraFollower.SetParentToRig();
					return;
				}
				this.SocialCameraFollower.SetParentNull();
			}
			return;
		}
		IGtCameraVisuals cameraVisuals3 = this.m_CameraVisuals;
		if (cameraVisuals3 != null)
		{
			cameraVisuals3.SetVisualsActive(false);
		}
		IGtCameraVisuals cameraVisuals4 = this.m_CameraVisuals;
		if (cameraVisuals4 == null)
		{
			return;
		}
		cameraVisuals4.SetRecordingState(false);
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0008B551 File Offset: 0x00089751
	private static bool GetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag)
	{
		return currentState.HasFlag(flag);
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x0008B564 File Offset: 0x00089764
	private static LckSocialCamera.CameraState SetFlag(LckSocialCamera.CameraState currentState, LckSocialCamera.CameraState flag, bool shouldBeSet)
	{
		if (shouldBeSet)
		{
			return currentState | flag;
		}
		return currentState & ~flag;
	}

	// Token: 0x06001887 RID: 6279 RVA: 0x0008B574 File Offset: 0x00089774
	protected override void Awake()
	{
		base.Awake();
		if (this.CameraVisuals != null && !this.CameraVisuals.TryGetComponent<IGtCameraVisuals>(out this.m_CameraVisuals))
		{
			Debug.LogError("LCK: LckSocialCamera failed to find IGtCameraVisuals component on CameraVisuals");
		}
		if (this.m_rigNetworkController.IsNull())
		{
			this.m_rigNetworkController = base.GetComponentInParent<VRRigSerializer>();
		}
		if (this.m_rigNetworkController.IsNull())
		{
			return;
		}
		ListProcessor<InAction<RigContainer, PhotonMessageInfoWrapped>> succesfullSpawnEvent = this.m_rigNetworkController.SuccesfullSpawnEvent;
		InAction<RigContainer, PhotonMessageInfoWrapped> inAction = new InAction<RigContainer, PhotonMessageInfoWrapped>(this.OnSuccesfullSpawn);
		succesfullSpawnEvent.Add(in inAction);
	}

	// Token: 0x06001888 RID: 6280 RVA: 0x0008B5F8 File Offset: 0x000897F8
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		if (this.m_lckDelegateRegistered)
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Remove(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
		}
	}

	// Token: 0x06001889 RID: 6281 RVA: 0x0008B628 File Offset: 0x00089828
	private void OnSuccesfullSpawn(in RigContainer rig, in PhotonMessageInfoWrapped info)
	{
		this._vrrig = rig.Rig;
		LCKSocialCameraFollower lcksocialCameraFollower = ((this.m_cameraType == LckSocialCamera.CameraType.Cococam) ? rig.LckCococamFollower : rig.LCKTabletFollower);
		this._scaleTransform = lcksocialCameraFollower.ScaleTransform;
		this.CameraVisuals = lcksocialCameraFollower.CameraVisualsRoot;
		this.m_CameraVisuals = this.CameraVisuals.GetComponent<IGtCameraVisuals>();
		if (!base.IsLocallyOwned && lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>() != null)
		{
			lcksocialCameraFollower.GetComponent<ILckCosmeticDependantPlayerIdSupplier>().UpdatePlayerId();
		}
		this.SocialCameraFollower = lcksocialCameraFollower;
		this.m_isCorrupted = false;
		if (!this._vrrig.isOfflineVRRig)
		{
			lcksocialCameraFollower.SetNetworkController(this);
			return;
		}
		LckSocialCameraManager instance = LckSocialCameraManager.Instance;
		if (!(instance != null))
		{
			LckSocialCameraManager.OnManagerSpawned = (Action<LckSocialCameraManager>)Delegate.Combine(LckSocialCameraManager.OnManagerSpawned, new Action<LckSocialCameraManager>(this.OnManagerSpawned));
			this.m_lckDelegateRegistered = true;
			return;
		}
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			instance.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		instance.SetLckSocialTabletCamera(this);
	}

	// Token: 0x0600188A RID: 6282 RVA: 0x0008B720 File Offset: 0x00089920
	public void SliceUpdate()
	{
		if (this._vrrig.IsNull())
		{
			return;
		}
		if (this.m_cameraType != LckSocialCamera.CameraType.Tablet)
		{
			if (this.m_cameraType == LckSocialCamera.CameraType.Cococam)
			{
				this.SocialCameraFollower.transform.localScale = Vector3.one * this._vrrig.scaleFactor;
			}
			return;
		}
		if (this.IsOnNeck)
		{
			this.SocialCameraFollower.transform.localScale = Vector3.one * 0.3f;
			return;
		}
		this.SocialCameraFollower.transform.localScale = Vector3.one * 0.3f * this._vrrig.scaleFactor;
	}

	// Token: 0x0600188B RID: 6283 RVA: 0x0008B7C9 File Offset: 0x000899C9
	public new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		base.OnEnable();
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600188C RID: 6284 RVA: 0x0008B7E0 File Offset: 0x000899E0
	public new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		base.OnDisable();
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.m_isCorrupted)
		{
			return;
		}
		if (this.SocialCameraFollower.IsNotNull())
		{
			this.SocialCameraFollower.RemoveNetworkController(this);
		}
		this._scaleTransform = null;
		this.CameraVisuals = null;
	}

	// Token: 0x0600188D RID: 6285 RVA: 0x0008B834 File Offset: 0x00089A34
	private void OnManagerSpawned(LckSocialCameraManager manager)
	{
		LckSocialCamera.CameraType cameraType = this.m_cameraType;
		if (cameraType == LckSocialCamera.CameraType.Cococam)
		{
			manager.SetLckSocialCococamCamera(this);
			return;
		}
		if (cameraType != LckSocialCamera.CameraType.Tablet)
		{
			throw new ArgumentOutOfRangeException();
		}
		manager.SetLckSocialTabletCamera(this);
	}

	// Token: 0x0600188E RID: 6286 RVA: 0x0008B866 File Offset: 0x00089A66
	public void TurnOff()
	{
		this.m_isCorrupted = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06001890 RID: 6288 RVA: 0x0008B88A File Offset: 0x00089A8A
	[WeaverGenerated]
	public unsafe override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		*this._networkedData = this.__networkedData;
	}

	// Token: 0x06001891 RID: 6289 RVA: 0x0008B8A7 File Offset: 0x00089AA7
	[WeaverGenerated]
	public unsafe override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this.__networkedData = *this._networkedData;
	}

	// Token: 0x040023AC RID: 9132
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x040023AD RID: 9133
	[SerializeField]
	public GameObject CameraVisuals;

	// Token: 0x040023AE RID: 9134
	[SerializeField]
	private VRRig _vrrig;

	// Token: 0x040023AF RID: 9135
	[SerializeField]
	private VRRigSerializer m_rigNetworkController;

	// Token: 0x040023B0 RID: 9136
	[SerializeField]
	private LckSocialCamera.CameraType m_cameraType;

	// Token: 0x040023B2 RID: 9138
	private bool m_isCorrupted = true;

	// Token: 0x040023B3 RID: 9139
	private bool m_lckDelegateRegistered;

	// Token: 0x040023B4 RID: 9140
	private IGtCameraVisuals m_CameraVisuals;

	// Token: 0x040023B5 RID: 9141
	private LckSocialCamera.CameraState _localOwnedState;

	// Token: 0x040023B6 RID: 9142
	private LckSocialCamera.CameraState _networkOwnedState;

	// Token: 0x040023B7 RID: 9143
	[WeaverGenerated]
	[DefaultForProperty("_networkedData", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private LckSocialCamera.CameraData __networkedData;

	// Token: 0x0200040B RID: 1035
	private enum CameraState
	{
		// Token: 0x040023B9 RID: 9145
		Empty,
		// Token: 0x040023BA RID: 9146
		Visible,
		// Token: 0x040023BB RID: 9147
		Recording,
		// Token: 0x040023BC RID: 9148
		OnNeck = 4
	}

	// Token: 0x0200040C RID: 1036
	private enum CameraType
	{
		// Token: 0x040023BE RID: 9150
		Cococam,
		// Token: 0x040023BF RID: 9151
		Tablet
	}

	// Token: 0x0200040D RID: 1037
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct CameraData : INetworkStruct
	{
		// Token: 0x06001892 RID: 6290 RVA: 0x0008B8C0 File Offset: 0x00089AC0
		public CameraData(LckSocialCamera.CameraState state)
		{
			this.currentState = state;
		}

		// Token: 0x040023C0 RID: 9152
		[FieldOffset(0)]
		public LckSocialCamera.CameraState currentState;
	}
}
