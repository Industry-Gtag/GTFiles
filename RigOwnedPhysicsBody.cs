using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D0C RID: 3340
public class RigOwnedPhysicsBody : MonoBehaviour
{
	// Token: 0x060052FA RID: 21242 RVA: 0x001B6390 File Offset: 0x001B4590
	private void Awake()
	{
		this.hasTransformView = this.transformView != null;
		this.hasRigidbodyView = this.rigidbodyView != null;
		if (!this.hasTransformView && !this.hasRigidbodyView && this.otherComponents.Length == 0)
		{
			GTDev.LogError<string>("RigOwnedPhysicsBody has nothing to do! No TransformView, RigidbodyView, or otherComponents", null);
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.transform.parent = null;
				return;
			}
			if (this.hasRigidbodyView)
			{
				this.rigidbodyView.transform.parent = null;
			}
		}
	}

	// Token: 0x060052FB RID: 21243 RVA: 0x001B6420 File Offset: 0x001B4620
	private void OnEnable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnNetDisconnect;
		if (!this.hasRig)
		{
			this.rig = base.GetComponentInParent<VRRig>();
			this.hasRig = this.rig != null;
		}
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(true);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(true);
			}
		}
		if (NetworkSystem.Instance.InRoom)
		{
			this.OnNetConnect();
			return;
		}
		this.OnNetDisconnect();
	}

	// Token: 0x060052FC RID: 21244 RVA: 0x001B64F8 File Offset: 0x001B46F8
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnNetConnect;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnNetDisconnect;
		if (this.detachTransform)
		{
			if (this.hasTransformView)
			{
				this.transformView.gameObject.SetActive(false);
			}
			else if (this.hasRigidbodyView)
			{
				this.rigidbodyView.gameObject.SetActive(false);
			}
		}
		this.OnNetDisconnect();
	}

	// Token: 0x060052FD RID: 21245 RVA: 0x001B658C File Offset: 0x001B478C
	private void OnNetConnect()
	{
		if (this.hasTransformView)
		{
			this.transformView.enabled = this.hasRig;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = this.hasRig;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = this.hasRig;
		}
		if (!this.hasRig)
		{
			return;
		}
		PhotonView getView = this.rig.netView.GetView;
		List<Component> observedComponents = getView.ObservedComponents;
		if (this.hasTransformView)
		{
			this.transformView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.transformView))
			{
				observedComponents.Add(this.transformView);
			}
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.SetIsMine(getView.IsMine);
			if (!observedComponents.Contains(this.rigidbodyView))
			{
				observedComponents.Add(this.rigidbodyView);
			}
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			if (!observedComponents.Contains(monoBehaviourPun))
			{
				observedComponents.Add(monoBehaviourPun);
			}
		}
	}

	// Token: 0x060052FE RID: 21246 RVA: 0x001B66A4 File Offset: 0x001B48A4
	private void OnNetDisconnect()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.hasTransformView)
		{
			this.transformView.enabled = false;
		}
		if (this.hasRigidbodyView)
		{
			this.rigidbodyView.enabled = false;
		}
		MonoBehaviourPun[] array = this.otherComponents;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		if (!this.hasRig || !NetworkSystem.Instance.InRoom)
		{
			return;
		}
		List<Component> observedComponents = this.rig.netView.GetView.ObservedComponents;
		if (this.hasTransformView)
		{
			observedComponents.Remove(this.transformView);
		}
		if (this.hasRigidbodyView)
		{
			observedComponents.Remove(this.rigidbodyView);
		}
		foreach (MonoBehaviourPun monoBehaviourPun in this.otherComponents)
		{
			observedComponents.Remove(monoBehaviourPun);
		}
	}

	// Token: 0x04006490 RID: 25744
	private VRRig rig;

	// Token: 0x04006491 RID: 25745
	public RigOwnedTransformView transformView;

	// Token: 0x04006492 RID: 25746
	private bool hasTransformView;

	// Token: 0x04006493 RID: 25747
	public RigOwnedRigidbodyView rigidbodyView;

	// Token: 0x04006494 RID: 25748
	private bool hasRigidbodyView;

	// Token: 0x04006495 RID: 25749
	public MonoBehaviourPun[] otherComponents;

	// Token: 0x04006496 RID: 25750
	private bool hasRig;

	// Token: 0x04006497 RID: 25751
	[Tooltip("To make a rigidbody unaffected by the movement of the holdable part, put this script on the holdable, make the RigOwnedRigidbodyView a child of it, and check this box")]
	[SerializeField]
	private bool detachTransform;
}
