using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000795 RID: 1941
public class GREnemyBossMoonEye : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent
{
	// Token: 0x0600314E RID: 12622 RVA: 0x0010BC3C File Offset: 0x00109E3C
	private void Awake()
	{
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		if (this.armor != null)
		{
			this.armor.SetHp(0);
		}
		if (this.navAgent != null)
		{
			this.navAgent.updateRotation = false;
		}
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[8];
	}

	// Token: 0x0600314F RID: 12623 RVA: 0x0010BCB8 File Offset: 0x00109EB8
	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoonEye.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.AttackLaser, this.abilityAttackLaser, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Closed, this.abilityClosed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityStart, this.abilityGravityStart, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityEnd, this.abilityGravityEnd, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.GravityIdle, this.abilityGravityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoonEye.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			foreach (GRBonusEntry grbonusEntry in this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig().enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		int num = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(num);
		}
		this.SetHP(num);
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	// Token: 0x06003150 RID: 12624 RVA: 0x0010BF08 File Offset: 0x0010A108
	private void SetupAbility(GREnemyBossMoonEye.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06003151 RID: 12625 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x0010BF24 File Offset: 0x0010A124
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x06003154 RID: 12628 RVA: 0x0010BF3D File Offset: 0x0010A13D
	public void Setup(long entityCreateData)
	{
		this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, true);
	}

	// Token: 0x06003155 RID: 12629 RVA: 0x0010BF47 File Offset: 0x0010A147
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 8)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoonEye.Behavior)newState, false);
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x0010BF5A File Offset: 0x0010A15A
	public void ResetEye()
	{
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Idle, false);
		}
	}

	// Token: 0x06003157 RID: 12631 RVA: 0x0010BF71 File Offset: 0x0010A171
	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	// Token: 0x06003158 RID: 12632 RVA: 0x0010BF94 File Offset: 0x0010A194
	public bool TrySetBehavior(GREnemyBossMoonEye.Behavior newBehavior)
	{
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x06003159 RID: 12633 RVA: 0x0010BFA0 File Offset: 0x0010A1A0
	private void SetBehavior(GREnemyBossMoonEye.Behavior newBehavior, bool force = false)
	{
		if (this.abilities == null)
		{
			Debug.LogError("Abilities have not been initialized", this);
			return;
		}
		if (newBehavior < GREnemyBossMoonEye.Behavior.Idle || newBehavior >= (GREnemyBossMoonEye.Behavior)this.abilities.Length)
		{
			Debug.LogErrorFormat("New Behavior Index is invalid {0} {1} {2}", new object[]
			{
				(int)newBehavior,
				newBehavior,
				base.gameObject.name
			});
			return;
		}
		GRAbilityBase grabilityBase = this.abilities[(int)newBehavior];
		if (this.currBehavior == newBehavior && !force)
		{
			return;
		}
		Debug.LogFormat("Boss Eye SetBehavior {0} -> {1}", new object[] { this.currBehavior, newBehavior });
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			this.SetHP(this.CalcMaxHP());
		}
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.AttackLaser)
		{
			this.abilityAttackLaser.SetTargetPlayer(this.agent.targetPlayer);
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x0600315A RID: 12634 RVA: 0x0010C0D0 File Offset: 0x0010A2D0
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		return (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
	}

	// Token: 0x0600315B RID: 12635 RVA: 0x00002C2D File Offset: 0x00000E2D
	private void RefreshBody()
	{
	}

	// Token: 0x0600315C RID: 12636 RVA: 0x0010C109 File Offset: 0x0010A309
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x0600315D RID: 12637 RVA: 0x0010C118 File Offset: 0x0010A318
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyBossMoonEye.tempRigs.Clear();
		GREnemyBossMoonEye.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoonEye.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoonEye.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Idle)
		{
			this.ChooseNewBehavior();
		}
	}

	// Token: 0x0600315E RID: 12638 RVA: 0x0010C1C0 File Offset: 0x0010A3C0
	private bool TryChooseAttackBehavior()
	{
		if (Time.timeAsDouble > this.lastHitTime + (double)this.counterAttackWindow)
		{
			return false;
		}
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Closed)
		{
			return false;
		}
		GREnemyBossMoonEye.tempPotentialAttacks.Clear();
		if (this.allowLaserAttack)
		{
			GREnemyBossMoonEye.tempPotentialAttacks.Add(GREnemyBossMoonEye.Behavior.AttackLaser);
		}
		for (int i = GREnemyBossMoonEye.tempPotentialAttacks.Count - 1; i >= 0; i--)
		{
			GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoonEye.tempPotentialAttacks[i]];
			if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
			{
				GREnemyBossMoonEye.tempPotentialAttacks.RemoveAt(i);
			}
		}
		if (GREnemyBossMoonEye.tempPotentialAttacks.Count <= 0)
		{
			return false;
		}
		int num = Random.Range(0, GREnemyBossMoonEye.tempPotentialAttacks.Count);
		this.SetBehavior(GREnemyBossMoonEye.tempPotentialAttacks[num], false);
		return true;
	}

	// Token: 0x0600315F RID: 12639 RVA: 0x0010C293 File Offset: 0x0010A493
	private void ChooseNewBehavior()
	{
		if (!GhostReactorManager.AggroDisabled && this.TryChooseAttackBehavior())
		{
			return;
		}
		this.TrySetBehavior(GREnemyBossMoonEye.Behavior.Idle);
	}

	// Token: 0x06003160 RID: 12640 RVA: 0x0010C2AD File Offset: 0x0010A4AD
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003161 RID: 12641 RVA: 0x0010C2CB File Offset: 0x0010A4CB
	private void OnUpdateAuthority(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			if (this.currAbility.IsDone())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.None, false);
				this.ChooseNewBehavior();
			}
		}
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x0010C2FC File Offset: 0x0010A4FC
	private void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x0010C312 File Offset: 0x0010A512
	public void InstantKill()
	{
		if (this.hp <= 0)
		{
			return;
		}
		this.SetHP(0);
		this.lastHitTime = Time.timeAsDouble;
		if (this.entity.IsAuthority())
		{
			this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
		}
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x0010C348 File Offset: 0x0010A548
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		if (this.currBehavior == GREnemyBossMoonEye.Behavior.Dying)
		{
			return;
		}
		this.SetHP(this.hp - hit.hitAmount);
		this.lastHitTime = Time.timeAsDouble;
		if (this.hp <= 0)
		{
			this.hp = 0;
			if (this.entity.IsAuthority())
			{
				this.SetBehavior(GREnemyBossMoonEye.Behavior.Closed, false);
				return;
			}
		}
		else
		{
			this.lastSeenTargetPosition = tool.transform.position;
			this.lastSeenTargetTime = Time.timeAsDouble;
		}
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x0010C3BF File Offset: 0x0010A5BF
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x0010C3CC File Offset: 0x0010A5CC
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBehavior != GREnemyBossMoonEye.Behavior.AttackLaser)
		{
			return;
		}
		GRShieldCollider component = collider.GetComponent<GRShieldCollider>();
		if (component != null)
		{
			GameHittable component2 = base.GetComponent<GameHittable>();
			component.BlockHittable(this.headTransform.position, base.transform.forward, component2);
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

	// Token: 0x06003167 RID: 12647 RVA: 0x0010C530 File Offset: 0x0010A730
	private IEnumerator TryHitPlayer(GRPlayer player)
	{
		yield return new WaitForUpdate();
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			Vector3 vector = player.transform.position - base.transform.position;
			vector.y = 0f;
			vector = vector.normalized * 6f;
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, vector);
		}
		yield break;
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x0010C546 File Offset: 0x0010A746
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Format("State: <color=\"yellow\">{0}<color=\"white\"> HP: <color=\"yellow\">{1}<color=\"white\">", this.currBehavior.ToString(), this.hp));
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x0010C57C File Offset: 0x0010A77C
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		int num = ((this.targetPlayer == null) ? (-1) : this.targetPlayer.ActorNumber);
		writer.Write(b);
		writer.Write(this.hp);
		writer.Write(num);
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x0010C5C4 File Offset: 0x0010A7C4
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoonEye.Behavior behavior = (GREnemyBossMoonEye.Behavior)reader.ReadByte();
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num2);
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x0010C608 File Offset: 0x0010A808
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			if (hitTypeId == GameHitType.Club)
			{
				this.OnHitByClub(gameComponent, hit);
				return;
			}
			if (hitTypeId != GameHitType.Shield)
			{
				return;
			}
			this.OnHitByShield(gameComponent, hit);
		}
	}

	// Token: 0x04003F3E RID: 16190
	public GameEntity entity;

	// Token: 0x04003F3F RID: 16191
	public GameAgent agent;

	// Token: 0x04003F40 RID: 16192
	public GREnemy enemy;

	// Token: 0x04003F41 RID: 16193
	public GRArmorEnemy armor;

	// Token: 0x04003F42 RID: 16194
	public GameHittable hittable;

	// Token: 0x04003F43 RID: 16195
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003F44 RID: 16196
	public GRSenseNearby senseNearby;

	// Token: 0x04003F45 RID: 16197
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003F46 RID: 16198
	public Animation anim;

	// Token: 0x04003F47 RID: 16199
	private GRAbilityBase[] abilities;

	// Token: 0x04003F48 RID: 16200
	private GRAbilityBase currAbility;

	// Token: 0x04003F49 RID: 16201
	public GRAbilityAgent abilityAgent;

	// Token: 0x04003F4A RID: 16202
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003F4B RID: 16203
	public GRAbilityIdle abilityClosed;

	// Token: 0x04003F4C RID: 16204
	public GRAbilityAttackLaser abilityAttackLaser;

	// Token: 0x04003F4D RID: 16205
	public GRAbilityDie abilityDie;

	// Token: 0x04003F4E RID: 16206
	public GRAbilityIdle abilityGravityStart;

	// Token: 0x04003F4F RID: 16207
	public GRAbilityIdle abilityGravityEnd;

	// Token: 0x04003F50 RID: 16208
	public GRAbilityIdle abilityGravityIdle;

	// Token: 0x04003F51 RID: 16209
	public Transform headTransform;

	// Token: 0x04003F52 RID: 16210
	public NavMeshAgent navAgent;

	// Token: 0x04003F53 RID: 16211
	public AudioSource audioSource;

	// Token: 0x04003F54 RID: 16212
	public float counterAttackWindow = 3f;

	// Token: 0x04003F55 RID: 16213
	private Transform target;

	// Token: 0x04003F56 RID: 16214
	[ReadOnly]
	public int hp;

	// Token: 0x04003F57 RID: 16215
	[ReadOnly]
	public GREnemyBossMoonEye.Behavior currBehavior;

	// Token: 0x04003F58 RID: 16216
	[ReadOnly]
	public GREnemyBossMoonEye.BodyState currBodyState;

	// Token: 0x04003F59 RID: 16217
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003F5A RID: 16218
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003F5B RID: 16219
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003F5C RID: 16220
	public bool allowLaserAttack;

	// Token: 0x04003F5D RID: 16221
	public bool canChaseJump = true;

	// Token: 0x04003F5E RID: 16222
	public float chaseJumpDistance = 5f;

	// Token: 0x04003F5F RID: 16223
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003F60 RID: 16224
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003F61 RID: 16225
	private double lastHitTime;

	// Token: 0x04003F62 RID: 16226
	private List<Collider> colliders;

	// Token: 0x04003F63 RID: 16227
	private float lastHitPlayerTime;

	// Token: 0x04003F64 RID: 16228
	private float minTimeBetweenHits = 0.5f;

	// Token: 0x04003F65 RID: 16229
	public float hearingRadius = 5f;

	// Token: 0x04003F66 RID: 16230
	public int maxSimultaneousSummonedEntities = 6;

	// Token: 0x04003F67 RID: 16231
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003F68 RID: 16232
	private static List<GREnemyBossMoonEye.Behavior> tempPotentialAttacks = new List<GREnemyBossMoonEye.Behavior>(16);

	// Token: 0x04003F69 RID: 16233
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x02000796 RID: 1942
	public enum Behavior
	{
		// Token: 0x04003F6B RID: 16235
		Idle,
		// Token: 0x04003F6C RID: 16236
		AttackLaser,
		// Token: 0x04003F6D RID: 16237
		Closed,
		// Token: 0x04003F6E RID: 16238
		GravityStart,
		// Token: 0x04003F6F RID: 16239
		GravityEnd,
		// Token: 0x04003F70 RID: 16240
		GravityIdle,
		// Token: 0x04003F71 RID: 16241
		Dying,
		// Token: 0x04003F72 RID: 16242
		None,
		// Token: 0x04003F73 RID: 16243
		Count
	}

	// Token: 0x02000797 RID: 1943
	public enum BodyState
	{
		// Token: 0x04003F75 RID: 16245
		Destroyed,
		// Token: 0x04003F76 RID: 16246
		Bones,
		// Token: 0x04003F77 RID: 16247
		Shell,
		// Token: 0x04003F78 RID: 16248
		Count
	}
}
