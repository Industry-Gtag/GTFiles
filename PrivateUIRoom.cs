using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

// Token: 0x020004A4 RID: 1188
public class PrivateUIRoom : MonoBehaviourTick
{
	// Token: 0x17000311 RID: 785
	// (get) Token: 0x06001CC6 RID: 7366 RVA: 0x0009BDDB File Offset: 0x00099FDB
	private bool overlayForcedActive
	{
		get
		{
			return this.overlayForcedSources > (PrivateUIRoom.OverlaySource)0;
		}
	}

	// Token: 0x17000312 RID: 786
	// (get) Token: 0x06001CC7 RID: 7367 RVA: 0x0009B2CA File Offset: 0x000994CA
	private GTPlayer localPlayer
	{
		get
		{
			return GTPlayer.Instance;
		}
	}

	// Token: 0x06001CC8 RID: 7368 RVA: 0x0009BDE8 File Offset: 0x00099FE8
	private void Awake()
	{
		if (PrivateUIRoom.instance == null)
		{
			PrivateUIRoom.instance = this;
			this.occluder.SetActive(false);
			this.leftHandObject.SetActive(false);
			this.rightHandObject.SetActive(false);
			this.ui = new List<Transform>();
			this.uiParents = new Dictionary<Transform, Transform>();
			this.backgroundDirectionPropertyID = Shader.PropertyToID(this.backgroundDirectionPropertyName);
			this._uiRoot = new GameObject("UIRoot").transform;
			this._uiRoot.parent = base.transform;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x0009BE80 File Offset: 0x0009A080
	private new void OnEnable()
	{
		base.OnEnable();
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Listen(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001CCA RID: 7370 RVA: 0x0009BEA3 File Offset: 0x0009A0A3
	private new void OnDisable()
	{
		base.OnDisable();
		SteamVR_Events.System(EVREventType.VREvent_InputFocusChanged).Remove(new UnityAction<VREvent_t>(this.ToggleHands));
	}

	// Token: 0x06001CCB RID: 7371 RVA: 0x0009BEC8 File Offset: 0x0009A0C8
	private static bool FindShoulderCamera()
	{
		if (PrivateUIRoom._shoulderCameraReference.IsNotNull())
		{
			return true;
		}
		if (GorillaTagger.Instance.IsNull())
		{
			return false;
		}
		PrivateUIRoom._shoulderCameraReference = GorillaTagger.Instance.thirdPersonCamera.GetComponentInChildren<Camera>(true);
		if (PrivateUIRoom._shoulderCameraReference == null)
		{
			Debug.LogError("[PRIVATE_UI_ROOMS] Could not find Shoulder Camera");
			return false;
		}
		PrivateUIRoom._virtualCameraReference = PrivateUIRoom._shoulderCameraReference.GetComponentInChildren<CinemachineVirtualCamera>();
		return true;
	}

	// Token: 0x06001CCC RID: 7372 RVA: 0x0009BF30 File Offset: 0x0009A130
	private void ToggleHands(VREvent_t ev)
	{
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] Toggling hands visibility. Event: {0} ({1})", ev.eventType, (EVREventType)ev.eventType));
		Debug.Log(string.Format("[PrivateUIRoom::ToggleHands] _handsShowing: {0}", PrivateUIRoom.instance.rightHandObject.activeSelf));
		if (PrivateUIRoom.instance.rightHandObject.activeSelf)
		{
			this.HideHands();
			return;
		}
		this.ShowHands();
	}

	// Token: 0x06001CCD RID: 7373 RVA: 0x0009BFA3 File Offset: 0x0009A1A3
	private void HideHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu shown, disabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
	}

	// Token: 0x06001CCE RID: 7374 RVA: 0x0009BFCF File Offset: 0x0009A1CF
	private void ShowHands()
	{
		Debug.Log("[PrivateUIRoom::OnSteamMenuShown] Steam menu hidden, re-enabling hands.");
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
	}

	// Token: 0x06001CCF RID: 7375 RVA: 0x0009BFFC File Offset: 0x0009A1FC
	private void ToggleLevelVisibility(bool levelShouldBeVisible)
	{
		Camera component = GorillaTagger.Instance.mainCamera.GetComponent<Camera>();
		if (levelShouldBeVisible)
		{
			component.cullingMask = this.savedCullingLayers;
			if (this.savedCullingLayersShoudlerCam != null)
			{
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.savedCullingLayersShoudlerCam.Value;
				this.savedCullingLayersShoudlerCam = null;
				return;
			}
		}
		else
		{
			this.savedCullingLayers = component.cullingMask;
			component.cullingMask = this.visibleLayers;
			if (PrivateUIRoom.FindShoulderCamera())
			{
				this.savedCullingLayersShoudlerCam = new int?(PrivateUIRoom._shoulderCameraReference.cullingMask);
				PrivateUIRoom._shoulderCameraReference.cullingMask = this.visibleLayers;
				PrivateUIRoom._virtualCameraReference.enabled = false;
			}
		}
	}

	// Token: 0x06001CD0 RID: 7376 RVA: 0x0009C0B0 File Offset: 0x0009A2B0
	private static void StopOverlay()
	{
		PrivateUIRoom.instance.localPlayer.inOverlay = false;
		PrivateUIRoom.instance.inOverlay = false;
		PrivateUIRoom.instance.localPlayer.disableMovement = false;
		PrivateUIRoom.instance.localPlayer.InReportMenu = false;
		PrivateUIRoom.instance.ToggleLevelVisibility(true);
		PrivateUIRoom.instance.occluder.SetActive(false);
		PrivateUIRoom.instance.leftHandObject.SetActive(false);
		PrivateUIRoom.instance.rightHandObject.SetActive(false);
		PrivateUIRoom._virtualCameraReference.enabled = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(false);
		Debug.Log("[PrivateUIRoom::StopOverlay] Re-enabling Game Audio");
	}

	// Token: 0x06001CD1 RID: 7377 RVA: 0x0009C154 File Offset: 0x0009A354
	private void GetIdealScreenPositionRotation(out Vector3 position, out Quaternion rotation, out Vector3 scale)
	{
		GameObject mainCamera = GorillaTagger.Instance.mainCamera;
		rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
		scale = this.localPlayer.turnParent.transform.localScale;
		position = mainCamera.transform.position + rotation * Vector3.zero * scale.x;
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x0009C1E0 File Offset: 0x0009A3E0
	private static void AssignShoulderCameraToCanvases(Transform focus)
	{
		Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] setting up canvases with shoulder camera.");
		if (!PrivateUIRoom.FindShoulderCamera())
		{
			return;
		}
		Canvas componentInChildren = focus.GetComponentInChildren<Canvas>(true);
		if (componentInChildren != null)
		{
			componentInChildren.worldCamera = PrivateUIRoom._shoulderCameraReference;
			Debug.Log("[KID::PrivateUIRoom::CanvasCameraAssigner] Assigned shoulder camera to Canvas: " + componentInChildren.name);
			return;
		}
		Debug.LogError("[KID::PrivateUIRoom::CanvasCameraAssigner] No Canvas component found on this GameObject.");
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x0009C23C File Offset: 0x0009A43C
	public static void AddUI(Transform focus)
	{
		if (PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		PrivateUIRoom.instance._text.text = "";
		PrivateUIRoom.AssignShoulderCameraToCanvases(focus);
		PrivateUIRoom.instance.uiParents.Add(focus, focus.parent);
		focus.gameObject.SetActive(false);
		focus.parent = PrivateUIRoom.instance._uiRoot;
		focus.localPosition = Vector3.zero;
		focus.localRotation = Quaternion.identity;
		PrivateUIRoom.instance.ui.Add(focus);
		if (PrivateUIRoom.instance.ui.Count == 1 && PrivateUIRoom.instance.focusTransform == null)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			if (!PrivateUIRoom.instance.inOverlay)
			{
				PrivateUIRoom.StartOverlay();
			}
		}
		PrivateUIRoom.instance.UpdateUIPositionAndRotation();
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x0009C33C File Offset: 0x0009A53C
	public static void RemoveUI(Transform focus)
	{
		if (!PrivateUIRoom.instance.ui.Contains(focus))
		{
			return;
		}
		focus.gameObject.SetActive(false);
		PrivateUIRoom.instance.ui.Remove(focus);
		if (PrivateUIRoom.instance.focusTransform == focus)
		{
			PrivateUIRoom.instance.focusTransform = null;
		}
		if (PrivateUIRoom.instance.uiParents[focus] != null)
		{
			focus.parent = PrivateUIRoom.instance.uiParents[focus];
			PrivateUIRoom.instance.uiParents.Remove(focus);
		}
		else
		{
			Object.Destroy(focus.gameObject);
		}
		if (PrivateUIRoom.instance.ui.Count > 0)
		{
			PrivateUIRoom.instance.focusTransform = PrivateUIRoom.instance.ui[0];
			PrivateUIRoom.instance.focusTransform.gameObject.SetActive(true);
			return;
		}
		if (!PrivateUIRoom.instance.overlayForcedActive)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x0009C438 File Offset: 0x0009A638
	public static void ForceStartOverlay(PrivateUIRoom.OverlaySource source, string text = "")
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedSources |= source;
		if (PrivateUIRoom.instance.inOverlay)
		{
			return;
		}
		PrivateUIRoom.instance._text.text = text;
		PrivateUIRoom.StartOverlay();
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x0009C488 File Offset: 0x0009A688
	public static void StopForcedOverlay(PrivateUIRoom.OverlaySource source)
	{
		if (PrivateUIRoom.instance == null)
		{
			return;
		}
		PrivateUIRoom.instance.overlayForcedSources &= ~source;
		if (PrivateUIRoom.instance.overlayForcedActive)
		{
			return;
		}
		if (PrivateUIRoom.instance.ui.Count == 0 && PrivateUIRoom.instance.inOverlay)
		{
			PrivateUIRoom.StopOverlay();
		}
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x0009C4E8 File Offset: 0x0009A6E8
	private static void StartOverlay()
	{
		Vector3 vector;
		Quaternion quaternion;
		Vector3 vector2;
		PrivateUIRoom.instance.GetIdealScreenPositionRotation(out vector, out quaternion, out vector2);
		PrivateUIRoom.instance.leftHandObject.transform.localScale = vector2;
		PrivateUIRoom.instance.rightHandObject.transform.localScale = vector2;
		PrivateUIRoom.instance.occluder.transform.localScale = vector2;
		PrivateUIRoom.instance.localPlayer.InReportMenu = true;
		PrivateUIRoom.instance.localPlayer.disableMovement = true;
		PrivateUIRoom.instance.occluder.SetActive(true);
		PrivateUIRoom.instance.rightHandObject.SetActive(true);
		PrivateUIRoom.instance.leftHandObject.SetActive(true);
		PrivateUIRoom.instance.ToggleLevelVisibility(false);
		PrivateUIRoom.instance.localPlayer.inOverlay = true;
		PrivateUIRoom.instance.inOverlay = true;
		KIDAudioManager.Instance.SetKIDUIAudioActive(true);
		Debug.Log("[PrivateUIRoom::StartOverlay] Muting Game Audio");
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0009C5D0 File Offset: 0x0009A7D0
	public override void Tick()
	{
		if (!this.localPlayer.InReportMenu)
		{
			return;
		}
		this.occluder.transform.position = GorillaTagger.Instance.mainCamera.transform.position;
		Transform controllerTransform = this.localPlayer.GetControllerTransform(true);
		Transform controllerTransform2 = this.localPlayer.GetControllerTransform(false);
		this.rightHandObject.transform.SetPositionAndRotation(controllerTransform2.position, controllerTransform2.rotation);
		this.leftHandObject.transform.SetPositionAndRotation(controllerTransform.position, controllerTransform.rotation);
		if (this.ShouldUpdateRotation())
		{
			this.UpdateUIPositionAndRotation();
			return;
		}
		if (this.ShouldUpdatePosition())
		{
			this.UpdateUIPosition();
		}
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0009C680 File Offset: 0x0009A880
	private bool ShouldUpdateRotation()
	{
		float magnitude = (GorillaTagger.Instance.mainCamera.transform.position - this.lastStablePosition).X_Z().magnitude;
		Quaternion quaternion = Quaternion.Euler(0f, GorillaTagger.Instance.mainCamera.transform.rotation.eulerAngles.y, 0f);
		float num = Quaternion.Angle(this.lastStableRotation, quaternion);
		return magnitude > this.lateralPlay || num >= this.rotationalPlay;
	}

	// Token: 0x06001CDA RID: 7386 RVA: 0x0009C70D File Offset: 0x0009A90D
	private bool ShouldUpdatePosition()
	{
		return Mathf.Abs(GorillaTagger.Instance.mainCamera.transform.position.y - this.lastStablePosition.y) > this.verticalPlay;
	}

	// Token: 0x06001CDB RID: 7387 RVA: 0x0009C744 File Offset: 0x0009A944
	private void UpdateUIPositionAndRotation()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this.lastStableRotation = transform.rotation;
		Vector3 normalized = transform.forward.X_Z().normalized;
		this._uiRoot.SetPositionAndRotation(this.lastStablePosition + normalized * 0.02f, Quaternion.LookRotation(normalized));
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		PrivateUIRoom._shoulderCameraReference.transform.rotation = this._uiRoot.rotation;
		this.backgroundRenderer.material.SetVector(this.backgroundDirectionPropertyID, this.backgroundRenderer.transform.InverseTransformDirection(normalized));
		this.SetTextPositionAndRotation(transform);
	}

	// Token: 0x06001CDC RID: 7388 RVA: 0x0009C81C File Offset: 0x0009AA1C
	private void SetTextPositionAndRotation(Transform pov)
	{
		if (!this._text.enabled || string.IsNullOrEmpty(this._text.text))
		{
			return;
		}
		this._text.transform.position = pov.position + this._textDistance * (pov.rotation * Vector3.forward);
		this._text.transform.rotation = Quaternion.LookRotation(pov.rotation * Vector3.forward, Vector3.up);
	}

	// Token: 0x06001CDD RID: 7389 RVA: 0x0009C8AC File Offset: 0x0009AAAC
	private void UpdateUIPosition()
	{
		Transform transform = GorillaTagger.Instance.mainCamera.transform;
		this.lastStablePosition = transform.position;
		this._uiRoot.position = this.lastStablePosition + this.lastStableRotation * new Vector3(0f, 0f, 0.02f);
		PrivateUIRoom._shoulderCameraReference.transform.position = this._uiRoot.position;
		this.SetTextPositionAndRotation(transform);
	}

	// Token: 0x06001CDE RID: 7390 RVA: 0x0009C92B File Offset: 0x0009AB2B
	public static bool GetInOverlay()
	{
		return !(PrivateUIRoom.instance == null) && PrivateUIRoom.instance.inOverlay;
	}

	// Token: 0x040026EF RID: 9967
	[SerializeField]
	private TextMeshPro _text;

	// Token: 0x040026F0 RID: 9968
	[SerializeField]
	private float _textDistance = 4f;

	// Token: 0x040026F1 RID: 9969
	[SerializeField]
	private GameObject occluder;

	// Token: 0x040026F2 RID: 9970
	[SerializeField]
	private LayerMask visibleLayers;

	// Token: 0x040026F3 RID: 9971
	[SerializeField]
	private GameObject leftHandObject;

	// Token: 0x040026F4 RID: 9972
	[SerializeField]
	private GameObject rightHandObject;

	// Token: 0x040026F5 RID: 9973
	[SerializeField]
	private MeshRenderer backgroundRenderer;

	// Token: 0x040026F6 RID: 9974
	[SerializeField]
	private string backgroundDirectionPropertyName = "_SpotDirection";

	// Token: 0x040026F7 RID: 9975
	private int backgroundDirectionPropertyID;

	// Token: 0x040026F8 RID: 9976
	private int savedCullingLayers;

	// Token: 0x040026F9 RID: 9977
	private Transform _uiRoot;

	// Token: 0x040026FA RID: 9978
	private Transform focusTransform;

	// Token: 0x040026FB RID: 9979
	private List<Transform> ui;

	// Token: 0x040026FC RID: 9980
	private Dictionary<Transform, Transform> uiParents;

	// Token: 0x040026FD RID: 9981
	private float _initialAudioVolume;

	// Token: 0x040026FE RID: 9982
	private bool inOverlay;

	// Token: 0x040026FF RID: 9983
	private PrivateUIRoom.OverlaySource overlayForcedSources;

	// Token: 0x04002700 RID: 9984
	private static PrivateUIRoom instance;

	// Token: 0x04002701 RID: 9985
	private Vector3 lastStablePosition;

	// Token: 0x04002702 RID: 9986
	private Quaternion lastStableRotation;

	// Token: 0x04002703 RID: 9987
	[SerializeField]
	private float verticalPlay = 0.1f;

	// Token: 0x04002704 RID: 9988
	[SerializeField]
	private float lateralPlay = 0.5f;

	// Token: 0x04002705 RID: 9989
	[SerializeField]
	private float rotationalPlay = 45f;

	// Token: 0x04002706 RID: 9990
	private int? savedCullingLayersShoudlerCam;

	// Token: 0x04002707 RID: 9991
	private static Camera _shoulderCameraReference;

	// Token: 0x04002708 RID: 9992
	private static CinemachineVirtualCamera _virtualCameraReference;

	// Token: 0x020004A5 RID: 1189
	[Flags]
	public enum OverlaySource
	{
		// Token: 0x0400270A RID: 9994
		KID = 1,
		// Token: 0x0400270B RID: 9995
		ModIO = 2,
		// Token: 0x0400270C RID: 9996
		CustomMap = 4,
		// Token: 0x0400270D RID: 9997
		AlarmClock = 8,
		// Token: 0x0400270E RID: 9998
		VStumpConsent = 16
	}
}
