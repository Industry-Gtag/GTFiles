using System;
using System.Collections.Generic;
using GorillaLocomotion;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000DD RID: 221
[RequireComponent(typeof(GameGrabbable))]
[RequireComponent(typeof(GameSnappable))]
[RequireComponent(typeof(GameButtonActivatable))]
[RequireComponent(typeof(SIGadgetBlasterType))]
public class SIGadgetBlaster : SIGadget, ITickSystemTick
{
	// Token: 0x17000060 RID: 96
	// (get) Token: 0x0600052D RID: 1325 RVA: 0x0001D42B File Offset: 0x0001B62B
	public bool LocalEquippedOrActivated
	{
		get
		{
			return this.IsEquippedLocal() || this.activatedLocally;
		}
	}

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001D43D File Offset: 0x0001B63D
	// (set) Token: 0x0600052F RID: 1327 RVA: 0x0001D445 File Offset: 0x0001B645
	public bool TickRunning { get; set; }

	// Token: 0x06000530 RID: 1328 RVA: 0x0001D450 File Offset: 0x0001B650
	protected override void OnEnable()
	{
		base.OnEnable();
		this.blasterType = base.GetComponent<SIGadgetBlasterType>();
		this.lastFired = 0f;
		this.environmentLayerMask = GTPlayer.Instance.locomotionEnabledLayers;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.StartGrabbing));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.StartGrabbing));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnReleased = (Action)Delegate.Combine(gameEntity3.OnReleased, new Action(this.StopGrabbing));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnUnsnapped = (Action)Delegate.Combine(gameEntity4.OnUnsnapped, new Action(this.StopGrabbing));
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000531 RID: 1329 RVA: 0x0001D52C File Offset: 0x0001B72C
	private new void OnDisable()
	{
		base.OnDisable();
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001D53C File Offset: 0x0001B73C
	public void Tick()
	{
		if (this.projectilesToDespawn.Count <= 0)
		{
			return;
		}
		if (Time.time < this.projectilesToDespawnTimes.Peek() + 1f)
		{
			return;
		}
		SIGadgetBlasterProjectile sigadgetBlasterProjectile = this.projectilesToDespawn.Dequeue();
		this.activeProjectiles.RemoveIfContains(sigadgetBlasterProjectile);
		if (sigadgetBlasterProjectile == null || sigadgetBlasterProjectile.gameObject == null)
		{
			return;
		}
		SIGadgetBlaster.blasterProjectilePools[sigadgetBlasterProjectile.poolId].Add(sigadgetBlasterProjectile.gameObject);
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001D5BC File Offset: 0x0001B7BC
	protected override void OnUpdateAuthority(float dt)
	{
		base.OnUpdateAuthority(dt);
		this.blasterType.OnUpdateAuthority(dt);
	}

	// Token: 0x06000534 RID: 1332 RVA: 0x0001D5D4 File Offset: 0x0001B7D4
	protected override void OnUpdateRemote(float dt)
	{
		base.OnUpdateRemote(dt);
		SIGadgetBlasterState sigadgetBlasterState = (SIGadgetBlasterState)this.gameEntity.GetState();
		if (sigadgetBlasterState != this.currentState)
		{
			this.SetStateShared(sigadgetBlasterState);
		}
		this.blasterType.OnUpdateRemote(dt);
	}

	// Token: 0x06000535 RID: 1333 RVA: 0x0001D611 File Offset: 0x0001B811
	public void SetStateAuthority(SIGadgetBlasterState newState)
	{
		this.SetStateShared(newState);
		this.gameEntity.RequestState(this.gameEntity.id, (long)newState);
	}

	// Token: 0x06000536 RID: 1334 RVA: 0x0001D632 File Offset: 0x0001B832
	private void SetStateShared(SIGadgetBlasterState newState)
	{
		if (newState == this.currentState || !SIGadgetBlaster.CanChangeState((long)newState))
		{
			return;
		}
		SIGadgetBlasterState sigadgetBlasterState = this.currentState;
		this.currentState = newState;
		this.blasterType.SetStateShared();
	}

	// Token: 0x06000537 RID: 1335 RVA: 0x0001D660 File Offset: 0x0001B860
	public override void ApplyUpgradeNodes(SIUpgradeSet withUpgrades)
	{
		this.blasterType.ApplyUpgradeNodes(withUpgrades);
	}

	// Token: 0x06000538 RID: 1336 RVA: 0x0001D66E File Offset: 0x0001B86E
	public static bool CanChangeState(long newStateIndex)
	{
		return newStateIndex >= 0L && newStateIndex < 4L;
	}

	// Token: 0x06000539 RID: 1337 RVA: 0x0001D67C File Offset: 0x0001B87C
	public bool CheckInput()
	{
		float num = (this.wasActivated ? this.inputActivateThreshold : this.inputDeactivateThreshold);
		this.wasActivated = this.buttonActivatable.CheckInput(num);
		return this.wasActivated;
	}

	// Token: 0x0600053A RID: 1338 RVA: 0x0001D6B8 File Offset: 0x0001B8B8
	public int NextFireId()
	{
		int num = this.projectileId;
		this.projectileId = num + 1;
		return num;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x0001D6D8 File Offset: 0x0001B8D8
	public override void ProcessClientToClientRPC(PhotonMessageInfo info, int rpcID, object[] data)
	{
		if (rpcID != 0)
		{
			if (rpcID != 1)
			{
				return;
			}
			if (data == null || data.Length < 2)
			{
				return;
			}
			int num;
			if (!GameEntityManager.ValidateDataType<int>(data[0], out num))
			{
				return;
			}
			SIGadgetBlasterProjectile sigadgetBlasterProjectile = null;
			for (int i = 0; i < this.activeProjectiles.Count; i++)
			{
				if (this.activeProjectiles[i].projectileId == num)
				{
					sigadgetBlasterProjectile = this.activeProjectiles[i];
					break;
				}
			}
			if (sigadgetBlasterProjectile == null)
			{
				return;
			}
			if (sigadgetBlasterProjectile.firedByPlayer != SIPlayer.Get(info.Sender.ActorNumber))
			{
				return;
			}
			sigadgetBlasterProjectile.GetComponent<SIGadgetProjectileType>().NetworkedProjectileHit(data);
			return;
		}
		else
		{
			if (data == null || data.Length == 0)
			{
				return;
			}
			if (!this.gameEntity.IsAttachedToPlayer(NetPlayer.Get(info.Sender)))
			{
				return;
			}
			this.blasterType.NetworkFireProjectile(data);
			return;
		}
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x0001D7A3 File Offset: 0x0001B9A3
	public void StartGrabbing()
	{
		if (this.IsEquippedLocal() || this.activatedLocally)
		{
			this.SetStateAuthority(SIGadgetBlasterState.Idle);
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0001D7BC File Offset: 0x0001B9BC
	public void StopGrabbing()
	{
		this.SetStateShared(SIGadgetBlasterState.Idle);
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x0001D7C5 File Offset: 0x0001B9C5
	public void DespawnProjectile(SIGadgetBlasterProjectile projectile)
	{
		projectile.gameObject.SetActive(false);
		if (!this.projectilesToDespawn.Contains(projectile))
		{
			this.projectilesToDespawn.Enqueue(projectile);
			this.projectilesToDespawnTimes.Enqueue(Time.time);
		}
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0001D800 File Offset: 0x0001BA00
	public GameObject InstantiateProjectile(SIGadgetBlasterProjectile projectilePrefab, Vector3 position, Quaternion rotation, int thisFireId)
	{
		if (SIGadgetBlaster.blasterProjectilePools == null)
		{
			SIGadgetBlaster.blasterProjectilePools = new Dictionary<int, List<GameObject>>();
		}
		int instanceID = projectilePrefab.GetInstanceID();
		if (!SIGadgetBlaster.blasterProjectilePools.ContainsKey(instanceID))
		{
			SIGadgetBlaster.blasterProjectilePools.Add(instanceID, new List<GameObject>());
		}
		List<GameObject> list = SIGadgetBlaster.blasterProjectilePools[instanceID];
		GameObject gameObject;
		if (list.Count <= 0)
		{
			gameObject = Object.Instantiate<GameObject>(projectilePrefab.gameObject, position, rotation);
		}
		else
		{
			gameObject = list[list.Count - 1];
			list.RemoveAt(list.Count - 1);
			gameObject.SetActive(true);
		}
		SIGadgetBlasterProjectile component = gameObject.GetComponent<SIGadgetBlasterProjectile>();
		component.transform.position = position;
		component.transform.rotation = rotation;
		component.parentBlaster = this;
		component.projectileId = thisFireId;
		component.firedByPlayer = (this.gameEntity.IsHeld() ? SIPlayer.Get(this.gameEntity.heldByActorNumber) : SIPlayer.Get(this.gameEntity.snappedByActorNumber));
		component.poolId = instanceID;
		this.activeProjectiles.Add(component);
		this.lastFired = Time.time;
		component.InitializeProjectile();
		return gameObject;
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x0001D913 File Offset: 0x0001BB13
	public void FireProjectileHaptics(float strength, float duration)
	{
		GorillaTagger.Instance.StartVibration(this.gameEntity.EquippedHandedness == EHandedness.Left, strength, duration);
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x0001D930 File Offset: 0x0001BB30
	public float CurrentFireRate()
	{
		int count = this.activeProjectiles.Count;
		if (count <= 1)
		{
			return 0f;
		}
		return (float)(count - 1) / (this.activeProjectiles[count - 1].timeSpawned - this.activeProjectiles[0].timeSpawned);
	}

	// Token: 0x040005F2 RID: 1522
	[OnEnterPlay_SetNull]
	public static Dictionary<int, List<GameObject>> blasterProjectilePools;

	// Token: 0x040005F3 RID: 1523
	[NonSerialized]
	public const float PROJECTILE_MAX_LATENCY = 1f;

	// Token: 0x040005F4 RID: 1524
	private SIGadgetBlasterType blasterType;

	// Token: 0x040005F5 RID: 1525
	[NonSerialized]
	public SIGadgetBlasterState currentState;

	// Token: 0x040005F7 RID: 1527
	[SerializeField]
	private GameButtonActivatable buttonActivatable;

	// Token: 0x040005F8 RID: 1528
	[SerializeField]
	private float inputActivateThreshold = 0.35f;

	// Token: 0x040005F9 RID: 1529
	[SerializeField]
	private float inputDeactivateThreshold = 0.25f;

	// Token: 0x040005FA RID: 1530
	public int maxProjectileCount = 10;

	// Token: 0x040005FB RID: 1531
	public float maxLagDistance = 5f;

	// Token: 0x040005FC RID: 1532
	private bool wasActivated;

	// Token: 0x040005FD RID: 1533
	[NonSerialized]
	public float lastFired;

	// Token: 0x040005FE RID: 1534
	[NonSerialized]
	public int projectileCount;

	// Token: 0x040005FF RID: 1535
	private int projectileId;

	// Token: 0x04000600 RID: 1536
	[NonSerialized]
	public List<SIGadgetBlasterProjectile> activeProjectiles = new List<SIGadgetBlasterProjectile>();

	// Token: 0x04000601 RID: 1537
	[NonSerialized]
	public Queue<SIGadgetBlasterProjectile> projectilesToDespawn = new Queue<SIGadgetBlasterProjectile>();

	// Token: 0x04000602 RID: 1538
	[NonSerialized]
	public Queue<float> projectilesToDespawnTimes = new Queue<float>();

	// Token: 0x04000603 RID: 1539
	public Transform firingPosition;

	// Token: 0x04000604 RID: 1540
	public AudioSource firingSource;

	// Token: 0x04000605 RID: 1541
	public AudioSource blasterSource;

	// Token: 0x04000606 RID: 1542
	[NonSerialized]
	public LayerMask environmentLayerMask;

	// Token: 0x020000DE RID: 222
	public enum RPCCalls
	{
		// Token: 0x04000608 RID: 1544
		FireProjectile,
		// Token: 0x04000609 RID: 1545
		ProjectileHitPlayer
	}
}
