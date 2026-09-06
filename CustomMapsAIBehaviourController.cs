using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaGameModes;
using GorillaTagScripts.VirtualStumpCustomMaps;
using GT_CustomMapSupportRuntime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A71 RID: 2673
public class CustomMapsAIBehaviourController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x17000690 RID: 1680
	// (get) Token: 0x060044BF RID: 17599 RVA: 0x0016FA86 File Offset: 0x0016DC86
	// (set) Token: 0x060044BE RID: 17598 RVA: 0x0016FA7D File Offset: 0x0016DC7D
	public GRPlayer TargetPlayer { get; private set; }

	// Token: 0x060044C0 RID: 17600 RVA: 0x0016FA90 File Offset: 0x0016DC90
	private void Awake()
	{
		this.TargetPlayer = null;
		this.visibilityLayerMask = LayerMask.GetMask(new string[] { "Default", "Gorilla Object" });
		this.agent.onBehaviorStateChanged += this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x060044C1 RID: 17601 RVA: 0x0016FAE1 File Offset: 0x0016DCE1
	private void OnDestroy()
	{
		this.agent.onBehaviorStateChanged -= this.OnNetworkBehaviourStateChanged;
	}

	// Token: 0x060044C2 RID: 17602 RVA: 0x0016FAFA File Offset: 0x0016DCFA
	public void SetTarget(GRPlayer newTarget)
	{
		if (newTarget.IsNull())
		{
			this.ClearTarget();
			return;
		}
		this.TargetPlayer = newTarget;
	}

	// Token: 0x060044C3 RID: 17603 RVA: 0x0016FB12 File Offset: 0x0016DD12
	public void ClearTarget()
	{
		this.TargetPlayer = null;
	}

	// Token: 0x060044C4 RID: 17604 RVA: 0x0016FB1B File Offset: 0x0016DD1B
	private void Update()
	{
		this.OnThink();
		this.UpdateAnimators();
	}

	// Token: 0x060044C5 RID: 17605 RVA: 0x0016FB2C File Offset: 0x0016DD2C
	private void OnTriggerEnter(Collider collider)
	{
		CustomMapsBehaviourBase customMapsBehaviourBase = this.behaviourDict[this.currentBehaviour];
		if (customMapsBehaviourBase != null)
		{
			customMapsBehaviourBase.OnTriggerEnter(collider);
		}
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x0016FB55 File Offset: 0x0016DD55
	private void InitAnimators()
	{
		this.animators = base.gameObject.GetComponentsInChildren<Animator>();
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x0016FB68 File Offset: 0x0016DD68
	private void UpdateAnimators()
	{
		if (this.animators.IsNullOrEmpty<Animator>())
		{
			return;
		}
		float magnitude = this.agent.navAgent.velocity.magnitude;
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].SetFloat(CustomMapsAIBehaviourController.movementSpeedParamIndex, magnitude);
		}
	}

	// Token: 0x060044C8 RID: 17608 RVA: 0x0016FBC4 File Offset: 0x0016DDC4
	public void PlayAnimation(string stateName, float blendTime = 0f)
	{
		for (int i = 0; i < this.animators.Length; i++)
		{
			this.animators[i].CrossFadeInFixedTime(stateName, blendTime);
		}
	}

	// Token: 0x060044C9 RID: 17609 RVA: 0x0016FBF4 File Offset: 0x0016DDF4
	public bool IsAnimationPlaying(string stateName)
	{
		int num = 0;
		if (num >= this.animators.Length)
		{
			return false;
		}
		Animator animator = this.animators[num];
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		return (currentAnimatorStateInfo.IsName(stateName) && currentAnimatorStateInfo.normalizedTime < 1f) || animator.GetNextAnimatorStateInfo(0).IsName(stateName);
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0016FC54 File Offset: 0x0016DE54
	public void SetupBehaviours(AIAgent aiAgent)
	{
		this.allowTargetingTaggedPlayers = aiAgent.allowTargetingTaggedPlayers;
		for (int i = 0; i < aiAgent.agentBehaviours.Count; i++)
		{
			if (!this.usedBehaviours.Contains(aiAgent.agentBehaviours[i]))
			{
				switch (aiAgent.agentBehaviours[i])
				{
				case AgentBehaviours.Search:
					this.behaviourDict[AgentBehaviours.Search] = new CustomMapsSearchBehaviour(this, aiAgent);
					break;
				case AgentBehaviours.Chase:
					this.behaviourDict[AgentBehaviours.Chase] = new CustomMapsChaseBehaviour(this, aiAgent);
					break;
				case AgentBehaviours.Attack:
					this.behaviourDict[AgentBehaviours.Attack] = new CustomMapsAttackBehaviour(this, aiAgent);
					break;
				default:
					goto IL_00A1;
				}
				this.usedBehaviours.Add(aiAgent.agentBehaviours[i]);
			}
			IL_00A1:;
		}
	}

	// Token: 0x060044CB RID: 17611 RVA: 0x0016FD17 File Offset: 0x0016DF17
	public void StopMoving()
	{
		this.RequestDestination(base.transform.position);
	}

	// Token: 0x060044CC RID: 17612 RVA: 0x0016FD2A File Offset: 0x0016DF2A
	public void RequestDestination(Vector3 destination)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		this.agent.RequestDestination(destination);
	}

	// Token: 0x060044CD RID: 17613 RVA: 0x0016FD48 File Offset: 0x0016DF48
	private void OnThink()
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (this.behaviourDict == null || this.behaviourDict.Count == 0)
		{
			return;
		}
		int num = -1;
		if (this.currentBehaviourIndex != -1 && this.behaviourDict[this.usedBehaviours[this.currentBehaviourIndex]].CanContinueExecuting())
		{
			num = this.currentBehaviourIndex;
		}
		else
		{
			for (int i = 0; i < this.usedBehaviours.Count; i++)
			{
				if (i != this.currentBehaviourIndex && this.behaviourDict[this.usedBehaviours[i]].CanExecute())
				{
					num = i;
					break;
				}
			}
		}
		if (num == -1)
		{
			return;
		}
		if (this.currentBehaviourIndex != num)
		{
			this.currentBehaviourIndex = num;
			this.currentBehaviour = this.usedBehaviours[num];
			this.agent.RequestBehaviorChange((byte)this.currentBehaviour);
		}
		this.behaviourDict[this.currentBehaviour].Execute();
	}

	// Token: 0x060044CE RID: 17614 RVA: 0x0016FE40 File Offset: 0x0016E040
	private void OnNetworkBehaviourStateChanged(byte newstate)
	{
		if (newstate < 0 || newstate >= 3)
		{
			return;
		}
		if (!this.behaviourDict.ContainsKey((AgentBehaviours)newstate))
		{
			return;
		}
		if (this.currentBehaviour != (AgentBehaviours)newstate && this.behaviourDict.ContainsKey(this.currentBehaviour))
		{
			this.behaviourDict[this.currentBehaviour].ResetBehavior();
		}
		this.currentBehaviour = (AgentBehaviours)newstate;
		this.behaviourDict[this.currentBehaviour].NetExecute();
	}

	// Token: 0x060044CF RID: 17615 RVA: 0x0016FEB8 File Offset: 0x0016E0B8
	public void OnEntityInit()
	{
		bool flag = AISpawnManager.HasInstance && AISpawnManager.instance != null;
		if (!flag && MapSpawnManager.instance == null)
		{
			return;
		}
		this.entity.transform.parent = (flag ? AISpawnManager.instance.transform : MapSpawnManager.instance.transform);
		byte b;
		AIAgent.UnpackCreateData(this.entity.createData, out b, out this.luaAgentID);
		AIAgent aiagent;
		if (flag && AISpawnManager.instance.SpawnEnemy((int)b, out aiagent))
		{
			this.SetupNewEnemy(aiagent);
			return;
		}
		MapEntity mapEntity;
		if (!flag && MapSpawnManager.instance.SpawnEntity((int)b, out mapEntity))
		{
			this.SetupNewEnemy((AIAgent)mapEntity);
			return;
		}
		GTDev.LogError<string>("CustomMapsAIBehaviourController::OnEntityInit could not spawn enemy", null);
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060044D0 RID: 17616 RVA: 0x0016FF7C File Offset: 0x0016E17C
	private void SetupNewEnemy(AIAgent newEnemy)
	{
		newEnemy.gameObject.SetActive(true);
		newEnemy.transform.parent = this.entity.transform;
		newEnemy.transform.localPosition = Vector3.zero;
		newEnemy.transform.localRotation = Quaternion.identity;
		this.InitAnimators();
		NavMeshAgent component = this.entity.gameObject.GetComponent<NavMeshAgent>();
		if (component.IsNull())
		{
			GTDev.LogError<string>("nav mesh agent is null", null);
			Object.Destroy(base.gameObject);
			return;
		}
		component.agentTypeID = this.GetNavAgentType(newEnemy.navAgentType);
		component.speed = newEnemy.movementSpeed;
		component.angularSpeed = newEnemy.turnSpeed;
		component.acceleration = newEnemy.acceleration;
		this.SetupBehaviours(newEnemy);
	}

	// Token: 0x060044D1 RID: 17617 RVA: 0x00170040 File Offset: 0x0016E240
	private int GetNavAgentType(NavAgentType navType)
	{
		int settingsCount = NavMesh.GetSettingsCount();
		int num = NavMesh.GetSettingsByIndex(0).agentTypeID;
		for (int i = 0; i < settingsCount; i++)
		{
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(i);
			if (NavMesh.GetSettingsNameFromID(settingsByIndex.agentTypeID) == navType.ToString())
			{
				num = settingsByIndex.agentTypeID;
				break;
			}
		}
		return num;
	}

	// Token: 0x060044D2 RID: 17618 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060044D3 RID: 17619 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x060044D4 RID: 17620 RVA: 0x001700A4 File Offset: 0x0016E2A4
	public GRPlayer FindBestTarget(Vector3 sourcePos, float maxRange, float maxRangeSq, float minDotVal)
	{
		float num = 0f;
		GRPlayer grplayer = null;
		this.tempRigs.Clear();
		this.tempRigs.Add(VRRig.LocalRig);
		VRRigCache.Instance.GetAllUsedRigs(this.tempRigs);
		Vector3 vector = base.transform.rotation * Vector3.forward;
		for (int i = 0; i < this.tempRigs.Count; i++)
		{
			GRPlayer component = this.tempRigs[i].GetComponent<GRPlayer>();
			Vector3 vector2;
			if (this.IsTargetInRange(sourcePos, component, maxRangeSq, out vector2))
			{
				float num2 = 0f;
				if (vector2.sqrMagnitude > 0f)
				{
					num2 = Mathf.Sqrt(vector2.magnitude);
				}
				float num3 = Vector3.Dot(vector2.normalized, vector);
				if (num3 >= minDotVal)
				{
					float num4 = Mathf.Lerp(0f, 0.5f, 1f - num2 / maxRange);
					float num5 = Mathf.Lerp(0f, 0.5f, (1f - minDotVal - (1f - num3)) / (1f - minDotVal));
					if (num4 + num5 > num && this.IsTargetVisible(sourcePos, component, maxRange))
					{
						num = num4 + num5;
						grplayer = component;
					}
				}
			}
		}
		return grplayer;
	}

	// Token: 0x060044D5 RID: 17621 RVA: 0x001701D8 File Offset: 0x0016E3D8
	public bool IsTargetVisible(Vector3 startPos, GRPlayer target, float maxDist)
	{
		if (!this.IsTargetable(target))
		{
			return false;
		}
		int num = Physics.RaycastNonAlloc(new Ray(startPos, target.transform.position - startPos), CustomMapsAIBehaviourController.visibilityHits, Mathf.Min(Vector3.Distance(target.transform.position, startPos), maxDist), this.visibilityLayerMask.value, QueryTriggerInteraction.Ignore);
		for (int i = 0; i < num; i++)
		{
			if (CustomMapsAIBehaviourController.visibilityHits[i].transform != base.transform && !CustomMapsAIBehaviourController.visibilityHits[i].transform.IsChildOf(base.transform))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060044D6 RID: 17622 RVA: 0x00170280 File Offset: 0x0016E480
	public bool IsTargetInRange(Vector3 startPos, GRPlayer target, float maxRangeSq, out Vector3 toTarget)
	{
		toTarget = Vector3.zero;
		if (!this.IsTargetable(target))
		{
			return false;
		}
		Vector3 position = target.transform.position;
		toTarget = position - startPos;
		return toTarget.sqrMagnitude <= maxRangeSq;
	}

	// Token: 0x060044D7 RID: 17623 RVA: 0x001702CC File Offset: 0x0016E4CC
	public bool IsTargetable(GRPlayer potentialTarget)
	{
		if (potentialTarget.IsNull())
		{
			return false;
		}
		if (potentialTarget.State == GRPlayer.GRPlayerState.Ghost)
		{
			return false;
		}
		if (potentialTarget.MyRig.isLocal)
		{
			if (CustomMapManager.IsLocalPlayerInVirtualStump())
			{
				return false;
			}
		}
		else if (CustomMapManager.IsRemotePlayerInVirtualStump(potentialTarget.MyRig.OwningNetPlayer.UserId))
		{
			return false;
		}
		return this.allowTargetingTaggedPlayers || GameMode.ActiveGameMode.GameType() == GameModeType.Custom || !GameMode.LocalIsTagged(potentialTarget.MyRig.OwningNetPlayer);
	}

	// Token: 0x040056D5 RID: 22229
	private static readonly int movementSpeedParamIndex = Animator.StringToHash("MovementSpeed");

	// Token: 0x040056D6 RID: 22230
	public GameEntity entity;

	// Token: 0x040056D7 RID: 22231
	public GameAgent agent;

	// Token: 0x040056D8 RID: 22232
	public GRAttributes attributes;

	// Token: 0x040056D9 RID: 22233
	private Animator[] animators;

	// Token: 0x040056DA RID: 22234
	public short luaAgentID;

	// Token: 0x040056DB RID: 22235
	private List<VRRig> tempRigs = new List<VRRig>(20);

	// Token: 0x040056DC RID: 22236
	private static RaycastHit[] visibilityHits = new RaycastHit[10];

	// Token: 0x040056DD RID: 22237
	private LayerMask visibilityLayerMask;

	// Token: 0x040056DE RID: 22238
	private bool allowTargetingTaggedPlayers;

	// Token: 0x040056E0 RID: 22240
	private Dictionary<AgentBehaviours, CustomMapsBehaviourBase> behaviourDict = new Dictionary<AgentBehaviours, CustomMapsBehaviourBase>(8);

	// Token: 0x040056E1 RID: 22241
	private List<AgentBehaviours> usedBehaviours = new List<AgentBehaviours>(8);

	// Token: 0x040056E2 RID: 22242
	private AgentBehaviours currentBehaviour;

	// Token: 0x040056E3 RID: 22243
	private int currentBehaviourIndex;

	// Token: 0x040056E4 RID: 22244
	private const int BEHAVIOUR_COUNT = 3;

	// Token: 0x02000A72 RID: 2674
	public enum CustomMapsAIBehaviour
	{
		// Token: 0x040056E6 RID: 22246
		Search,
		// Token: 0x040056E7 RID: 22247
		Chase,
		// Token: 0x040056E8 RID: 22248
		Attack
	}
}
