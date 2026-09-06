using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaTag;
using Liv.Lck.GorillaTag;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200040E RID: 1038
public class LCKSocialCameraFollower : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001893 RID: 6291 RVA: 0x0008B8C9 File Offset: 0x00089AC9
	public Transform ScaleTransform
	{
		get
		{
			return this._scaleTransform;
		}
	}

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x06001894 RID: 6292 RVA: 0x0008B8D1 File Offset: 0x00089AD1
	public GameObject CameraVisualsRoot
	{
		get
		{
			return this._cameraVisualsRoot;
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x06001895 RID: 6293 RVA: 0x0008B8D9 File Offset: 0x00089AD9
	public List<GameObject> VisualObjects
	{
		get
		{
			return this._visualObjects;
		}
	}

	// Token: 0x06001896 RID: 6294 RVA: 0x0008B8E4 File Offset: 0x00089AE4
	private void Awake()
	{
		this.m_gtCameraVisuals = this._cameraVisualsRoot.GetComponent<IGtCameraVisuals>();
		if (this.m_rigContainer.Rig.isOfflineVRRig)
		{
			base.gameObject.SetActive(false);
			return;
		}
		ListProcessor<Action<RigContainer>> disableEvent = this.m_rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.PreRigDisable);
		disableEvent.Add(in action);
		ListProcessor<Action<RigContainer>> enableEvent = this.m_rigContainer.RigEvents.enableEvent;
		action = new Action<RigContainer>(this.PostRigEnable);
		enableEvent.Add(in action);
	}

	// Token: 0x06001897 RID: 6295 RVA: 0x0008B969 File Offset: 0x00089B69
	private void Start()
	{
		if (!this.isParentedToRig)
		{
			base.transform.parent = null;
		}
	}

	// Token: 0x06001898 RID: 6296 RVA: 0x0008B980 File Offset: 0x00089B80
	public void SetParentToRig()
	{
		this.isParentedToRig = true;
		base.transform.parent = this.m_rigContainer.transform;
		base.transform.localPosition = new Vector3(0f, -0.2f, 0.132f);
		base.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x06001899 RID: 6297 RVA: 0x0008B9D9 File Offset: 0x00089BD9
	public void SetParentNull()
	{
		this.isParentedToRig = false;
		base.transform.parent = null;
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x0008B9EE File Offset: 0x00089BEE
	private void PostRigEnable(RigContainer _)
	{
		base.gameObject.SetActive(true);
		this.m_gtCameraVisuals.SetNetworkedVisualsActive(false);
		this.m_gtCameraVisuals.SetRecordingState(false);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x00044B04 File Offset: 0x00042D04
	private void PreRigDisable(RigContainer _)
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x0008BA14 File Offset: 0x00089C14
	public void SetNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController.IsNotNull() && this.m_networkController != networkController)
		{
			this.m_networkController.TurnOff();
		}
		this.m_networkController = networkController;
		this.m_transformToFollow = this.m_networkController.transform;
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x0008BA65 File Offset: 0x00089C65
	public void RemoveNetworkController(LckSocialCamera networkController)
	{
		if (this.m_networkController != networkController)
		{
			return;
		}
		this.m_transformToFollow = null;
		this.m_networkController = null;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x17000272 RID: 626
	// (get) Token: 0x0600189E RID: 6302 RVA: 0x0008BA8A File Offset: 0x00089C8A
	// (set) Token: 0x0600189F RID: 6303 RVA: 0x0008BA92 File Offset: 0x00089C92
	bool ITickSystemTick.TickRunning { get; set; }

	// Token: 0x060018A0 RID: 6304 RVA: 0x0008BA9C File Offset: 0x00089C9C
	void ITickSystemTick.Tick()
	{
		if (this.isParentedToRig || this.m_transformToFollow == null)
		{
			return;
		}
		base.transform.position = this.m_transformToFollow.position;
		base.transform.root.rotation = this.m_transformToFollow.rotation;
	}

	// Token: 0x040023C1 RID: 9153
	[SerializeField]
	private Transform _scaleTransform;

	// Token: 0x040023C2 RID: 9154
	[FormerlySerializedAs("_coconutCamera")]
	[SerializeField]
	private GameObject _cameraVisualsRoot;

	// Token: 0x040023C3 RID: 9155
	[SerializeField]
	private List<GameObject> _visualObjects;

	// Token: 0x040023C4 RID: 9156
	[SerializeField]
	private RigContainer m_rigContainer;

	// Token: 0x040023C5 RID: 9157
	private Transform m_transformToFollow;

	// Token: 0x040023C6 RID: 9158
	private LckSocialCamera m_networkController;

	// Token: 0x040023C7 RID: 9159
	private IGtCameraVisuals m_gtCameraVisuals;

	// Token: 0x040023C8 RID: 9160
	private bool isParentedToRig;
}
