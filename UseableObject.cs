using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005AF RID: 1455
[RequireComponent(typeof(UseableObjectEvents))]
public class UseableObject : TransferrableObject
{
	// Token: 0x170003DB RID: 987
	// (get) Token: 0x060024DA RID: 9434 RVA: 0x000C6017 File Offset: 0x000C4217
	public bool isMidUse
	{
		get
		{
			return this._isMidUse;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x060024DB RID: 9435 RVA: 0x000C601F File Offset: 0x000C421F
	public float useTimeElapsed
	{
		get
		{
			return this._useTimeElapsed;
		}
	}

	// Token: 0x170003DD RID: 989
	// (get) Token: 0x060024DC RID: 9436 RVA: 0x000C6027 File Offset: 0x000C4227
	public bool justUsed
	{
		get
		{
			if (!this._justUsed)
			{
				return false;
			}
			this._justUsed = false;
			return true;
		}
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000C603B File Offset: 0x000C423B
	protected override void Awake()
	{
		base.Awake();
		this._events = base.gameObject.GetOrAddComponent<UseableObjectEvents>();
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000C6054 File Offset: 0x000C4254
	internal override void OnEnable()
	{
		base.OnEnable();
		UseableObjectEvents events = this._events;
		VRRig myOnlineRig = base.myOnlineRig;
		NetPlayer netPlayer;
		if ((netPlayer = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
		{
			VRRig myRig = base.myRig;
			netPlayer = ((myRig != null) ? myRig.creator : null);
		}
		events.Init(netPlayer);
		this._events.Activate += this.OnObjectActivated;
		this._events.Deactivate += this.OnObjectDeactivated;
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000C60DE File Offset: 0x000C42DE
	internal override void OnDisable()
	{
		base.OnDisable();
		Object.Destroy(this._events);
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000C60F1 File Offset: 0x000C42F1
	private void OnObjectActivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000C60F1 File Offset: 0x000C42F1
	private void OnObjectDeactivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000C60F7 File Offset: 0x000C42F7
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isMidUse)
		{
			this._useTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000C611C File Offset: 0x000C431C
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onActivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._useTimeElapsed = 0f;
			this._isMidUse = true;
		}
		if (this._raiseActivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent activate = events.Activate;
			if (activate == null)
			{
				return;
			}
			activate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x000C6184 File Offset: 0x000C4384
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onDeactivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._isMidUse = false;
			this._justUsed = true;
		}
		if (this._raiseDeactivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent deactivate = events.Deactivate;
			if (deactivate == null)
			{
				return;
			}
			deactivate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000C61E5 File Offset: 0x000C43E5
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000C61F0 File Offset: 0x000C43F0
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04003061 RID: 12385
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04003062 RID: 12386
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04003063 RID: 12387
	[SerializeField]
	private UseableObjectEvents _events;

	// Token: 0x04003064 RID: 12388
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04003065 RID: 12389
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04003066 RID: 12390
	[NonSerialized]
	private DateTime _lastActivate;

	// Token: 0x04003067 RID: 12391
	[NonSerialized]
	private DateTime _lastDeactivate;

	// Token: 0x04003068 RID: 12392
	[NonSerialized]
	private bool _isMidUse;

	// Token: 0x04003069 RID: 12393
	[NonSerialized]
	private float _useTimeElapsed;

	// Token: 0x0400306A RID: 12394
	[NonSerialized]
	private bool _justUsed;

	// Token: 0x0400306B RID: 12395
	[NonSerialized]
	private int tempHandPos;

	// Token: 0x0400306C RID: 12396
	public UnityEvent onActivateLocal;

	// Token: 0x0400306D RID: 12397
	public UnityEvent onDeactivateLocal;
}
