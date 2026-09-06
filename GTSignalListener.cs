using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020008F1 RID: 2289
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x06003BFA RID: 15354 RVA: 0x001479F7 File Offset: 0x00145BF7
	// (set) Token: 0x06003BFB RID: 15355 RVA: 0x001479FF File Offset: 0x00145BFF
	public int rigActorID { get; private set; } = -1;

	// Token: 0x06003BFC RID: 15356 RVA: 0x00147A08 File Offset: 0x00145C08
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x06003BFD RID: 15357 RVA: 0x00147A10 File Offset: 0x00145C10
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x06003BFE RID: 15358 RVA: 0x00147A24 File Offset: 0x00145C24
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x06003BFF RID: 15359 RVA: 0x00147A32 File Offset: 0x00145C32
	private void RefreshActorID()
	{
		this.rig = base.GetComponentInParent<VRRig>(true);
		int num;
		if (!(this.rig == null))
		{
			NetPlayer creator = this.rig.Creator;
			num = ((creator != null) ? creator.ActorNumber : (-1));
		}
		else
		{
			num = -1;
		}
		this.rigActorID = num;
	}

	// Token: 0x06003C00 RID: 15360 RVA: 0x00147A6F File Offset: 0x00145C6F
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x06003C01 RID: 15361 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x06003C02 RID: 15362 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06003C03 RID: 15363 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06003C04 RID: 15364 RVA: 0x00002C2D File Offset: 0x00000E2D
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x04004C76 RID: 19574
	[Space]
	public GTSignalID signal;

	// Token: 0x04004C77 RID: 19575
	[Space]
	public VRRig rig;

	// Token: 0x04004C79 RID: 19577
	[Space]
	public bool deafen;

	// Token: 0x04004C7A RID: 19578
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x04004C7B RID: 19579
	public bool ignoreSelf;

	// Token: 0x04004C7C RID: 19580
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x04004C7D RID: 19581
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x04004C7E RID: 19582
	[Space]
	public UnityEvent onSignalReceived;
}
