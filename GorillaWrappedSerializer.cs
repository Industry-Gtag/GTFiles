using System;
using Fusion;
using GorillaTag;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200084B RID: 2123
[NetworkBehaviourWeaved(0)]
internal abstract class GorillaWrappedSerializer : NetworkBehaviour, IPunObservable, IPunInstantiateMagicCallback, IOnPhotonViewPreNetDestroy, IPhotonViewCallback
{
	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x0600369B RID: 13979 RVA: 0x0012D8D8 File Offset: 0x0012BAD8
	public NetworkView NetView
	{
		get
		{
			return this.netView;
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x0600369C RID: 13980 RVA: 0x0012D8E0 File Offset: 0x0012BAE0
	// (set) Token: 0x0600369D RID: 13981 RVA: 0x0012D8E8 File Offset: 0x0012BAE8
	protected virtual object data { get; set; }

	// Token: 0x170004CC RID: 1228
	// (get) Token: 0x0600369E RID: 13982 RVA: 0x0012D8F1 File Offset: 0x0012BAF1
	public bool IsLocallyOwned
	{
		get
		{
			return this.netView.IsMine;
		}
	}

	// Token: 0x170004CD RID: 1229
	// (get) Token: 0x0600369F RID: 13983 RVA: 0x0012D8FE File Offset: 0x0012BAFE
	public bool IsValid
	{
		get
		{
			return this.netView.IsValid;
		}
	}

	// Token: 0x060036A0 RID: 13984 RVA: 0x0012D90B File Offset: 0x0012BB0B
	private void Awake()
	{
		if (this.netView == null)
		{
			this.netView = base.GetComponent<NetworkView>();
		}
	}

	// Token: 0x060036A1 RID: 13985 RVA: 0x0012D928 File Offset: 0x0012BB28
	void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.netView == null || !this.netView.IsValid)
		{
			return;
		}
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	// Token: 0x060036A2 RID: 13986 RVA: 0x0012D960 File Offset: 0x0012BB60
	public override void Spawned()
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(base.Object.StateAuthority.PlayerId, base.Runner.Tick.Raw);
		this.ProcessSpawn(photonMessageInfoWrapped);
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x0012D9A0 File Offset: 0x0012BBA0
	private void ProcessSpawn(PhotonMessageInfoWrapped wrappedInfo)
	{
		this.successfullInstantiate = this.OnSpawnSetupCheck(wrappedInfo, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			GameObject gameObject = this.targetObject;
			IWrappedSerializable wrappedSerializable = ((gameObject != null) ? gameObject.GetComponent(this.targetType) : null) as IWrappedSerializable;
			if (wrappedSerializable != null)
			{
				this.serializeTarget = wrappedSerializable;
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccesfullySpawned(wrappedInfo);
			return;
		}
		this.FailedToSpawn();
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x0012DA1B File Offset: 0x0012BC1B
	protected virtual bool OnSpawnSetupCheck(PhotonMessageInfoWrapped wrappedInfo, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IWrappedSerializable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x060036A5 RID: 13989
	protected abstract void OnSuccesfullySpawned(PhotonMessageInfoWrapped info);

	// Token: 0x060036A6 RID: 13990 RVA: 0x0012DA32 File Offset: 0x0012BC32
	private void FailedToSpawn()
	{
		Debug.LogError("Failed to network instantiate");
		MonkeAgentCleanup.RegisterForDestroy(this.netView.GetView);
		this.netView.GetView.ObservedComponents.Remove(this);
	}

	// Token: 0x060036A7 RID: 13991
	protected abstract void OnFailedSpawn();

	// Token: 0x060036A8 RID: 13992 RVA: 0x0012D6E1 File Offset: 0x0012B8E1
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x060036A9 RID: 13993 RVA: 0x0012DA65 File Offset: 0x0012BC65
	public override void FixedUpdateNetwork()
	{
		this.data = this.serializeTarget.OnSerializeWrite();
	}

	// Token: 0x060036AA RID: 13994 RVA: 0x0012DA78 File Offset: 0x0012BC78
	public override void Render()
	{
		if (!base.Object.HasStateAuthority)
		{
			this.serializeTarget.OnSerializeRead(this.data);
		}
	}

	// Token: 0x060036AB RID: 13995 RVA: 0x0012DA98 File Offset: 0x0012BC98
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, in info))
		{
			return;
		}
		if (stream.IsWriting)
		{
			this.serializeTarget.OnSerializeWrite(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeRead(stream, info);
	}

	// Token: 0x060036AC RID: 13996 RVA: 0x0012DAE4 File Offset: 0x0012BCE4
	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060036AD RID: 13997 RVA: 0x0012DAE4 File Offset: 0x0012BCE4
	void IOnPhotonViewPreNetDestroy.OnPreNetDestroy(PhotonView rootView)
	{
		this.OnBeforeDespawn();
	}

	// Token: 0x060036AE RID: 13998
	protected abstract void OnBeforeDespawn();

	// Token: 0x060036AF RID: 13999 RVA: 0x0012DAEC File Offset: 0x0012BCEC
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.netView.GetView.RefreshRpcMonoBehaviourCache();
		t.SetClassTarget(this.serializeTarget, this);
		return t;
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x0012DB1C File Offset: 0x0012BD1C
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget rpcTarget = (targetOthers ? RpcTarget.Others : RpcTarget.MasterClient);
		this.netView.SendRPC(rpcName, rpcTarget, data);
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void FusionDataRPC(string method, RpcTarget target, params object[] parameters)
	{
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void FusionDataRPC(string method, NetPlayer targetPlayer, params object[] parameters)
	{
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x0012DB3F File Offset: 0x0012BD3F
	public void SendRPC(string rpcName, NetPlayer targetPlayer, params object[] data)
	{
		this.netView.GetView.RPC(rpcName, ((PunNetPlayer)targetPlayer).PlayerRef, data);
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x00002C2D File Offset: 0x00000E2D
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
	}

	// Token: 0x04004746 RID: 18246
	protected bool successfullInstantiate;

	// Token: 0x04004747 RID: 18247
	protected IWrappedSerializable serializeTarget;

	// Token: 0x04004748 RID: 18248
	private Type targetType;

	// Token: 0x04004749 RID: 18249
	protected GameObject targetObject;

	// Token: 0x0400474A RID: 18250
	[SerializeField]
	protected NetworkView netView;
}
