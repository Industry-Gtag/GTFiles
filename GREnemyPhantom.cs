using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007A6 RID: 1958
public class GREnemyPhantom : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x060031F7 RID: 12791 RVA: 0x00110888 File Offset: 0x0010EA88
	private void Awake()
	{
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		this.navAgent.updateRotation = false;
		this.behaviorStartTime = -1.0;
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
	}

	// Token: 0x060031F8 RID: 12792 RVA: 0x00110934 File Offset: 0x0010EB34
	public void OnEntityInit()
	{
		this.abilityMine.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityRage.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAlert.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityReturn.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		int num = (int)this.entity.createData;
		this.Setup((long)num);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry grbonusEntry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
	}

	// Token: 0x060031F9 RID: 12793 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060031FA RID: 12794 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060031FB RID: 12795 RVA: 0x00110BFC File Offset: 0x0010EDFC
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060031FC RID: 12796 RVA: 0x00110C2C File Offset: 0x0010EE2C
	private void Setup(long createData)
	{
		this.SetPatrolPath(createData);
		if (this.patrolPath != null && this.patrolPath.patrolNodes.Count > 0)
		{
			this.nextPatrolNode = 0;
			this.target = this.patrolPath.patrolNodes[0];
			this.idleLocation = this.target;
			this.SetBehavior(GREnemyPhantom.Behavior.Return, true);
		}
		else
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Mine, true);
		}
		this.SetBodyState(GREnemyPhantom.BodyState.Bones, true);
		if (this.attackLight != null)
		{
			this.attackLight.gameObject.SetActive(false);
		}
		if (this.negativeLight != null)
		{
			this.negativeLight.gameObject.SetActive(false);
		}
		GREnemy.HideRenderers(this.bones, false);
		GREnemy.HideRenderers(this.always, false);
	}

	// Token: 0x060031FD RID: 12797 RVA: 0x00110CFB File Offset: 0x0010EEFB
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPhantom.Behavior.Jump, false);
	}

	// Token: 0x060031FE RID: 12798 RVA: 0x00110D15 File Offset: 0x0010EF15
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 9)
		{
			return;
		}
		this.SetBehavior((GREnemyPhantom.Behavior)newState, false);
	}

	// Token: 0x060031FF RID: 12799 RVA: 0x00110D29 File Offset: 0x0010EF29
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 2)
		{
			return;
		}
		this.SetBodyState((GREnemyPhantom.BodyState)newState, false);
	}

	// Token: 0x06003200 RID: 12800 RVA: 0x00110D3C File Offset: 0x0010EF3C
	public void SetPatrolPath(long createData)
	{
		GRPatrolPath grpatrolPath = GhostReactorManager.Get(this.entity).reactor.GetPatrolPath(createData);
		this.patrolPath = grpatrolPath;
	}

	// Token: 0x06003201 RID: 12801 RVA: 0x00110D67 File Offset: 0x0010EF67
	public void SetNextPatrolNode(int nextPatrolNode)
	{
		this.nextPatrolNode = nextPatrolNode;
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x00110D70 File Offset: 0x0010EF70
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x00110D7C File Offset: 0x0010EF7C
	public void SetBehavior(GREnemyPhantom.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		this.lastStateChange = PhotonNetwork.Time;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Stop();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Stop();
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Stop();
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Stop();
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Stop();
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Stop();
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(false);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.Start();
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPhantom.Behavior.Alert:
			this.abilityAlert.Start();
			this.soundAlert.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.Start();
			this.soundReturn.Play(this.audioSource);
			this.abilityReturn.SetTarget(this.idleLocation);
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.Start();
			this.soundRage.Play(this.audioSource);
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.Start();
			this.soundChase.Play(this.audioSource);
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = null;
			if (this.negativeLight != null)
			{
				this.negativeLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			this.investigateLocation = null;
			this.soundAttack.Play(this.audioSource);
			if (this.attackLight != null)
			{
				this.attackLight.gameObject.SetActive(true);
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.Start();
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x00111064 File Offset: 0x0010F264
	public void SetBodyState(GREnemyPhantom.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyPhantom.BodyState.Bones)
		{
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x06003205 RID: 12805 RVA: 0x001110E0 File Offset: 0x0010F2E0
	private void RefreshBody()
	{
		GREnemyPhantom.BodyState bodyState = this.currBodyState;
		if (bodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			this.armor.SetHp(0);
			return;
		}
		if (bodyState != GREnemyPhantom.BodyState.Bones)
		{
			return;
		}
		this.armor.SetHp(0);
	}

	// Token: 0x06003206 RID: 12806 RVA: 0x00111115 File Offset: 0x0010F315
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003207 RID: 12807 RVA: 0x00111124 File Offset: 0x0010F324
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = null;
			this.SetBehavior(GREnemyPhantom.Behavior.Alert, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPhantom.Behavior.Investigate, false);
			return;
		}
		if (this.currBehavior == GREnemyPhantom.Behavior.Investigate)
		{
			if (this.idleLocation != null)
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			this.SetBehavior(GREnemyPhantom.Behavior.Idle, false);
		}
	}

	// Token: 0x06003208 RID: 12808 RVA: 0x001111D0 File Offset: 0x0010F3D0
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPhantom.tempRigs.Clear();
		GREnemyPhantom.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPhantom.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPhantom.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Alert:
		case GREnemyPhantom.Behavior.Rage:
		case GREnemyPhantom.Behavior.Attack:
			break;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.SetTarget(this.idleLocation);
			this.abilityReturn.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPhantom.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			break;
		default:
			return;
		}
	}

	// Token: 0x06003209 RID: 12809 RVA: 0x001112F6 File Offset: 0x0010F4F6
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x0600320A RID: 12810 RVA: 0x00111314 File Offset: 0x0010F514
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Mine:
			this.abilityMine.UpdateAuthority(dt);
			if (this.idleLocation != null)
			{
				GameAgent.UpdateFacingDir(base.transform, this.agent.navAgent, this.idleLocation.forward, 180f);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyPhantom.Behavior.Alert:
			this.UpdateAlert(dt);
			return;
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.UpdateAuthority(dt);
			if (this.abilityReturn.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Mine, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Rage:
			this.abilityRage.UpdateAuthority(dt);
			if (this.abilityRage.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPhantom.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.UpdateAuthority(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPhantom.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600320B RID: 12811 RVA: 0x001114C4 File Offset: 0x0010F6C4
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPhantom.Behavior.Return:
			this.abilityReturn.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Rage:
			break;
		case GREnemyPhantom.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPhantom.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x0011153C File Offset: 0x0010F73C
	public void UpdateAlert(float dt)
	{
		this.abilityAlert.SetTargetPlayer(this.agent.targetPlayer);
		this.abilityAlert.UpdateAuthority(dt);
		double timeAsDouble = Time.timeAsDouble;
		if (!this.senseNearby.IsAnyoneNearby())
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Return, false);
			return;
		}
		float num;
		if (this.abilityAlert.IsDone() && this.senseNearby.PickClosest(out num) != null)
		{
			this.SetBehavior(GREnemyPhantom.Behavior.Rage, false);
		}
	}

	// Token: 0x0600320D RID: 12813 RVA: 0x001115B4 File Offset: 0x0010F7B4
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyPhantom.BodyState.Destroyed)
		{
			return;
		}
		if (this.currBehavior != GREnemyPhantom.Behavior.Attack)
		{
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component = attachedRigidbody.GetComponent<GRPlayer>();
			if (component != null && component.gamePlayer.IsLocal())
			{
				GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Phantom, this.entity.id, component, base.transform.position);
			}
			GRBreakable component2 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component3 = attachedRigidbody.GetComponent<GameHittable>();
			if (component2 != null && component3 != null)
			{
				GameHitData gameHitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component3.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component2.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component2.transform.position,
					hittablePoint = component3.FindHittablePoint(collider)
				};
				component3.RequestHit(gameHitData);
			}
		}
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x001116CD File Offset: 0x0010F8CD
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x0600320F RID: 12815 RVA: 0x00111704 File Offset: 0x0010F904
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		byte b3 = (byte)this.nextPatrolNode;
		writer.Write(b);
		writer.Write(b2);
		writer.Write(this.hp);
		writer.Write(b3);
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x0011174C File Offset: 0x0010F94C
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPhantom.Behavior behavior = (GREnemyPhantom.Behavior)reader.ReadByte();
		GREnemyPhantom.BodyState bodyState = (GREnemyPhantom.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		byte b = reader.ReadByte();
		this.SetPatrolPath(this.entity.createData);
		this.SetNextPatrolNode((int)b);
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
	}

	// Token: 0x04004062 RID: 16482
	public GameEntity entity;

	// Token: 0x04004063 RID: 16483
	public GameAgent agent;

	// Token: 0x04004064 RID: 16484
	public GRArmorEnemy armor;

	// Token: 0x04004065 RID: 16485
	public GRAttributes attributes;

	// Token: 0x04004066 RID: 16486
	public Animation anim;

	// Token: 0x04004067 RID: 16487
	public GRSenseNearby senseNearby;

	// Token: 0x04004068 RID: 16488
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04004069 RID: 16489
	public GRAbilityIdle abilityMine;

	// Token: 0x0400406A RID: 16490
	public AbilitySound soundMine;

	// Token: 0x0400406B RID: 16491
	public GRAbilityIdle abilityIdle;

	// Token: 0x0400406C RID: 16492
	public GRAbilityWatch abilityRage;

	// Token: 0x0400406D RID: 16493
	public AbilitySound soundRage;

	// Token: 0x0400406E RID: 16494
	public GRAbilityWatch abilityAlert;

	// Token: 0x0400406F RID: 16495
	public AbilitySound soundAlert;

	// Token: 0x04004070 RID: 16496
	public GRAbilityChase abilityChase;

	// Token: 0x04004071 RID: 16497
	public AbilitySound soundChase;

	// Token: 0x04004072 RID: 16498
	public GRAbilityMoveToTarget abilityReturn;

	// Token: 0x04004073 RID: 16499
	public AbilitySound soundReturn;

	// Token: 0x04004074 RID: 16500
	public GRAbilityAttackLatchOn abilityAttack;

	// Token: 0x04004075 RID: 16501
	public AbilitySound soundAttack;

	// Token: 0x04004076 RID: 16502
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04004077 RID: 16503
	public GRAbilityJump abilityJump;

	// Token: 0x04004078 RID: 16504
	public List<Renderer> bones;

	// Token: 0x04004079 RID: 16505
	public List<Renderer> always;

	// Token: 0x0400407A RID: 16506
	public Transform coreMarker;

	// Token: 0x0400407B RID: 16507
	public GRCollectible corePrefab;

	// Token: 0x0400407C RID: 16508
	public Transform headTransform;

	// Token: 0x0400407D RID: 16509
	public float attackRange = 2f;

	// Token: 0x0400407E RID: 16510
	public float hearingRadius = 7f;

	// Token: 0x0400407F RID: 16511
	public List<VRRig> rigsNearby;

	// Token: 0x04004080 RID: 16512
	public GameLight attackLight;

	// Token: 0x04004081 RID: 16513
	public GameLight negativeLight;

	// Token: 0x04004082 RID: 16514
	[ReadOnly]
	[SerializeField]
	private GRPatrolPath patrolPath;

	// Token: 0x04004083 RID: 16515
	private Transform idleLocation;

	// Token: 0x04004084 RID: 16516
	public NavMeshAgent navAgent;

	// Token: 0x04004085 RID: 16517
	public AudioSource audioSource;

	// Token: 0x04004086 RID: 16518
	public double lastStateChange;

	// Token: 0x04004087 RID: 16519
	private Vector3? investigateLocation;

	// Token: 0x04004088 RID: 16520
	private Transform target;

	// Token: 0x04004089 RID: 16521
	[ReadOnly]
	public int hp;

	// Token: 0x0400408A RID: 16522
	[ReadOnly]
	public GREnemyPhantom.Behavior currBehavior;

	// Token: 0x0400408B RID: 16523
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x0400408C RID: 16524
	[ReadOnly]
	public GREnemyPhantom.BodyState currBodyState;

	// Token: 0x0400408D RID: 16525
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x0400408E RID: 16526
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x0400408F RID: 16527
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04004090 RID: 16528
	private Rigidbody rigidBody;

	// Token: 0x04004091 RID: 16529
	private List<Collider> colliders;

	// Token: 0x04004092 RID: 16530
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x020007A7 RID: 1959
	public enum Behavior
	{
		// Token: 0x04004094 RID: 16532
		Mine,
		// Token: 0x04004095 RID: 16533
		Idle,
		// Token: 0x04004096 RID: 16534
		Alert,
		// Token: 0x04004097 RID: 16535
		Return,
		// Token: 0x04004098 RID: 16536
		Rage,
		// Token: 0x04004099 RID: 16537
		Chase,
		// Token: 0x0400409A RID: 16538
		Attack,
		// Token: 0x0400409B RID: 16539
		Investigate,
		// Token: 0x0400409C RID: 16540
		Jump,
		// Token: 0x0400409D RID: 16541
		Count
	}

	// Token: 0x020007A8 RID: 1960
	public enum BodyState
	{
		// Token: 0x0400409F RID: 16543
		Destroyed,
		// Token: 0x040040A0 RID: 16544
		Bones,
		// Token: 0x040040A1 RID: 16545
		Count
	}
}
