using System;
using GorillaExtensions;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x0200069D RID: 1693
public class Flocking : MonoBehaviour
{
	// Token: 0x1700043E RID: 1086
	// (get) Token: 0x06002A39 RID: 10809 RVA: 0x000E3DA1 File Offset: 0x000E1FA1
	// (set) Token: 0x06002A3A RID: 10810 RVA: 0x000E3DA9 File Offset: 0x000E1FA9
	public FlockingManager.FishArea FishArea { get; set; }

	// Token: 0x06002A3B RID: 10811 RVA: 0x000E3DB2 File Offset: 0x000E1FB2
	private void Awake()
	{
		this.manager = base.GetComponentInParent<FlockingManager>();
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000E3DC0 File Offset: 0x000E1FC0
	private void Start()
	{
		this.speed = Random.Range(this.minSpeed, this.maxSpeed);
		this.fishState = Flocking.FishState.patrol;
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000E3DE0 File Offset: 0x000E1FE0
	private void OnDisable()
	{
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Remove(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Remove(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.UnregisterFlocking(this);
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000E3E44 File Offset: 0x000E2044
	public void InvokeUpdate()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		this.AvoidPlayerHands();
		this.MaybeTurn();
		switch (this.fishState)
		{
		case Flocking.FishState.flock:
			this.Flock(this.FishArea.nextWaypoint);
			this.SwitchState(Flocking.FishState.patrol);
			break;
		case Flocking.FishState.patrol:
			if (Random.Range(0, 10) < 2)
			{
				this.SwitchState(Flocking.FishState.flock);
			}
			break;
		case Flocking.FishState.followFood:
			if (this.isTurning)
			{
				return;
			}
			if (this.isRealFood)
			{
				if ((double)Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFoodStopDistance)
				{
					this.FollowFood();
				}
				else
				{
					this.followingFood = false;
					this.Flock(this.projectileGameObject.transform.position);
					this.feedingTimeStarted += Time.deltaTime;
					if (this.feedingTimeStarted > this.eatFoodDuration)
					{
						this.SwitchState(Flocking.FishState.patrol);
					}
				}
			}
			else if (Vector3.Distance(base.transform.position, this.projectileGameObject.transform.position) > this.FollowFakeFoodStopDistance)
			{
				this.FollowFood();
			}
			else
			{
				this.followingFood = false;
				this.SwitchState(Flocking.FishState.patrol);
			}
			break;
		}
		if (!this.followingFood)
		{
			base.transform.Translate(0f, 0f, this.speed * Time.deltaTime);
		}
		this.pos = base.transform.position;
		this.rot = base.transform.rotation;
	}

	// Token: 0x06002A3F RID: 10815 RVA: 0x000E3FE0 File Offset: 0x000E21E0
	private void MaybeTurn()
	{
		if (!this.manager.IsInside(base.transform.position, this.FishArea))
		{
			this.Turn(this.FishArea.colliderCenter);
			if (Vector3.Angle(this.FishArea.colliderCenter - base.transform.position, Vector3.forward) > 5f)
			{
				this.isTurning = true;
				return;
			}
		}
		else
		{
			this.isTurning = false;
		}
	}

	// Token: 0x06002A40 RID: 10816 RVA: 0x000E4058 File Offset: 0x000E2258
	private void Turn(Vector3 towardPoint)
	{
		this.isTurning = true;
		Quaternion quaternion = Quaternion.LookRotation(towardPoint - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
	}

	// Token: 0x06002A41 RID: 10817 RVA: 0x000E40AB File Offset: 0x000E22AB
	private void SwitchState(Flocking.FishState state)
	{
		this.fishState = state;
	}

	// Token: 0x06002A42 RID: 10818 RVA: 0x000E40B4 File Offset: 0x000E22B4
	private void Flock(Vector3 nextGoal)
	{
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		float num = 1f;
		int num2 = 0;
		foreach (Flocking flocking in this.FishArea.fishList)
		{
			if (flocking.gameObject != base.gameObject)
			{
				float num3 = Vector3.Distance(flocking.transform.position, base.transform.position);
				if (num3 <= this.maxNeighbourDistance)
				{
					vector += flocking.transform.position;
					num2++;
					if (num3 < this.flockingAvoidanceDistance)
					{
						vector2 += base.transform.position - flocking.transform.position;
					}
					num += flocking.speed;
				}
			}
		}
		if (num2 > 0)
		{
			this.fishState = Flocking.FishState.flock;
			vector = vector / (float)num2 + (nextGoal - base.transform.position);
			this.speed = num / (float)num2;
			this.speed = Mathf.Clamp(this.speed, this.minSpeed, this.maxSpeed);
			Vector3 vector3 = vector + vector2 - base.transform.position;
			if (vector3 != Vector3.zero)
			{
				Quaternion quaternion = Quaternion.LookRotation(vector3);
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
			}
		}
	}

	// Token: 0x06002A43 RID: 10819 RVA: 0x000E4258 File Offset: 0x000E2458
	private void HandleOnFoodDetected(FlockingManager.FishFood fishFood)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (fishFood.collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.followFood);
		this.feedingTimeStarted = 0f;
		this.projectileGameObject = fishFood.slingshotProjectile.gameObject;
		this.isRealFood = fishFood.isRealFood;
	}

	// Token: 0x06002A44 RID: 10820 RVA: 0x000E42C8 File Offset: 0x000E24C8
	private void HandleOnFoodDestroyed(BoxCollider collider)
	{
		bool flag = false;
		foreach (BoxCollider boxCollider in this.FishArea.colliders)
		{
			if (collider == boxCollider)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		this.SwitchState(Flocking.FishState.patrol);
		this.projectileGameObject = null;
		this.followingFood = false;
	}

	// Token: 0x06002A45 RID: 10821 RVA: 0x000E431C File Offset: 0x000E251C
	private void FollowFood()
	{
		this.followingFood = true;
		Quaternion quaternion = Quaternion.LookRotation(this.projectileGameObject.transform.position - base.transform.position);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, this.rotationSpeed * Time.deltaTime);
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.projectileGameObject.transform.position, this.speed * this.followFoodSpeedMult * Time.deltaTime);
	}

	// Token: 0x06002A46 RID: 10822 RVA: 0x000E43BC File Offset: 0x000E25BC
	private void AvoidPlayerHands()
	{
		foreach (GameObject gameObject in FlockingManager.avoidPoints)
		{
			Vector3 position = gameObject.transform.position;
			if ((base.transform.position - position).IsShorterThan(this.avointPointRadius))
			{
				Vector3 randomPointInsideCollider = this.manager.GetRandomPointInsideCollider(this.FishArea);
				this.Turn(randomPointInsideCollider);
				this.speed = this.avoidHandSpeed;
			}
		}
	}

	// Token: 0x06002A47 RID: 10823 RVA: 0x000E4454 File Offset: 0x000E2654
	internal void SetSyncPosRot(Vector3 syncPos, Quaternion syncRot)
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		if (this.FishArea == null)
		{
			Debug.LogError("FISH AREA NULL");
		}
		if ((in syncRot).IsValid())
		{
			this.rot = syncRot;
		}
		float num = 10000f;
		if ((in syncPos).IsValid(in num))
		{
			this.pos = this.manager.RestrictPointToArea(syncPos, this.FishArea);
		}
	}

	// Token: 0x06002A48 RID: 10824 RVA: 0x000E44C8 File Offset: 0x000E26C8
	private void OnEnable()
	{
		if (this.manager == null)
		{
			this.manager = base.GetComponentInParent<FlockingManager>();
		}
		FlockingManager flockingManager = this.manager;
		flockingManager.onFoodDetected = (UnityAction<FlockingManager.FishFood>)Delegate.Combine(flockingManager.onFoodDetected, new UnityAction<FlockingManager.FishFood>(this.HandleOnFoodDetected));
		FlockingManager flockingManager2 = this.manager;
		flockingManager2.onFoodDestroyed = (UnityAction<BoxCollider>)Delegate.Combine(flockingManager2.onFoodDestroyed, new UnityAction<BoxCollider>(this.HandleOnFoodDestroyed));
		FlockingUpdateManager.RegisterFlocking(this);
	}

	// Token: 0x04003701 RID: 14081
	[Tooltip("Speed is randomly generated from min and max speed")]
	public float minSpeed = 2f;

	// Token: 0x04003702 RID: 14082
	public float maxSpeed = 4f;

	// Token: 0x04003703 RID: 14083
	public float rotationSpeed = 360f;

	// Token: 0x04003704 RID: 14084
	[Tooltip("Maximum distance to the neighbours to form a flocking group")]
	public float maxNeighbourDistance = 4f;

	// Token: 0x04003705 RID: 14085
	public float eatFoodDuration = 10f;

	// Token: 0x04003706 RID: 14086
	[Tooltip("How fast should it follow the food? This value multiplies by the current speed")]
	public float followFoodSpeedMult = 3f;

	// Token: 0x04003707 RID: 14087
	[Tooltip("How fast should it run away from players hand?")]
	public float avoidHandSpeed = 1.2f;

	// Token: 0x04003708 RID: 14088
	[FormerlySerializedAs("avoidanceDistance")]
	[Tooltip("When flocking they will avoid each other if the distance between them is less than this value")]
	public float flockingAvoidanceDistance = 2f;

	// Token: 0x04003709 RID: 14089
	[Tooltip("Follow the fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFood")]
	public double FollowFoodStopDistance = 0.20000000298023224;

	// Token: 0x0400370A RID: 14090
	[Tooltip("Follow any fake fish food until they are this far from it")]
	[FormerlySerializedAs("distanceToFollowFakeFood")]
	public float FollowFakeFoodStopDistance = 2f;

	// Token: 0x0400370B RID: 14091
	private float speed;

	// Token: 0x0400370C RID: 14092
	private Vector3 averageHeading;

	// Token: 0x0400370D RID: 14093
	private Vector3 averagePosition;

	// Token: 0x0400370E RID: 14094
	private float feedingTimeStarted;

	// Token: 0x0400370F RID: 14095
	private GameObject projectileGameObject;

	// Token: 0x04003710 RID: 14096
	private bool followingFood;

	// Token: 0x04003711 RID: 14097
	private FlockingManager manager;

	// Token: 0x04003712 RID: 14098
	private GameObjectManagerWithId _fishSceneGameObjectsManager;

	// Token: 0x04003713 RID: 14099
	private UnityEvent<string, Transform> sendIdEvent;

	// Token: 0x04003714 RID: 14100
	private Flocking.FishState fishState;

	// Token: 0x04003715 RID: 14101
	[HideInInspector]
	public Vector3 pos;

	// Token: 0x04003716 RID: 14102
	[HideInInspector]
	public Quaternion rot;

	// Token: 0x04003717 RID: 14103
	private float velocity;

	// Token: 0x04003718 RID: 14104
	private bool isTurning;

	// Token: 0x04003719 RID: 14105
	private bool isRealFood;

	// Token: 0x0400371A RID: 14106
	public float avointPointRadius = 0.5f;

	// Token: 0x0400371B RID: 14107
	private float cacheSpeed;

	// Token: 0x0200069E RID: 1694
	public enum FishState
	{
		// Token: 0x0400371E RID: 14110
		flock,
		// Token: 0x0400371F RID: 14111
		patrol,
		// Token: 0x04003720 RID: 14112
		followFood
	}
}
