using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using GorillaTag.Rendering;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

// Token: 0x0200021D RID: 541
[NetworkBehaviourWeaved(3)]
public class AngryBeeSwarm : NetworkComponent
{
	// Token: 0x17000149 RID: 329
	// (get) Token: 0x06000E2D RID: 3629 RVA: 0x0004D920 File Offset: 0x0004BB20
	public bool isDormant
	{
		get
		{
			return this.currentState == AngryBeeSwarm.ChaseState.Dormant;
		}
	}

	// Token: 0x06000E2E RID: 3630 RVA: 0x0004D92C File Offset: 0x0004BB2C
	protected override void Awake()
	{
		base.Awake();
		AngryBeeSwarm.instance = this;
		this.targetPlayer = null;
		this.currentState = AngryBeeSwarm.ChaseState.Dormant;
		this.grabTimestamp = -this.minGrabCooldown;
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06000E2F RID: 3631 RVA: 0x0004D97C File Offset: 0x0004BB7C
	private void InitializeSwarm()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.beeAnimator.transform.localPosition = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06000E30 RID: 3632 RVA: 0x0004D9C8 File Offset: 0x0004BBC8
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = AngryBeeSwarm.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			AngryBeeSwarm.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case AngryBeeSwarm.ChaseState.Dormant:
				if (Application.isEditor && Keyboard.current[Key.Space].wasPressedThisFrame)
				{
					this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
				}
				break;
			case AngryBeeSwarm.ChaseState.InitialEmerge:
				if (Time.time > this.emergeStartedTimestamp + this.totalTimeToEmerge)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Chasing;
				}
				break;
			case (AngryBeeSwarm.ChaseState)3:
				break;
			case AngryBeeSwarm.ChaseState.Chasing:
				if (this.followTarget == null || this.targetPlayer == null || Time.time > this.NextRefreshClosestPlayerTimestamp)
				{
					this.ChooseClosestTarget();
					if (this.followTarget != null)
					{
						this.BoredToDeathAtTimestamp = -1f;
					}
					else if (this.BoredToDeathAtTimestamp < 0f)
					{
						this.BoredToDeathAtTimestamp = Time.time + this.boredAfterDuration;
					}
				}
				if (this.BoredToDeathAtTimestamp >= 0f && Time.time > this.BoredToDeathAtTimestamp)
				{
					this.currentState = AngryBeeSwarm.ChaseState.Dormant;
				}
				else if (!(this.followTarget == null) && (this.followTarget.position - this.beeAnimator.transform.position).magnitude < this.catchDistance)
				{
					float num = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
					if (this.followTarget.position.y > num)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Grabbing;
					}
				}
				break;
			default:
				if (chaseState == AngryBeeSwarm.ChaseState.Grabbing)
				{
					if (Time.time > this.grabTimestamp + this.grabDuration)
					{
						this.currentState = AngryBeeSwarm.ChaseState.Dormant;
					}
				}
				break;
			}
		}
		if (this.lastState != this.currentState)
		{
			this.OnChangeState(this.currentState);
			this.lastState = this.currentState;
		}
		this.UpdateState();
	}

	// Token: 0x06000E31 RID: 3633 RVA: 0x0004DBAC File Offset: 0x0004BDAC
	public void UpdateState()
	{
		AngryBeeSwarm.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			if (NetworkSystem.Instance.InRoom)
			{
				this.SwarmEmergeUpdateShared();
				return;
			}
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				return;
			}
			break;
		default:
			if (chaseState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (this.targetPlayer == NetworkSystem.Instance.LocalPlayer)
				{
					this.RiseGrabbedLocalPlayer();
				}
				this.GrabBodyShared();
			}
			break;
		}
	}

	// Token: 0x06000E32 RID: 3634 RVA: 0x0004DC3B File Offset: 0x0004BE3B
	public void Emerge(Vector3 fromPosition, Vector3 toPosition)
	{
		base.transform.position = fromPosition;
		this.emergeFromPosition = fromPosition;
		this.emergeToPosition = toPosition;
		this.currentState = AngryBeeSwarm.ChaseState.InitialEmerge;
		this.emergeStartedTimestamp = Time.time;
	}

	// Token: 0x06000E33 RID: 3635 RVA: 0x0004DC6C File Offset: 0x0004BE6C
	private void OnChangeState(AngryBeeSwarm.ChaseState newState)
	{
		switch (newState)
		{
		case AngryBeeSwarm.ChaseState.Dormant:
			if (this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				base.transform.position = new Vector3(0f, -9999f, 0f);
				this.InitializeSwarm();
			}
			this.SetInitialRotations();
			return;
		case AngryBeeSwarm.ChaseState.InitialEmerge:
			this.emergeStartedTimestamp = Time.time;
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(0f);
			if (base.IsMine)
			{
				this.currentSpeed = 0f;
				this.ChooseClosestTarget();
			}
			this.SetInitialRotations();
			return;
		case (AngryBeeSwarm.ChaseState)3:
			break;
		case AngryBeeSwarm.ChaseState.Chasing:
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.beeAnimator.SetEmergeFraction(1f);
			this.ResetPath();
			this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
			this.BoredToDeathAtTimestamp = -1f;
			return;
		default:
		{
			if (newState != AngryBeeSwarm.ChaseState.Grabbing)
			{
				return;
			}
			if (!this.beeAnimator.gameObject.activeSelf)
			{
				this.beeAnimator.gameObject.SetActive(true);
			}
			this.grabTimestamp = Time.time;
			this.beeAnimator.transform.localPosition = this.ghostOffsetGrabbingLocal;
			VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
			if (vrrig != null)
			{
				this.followTarget = vrrig.transform;
			}
			break;
		}
		}
	}

	// Token: 0x06000E34 RID: 3636 RVA: 0x0004DE14 File Offset: 0x0004C014
	private void ChooseClosestTarget()
	{
		float num = Mathf.Lerp(this.initialRangeLimit, this.finalRangeLimit, (Time.time + this.totalTimeToEmerge - this.emergeStartedTimestamp) / this.rangeLimitBlendDuration);
		float num2 = num * num;
		VRRig vrrig = null;
		float num3 = ZoneShaderSettings.GetWaterY() + this.PlayerMinHeightAboveWater;
		foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
		{
			VRRig rig = rigContainer.Rig;
			if (rig.head != null && !(rig.head.rigTarget == null) && rig.head.rigTarget.position.y > num3)
			{
				float sqrMagnitude = (base.transform.position - rig.head.rigTarget.transform.position).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					vrrig = rig;
				}
			}
		}
		if (vrrig.IsNotNull())
		{
			this.targetPlayer = vrrig.creator;
			this.followTarget = vrrig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
		}
		else
		{
			this.targetPlayer = null;
			this.followTarget = null;
		}
		this.NextRefreshClosestPlayerTimestamp = Time.time + this.RefreshClosestPlayerInterval;
	}

	// Token: 0x06000E35 RID: 3637 RVA: 0x0004DF78 File Offset: 0x0004C178
	private void SetInitialRotations()
	{
		this.beeAnimator.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06000E36 RID: 3638 RVA: 0x0004DF90 File Offset: 0x0004C190
	private void SwarmEmergeUpdateShared()
	{
		if (Time.time < this.emergeStartedTimestamp + this.totalTimeToEmerge)
		{
			float num = (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge;
			if (base.IsMine)
			{
				base.transform.position = Vector3.Lerp(this.emergeFromPosition, this.emergeToPosition, (Time.time - this.emergeStartedTimestamp) / this.totalTimeToEmerge);
			}
			this.beeAnimator.SetEmergeFraction(num);
		}
	}

	// Token: 0x06000E37 RID: 3639 RVA: 0x0004E008 File Offset: 0x0004C208
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTimestamp + this.minGrabCooldown)
		{
			this.grabTimestamp = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTimestamp + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06000E38 RID: 3640 RVA: 0x0004E0B8 File Offset: 0x0004C2B8
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.pathPoints[this.pathPoints.Count - 1] = destination;
		Vector3 vector = this.pathPoints[this.currentPathPointIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentPathPointIdx + 1 < this.pathPoints.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentPathPointIdx++;
		}
	}

	// Token: 0x06000E39 RID: 3641 RVA: 0x0004E1C8 File Offset: 0x0004C3C8
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.pathPoints = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.pathPoints.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.pathPoints.Add(destination);
		this.currentPathPointIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06000E3A RID: 3642 RVA: 0x0004E29C File Offset: 0x0004C49C
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06000E3B RID: 3643 RVA: 0x0004E2A8 File Offset: 0x0004C4A8
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseInterval)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			float num = ZoneShaderSettings.GetWaterY() + this.MinHeightAboveWater;
			Vector3 position = this.followTarget.position;
			if (position.y < num)
			{
				position.y = num;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, position, this.currentSpeed * Time.deltaTime);
		}
	}

	// Token: 0x06000E3C RID: 3644 RVA: 0x0004E360 File Offset: 0x0004C560
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.beeAnimator.transform.localPosition = this.noisyOffset;
	}

	// Token: 0x06000E3D RID: 3645 RVA: 0x0004E3CD File Offset: 0x0004C5CD
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x1700014A RID: 330
	// (get) Token: 0x06000E3E RID: 3646 RVA: 0x0004E409 File Offset: 0x0004C609
	// (set) Token: 0x06000E3F RID: 3647 RVA: 0x0004E433 File Offset: 0x0004C633
	[Networked]
	[NetworkedWeaved(0, 3)]
	public unsafe BeeSwarmData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(BeeSwarmData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing AngryBeeSwarm.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(BeeSwarmData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06000E40 RID: 3648 RVA: 0x0004E45E File Offset: 0x0004C65E
	public override void WriteDataFusion()
	{
		this.Data = new BeeSwarmData(this.targetPlayer.ActorNumber, (int)this.currentState, this.currentSpeed);
	}

	// Token: 0x06000E41 RID: 3649 RVA: 0x0004E484 File Offset: 0x0004C684
	public override void ReadDataFusion()
	{
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(this.Data.TargetActorNumber);
		this.currentState = (AngryBeeSwarm.ChaseState)this.Data.CurrentState;
		if (float.IsFinite(this.Data.CurrentSpeed))
		{
			this.currentSpeed = this.Data.CurrentSpeed;
		}
	}

	// Token: 0x06000E42 RID: 3650 RVA: 0x0004E4EC File Offset: 0x0004C6EC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender == null || !info.Sender.Equals(PhotonNetwork.MasterClient))
		{
			return;
		}
		NetPlayer netPlayer = this.targetPlayer;
		stream.SendNext((netPlayer != null) ? netPlayer.ActorNumber : (-1));
		stream.SendNext(this.currentState);
		stream.SendNext(this.currentSpeed);
	}

	// Token: 0x06000E43 RID: 3651 RVA: 0x0004E554 File Offset: 0x0004C754
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num);
		this.currentState = (AngryBeeSwarm.ChaseState)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		if (float.IsFinite(num2))
		{
			this.currentSpeed = num2;
		}
	}

	// Token: 0x06000E44 RID: 3652 RVA: 0x0004E5B8 File Offset: 0x0004C7B8
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06000E45 RID: 3653 RVA: 0x0004E5D6 File Offset: 0x0004C7D6
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeSwarm();
		}
	}

	// Token: 0x06000E46 RID: 3654 RVA: 0x0004E5EA File Offset: 0x0004C7EA
	private void TestEmerge()
	{
		this.Emerge(this.testEmergeFrom.transform.position, this.testEmergeTo.transform.position);
	}

	// Token: 0x06000E48 RID: 3656 RVA: 0x0004E6A0 File Offset: 0x0004C8A0
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06000E49 RID: 3657 RVA: 0x0004E6B8 File Offset: 0x0004C8B8
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04001109 RID: 4361
	public static AngryBeeSwarm instance;

	// Token: 0x0400110A RID: 4362
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x0400110B RID: 4363
	public Transform followTarget;

	// Token: 0x0400110C RID: 4364
	[SerializeField]
	private float velocityStep = 1f;

	// Token: 0x0400110D RID: 4365
	private float currentSpeed;

	// Token: 0x0400110E RID: 4366
	[SerializeField]
	private float velocityIncreaseInterval = 20f;

	// Token: 0x0400110F RID: 4367
	public Vector3 noisyOffset;

	// Token: 0x04001110 RID: 4368
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04001111 RID: 4369
	private float emergeStartedTimestamp;

	// Token: 0x04001112 RID: 4370
	private float grabTimestamp;

	// Token: 0x04001113 RID: 4371
	private float lastSpeedIncreased;

	// Token: 0x04001114 RID: 4372
	[SerializeField]
	private float totalTimeToEmerge;

	// Token: 0x04001115 RID: 4373
	[SerializeField]
	private float catchDistance;

	// Token: 0x04001116 RID: 4374
	[SerializeField]
	private float grabDuration;

	// Token: 0x04001117 RID: 4375
	[SerializeField]
	private float grabSpeed = 1f;

	// Token: 0x04001118 RID: 4376
	[SerializeField]
	private float minGrabCooldown;

	// Token: 0x04001119 RID: 4377
	[SerializeField]
	private float initialRangeLimit;

	// Token: 0x0400111A RID: 4378
	[SerializeField]
	private float finalRangeLimit;

	// Token: 0x0400111B RID: 4379
	[SerializeField]
	private float rangeLimitBlendDuration;

	// Token: 0x0400111C RID: 4380
	[SerializeField]
	private float boredAfterDuration;

	// Token: 0x0400111D RID: 4381
	public NetPlayer targetPlayer;

	// Token: 0x0400111E RID: 4382
	public AngryBeeAnimator beeAnimator;

	// Token: 0x0400111F RID: 4383
	public AngryBeeSwarm.ChaseState currentState;

	// Token: 0x04001120 RID: 4384
	public AngryBeeSwarm.ChaseState lastState;

	// Token: 0x04001121 RID: 4385
	public NetPlayer grabbedPlayer;

	// Token: 0x04001122 RID: 4386
	private bool targetIsOnNavMesh;

	// Token: 0x04001123 RID: 4387
	private const float navMeshSampleRange = 5f;

	// Token: 0x04001124 RID: 4388
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04001125 RID: 4389
	public float hapticDuration = 1.5f;

	// Token: 0x04001126 RID: 4390
	public float MinHeightAboveWater = 0.5f;

	// Token: 0x04001127 RID: 4391
	public float PlayerMinHeightAboveWater = 0.5f;

	// Token: 0x04001128 RID: 4392
	public float RefreshClosestPlayerInterval = 1f;

	// Token: 0x04001129 RID: 4393
	private float NextRefreshClosestPlayerTimestamp = 1f;

	// Token: 0x0400112A RID: 4394
	private float BoredToDeathAtTimestamp = -1f;

	// Token: 0x0400112B RID: 4395
	[SerializeField]
	private Transform testEmergeFrom;

	// Token: 0x0400112C RID: 4396
	[SerializeField]
	private Transform testEmergeTo;

	// Token: 0x0400112D RID: 4397
	private Vector3 emergeFromPosition;

	// Token: 0x0400112E RID: 4398
	private Vector3 emergeToPosition;

	// Token: 0x0400112F RID: 4399
	private NavMeshPath path;

	// Token: 0x04001130 RID: 4400
	public List<Vector3> pathPoints;

	// Token: 0x04001131 RID: 4401
	public int currentPathPointIdx;

	// Token: 0x04001132 RID: 4402
	private float nextPathTimestamp;

	// Token: 0x04001133 RID: 4403
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 3)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private BeeSwarmData _Data;

	// Token: 0x0200021E RID: 542
	public enum ChaseState
	{
		// Token: 0x04001135 RID: 4405
		Dormant = 1,
		// Token: 0x04001136 RID: 4406
		InitialEmerge,
		// Token: 0x04001137 RID: 4407
		Chasing = 4,
		// Token: 0x04001138 RID: 4408
		Grabbing = 8
	}
}
