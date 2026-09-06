using System;
using System.Runtime.CompilerServices;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x0200088F RID: 2191
public class GorillaIOBT : MonoBehaviour
{
	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x06003913 RID: 14611 RVA: 0x001378AE File Offset: 0x00135AAE
	// (set) Token: 0x06003914 RID: 14612 RVA: 0x001378B6 File Offset: 0x00135AB6
	public OVRInput.Controller leftActiveController { get; private set; }

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x06003915 RID: 14613 RVA: 0x001378BF File Offset: 0x00135ABF
	// (set) Token: 0x06003916 RID: 14614 RVA: 0x001378C7 File Offset: 0x00135AC7
	public OVRInput.Controller rightActiveController { get; private set; }

	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06003917 RID: 14615 RVA: 0x001378D0 File Offset: 0x00135AD0
	public bool IsHandTracking
	{
		get
		{
			return this.leftActiveController == OVRInput.Controller.LHand || this.rightActiveController == OVRInput.Controller.RHand;
		}
	}

	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x06003918 RID: 14616 RVA: 0x001378E8 File Offset: 0x00135AE8
	// (set) Token: 0x06003919 RID: 14617 RVA: 0x001378F0 File Offset: 0x00135AF0
	public HandTrackingFingerCurl leftHandCurl { get; private set; }

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x0600391A RID: 14618 RVA: 0x001378F9 File Offset: 0x00135AF9
	// (set) Token: 0x0600391B RID: 14619 RVA: 0x00137901 File Offset: 0x00135B01
	public HandTrackingFingerCurl rightHandCurl { get; private set; }

	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x0600391C RID: 14620 RVA: 0x0013790A File Offset: 0x00135B0A
	// (set) Token: 0x0600391D RID: 14621 RVA: 0x00137912 File Offset: 0x00135B12
	public Transform trackingSpace { get; private set; }

	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x0600391E RID: 14622 RVA: 0x0013791B File Offset: 0x00135B1B
	// (set) Token: 0x0600391F RID: 14623 RVA: 0x00137923 File Offset: 0x00135B23
	public Transform centerEyeAnchor { get; private set; }

	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x06003920 RID: 14624 RVA: 0x0013792C File Offset: 0x00135B2C
	// (set) Token: 0x06003921 RID: 14625 RVA: 0x00137934 File Offset: 0x00135B34
	public Transform leftHandAnchor { get; private set; }

	// Token: 0x1700051B RID: 1307
	// (get) Token: 0x06003922 RID: 14626 RVA: 0x0013793D File Offset: 0x00135B3D
	// (set) Token: 0x06003923 RID: 14627 RVA: 0x00137945 File Offset: 0x00135B45
	public Transform rightHandAnchor { get; private set; }

	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x06003924 RID: 14628 RVA: 0x0013794E File Offset: 0x00135B4E
	// (set) Token: 0x06003925 RID: 14629 RVA: 0x00137956 File Offset: 0x00135B56
	public Transform leftControllerAnchor { get; private set; }

	// Token: 0x1700051D RID: 1309
	// (get) Token: 0x06003926 RID: 14630 RVA: 0x0013795F File Offset: 0x00135B5F
	// (set) Token: 0x06003927 RID: 14631 RVA: 0x00137967 File Offset: 0x00135B67
	public Transform rightControllerAnchor { get; private set; }

	// Token: 0x14000065 RID: 101
	// (add) Token: 0x06003928 RID: 14632 RVA: 0x00137970 File Offset: 0x00135B70
	// (remove) Token: 0x06003929 RID: 14633 RVA: 0x001379A8 File Offset: 0x00135BA8
	public event Action<GorillaIOBT> UpdatedAnchors;

	// Token: 0x14000066 RID: 102
	// (add) Token: 0x0600392A RID: 14634 RVA: 0x001379E0 File Offset: 0x00135BE0
	// (remove) Token: 0x0600392B RID: 14635 RVA: 0x00137A18 File Offset: 0x00135C18
	public event Action<Transform> TrackingSpaceChanged;

	// Token: 0x0600392C RID: 14636 RVA: 0x00137A4D File Offset: 0x00135C4D
	protected virtual void Awake()
	{
		this._skipUpdate = true;
		this.EnsureGameObjectIntegrity();
		this.upperBodySkeleton = base.GetComponent<OVRSkeleton>();
	}

	// Token: 0x0600392D RID: 14637 RVA: 0x00137A68 File Offset: 0x00135C68
	protected virtual void Start()
	{
		this.UpdateAnchors();
		Application.onBeforeRender += this.OnBeforeRenderCallback;
	}

	// Token: 0x0600392E RID: 14638 RVA: 0x00137A82 File Offset: 0x00135C82
	protected virtual void Update()
	{
		this._skipUpdate = false;
		this.UpdateAnchors();
	}

	// Token: 0x0600392F RID: 14639 RVA: 0x00137A91 File Offset: 0x00135C91
	protected virtual void OnDestroy()
	{
		Application.onBeforeRender -= this.OnBeforeRenderCallback;
	}

	// Token: 0x06003930 RID: 14640 RVA: 0x00137AA8 File Offset: 0x00135CA8
	protected virtual void UpdateAnchors()
	{
		if (!OVRManager.OVRManagerinitialized)
		{
			return;
		}
		this.EnsureGameObjectIntegrity();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this._skipUpdate)
		{
			this.centerEyeAnchor.FromOVRPose(OVRPose.identity, true);
			return;
		}
		bool monoscopic = OVRManager.instance.monoscopic;
		OVRNodeStateProperties.IsHmdPresent();
		OVRManager.tracker.GetPose(0);
		Quaternion.Euler(-OVRManager.instance.headPoseRelativeOffsetRotation.x, -OVRManager.instance.headPoseRelativeOffsetRotation.y, OVRManager.instance.headPoseRelativeOffsetRotation.z);
		OVRInput.Controller leftActiveController = this.leftActiveController;
		OVRInput.Controller rightActiveController = this.rightActiveController;
		this.leftActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.LeftHanded);
		this.rightActiveController = OVRInput.GetActiveControllerForHand(OVRInput.Handedness.RightHanded);
		if (this.leftActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LHand))
			{
				this.leftActiveController = OVRInput.Controller.LHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch))
			{
				this.leftActiveController = OVRInput.Controller.LTouch;
			}
		}
		if (this.rightActiveController == OVRInput.Controller.None)
		{
			if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RHand))
			{
				this.rightActiveController = OVRInput.Controller.RHand;
			}
			else if (OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch))
			{
				this.rightActiveController = OVRInput.Controller.RTouch;
			}
		}
		if (leftActiveController == OVRInput.Controller.None && this.leftActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (leftActiveController != OVRInput.Controller.None && this.leftActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (rightActiveController == OVRInput.Controller.None && this.rightActiveController != OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingGainedClip);
		}
		else if (rightActiveController != OVRInput.Controller.None && this.rightActiveController == OVRInput.Controller.None)
		{
			this.trackingChangedAudioSource.PlayOneShot(this.trackingLostClip);
		}
		if (this.leftActiveController == OVRInput.Controller.LHand)
		{
			this.leftHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.leftActiveController);
			this.leftHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.leftActiveController);
			this.leftHandAnchor.localRotation = this.leftHandAnchor.localRotation * Quaternion.Euler(0f, 90f, -90f);
		}
		if (this.rightActiveController == OVRInput.Controller.RHand)
		{
			this.rightHandAnchor.localPosition = OVRInput.GetLocalControllerPosition(this.rightActiveController);
			this.rightHandAnchor.localRotation = OVRInput.GetLocalControllerRotation(this.rightActiveController);
			this.rightHandAnchor.localRotation = this.rightHandAnchor.localRotation * Quaternion.Euler(0f, -90f, 90f);
		}
		OVRPose ovrpose = OVRPose.identity;
		OVRPose ovrpose2 = OVRPose.identity;
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.OpenVR)
		{
			ovrpose = OVRManager.GetOpenVRControllerOffset(XRNode.LeftHand);
			ovrpose2 = OVRManager.GetOpenVRControllerOffset(XRNode.RightHand);
			OVRManager.SetOpenVRLocalPose(this.trackingSpace.InverseTransformPoint(this.leftControllerAnchor.position), this.trackingSpace.InverseTransformPoint(this.rightControllerAnchor.position), Quaternion.Inverse(this.trackingSpace.rotation) * this.leftControllerAnchor.rotation, Quaternion.Inverse(this.trackingSpace.rotation) * this.rightControllerAnchor.rotation);
		}
		this.rightControllerAnchor.localPosition = ovrpose2.position;
		this.rightControllerAnchor.localRotation = ovrpose2.orientation;
		this.leftControllerAnchor.localPosition = ovrpose.position;
		this.leftControllerAnchor.localRotation = ovrpose.orientation;
		GTPlayer.Instance.SetHandOffsets(true, new Vector3(0.03f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		GTPlayer.Instance.SetHandOffsets(false, new Vector3(-0.01f, -0.16f, 0f), Quaternion.Euler(89f, 6f, 11f));
		this.RaiseUpdatedAnchorsEvent();
		this.CheckForTrackingSpaceChangesAndRaiseEvent();
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x00137E38 File Offset: 0x00136038
	protected virtual void OnBeforeRenderCallback()
	{
		if (OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus && OVRManager.instance.LateControllerUpdate)
		{
			this.UpdateAnchors();
		}
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x00137E54 File Offset: 0x00136054
	protected virtual void CheckForTrackingSpaceChangesAndRaiseEvent()
	{
		if (this.trackingSpace == null)
		{
			return;
		}
		Matrix4x4 localToWorldMatrix = this.trackingSpace.localToWorldMatrix;
		bool flag = this.TrackingSpaceChanged != null && !this._previousTrackingSpaceTransform.Equals(localToWorldMatrix);
		this._previousTrackingSpaceTransform = localToWorldMatrix;
		if (flag)
		{
			this.TrackingSpaceChanged(this.trackingSpace);
		}
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x00137EB0 File Offset: 0x001360B0
	protected virtual void RaiseUpdatedAnchorsEvent()
	{
		if (this.UpdatedAnchors != null)
		{
			this.UpdatedAnchors(this);
		}
	}

	// Token: 0x06003934 RID: 14644 RVA: 0x00137EC8 File Offset: 0x001360C8
	public virtual void EnsureGameObjectIntegrity()
	{
		if (OVRManager.instance != null)
		{
			bool monoscopic = OVRManager.instance.monoscopic;
		}
		if (this.trackingSpace == null)
		{
			this.trackingSpace = this.ConfigureAnchor(null, this.trackingSpaceName);
			this._previousTrackingSpaceTransform = this.trackingSpace.localToWorldMatrix;
		}
		if (this.centerEyeAnchor == null)
		{
			this.centerEyeAnchor = this.ConfigureAnchor(this.trackingSpace, this.centerEyeAnchorName);
		}
		if (this.leftHandAnchor == null)
		{
			this.leftHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.leftHandAnchorName);
		}
		if (this.rightHandAnchor == null)
		{
			this.rightHandAnchor = this.ConfigureAnchor(this.trackingSpace, this.rightHandAnchorName);
		}
		if (this.leftControllerAnchor == null)
		{
			this.leftControllerAnchor = this.ConfigureAnchor(this.leftHandAnchor, this.leftControllerAnchorName);
		}
		if (this.rightControllerAnchor == null)
		{
			this.rightControllerAnchor = this.ConfigureAnchor(this.rightHandAnchor, this.rightControllerAnchorName);
		}
		if (this.leftHandCurl == null)
		{
			Transform leftHandAnchor = this.leftHandAnchor;
			this.leftHandCurl = ((leftHandAnchor != null) ? leftHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
		if (this.rightHandCurl == null)
		{
			Transform rightHandAnchor = this.rightHandAnchor;
			this.rightHandCurl = ((rightHandAnchor != null) ? rightHandAnchor.GetComponent<HandTrackingFingerCurl>() : null);
		}
	}

	// Token: 0x06003935 RID: 14645 RVA: 0x0013802C File Offset: 0x0013622C
	protected Transform ConfigureAnchor(Transform root, string name)
	{
		Transform transform = ((root != null) ? root.Find(name) : null);
		if (transform == null)
		{
			transform = base.transform.Find(name);
		}
		if (transform == null)
		{
			transform = new GameObject(name).transform;
		}
		transform.name = name;
		transform.parent = ((root != null) ? root : base.transform);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		return transform;
	}

	// Token: 0x06003936 RID: 14646 RVA: 0x001380BC File Offset: 0x001362BC
	public virtual Matrix4x4 ComputeTrackReferenceMatrix()
	{
		if (this.centerEyeAnchor == null)
		{
			Debug.LogError("centerEyeAnchor is required");
			return Matrix4x4.identity;
		}
		OVRPose identity = OVRPose.identity;
		Vector3 vector;
		if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.Position, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out vector))
		{
			identity.position = vector;
		}
		Quaternion quaternion;
		if (OVRNodeStateProperties.GetNodeStatePropertyQuaternion(XRNode.Head, NodeStatePropertyType.Orientation, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out quaternion))
		{
			identity.orientation = quaternion;
		}
		OVRPose ovrpose = identity.Inverse();
		Matrix4x4 matrix4x = Matrix4x4.TRS(ovrpose.position, ovrpose.orientation, Vector3.one);
		return this.centerEyeAnchor.localToWorldMatrix * matrix4x;
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x0013814C File Offset: 0x0013634C
	protected void CheckForAnchorsInParent()
	{
		Transform transform = base.transform.parent;
		while (transform)
		{
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSpatialAnchor>(transform);
			this.<CheckForAnchorsInParent>g__Check|71_0<OVRSceneAnchor>(transform);
			transform = transform.parent;
		}
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x001381DC File Offset: 0x001363DC
	[CompilerGenerated]
	private void <CheckForAnchorsInParent>g__Check|71_0<T>(Transform node) where T : MonoBehaviour
	{
		T component = node.GetComponent<T>();
		if (component && component.enabled)
		{
			component.enabled = false;
			Debug.LogError(string.Concat(new string[]
			{
				"The ",
				typeof(T).Name,
				" '",
				component.name,
				"' is a parent of the GorillaIOBT '",
				base.name,
				"', which is not allowed. An ",
				typeof(T).Name,
				" may not be the parent of an GorillaIOBT because the GorillaIOBT defines the tracking space for the anchor, and its transform is relative to the GorillaIOBT."
			}));
		}
	}

	// Token: 0x0400493A RID: 18746
	private OVRSkeleton upperBodySkeleton;

	// Token: 0x04004943 RID: 18755
	public AudioSource trackingChangedAudioSource;

	// Token: 0x04004944 RID: 18756
	public AudioClip trackingGainedClip;

	// Token: 0x04004945 RID: 18757
	public AudioClip trackingLostClip;

	// Token: 0x04004946 RID: 18758
	protected bool _skipUpdate;

	// Token: 0x04004947 RID: 18759
	protected readonly string trackingSpaceName = "TurnParent";

	// Token: 0x04004948 RID: 18760
	protected readonly string centerEyeAnchorName = "Main Camera";

	// Token: 0x04004949 RID: 18761
	protected readonly string leftHandAnchorName = "LeftHand Controller";

	// Token: 0x0400494A RID: 18762
	protected readonly string rightHandAnchorName = "RightHand Controller";

	// Token: 0x0400494B RID: 18763
	protected readonly string leftControllerAnchorName = "LeftControllerAnchor";

	// Token: 0x0400494C RID: 18764
	protected readonly string rightControllerAnchorName = "RightControllerAnchor";

	// Token: 0x0400494D RID: 18765
	protected Matrix4x4 _previousTrackingSpaceTransform;
}
