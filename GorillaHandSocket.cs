using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200087A RID: 2170
[DisallowMultipleComponent]
public class GorillaHandSocket : MonoBehaviour
{
	// Token: 0x17000507 RID: 1287
	// (get) Token: 0x06003886 RID: 14470 RVA: 0x001341DC File Offset: 0x001323DC
	public GorillaHandNode attachedHand
	{
		get
		{
			return this._attachedHand;
		}
	}

	// Token: 0x17000508 RID: 1288
	// (get) Token: 0x06003887 RID: 14471 RVA: 0x001341E4 File Offset: 0x001323E4
	public bool inUse
	{
		get
		{
			return this._inUse;
		}
	}

	// Token: 0x06003888 RID: 14472 RVA: 0x001341EC File Offset: 0x001323EC
	public static bool FetchSocket(Collider collider, out GorillaHandSocket socket)
	{
		return GorillaHandSocket.gColliderToSocket.TryGetValue(collider, out socket);
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x001341FA File Offset: 0x001323FA
	public bool CanAttach()
	{
		return !this._inUse && this._sinceSocketStateChange.HasElapsed(this.attachCooldown, true);
	}

	// Token: 0x0600388A RID: 14474 RVA: 0x00134218 File Offset: 0x00132418
	public void Attach(GorillaHandNode hand)
	{
		if (!this.CanAttach())
		{
			return;
		}
		if (hand == null)
		{
			return;
		}
		hand.attachedToSocket = this;
		this._attachedHand = hand;
		this._inUse = true;
		this.OnHandAttach();
	}

	// Token: 0x0600388B RID: 14475 RVA: 0x00134248 File Offset: 0x00132448
	public void Detach()
	{
		GorillaHandNode gorillaHandNode;
		this.Detach(out gorillaHandNode);
	}

	// Token: 0x0600388C RID: 14476 RVA: 0x00134260 File Offset: 0x00132460
	public void Detach(out GorillaHandNode hand)
	{
		if (this._inUse)
		{
			this._inUse = false;
		}
		if (this._attachedHand == null)
		{
			hand = null;
			return;
		}
		hand = this._attachedHand;
		hand.attachedToSocket = null;
		this._attachedHand = null;
		this.OnHandDetach();
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x0600388D RID: 14477 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnHandAttach()
	{
	}

	// Token: 0x0600388E RID: 14478 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnHandDetach()
	{
	}

	// Token: 0x0600388F RID: 14479 RVA: 0x001342B6 File Offset: 0x001324B6
	protected virtual void OnUpdateAttached()
	{
		this._attachedHand.transform.position = base.transform.position;
	}

	// Token: 0x06003890 RID: 14480 RVA: 0x001342D3 File Offset: 0x001324D3
	private void OnEnable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.TryAdd(this.collider, this);
	}

	// Token: 0x06003891 RID: 14481 RVA: 0x001342F6 File Offset: 0x001324F6
	private void OnDisable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.Remove(this.collider);
	}

	// Token: 0x06003892 RID: 14482 RVA: 0x00134318 File Offset: 0x00132518
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x06003893 RID: 14483 RVA: 0x00134320 File Offset: 0x00132520
	private void FixedUpdate()
	{
		if (!this._inUse)
		{
			return;
		}
		if (!this._attachedHand)
		{
			return;
		}
		this.OnUpdateAttached();
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x00134340 File Offset: 0x00132540
	private void Setup()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(UnityTag.GorillaHandSocket);
		base.gameObject.SetLayer(UnityLayer.GorillaHandSocket);
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x04004887 RID: 18567
	public Collider collider;

	// Token: 0x04004888 RID: 18568
	public float attachCooldown = 0.5f;

	// Token: 0x04004889 RID: 18569
	public HandSocketConstraint constraint;

	// Token: 0x0400488A RID: 18570
	[NonSerialized]
	private GorillaHandNode _attachedHand;

	// Token: 0x0400488B RID: 18571
	[NonSerialized]
	private bool _inUse;

	// Token: 0x0400488C RID: 18572
	[NonSerialized]
	private TimeSince _sinceSocketStateChange;

	// Token: 0x0400488D RID: 18573
	private static readonly Dictionary<Collider, GorillaHandSocket> gColliderToSocket = new Dictionary<Collider, GorillaHandSocket>(64);
}
