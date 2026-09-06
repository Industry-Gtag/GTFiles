using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200084A RID: 2122
internal class GorillaSerializerScene : GorillaSerializer, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x06003691 RID: 13969 RVA: 0x0012D775 File Offset: 0x0012B975
	internal bool HasAuthority
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06003692 RID: 13970 RVA: 0x0012D784 File Offset: 0x0012B984
	protected virtual void Start()
	{
		if (!this.targetComponent.IsNull())
		{
			IGorillaSerializeableScene gorillaSerializeableScene = this.targetComponent as IGorillaSerializeableScene;
			if (gorillaSerializeableScene != null)
			{
				gorillaSerializeableScene.OnSceneLinking(this);
				this.serializeTarget = gorillaSerializeableScene;
				this.sceneSerializeTarget = gorillaSerializeableScene;
				this.successfullInstantiate = true;
				this.photonView.AddCallbackTarget(this);
				return;
			}
		}
		Debug.LogError("GorillaSerializerscene: missing target component or invalid target", base.gameObject);
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003693 RID: 13971 RVA: 0x0012D7F2 File Offset: 0x0012B9F2
	private void OnEnable()
	{
		if (!this.successfullInstantiate)
		{
			return;
		}
		if (!this.validDisable)
		{
			this.validDisable = true;
			return;
		}
		this.OnValidEnable();
	}

	// Token: 0x06003694 RID: 13972 RVA: 0x0012D813 File Offset: 0x0012BA13
	protected virtual void OnValidEnable()
	{
		this.sceneSerializeTarget.OnNetworkObjectEnable();
	}

	// Token: 0x06003695 RID: 13973 RVA: 0x0012D820 File Offset: 0x0012BA20
	private void OnDisable()
	{
		if (!this.successfullInstantiate || !this.validDisable)
		{
			return;
		}
		this.OnValidDisable();
	}

	// Token: 0x06003696 RID: 13974 RVA: 0x0012D839 File Offset: 0x0012BA39
	protected virtual void OnValidDisable()
	{
		this.sceneSerializeTarget.OnNetworkObjectDisable();
	}

	// Token: 0x06003697 RID: 13975 RVA: 0x0012D848 File Offset: 0x0012BA48
	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		MonkeAgent.instance.SendReport("bad net obj creation", info.Sender.UserId, info.Sender.NickName);
		if (info.photonView.IsMine)
		{
			PhotonNetwork.Destroy(info.photonView);
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003698 RID: 13976 RVA: 0x0012D8A0 File Offset: 0x0012BAA0
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.validDisable = false;
	}

	// Token: 0x06003699 RID: 13977 RVA: 0x0012D8A9 File Offset: 0x0012BAA9
	protected override bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		if (!this.transferrable)
		{
			return info.Sender == PhotonNetwork.MasterClient;
		}
		return base.ValidOnSerialize(stream, in info);
	}

	// Token: 0x04004742 RID: 18242
	[SerializeField]
	private bool transferrable;

	// Token: 0x04004743 RID: 18243
	[SerializeField]
	private MonoBehaviour targetComponent;

	// Token: 0x04004744 RID: 18244
	private IGorillaSerializeableScene sceneSerializeTarget;

	// Token: 0x04004745 RID: 18245
	protected bool validDisable = true;
}
