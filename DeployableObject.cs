using System;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaTag;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002BE RID: 702
public class DeployableObject : TransferrableObject
{
	// Token: 0x06001222 RID: 4642 RVA: 0x000616D3 File Offset: 0x0005F8D3
	protected override void Awake()
	{
		this._deploySignal.OnSignal += this.DeployRPC;
		base.Awake();
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x000616F4 File Offset: 0x0005F8F4
	internal override void OnEnable()
	{
		this._deploySignal.Enable();
		VRRig componentInParent = base.GetComponentInParent<VRRig>();
		for (int i = 0; i < this._rigAwareObjects.Length; i++)
		{
			IRigAware rigAware = this._rigAwareObjects[i] as IRigAware;
			if (rigAware != null)
			{
				rigAware.SetRig(componentInParent);
			}
		}
		this.m_VRRig = componentInParent;
		ListProcessor<Action<RigContainer>> disableEvent = this.m_VRRig.rigContainer.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Add(in action);
		base.OnEnable();
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.itemState &= (TransferrableObject.ItemStates)(-2);
	}

	// Token: 0x06001224 RID: 4644 RVA: 0x00061791 File Offset: 0x0005F991
	internal override void OnDisable()
	{
		this.m_VRRig = null;
		this._deploySignal.Disable();
		if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
		base.OnDisable();
	}

	// Token: 0x06001225 RID: 4645 RVA: 0x000617C0 File Offset: 0x0005F9C0
	private void OnRigPreDisable(RigContainer rc)
	{
		this.m_spamChecker.Reset();
		ListProcessor<Action<RigContainer>> disableEvent = rc.RigEvents.disableEvent;
		Action<RigContainer> action = new Action<RigContainer>(this.OnRigPreDisable);
		disableEvent.Remove(in action);
	}

	// Token: 0x06001226 RID: 4646 RVA: 0x000617F8 File Offset: 0x0005F9F8
	protected override void OnDestroy()
	{
		this._deploySignal.Dispose();
		base.OnDestroy();
	}

	// Token: 0x06001227 RID: 4647 RVA: 0x0006180C File Offset: 0x0005FA0C
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (!this._objectToDeploy.activeSelf)
			{
				this.DeployChild();
				return;
			}
		}
		else if (this._objectToDeploy.activeSelf)
		{
			this.ReturnChild();
		}
	}

	// Token: 0x06001228 RID: 4648 RVA: 0x00061860 File Offset: 0x0005FA60
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (VRRig.LocalRig != this.ownerRig)
		{
			return false;
		}
		bool flag = releasingHand == EquipmentInteractor.instance.leftHand;
		GorillaVelocityTracker interactPointVelocityTracker = GTPlayer.Instance.GetInteractPointVelocityTracker(flag);
		Transform transform = base.transform;
		Vector3 vector = transform.TransformPoint(Vector3.zero);
		Quaternion rotation = transform.rotation;
		Vector3 averageVelocity = interactPointVelocityTracker.GetAverageVelocity(true, 0.15f, false);
		this.DeployLocal(vector, rotation, averageVelocity, false);
		this._deploySignal.Raise(ReceiverGroup.Others, BitPackUtils.PackWorldPosForNetwork(vector), BitPackUtils.PackQuaternionForNetwork(rotation), BitPackUtils.PackWorldPosForNetwork(averageVelocity * 100f));
		return true;
	}

	// Token: 0x06001229 RID: 4649 RVA: 0x00061903 File Offset: 0x0005FB03
	protected virtual void DeployLocal(Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this.DisableWhileDeployed(true);
		this._child.Deploy(this, launchPos, launchRot, releaseVel, isRemote);
	}

	// Token: 0x0600122A RID: 4650 RVA: 0x00061920 File Offset: 0x0005FB20
	private void DeployRPC(long packedPos, int packedRot, long packedVel, PhotonSignalInfo info)
	{
		if (info.sender != base.OwningPlayer())
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "DeployRPC");
		if (!this.m_spamChecker.CheckCallTime(Time.unscaledTime))
		{
			return;
		}
		Vector3 vector = BitPackUtils.UnpackWorldPosFromNetwork(packedPos);
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(packedRot);
		Vector3 vector2 = BitPackUtils.UnpackWorldPosFromNetwork(packedVel) / 100f;
		float num = 10000f;
		if (!(in vector).IsValid(in num) || !(in quaternion).IsValid() || !this.m_VRRig.IsPositionInRange(vector, this._maxDeployDistance))
		{
			return;
		}
		this.DeployLocal(vector, quaternion, this.m_VRRig.ClampVelocityRelativeToPlayerSafe(vector2, this._maxThrowVelocity, 30f), true);
	}

	// Token: 0x0600122B RID: 4651 RVA: 0x000619D0 File Offset: 0x0005FBD0
	private void DisableWhileDeployed(bool active)
	{
		if (this._disabledWhileDeployed.IsNullOrEmpty<GameObject>())
		{
			return;
		}
		for (int i = 0; i < this._disabledWhileDeployed.Length; i++)
		{
			this._disabledWhileDeployed[i].SetActive(!active);
		}
	}

	// Token: 0x0600122C RID: 4652 RVA: 0x00061A0F File Offset: 0x0005FC0F
	public void DeployChild()
	{
		this.itemState |= TransferrableObject.ItemStates.State0;
		this._objectToDeploy.SetActive(true);
		this.DisableWhileDeployed(true);
		UnityEvent onDeploy = this._onDeploy;
		if (onDeploy == null)
		{
			return;
		}
		onDeploy.Invoke();
	}

	// Token: 0x0600122D RID: 4653 RVA: 0x00061A42 File Offset: 0x0005FC42
	public void ReturnChild()
	{
		this.itemState &= (TransferrableObject.ItemStates)(-2);
		this._objectToDeploy.SetActive(false);
		this.DisableWhileDeployed(false);
		UnityEvent onReturn = this._onReturn;
		if (onReturn == null)
		{
			return;
		}
		onReturn.Invoke();
	}

	// Token: 0x040015EE RID: 5614
	[SerializeField]
	private GameObject _objectToDeploy;

	// Token: 0x040015EF RID: 5615
	[SerializeField]
	private DeployedChild _child;

	// Token: 0x040015F0 RID: 5616
	[SerializeField]
	private GameObject[] _disabledWhileDeployed = new GameObject[0];

	// Token: 0x040015F1 RID: 5617
	[SerializeField]
	private SoundBankPlayer deploySound;

	// Token: 0x040015F2 RID: 5618
	[SerializeField]
	private PhotonSignal<long, int, long> _deploySignal = "_deploySignal";

	// Token: 0x040015F3 RID: 5619
	[SerializeField]
	private float _maxDeployDistance = 4f;

	// Token: 0x040015F4 RID: 5620
	[SerializeField]
	private float _maxThrowVelocity = 50f;

	// Token: 0x040015F5 RID: 5621
	[SerializeField]
	private UnityEvent _onDeploy;

	// Token: 0x040015F6 RID: 5622
	[SerializeField]
	private UnityEvent _onReturn;

	// Token: 0x040015F7 RID: 5623
	[SerializeField]
	private Component[] _rigAwareObjects = new Component[0];

	// Token: 0x040015F8 RID: 5624
	[SerializeField]
	private CallLimiter m_spamChecker = new CallLimiter(1, 0.4f, 0.5f);

	// Token: 0x040015F9 RID: 5625
	private VRRig m_VRRig;
}
