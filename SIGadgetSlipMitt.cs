using System;
using Drawing;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020000F4 RID: 244
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
public class SIGadgetSlipMitt : SIGadget
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x060005DD RID: 1501 RVA: 0x00021F80 File Offset: 0x00020180
	private int _HandIndex
	{
		get
		{
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandL) || this.gameEntity.heldByHandIndex == 0)
			{
				return 0;
			}
			if ((this.m_snappable.snappedToJoint != null && this.m_snappable.snappedToJoint.jointType == SnapJointType.HandR) || this.gameEntity.heldByHandIndex == 1)
			{
				return 1;
			}
			return -1;
		}
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00021FFC File Offset: 0x000201FC
	private void Start()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
		foreach (AudioClip audioClip in this.m_clips)
		{
			if (audioClip)
			{
				audioClip.LoadAudioData();
			}
		}
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x000220D0 File Offset: 0x000202D0
	private void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this._HandleStartInteraction));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this._HandleStartInteraction));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Remove(gameEntity3.OnReleased, new Action(this._HandleStopInteraction));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Remove(gameEntity4.OnUnsnapped, new Action(this._HandleStopInteraction));
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00022184 File Offset: 0x00020384
	private void _HandleStartInteraction()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._attachedPlayerActorNr = this.gameEntity.AttachedPlayerActorNr;
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this._attachedPlayerActorNr, out gamePlayer))
		{
			return;
		}
		this._attachedVRRig = gamePlayer.rig;
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x000221C6 File Offset: 0x000203C6
	private void _HandleStopInteraction()
	{
		this._attachedPlayerActorNr = -1;
		this._attachedVRRig = null;
		if (!this.gameEntity.IsAuthority())
		{
			return;
		}
		this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x000221EC File Offset: 0x000203EC
	protected void FixedUpdate()
	{
		if ((!this.IsEquippedLocal() && !this.activatedLocally) || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._wasActivated = this._isActivated;
		this._isActivated = this._CheckInput();
		if (Time.unscaledTime < this._airGrabTime + this.m_slipperySurfacesTime)
		{
			GTPlayer.Instance.SetMaximumSlipThisFrame();
		}
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
			if (state != SIGadgetSlipMitt.EState.Slip)
			{
				return;
			}
			if (!this._isActivated)
			{
				this.SetStateAuthority(SIGadgetSlipMitt.EState.Idle);
				GTPlayer.Instance.UnsetGravityOverride(this);
				return;
			}
			this._airReleaseSpeed = 0f;
			if (this._HandIndex == 0)
			{
				GTPlayer.Instance.SetLeftMaximumSlipThisFrame();
				this._attachedHandState = GTPlayer.Instance.LeftHand;
				return;
			}
			GTPlayer.Instance.SetRightMaximumSlipThisFrame();
			this._attachedHandState = GTPlayer.Instance.RightHand;
		}
		else if (this._isActivated && !base.IsBlocked(SIExclusionType.AffectsLocalMovement))
		{
			this._PlayHaptic(0.1f);
			GTPlayer.Instance.SetGravityOverride(this, new Action<GTPlayer>(this._HandleGTPlayerOnUpdateGravity));
			this.SetStateAuthority(SIGadgetSlipMitt.EState.Slip);
			return;
		}
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x000222FC File Offset: 0x000204FC
	private void _HandleGTPlayerOnUpdateGravity(GTPlayer gtPlayer)
	{
		Transform handFollower = this._attachedHandState.handFollower;
		Ray ray = new Ray(handFollower.position, handFollower.forward);
		int value = gtPlayer.locomotionEnabledLayers.value;
		float num = 1f;
		float num2 = 20f;
		int num3 = Physics.RaycastNonAlloc(ray, this._raycastHitResults, num, value, QueryTriggerInteraction.Ignore);
		RaycastHit[] raycastHitResults = this._raycastHitResults;
		Vector3 gravity = Physics.gravity;
		Vector3 vector = ray.direction * num2;
		Vector3 vector2 = ((num3 > 0) ? vector : gravity);
		Draw.ingame.Arrow(ray.origin, ray.origin + ray.direction);
		gtPlayer.AddForce(vector2 * gtPlayer.scale, ForceMode.Acceleration);
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000223C4 File Offset: 0x000205C4
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetSlipMitt.EState estate = (SIGadgetSlipMitt.EState)this.gameEntity.GetState();
		if (estate != this._state)
		{
			this._SetStateShared(estate);
		}
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x000223F5 File Offset: 0x000205F5
	private static bool _CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 3L;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00022403 File Offset: 0x00020603
	private void SetStateAuthority(SIGadgetSlipMitt.EState newState)
	{
		this._SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00022424 File Offset: 0x00020624
	private void _SetStateShared(SIGadgetSlipMitt.EState newState)
	{
		if (newState == this._state || !SIGadgetSlipMitt._CanChangeState((long)newState))
		{
			return;
		}
		this._state = newState;
		SIGadgetSlipMitt.EState state = this._state;
		if (state != SIGadgetSlipMitt.EState.Idle)
		{
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x0002245C File Offset: 0x0002065C
	private bool _CheckInput()
	{
		float num = (this._wasActivated ? this.m_inputDeactivateThreshold : this.m_inputActivateThreshold);
		return this.m_buttonActivatable.CheckInput(num);
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x0002248C File Offset: 0x0002068C
	private void _DoAirGrab()
	{
		float magnitude = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex).magnitude;
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x000224B4 File Offset: 0x000206B4
	private void _DoDash()
	{
		this._airGrabTime = Time.unscaledTime;
		Vector3 handVelocity = GamePlayerLocal.instance.GetHandVelocity(this._HandIndex);
		float num = this._CalculateDashSpeed(handVelocity.magnitude);
		GTPlayer instance = GTPlayer.Instance;
		instance.SetMaximumSlipThisFrame();
		instance.SetVelocity(handVelocity.normalized * -num);
		this._PlayHaptic(2f);
		this.SetStateAuthority(SIGadgetSlipMitt.EState.DashUsed);
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00022520 File Offset: 0x00020720
	private float _CalculateDashSpeed(float currentYankSpeed)
	{
		float num = Mathf.InverseLerp(this.m_yankMinSpeed, this.m_yankMaxSpeed, currentYankSpeed);
		float num2 = this.m_speedMappingCurve.Evaluate(num);
		return Mathf.Lerp(this.m_minDashSpeed, this._maxDashSpeed, num2);
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00022560 File Offset: 0x00020760
	private void _PlayHaptic(float strengthMultiplier)
	{
		bool flag;
		if (base.FindAttachedHand(out flag))
		{
			GorillaTagger.Instance.StartVibration(flag, GorillaTagger.Instance.tapHapticStrength * strengthMultiplier, GorillaTagger.Instance.tapHapticDuration);
		}
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00022598 File Offset: 0x00020798
	private void _PlayAudio(int index)
	{
		this.m_audioSource.clip = this.m_clips[index];
		this.m_audioSource.volume = this.m_clipVolumes[index];
		this.m_audioSource.GTPlay();
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x000225CB File Offset: 0x000207CB
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this._maxDashSpeed = (withUpgrades.Contains(SIUpgradeType.Dash_Yoyo_Speed) ? this.m_maxDashSpeedUpgraded : this.m_maxDashSpeedDefault);
	}

	// Token: 0x04000738 RID: 1848
	private const string preLog = "[SIGadgetSlipMitt]  ";

	// Token: 0x04000739 RID: 1849
	private const string preErr = "[SIGadgetSlipMitt]  ERROR!!!  ";

	// Token: 0x0400073A RID: 1850
	[SerializeField]
	private GameSnappable m_snappable;

	// Token: 0x0400073B RID: 1851
	[SerializeField]
	private Transform m_yoyoDefaultPosXform;

	// Token: 0x0400073C RID: 1852
	[SerializeField]
	private GameButtonActivatable m_buttonActivatable;

	// Token: 0x0400073D RID: 1853
	[SerializeField]
	private float m_inputActivateThreshold = 0.35f;

	// Token: 0x0400073E RID: 1854
	[SerializeField]
	private float m_inputDeactivateThreshold = 0.25f;

	// Token: 0x0400073F RID: 1855
	[SerializeField]
	private MeshRenderer m_yoyoRenderer;

	// Token: 0x04000740 RID: 1856
	[SerializeField]
	private AudioSource m_audioSource;

	// Token: 0x04000741 RID: 1857
	[SerializeField]
	public AudioClip[] m_clips;

	// Token: 0x04000742 RID: 1858
	[SerializeField]
	public float[] m_clipVolumes;

	// Token: 0x04000743 RID: 1859
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMinSpeed = 2f;

	// Token: 0x04000744 RID: 1860
	[Tooltip("Yank min/max: How fast you have to be moving your hand for the yank to register and result in a dash.")]
	[SerializeField]
	private float m_yankMaxSpeed = 8f;

	// Token: 0x04000745 RID: 1861
	[Tooltip("Dash min/max speed: The fastest speed the player will move")]
	[SerializeField]
	private float m_minDashSpeed = 4f;

	// Token: 0x04000746 RID: 1862
	private float _maxDashSpeed;

	// Token: 0x04000747 RID: 1863
	[SerializeField]
	private float m_maxDashSpeedDefault = 11f;

	// Token: 0x04000748 RID: 1864
	[SerializeField]
	private float m_maxDashSpeedUpgraded = 13f;

	// Token: 0x04000749 RID: 1865
	[Tooltip("Maps yank speed to dash speed.\nX = Yank Speed (min to max)\nY = Dash Speed (min to max).")]
	[SerializeField]
	private AnimationCurve m_speedMappingCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x0400074A RID: 1866
	[SerializeField]
	private float m_slipperySurfacesTime = 0.25f;

	// Token: 0x0400074B RID: 1867
	[SerializeField]
	private float m_maxInfluenceAngleDefault = 10f;

	// Token: 0x0400074C RID: 1868
	[SerializeField]
	private float m_maxInfluenceAngleUpgrade = 15f;

	// Token: 0x0400074D RID: 1869
	[SerializeField]
	private float m_cooldownDurationDefault = 6f;

	// Token: 0x0400074E RID: 1870
	[SerializeField]
	private float m_cooldownDurationUpgrade = 5f;

	// Token: 0x0400074F RID: 1871
	[SerializeField]
	private Transform m_airGrabXform;

	// Token: 0x04000750 RID: 1872
	private bool _isActivated;

	// Token: 0x04000751 RID: 1873
	private bool _wasActivated;

	// Token: 0x04000752 RID: 1874
	private float _airGrabTime;

	// Token: 0x04000753 RID: 1875
	private float _airReleaseSpeed;

	// Token: 0x04000754 RID: 1876
	private Vector3 _airReleaseVector;

	// Token: 0x04000755 RID: 1877
	private VRRig _attachedVRRig;

	// Token: 0x04000756 RID: 1878
	private GTPlayer.HandState _attachedHandState;

	// Token: 0x04000757 RID: 1879
	private int _lastAttachedPlayerActorNr;

	// Token: 0x04000758 RID: 1880
	private int _attachedPlayerActorNr = int.MinValue;

	// Token: 0x04000759 RID: 1881
	private bool _isTagged;

	// Token: 0x0400075A RID: 1882
	private SIGadgetSlipMitt.EState _state;

	// Token: 0x0400075B RID: 1883
	private RaycastHit[] _raycastHitResults = new RaycastHit[1];

	// Token: 0x020000F5 RID: 245
	private enum EState
	{
		// Token: 0x0400075D RID: 1885
		Idle,
		// Token: 0x0400075E RID: 1886
		Slip,
		// Token: 0x0400075F RID: 1887
		DashUsed,
		// Token: 0x04000760 RID: 1888
		Count
	}
}
