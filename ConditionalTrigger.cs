using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000671 RID: 1649
public class ConditionalTrigger : MonoBehaviour, IRigAware
{
	// Token: 0x17000415 RID: 1045
	// (get) Token: 0x06002918 RID: 10520 RVA: 0x000DF4B5 File Offset: 0x000DD6B5
	private int intValue
	{
		get
		{
			return (int)this._tracking;
		}
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x000DF4BD File Offset: 0x000DD6BD
	public void SetProximityFromRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._from = this._rig.transform;
		}
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x000DF4FB File Offset: 0x000DD6FB
	public void SetProximityToRig()
	{
		if (this._rig.AsNull<VRRig>() == null)
		{
			ConditionalTrigger.FindRig(out this._rig);
		}
		if (this._rig)
		{
			this._to = this._rig.transform;
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000DF539 File Offset: 0x000DD739
	public void SetProximityFrom(Transform from)
	{
		this._from = from;
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000DF542 File Offset: 0x000DD742
	public void SetProxmityTo(Transform to)
	{
		this._to = to;
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x000DF54B File Offset: 0x000DD74B
	public void TrackedSet(TriggerCondition conditions)
	{
		this._tracking = conditions;
	}

	// Token: 0x0600291E RID: 10526 RVA: 0x000DF554 File Offset: 0x000DD754
	public void TrackedAdd(TriggerCondition conditions)
	{
		this._tracking |= conditions;
	}

	// Token: 0x0600291F RID: 10527 RVA: 0x000DF564 File Offset: 0x000DD764
	public void TrackedRemove(TriggerCondition conditions)
	{
		this._tracking &= ~conditions;
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000DF54B File Offset: 0x000DD74B
	public void TrackedSet(int conditions)
	{
		this._tracking = (TriggerCondition)conditions;
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000DF554 File Offset: 0x000DD754
	public void TrackedAdd(int conditions)
	{
		this._tracking |= (TriggerCondition)conditions;
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000DF564 File Offset: 0x000DD764
	public void TrackedRemove(int conditions)
	{
		this._tracking &= (TriggerCondition)(~(TriggerCondition)conditions);
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000DF575 File Offset: 0x000DD775
	public void TrackedClear()
	{
		this._tracking = TriggerCondition.None;
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000DF57E File Offset: 0x000DD77E
	private void OnEnable()
	{
		this._timeSince = 0f;
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000DF590 File Offset: 0x000DD790
	private void Update()
	{
		if (this.IsTracking(TriggerCondition.TimeElapsed))
		{
			this.TrackTimeElapsed();
		}
		if (this.IsTracking(TriggerCondition.Proximity))
		{
			this.TrackProximity();
			return;
		}
		this._distance = 0f;
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000DF5BC File Offset: 0x000DD7BC
	private void TrackTimeElapsed()
	{
		if (this._timeSince.HasElapsed(this._interval, true))
		{
			UnityEvent unityEvent = this.onTimeElapsed;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000DF5E4 File Offset: 0x000DD7E4
	private void TrackProximity()
	{
		if (!this._from || !this._to)
		{
			this._distance = 0f;
			return;
		}
		this._distance = Vector3.Distance(this._to.position, this._from.position);
		if (this._distance >= this._maxDistance)
		{
			UnityEvent unityEvent = this.onMaxDistance;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000DF656 File Offset: 0x000DD856
	private bool IsTracking(TriggerCondition condition)
	{
		return (this._tracking & condition) == condition;
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000DF663 File Offset: 0x000DD863
	private static void FindRig(out VRRig rig)
	{
		if (PhotonNetwork.InRoom)
		{
			rig = GorillaGameManager.StaticFindRigForPlayer(NetPlayer.Get(PhotonNetwork.LocalPlayer));
			return;
		}
		rig = VRRig.LocalRig;
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000DF685 File Offset: 0x000DD885
	public void SetRig(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x040035A1 RID: 13729
	[Space]
	[SerializeField]
	private TriggerCondition _tracking;

	// Token: 0x040035A2 RID: 13730
	[Space]
	[SerializeField]
	private Transform _from;

	// Token: 0x040035A3 RID: 13731
	[SerializeField]
	private Transform _to;

	// Token: 0x040035A4 RID: 13732
	[SerializeField]
	private float _maxDistance;

	// Token: 0x040035A5 RID: 13733
	[NonSerialized]
	private float _distance;

	// Token: 0x040035A6 RID: 13734
	[Space]
	public UnityEvent onMaxDistance;

	// Token: 0x040035A7 RID: 13735
	[SerializeField]
	private float _interval = 1f;

	// Token: 0x040035A8 RID: 13736
	[NonSerialized]
	private TimeSince _timeSince;

	// Token: 0x040035A9 RID: 13737
	[Space]
	public UnityEvent onTimeElapsed;

	// Token: 0x040035AA RID: 13738
	[Space]
	private VRRig _rig;
}
