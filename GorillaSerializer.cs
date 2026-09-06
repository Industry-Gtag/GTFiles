using System;
using GorillaExtensions;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000848 RID: 2120
[RequireComponent(typeof(PhotonView))]
internal class GorillaSerializer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
{
	// Token: 0x06003684 RID: 13956 RVA: 0x0012D5A8 File Offset: 0x0012B7A8
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!this.successfullInstantiate || this.serializeTarget == null || !this.ValidOnSerialize(stream, in info))
		{
			return;
		}
		if (stream.IsReading)
		{
			this.serializeTarget.OnSerializeRead(stream, info);
			return;
		}
		this.serializeTarget.OnSerializeWrite(stream, info);
	}

	// Token: 0x06003685 RID: 13957 RVA: 0x0012D5F4 File Offset: 0x0012B7F4
	public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if (this.photonView == null)
		{
			return;
		}
		this.successfullInstantiate = this.OnInstantiateSetup(info, out this.targetObject, out this.targetType);
		if (this.successfullInstantiate)
		{
			if (this.targetType != null && this.targetObject.IsNotNull())
			{
				IGorillaSerializeable gorillaSerializeable = this.targetObject.GetComponent(this.targetType) as IGorillaSerializeable;
				if (gorillaSerializeable != null)
				{
					this.serializeTarget = gorillaSerializeable;
				}
			}
			if (this.serializeTarget == null)
			{
				this.successfullInstantiate = false;
			}
		}
		if (this.successfullInstantiate)
		{
			this.OnSuccessfullInstantiate(info);
			return;
		}
		if (PhotonNetwork.InRoom && this.photonView.IsMine)
		{
			MonkeAgentCleanup.RegisterForDestroy(this.photonView);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		this.photonView.ObservedComponents.Remove(this);
	}

	// Token: 0x06003686 RID: 13958 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnSuccessfullInstantiate(PhotonMessageInfo info)
	{
	}

	// Token: 0x06003687 RID: 13959 RVA: 0x0012D6CA File Offset: 0x0012B8CA
	protected virtual bool OnInstantiateSetup(PhotonMessageInfo info, out GameObject outTargetObject, out Type outTargetType)
	{
		outTargetType = typeof(IGorillaSerializeable);
		outTargetObject = base.gameObject;
		return true;
	}

	// Token: 0x06003688 RID: 13960 RVA: 0x0012D6E1 File Offset: 0x0012B8E1
	protected virtual bool ValidOnSerialize(PhotonStream stream, in PhotonMessageInfo info)
	{
		return info.Sender == info.photonView.Owner;
	}

	// Token: 0x06003689 RID: 13961 RVA: 0x0012D6F9 File Offset: 0x0012B8F9
	public virtual T AddRPCComponent<T>() where T : RPCNetworkBase
	{
		T t = base.gameObject.AddComponent<T>();
		this.photonView.RefreshRpcMonoBehaviourCache();
		return t;
	}

	// Token: 0x0600368A RID: 13962 RVA: 0x0012D714 File Offset: 0x0012B914
	public void SendRPC(string rpcName, bool targetOthers, params object[] data)
	{
		RpcTarget rpcTarget = (targetOthers ? RpcTarget.Others : RpcTarget.MasterClient);
		this.photonView.RPC(rpcName, rpcTarget, data);
	}

	// Token: 0x0600368B RID: 13963 RVA: 0x0012D737 File Offset: 0x0012B937
	public void SendRPC(string rpcName, Player targetPlayer, params object[] data)
	{
		this.photonView.RPC(rpcName, targetPlayer, data);
	}

	// Token: 0x0400473D RID: 18237
	protected bool successfullInstantiate;

	// Token: 0x0400473E RID: 18238
	protected IGorillaSerializeable serializeTarget;

	// Token: 0x0400473F RID: 18239
	private Type targetType;

	// Token: 0x04004740 RID: 18240
	protected GameObject targetObject;

	// Token: 0x04004741 RID: 18241
	[SerializeField]
	protected PhotonView photonView;
}
