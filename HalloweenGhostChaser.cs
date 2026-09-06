using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Fusion;
using Fusion.CodeGen;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020008F3 RID: 2291
[NetworkBehaviourWeaved(5)]
public class HalloweenGhostChaser : NetworkComponent
{
	// Token: 0x06003C0F RID: 15375 RVA: 0x00147CC7 File Offset: 0x00145EC7
	protected override void Awake()
	{
		base.Awake();
		this.spawnIndex = 0;
		this.targetPlayer = null;
		this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
		this.grabTime = -this.minGrabCooldown;
		this.possibleTarget = new List<NetPlayer>();
	}

	// Token: 0x06003C10 RID: 15376 RVA: 0x00147CFC File Offset: 0x00145EFC
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
		RoomSystem.JoinedRoomEvent += new Action(this.OnJoinedRoom);
	}

	// Token: 0x06003C11 RID: 15377 RVA: 0x00147D2C File Offset: 0x00145F2C
	private void InitializeGhost()
	{
		if (NetworkSystem.Instance.InRoom && base.IsMine)
		{
			this.lastHeadAngleTime = 0f;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Random.value * this.maxTimeToNextHeadAngle;
			this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			this.ghostBody.transform.localPosition = Vector3.zero;
			base.transform.eulerAngles = Vector3.zero;
			this.lastSpeedIncreased = 0f;
			this.currentSpeed = 0f;
		}
	}

	// Token: 0x06003C12 RID: 15378 RVA: 0x00147DCC File Offset: 0x00145FCC
	private void LateUpdate()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
			this.UpdateState();
			return;
		}
		if (base.IsMine)
		{
			HalloweenGhostChaser.ChaseState chaseState = this.currentState;
			switch (chaseState)
			{
			case HalloweenGhostChaser.ChaseState.Dormant:
				if (Time.time >= this.nextTimeToChasePlayer)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				if (Time.time >= this.lastSummonCheck + this.summoningDuration)
				{
					this.lastSummonCheck = Time.time;
					this.possibleTarget.Clear();
					int num = 0;
					int i = 0;
					while (i < this.spawnTransforms.Length)
					{
						int num2 = 0;
						for (int j = 0; j < VRRigCache.ActiveRigContainers.Count; j++)
						{
							if ((VRRigCache.ActiveRigContainers[j].transform.position - this.spawnTransforms[i].position).magnitude < this.summonDistance)
							{
								this.possibleTarget.Add(VRRigCache.ActiveRigContainers[j].Creator);
								num2++;
								if (num2 >= this.summonCount)
								{
									break;
								}
							}
						}
						if (num2 >= this.summonCount)
						{
							if (!this.wasSurroundedLastCheck)
							{
								this.wasSurroundedLastCheck = true;
								break;
							}
							this.wasSurroundedLastCheck = false;
							this.isSummoned = true;
							this.currentState = HalloweenGhostChaser.ChaseState.Gong;
							break;
						}
						else
						{
							num++;
							i++;
						}
					}
					if (num == this.spawnTransforms.Length)
					{
						this.wasSurroundedLastCheck = false;
					}
				}
				break;
			case HalloweenGhostChaser.ChaseState.InitialRise:
				if (Time.time > this.timeRiseStarted + this.totalTimeToRise)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.Chasing;
				}
				break;
			case (HalloweenGhostChaser.ChaseState)3:
				break;
			case HalloweenGhostChaser.ChaseState.Gong:
				if (Time.time > this.timeGongStarted + this.gongDuration)
				{
					this.currentState = HalloweenGhostChaser.ChaseState.InitialRise;
				}
				break;
			default:
				if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
				{
					if (chaseState == HalloweenGhostChaser.ChaseState.Grabbing)
					{
						if (Time.time > this.grabTime + this.grabDuration)
						{
							this.currentState = HalloweenGhostChaser.ChaseState.Dormant;
						}
					}
				}
				else
				{
					if (this.followTarget == null || this.targetPlayer == null)
					{
						this.ChooseRandomTarget();
					}
					if (!(this.followTarget == null) && (this.followTarget.position - this.ghostBody.transform.position).magnitude < this.catchDistance)
					{
						this.currentState = HalloweenGhostChaser.ChaseState.Grabbing;
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

	// Token: 0x06003C13 RID: 15379 RVA: 0x0014804C File Offset: 0x0014624C
	public void UpdateState()
	{
		HalloweenGhostChaser.ChaseState chaseState = this.currentState;
		switch (chaseState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			this.isSummoned = false;
			if (this.ghostMaterial.color == this.summonedColor)
			{
				this.ghostMaterial.color = this.defaultColor;
				return;
			}
			break;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.RiseHost();
				}
				this.MoveHead();
				return;
			}
			break;
		case (HalloweenGhostChaser.ChaseState)3:
		case HalloweenGhostChaser.ChaseState.Gong:
			break;
		default:
			if (chaseState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (chaseState != HalloweenGhostChaser.ChaseState.Grabbing)
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
					this.MoveHead();
				}
			}
			else if (NetworkSystem.Instance.InRoom)
			{
				if (base.IsMine)
				{
					this.ChaseHost();
				}
				this.MoveBodyShared();
				this.MoveHead();
				return;
			}
			break;
		}
	}

	// Token: 0x06003C14 RID: 15380 RVA: 0x00148130 File Offset: 0x00146330
	private void OnChangeState(HalloweenGhostChaser.ChaseState newState)
	{
		switch (newState)
		{
		case HalloweenGhostChaser.ChaseState.Dormant:
			if (this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(false);
			}
			if (base.IsMine)
			{
				this.targetPlayer = null;
				this.InitializeGhost();
			}
			else
			{
				this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
			}
			this.SetInitialRotations();
			return;
		case HalloweenGhostChaser.ChaseState.InitialRise:
			this.timeRiseStarted = Time.time;
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				if (!this.isSummoned)
				{
					this.currentSpeed = 0f;
					this.ChooseRandomTarget();
					this.SetInitialSpawnPoint();
				}
				else
				{
					this.currentSpeed = 3f;
				}
			}
			if (this.isSummoned)
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				this.ghostMaterial.color = this.summonedColor;
			}
			else
			{
				this.laugh.volume = 0.25f;
				this.laugh.GTPlay();
				this.ghostMaterial.color = this.defaultColor;
			}
			this.SetInitialRotations();
			return;
		case (HalloweenGhostChaser.ChaseState)3:
			break;
		case HalloweenGhostChaser.ChaseState.Gong:
			if (!this.ghostBody.activeSelf)
			{
				this.ghostBody.SetActive(true);
			}
			if (base.IsMine)
			{
				this.ChooseRandomTarget();
				this.SetInitialSpawnPoint();
				base.transform.position = this.spawnTransforms[this.spawnIndex].position;
			}
			this.timeGongStarted = Time.time;
			this.laugh.volume = 1f;
			this.laugh.GTPlayOneShot(this.gong, 1f);
			this.isSummoned = true;
			return;
		default:
			if (newState != HalloweenGhostChaser.ChaseState.Chasing)
			{
				if (newState != HalloweenGhostChaser.ChaseState.Grabbing)
				{
					return;
				}
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.grabTime = Time.time;
				if (this.isSummoned)
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlayOneShot(this.deepLaugh, 1f);
				}
				else
				{
					this.laugh.volume = 0.25f;
					this.laugh.GTPlay();
				}
				this.leftArm.localEulerAngles = this.leftArmGrabbingLocal;
				this.rightArm.localEulerAngles = this.rightArmGrabbingLocal;
				this.leftHand.localEulerAngles = this.leftHandGrabbingLocal;
				this.rightHand.localEulerAngles = this.rightHandGrabbingLocal;
				this.ghostBody.transform.localPosition = this.ghostOffsetGrabbingLocal;
				this.ghostBody.transform.localEulerAngles = this.ghostGrabbingEulerRotation;
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(this.targetPlayer);
				if (vrrig != null)
				{
					this.followTarget = vrrig.transform;
					return;
				}
			}
			else
			{
				if (!this.ghostBody.activeSelf)
				{
					this.ghostBody.SetActive(true);
				}
				this.ResetPath();
			}
			break;
		}
	}

	// Token: 0x06003C15 RID: 15381 RVA: 0x00148428 File Offset: 0x00146628
	private void SetInitialSpawnPoint()
	{
		float num = 1000f;
		this.spawnIndex = 0;
		if (this.followTarget == null)
		{
			return;
		}
		for (int i = 0; i < this.spawnTransforms.Length; i++)
		{
			float magnitude = (this.followTarget.position - this.spawnTransformOffsets[i].position).magnitude;
			if (magnitude < num)
			{
				num = magnitude;
				this.spawnIndex = i;
			}
		}
	}

	// Token: 0x06003C16 RID: 15382 RVA: 0x00148498 File Offset: 0x00146698
	private void ChooseRandomTarget()
	{
		int num = -1;
		if (this.possibleTarget.Count >= this.summonCount)
		{
			int randomTarget = Random.Range(0, this.possibleTarget.Count);
			num = VRRigCache.ActiveRigContainers.FindIndex((RigContainer x) => x.Creator != null && x.Creator == this.possibleTarget[randomTarget]);
			this.currentSpeed = 3f;
		}
		if (num == -1)
		{
			num = Random.Range(0, VRRigCache.ActiveRigContainers.Count);
		}
		this.possibleTarget.Clear();
		if (num < VRRigCache.ActiveRigContainers.Count)
		{
			VRRig rig = VRRigCache.ActiveRigContainers[num].Rig;
			this.targetPlayer = rig.creator;
			this.followTarget = rig.head.rigTarget;
			NavMeshHit navMeshHit;
			this.targetIsOnNavMesh = NavMesh.SamplePosition(this.followTarget.position, out navMeshHit, 5f, 1);
			return;
		}
		this.targetPlayer = null;
		this.followTarget = null;
	}

	// Token: 0x06003C17 RID: 15383 RVA: 0x00148588 File Offset: 0x00146788
	private void SetInitialRotations()
	{
		this.leftArm.localEulerAngles = Vector3.zero;
		this.rightArm.localEulerAngles = Vector3.zero;
		this.leftHand.localEulerAngles = this.leftHandStartingLocal;
		this.rightHand.localEulerAngles = this.rightHandStartingLocal;
		this.ghostBody.transform.localPosition = Vector3.zero;
		this.ghostBody.transform.localEulerAngles = this.ghostStartingEulerRotation;
	}

	// Token: 0x06003C18 RID: 15384 RVA: 0x00148604 File Offset: 0x00146804
	private void MoveHead()
	{
		if (Time.time > this.nextHeadAngleTime)
		{
			this.skullTransform.localEulerAngles = this.headEulerAngles[Random.Range(0, this.headEulerAngles.Length)];
			this.lastHeadAngleTime = Time.time;
			this.nextHeadAngleTime = this.lastHeadAngleTime + Mathf.Max(Random.value * this.maxTimeToNextHeadAngle, 0.05f);
		}
	}

	// Token: 0x06003C19 RID: 15385 RVA: 0x00148670 File Offset: 0x00146870
	private void RiseHost()
	{
		if (Time.time < this.timeRiseStarted + this.totalTimeToRise)
		{
			if (this.spawnIndex == -1)
			{
				this.spawnIndex = 0;
			}
			base.transform.position = this.spawnTransforms[this.spawnIndex].position + Vector3.up * (Time.time - this.timeRiseStarted) / this.totalTimeToRise * this.riseDistance;
			base.transform.rotation = this.spawnTransforms[this.spawnIndex].rotation;
		}
	}

	// Token: 0x06003C1A RID: 15386 RVA: 0x0014870C File Offset: 0x0014690C
	private void RiseGrabbedLocalPlayer()
	{
		if (Time.time > this.grabTime + this.minGrabCooldown)
		{
			this.grabTime = Time.time;
			GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, GorillaTagger.Instance.tagCooldown);
			GorillaTagger.Instance.StartVibration(true, this.hapticStrength, this.hapticDuration);
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}
		if (Time.time < this.grabTime + this.grabDuration)
		{
			GorillaTagger.Instance.rigidbody.linearVelocity = Vector3.up * this.grabSpeed;
			EquipmentInteractor.instance.ForceStopClimbing();
		}
	}

	// Token: 0x06003C1B RID: 15387 RVA: 0x001487BC File Offset: 0x001469BC
	public void UpdateFollowPath(Vector3 destination, float currentSpeed)
	{
		if (this.path == null)
		{
			this.GetNewPath(destination);
		}
		this.points[this.points.Count - 1] = destination;
		Vector3 vector = this.points[this.currentTargetIdx];
		base.transform.position = Vector3.MoveTowards(base.transform.position, vector, currentSpeed * Time.deltaTime);
		Vector3 eulerAngles = Quaternion.LookRotation(vector - base.transform.position).eulerAngles;
		if (Mathf.Abs(eulerAngles.x) > 45f)
		{
			eulerAngles.x = 0f;
		}
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		if (this.currentTargetIdx + 1 < this.points.Count && (base.transform.position - vector).sqrMagnitude < 0.1f)
		{
			if (this.nextPathTimestamp <= Time.time)
			{
				this.GetNewPath(destination);
				return;
			}
			this.currentTargetIdx++;
		}
	}

	// Token: 0x06003C1C RID: 15388 RVA: 0x001488CC File Offset: 0x00146ACC
	private void GetNewPath(Vector3 destination)
	{
		this.path = new NavMeshPath();
		NavMeshHit navMeshHit;
		NavMesh.SamplePosition(base.transform.position, out navMeshHit, 5f, 1);
		NavMeshHit navMeshHit2;
		this.targetIsOnNavMesh = NavMesh.SamplePosition(destination, out navMeshHit2, 5f, 1);
		NavMesh.CalculatePath(navMeshHit.position, navMeshHit2.position, -1, this.path);
		this.points = new List<Vector3>();
		foreach (Vector3 vector in this.path.corners)
		{
			this.points.Add(vector + Vector3.up * this.heightAboveNavmesh);
		}
		this.points.Add(destination);
		this.currentTargetIdx = 0;
		this.nextPathTimestamp = Time.time + 2f;
	}

	// Token: 0x06003C1D RID: 15389 RVA: 0x001489A0 File Offset: 0x00146BA0
	public void ResetPath()
	{
		this.path = null;
	}

	// Token: 0x06003C1E RID: 15390 RVA: 0x001489AC File Offset: 0x00146BAC
	private void ChaseHost()
	{
		if (this.followTarget != null)
		{
			if (Time.time > this.lastSpeedIncreased + this.velocityIncreaseTime)
			{
				this.lastSpeedIncreased = Time.time;
				this.currentSpeed += this.velocityStep;
			}
			if (this.targetIsOnNavMesh)
			{
				this.UpdateFollowPath(this.followTarget.position, this.currentSpeed);
				return;
			}
			base.transform.position = Vector3.MoveTowards(base.transform.position, this.followTarget.position, this.currentSpeed * Time.deltaTime);
			base.transform.rotation = Quaternion.LookRotation(this.followTarget.position - base.transform.position, Vector3.up);
		}
	}

	// Token: 0x06003C1F RID: 15391 RVA: 0x00148A80 File Offset: 0x00146C80
	private void MoveBodyShared()
	{
		this.noisyOffset = new Vector3(Mathf.PerlinNoise(Time.time, 0f) - 0.5f, Mathf.PerlinNoise(Time.time, 10f) - 0.5f, Mathf.PerlinNoise(Time.time, 20f) - 0.5f);
		this.childGhost.localPosition = this.noisyOffset;
		this.leftArm.localEulerAngles = this.noisyOffset * 20f;
		this.rightArm.localEulerAngles = this.noisyOffset * -20f;
	}

	// Token: 0x06003C20 RID: 15392 RVA: 0x00148B1E File Offset: 0x00146D1E
	private void GrabBodyShared()
	{
		if (this.followTarget != null)
		{
			base.transform.rotation = this.followTarget.rotation;
			base.transform.position = this.followTarget.position;
		}
	}

	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x06003C21 RID: 15393 RVA: 0x00148B5A File Offset: 0x00146D5A
	// (set) Token: 0x06003C22 RID: 15394 RVA: 0x00148B84 File Offset: 0x00146D84
	[Networked]
	[NetworkedWeaved(0, 5)]
	public unsafe HalloweenGhostChaser.GhostData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(HalloweenGhostChaser.GhostData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing HalloweenGhostChaser.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(HalloweenGhostChaser.GhostData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06003C23 RID: 15395 RVA: 0x00148BB0 File Offset: 0x00146DB0
	public override void WriteDataFusion()
	{
		HalloweenGhostChaser.GhostData ghostData = default(HalloweenGhostChaser.GhostData);
		NetPlayer netPlayer = this.targetPlayer;
		ghostData.TargetActorNumber = ((netPlayer != null) ? netPlayer.ActorNumber : (-1));
		ghostData.CurrentState = (int)this.currentState;
		ghostData.SpawnIndex = this.spawnIndex;
		ghostData.CurrentSpeed = this.currentSpeed;
		ghostData.IsSummoned = this.isSummoned;
		this.Data = ghostData;
	}

	// Token: 0x06003C24 RID: 15396 RVA: 0x00148C20 File Offset: 0x00146E20
	public override void ReadDataFusion()
	{
		int targetActorNumber = this.Data.TargetActorNumber;
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(targetActorNumber);
		this.currentState = (HalloweenGhostChaser.ChaseState)this.Data.CurrentState;
		this.spawnIndex = this.Data.SpawnIndex;
		float num = this.Data.CurrentSpeed;
		this.isSummoned = this.Data.IsSummoned;
		if (float.IsFinite(num))
		{
			this.currentSpeed = num;
		}
	}

	// Token: 0x06003C25 RID: 15397 RVA: 0x00148CA0 File Offset: 0x00146EA0
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		if (this.targetPlayer == null)
		{
			stream.SendNext(-1);
		}
		else
		{
			stream.SendNext(this.targetPlayer.ActorNumber);
		}
		stream.SendNext(this.currentState);
		stream.SendNext(this.spawnIndex);
		stream.SendNext(this.currentSpeed);
		stream.SendNext(this.isSummoned);
	}

	// Token: 0x06003C26 RID: 15398 RVA: 0x00148D3C File Offset: 0x00146F3C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (NetworkSystem.Instance.GetPlayer(info.Sender) != NetworkSystem.Instance.MasterClient)
		{
			return;
		}
		int num = (int)stream.ReceiveNext();
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num);
		this.currentState = (HalloweenGhostChaser.ChaseState)stream.ReceiveNext();
		this.spawnIndex = (int)stream.ReceiveNext();
		float num2 = (float)stream.ReceiveNext();
		this.isSummoned = (bool)stream.ReceiveNext();
		if (float.IsFinite(num2))
		{
			this.currentSpeed = num2;
		}
	}

	// Token: 0x06003C27 RID: 15399 RVA: 0x00148DD1 File Offset: 0x00146FD1
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.OnChangeState(this.currentState);
		}
	}

	// Token: 0x06003C28 RID: 15400 RVA: 0x00148DEF File Offset: 0x00146FEF
	public void OnJoinedRoom()
	{
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.InitializeGhost();
			return;
		}
		this.nextTimeToChasePlayer = Time.time + Random.Range(this.minGrabCooldown, this.maxNextTimeToChasePlayer);
	}

	// Token: 0x06003C2A RID: 15402 RVA: 0x00148EB7 File Offset: 0x001470B7
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06003C2B RID: 15403 RVA: 0x00148ECF File Offset: 0x001470CF
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04004C82 RID: 19586
	public float heightAboveNavmesh = 0.5f;

	// Token: 0x04004C83 RID: 19587
	public Transform followTarget;

	// Token: 0x04004C84 RID: 19588
	public Transform childGhost;

	// Token: 0x04004C85 RID: 19589
	public float velocityStep = 1f;

	// Token: 0x04004C86 RID: 19590
	public float currentSpeed;

	// Token: 0x04004C87 RID: 19591
	public float velocityIncreaseTime = 20f;

	// Token: 0x04004C88 RID: 19592
	public float riseDistance = 2f;

	// Token: 0x04004C89 RID: 19593
	public float summonDistance = 5f;

	// Token: 0x04004C8A RID: 19594
	public float timeEncircled;

	// Token: 0x04004C8B RID: 19595
	public float lastSummonCheck;

	// Token: 0x04004C8C RID: 19596
	public float timeGongStarted;

	// Token: 0x04004C8D RID: 19597
	public float summoningDuration = 30f;

	// Token: 0x04004C8E RID: 19598
	public float summoningCheckCountdown = 5f;

	// Token: 0x04004C8F RID: 19599
	public float gongDuration = 5f;

	// Token: 0x04004C90 RID: 19600
	public int summonCount = 5;

	// Token: 0x04004C91 RID: 19601
	public bool wasSurroundedLastCheck;

	// Token: 0x04004C92 RID: 19602
	public AudioSource laugh;

	// Token: 0x04004C93 RID: 19603
	public List<NetPlayer> possibleTarget;

	// Token: 0x04004C94 RID: 19604
	public AudioClip defaultLaugh;

	// Token: 0x04004C95 RID: 19605
	public AudioClip deepLaugh;

	// Token: 0x04004C96 RID: 19606
	public AudioClip gong;

	// Token: 0x04004C97 RID: 19607
	public Vector3 noisyOffset;

	// Token: 0x04004C98 RID: 19608
	public Vector3 leftArmGrabbingLocal;

	// Token: 0x04004C99 RID: 19609
	public Vector3 rightArmGrabbingLocal;

	// Token: 0x04004C9A RID: 19610
	public Vector3 leftHandGrabbingLocal;

	// Token: 0x04004C9B RID: 19611
	public Vector3 rightHandGrabbingLocal;

	// Token: 0x04004C9C RID: 19612
	public Vector3 leftHandStartingLocal;

	// Token: 0x04004C9D RID: 19613
	public Vector3 rightHandStartingLocal;

	// Token: 0x04004C9E RID: 19614
	public Vector3 ghostOffsetGrabbingLocal;

	// Token: 0x04004C9F RID: 19615
	public Vector3 ghostStartingEulerRotation;

	// Token: 0x04004CA0 RID: 19616
	public Vector3 ghostGrabbingEulerRotation;

	// Token: 0x04004CA1 RID: 19617
	public float maxTimeToNextHeadAngle;

	// Token: 0x04004CA2 RID: 19618
	public float lastHeadAngleTime;

	// Token: 0x04004CA3 RID: 19619
	public float nextHeadAngleTime;

	// Token: 0x04004CA4 RID: 19620
	public float nextTimeToChasePlayer;

	// Token: 0x04004CA5 RID: 19621
	public float maxNextTimeToChasePlayer;

	// Token: 0x04004CA6 RID: 19622
	public float timeRiseStarted;

	// Token: 0x04004CA7 RID: 19623
	public float totalTimeToRise;

	// Token: 0x04004CA8 RID: 19624
	public float catchDistance;

	// Token: 0x04004CA9 RID: 19625
	public float grabTime;

	// Token: 0x04004CAA RID: 19626
	public float grabDuration;

	// Token: 0x04004CAB RID: 19627
	public float grabSpeed = 1f;

	// Token: 0x04004CAC RID: 19628
	public float minGrabCooldown;

	// Token: 0x04004CAD RID: 19629
	public float lastSpeedIncreased;

	// Token: 0x04004CAE RID: 19630
	public Vector3[] headEulerAngles;

	// Token: 0x04004CAF RID: 19631
	public Transform skullTransform;

	// Token: 0x04004CB0 RID: 19632
	public Transform leftArm;

	// Token: 0x04004CB1 RID: 19633
	public Transform rightArm;

	// Token: 0x04004CB2 RID: 19634
	public Transform leftHand;

	// Token: 0x04004CB3 RID: 19635
	public Transform rightHand;

	// Token: 0x04004CB4 RID: 19636
	public Transform[] spawnTransforms;

	// Token: 0x04004CB5 RID: 19637
	public Transform[] spawnTransformOffsets;

	// Token: 0x04004CB6 RID: 19638
	public NetPlayer targetPlayer;

	// Token: 0x04004CB7 RID: 19639
	public GameObject ghostBody;

	// Token: 0x04004CB8 RID: 19640
	public HalloweenGhostChaser.ChaseState currentState;

	// Token: 0x04004CB9 RID: 19641
	public HalloweenGhostChaser.ChaseState lastState;

	// Token: 0x04004CBA RID: 19642
	public int spawnIndex;

	// Token: 0x04004CBB RID: 19643
	public NetPlayer grabbedPlayer;

	// Token: 0x04004CBC RID: 19644
	public Material ghostMaterial;

	// Token: 0x04004CBD RID: 19645
	public Color defaultColor;

	// Token: 0x04004CBE RID: 19646
	public Color summonedColor;

	// Token: 0x04004CBF RID: 19647
	public bool isSummoned;

	// Token: 0x04004CC0 RID: 19648
	private bool targetIsOnNavMesh;

	// Token: 0x04004CC1 RID: 19649
	private const float navMeshSampleRange = 5f;

	// Token: 0x04004CC2 RID: 19650
	[Tooltip("Haptic vibration when chased by lucy")]
	public float hapticStrength = 1f;

	// Token: 0x04004CC3 RID: 19651
	public float hapticDuration = 1.5f;

	// Token: 0x04004CC4 RID: 19652
	private NavMeshPath path;

	// Token: 0x04004CC5 RID: 19653
	public List<Vector3> points;

	// Token: 0x04004CC6 RID: 19654
	public int currentTargetIdx;

	// Token: 0x04004CC7 RID: 19655
	private float nextPathTimestamp;

	// Token: 0x04004CC8 RID: 19656
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 5)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private HalloweenGhostChaser.GhostData _Data;

	// Token: 0x020008F4 RID: 2292
	public enum ChaseState
	{
		// Token: 0x04004CCA RID: 19658
		Dormant = 1,
		// Token: 0x04004CCB RID: 19659
		InitialRise,
		// Token: 0x04004CCC RID: 19660
		Gong = 4,
		// Token: 0x04004CCD RID: 19661
		Chasing = 8,
		// Token: 0x04004CCE RID: 19662
		Grabbing = 16
	}

	// Token: 0x020008F5 RID: 2293
	[NetworkStructWeaved(5)]
	[StructLayout(LayoutKind.Explicit, Size = 20)]
	public struct GhostData : INetworkStruct
	{
		// Token: 0x17000574 RID: 1396
		// (get) Token: 0x06003C2C RID: 15404 RVA: 0x00148EE3 File Offset: 0x001470E3
		// (set) Token: 0x06003C2D RID: 15405 RVA: 0x00148EF1 File Offset: 0x001470F1
		[Networked]
		[NetworkedWeaved(3, 1)]
		public unsafe float CurrentSpeed
		{
			readonly get
			{
				return *(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed);
			}
			set
			{
				*(float*)Native.ReferenceToPointer<FixedStorage@1>(ref this._CurrentSpeed) = value;
			}
		}

		// Token: 0x04004CCF RID: 19663
		[FieldOffset(0)]
		public int TargetActorNumber;

		// Token: 0x04004CD0 RID: 19664
		[FieldOffset(4)]
		public int CurrentState;

		// Token: 0x04004CD1 RID: 19665
		[FieldOffset(8)]
		public int SpawnIndex;

		// Token: 0x04004CD2 RID: 19666
		[FixedBufferProperty(typeof(float), typeof(UnityValueSurrogate@ElementReaderWriterSingle), 0, order = -2147483647)]
		[WeaverGenerated]
		[SerializeField]
		[FieldOffset(12)]
		private FixedStorage@1 _CurrentSpeed;

		// Token: 0x04004CD3 RID: 19667
		[FieldOffset(16)]
		public NetworkBool IsSummoned;
	}
}
