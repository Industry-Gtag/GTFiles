using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020007A2 RID: 1954
public class GREnemyPest : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, ITickSystemTick
{
	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x060031CB RID: 12747 RVA: 0x0010F613 File Offset: 0x0010D813
	// (set) Token: 0x060031CC RID: 12748 RVA: 0x0010F61B File Offset: 0x0010D81B
	public bool TickRunning { get; set; }

	// Token: 0x060031CD RID: 12749 RVA: 0x0010F624 File Offset: 0x0010D824
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
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.senseNearby.Setup(this.headTransform, this.entity);
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
		base.Invoke("PlaySpawnAudio", 0.1f);
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x0001A297 File Offset: 0x00018497
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x0001A29F File Offset: 0x0001849F
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x0010F716 File Offset: 0x0010D916
	private void PlaySpawnAudio()
	{
		this.spawnSound.Play(null);
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x0010F724 File Offset: 0x0010D924
	public void OnEntityInit()
	{
		this.abilityIdle.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityChase.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityAttack.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityWander.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityDie.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityGrabbed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityThrown.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityStagger.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityFlashed.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityInvestigate.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.abilityJump.Setup(this.agent, this.anim, this.audioSource, base.transform, this.headTransform, this.senseLineOfSight);
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry grbonusEntry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
		}
		this.navAgent.speed = this.attributes.CalculateFinalFloatValueForAttribute(GRAttributeType.PatrolSpeed);
		this.SetHP(this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax));
		this.agent.navAgent.autoTraverseOffMeshLink = false;
		this.agent.onJumpRequested += this.OnAgentJumpRequested;
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyPest.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyPest.BodyState.Bones, true);
	}

	// Token: 0x060031D2 RID: 12754 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060031D3 RID: 12755 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060031D4 RID: 12756 RVA: 0x0010FA70 File Offset: 0x0010DC70
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060031D5 RID: 12757 RVA: 0x0010FA89 File Offset: 0x0010DC89
	private void OnAgentJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		this.abilityJump.SetupJump(start, end, heightScale, speedScale);
		this.SetBehavior(GREnemyPest.Behavior.Jump, false);
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x0010FAA4 File Offset: 0x0010DCA4
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 11)
		{
			return;
		}
		this.SetBehavior((GREnemyPest.Behavior)newState, false);
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x0010FAB8 File Offset: 0x0010DCB8
	public void SetHP(int hp)
	{
		this.hp = hp;
	}

	// Token: 0x060031D8 RID: 12760 RVA: 0x0010FAC1 File Offset: 0x0010DCC1
	public bool TrySetBehavior(GREnemyPest.Behavior newBehavior)
	{
		if (this.currBehavior == GREnemyPest.Behavior.Jump && newBehavior == GREnemyPest.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060031D9 RID: 12761 RVA: 0x0010FADC File Offset: 0x0010DCDC
	public void SetBehavior(GREnemyPest.Behavior newBehavior, bool force = false)
	{
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Stop();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Stop();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Stop();
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Stop();
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Stop();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Stop();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Stop();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Stop();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Stop();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Stop();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Stop();
			break;
		}
		this.currBehavior = newBehavior;
		this.behaviorStartTime = Time.timeAsDouble;
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.Start();
			break;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Start();
			break;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.Start();
			this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.Start();
			this.abilityAttack.SetTargetPlayer(this.agent.targetPlayer);
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.Start();
			break;
		case GREnemyPest.Behavior.Grabbed:
			this.abilityGrabbed.Start();
			break;
		case GREnemyPest.Behavior.Thrown:
			this.abilityThrown.Start();
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.Start();
			break;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.Start();
			break;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.Start();
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.Start();
			break;
		}
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060031DA RID: 12762 RVA: 0x0010FCF2 File Offset: 0x0010DEF2
	private void OnGrabbed()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Grabbed, false);
	}

	// Token: 0x060031DB RID: 12763 RVA: 0x0010FD06 File Offset: 0x0010DF06
	private void OnReleased()
	{
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Thrown, false);
	}

	// Token: 0x060031DC RID: 12764 RVA: 0x0010FD1A File Offset: 0x0010DF1A
	public void Tick()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x060031DD RID: 12765 RVA: 0x0010FD28 File Offset: 0x0010DF28
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyPest.tempRigs.Clear();
		GREnemyPest.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyPest.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyPest.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		GREnemyPest.Behavior behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.Think(dt);
			this.ChooseNewBehavior();
			return;
		case GREnemyPest.Behavior.Chase:
			if (this.agent.targetPlayer != null)
			{
				this.abilityChase.SetTargetPlayer(this.agent.targetPlayer);
			}
			this.abilityChase.Think(dt);
			return;
		default:
			if (behavior != GREnemyPest.Behavior.Investigate)
			{
				return;
			}
			this.abilityInvestigate.Think(dt);
			this.ChooseNewBehavior();
			return;
		}
	}

	// Token: 0x060031DE RID: 12766 RVA: 0x0010FE28 File Offset: 0x0010E028
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.senseNearby.IsAnyoneNearby())
		{
			this.investigateLocation = null;
			this.SetBehavior(GREnemyPest.Behavior.Chase, false);
			return;
		}
		this.investigateLocation = AbilityHelperFunctions.GetLocationToInvestigate(base.transform.position, this.hearingRadius, this.investigateLocation);
		if (this.investigateLocation != null)
		{
			this.abilityInvestigate.SetTargetPos(this.investigateLocation.Value);
			this.SetBehavior(GREnemyPest.Behavior.Investigate, false);
			return;
		}
		this.SetBehavior(GREnemyPest.Behavior.Wander, false);
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x0010FEB4 File Offset: 0x0010E0B4
	public void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x0010FED4 File Offset: 0x0010E0D4
	public void OnUpdateAuthority(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Idle:
			this.abilityIdle.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Chase:
		{
			this.abilityChase.UpdateAuthority(dt);
			if (this.abilityChase.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			GRPlayer grplayer = GRPlayer.Get(this.agent.targetPlayer);
			if (grplayer != null)
			{
				float num = this.attackRange * this.attackRange;
				if ((grplayer.transform.position - base.transform.position).sqrMagnitude < num)
				{
					this.SetBehavior(GREnemyPest.Behavior.Attack, false);
					return;
				}
			}
			break;
		}
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.UpdateAuthority(dt);
			if (this.abilityAttack.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Chase, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.UpdateAuthority(dt);
			if (this.abilityStagger.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Grabbed:
			break;
		case GREnemyPest.Behavior.Thrown:
			if (this.abilityThrown.IsDone())
			{
				this.SetBehavior(GREnemyPest.Behavior.Wander, false);
				return;
			}
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.UpdateAuthority(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.UpdateAuthority(dt);
			if (this.abilityJump.IsDone())
			{
				this.ChooseNewBehavior();
				return;
			}
			break;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.UpdateAuthority(dt);
			if (this.abilityFlashed.IsDone())
			{
				this.ChooseNewBehavior();
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x00110070 File Offset: 0x0010E270
	public void OnUpdateRemote(float dt)
	{
		switch (this.currBehavior)
		{
		case GREnemyPest.Behavior.Wander:
			this.abilityWander.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Chase:
			this.abilityChase.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Attack:
			this.abilityAttack.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Stagger:
			this.abilityStagger.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Grabbed:
		case GREnemyPest.Behavior.Thrown:
			break;
		case GREnemyPest.Behavior.Destroyed:
			this.abilityDie.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Investigate:
			this.abilityInvestigate.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Jump:
			this.abilityJump.UpdateRemote(dt);
			return;
		case GREnemyPest.Behavior.Flashed:
			this.abilityFlashed.UpdateRemote(dt);
			break;
		default:
			return;
		}
	}

	// Token: 0x060031E2 RID: 12770 RVA: 0x0011011C File Offset: 0x0010E31C
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		writer.Write(b);
		writer.Write(this.hp);
		writer.Write(b2);
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x00110154 File Offset: 0x0010E354
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyPest.Behavior behavior = (GREnemyPest.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		GREnemyPest.BodyState bodyState = (GREnemyPest.BodyState)reader.ReadByte();
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x00110190 File Offset: 0x0010E390
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x060031E6 RID: 12774 RVA: 0x001101F0 File Offset: 0x0010E3F0
	private void OnHitByClub(GameHitData hit)
	{
		if (this.currBodyState != GREnemyPest.BodyState.Bones)
		{
			if (this.currBodyState == GREnemyPest.BodyState.Shell && this.armor != null)
			{
				this.armor.PlayBlockFx(hit.hitEntityPosition);
			}
			return;
		}
		if (this.currBehavior == GREnemyPest.Behavior.Destroyed)
		{
			return;
		}
		this.hp -= hit.hitAmount;
		if (this.hp <= 0)
		{
			this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hit.hitByEntityId));
			this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
			return;
		}
		this.abilityStagger.SetStaggerVelocity(hit.hitImpulse);
		this.TrySetBehavior(GREnemyPest.Behavior.Stagger);
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x00110293 File Offset: 0x0010E493
	public void InstantDeath()
	{
		this.hp = 0;
		this.SetBehavior(GREnemyPest.Behavior.Destroyed, false);
	}

	// Token: 0x060031E8 RID: 12776 RVA: 0x001102A4 File Offset: 0x0010E4A4
	private void OnHitByFlash(GRTool tool, GameHitData hit)
	{
		this.abilityFlashed.SetStaggerVelocity(hit.hitImpulse);
		if (this.currBodyState == GREnemyPest.BodyState.Shell)
		{
			this.hp -= hit.hitAmount;
			if (this.armor != null)
			{
				this.armor.SetHp(this.hp);
			}
			if (this.hp <= 0)
			{
				if (this.armor != null)
				{
					this.armor.PlayDestroyFx(this.armor.transform.position);
				}
				this.SetBodyState(GREnemyPest.BodyState.Bones, false);
				if (tool.gameEntity.IsHeldByLocalPlayer())
				{
					PlayerGameEvents.MiscEvent("GRArmorBreak_" + base.name, 1);
				}
				if (tool.HasUpgradeInstalled(GRToolProgressionManager.ToolParts.FlashDamage3))
				{
					this.armor.FragmentArmor();
				}
			}
			else
			{
				if (this.armor != null)
				{
					this.armor.PlayHitFx(this.armor.transform.position);
				}
				this.RefreshBody();
			}
		}
		GRToolFlash component = tool.GetComponent<GRToolFlash>();
		if (component != null)
		{
			this.abilityFlashed.SetStunTime(component.stunDuration);
		}
		this.TrySetBehavior(GREnemyPest.Behavior.Flashed);
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x001103CC File Offset: 0x0010E5CC
	private void OnHitByShield(GameHitData hit)
	{
		this.OnHitByClub(hit);
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x001103D8 File Offset: 0x0010E5D8
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyPest.Behavior.Attack)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			Vector3 vector = this.abilityAttack.targetPos - this.abilityAttack.initialPos;
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(base.transform.position, vector, component2);
			return;
		}
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			GRPlayer component3 = attachedRigidbody.GetComponent<GRPlayer>();
			if (component3 != null && component3.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				if (this.tryHitPlayerCoroutine != null)
				{
					base.StopCoroutine(this.tryHitPlayerCoroutine);
				}
				this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(component3));
			}
			GRBreakable component4 = attachedRigidbody.GetComponent<GRBreakable>();
			GameHittable component5 = attachedRigidbody.GetComponent<GameHittable>();
			if (component4 != null && component5 != null)
			{
				GameHitData gameHitData = new GameHitData
				{
					hitTypeId = 0,
					hitEntityId = component5.gameEntity.id,
					hitByEntityId = this.entity.id,
					hitEntityPosition = component4.transform.position,
					hitImpulse = Vector3.zero,
					hitPosition = component4.transform.position,
					hittablePoint = component5.FindHittablePoint(collider)
				};
				component5.RequestHit(gameHitData);
			}
		}
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x00110552 File Offset: 0x0010E752
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (this.currBehavior == GREnemyPest.Behavior.Attack && player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position);
		}
		yield break;
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x00110568 File Offset: 0x0010E768
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			this.armor.SetHp(0);
			return;
		case GREnemyPest.BodyState.Bones:
			this.armor.SetHp(0);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, false);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		case GREnemyPest.BodyState.Shell:
			this.armor.SetHp(this.hp);
			GREnemy.HideObjects(this.bonesStateVisibleObjects, true);
			GREnemy.HideObjects(this.alwaysVisibleObjects, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x060031ED RID: 12781 RVA: 0x001105EC File Offset: 0x0010E7EC
	public void SetBodyState(GREnemyPest.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.currBodyState = newBodyState;
		switch (this.currBodyState)
		{
		case GREnemyPest.BodyState.Destroyed:
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
			break;
		case GREnemyPest.BodyState.Bones:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax);
			break;
		case GREnemyPest.BodyState.Shell:
			this.hp = this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax);
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x060031EE RID: 12782 RVA: 0x001106C4 File Offset: 0x0010E8C4
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
		float magnitude = (GRSenseNearby.GetRigTestLocation(VRRig.LocalRig) - base.transform.position).magnitude;
		bool flag = GRSenseLineOfSight.HasGeoLineOfSight(this.headTransform.position, GRSenseNearby.GetRigTestLocation(VRRig.LocalRig), this.senseLineOfSight.sightDist, this.senseLineOfSight.visibilityMask);
		strings.Add(string.Format("player rig dis: {0} has los: {1}", magnitude, flag));
	}

	// Token: 0x0400401F RID: 16415
	public GameEntity entity;

	// Token: 0x04004020 RID: 16416
	public GameAgent agent;

	// Token: 0x04004021 RID: 16417
	public GREnemy enemy;

	// Token: 0x04004022 RID: 16418
	public GRArmorEnemy armor;

	// Token: 0x04004023 RID: 16419
	public GRAttributes attributes;

	// Token: 0x04004024 RID: 16420
	public Animation anim;

	// Token: 0x04004025 RID: 16421
	public GRSenseNearby senseNearby;

	// Token: 0x04004026 RID: 16422
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04004027 RID: 16423
	public GRAbilityIdle abilityIdle;

	// Token: 0x04004028 RID: 16424
	public GRAbilityChase abilityChase;

	// Token: 0x04004029 RID: 16425
	public GRAbilityWander abilityWander;

	// Token: 0x0400402A RID: 16426
	public GRAbilityAttackJump abilityAttack;

	// Token: 0x0400402B RID: 16427
	public GRAbilityStagger abilityStagger;

	// Token: 0x0400402C RID: 16428
	public GRAbilityStagger abilityFlashed;

	// Token: 0x0400402D RID: 16429
	public GRAbilityDie abilityDie;

	// Token: 0x0400402E RID: 16430
	public GRAbilityGrabbed abilityGrabbed;

	// Token: 0x0400402F RID: 16431
	public GRAbilityThrown abilityThrown;

	// Token: 0x04004030 RID: 16432
	public AbilitySound spawnSound;

	// Token: 0x04004031 RID: 16433
	public GRAbilityMoveToTarget abilityInvestigate;

	// Token: 0x04004032 RID: 16434
	public GRAbilityJump abilityJump;

	// Token: 0x04004033 RID: 16435
	public List<GameObject> bonesStateVisibleObjects;

	// Token: 0x04004034 RID: 16436
	public List<GameObject> alwaysVisibleObjects;

	// Token: 0x04004035 RID: 16437
	public Transform coreMarker;

	// Token: 0x04004036 RID: 16438
	public GRCollectible corePrefab;

	// Token: 0x04004037 RID: 16439
	public Transform headTransform;

	// Token: 0x04004038 RID: 16440
	public float attackRange = 2f;

	// Token: 0x04004039 RID: 16441
	public List<VRRig> rigsNearby;

	// Token: 0x0400403A RID: 16442
	public NavMeshAgent navAgent;

	// Token: 0x0400403B RID: 16443
	public AudioSource audioSource;

	// Token: 0x0400403C RID: 16444
	public float hearingRadius = 5f;

	// Token: 0x0400403D RID: 16445
	private Vector3? investigateLocation;

	// Token: 0x0400403F RID: 16447
	[ReadOnly]
	public int hp;

	// Token: 0x04004040 RID: 16448
	[ReadOnly]
	public GREnemyPest.Behavior currBehavior;

	// Token: 0x04004041 RID: 16449
	[ReadOnly]
	public double behaviorEndTime;

	// Token: 0x04004042 RID: 16450
	[ReadOnly]
	public GREnemyPest.BodyState currBodyState;

	// Token: 0x04004043 RID: 16451
	[ReadOnly]
	public int nextPatrolNode;

	// Token: 0x04004044 RID: 16452
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04004045 RID: 16453
	[ReadOnly]
	public double behaviorStartTime;

	// Token: 0x04004046 RID: 16454
	private Rigidbody rigidBody;

	// Token: 0x04004047 RID: 16455
	private List<Collider> colliders;

	// Token: 0x04004048 RID: 16456
	private float lastHitPlayerTime;

	// Token: 0x04004049 RID: 16457
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x0400404A RID: 16458
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x0400404B RID: 16459
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x020007A3 RID: 1955
	public enum Behavior
	{
		// Token: 0x0400404D RID: 16461
		Idle,
		// Token: 0x0400404E RID: 16462
		Wander,
		// Token: 0x0400404F RID: 16463
		Chase,
		// Token: 0x04004050 RID: 16464
		Attack,
		// Token: 0x04004051 RID: 16465
		Stagger,
		// Token: 0x04004052 RID: 16466
		Grabbed,
		// Token: 0x04004053 RID: 16467
		Thrown,
		// Token: 0x04004054 RID: 16468
		Destroyed,
		// Token: 0x04004055 RID: 16469
		Investigate,
		// Token: 0x04004056 RID: 16470
		Jump,
		// Token: 0x04004057 RID: 16471
		Flashed,
		// Token: 0x04004058 RID: 16472
		Count
	}

	// Token: 0x020007A4 RID: 1956
	public enum BodyState
	{
		// Token: 0x0400405A RID: 16474
		Destroyed,
		// Token: 0x0400405B RID: 16475
		Bones,
		// Token: 0x0400405C RID: 16476
		Shell,
		// Token: 0x0400405D RID: 16477
		Count
	}
}
