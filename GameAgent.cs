using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x020006B1 RID: 1713
public class GameAgent : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x14000050 RID: 80
	// (add) Token: 0x06002AA5 RID: 10917 RVA: 0x000E5AF8 File Offset: 0x000E3CF8
	// (remove) Token: 0x06002AA6 RID: 10918 RVA: 0x000E5B30 File Offset: 0x000E3D30
	public event GameAgent.StateChangedEvent onBodyStateChanged;

	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06002AA7 RID: 10919 RVA: 0x000E5B68 File Offset: 0x000E3D68
	// (remove) Token: 0x06002AA8 RID: 10920 RVA: 0x000E5BA0 File Offset: 0x000E3DA0
	public event GameAgent.StateChangedEvent onBehaviorStateChanged;

	// Token: 0x14000052 RID: 82
	// (add) Token: 0x06002AA9 RID: 10921 RVA: 0x000E5BD8 File Offset: 0x000E3DD8
	// (remove) Token: 0x06002AAA RID: 10922 RVA: 0x000E5C10 File Offset: 0x000E3E10
	public event GameAgent.NavigationLinkReachedEvent onReachedNavigationLink;

	// Token: 0x14000053 RID: 83
	// (add) Token: 0x06002AAB RID: 10923 RVA: 0x000E5C48 File Offset: 0x000E3E48
	// (remove) Token: 0x06002AAC RID: 10924 RVA: 0x000E5C80 File Offset: 0x000E3E80
	public event GameAgent.JumpRequestedEvent onJumpRequested;

	// Token: 0x14000054 RID: 84
	// (add) Token: 0x06002AAD RID: 10925 RVA: 0x000E5CB8 File Offset: 0x000E3EB8
	// (remove) Token: 0x06002AAE RID: 10926 RVA: 0x000E5CF0 File Offset: 0x000E3EF0
	public event GameAgent.NavigationFailedEvent onNavigationFailed;

	// Token: 0x06002AAF RID: 10927 RVA: 0x000E5D25 File Offset: 0x000E3F25
	public GameAgentManager GetGameAgentManager()
	{
		return this.entity.manager.gameAgentManager;
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x000E5D37 File Offset: 0x000E3F37
	private void Awake()
	{
		this.agentComponents = new List<IGameAgentComponent>(1);
		base.GetComponentsInChildren<IGameAgentComponent>(this.agentComponents);
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000E5D51 File Offset: 0x000E3F51
	public void OnEntityInit()
	{
		this.GetGameAgentManager().AddGameAgent(this);
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000E5D5F File Offset: 0x000E3F5F
	public void OnEntityDestroy()
	{
		this.GetGameAgentManager().RemoveGameAgent(this);
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x00002C2D File Offset: 0x00000E2D
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000E5D6D File Offset: 0x000E3F6D
	public void OnBehaviorStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBehaviorStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000E5D80 File Offset: 0x000E3F80
	public void OnBodyStateChanged(byte newState)
	{
		GameAgent.StateChangedEvent stateChangedEvent = this.onBodyStateChanged;
		if (stateChangedEvent == null)
		{
			return;
		}
		stateChangedEvent(newState);
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000E5D94 File Offset: 0x000E3F94
	public void OnThink(float deltaTime)
	{
		if (!this.pauseEntityThink)
		{
			for (int i = 0; i < this.agentComponents.Count; i++)
			{
				this.agentComponents[i].OnEntityThink(deltaTime);
			}
		}
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000E5DD4 File Offset: 0x000E3FD4
	public void OnUpdate()
	{
		if (this.navAgent == null)
		{
			return;
		}
		if (this.navAgent.isOnNavMesh)
		{
			this.lastPosOnNavMesh = this.navAgent.transform.position;
		}
		if (!this.navAgent.autoTraverseOffMeshLink && !this.wasOnOffMeshNavLink && this.navAgent.isOnOffMeshLink)
		{
			if (this.entity.IsAuthority())
			{
				if ((this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.startPos).sqrMagnitude < (this.navAgent.transform.position - this.navAgent.currentOffMeshLinkData.endPos).sqrMagnitude)
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.endPos, 1f, 1f);
				}
				else
				{
					this.GetGameAgentManager().RequestJump(this, this.navAgent.transform.position, this.navAgent.currentOffMeshLinkData.startPos, 1f, 1f);
				}
			}
			GameAgent.NavigationLinkReachedEvent navigationLinkReachedEvent = this.onReachedNavigationLink;
			if (navigationLinkReachedEvent != null)
			{
				navigationLinkReachedEvent(this.navAgent.currentOffMeshLinkData);
			}
		}
		this.wasOnOffMeshNavLink = this.navAgent.isOnOffMeshLink;
		if (!this.hasNotifiedNavigationFailure && !this.navAgent.pathPending && (this.navAgent.pathStatus == NavMeshPathStatus.PathPartial || this.navAgent.pathStatus == NavMeshPathStatus.PathInvalid))
		{
			GameAgent.NavigationFailedEvent navigationFailedEvent = this.onNavigationFailed;
			if (navigationFailedEvent != null)
			{
				navigationFailedEvent(this.navAgent.pathStatus, this.navAgent.destination, this.navAgent.remainingDistance);
			}
			this.hasNotifiedNavigationFailure = true;
		}
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000E5FBD File Offset: 0x000E41BD
	public void OnJumpRequested(Vector3 start, Vector3 end, float heightScale, float speedScale)
	{
		GameAgent.JumpRequestedEvent jumpRequestedEvent = this.onJumpRequested;
		if (jumpRequestedEvent == null)
		{
			return;
		}
		jumpRequestedEvent(start, end, heightScale, speedScale);
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000E5FD4 File Offset: 0x000E41D4
	public bool IsOnNavMesh()
	{
		return this.navAgent != null && this.navAgent.isOnNavMesh;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x000E5FF1 File Offset: 0x000E41F1
	public Vector3 GetLastPosOnNavMesh()
	{
		return this.lastPosOnNavMesh;
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000E5FFC File Offset: 0x000E41FC
	public void RequestDestination(Vector3 dest)
	{
		if (!this.entity.IsAuthority())
		{
			return;
		}
		if (!this.IsOnNavMesh())
		{
			dest = this.lastPosOnNavMesh;
		}
		if (Vector3.Distance(this.lastRequestedDest, dest) < 0.5f)
		{
			return;
		}
		this.lastRequestedDest = dest;
		if (this.entity.IsAuthority())
		{
			this.GetGameAgentManager().RequestDestination(this, dest);
		}
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000E605C File Offset: 0x000E425C
	public void RequestBehaviorChange(byte behavior)
	{
		this.GetGameAgentManager().RequestBehavior(this, behavior);
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000E606B File Offset: 0x000E426B
	public void RequestStateChange(byte state)
	{
		this.GetGameAgentManager().RequestState(this, state);
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000E607A File Offset: 0x000E427A
	public void RequestTarget(NetPlayer targetPlayer)
	{
		this.GetGameAgentManager().RequestTarget(this, targetPlayer);
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000E608C File Offset: 0x000E428C
	public void ApplyDestination(Vector3 dest)
	{
		NavMeshHit navMeshHit;
		if (!NavMesh.SamplePosition(dest, out navMeshHit, 1.5f, -1))
		{
			return;
		}
		dest = navMeshHit.position;
		this.lastReceivedDest = dest;
		this.hasNotifiedNavigationFailure = false;
		if (this.navAgent != null && this.navAgent.isOnNavMesh)
		{
			this.navAgent.destination = dest;
		}
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000E60E8 File Offset: 0x000E42E8
	public void SetDisableNetworkSync(bool disable)
	{
		this.disableNetworkSync = disable;
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000E60F1 File Offset: 0x000E42F1
	public void SetIsPathing(bool isPathing, bool ignoreRigiBody = false)
	{
		if (this.navAgent != null)
		{
			this.navAgent.enabled = isPathing;
		}
		if (!ignoreRigiBody && this.rigidBody != null)
		{
			this.rigidBody.isKinematic = isPathing;
		}
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000E612A File Offset: 0x000E432A
	public void SetStopped(bool stopMovement)
	{
		if (this.navAgent != null)
		{
			this.navAgent.isStopped = stopMovement;
		}
	}

	// Token: 0x06002AC3 RID: 10947 RVA: 0x000E6146 File Offset: 0x000E4346
	public void SetSpeed(float speed)
	{
		if (this.navAgent != null)
		{
			this.navAgent.speed = speed;
		}
	}

	// Token: 0x06002AC4 RID: 10948 RVA: 0x000E6162 File Offset: 0x000E4362
	public void SetVelocity(Vector3 vel)
	{
		if (this.navAgent != null)
		{
			this.navAgent.velocity = vel;
		}
	}

	// Token: 0x06002AC5 RID: 10949 RVA: 0x000E617E File Offset: 0x000E437E
	public void ClearLastRequestedDestination()
	{
		this.lastRequestedDest = Vector3.one * 10000f;
	}

	// Token: 0x06002AC6 RID: 10950 RVA: 0x000E6198 File Offset: 0x000E4398
	public void ApplyNetworkUpdate(Vector3 position, Quaternion rotation)
	{
		if (this.disableNetworkSync)
		{
			return;
		}
		if ((base.transform.position - position).sqrMagnitude > this.networkPositionCorrectionDist * this.networkPositionCorrectionDist && this.navAgent != null)
		{
			this.navAgent.Warp(position);
			this.navAgent.destination = this.lastReceivedDest;
		}
		base.transform.rotation = rotation;
		if (this.rigidBody != null)
		{
			this.rigidBody.rotation = rotation;
		}
	}

	// Token: 0x06002AC7 RID: 10951 RVA: 0x000E6228 File Offset: 0x000E4428
	public static void UpdateFacing(Transform transform, NavMeshAgent navAgent, NetPlayer targetPlayer, float turnspeed = 3600f)
	{
		Transform transform2 = null;
		Vector3 forward = transform.forward;
		if (targetPlayer != null)
		{
			GRPlayer grplayer = GRPlayer.Get(targetPlayer.ActorNumber);
			if (grplayer != null && grplayer.State == GRPlayer.GRPlayerState.Alive)
			{
				transform2 = grplayer.transform;
			}
		}
		GameAgent.UpdateFacingTarget(transform, navAgent, transform2, turnspeed);
	}

	// Token: 0x06002AC8 RID: 10952 RVA: 0x000E6270 File Offset: 0x000E4470
	public static void UpdateFacingTarget(Transform transform, NavMeshAgent navAgent, Transform target, float turnspeed = 3600f)
	{
		Vector3 vector = transform.forward;
		if (target != null)
		{
			Vector3 position = target.position;
			Vector3 position2 = transform.position;
			Vector3 vector2 = position - position2;
			vector2.y = 0f;
			float magnitude = vector2.magnitude;
			if (magnitude > 0f)
			{
				vector = vector2 / magnitude;
			}
		}
		else
		{
			Vector3 vector3 = ((navAgent == null) ? Vector3.zero : navAgent.desiredVelocity);
			vector3.y = 0f;
			float magnitude2 = vector3.magnitude;
			if (magnitude2 > 0f)
			{
				vector = vector3 / magnitude2;
			}
		}
		Quaternion quaternion = Quaternion.LookRotation(vector);
		if (navAgent != null && navAgent.speed > 0f)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed * navAgent.speed / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
			return;
		}
		transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x06002AC9 RID: 10953 RVA: 0x000E639C File Offset: 0x000E459C
	public static void UpdateFacingForward(Transform transform, NavMeshAgent navAgent, float turnspeed = 3600f)
	{
		Vector3 vector = ((navAgent == null) ? Vector3.zero : navAgent.desiredVelocity);
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude <= 0f)
		{
			return;
		}
		Vector3 vector2 = vector / magnitude;
		GameAgent.UpdateFacingDir(transform, navAgent, vector2, turnspeed);
	}

	// Token: 0x06002ACA RID: 10954 RVA: 0x000E63F0 File Offset: 0x000E45F0
	public static void UpdateFacingPos(Transform transform, NavMeshAgent navAgent, Vector3 facingPos, float turnspeed = 3600f)
	{
		Vector3 vector = facingPos - transform.position;
		vector.y = 0f;
		vector.Normalize();
		GameAgent.UpdateFacingDir(transform, navAgent, vector, turnspeed);
	}

	// Token: 0x06002ACB RID: 10955 RVA: 0x000E6428 File Offset: 0x000E4628
	public static void UpdateFacingDir(Transform transform, NavMeshAgent navAgent, Vector3 facingDir, float turnspeed = 3600f)
	{
		float num = ((navAgent == null) ? 0f : navAgent.speed);
		Quaternion quaternion = Quaternion.LookRotation(facingDir);
		transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, Mathf.Clamp(turnspeed * num / Quaternion.Angle(transform.rotation, quaternion) * Time.deltaTime, 0f, 1f));
	}

	// Token: 0x040037A4 RID: 14244
	public GameEntity entity;

	// Token: 0x040037A5 RID: 14245
	public NavMeshAgent navAgent;

	// Token: 0x040037A6 RID: 14246
	public Rigidbody rigidBody;

	// Token: 0x040037A7 RID: 14247
	public float networkPositionCorrectionDist = 2.5f;

	// Token: 0x040037A8 RID: 14248
	[ReadOnly]
	public NetPlayer targetPlayer;

	// Token: 0x040037A9 RID: 14249
	private bool disableNetworkSync;

	// Token: 0x040037AA RID: 14250
	private Vector3 lastPosOnNavMesh;

	// Token: 0x040037AB RID: 14251
	private Vector3 lastRequestedDest;

	// Token: 0x040037AC RID: 14252
	private Vector3 lastReceivedDest;

	// Token: 0x040037B2 RID: 14258
	private bool hasNotifiedNavigationFailure;

	// Token: 0x040037B3 RID: 14259
	private List<IGameAgentComponent> agentComponents;

	// Token: 0x040037B4 RID: 14260
	private bool wasOnOffMeshNavLink;

	// Token: 0x040037B5 RID: 14261
	public bool navAgentless;

	// Token: 0x040037B6 RID: 14262
	[ReadOnly]
	public bool pauseEntityThink;

	// Token: 0x020006B2 RID: 1714
	// (Invoke) Token: 0x06002ACE RID: 10958
	public delegate void StateChangedEvent(byte newState);

	// Token: 0x020006B3 RID: 1715
	// (Invoke) Token: 0x06002AD2 RID: 10962
	public delegate void NavigationLinkReachedEvent(OffMeshLinkData linkData);

	// Token: 0x020006B4 RID: 1716
	// (Invoke) Token: 0x06002AD6 RID: 10966
	public delegate void JumpRequestedEvent(Vector3 start, Vector3 end, float heightScale, float speedScale);

	// Token: 0x020006B5 RID: 1717
	// (Invoke) Token: 0x06002ADA RID: 10970
	public delegate void NavigationFailedEvent(NavMeshPathStatus status, Vector3 destination, float remainingDistance);
}
