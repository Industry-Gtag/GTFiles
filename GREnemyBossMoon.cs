using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using GorillaLocomotion;
using GorillaTagScripts.GhostReactor;
using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x0200078C RID: 1932
public class GREnemyBossMoon : MonoBehaviour, IGameEntityComponent, IGameEntitySerialize, IGameHittable, IGameAgentComponent, IGameEntityDebugComponent, IGRSummoningEntity
{
	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x060030ED RID: 12525 RVA: 0x00109268 File Offset: 0x00107468
	// (set) Token: 0x060030EE RID: 12526 RVA: 0x00109270 File Offset: 0x00107470
	public bool BossHasRevealed { get; private set; }

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x060030EF RID: 12527 RVA: 0x00109279 File Offset: 0x00107479
	public GRAbilityBase CurrAbility
	{
		get
		{
			return this.currAbility;
		}
	}

	// Token: 0x060030F0 RID: 12528 RVA: 0x00109284 File Offset: 0x00107484
	private void Awake()
	{
		this.trackedEntities = new List<int>(16);
		this.trackedGameEntities = new List<GameEntity>(16);
		this.rigidBody = base.GetComponent<Rigidbody>();
		this.colliders = new List<Collider>(4);
		base.GetComponentsInChildren<Collider>(this.colliders);
		this.agent.onBodyStateChanged += this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviorStateChange;
		this.abilities = new GRAbilityBase[32];
		this.adaptiveMusicController = Object.FindObjectOfType<GRAdaptiveMusicController>();
	}

	// Token: 0x060030F1 RID: 12529 RVA: 0x00109318 File Offset: 0x00107518
	public void OnEntityInit()
	{
		this.currBehavior = GREnemyBossMoon.Behavior.None;
		this.currAbility = null;
		this.SetupAbility(GREnemyBossMoon.Behavior.HiddenIdle, this.abilityHiddenIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Reveal, this.abilityReveal, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Idle, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Exposed, this.abilityExposed, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.ExposedIdle, this.abilityExposedIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongue, this.abilityAttackTongue01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTongueSwipe, this.abilityAttackTongueSwipe01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle00, this.abilityAttackTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle01, this.abilityAttackTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle02, this.abilityAttackTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle03, this.abilityAttackTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle04, this.abilityAttackTentacle04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackTentacle05, this.abilityAttackTentacle05, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle00, this.abilityAttackQuickTentacle00, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle01, this.abilityAttackQuickTentacle01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle02, this.abilityAttackQuickTentacle02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.AttackQuickTentacle03, this.abilityAttackQuickTentacle03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonStart, this.abilitySummonStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.SummonEnd, this.abilitySummonEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon01, this.abilitySummon01, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon02, this.abilitySummon02, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon03, this.abilitySummon03, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Summon04, this.abilitySummon04, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatStart, this.abilityRetreatStart, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatEnd, this.abilityRetreatEnd, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.RetreatIdle, this.abilityRetreatIdle, this.agent, this.anim, this.audioSource, base.transform, this.headTransform, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Dying, this.abilityDie, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.DyingIdle, this.abilityDieIdle, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.Runaway, this.abilityRunaway, this.agent, this.anim, this.audioSource, base.transform, null, null);
		this.SetupAbility(GREnemyBossMoon.Behavior.NextPhase, this.abilityIdle, this.agent, this.anim, this.audioSource, null, null, null);
		this.senseNearby.Setup(this.headTransform, this.entity);
		this.Setup(this.entity.createData);
		if (this.entity && this.entity.manager && this.entity.manager.ghostReactorManager && this.entity.manager.ghostReactorManager.reactor)
		{
			GhostReactorLevelGenConfig currLevelGenConfig = this.entity.manager.ghostReactorManager.reactor.GetCurrLevelGenConfig();
			foreach (GRBonusEntry grbonusEntry in currLevelGenConfig.enemyGlobalBonuses)
			{
				this.attributes.AddBonus(grbonusEntry);
			}
			if (currLevelGenConfig.minEnemyKills.Count > 0)
			{
				GREnemyCount grenemyCount = currLevelGenConfig.minEnemyKills[0];
				GREnemyType enemyType = grenemyCount.EnemyType;
				if (enemyType != GREnemyType.MoonBoss_Phase1)
				{
					if (enemyType == GREnemyType.MoonBoss_Phase2)
					{
						this.phases[1].runawayAfterPhase = true;
					}
				}
				else
				{
					this.phases[0].runawayAfterPhase = true;
				}
				GRBreakableItemSpawnConfig lootTableForType = this.GetLootTableForType(grenemyCount.EnemyType);
				this.abilityDie.lootTable = lootTableForType;
				this.abilityRunaway.lootTable = lootTableForType;
			}
		}
		if (this.agent.navAgent != null)
		{
			this.agent.navAgent.autoTraverseOffMeshLink = false;
		}
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		int num = this.CalcMaxHP();
		if (this.enemy != null)
		{
			this.enemy.SetMaxHP(num);
		}
		this.SetHP(num);
	}

	// Token: 0x060030F2 RID: 12530 RVA: 0x001099EC File Offset: 0x00107BEC
	private GRBreakableItemSpawnConfig GetLootTableForType(GREnemyType enemyType)
	{
		for (int i = 0; i < this.lootPhases.Count; i++)
		{
			if (this.lootPhases[i].enemyType == enemyType)
			{
				return this.lootPhases[i].lootTable;
			}
		}
		return null;
	}

	// Token: 0x060030F3 RID: 12531 RVA: 0x00109A36 File Offset: 0x00107C36
	private void SetupAbility(GREnemyBossMoon.Behavior behavior, GRAbilityBase ability, GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.abilities[(int)behavior] = ability;
		ability.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x060030F4 RID: 12532 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060030F5 RID: 12533 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x060030F6 RID: 12534 RVA: 0x00109A52 File Offset: 0x00107C52
	private void OnDestroy()
	{
		this.agent.onBodyStateChanged -= this.OnNetworkBodyStateChange;
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviorStateChange;
	}

	// Token: 0x060030F7 RID: 12535 RVA: 0x00109A82 File Offset: 0x00107C82
	public void Setup(long entityCreateData)
	{
		this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, true);
		if (this.attributes.CalculateFinalValueForAttribute(GRAttributeType.ArmorMax) > 0)
		{
			this.SetBodyState(GREnemyBossMoon.BodyState.Shell, true);
			return;
		}
		this.SetBodyState(GREnemyBossMoon.BodyState.Bones, true);
	}

	// Token: 0x060030F8 RID: 12536 RVA: 0x00109AAC File Offset: 0x00107CAC
	public void OnNetworkBehaviorStateChange(byte newState)
	{
		if (newState < 0 || newState >= 32)
		{
			return;
		}
		this.SetBehavior((GREnemyBossMoon.Behavior)newState, false);
	}

	// Token: 0x060030F9 RID: 12537 RVA: 0x00109AC0 File Offset: 0x00107CC0
	public void OnNetworkBodyStateChange(byte newState)
	{
		if (newState < 0 || newState >= 3)
		{
			return;
		}
		this.SetBodyState((GREnemyBossMoon.BodyState)newState, false);
	}

	// Token: 0x060030FA RID: 12538 RVA: 0x00109AD3 File Offset: 0x00107CD3
	public void SetHP(int hp)
	{
		this.hp = hp;
		if (this.enemy != null)
		{
			this.enemy.SetHP(hp);
		}
	}

	// Token: 0x060030FB RID: 12539 RVA: 0x00109AF6 File Offset: 0x00107CF6
	public bool TrySetBehavior(GREnemyBossMoon.Behavior newBehavior)
	{
		if (newBehavior == GREnemyBossMoon.Behavior.Stagger)
		{
			return false;
		}
		this.SetBehavior(newBehavior, false);
		return true;
	}

	// Token: 0x060030FC RID: 12540 RVA: 0x00109B08 File Offset: 0x00107D08
	public void SetBehavior(GREnemyBossMoon.Behavior newBehavior, bool force = false)
	{
		if (newBehavior < GREnemyBossMoon.Behavior.HiddenIdle || newBehavior >= (GREnemyBossMoon.Behavior)this.abilities.Length)
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
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.AttackTongue)
		{
			if (behavior == GREnemyBossMoon.Behavior.NextPhase)
			{
				this.IncrementBossPhase();
			}
		}
		else
		{
			for (int i = 0; i < this.eyes.Count; i++)
			{
				this.eyes[i].ResetEye();
			}
			this.consecutiveCombos = 0;
			this.attacksAfterSummon = 0;
			this.currSummon = null;
			this.KillAllSummoned(true, true);
			if (this.triggerNextMusicTransition)
			{
				this.triggerNextMusicTransition = false;
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToNextTrack();
				}
			}
		}
		Debug.LogFormat("Boss SetBehavior {0} -> {1}", new object[] { this.currBehavior, newBehavior });
		if (this.currAbility != null)
		{
			this.currAbility.Stop();
		}
		this.lastBehavior = this.currBehavior;
		this.currBehavior = newBehavior;
		this.currAbility = grabilityBase;
		if (this.currAbility != null)
		{
			this.currAbility.Start();
		}
		behavior = this.currBehavior;
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.Reveal:
			if (this.firstTimeReveal)
			{
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.Restart();
				}
				this.internalPhaseIndex = 0;
			}
			this.firstTimeReveal = false;
			this.BossHasRevealed = true;
			break;
		case GREnemyBossMoon.Behavior.Exposed:
			this.ToggleShockColliders(false);
			break;
		case GREnemyBossMoon.Behavior.ExposedIdle:
			break;
		case GREnemyBossMoon.Behavior.Stagger:
			this.lastStaggerTime = Time.time;
			break;
		case GREnemyBossMoon.Behavior.Dying:
		{
			this.KillAllSummoned();
			this.TurnOffGrav();
			for (int j = 0; j < this.eyes.Count; j++)
			{
				this.eyes[j].TrySetBehavior(GREnemyBossMoonEye.Behavior.Dying);
			}
			if (this.adaptiveMusicController != null)
			{
				this.adaptiveMusicController.TransitionToLastTrack();
			}
			this.ToggleShockColliders(false);
			break;
		}
		default:
			switch (behavior)
			{
			case GREnemyBossMoon.Behavior.AttackTongue:
				this.ToggleShockColliders(true);
				break;
			case GREnemyBossMoon.Behavior.Summon01:
			case GREnemyBossMoon.Behavior.Summon02:
			case GREnemyBossMoon.Behavior.Summon03:
			case GREnemyBossMoon.Behavior.Summon04:
				this.currSummon = (GRAbilitySummon)this.currAbility;
				break;
			case GREnemyBossMoon.Behavior.RetreatStart:
				this.TurnOnGrav();
				break;
			case GREnemyBossMoon.Behavior.RetreatEnd:
				this.TurnOffGrav();
				break;
			case GREnemyBossMoon.Behavior.Runaway:
				if (this.entity.manager.ghostReactorManager != null)
				{
					this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
				}
				if (this.adaptiveMusicController != null)
				{
					this.adaptiveMusicController.TransitionToLastTrack();
				}
				break;
			}
			break;
		}
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestBehaviorChange((byte)this.currBehavior);
		}
	}

	// Token: 0x060030FD RID: 12541 RVA: 0x00109E14 File Offset: 0x00108014
	public void SetSquishVolumeState(bool squishEnabled)
	{
		for (int i = 0; i < this.squishVolumes.Count; i++)
		{
			this.squishVolumes[i].overrideDisabled = !squishEnabled;
			this.squishVolumes[i].SliceUpdate();
		}
	}

	// Token: 0x060030FE RID: 12542 RVA: 0x00109E60 File Offset: 0x00108060
	private int CalcMaxHP()
	{
		float difficultyScalingForCurrentFloor = this.entity.manager.ghostReactorManager.reactor.difficultyScalingForCurrentFloor;
		int num = (int)((float)this.attributes.CalculateFinalValueForAttribute(GRAttributeType.HPMax) * difficultyScalingForCurrentFloor);
		for (int i = 0; i < this.phases.Count; i++)
		{
			this.phases[i].minHP = Mathf.RoundToInt((float)this.phases[i].minHP * difficultyScalingForCurrentFloor);
		}
		return num;
	}

	// Token: 0x060030FF RID: 12543 RVA: 0x00109EDC File Offset: 0x001080DC
	public int GetCurrPhaseIndex()
	{
		if (this.phases == null)
		{
			return -1;
		}
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.hp > this.phases[i].minHP)
			{
				return i;
			}
		}
		return this.phases.Count - 1;
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x00109F34 File Offset: 0x00108134
	public GREnemyBossMoon.PhaseDef GetCurrPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return null;
		}
		return this.phases[currPhaseIndex];
	}

	// Token: 0x06003101 RID: 12545 RVA: 0x00109F68 File Offset: 0x00108168
	public void RestoreFullHealth()
	{
		this.SetHP(this.CalcMaxHP());
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x00109F76 File Offset: 0x00108176
	public void HurtBossHP()
	{
		this.HurtBoss(100, this.entity.id, Vector3.zero);
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x00109F90 File Offset: 0x00108190
	public void KillAllEyes()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			this.eyes[i].InstantKill();
		}
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x00109FC4 File Offset: 0x001081C4
	public void KillAllSummoned()
	{
		this.KillAllSummoned(true, true);
	}

	// Token: 0x06003105 RID: 12549 RVA: 0x00109FD0 File Offset: 0x001081D0
	public void KillAllSummoned(bool ignoreMonkeye = false, bool killAllEnemies = true)
	{
		int num = 0;
		for (int i = 0; i < this.trackedGameEntities.Count; i++)
		{
			if (!(this.trackedGameEntities[i] == null))
			{
				GREnemyChaser component = this.trackedGameEntities[i].GetComponent<GREnemyChaser>();
				if (component != null)
				{
					component.InstantDeath();
					num++;
				}
				else
				{
					GREnemyRanged component2 = this.trackedGameEntities[i].GetComponent<GREnemyRanged>();
					if (component2 != null)
					{
						component2.InstantDeath();
						num++;
					}
					else
					{
						GREnemyPest component3 = this.trackedGameEntities[i].GetComponent<GREnemyPest>();
						if (component3 != null)
						{
							component3.InstantDeath();
							num++;
						}
						else
						{
							GREnemySummoner component4 = this.trackedGameEntities[i].GetComponent<GREnemySummoner>();
							if (component4 != null)
							{
								component4.InstantDeath();
								num++;
							}
							else if (!ignoreMonkeye)
							{
								GREnemyMonkeye component5 = this.trackedGameEntities[i].GetComponent<GREnemyMonkeye>();
								if (component5 != null)
								{
									component5.InstantDeath();
									num++;
								}
							}
						}
					}
				}
			}
		}
		if (killAllEnemies && this.entity.manager.ghostReactorManager != null)
		{
			this.entity.manager.ghostReactorManager.InstantDeathForCurrentEnemies();
		}
		Debug.Log(string.Format("Report killed all summon {0}", num));
	}

	// Token: 0x06003106 RID: 12550 RVA: 0x0010A128 File Offset: 0x00108328
	public void GoBackPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex <= 0)
		{
			Debug.LogWarning("GREnemyBossMoon - GoBackPhase - At first phase");
			return;
		}
		this.SetHP(this.phases[currPhaseIndex - 1].minHP);
	}

	// Token: 0x06003107 RID: 12551 RVA: 0x0010A164 File Offset: 0x00108364
	public void GoToNextPhase()
	{
		int currPhaseIndex = this.GetCurrPhaseIndex();
		if (currPhaseIndex < 0 || currPhaseIndex >= this.phases.Count)
		{
			return;
		}
		this.SetHP(this.phases[currPhaseIndex].minHP);
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x0010A1A4 File Offset: 0x001083A4
	private bool IsSummon(GREnemyBossMoon.Behavior behavior)
	{
		for (int i = 0; i < this.phases.Count; i++)
		{
			if (this.phases[i] != null && this.phases[i].summons != null && this.phases[i].summons.Contains(behavior))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x0010A204 File Offset: 0x00108404
	private bool IsAnySummonBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return this.currBehavior == GREnemyBossMoon.Behavior.SummonStart || this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd || this.currBehavior == GREnemyBossMoon.Behavior.Summon01 || this.currBehavior == GREnemyBossMoon.Behavior.Summon02 || this.currBehavior == GREnemyBossMoon.Behavior.Summon03 || this.currBehavior == GREnemyBossMoon.Behavior.Summon04;
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x0010A244 File Offset: 0x00108444
	public GREnemyBossMoon.Behavior ChooseSummonForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.summons);
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x0010A26C File Offset: 0x0010846C
	public GREnemyBossMoon.Behavior ChooseAttackForPhase()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (currPhase == null)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		return this.ChooseRandomBehavior(currPhase.attacks);
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x0010A294 File Offset: 0x00108494
	public GREnemyBossMoon.Behavior ChooseRandomBehavior(List<GREnemyBossMoon.Behavior> behaviors)
	{
		if (behaviors == null || behaviors.Count <= 0)
		{
			return GREnemyBossMoon.Behavior.None;
		}
		int num = Random.Range(0, behaviors.Count);
		return behaviors[num];
	}

	// Token: 0x0600310D RID: 12557 RVA: 0x0010A2C4 File Offset: 0x001084C4
	public void SetBodyState(GREnemyBossMoon.BodyState newBodyState, bool force = false)
	{
		if (this.currBodyState == newBodyState && !force)
		{
			return;
		}
		this.currBodyState = newBodyState;
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			GhostReactorManager.Get(this.entity).ReportEnemyDeath();
		}
		Debug.LogFormat("State Change {0} {1}", new object[]
		{
			this.entity.id.index,
			this.currBodyState
		});
		this.RefreshBody();
		if (this.entity.IsAuthority())
		{
			this.agent.RequestStateChange((byte)newBodyState);
		}
	}

	// Token: 0x0600310E RID: 12558 RVA: 0x0010A354 File Offset: 0x00108554
	private void RefreshBody()
	{
		switch (this.currBodyState)
		{
		case GREnemyBossMoon.BodyState.Destroyed:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Bones:
			GREnemy.HideRenderers(this.bones, false);
			GREnemy.HideRenderers(this.always, false);
			return;
		case GREnemyBossMoon.BodyState.Shell:
			GREnemy.HideRenderers(this.bones, true);
			GREnemy.HideRenderers(this.always, false);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600310F RID: 12559 RVA: 0x0010A3C5 File Offset: 0x001085C5
	private void Update()
	{
		this.OnUpdate(Time.deltaTime);
	}

	// Token: 0x06003110 RID: 12560 RVA: 0x0010A3D4 File Offset: 0x001085D4
	public void OnEntityThink(float dt)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		GREnemyBossMoon.tempRigs.Clear();
		GREnemyBossMoon.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(GREnemyBossMoon.tempRigs);
		this.senseNearby.UpdateNearby(GREnemyBossMoon.tempRigs, this.senseLineOfSight);
		float num;
		VRRig vrrig = this.senseNearby.PickClosest(out num);
		this.agent.RequestTarget((vrrig == null) ? null : vrrig.OwningNetPlayer);
		if (this.currAbility != null)
		{
			this.currAbility.Think(dt);
		}
		GREnemyBossMoon.Behavior behavior = this.currBehavior;
		if (behavior != GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (behavior != GREnemyBossMoon.Behavior.Idle)
			{
				if (behavior != GREnemyBossMoon.Behavior.RetreatIdle)
				{
					return;
				}
				this.waitInRetreat += dt * 12f;
				if (this.trackedEntities.Count <= 0 || this.waitInRetreat > 20f)
				{
					this.TrySetBehavior(GREnemyBossMoon.Behavior.RetreatEnd);
				}
			}
			else if (this.currAbility.IsDone())
			{
				this.ChooseNewBehavior(false);
				return;
			}
			return;
		}
		this.ChooseNewBehavior(true);
	}

	// Token: 0x06003111 RID: 12561 RVA: 0x0010A4D8 File Offset: 0x001086D8
	private GREnemyBossMoon.Behavior TryChooseAttackBehavior()
	{
		GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
		if (this.currBehavior == GREnemyBossMoon.Behavior.HiddenIdle)
		{
			if (currPhase != null && this.trackedEntities.Count <= currPhase.maxEnemiesForReveal && this.senseNearby.IsAnyoneNearby(this.abilityReveal.GetRange(), this.firstTimeReveal))
			{
				return GREnemyBossMoon.Behavior.Reveal;
			}
			return GREnemyBossMoon.Behavior.None;
		}
		else
		{
			if (GhostReactorManager.AggroDisabled)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase == null)
			{
				return GREnemyBossMoon.Behavior.None;
			}
			if (currPhase.summons != null && currPhase.summons.Count > 0 && this.attacksAfterSummon <= 0 && this.trackedEntities.Count < currPhase.maxSimultaneousEnemies)
			{
				this.attacksAfterSummon = currPhase.attacksBetweenSummons;
				if (currPhase.summons.Count > 0)
				{
					this.currSummon = (GRAbilitySummon)this.abilities[(int)currPhase.summons[0]];
					if (this.currSummon != null)
					{
						for (int i = this.trackedEntities.Count; i < currPhase.maxSimultaneousEnemies; i++)
						{
							this.currSummon.ForceSpawn();
						}
					}
				}
			}
			List<GREnemyBossMoon.Behavior> list = currPhase.attacks;
			if (currPhase.comboAttacks != null && currPhase.comboAttacks.Count > 0 && ((currPhase.allowConsecutiveCombos && this.consecutiveCombos < 3) || this.consecutiveCombos <= 0) && Random.value < currPhase.comboAttackChance)
			{
				list = currPhase.comboAttacks;
				this.consecutiveCombos++;
			}
			else
			{
				this.consecutiveCombos = 0;
			}
			if (list != null && list.Count > 0)
			{
				GREnemyBossMoon.tempPotentialAttacks.Clear();
				for (int j = 0; j < list.Count; j++)
				{
					GREnemyBossMoon.tempPotentialAttacks.Add(list[j]);
				}
				for (int k = GREnemyBossMoon.tempPotentialAttacks.Count - 1; k >= 0; k--)
				{
					GRAbilityBase grabilityBase = this.abilities[(int)GREnemyBossMoon.tempPotentialAttacks[k]];
					if (grabilityBase == null || !this.senseNearby.IsAnyoneNearby(grabilityBase.GetRange(), false) || !grabilityBase.IsCoolDownOver())
					{
						GREnemyBossMoon.tempPotentialAttacks.RemoveAt(k);
					}
				}
				if (GREnemyBossMoon.tempPotentialAttacks.Count > 0)
				{
					this.attacksAfterSummon--;
					int num = Random.Range(0, GREnemyBossMoon.tempPotentialAttacks.Count);
					return GREnemyBossMoon.tempPotentialAttacks[num];
				}
			}
			return GREnemyBossMoon.Behavior.None;
		}
	}

	// Token: 0x06003112 RID: 12562 RVA: 0x0010A718 File Offset: 0x00108918
	private bool AreAllEyesClosed()
	{
		for (int i = 0; i < this.eyes.Count; i++)
		{
			if (this.eyes[i].hp > 0)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003113 RID: 12563 RVA: 0x0010A752 File Offset: 0x00108952
	public void GotoDyingIdle()
	{
		this.SetBehavior(GREnemyBossMoon.Behavior.DyingIdle, true);
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x0010A760 File Offset: 0x00108960
	private void ChooseNewBehavior(bool forceAttack = false)
	{
		if (this.hp <= 0)
		{
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Dying);
			return;
		}
		if (this.AreAllEyesClosed())
		{
			if (this.eyesPushVolume != null)
			{
				this.eyesPushVolume.Trigger();
			}
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			return;
		}
		if (forceAttack || !this.restAfterAttack)
		{
			this.restAfterAttack = false;
			GREnemyBossMoon.Behavior behavior = this.TryChooseAttackBehavior();
			if (behavior != GREnemyBossMoon.Behavior.None)
			{
				if (this.TrySetBehavior(behavior) && this.currBehavior != GREnemyBossMoon.Behavior.AttackTongue)
				{
					GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
					this.restAfterAttack = currPhase.restAfterAttack;
				}
				if (this.currSummon != null)
				{
					GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
					if (this.trackedEntities.Count < currPhase2.maxSimultaneousEnemies && Random.value < currPhase2.randomSummonChance)
					{
						this.currSummon.ForceSpawn();
					}
				}
				return;
			}
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.None)
		{
			this.restAfterAttack = false;
			this.TrySetBehavior(GREnemyBossMoon.Behavior.Idle);
		}
	}

	// Token: 0x06003115 RID: 12565 RVA: 0x0010A842 File Offset: 0x00108A42
	private void OnUpdate(float dt)
	{
		if (this.entity.IsAuthority())
		{
			this.OnUpdateAuthority(dt);
			return;
		}
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06003116 RID: 12566 RVA: 0x0010A860 File Offset: 0x00108A60
	private void OnUpdateAuthority(float dt)
	{
		if (this.currBehavior == GREnemyBossMoon.Behavior.Runaway)
		{
			this.currAbility.UpdateAuthority(dt);
			return;
		}
		if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
		{
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			if (this.hp <= 0)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
			}
			else if (this.hp <= currPhase.minHP)
			{
				this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
			}
		}
		if (this.currAbility != null)
		{
			this.currAbility.UpdateAuthority(dt);
			GREnemyBossMoon.PhaseDef currPhase2 = this.GetCurrPhase();
			if (this.currAbility.IsDone())
			{
				if (this.currBehavior == GREnemyBossMoon.Behavior.NextPhase)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.Exposed)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.ExposedIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonStart)
				{
					GREnemyBossMoon.Behavior behavior = this.ChooseSummonForPhase();
					this.SetBehavior(behavior, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.SummonEnd && currPhase2.retreatAfterSummon)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatStart, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatStart)
				{
					this.waitInRetreat = 0f;
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatIdle, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.RetreatIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.RetreatEnd, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.ExposedIdle)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.AttackTongue, false);
					return;
				}
				if (this.currBehavior == GREnemyBossMoon.Behavior.AttackTongue)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.HiddenIdle, false);
					return;
				}
				if (!this.IsSummon(this.currBehavior))
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
					this.ChooseNewBehavior(false);
					return;
				}
				if (currPhase2 == null || this.trackedEntities.Count >= currPhase2.maxSimultaneousEnemies)
				{
					this.SetBehavior(GREnemyBossMoon.Behavior.SummonEnd, false);
					return;
				}
				this.SetBehavior(GREnemyBossMoon.Behavior.None, false);
				GREnemyBossMoon.Behavior behavior2 = this.ChooseSummonForPhase();
				this.SetBehavior(behavior2, false);
				return;
			}
			else if (this.AreAllEyesClosed() && this.currBehavior != GREnemyBossMoon.Behavior.Exposed && this.currBehavior != GREnemyBossMoon.Behavior.ExposedIdle && this.lastBehavior != GREnemyBossMoon.Behavior.Exposed && this.lastBehavior != GREnemyBossMoon.Behavior.ExposedIdle)
			{
				this.TrySetBehavior(GREnemyBossMoon.Behavior.Exposed);
			}
		}
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x0010AA2D File Offset: 0x00108C2D
	private void OnUpdateRemote(float dt)
	{
		if (this.currAbility != null)
		{
			this.currAbility.UpdateRemote(dt);
		}
	}

	// Token: 0x06003118 RID: 12568 RVA: 0x0010AA43 File Offset: 0x00108C43
	private void CatchUpPhase(int phase)
	{
		this.BossHasRevealed = true;
		this.internalPhaseIndex = phase;
		this.AdjustByPhaseIndex(phase);
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.RestartAt(phase);
		}
	}

	// Token: 0x06003119 RID: 12569 RVA: 0x0010AA74 File Offset: 0x00108C74
	private void IncrementBossPhase()
	{
		this.internalPhaseIndex++;
		this.triggerNextMusicTransition = true;
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Incrementing phase to phase {0}!", this.internalPhaseIndex));
	}

	// Token: 0x0600311A RID: 12570 RVA: 0x0010AAB4 File Offset: 0x00108CB4
	private void SyncPhase(int phase)
	{
		this.internalPhaseIndex = phase;
		if (this.adaptiveMusicController != null)
		{
			this.adaptiveMusicController.GoToTrack(this.internalPhaseIndex, false);
		}
		this.AdjustByPhaseIndex(this.internalPhaseIndex);
		Debug.Log(string.Format("Syncing phase to phase {0}!", this.internalPhaseIndex));
	}

	// Token: 0x0600311B RID: 12571 RVA: 0x0010AB10 File Offset: 0x00108D10
	private void AdjustByPhaseIndex(int phase)
	{
		switch (this.internalPhaseIndex)
		{
		case 1:
			this.abilityIdle.SpeedUp(3f);
			this.AdjustAttackAnimSpeed(1.2f);
			return;
		case 2:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.4f);
			return;
		case 3:
			this.abilityIdle.SpeedUp(4f);
			this.AdjustAttackAnimSpeed(1.6f);
			return;
		default:
			return;
		}
	}

	// Token: 0x0600311C RID: 12572 RVA: 0x0010AB8C File Offset: 0x00108D8C
	private void AdjustAttackAnimSpeed(float speed)
	{
		this.abilityAttackTentacle00.attackAnimData.speed = speed;
		this.abilityAttackTentacle01.attackAnimData.speed = speed;
		this.abilityAttackTentacle02.attackAnimData.speed = speed;
		this.abilityAttackTentacle03.attackAnimData.speed = speed;
		this.abilityAttackTentacle04.attackAnimData.speed = speed;
		this.abilityAttackTentacle05.attackAnimData.speed = speed;
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x0010ABFF File Offset: 0x00108DFF
	public void OnHitByClub(GRTool tool, GameHitData hit)
	{
		this.HurtBoss(hit.hitAmount, hit.hitEntityId, tool.transform.position);
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x0010AC20 File Offset: 0x00108E20
	private void HurtBoss(int hitAmount, GameEntityId hitByEntityId, Vector3 toolPosition)
	{
		if (this.currBehavior == GREnemyBossMoon.Behavior.Dying || this.currBehavior == GREnemyBossMoon.Behavior.DyingIdle || this.currBehavior == GREnemyBossMoon.Behavior.Runaway || this.IsAnySummonBehavior(this.currBehavior))
		{
			return;
		}
		if (this.currBodyState == GREnemyBossMoon.BodyState.Bones)
		{
			int num = this.hp;
			GREnemyBossMoon.PhaseDef currPhase = this.GetCurrPhase();
			this.SetHP(this.hp - hitAmount);
			if (this.damagedSounds.Count > 0)
			{
				this.damagedSoundIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.damagedSounds.Count, this.damagedSoundIndex);
				this.audioSource.PlayOneShot(this.damagedSounds[this.damagedSoundIndex], this.damagedSoundVolume);
			}
			if (this.fxDamaged != null)
			{
				this.fxDamaged.SetActive(false);
				this.fxDamaged.SetActive(true);
			}
			if (this.hp <= 0)
			{
				if (hitByEntityId != GameEntityId.Invalid)
				{
					this.abilityDie.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
				}
				this.SetBodyState(GREnemyBossMoon.BodyState.Destroyed, false);
				this.SetBehavior(GREnemyBossMoon.Behavior.Dying, false);
				return;
			}
			if (num > currPhase.minHP && this.hp <= currPhase.minHP)
			{
				if (currPhase.runawayAfterPhase)
				{
					Debug.Log("Force runaway!");
					if (hitByEntityId != GameEntityId.Invalid)
					{
						this.abilityRunaway.SetInstigatingPlayerIndex(this.entity.GetLastHeldByPlayerForEntityID(hitByEntityId));
					}
					this.SetBehavior(GREnemyBossMoon.Behavior.Runaway, false);
				}
				else
				{
					Debug.Log("Force next phase transition!");
					this.SetBehavior(GREnemyBossMoon.Behavior.NextPhase, false);
				}
			}
			this.lastSeenTargetPosition = toolPosition;
			this.lastSeenTargetTime = Time.timeAsDouble;
			Vector3 vector = this.lastSeenTargetPosition - base.transform.position;
			vector.y = 0f;
			this.searchPosition = this.lastSeenTargetPosition + vector.normalized * 1.5f;
		}
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnHitByFlash(GRTool grTool, GameHitData hit)
	{
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x0010ADF4 File Offset: 0x00108FF4
	public void OnHitByShield(GRTool tool, GameHitData hit)
	{
		this.OnHitByClub(tool, hit);
	}

	// Token: 0x06003121 RID: 12577 RVA: 0x0010AE00 File Offset: 0x00109000
	public void ReportDeathStat()
	{
		if (this.currAbility != null)
		{
			GRAbilityDie grabilityDie = this.currAbility as GRAbilityDie;
			if (grabilityDie != null)
			{
				grabilityDie.ReportDeathStat();
			}
		}
	}

	// Token: 0x06003122 RID: 12578 RVA: 0x0010AE2A File Offset: 0x0010902A
	private bool IsAttackBehavior(GREnemyBossMoon.Behavior behavior)
	{
		return behavior == GREnemyBossMoon.Behavior.AttackTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTentacle04 || behavior == GREnemyBossMoon.Behavior.AttackTentacle05 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle00 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle01 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle02 || behavior == GREnemyBossMoon.Behavior.AttackQuickTentacle03 || behavior == GREnemyBossMoon.Behavior.AttackTongue || behavior == GREnemyBossMoon.Behavior.AttackTongueSwipe;
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x0010AE68 File Offset: 0x00109068
	[CanBeNull]
	private GRAbilityBase GetAssociatedAbilityForBehavior(GREnemyBossMoon.Behavior behavior)
	{
		switch (behavior)
		{
		case GREnemyBossMoon.Behavior.AttackTentacle00:
			return this.abilityAttackTentacle00;
		case GREnemyBossMoon.Behavior.AttackTentacle01:
			return this.abilityAttackTentacle01;
		case GREnemyBossMoon.Behavior.AttackTentacle02:
			return this.abilityAttackTentacle02;
		case GREnemyBossMoon.Behavior.AttackTentacle03:
			return this.abilityAttackTentacle03;
		case GREnemyBossMoon.Behavior.AttackTentacle04:
			return this.abilityAttackTentacle04;
		case GREnemyBossMoon.Behavior.AttackTentacle05:
			return this.abilityAttackTentacle05;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle00:
			return this.abilityAttackQuickTentacle00;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle01:
			return this.abilityAttackQuickTentacle01;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle02:
			return this.abilityAttackQuickTentacle02;
		case GREnemyBossMoon.Behavior.AttackQuickTentacle03:
			return this.abilityAttackQuickTentacle03;
		case GREnemyBossMoon.Behavior.AttackTongue:
			return this.abilityAttackTongue01;
		case GREnemyBossMoon.Behavior.AttackTongueSwipe:
			return this.abilityAttackTongueSwipe01;
		}
		return null;
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x0010AF4C File Offset: 0x0010914C
	private void OnTriggerEnter(Collider collider)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed)
		{
			return;
		}
		if (!this.IsAttackBehavior(this.currBehavior))
		{
			return;
		}
		if (collider.isTrigger)
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
			GRPlayer grplayer = attachedRigidbody.GetComponent<GRPlayer>();
			if (grplayer == null)
			{
				GorillaTagger component3 = attachedRigidbody.GetComponent<GorillaTagger>();
				if (component3 != null && component3.offlineVRRig != null)
				{
					grplayer = component3.offlineVRRig.GetComponent<GRPlayer>();
				}
			}
			if (grplayer != null && grplayer.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
			{
				this.HitPlayer(grplayer, false);
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

	// Token: 0x06003125 RID: 12581 RVA: 0x0010B0DF File Offset: 0x001092DF
	private void TurnOnGrav()
	{
		if (this.currentGravActivator != null)
		{
			return;
		}
		this.currentGravActivator = this.gravActivators[Random.Range(0, this.gravActivators.Length)];
		this.currentGravActivator.SetActive(true);
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x0010B117 File Offset: 0x00109317
	private void TurnOffGrav()
	{
		if (this.currentGravActivator == null)
		{
			return;
		}
		this.currentGravActivator.SetActive(false);
		this.currentGravActivator = null;
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x0010B13B File Offset: 0x0010933B
	[ContextMenu("Debug Hit Player")]
	private void DebugHitPlayer()
	{
		this.HitPlayer(VRRig.LocalRig.GetComponent<GRPlayer>(), true);
	}

	// Token: 0x06003128 RID: 12584 RVA: 0x0010B14E File Offset: 0x0010934E
	public void HitPlayer(GRPlayer player, bool useImpulse = false)
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryHitPlayerCoroutine != null)
		{
			base.StopCoroutine(this.tryHitPlayerCoroutine);
		}
		this.tryHitPlayerCoroutine = base.StartCoroutine(this.TryHitPlayer(player, useImpulse));
	}

	// Token: 0x06003129 RID: 12585 RVA: 0x0010B180 File Offset: 0x00109380
	private IEnumerator TryHitPlayer(GRPlayer player, bool useImpulse = false)
	{
		yield return new WaitForUpdate();
		if (player != null && player.gamePlayer.IsLocal() && Time.time > this.lastHitPlayerTime + this.minTimeBetweenHits)
		{
			this.lastHitPlayerTime = Time.time;
			ICustomKnockbackAbility customKnockbackAbility = this.GetAssociatedAbilityForBehavior(this.currBehavior) as ICustomKnockbackAbility;
			Vector3 vector2;
			if (customKnockbackAbility != null)
			{
				Vector3? vector = customKnockbackAbility.CalculateImpulse(player.transform);
				if (vector != null)
				{
					Vector3 valueOrDefault = vector.GetValueOrDefault();
					vector2 = valueOrDefault;
					goto IL_00F4;
				}
			}
			vector2 = (player.transform.position - this.knockbackTransform.position).normalized * this.knockbackImpulse;
			IL_00F4:
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, player, base.transform.position, vector2);
			this.cameraShaker.Shake();
			float magnitude = vector2.magnitude;
			GorillaTagger.Instance.StartVibration(true, magnitude, 0.333f);
			GorillaTagger.Instance.StartVibration(false, magnitude, 0.333f);
			if (useImpulse)
			{
				GTPlayer.Instance.ApplyKnockback(vector2 / magnitude, magnitude, true);
			}
		}
		yield break;
	}

	// Token: 0x0600312A RID: 12586 RVA: 0x0010B19D File Offset: 0x0010939D
	public void ShockPlayer()
	{
		if (this.currBodyState == GREnemyBossMoon.BodyState.Destroyed || this.tryShockPlayerCoroutine != null)
		{
			return;
		}
		this.tryShockPlayerCoroutine = base.StartCoroutine(this.TryShockPlayer());
	}

	// Token: 0x0600312B RID: 12587 RVA: 0x0010B1C2 File Offset: 0x001093C2
	private IEnumerator TryShockPlayer()
	{
		this.bodyRenderer.sharedMaterials = this.shockedBodyMaterials;
		yield return new WaitForSecondsRealtime(1f);
		this.bodyRenderer.sharedMaterials = this.defaultBodyMaterials;
		this.tryShockPlayerCoroutine = null;
		yield break;
	}

	// Token: 0x0600312C RID: 12588 RVA: 0x0010B1D4 File Offset: 0x001093D4
	private void ToggleShockColliders(bool toggle)
	{
		for (int i = 0; i < this.shockColliders.Count; i++)
		{
			this.shockColliders[i].enabled = toggle;
		}
	}

	// Token: 0x0600312D RID: 12589 RVA: 0x0010B209 File Offset: 0x00109409
	public void GroundSlamWeak(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 0.1f, 6f, 5f);
	}

	// Token: 0x0600312E RID: 12590 RVA: 0x0010B221 File Offset: 0x00109421
	public void GroundSlam(Transform slamCenter)
	{
		this._GroundSlam(slamCenter, 1f, 11f, 8f);
	}

	// Token: 0x0600312F RID: 12591 RVA: 0x0010B23C File Offset: 0x0010943C
	public async void _GroundSlam(Transform slamCenter, float duration, float distance, float hitVelocity)
	{
		Vector3 slamPosition = slamCenter.position;
		float timeHit = Time.time;
		bool playerHit = false;
		GTPlayer player = GTPlayer.Instance;
		float upwardsAngleBoost = 55f;
		if ((player.HeadCenterPosition - slamCenter.position).magnitude < distance * 1.25f)
		{
			this.cameraShaker.Shake();
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 3f, 0.5f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 3f, 0.5f);
		}
		while (!playerHit && Time.time < timeHit + duration)
		{
			if ((player.IsGroundedHand || player.IsGroundedButt) && (player.HeadCenterPosition - slamPosition).magnitude < distance)
			{
				playerHit = true;
			}
			else
			{
				await Awaitable.WaitForSecondsAsync(0.1f, default(CancellationToken));
			}
		}
		if (playerHit)
		{
			Vector3 vector = player.HeadCenterPosition - slamPosition;
			float num = Vector3.Angle(base.transform.forward, Vector3.up);
			vector = Vector3.RotateTowards(vector.normalized, Vector3.up, Mathf.Clamp(num - upwardsAngleBoost, 0f, upwardsAngleBoost) * 0.017453292f, 0f);
			GorillaTagger.Instance.StartVibration(true, GorillaTagger.Instance.tapHapticStrength * 5f, 0.75f);
			GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tapHapticStrength * 5f, 0.75f);
			player.ApplyKnockback(vector, hitVelocity, true);
			GhostReactorManager.Get(this.entity).RequestEnemyHitPlayer(GhostReactor.EnemyType.Chaser, this.entity.id, GRPlayer.GetLocal(), base.transform.position, vector.normalized * hitVelocity);
		}
	}

	// Token: 0x06003130 RID: 12592 RVA: 0x0010B294 File Offset: 0x00109494
	public void GetDebugTextLines(out List<string> strings)
	{
		strings = new List<string>();
		strings.Add(string.Concat(new string[]
		{
			"<color=\"white\">State:</color> <color=\"yellow\">",
			this.currBehavior.ToString(),
			"</color>\n",
			string.Format("<color=\"white\">Phase:</color> <color=\"yellow\">{0}</color>\n", this.GetCurrPhaseIndex()),
			string.Format("<color=\"white\">HP:</color> <color=\"yellow\">{0}</color>", this.hp)
		}));
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x0010B310 File Offset: 0x00109510
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		byte b = (byte)this.currBehavior;
		byte b2 = (byte)this.currBodyState;
		int num = ((this.targetPlayer == null) ? (-1) : this.targetPlayer.ActorNumber);
		writer.Write(b);
		writer.Write(b2);
		writer.Write(this.hp);
		writer.Write(num);
		writer.Write(this.internalPhaseIndex);
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x0010B374 File Offset: 0x00109574
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		GREnemyBossMoon.Behavior behavior = (GREnemyBossMoon.Behavior)reader.ReadByte();
		GREnemyBossMoon.BodyState bodyState = (GREnemyBossMoon.BodyState)reader.ReadByte();
		int num = reader.ReadInt32();
		int num2 = reader.ReadInt32();
		int num3 = reader.ReadInt32();
		this.SetHP(num);
		this.SetBehavior(behavior, true);
		this.SetBodyState(bodyState, true);
		this.targetPlayer = NetworkSystem.Instance.GetPlayer(num2);
		if (num3 != -1)
		{
			if (this.internalPhaseIndex == -1)
			{
				Debug.Log(string.Format("Catching up to boss phase {0}.", num3));
				this.CatchUpPhase(num3);
				return;
			}
			if (num3 != this.internalPhaseIndex)
			{
				Debug.Log(string.Format("Syncing up to boss phase {0}.", this.internalPhaseIndex));
				this.SyncPhase(num3);
			}
		}
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x00023F0C File Offset: 0x0002210C
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x0010B428 File Offset: 0x00109628
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		GRTool gameComponent = this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId);
		if (gameComponent != null)
		{
			switch (hitTypeId)
			{
			case GameHitType.Club:
				this.OnHitByClub(gameComponent, hit);
				return;
			case GameHitType.Flash:
				this.OnHitByFlash(gameComponent, hit);
				return;
			case GameHitType.Shield:
				this.OnHitByShield(gameComponent, hit);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x0010B48C File Offset: 0x0010968C
	private void AddTrackedEntity(GameEntity entityToTrack)
	{
		int netId = entityToTrack.GetNetId();
		this.trackedEntities.AddIfNew(netId);
		if (!this.trackedGameEntities.Contains(entityToTrack))
		{
			this.trackedGameEntities.Add(entityToTrack);
		}
	}

	// Token: 0x06003136 RID: 12598 RVA: 0x0010B4C8 File Offset: 0x001096C8
	private void RemoveTrackedEntity(GameEntity entityToRemove)
	{
		int netId = entityToRemove.GetNetId();
		if (this.trackedEntities.Contains(netId))
		{
			this.trackedEntities.Remove(netId);
		}
		if (this.trackedGameEntities.Contains(entityToRemove))
		{
			this.trackedGameEntities.Remove(entityToRemove);
		}
	}

	// Token: 0x06003137 RID: 12599 RVA: 0x0010B512 File Offset: 0x00109712
	public void OnSummonedEntityInit(GameEntity entity)
	{
		this.AddTrackedEntity(entity);
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0010B51B File Offset: 0x0010971B
	public void OnSummonedEntityDestroy(GameEntity entity)
	{
		this.RemoveTrackedEntity(entity);
	}

	// Token: 0x04003E89 RID: 16009
	public GameEntity entity;

	// Token: 0x04003E8A RID: 16010
	public GameAgent agent;

	// Token: 0x04003E8B RID: 16011
	public GREnemy enemy;

	// Token: 0x04003E8C RID: 16012
	public GameHittable hittable;

	// Token: 0x04003E8D RID: 16013
	[SerializeField]
	private GRAttributes attributes;

	// Token: 0x04003E8E RID: 16014
	public List<GREnemyBossMoon.PhaseDef> phases;

	// Token: 0x04003E8F RID: 16015
	private int internalPhaseIndex = -1;

	// Token: 0x04003E90 RID: 16016
	public List<GREnemyBossMoon.LootPhase> lootPhases;

	// Token: 0x04003E91 RID: 16017
	public GRSenseNearby senseNearby;

	// Token: 0x04003E92 RID: 16018
	public GRSenseLineOfSight senseLineOfSight;

	// Token: 0x04003E93 RID: 16019
	public List<GREnemyBossMoonEye> eyes;

	// Token: 0x04003E94 RID: 16020
	public GRSpherePushVolume eyesPushVolume;

	// Token: 0x04003E95 RID: 16021
	public Animation anim;

	// Token: 0x04003E96 RID: 16022
	public GRAbilityIdle abilityReveal;

	// Token: 0x04003E97 RID: 16023
	private bool firstTimeReveal = true;

	// Token: 0x04003E99 RID: 16025
	public GRAbilityIdle abilityIdle;

	// Token: 0x04003E9A RID: 16026
	public GRAbilityIdle abilityHiddenIdle;

	// Token: 0x04003E9B RID: 16027
	public GRBossMoonTentacleAttack abilityAttackTentacle00;

	// Token: 0x04003E9C RID: 16028
	public GRBossMoonTentacleAttack abilityAttackTentacle01;

	// Token: 0x04003E9D RID: 16029
	public GRBossMoonTentacleAttack abilityAttackTentacle02;

	// Token: 0x04003E9E RID: 16030
	public GRBossMoonTentacleAttack abilityAttackTentacle03;

	// Token: 0x04003E9F RID: 16031
	public GRBossMoonTentacleAttack abilityAttackTentacle04;

	// Token: 0x04003EA0 RID: 16032
	public GRBossMoonTentacleAttack abilityAttackTentacle05;

	// Token: 0x04003EA1 RID: 16033
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle00;

	// Token: 0x04003EA2 RID: 16034
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle01;

	// Token: 0x04003EA3 RID: 16035
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle02;

	// Token: 0x04003EA4 RID: 16036
	public GRBossMoonTentacleAttack abilityAttackQuickTentacle03;

	// Token: 0x04003EA5 RID: 16037
	public GRBossMoonTentacleAttack abilityAttackTongue01;

	// Token: 0x04003EA6 RID: 16038
	public GRBossMoonTentacleAttack abilityAttackTongueSwipe01;

	// Token: 0x04003EA7 RID: 16039
	public GRAbilityIdle abilitySummonStart;

	// Token: 0x04003EA8 RID: 16040
	public GRAbilityIdle abilitySummonEnd;

	// Token: 0x04003EA9 RID: 16041
	public GRAbilitySummon abilitySummon01;

	// Token: 0x04003EAA RID: 16042
	public GRAbilitySummon abilitySummon02;

	// Token: 0x04003EAB RID: 16043
	public GRAbilitySummon abilitySummon03;

	// Token: 0x04003EAC RID: 16044
	public GRAbilitySummon abilitySummon04;

	// Token: 0x04003EAD RID: 16045
	public GRAbilityIdle abilityRetreatStart;

	// Token: 0x04003EAE RID: 16046
	public GRAbilityIdle abilityRetreatEnd;

	// Token: 0x04003EAF RID: 16047
	public GRAbilityIdle abilityRetreatIdle;

	// Token: 0x04003EB0 RID: 16048
	public GRAbilityIdle abilityExposed;

	// Token: 0x04003EB1 RID: 16049
	public GRAbilityIdle abilityExposedIdle;

	// Token: 0x04003EB2 RID: 16050
	public GRAbilityDie abilityDie;

	// Token: 0x04003EB3 RID: 16051
	public GRAbilityDie abilityDieIdle;

	// Token: 0x04003EB4 RID: 16052
	public GRAbilityDie abilityRunaway;

	// Token: 0x04003EB5 RID: 16053
	public GRAbilityIdle abilityNextPhase;

	// Token: 0x04003EB6 RID: 16054
	private GRAbilityBase[] abilities;

	// Token: 0x04003EB7 RID: 16055
	private GRAbilityBase currAbility;

	// Token: 0x04003EB8 RID: 16056
	private GRAbilitySummon currSummon;

	// Token: 0x04003EB9 RID: 16057
	public GRAbilityAgent abilityAgent;

	// Token: 0x04003EBA RID: 16058
	public List<Renderer> bones;

	// Token: 0x04003EBB RID: 16059
	public List<Renderer> always;

	// Token: 0x04003EBC RID: 16060
	public Transform headTransform;

	// Token: 0x04003EBD RID: 16061
	public AudioSource audioSource;

	// Token: 0x04003EBE RID: 16062
	public AudioClip damagedSound;

	// Token: 0x04003EBF RID: 16063
	public float damagedSoundVolume;

	// Token: 0x04003EC0 RID: 16064
	public List<AudioClip> damagedSounds;

	// Token: 0x04003EC1 RID: 16065
	private int damagedSoundIndex;

	// Token: 0x04003EC2 RID: 16066
	public GameObject fxDamaged;

	// Token: 0x04003EC3 RID: 16067
	public GameObject[] gravActivators;

	// Token: 0x04003EC4 RID: 16068
	private GameObject currentGravActivator;

	// Token: 0x04003EC5 RID: 16069
	public Renderer bodyRenderer;

	// Token: 0x04003EC6 RID: 16070
	public Material[] defaultBodyMaterials;

	// Token: 0x04003EC7 RID: 16071
	public Material[] shockedBodyMaterials;

	// Token: 0x04003EC8 RID: 16072
	private float lastStaggerTime;

	// Token: 0x04003EC9 RID: 16073
	public float staggerImmuneTime = 10f;

	// Token: 0x04003ECA RID: 16074
	private Transform target;

	// Token: 0x04003ECB RID: 16075
	[ReadOnly]
	public int hp;

	// Token: 0x04003ECC RID: 16076
	[ReadOnly]
	public GREnemyBossMoon.Behavior currBehavior;

	// Token: 0x04003ECD RID: 16077
	[ReadOnly]
	public GREnemyBossMoon.BodyState currBodyState;

	// Token: 0x04003ECE RID: 16078
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x04003ECF RID: 16079
	[ReadOnly]
	public Vector3 lastSeenTargetPosition;

	// Token: 0x04003ED0 RID: 16080
	[ReadOnly]
	public double lastSeenTargetTime;

	// Token: 0x04003ED1 RID: 16081
	[ReadOnly]
	public Vector3 searchPosition;

	// Token: 0x04003ED2 RID: 16082
	private GREnemyBossMoon.Behavior lastBehavior;

	// Token: 0x04003ED3 RID: 16083
	private bool restAfterAttack;

	// Token: 0x04003ED4 RID: 16084
	private int consecutiveCombos;

	// Token: 0x04003ED5 RID: 16085
	private int attacksAfterSummon = 3;

	// Token: 0x04003ED6 RID: 16086
	private float waitInRetreat;

	// Token: 0x04003ED7 RID: 16087
	private double lastJumpEndtime;

	// Token: 0x04003ED8 RID: 16088
	public bool canChaseJump = true;

	// Token: 0x04003ED9 RID: 16089
	public float chaseJumpDistance = 5f;

	// Token: 0x04003EDA RID: 16090
	public float chaseJumpMinInterval = 1f;

	// Token: 0x04003EDB RID: 16091
	public float minChaseJumpDistance = 2f;

	// Token: 0x04003EDC RID: 16092
	public float knockbackImpulse = 11f;

	// Token: 0x04003EDD RID: 16093
	public Transform knockbackTransform;

	// Token: 0x04003EDE RID: 16094
	private Rigidbody rigidBody;

	// Token: 0x04003EDF RID: 16095
	private List<Collider> colliders;

	// Token: 0x04003EE0 RID: 16096
	private float lastHitPlayerTime;

	// Token: 0x04003EE1 RID: 16097
	private float minTimeBetweenHits = 2f;

	// Token: 0x04003EE2 RID: 16098
	public float hearingRadius = 5f;

	// Token: 0x04003EE3 RID: 16099
	public List<GREnemyBossMoonColliderHelper> shockColliders;

	// Token: 0x04003EE4 RID: 16100
	public List<GRSquishVolume> squishVolumes;

	// Token: 0x04003EE5 RID: 16101
	public CameraShakeDispatcher cameraShaker;

	// Token: 0x04003EE6 RID: 16102
	private List<int> trackedEntities;

	// Token: 0x04003EE7 RID: 16103
	private List<GameEntity> trackedGameEntities;

	// Token: 0x04003EE8 RID: 16104
	private GRAdaptiveMusicController adaptiveMusicController;

	// Token: 0x04003EE9 RID: 16105
	private bool triggerNextMusicTransition;

	// Token: 0x04003EEA RID: 16106
	private static List<VRRig> tempRigs = new List<VRRig>(16);

	// Token: 0x04003EEB RID: 16107
	private static List<GREnemyBossMoon.Behavior> tempPotentialAttacks = new List<GREnemyBossMoon.Behavior>(16);

	// Token: 0x04003EEC RID: 16108
	private Coroutine tryHitPlayerCoroutine;

	// Token: 0x04003EED RID: 16109
	private Coroutine tryShockPlayerCoroutine;

	// Token: 0x0200078D RID: 1933
	[Serializable]
	public class PhaseDef
	{
		// Token: 0x04003EEE RID: 16110
		public int minHP;

		// Token: 0x04003EEF RID: 16111
		public List<GREnemyBossMoon.Behavior> attacks;

		// Token: 0x04003EF0 RID: 16112
		public List<GREnemyBossMoon.Behavior> comboAttacks;

		// Token: 0x04003EF1 RID: 16113
		public bool restAfterAttack = true;

		// Token: 0x04003EF2 RID: 16114
		public float comboAttackChance = 0.25f;

		// Token: 0x04003EF3 RID: 16115
		public bool allowConsecutiveCombos;

		// Token: 0x04003EF4 RID: 16116
		public List<GREnemyBossMoon.Behavior> summons;

		// Token: 0x04003EF5 RID: 16117
		public int maxSimultaneousEnemies = 6;

		// Token: 0x04003EF6 RID: 16118
		public int maxEnemiesForReveal = 4;

		// Token: 0x04003EF7 RID: 16119
		public int attacksBetweenSummons = 4;

		// Token: 0x04003EF8 RID: 16120
		public bool retreatAfterSummon = true;

		// Token: 0x04003EF9 RID: 16121
		public float randomSummonChance = 0.1f;

		// Token: 0x04003EFA RID: 16122
		public bool runawayAfterPhase;
	}

	// Token: 0x0200078E RID: 1934
	[Serializable]
	public class LootPhase
	{
		// Token: 0x04003EFB RID: 16123
		public GREnemyType enemyType;

		// Token: 0x04003EFC RID: 16124
		public GRBreakableItemSpawnConfig lootTable;
	}

	// Token: 0x0200078F RID: 1935
	public enum Behavior
	{
		// Token: 0x04003EFE RID: 16126
		HiddenIdle,
		// Token: 0x04003EFF RID: 16127
		Idle,
		// Token: 0x04003F00 RID: 16128
		Reveal,
		// Token: 0x04003F01 RID: 16129
		Exposed,
		// Token: 0x04003F02 RID: 16130
		ExposedIdle,
		// Token: 0x04003F03 RID: 16131
		Stagger,
		// Token: 0x04003F04 RID: 16132
		Dying,
		// Token: 0x04003F05 RID: 16133
		AttackTentacle00,
		// Token: 0x04003F06 RID: 16134
		AttackTentacle01,
		// Token: 0x04003F07 RID: 16135
		AttackTentacle02,
		// Token: 0x04003F08 RID: 16136
		AttackTentacle03,
		// Token: 0x04003F09 RID: 16137
		AttackTentacle04,
		// Token: 0x04003F0A RID: 16138
		AttackTentacle05,
		// Token: 0x04003F0B RID: 16139
		AttackQuickTentacle00,
		// Token: 0x04003F0C RID: 16140
		AttackQuickTentacle01,
		// Token: 0x04003F0D RID: 16141
		AttackQuickTentacle02,
		// Token: 0x04003F0E RID: 16142
		AttackQuickTentacle03,
		// Token: 0x04003F0F RID: 16143
		AttackTongue,
		// Token: 0x04003F10 RID: 16144
		SummonStart,
		// Token: 0x04003F11 RID: 16145
		SummonEnd,
		// Token: 0x04003F12 RID: 16146
		Summon01,
		// Token: 0x04003F13 RID: 16147
		Summon02,
		// Token: 0x04003F14 RID: 16148
		Summon03,
		// Token: 0x04003F15 RID: 16149
		Summon04,
		// Token: 0x04003F16 RID: 16150
		RetreatStart,
		// Token: 0x04003F17 RID: 16151
		RetreatEnd,
		// Token: 0x04003F18 RID: 16152
		RetreatIdle,
		// Token: 0x04003F19 RID: 16153
		DyingIdle,
		// Token: 0x04003F1A RID: 16154
		Runaway,
		// Token: 0x04003F1B RID: 16155
		AttackTongueSwipe,
		// Token: 0x04003F1C RID: 16156
		NextPhase,
		// Token: 0x04003F1D RID: 16157
		None,
		// Token: 0x04003F1E RID: 16158
		Count
	}

	// Token: 0x02000790 RID: 1936
	public enum BodyState
	{
		// Token: 0x04003F20 RID: 16160
		Destroyed,
		// Token: 0x04003F21 RID: 16161
		Bones,
		// Token: 0x04003F22 RID: 16162
		Shell,
		// Token: 0x04003F23 RID: 16163
		Count
	}
}
