using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using CjLib;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaNetworking;
using GorillaTag.Cosmetics;
using GorillaTag.GuidedRefs;
using GorillaTagScripts;
using Photon.Pun;
using Photon.Voice.Unity;
using Steamworks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.XR;

// Token: 0x020008CA RID: 2250
public class GorillaTagger : MonoBehaviour, IGuidedRefReceiverMono, IGuidedRefMonoBehaviour, IGuidedRefObject
{
	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x06003ACF RID: 15055 RVA: 0x0013F584 File Offset: 0x0013D784
	public static GorillaTagger Instance
	{
		get
		{
			return GorillaTagger._instance;
		}
	}

	// Token: 0x17000547 RID: 1351
	// (get) Token: 0x06003AD0 RID: 15056 RVA: 0x0013F58B File Offset: 0x0013D78B
	public bool ForcePerfRefreshRate
	{
		get
		{
			return this._forcePerfRefreshRate;
		}
	}

	// Token: 0x06003AD1 RID: 15057 RVA: 0x0013F594 File Offset: 0x0013D794
	public void SetExtraHandPosition(StiltID stiltID, Vector3 position, bool canTag, bool canStun)
	{
		this.stiltTagData[(int)stiltID].currentPositionForTag = position;
		this.stiltTagData[(int)stiltID].hasCurrentPosition = true;
		this.stiltTagData[(int)stiltID].canTag = canTag;
		this.stiltTagData[(int)stiltID].canStun = canStun;
	}

	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06003AD2 RID: 15058 RVA: 0x0013F5EA File Offset: 0x0013D7EA
	public NetworkView myVRRig
	{
		get
		{
			return this.offlineVRRig.netView;
		}
	}

	// Token: 0x17000549 RID: 1353
	// (get) Token: 0x06003AD3 RID: 15059 RVA: 0x0013F5F7 File Offset: 0x0013D7F7
	internal VRRigSerializer rigSerializer
	{
		get
		{
			return this.offlineVRRig.rigSerializer;
		}
	}

	// Token: 0x1700054A RID: 1354
	// (get) Token: 0x06003AD4 RID: 15060 RVA: 0x0013F604 File Offset: 0x0013D804
	public bool PerformanceOn
	{
		get
		{
			return this._performanceOn;
		}
	}

	// Token: 0x1700054B RID: 1355
	// (get) Token: 0x06003AD5 RID: 15061 RVA: 0x0013F60C File Offset: 0x0013D80C
	// (set) Token: 0x06003AD6 RID: 15062 RVA: 0x0013F614 File Offset: 0x0013D814
	public Rigidbody rigidbody { get; private set; }

	// Token: 0x1700054C RID: 1356
	// (get) Token: 0x06003AD7 RID: 15063 RVA: 0x0013F61D File Offset: 0x0013D81D
	public float DefaultHandTapVolume
	{
		get
		{
			return this.cacheHandTapVolume;
		}
	}

	// Token: 0x1700054D RID: 1357
	// (get) Token: 0x06003AD8 RID: 15064 RVA: 0x0013F625 File Offset: 0x0013D825
	// (set) Token: 0x06003AD9 RID: 15065 RVA: 0x0013F62D File Offset: 0x0013D82D
	public Recorder myRecorder { get; private set; }

	// Token: 0x1700054E RID: 1358
	// (get) Token: 0x06003ADA RID: 15066 RVA: 0x0013F636 File Offset: 0x0013D836
	public float sphereCastRadius
	{
		get
		{
			if (this.tagRadiusOverride == null)
			{
				return 0.03f;
			}
			return this.tagRadiusOverride.Value;
		}
	}

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x06003ADB RID: 15067 RVA: 0x0013F658 File Offset: 0x0013D858
	// (remove) Token: 0x06003ADC RID: 15068 RVA: 0x0013F690 File Offset: 0x0013D890
	public event Action<bool, Vector3, Vector3> OnHandTap;

	// Token: 0x1700054F RID: 1359
	// (get) Token: 0x06003ADD RID: 15069 RVA: 0x0013F6C5 File Offset: 0x0013D8C5
	// (set) Token: 0x06003ADE RID: 15070 RVA: 0x0013F6CD File Offset: 0x0013D8CD
	public bool hasTappedSurface { get; private set; }

	// Token: 0x06003ADF RID: 15071 RVA: 0x0013F6D6 File Offset: 0x0013D8D6
	public void ResetTappedSurfaceCheck()
	{
		this.hasTappedSurface = false;
	}

	// Token: 0x06003AE0 RID: 15072 RVA: 0x0013F6DF File Offset: 0x0013D8DF
	public void SetTagRadiusOverrideThisFrame(float radius)
	{
		this.tagRadiusOverride = new float?(radius);
		this.tagRadiusOverrideFrame = Time.frameCount;
	}

	// Token: 0x06003AE1 RID: 15073 RVA: 0x0013F6F8 File Offset: 0x0013D8F8
	protected void Awake()
	{
		this.GuidedRefInitialize();
		this.RecoverMissingRefs();
		this.MirrorCameraCullingMask = new Watchable<int>(this.BaseMirrorCameraCullingMask);
		this.stiltTagData[0].isLeftHand = true;
		this.stiltTagData[4].isLeftHand = true;
		this.stiltTagData[5].isLeftHand = true;
		this.stiltTagData[2].isLeftHand = true;
		this.stiltTagData[6].isLeftHand = true;
		this.stiltTagData[7].isLeftHand = true;
		if (GorillaTagger._instance != null && GorillaTagger._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GorillaTagger._instance = this;
			GorillaTagger.hasInstance = true;
			Action action = GorillaTagger.onPlayerSpawnedRootCallback;
			if (action != null)
			{
				action();
			}
		}
		GRFirstTimeUserExperience grfirstTimeUserExperience = Object.FindAnyObjectByType<GRFirstTimeUserExperience>(FindObjectsInactive.Include);
		GameObject gameObject = ((grfirstTimeUserExperience != null) ? grfirstTimeUserExperience.gameObject : null);
		if (!this.disableTutorial && (this.testTutorial || (PlayerPrefs.GetString("tutorial") != "done" && PlayerPrefs.GetString("didTutorial") != "done" && NetworkSystemConfig.AppVersion != "dev")))
		{
			base.transform.parent.position = new Vector3(-140f, 28f, -102f);
			base.transform.parent.eulerAngles = new Vector3(0f, 180f, 0f);
			GTPlayer.Instance.InitializeValues();
			PlayerPrefs.SetFloat("redValue", Random.value);
			PlayerPrefs.SetFloat("greenValue", Random.value);
			PlayerPrefs.SetFloat("blueValue", Random.value);
			PlayerPrefs.Save();
		}
		else
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("didTutorial", true);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable, null, null);
			PlayerPrefs.SetString("didTutorial", "done");
			PlayerPrefs.Save();
			bool flag = true;
			if (gameObject != null && PlayerPrefs.GetString("spawnInWrongStump") == "flagged" && flag)
			{
				gameObject.SetActive(true);
				GRFirstTimeUserExperience grfirstTimeUserExperience2;
				if (gameObject.TryGetComponent<GRFirstTimeUserExperience>(out grfirstTimeUserExperience2) && grfirstTimeUserExperience2.spawnPoint != null)
				{
					GTPlayer.Instance.TeleportTo(grfirstTimeUserExperience2.spawnPoint.position, grfirstTimeUserExperience2.spawnPoint.rotation, false, false);
					GTPlayer.Instance.InitializeValues();
					PlayerPrefs.DeleteKey("spawnInWrongStump");
					PlayerPrefs.Save();
				}
			}
		}
		this.thirdPersonCamera.SetActive(Application.platform != RuntimePlatform.Android);
		this.inputDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
		this.wasInOverlay = false;
		this.baseSlideControl = GTPlayer.Instance.slideControl;
		this.gorillaTagColliderLayerMask = UnityLayer.GorillaTagCollider.ToLayerMask();
		this.rigidbody = base.GetComponent<Rigidbody>();
		this.cacheHandTapVolume = this.handTapVolume;
		OVRManager.foveatedRenderingLevel = OVRManager.FoveatedRenderingLevel.Medium;
		this._leftHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
		this._rightHandDown = new GorillaTagger.DebouncedBool(this._framesForHandTrigger, false);
		this.ClearFramerateTracker();
	}

	// Token: 0x06003AE2 RID: 15074 RVA: 0x0013FA1D File Offset: 0x0013DC1D
	protected void OnDestroy()
	{
		if (GorillaTagger._instance == this)
		{
			GorillaTagger._instance = null;
			GorillaTagger.hasInstance = false;
		}
	}

	// Token: 0x06003AE3 RID: 15075 RVA: 0x0013FA38 File Offset: 0x0013DC38
	private async void IsXRSubsystemActive()
	{
		this.loadedDeviceName = XRSettings.loadedDeviceName;
		while (!this.xrSubsystemIsActive)
		{
			List<XRDisplaySubsystem> list = new List<XRDisplaySubsystem>();
			SubsystemManager.GetSubsystems<XRDisplaySubsystem>(list);
			foreach (XRDisplaySubsystem xrdisplaySubsystem in list)
			{
				if (xrdisplaySubsystem.running)
				{
					this.xrSubsystemIsActive = true;
					this.activeXRDisplay = xrdisplaySubsystem;
					return;
				}
			}
			await Awaitable.WaitForSecondsAsync(0.1f, default(CancellationToken));
		}
	}

	// Token: 0x06003AE4 RID: 15076 RVA: 0x0013FA6F File Offset: 0x0013DC6F
	public bool IsOculusQuest2()
	{
		return Application.platform == RuntimePlatform.Android && OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2;
	}

	// Token: 0x06003AE5 RID: 15077 RVA: 0x0013FA88 File Offset: 0x0013DC88
	protected void Start()
	{
		this.IsXRSubsystemActive();
		if (this.loadedDeviceName == "OpenVR Display")
		{
			Quaternion quaternion = Quaternion.Euler(new Vector3(-90f, 180f, -20f));
			Quaternion quaternion2 = Quaternion.Euler(new Vector3(-90f, 180f, 20f));
			Quaternion quaternion3 = Quaternion.Euler(new Vector3(-141f, 204f, -27f));
			Quaternion quaternion4 = Quaternion.Euler(new Vector3(-141f, 156f, 27f));
			GTPlayer.Instance.SetHandOffsets(true, new Vector3(-0.02f, 0f, -0.07f), quaternion3 * Quaternion.Inverse(quaternion));
			GTPlayer.Instance.SetHandOffsets(false, new Vector3(0.02f, 0f, -0.07f), quaternion4 * Quaternion.Inverse(quaternion2));
		}
		this.bodyVector = new Vector3(0f, this.bodyCollider.height / 2f - this.bodyCollider.radius, 0f);
		if (SteamManager.Initialized)
		{
			this.gameOverlayActivatedCb = Callback<GameOverlayActivated_t>.Create(new Callback<GameOverlayActivated_t>.DispatchDelegate(this.OnGameOverlayActivated));
		}
	}

	// Token: 0x06003AE6 RID: 15078 RVA: 0x0013FBBE File Offset: 0x0013DDBE
	private void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		this.isGameOverlayActive = pCallback.m_bActive > 0;
	}

	// Token: 0x06003AE7 RID: 15079 RVA: 0x0013FBCF File Offset: 0x0013DDCF
	[ContextMenu("Toggle Performance Refresh Rate")]
	public void ToggleForcedPerformanceRefresh()
	{
		this.SetForcedRefreshRate(true, 72f);
	}

	// Token: 0x06003AE8 RID: 15080 RVA: 0x0013FBDD File Offset: 0x0013DDDD
	public void ToggleDefaultPerformanceRefresh()
	{
		this.SetForcedRefreshRate(false, this._defaultRefreshRate);
	}

	// Token: 0x06003AE9 RID: 15081 RVA: 0x0013FBEC File Offset: 0x0013DDEC
	public void ToggleForcedRefreshRate(float newRefreshRate = 90f)
	{
		this.SetForcedRefreshRate(!this._forcePerfRefreshRate, newRefreshRate);
	}

	// Token: 0x06003AEA RID: 15082 RVA: 0x0013FC00 File Offset: 0x0013DE00
	public void SetForcedRefreshRate(bool forcePerf, float newRefreshRate = 90f)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - {0} / {1}", forcePerf, newRefreshRate));
		this._framerateUpdated = false;
		this._forceFramerateCheck = true;
		this._forcePerfRefreshRate = forcePerf;
		this._perfRefreshRate = Mathf.Clamp(newRefreshRate, 32f, 144f);
		this._performanceOn = newRefreshRate <= 72f;
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - New refresh {0} with perf {1}", this._perfRefreshRate, this._performanceOn));
		this.UpdateResolutionScale(this._performanceOn);
		if (forcePerf)
		{
			DebugHudStats.FPS_THRESHOLD = (int)this._perfRefreshRate - 1;
		}
		else
		{
			DebugHudStats.FPS_THRESHOLD = (int)this._defaultRefreshRate - 1;
		}
		Debug.Log(string.Format("GorillaTagger - SetForcedRefreshRate - New DebugHudStats FPS threshold {0}", DebugHudStats.FPS_THRESHOLD));
	}

	// Token: 0x06003AEB RID: 15083 RVA: 0x0013FCDC File Offset: 0x0013DEDC
	private void ClearFramerateTracker()
	{
		this._framerateIndex = 0;
		this._framerateTotal = 0f;
		for (int i = 0; i < this._framerateTracker.Length; i++)
		{
			this._framerateTracker[i] = 0f;
		}
	}

	// Token: 0x06003AEC RID: 15084 RVA: 0x0013FD1C File Offset: 0x0013DF1C
	private void UpdateResolutionScale(bool performanceMode)
	{
		float num = 1f;
		if (performanceMode)
		{
			num = 0.975f;
			if (Application.platform == RuntimePlatform.Android)
			{
				num = 0.95f;
				if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2)
				{
					num = 0.9f;
				}
			}
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			num = 0.975f;
			if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Oculus_Quest_2)
			{
				num = 0.925f;
			}
		}
		XRSettings.eyeTextureResolutionScale = num;
		XRSettings.renderViewportScale = num;
		Debug.Log(string.Format("GorillaTagger - UpdateResolutionScale - {0}", num));
	}

	// Token: 0x06003AED RID: 15085 RVA: 0x0013FD98 File Offset: 0x0013DF98
	protected void LateUpdate()
	{
		GorillaTagger.<>c__DisplayClass159_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.isGameOverlayActive)
		{
			if (this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(false);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = true;
		}
		else
		{
			if (!this.leftHandTriggerCollider.activeSelf)
			{
				this.leftHandTriggerCollider.SetActive(true);
				this.rightHandTriggerCollider.SetActive(true);
			}
			GTPlayer.Instance.inOverlay = false;
		}
		this._framerateTimer -= Time.deltaTime;
		if (this._framerateTimer <= 0f)
		{
			this._framerateTimer += 0.1f;
			if (Time.smoothDeltaTime > 0f)
			{
				float num = 1f / Time.smoothDeltaTime;
				this._framerateTotal -= this._framerateTracker[this._framerateIndex];
				this._framerateTracker[this._framerateIndex] = num;
				this._framerateTotal += num;
				this._framerateIndex++;
				if (this._framerateIndex >= this._framerateTracker.Length)
				{
					this._framerateIndex = 0;
				}
				this._prevSmoothedFramerate = this.SmoothedFramerate;
				this.SmoothedFramerate = Mathf.RoundToInt(this._framerateTotal / (float)this._framerateTracker.Length);
				int smoothedFramerate = this.SmoothedFramerate;
				int fps_THRESHOLD = DebugHudStats.FPS_THRESHOLD;
			}
		}
		if (this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android && this.activeXRDisplay != null && this.activeXRDisplay.TryGetDisplayRefreshRate(out this._defaultRefreshRate))
		{
			float num2 = (this._forcePerfRefreshRate ? this._perfRefreshRate : this._defaultRefreshRate);
			float num3 = 1f / num2;
			if (num2 > 0f)
			{
				DebugHudStats.FPS_THRESHOLD = (int)num2 - 1;
			}
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num3) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				Debug.Log(" =========== Adjusting refresh size =========");
				Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" Refresh rate         :\t" + num2.ToString());
				Time.fixedDeltaTime = num3;
				this.UpdateResolutionScale(num2 < this._defaultRefreshRate);
				Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
				Debug.Log(" History size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
				GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(num2 * 0.083333336f), 10), 6);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log("New history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
				Debug.Log(" ============================================");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(num2);
				GTPlayer.Instance.InitializeValues();
			}
		}
		else if (Application.platform != RuntimePlatform.Android && OVRManager.instance != null && OVRManager.OVRManagerinitialized && OVRManager.instance.gameObject != null && OVRManager.instance.gameObject.activeSelf)
		{
			Object.Destroy(OVRManager.instance.gameObject);
		}
		else if ((this._forceFramerateCheck && OVRManager.instance != null) || (!this._framerateUpdated && Application.platform == RuntimePlatform.Android && OVRManager.instance.gameObject.activeSelf))
		{
			InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;
			int num4 = OVRManager.display.displayFrequenciesAvailable.Length - 1;
			float num5 = OVRManager.display.displayFrequenciesAvailable[num4];
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			while (num5 > 90f)
			{
				num4--;
				if (num4 < 0)
				{
					break;
				}
				num5 = OVRManager.display.displayFrequenciesAvailable[num4];
			}
			this._defaultRefreshRate = num5;
			if (this._forcePerfRefreshRate)
			{
				num5 = this._perfRefreshRate;
			}
			float num6 = 1f;
			float num7 = 1f / num5;
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num7 * num6) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				float num8 = Time.fixedDeltaTime - num7 * num6;
				Debug.Log(" =========== ADJUSTING REFRESH SIZE ========= ");
				Debug.Log(string.Format("!!!! Time.fixedDeltaTime - (1f / newRefreshRate) * {0}) {1}", num6, num8));
				Debug.Log(string.Format("Old Refresh rate: {0}", systemDisplayFrequency));
				Debug.Log(string.Format("New Refresh rate: {0}", num5));
				Debug.Log(string.Format("   fixedDeltaTime before:\t{0}", Time.fixedDeltaTime));
				Debug.Log(string.Format("   fixedDeltaTime after :\t{0}", num7));
				Application.targetFrameRate = (int)num5;
				Time.fixedDeltaTime = num7 * num6;
				OVRPlugin.systemDisplayFrequency = num5;
				this.UpdateResolutionScale(num5 <= 72f);
				GTPlayer.Instance.velocityHistorySize = Mathf.FloorToInt(num5 * 0.083333336f);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(string.Format("   FixedDeltaTime after :\t{0}", Time.fixedDeltaTime));
				Debug.Log(string.Format("   History size before  :\t{0}", GTPlayer.Instance.velocityHistorySize));
				Debug.Log(string.Format("New history size: {0}", GTPlayer.Instance.velocityHistorySize));
				Debug.Log(" ============================================ ");
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(XRDevice.refreshRate);
				GTPlayer.Instance.InitializeValues();
				OVRManager.instance.gameObject.SetActive(false);
				this._framerateUpdated = true;
				this.ConfirmUpdatedFrameRate();
			}
		}
		else if (!this.xrSubsystemIsActive && Application.platform != RuntimePlatform.Android)
		{
			this._defaultRefreshRate = 144f;
			int num9 = (this._forcePerfRefreshRate ? ((int)this._perfRefreshRate) : ((int)this._defaultRefreshRate));
			float num10 = 1f / (float)num9;
			if (this._forceFramerateCheck || Mathf.Abs(Time.fixedDeltaTime - num10) > 0.0001f)
			{
				this._forceFramerateCheck = false;
				Debug.Log(string.Format("Updating delta time. Was: {0}. Now it's {1} at framerate {2}.", Time.fixedDeltaTime, num10, num9));
				Application.targetFrameRate = num9;
				Time.fixedDeltaTime = num10;
				this.UpdateResolutionScale((float)num9 < this._defaultRefreshRate);
				GTPlayer.Instance.velocityHistorySize = Mathf.Min(Mathf.FloorToInt((float)num9 * 0.083333336f), 10);
				if (GTPlayer.Instance.velocityHistorySize > 9)
				{
					GTPlayer.Instance.velocityHistorySize--;
				}
				Debug.Log(string.Format("New history size: {0}", GTPlayer.Instance.velocityHistorySize));
				GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl((float)num9);
				GTPlayer.Instance.InitializeValues();
			}
		}
		this.otherPlayer = null;
		this.touchedPlayer = null;
		CS$<>8__locals1.otherTouchedPlayer = null;
		if (this.tagRadiusOverrideFrame < Time.frameCount)
		{
			this.tagRadiusOverride = null;
		}
		Vector3 position = this.leftHandTransform.position;
		Vector3 position2 = this.rightHandTransform.position;
		Vector3 position3 = this.headCollider.transform.position;
		Vector3 position4 = this.bodyCollider.transform.position;
		float scale = GTPlayer.Instance.scale;
		float num11 = this.sphereCastRadius * scale;
		CS$<>8__locals1.bodyHit = false;
		CS$<>8__locals1.leftHandHit = false;
		CS$<>8__locals1.canTagHit = false;
		CS$<>8__locals1.canStunHit = false;
		if (!(GorillaGameManager.instance is CasualGameMode))
		{
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastLeftHandPositionForTag, position, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(true, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(this.lastRightHandPositionForTag, position2, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(position3, position2, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(false, this.maxTagDistance, true, false, ref CS$<>8__locals1);
			for (int i = 0; i < 12; i++)
			{
				GorillaTagger.StiltTagData stiltTagData = this.stiltTagData[i];
				if (stiltTagData.hasLastPosition && stiltTagData.hasCurrentPosition && (stiltTagData.canTag || stiltTagData.canStun))
				{
					this.nonAllocHits = Physics.OverlapCapsuleNonAlloc(stiltTagData.currentPositionForTag, stiltTagData.lastPositionForTag, num11, this.colliderOverlaps, this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
					this.<LateUpdate>g__TryTaggingAllHitsOverlap|159_0(i == 0 || i == 2, this.maxStiltTagDistance, stiltTagData.canTag, stiltTagData.canStun, ref CS$<>8__locals1);
				}
			}
			this.topVector = this.lastHeadPositionForTag;
			this.bottomVector = this.lastBodyPositionForTag - this.bodyVector;
			this.nonAllocHits = Physics.CapsuleCastNonAlloc(this.topVector, this.bottomVector, this.bodyCollider.radius * 2f * GTPlayer.Instance.scale, this.bodyRaycastSweep.normalized, this.nonAllocRaycastHits, Mathf.Max(this.bodyRaycastSweep.magnitude, num11), this.gorillaTagColliderLayerMask, QueryTriggerInteraction.Collide);
			this.<LateUpdate>g__TryTaggingAllHitsCapsulecast|159_1(this.maxTagDistance, true, false, ref CS$<>8__locals1);
		}
		if (this.otherPlayer != null)
		{
			if (CS$<>8__locals1.canTagHit && (!CS$<>8__locals1.canStunHit || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.otherPlayer)))
			{
				GameMode.ActiveGameMode.LocalTag(this.otherPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.bodyHit, CS$<>8__locals1.leftHandHit);
				GameMode.ReportTag(this.otherPlayer);
			}
			if (CS$<>8__locals1.canStunHit)
			{
				RoomSystem.SendStatusEffectToPlayer(RoomSystem.StatusEffects.TaggedTime, this.otherPlayer);
			}
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null && GorillaGameManager.instance != null)
		{
			CustomGameMode.TouchPlayer(CS$<>8__locals1.otherTouchedPlayer);
		}
		if (CS$<>8__locals1.otherTouchedPlayer != null)
		{
			this.HitWithKnockBack(CS$<>8__locals1.otherTouchedPlayer, NetworkSystem.Instance.LocalPlayer, CS$<>8__locals1.leftHandHit);
		}
		bool flag = true;
		StiltID stiltID = StiltID.None;
		this.ProcessHandTapping(in flag, in stiltID, ref this.lastLeftTap, ref this.lastLeftUpTap, ref this.leftHandWasTouching, in this.leftHandSlideSource);
		flag = false;
		stiltID = StiltID.None;
		this.ProcessHandTapping(in flag, in stiltID, ref this.lastRightTap, ref this.lastRightUpTap, ref this.rightHandWasTouching, in this.rightHandSlideSource);
		for (int j = 0; j < 12; j++)
		{
			GorillaTagger.StiltTagData stiltTagData2 = this.stiltTagData[j];
			if (stiltTagData2.hasLastPosition && stiltTagData2.hasCurrentPosition)
			{
				stiltID = (StiltID)j;
				this.ProcessHandTapping(in stiltTagData2.isLeftHand, in stiltID, ref stiltTagData2.lastTap, ref stiltTagData2.lastUpTap, ref stiltTagData2.wasTouching, in this.leftHandSlideSource);
				this.stiltTagData[j] = stiltTagData2;
			}
		}
		this.CheckEndStatusEffect();
		this.lastLeftHandPositionForTag = position;
		this.lastRightHandPositionForTag = position2;
		this.lastBodyPositionForTag = position4;
		this.lastHeadPositionForTag = position3;
		for (int k = 0; k < 12; k++)
		{
			GorillaTagger.StiltTagData stiltTagData3 = this.stiltTagData[k];
			if (stiltTagData3.hasLastPosition || stiltTagData3.hasCurrentPosition)
			{
				stiltTagData3.lastPositionForTag = stiltTagData3.currentPositionForTag;
				stiltTagData3.hasLastPosition = stiltTagData3.hasCurrentPosition;
				stiltTagData3.hasCurrentPosition = false;
				this.stiltTagData[k] = stiltTagData3;
			}
		}
		if (GTPlayer.Instance.IsBodySliding && (double)GTPlayer.Instance.RigidbodyVelocity.magnitude >= 0.15)
		{
			if (!this.bodySlideSource.isPlaying)
			{
				this.bodySlideSource.Play();
			}
		}
		else
		{
			this.bodySlideSource.Stop();
		}
		if (GorillaComputer.instance == null || NetworkSystem.Instance.LocalRecorder == null)
		{
			return;
		}
		if (float.IsFinite(GorillaTagger.moderationMutedTime) && GorillaTagger.moderationMutedTime >= 0f)
		{
			GorillaTagger.moderationMutedTime -= Time.deltaTime;
		}
		if (GorillaComputer.instance.voiceChatOn == "TRUE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (GorillaTagger.moderationMutedTime > 0f)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						bool transmitEnabled = this.myRecorder.TransmitEnabled;
						this.myRecorder.TransmitEnabled = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
						{
							this.myRecorder.TransmitEnabled = true;
							return;
						}
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
					{
						this.myRecorder.TransmitEnabled = true;
						return;
					}
				}
				else if (GorillaComputer.instance.pttType == "PUSH TO TALK")
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = false;
					bool transmitEnabled2 = this.myRecorder.TransmitEnabled;
					this.myRecorder.TransmitEnabled = false;
					return;
				}
			}
			else
			{
				if (GorillaTagger.moderationMutedTime <= 0f && !this.myRecorder.TransmitEnabled)
				{
					this.myRecorder.TransmitEnabled = true;
				}
				if (!this.offlineVRRig.shouldSendSpeakingLoudness)
				{
					this.offlineVRRig.shouldSendSpeakingLoudness = true;
					return;
				}
			}
		}
		else if (GorillaComputer.instance.voiceChatOn == "FALSE")
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (!this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = true;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
			if (GorillaComputer.instance.pttType != "OPEN MIC")
			{
				this.primaryButtonPressRight = false;
				this.secondaryButtonPressRight = false;
				this.primaryButtonPressLeft = false;
				this.secondaryButtonPressLeft = false;
				this.primaryButtonPressRight = ControllerInputPoller.PrimaryButtonPress(XRNode.RightHand);
				this.secondaryButtonPressRight = ControllerInputPoller.SecondaryButtonPress(XRNode.RightHand);
				this.primaryButtonPressLeft = ControllerInputPoller.PrimaryButtonPress(XRNode.LeftHand);
				this.secondaryButtonPressLeft = ControllerInputPoller.SecondaryButtonPress(XRNode.LeftHand);
				if (this.primaryButtonPressRight || this.secondaryButtonPressRight || this.primaryButtonPressLeft || this.secondaryButtonPressLeft)
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
				}
				else
				{
					if (GorillaComputer.instance.pttType == "PUSH TO MUTE")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = true;
						return;
					}
					if (GorillaComputer.instance.pttType == "PUSH TO TALK")
					{
						this.offlineVRRig.shouldSendSpeakingLoudness = false;
						return;
					}
				}
			}
			else if (!this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = true;
				return;
			}
		}
		else
		{
			this.myRecorder = NetworkSystem.Instance.LocalRecorder;
			if (this.offlineVRRig.remoteUseReplacementVoice)
			{
				this.offlineVRRig.remoteUseReplacementVoice = false;
			}
			if (this.offlineVRRig.shouldSendSpeakingLoudness)
			{
				this.offlineVRRig.shouldSendSpeakingLoudness = false;
			}
			if (this.myRecorder.TransmitEnabled)
			{
				this.myRecorder.TransmitEnabled = false;
			}
		}
	}

	// Token: 0x06003AEE RID: 15086 RVA: 0x00140E2C File Offset: 0x0013F02C
	private bool TryToTag(VRRig rig, Vector3 hitObjectPos, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedPlayer)
	{
		taggedPlayer = null;
		touchedPlayer = null;
		if (NetworkSystem.Instance.InRoom)
		{
			this.tempCreator = ((rig != null) ? rig.creator : null);
			if (this.tempCreator != null && NetworkSystem.Instance.LocalPlayer != this.tempCreator)
			{
				touchedPlayer = this.tempCreator;
				if (GorillaGameManager.instance != null && Time.time > this.taggedTime + this.tagCooldown && (canStun || GorillaGameManager.instance.LocalCanTag(NetworkSystem.Instance.LocalPlayer, this.tempCreator)) && (this.headCollider.transform.position - hitObjectPos).sqrMagnitude < maxTagDistance * maxTagDistance * GTPlayer.Instance.scale)
				{
					if (!isBodyTag)
					{
						this.StartVibration((this.leftHandTransform.position - hitObjectPos).magnitude < (this.rightHandTransform.position - hitObjectPos).magnitude, this.tagHapticStrength, this.tagHapticDuration);
					}
					else
					{
						this.StartVibration(true, this.tagHapticStrength, this.tagHapticDuration);
						this.StartVibration(false, this.tagHapticStrength, this.tagHapticDuration);
					}
					taggedPlayer = this.tempCreator;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06003AEF RID: 15087 RVA: 0x00140F8C File Offset: 0x0013F18C
	private bool TryToTag(Collider hitCollider, bool isBodyTag, bool canStun, float maxTagDistance, out NetPlayer taggedPlayer, out NetPlayer touchedNetPlayer)
	{
		VRRig vrrig;
		if (!this.tagRigDict.TryGetValue(hitCollider, out vrrig))
		{
			vrrig = hitCollider.GetComponentInParent<VRRig>();
			this.tagRigDict.Add(hitCollider, vrrig);
		}
		if (vrrig == null)
		{
			PropHuntTaggableProp componentInParent = hitCollider.GetComponentInParent<PropHuntTaggableProp>();
			if (!(componentInParent != null))
			{
				taggedPlayer = null;
				touchedNetPlayer = null;
				return false;
			}
			vrrig = componentInParent.ownerRig;
		}
		else if (GorillaGameManager.instance != null && GorillaGameManager.instance.GameType() == GameModeType.PropHunt)
		{
			taggedPlayer = null;
			touchedNetPlayer = null;
			return false;
		}
		return this.TryToTag(vrrig, hitCollider.transform.position, isBodyTag, canStun, maxTagDistance, out taggedPlayer, out touchedNetPlayer);
	}

	// Token: 0x06003AF0 RID: 15088 RVA: 0x00141028 File Offset: 0x0013F228
	private void HitWithKnockBack(NetPlayer taggedPlayer, NetPlayer taggingPlayer, bool leftHand)
	{
		Vector3 averageVelocity = GTPlayer.Instance.GetHandVelocityTracker(leftHand).GetAverageVelocity(true, 0.15f, false);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(taggingPlayer, out rigContainer))
		{
			return;
		}
		VRMap vrmap = (leftHand ? rigContainer.Rig.leftHand : rigContainer.Rig.rightHand);
		Vector3 vector = (leftHand ? (-vrmap.rigTarget.right) : vrmap.rigTarget.right);
		RigContainer rigContainer2;
		CosmeticEffectsOnPlayers.CosmeticEffect cosmeticEffect;
		if (VRRigCache.Instance.TryGetVrrig(taggedPlayer, out rigContainer2) && rigContainer2.Rig.TemporaryCosmeticEffects.TryGetValue(CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback, out cosmeticEffect))
		{
			RoomSystem.HitPlayer(taggedPlayer, vector.normalized, averageVelocity.magnitude);
		}
	}

	// Token: 0x06003AF1 RID: 15089 RVA: 0x001410D3 File Offset: 0x0013F2D3
	public void StartVibration(bool forLeftController, float amplitude, float duration)
	{
		base.StartCoroutine(this.HapticPulses(forLeftController, amplitude, duration));
	}

	// Token: 0x06003AF2 RID: 15090 RVA: 0x001410E5 File Offset: 0x0013F2E5
	private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
	{
		float startTime = Time.time;
		uint channel = 0U;
		global::UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			device = ControllerInputPoller.instance.leftControllerDevice;
		}
		else
		{
			device = ControllerInputPoller.instance.rightControllerDevice;
		}
		while (Time.time < startTime + duration)
		{
			device.SendHapticImpulse(channel, amplitude, this.hapticWaitSeconds);
			yield return new WaitForSeconds(this.hapticWaitSeconds * 0.9f);
		}
		yield break;
	}

	// Token: 0x06003AF3 RID: 15091 RVA: 0x0014110C File Offset: 0x0013F30C
	public void PlayHapticClip(bool forLeftController, AudioClip clip, float strength)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
			}
			this.leftHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
			return;
		}
		if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
		}
		this.rightHapticsRoutine = base.StartCoroutine(this.AudioClipHapticPulses(forLeftController, clip, strength));
	}

	// Token: 0x06003AF4 RID: 15092 RVA: 0x0014116F File Offset: 0x0013F36F
	public void StopHapticClip(bool forLeftController)
	{
		if (forLeftController)
		{
			if (this.leftHapticsRoutine != null)
			{
				base.StopCoroutine(this.leftHapticsRoutine);
				this.leftHapticsRoutine = null;
				return;
			}
		}
		else if (this.rightHapticsRoutine != null)
		{
			base.StopCoroutine(this.rightHapticsRoutine);
			this.rightHapticsRoutine = null;
		}
	}

	// Token: 0x06003AF5 RID: 15093 RVA: 0x001411AB File Offset: 0x0013F3AB
	private IEnumerator AudioClipHapticPulses(bool forLeftController, AudioClip clip, float strength)
	{
		uint channel = 0U;
		int bufferSize = 8192;
		int sampleWindowSize = 256;
		float[] audioData;
		global::UnityEngine.XR.InputDevice device;
		if (forLeftController)
		{
			float[] array;
			if ((array = this.leftHapticsBuffer) == null)
			{
				array = (this.leftHapticsBuffer = new float[bufferSize]);
			}
			audioData = array;
			device = ControllerInputPoller.instance.leftControllerDevice;
		}
		else
		{
			float[] array2;
			if ((array2 = this.rightHapticsBuffer) == null)
			{
				array2 = (this.rightHapticsBuffer = new float[bufferSize]);
			}
			audioData = array2;
			device = ControllerInputPoller.instance.rightControllerDevice;
		}
		int sampleOffset = -bufferSize;
		float startTime = Time.time;
		float length = clip.length;
		float endTime = Time.time + length;
		float sampleRate = (float)clip.samples;
		while (Time.time <= endTime)
		{
			float num = (Time.time - startTime) / length;
			int num2 = (int)(sampleRate * num);
			if (Mathf.Max(num2 + sampleWindowSize - 1, audioData.Length - 1) >= sampleOffset + bufferSize)
			{
				clip.GetData(audioData, num2);
				sampleOffset = num2;
			}
			float num3 = 0f;
			int num4 = Mathf.Min(clip.samples - num2, sampleWindowSize);
			for (int i = 0; i < num4; i++)
			{
				float num5 = audioData[num2 - sampleOffset + i];
				num3 += num5 * num5;
			}
			float num6 = Mathf.Clamp01(((num4 > 0) ? Mathf.Sqrt(num3 / (float)num4) : 0f) * strength);
			device.SendHapticImpulse(channel, num6, Time.fixedDeltaTime);
			yield return null;
		}
		if (forLeftController)
		{
			this.leftHapticsRoutine = null;
		}
		else
		{
			this.rightHapticsRoutine = null;
		}
		yield break;
	}

	// Token: 0x06003AF6 RID: 15094 RVA: 0x001411D0 File Offset: 0x0013F3D0
	public void DoVibration(XRNode node, float amplitude, float duration)
	{
		global::UnityEngine.XR.InputDevice deviceAtXRNode = InputDevices.GetDeviceAtXRNode(node);
		if (deviceAtXRNode.isValid)
		{
			deviceAtXRNode.SendHapticImpulse(0U, amplitude, duration);
		}
	}

	// Token: 0x06003AF7 RID: 15095 RVA: 0x001411F8 File Offset: 0x0013F3F8
	public void UpdateColor(float red, float green, float blue)
	{
		this.offlineVRRig.InitializeNoobMaterialLocal(red, green, blue);
		if (NetworkSystem.Instance != null && !NetworkSystem.Instance.InRoom)
		{
			this.offlineVRRig.bodyRenderer.ResetBodyMaterial();
		}
	}

	// Token: 0x06003AF8 RID: 15096 RVA: 0x00141234 File Offset: 0x0013F434
	protected void OnTriggerEnter(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxTriggered();
		}
	}

	// Token: 0x06003AF9 RID: 15097 RVA: 0x00141254 File Offset: 0x0013F454
	protected void OnTriggerExit(Collider other)
	{
		GorillaTriggerBox gorillaTriggerBox;
		if (other.TryGetComponent<GorillaTriggerBox>(out gorillaTriggerBox))
		{
			gorillaTriggerBox.OnBoxExited();
		}
	}

	// Token: 0x06003AFA RID: 15098 RVA: 0x00141274 File Offset: 0x0013F474
	public void ShowCosmeticParticles(bool showParticles)
	{
		if (showParticles)
		{
			this.mainCamera.GetComponent<Camera>().cullingMask |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			this.MirrorCameraCullingMask.value |= UnityLayer.GorillaCosmeticParticle.ToLayerMask();
			return;
		}
		this.mainCamera.GetComponent<Camera>().cullingMask &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
		this.MirrorCameraCullingMask.value &= ~UnityLayer.GorillaCosmeticParticle.ToLayerMask();
	}

	// Token: 0x06003AFB RID: 15099 RVA: 0x001412F5 File Offset: 0x0013F4F5
	public void ApplyStatusEffect(GorillaTagger.StatusEffect newStatus, float duration)
	{
		this.EndStatusEffect(this.currentStatus);
		this.currentStatus = newStatus;
		this.statusEndTime = Time.time + duration;
		switch (newStatus)
		{
		case GorillaTagger.StatusEffect.None:
		case GorillaTagger.StatusEffect.Slowed:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = true;
			break;
		default:
			return;
		}
	}

	// Token: 0x06003AFC RID: 15100 RVA: 0x00141335 File Offset: 0x0013F535
	private void CheckEndStatusEffect()
	{
		if (Time.time > this.statusEndTime)
		{
			this.EndStatusEffect(this.currentStatus);
		}
	}

	// Token: 0x06003AFD RID: 15101 RVA: 0x00141350 File Offset: 0x0013F550
	private void EndStatusEffect(GorillaTagger.StatusEffect effectToEnd)
	{
		switch (effectToEnd)
		{
		case GorillaTagger.StatusEffect.None:
			break;
		case GorillaTagger.StatusEffect.Frozen:
			GTPlayer.Instance.disableMovement = false;
			this.currentStatus = GorillaTagger.StatusEffect.None;
			return;
		case GorillaTagger.StatusEffect.Slowed:
			this.currentStatus = GorillaTagger.StatusEffect.None;
			break;
		default:
			return;
		}
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x0014137F File Offset: 0x0013F57F
	private float CalcSlideControl(float fps)
	{
		return Mathf.Pow(Mathf.Pow(1f - this.baseSlideControl, 120f), 1f / fps);
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x001413A3 File Offset: 0x0013F5A3
	public static void OnPlayerSpawned(Action action)
	{
		if (GorillaTagger._instance)
		{
			action();
			return;
		}
		GorillaTagger.onPlayerSpawnedRootCallback = (Action)Delegate.Combine(GorillaTagger.onPlayerSpawnedRootCallback, action);
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x001413D0 File Offset: 0x0013F5D0
	private void ProcessHandTapping(in bool isLeftHand, in StiltID stiltID, ref float lastTapTime, ref float lastTapUpTime, ref bool wasHandTouching, in AudioSource handSlideSource)
	{
		bool flag;
		bool flag2;
		int num;
		GorillaSurfaceOverride gorillaSurfaceOverride;
		RaycastHit raycastHit;
		Vector3 vector;
		GorillaVelocityTracker gorillaVelocityTracker;
		GTPlayer.Instance.GetHandTapData(isLeftHand, stiltID, out flag, out flag2, out num, out gorillaSurfaceOverride, out raycastHit, out vector, out gorillaVelocityTracker);
		GorillaTagger.DebouncedBool debouncedBool = (isLeftHand ? this._leftHandDown : this._rightHandDown);
		if (GTPlayer.Instance.inOverlay)
		{
			handSlideSource.GTStop();
			return;
		}
		if (flag2)
		{
			this.StartVibration(isLeftHand, this.tapHapticStrength / 5f, Time.fixedDeltaTime);
			if (!handSlideSource.isPlaying)
			{
				handSlideSource.GTPlay();
			}
			return;
		}
		handSlideSource.GTStop();
		bool wasStablyEnabled = debouncedBool.WasStablyEnabled;
		debouncedBool.Set(flag);
		bool flag3 = !wasHandTouching && flag && debouncedBool.JustEnabled;
		bool flag4 = wasHandTouching && !flag && wasStablyEnabled;
		wasHandTouching = flag;
		if (!flag4 && !flag3)
		{
			return;
		}
		Tappable tappable = null;
		bool flag5 = gorillaSurfaceOverride != null && gorillaSurfaceOverride.TryGetComponent<Tappable>(out tappable);
		HandEffectContext handEffect = this.offlineVRRig.GetHandEffect(isLeftHand, stiltID);
		if ((!flag5 || !tappable.overrideTapCooldown) && (!handEffect.SeparateUpTapCooldown || !flag4 || Time.time <= lastTapUpTime + this.tapCoolDown) && (!flag3 || Time.time <= lastTapTime + this.tapCoolDown))
		{
			return;
		}
		float sqrMagnitude = (gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false) / GTPlayer.Instance.scale).sqrMagnitude;
		float sqrMagnitude2 = gorillaVelocityTracker.GetAverageVelocity(false, 0.03f, false).sqrMagnitude;
		this.handTapSpeed = Mathf.Sqrt(Mathf.Max(sqrMagnitude, sqrMagnitude2));
		if (handEffect.SeparateUpTapCooldown && flag4)
		{
			lastTapUpTime = Time.time;
		}
		else
		{
			lastTapTime = Time.time;
		}
		this.dirFromHitToHand = Vector3.Normalize(raycastHit.point - vector);
		GorillaAmbushManager gorillaAmbushManager = GameMode.ActiveGameMode as GorillaAmbushManager;
		if (gorillaAmbushManager != null && gorillaAmbushManager.IsInfected(NetworkSystem.Instance.LocalPlayer))
		{
			this.handTapVolume = Mathf.Clamp(this.handTapSpeed, 0f, gorillaAmbushManager.crawlingSpeedForMaxVolume);
		}
		else
		{
			this.handTapVolume = this.cacheHandTapVolume;
		}
		GorillaFreezeTagManager gorillaFreezeTagManager = GameMode.ActiveGameMode as GorillaFreezeTagManager;
		if (gorillaFreezeTagManager != null && gorillaFreezeTagManager.IsFrozen(NetworkSystem.Instance.LocalPlayer))
		{
			this.audioClipIndex = gorillaFreezeTagManager.GetFrozenHandTapAudioIndex();
		}
		else if (gorillaSurfaceOverride != null)
		{
			this.audioClipIndex = gorillaSurfaceOverride.overrideIndex;
		}
		else
		{
			this.audioClipIndex = num;
		}
		if (gorillaSurfaceOverride != null)
		{
			if (gorillaSurfaceOverride.sendOnTapEvent)
			{
				IBuilderTappable builderTappable;
				if (flag5)
				{
					tappable.OnTap(this.handTapVolume);
				}
				else if (gorillaSurfaceOverride.TryGetComponent<IBuilderTappable>(out builderTappable))
				{
					builderTappable.OnTapLocal(this.handTapVolume);
				}
			}
			PlayerGameEvents.TapObject(gorillaSurfaceOverride.name);
		}
		Vector3 averageVelocity = gorillaVelocityTracker.GetAverageVelocity(true, 0.03f, false);
		if (GameMode.ActiveGameMode != null)
		{
			GameMode.ActiveGameMode.HandleHandTap(NetworkSystem.Instance.LocalPlayer, tappable, isLeftHand, averageVelocity, raycastHit.normal);
		}
		this.StartVibration(isLeftHand, this.tapHapticStrength, this.tapHapticDuration);
		this.offlineVRRig.SetHandEffectData(handEffect, this.audioClipIndex, flag3, isLeftHand, stiltID, this.handTapVolume, this.handTapSpeed, this.dirFromHitToHand);
		FXSystem.PlayFX(handEffect);
		Action<bool, Vector3, Vector3> onHandTap = this.OnHandTap;
		if (onHandTap != null)
		{
			onHandTap(isLeftHand, raycastHit.point, raycastHit.normal);
		}
		this.hasTappedSurface = true;
		if (CrittersManager.instance.IsNotNull() && CrittersManager.instance.LocalAuthority())
		{
			CrittersRigActorSetup crittersRigActorSetup = CrittersManager.instance.rigSetupByRig[this.offlineVRRig];
			if (crittersRigActorSetup.IsNotNull())
			{
				CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)crittersRigActorSetup.rigActors[isLeftHand ? 0 : 2].actorSet;
				if (crittersLoudNoise.IsNotNull())
				{
					crittersLoudNoise.PlayHandTapLocal(isLeftHand);
				}
			}
		}
		GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this.offlineVRRig.zoneEntity.currentZone);
		if (managerForZone.IsNotNull() && managerForZone.ghostReactorManager.IsNotNull() && !averageVelocity.AlmostZero())
		{
			Transform handFollower = GTPlayer.Instance.GetHandFollower(isLeftHand);
			RaycastHit raycastHit2;
			if (Physics.Raycast(new Ray(handFollower.position, averageVelocity.normalized), out raycastHit2, 10f))
			{
				Vector3 vector2 = Vector3.ProjectOnPlane(-handFollower.forward, raycastHit2.normal);
				managerForZone.ghostReactorManager.OnTapLocal(isLeftHand, raycastHit2.point + raycastHit2.normal * 0.005f, Quaternion.LookRotation(vector2.normalized, isLeftHand ? (-raycastHit2.normal) : raycastHit2.normal), gorillaSurfaceOverride, averageVelocity);
			}
		}
		if (NetworkSystem.Instance.InRoom && this.myVRRig.IsNotNull() && this.myVRRig != null)
		{
			this.myVRRig.GetView.RPC("OnHandTapRPC", RpcTarget.Others, new object[]
			{
				this.audioClipIndex,
				flag3,
				isLeftHand,
				stiltID,
				this.handTapSpeed,
				Utils.PackVector3ToLong(this.dirFromHitToHand)
			});
		}
	}

	// Token: 0x06003B01 RID: 15105 RVA: 0x0014190C File Offset: 0x0013FB0C
	public async void ConfirmUpdatedFrameRate()
	{
		await Awaitable.WaitForSecondsAsync(1f, default(CancellationToken));
		if (Mathf.RoundToInt(OVRPlugin.systemDisplayFrequency) != Application.targetFrameRate)
		{
			float systemDisplayFrequency = OVRPlugin.systemDisplayFrequency;
			float num = 1f / systemDisplayFrequency;
			Debug.Log("Thinger: =========== Force Re-adjusting, presumably overwritten =========");
			Debug.Log(" fixedDeltaTime before:\t" + Time.fixedDeltaTime.ToString());
			Debug.Log(" Refresh rate         :\t" + systemDisplayFrequency.ToString());
			Application.targetFrameRate = Mathf.RoundToInt(OVRPlugin.systemDisplayFrequency);
			Time.fixedDeltaTime = num;
			this.UpdateResolutionScale(systemDisplayFrequency < this._defaultRefreshRate);
			Debug.Log(" fixedDeltaTime after :\t" + Time.fixedDeltaTime.ToString());
			Debug.Log(" History size before  :\t" + GTPlayer.Instance.velocityHistorySize.ToString());
			GTPlayer.Instance.velocityHistorySize = Mathf.Max(Mathf.Min(Mathf.FloorToInt(systemDisplayFrequency * 0.083333336f), 10), 6);
			if (GTPlayer.Instance.velocityHistorySize > 9)
			{
				GTPlayer.Instance.velocityHistorySize--;
			}
			Debug.Log("New history size: " + GTPlayer.Instance.velocityHistorySize.ToString());
			Debug.Log(" ============================================");
			GTPlayer.Instance.slideControl = 1f - this.CalcSlideControl(systemDisplayFrequency);
			GTPlayer.Instance.InitializeValues();
		}
	}

	// Token: 0x06003B02 RID: 15106 RVA: 0x00141944 File Offset: 0x0013FB44
	public void DebugDrawTagCasts(Color color)
	{
		float num = this.sphereCastRadius * GTPlayer.Instance.scale;
		this.DrawSphereCast(this.lastLeftHandPositionForTag, this.leftRaycastSweep.normalized, num, Mathf.Max(this.leftRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.leftHeadRaycastSweep.normalized, num, Mathf.Max(this.leftHeadRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.lastRightHandPositionForTag, this.rightRaycastSweep.normalized, num, Mathf.Max(this.rightRaycastSweep.magnitude, num), color);
		this.DrawSphereCast(this.headCollider.transform.position, this.rightHeadRaycastSweep.normalized, num, Mathf.Max(this.rightHeadRaycastSweep.magnitude, num), color);
	}

	// Token: 0x06003B03 RID: 15107 RVA: 0x00141A1F File Offset: 0x0013FC1F
	private void DrawSphereCast(Vector3 start, Vector3 dir, float radius, float dist, Color color)
	{
		DebugUtil.DrawCapsule(start, start + dir * dist, radius, 16, 16, color, true, DebugUtil.Style.Wireframe);
	}

	// Token: 0x06003B04 RID: 15108 RVA: 0x00141A3E File Offset: 0x0013FC3E
	private void RecoverMissingRefs()
	{
		if (!this.offlineVRRig)
		{
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.leftHandSlideSource, "leftHandSlideSource", "./**/Left Arm IK/SlideAudio");
			this.RecoverMissingRefs_Asdf<AudioSource>(ref this.rightHandSlideSource, "rightHandSlideSource", "./**/Right Arm IK/SlideAudio");
		}
	}

	// Token: 0x06003B05 RID: 15109 RVA: 0x00141A7C File Offset: 0x0013FC7C
	private void RecoverMissingRefs_Asdf<T>(ref T objRef, string objFieldName, string recoveryPath) where T : Object
	{
		if (objRef)
		{
			return;
		}
		Transform transform;
		if (!this.offlineVRRig.transform.TryFindByPath(recoveryPath, out transform, false))
		{
			Debug.LogError(string.Concat(new string[] { "`", objFieldName, "` reference missing and could not find by path: \"", recoveryPath, "\"" }), this);
		}
		objRef = transform.GetComponentInChildren<T>();
		if (!objRef)
		{
			Debug.LogError(string.Concat(new string[] { "`", objFieldName, "` reference is missing. Found transform with recover path, but did not find the component. Recover path: \"", recoveryPath, "\"" }), this);
		}
	}

	// Token: 0x06003B06 RID: 15110 RVA: 0x00141B32 File Offset: 0x0013FD32
	public void GuidedRefInitialize()
	{
		GuidedRefHub.RegisterReceiverField<GorillaTagger>(this, "offlineVRRig", ref this.offlineVRRig_gRef);
		GuidedRefHub.ReceiverFullyRegistered<GorillaTagger>(this);
	}

	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x06003B07 RID: 15111 RVA: 0x00141B4B File Offset: 0x0013FD4B
	// (set) Token: 0x06003B08 RID: 15112 RVA: 0x00141B53 File Offset: 0x0013FD53
	int IGuidedRefReceiverMono.GuidedRefsWaitingToResolveCount { get; set; }

	// Token: 0x06003B09 RID: 15113 RVA: 0x00141B5C File Offset: 0x0013FD5C
	bool IGuidedRefReceiverMono.GuidedRefTryResolveReference(GuidedRefTryResolveInfo target)
	{
		if (this.offlineVRRig_gRef.fieldId == target.fieldId && this.offlineVRRig == null)
		{
			this.offlineVRRig = target.targetMono.GuidedRefTargetObject as VRRig;
			return this.offlineVRRig != null;
		}
		return false;
	}

	// Token: 0x06003B0A RID: 15114 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IGuidedRefReceiverMono.OnAllGuidedRefsResolved()
	{
	}

	// Token: 0x06003B0B RID: 15115 RVA: 0x00002C2D File Offset: 0x00000E2D
	void IGuidedRefReceiverMono.OnGuidedRefTargetDestroyed(int fieldId)
	{
	}

	// Token: 0x06003B0E RID: 15118 RVA: 0x000874AD File Offset: 0x000856AD
	Transform IGuidedRefMonoBehaviour.get_transform()
	{
		return base.transform;
	}

	// Token: 0x06003B0F RID: 15119 RVA: 0x00019405 File Offset: 0x00017605
	int IGuidedRefObject.GetInstanceID()
	{
		return base.GetInstanceID();
	}

	// Token: 0x06003B10 RID: 15120 RVA: 0x00141CD4 File Offset: 0x0013FED4
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsOverlap|159_0(bool isLeftHand, float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass159_0 A_5)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig vrrig;
			if (this.colliderOverlaps[i].gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.colliderOverlaps[i], out vrrig) || !(vrrig == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.colliderOverlaps[i], true, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_5.bodyHit = false;
					A_5.leftHandHit = isLeftHand;
					A_5.canTagHit = canTag;
					A_5.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_5.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x06003B11 RID: 15121 RVA: 0x00141D98 File Offset: 0x0013FF98
	[CompilerGenerated]
	private void <LateUpdate>g__TryTaggingAllHitsCapsulecast|159_1(float maxTagDistance, bool canTag = true, bool canStun = false, ref GorillaTagger.<>c__DisplayClass159_0 A_4)
	{
		for (int i = 0; i < this.nonAllocHits; i++)
		{
			VRRig vrrig;
			if (this.nonAllocRaycastHits[i].collider.gameObject.activeSelf && (!this.tagRigDict.TryGetValue(this.nonAllocRaycastHits[i].collider, out vrrig) || !(vrrig == VRRig.LocalRig)))
			{
				if (this.TryToTag(this.nonAllocRaycastHits[i].collider, false, canStun, maxTagDistance, out this.tryPlayer, out this.touchedPlayer))
				{
					this.otherPlayer = this.tryPlayer;
					A_4.bodyHit = true;
					A_4.canTagHit = canTag;
					A_4.canStunHit = canStun;
					return;
				}
				if (this.touchedPlayer != null)
				{
					A_4.otherTouchedPlayer = this.touchedPlayer;
				}
			}
		}
	}

	// Token: 0x04004AD5 RID: 19157
	[OnEnterPlay_SetNull]
	private static GorillaTagger _instance;

	// Token: 0x04004AD6 RID: 19158
	[OnEnterPlay_Set(false)]
	public static bool hasInstance;

	// Token: 0x04004AD7 RID: 19159
	public static float moderationMutedTime = -1f;

	// Token: 0x04004AD8 RID: 19160
	public int SmoothedFramerate;

	// Token: 0x04004AD9 RID: 19161
	private int _prevSmoothedFramerate;

	// Token: 0x04004ADA RID: 19162
	public int FramerateHealth;

	// Token: 0x04004ADB RID: 19163
	private int _prevFramerateHealth;

	// Token: 0x04004ADC RID: 19164
	private float _framerateHealthTimer;

	// Token: 0x04004ADD RID: 19165
	private float[] _framerateTracker = new float[30];

	// Token: 0x04004ADE RID: 19166
	private float _framerateTotal;

	// Token: 0x04004ADF RID: 19167
	private int _framerateIndex;

	// Token: 0x04004AE0 RID: 19168
	private float _framerateTimer;

	// Token: 0x04004AE1 RID: 19169
	private bool _forcePerfRefreshRate;

	// Token: 0x04004AE2 RID: 19170
	private float _perfRefreshRate = 72f;

	// Token: 0x04004AE3 RID: 19171
	private float _defaultRefreshRate = 90f;

	// Token: 0x04004AE4 RID: 19172
	public bool inCosmeticsRoom;

	// Token: 0x04004AE5 RID: 19173
	public SphereCollider headCollider;

	// Token: 0x04004AE6 RID: 19174
	public CapsuleCollider bodyCollider;

	// Token: 0x04004AE7 RID: 19175
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x04004AE8 RID: 19176
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x04004AE9 RID: 19177
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04004AEA RID: 19178
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04004AEB RID: 19179
	private GorillaTagger.StiltTagData[] stiltTagData = new GorillaTagger.StiltTagData[12];

	// Token: 0x04004AEC RID: 19180
	public Transform rightHandTransform;

	// Token: 0x04004AED RID: 19181
	public Transform leftHandTransform;

	// Token: 0x04004AEE RID: 19182
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x04004AEF RID: 19183
	public float handTapVolume = 0.1f;

	// Token: 0x04004AF0 RID: 19184
	public float handTapSpeed;

	// Token: 0x04004AF1 RID: 19185
	public float tapCoolDown = 0.15f;

	// Token: 0x04004AF2 RID: 19186
	public float lastLeftTap;

	// Token: 0x04004AF3 RID: 19187
	public float lastLeftUpTap;

	// Token: 0x04004AF4 RID: 19188
	public float lastRightTap;

	// Token: 0x04004AF5 RID: 19189
	public float lastRightUpTap;

	// Token: 0x04004AF6 RID: 19190
	private bool leftHandWasTouching;

	// Token: 0x04004AF7 RID: 19191
	private bool rightHandWasTouching;

	// Token: 0x04004AF8 RID: 19192
	public float tapHapticDuration = 0.05f;

	// Token: 0x04004AF9 RID: 19193
	public float tapHapticStrength = 0.5f;

	// Token: 0x04004AFA RID: 19194
	public float tagHapticDuration = 0.15f;

	// Token: 0x04004AFB RID: 19195
	public float tagHapticStrength = 1f;

	// Token: 0x04004AFC RID: 19196
	public float taggedHapticDuration = 0.35f;

	// Token: 0x04004AFD RID: 19197
	public float taggedHapticStrength = 1f;

	// Token: 0x04004AFE RID: 19198
	public float taggedTime;

	// Token: 0x04004AFF RID: 19199
	public float tagCooldown;

	// Token: 0x04004B00 RID: 19200
	public float slowCooldown = 3f;

	// Token: 0x04004B01 RID: 19201
	public float maxTagDistance = 2.2f;

	// Token: 0x04004B02 RID: 19202
	public float maxStiltTagDistance = 3.2f;

	// Token: 0x04004B03 RID: 19203
	public VRRig offlineVRRig;

	// Token: 0x04004B04 RID: 19204
	[FormerlySerializedAs("offlineVRRig_guidedRef")]
	public GuidedRefReceiverFieldInfo offlineVRRig_gRef = new GuidedRefReceiverFieldInfo(false);

	// Token: 0x04004B05 RID: 19205
	public GameObject thirdPersonCamera;

	// Token: 0x04004B06 RID: 19206
	public GameObject mainCamera;

	// Token: 0x04004B07 RID: 19207
	public bool testTutorial;

	// Token: 0x04004B08 RID: 19208
	public bool disableTutorial;

	// Token: 0x04004B09 RID: 19209
	private bool _framerateUpdated;

	// Token: 0x04004B0A RID: 19210
	private bool _performanceOn;

	// Token: 0x04004B0B RID: 19211
	public GameObject leftHandTriggerCollider;

	// Token: 0x04004B0C RID: 19212
	public GameObject rightHandTriggerCollider;

	// Token: 0x04004B0D RID: 19213
	public AudioSource leftHandSlideSource;

	// Token: 0x04004B0E RID: 19214
	public AudioSource rightHandSlideSource;

	// Token: 0x04004B0F RID: 19215
	public AudioSource bodySlideSource;

	// Token: 0x04004B10 RID: 19216
	public bool overrideNotInFocus;

	// Token: 0x04004B12 RID: 19218
	private Vector3 leftRaycastSweep;

	// Token: 0x04004B13 RID: 19219
	private Vector3 leftHeadRaycastSweep;

	// Token: 0x04004B14 RID: 19220
	private Vector3 rightRaycastSweep;

	// Token: 0x04004B15 RID: 19221
	private Vector3 rightHeadRaycastSweep;

	// Token: 0x04004B16 RID: 19222
	private Vector3 headRaycastSweep;

	// Token: 0x04004B17 RID: 19223
	private Vector3 bodyRaycastSweep;

	// Token: 0x04004B18 RID: 19224
	private global::UnityEngine.XR.InputDevice rightDevice;

	// Token: 0x04004B19 RID: 19225
	private global::UnityEngine.XR.InputDevice leftDevice;

	// Token: 0x04004B1A RID: 19226
	private bool primaryButtonPressRight;

	// Token: 0x04004B1B RID: 19227
	private bool secondaryButtonPressRight;

	// Token: 0x04004B1C RID: 19228
	private bool primaryButtonPressLeft;

	// Token: 0x04004B1D RID: 19229
	private bool secondaryButtonPressLeft;

	// Token: 0x04004B1E RID: 19230
	private RaycastHit hitInfo;

	// Token: 0x04004B1F RID: 19231
	public NetPlayer otherPlayer;

	// Token: 0x04004B20 RID: 19232
	private NetPlayer tryPlayer;

	// Token: 0x04004B21 RID: 19233
	private NetPlayer touchedPlayer;

	// Token: 0x04004B22 RID: 19234
	private Vector3 topVector;

	// Token: 0x04004B23 RID: 19235
	private Vector3 bottomVector;

	// Token: 0x04004B24 RID: 19236
	private Vector3 bodyVector;

	// Token: 0x04004B25 RID: 19237
	private Vector3 dirFromHitToHand;

	// Token: 0x04004B26 RID: 19238
	private int audioClipIndex;

	// Token: 0x04004B27 RID: 19239
	private global::UnityEngine.XR.InputDevice inputDevice;

	// Token: 0x04004B28 RID: 19240
	private bool wasInOverlay;

	// Token: 0x04004B29 RID: 19241
	private PhotonView tempView;

	// Token: 0x04004B2A RID: 19242
	private NetPlayer tempCreator;

	// Token: 0x04004B2B RID: 19243
	private float cacheHandTapVolume;

	// Token: 0x04004B2C RID: 19244
	public GorillaTagger.StatusEffect currentStatus;

	// Token: 0x04004B2D RID: 19245
	public float statusStartTime;

	// Token: 0x04004B2E RID: 19246
	public float statusEndTime;

	// Token: 0x04004B2F RID: 19247
	private float refreshRate;

	// Token: 0x04004B30 RID: 19248
	private float baseSlideControl;

	// Token: 0x04004B31 RID: 19249
	private int gorillaTagColliderLayerMask;

	// Token: 0x04004B32 RID: 19250
	private RaycastHit[] nonAllocRaycastHits = new RaycastHit[30];

	// Token: 0x04004B33 RID: 19251
	private Collider[] colliderOverlaps = new Collider[30];

	// Token: 0x04004B34 RID: 19252
	private Dictionary<Collider, VRRig> tagRigDict = new Dictionary<Collider, VRRig>();

	// Token: 0x04004B35 RID: 19253
	private int nonAllocHits;

	// Token: 0x04004B37 RID: 19255
	private bool xrSubsystemIsActive;

	// Token: 0x04004B38 RID: 19256
	public string loadedDeviceName = "";

	// Token: 0x04004B39 RID: 19257
	private bool _forceFramerateCheck = true;

	// Token: 0x04004B3A RID: 19258
	[SerializeField]
	private int _framesForHandTrigger = 5;

	// Token: 0x04004B3B RID: 19259
	private GorillaTagger.DebouncedBool _leftHandDown;

	// Token: 0x04004B3C RID: 19260
	private GorillaTagger.DebouncedBool _rightHandDown;

	// Token: 0x04004B3D RID: 19261
	[SerializeField]
	private LayerMask BaseMirrorCameraCullingMask;

	// Token: 0x04004B3E RID: 19262
	public Watchable<int> MirrorCameraCullingMask;

	// Token: 0x04004B3F RID: 19263
	private float[] leftHapticsBuffer;

	// Token: 0x04004B40 RID: 19264
	private float[] rightHapticsBuffer;

	// Token: 0x04004B41 RID: 19265
	private Coroutine leftHapticsRoutine;

	// Token: 0x04004B42 RID: 19266
	private Coroutine rightHapticsRoutine;

	// Token: 0x04004B43 RID: 19267
	private Callback<GameOverlayActivated_t> gameOverlayActivatedCb;

	// Token: 0x04004B44 RID: 19268
	private bool isGameOverlayActive;

	// Token: 0x04004B45 RID: 19269
	private float? tagRadiusOverride;

	// Token: 0x04004B46 RID: 19270
	private int tagRadiusOverrideFrame = -1;

	// Token: 0x04004B49 RID: 19273
	public XRDisplaySubsystem activeXRDisplay;

	// Token: 0x04004B4A RID: 19274
	private static Action onPlayerSpawnedRootCallback;

	// Token: 0x020008CB RID: 2251
	private struct StiltTagData
	{
		// Token: 0x04004B4C RID: 19276
		public bool isLeftHand;

		// Token: 0x04004B4D RID: 19277
		public bool hasCurrentPosition;

		// Token: 0x04004B4E RID: 19278
		public bool hasLastPosition;

		// Token: 0x04004B4F RID: 19279
		public Vector3 currentPositionForTag;

		// Token: 0x04004B50 RID: 19280
		public Vector3 lastPositionForTag;

		// Token: 0x04004B51 RID: 19281
		public bool wasTouching;

		// Token: 0x04004B52 RID: 19282
		public float lastTap;

		// Token: 0x04004B53 RID: 19283
		public float lastUpTap;

		// Token: 0x04004B54 RID: 19284
		public bool canTag;

		// Token: 0x04004B55 RID: 19285
		public bool canStun;
	}

	// Token: 0x020008CC RID: 2252
	public enum StatusEffect
	{
		// Token: 0x04004B57 RID: 19287
		None,
		// Token: 0x04004B58 RID: 19288
		Frozen,
		// Token: 0x04004B59 RID: 19289
		Slowed,
		// Token: 0x04004B5A RID: 19290
		Dead,
		// Token: 0x04004B5B RID: 19291
		Infected,
		// Token: 0x04004B5C RID: 19292
		It
	}

	// Token: 0x020008CD RID: 2253
	private class DebouncedBool
	{
		// Token: 0x17000551 RID: 1361
		// (get) Token: 0x06003B12 RID: 15122 RVA: 0x00141E6B File Offset: 0x0014006B
		// (set) Token: 0x06003B13 RID: 15123 RVA: 0x00141E73 File Offset: 0x00140073
		public bool Value { get; private set; }

		// Token: 0x17000552 RID: 1362
		// (get) Token: 0x06003B14 RID: 15124 RVA: 0x00141E7C File Offset: 0x0014007C
		// (set) Token: 0x06003B15 RID: 15125 RVA: 0x00141E84 File Offset: 0x00140084
		public bool JustEnabled { get; private set; }

		// Token: 0x17000553 RID: 1363
		// (get) Token: 0x06003B16 RID: 15126 RVA: 0x00141E8D File Offset: 0x0014008D
		// (set) Token: 0x06003B17 RID: 15127 RVA: 0x00141E95 File Offset: 0x00140095
		public bool WasStablyEnabled { get; private set; }

		// Token: 0x06003B18 RID: 15128 RVA: 0x00141E9E File Offset: 0x0014009E
		public DebouncedBool(int callsUntilDisable, bool initialValue = false)
		{
			this._callsUntilStable = callsUntilDisable;
			this.Value = initialValue;
			this._lastValue = initialValue;
		}

		// Token: 0x06003B19 RID: 15129 RVA: 0x00141EBC File Offset: 0x001400BC
		public void Set(bool value)
		{
			this._lastValue = this.Value;
			if (!value)
			{
				this.WasStablyEnabled = false;
				this._callsSinceDisable++;
				if (this._callsSinceDisable == this._callsUntilStable)
				{
					this.Value = false;
				}
			}
			else
			{
				this.Value = true;
				this._callsSinceDisable = 0;
				this._callsSinceEnable++;
				if (this._callsSinceEnable >= this._callsUntilStable)
				{
					this.WasStablyEnabled = true;
				}
			}
			this.JustEnabled = this.Value && !this._lastValue;
		}

		// Token: 0x04004B5D RID: 19293
		private readonly int _callsUntilStable;

		// Token: 0x04004B5E RID: 19294
		private int _callsSinceDisable;

		// Token: 0x04004B5F RID: 19295
		private int _callsSinceEnable;

		// Token: 0x04004B60 RID: 19296
		private bool _lastValue;
	}
}
