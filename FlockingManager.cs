using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006A0 RID: 1696
[NetworkBehaviourWeaved(337)]
public class FlockingManager : NetworkComponent
{
	// Token: 0x06002A52 RID: 10834 RVA: 0x000E46C8 File Offset: 0x000E28C8
	protected override void Awake()
	{
		base.Awake();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			Flocking[] componentsInChildren = gameObject.GetComponentsInChildren<Flocking>(false);
			FlockingManager.FishArea fishArea = new FlockingManager.FishArea();
			fishArea.id = gameObject.name;
			fishArea.colliders = gameObject.GetComponentsInChildren<BoxCollider>();
			fishArea.colliderCenter = fishArea.colliders[0].bounds.center;
			fishArea.fishList.AddRange(componentsInChildren);
			fishArea.zoneBasedObject = gameObject.GetComponent<ZoneBasedObject>();
			this.areaToWaypointDict[fishArea.id] = Vector3.zero;
			Flocking[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FishArea = fishArea;
			}
			this.fishAreaList.Add(fishArea);
			this.allFish.AddRange(fishArea.fishList);
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				component.OnProjectileTriggerExit += this.ProjectileHitExit;
			}
			else
			{
				Debug.LogError("Needs SlingshotProjectileHitNotifier added to each fish area");
			}
		}
	}

	// Token: 0x06002A53 RID: 10835 RVA: 0x0003CE1F File Offset: 0x0003B01F
	private new void Start()
	{
		NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x000E4820 File Offset: 0x000E2A20
	private void OnDestroy()
	{
		NetworkBehaviourUtils.InternalOnDestroy(this);
		this.fishAreaList.Clear();
		this.areaToWaypointDict.Clear();
		this.allFish.Clear();
		foreach (GameObject gameObject in this.fishAreaContainer)
		{
			SlingshotProjectileHitNotifier component = gameObject.GetComponent<SlingshotProjectileHitNotifier>();
			if (component != null)
			{
				component.OnProjectileTriggerExit -= this.ProjectileHitExit;
				component.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
			}
		}
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x000E48C8 File Offset: 0x000E2AC8
	private void Update()
	{
		if (Random.Range(0, 10000) < 50)
		{
			foreach (FlockingManager.FishArea fishArea in this.fishAreaList)
			{
				if (fishArea.zoneBasedObject != null)
				{
					fishArea.zoneBasedObject.gameObject.SetActive(fishArea.zoneBasedObject.IsLocalPlayerInZone());
				}
				fishArea.nextWaypoint = this.GetRandomPointInsideCollider(fishArea);
				this.areaToWaypointDict[fishArea.id] = fishArea.nextWaypoint;
				Debug.DrawLine(fishArea.nextWaypoint, Vector3.forward * 5f, Color.magenta);
			}
		}
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x000E4994 File Offset: 0x000E2B94
	public Vector3 GetRandomPointInsideCollider(FlockingManager.FishArea fishArea)
	{
		int num = Random.Range(0, fishArea.colliders.Length);
		BoxCollider boxCollider = fishArea.colliders[num];
		Vector3 vector = boxCollider.size / 2f;
		Vector3 vector2 = new Vector3(Random.Range(-vector.x, vector.x), Random.Range(-vector.y, vector.y), Random.Range(-vector.z, vector.z));
		return boxCollider.transform.TransformPoint(vector2);
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x000E4A14 File Offset: 0x000E2C14
	public bool IsInside(Vector3 point, FlockingManager.FishArea fish)
	{
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector = boxCollider.transform.InverseTransformPoint(point);
			vector -= center;
			Vector3 size = boxCollider.size;
			if (Mathf.Abs(vector.x) < size.x / 2f && Mathf.Abs(vector.y) < size.y / 2f && Mathf.Abs(vector.z) < size.z / 2f)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x000E4AB0 File Offset: 0x000E2CB0
	public Vector3 RestrictPointToArea(Vector3 point, FlockingManager.FishArea fish)
	{
		Vector3 vector = default(Vector3);
		float num = float.MaxValue;
		foreach (BoxCollider boxCollider in fish.colliders)
		{
			Vector3 center = boxCollider.center;
			Vector3 vector2 = boxCollider.transform.InverseTransformPoint(point);
			Vector3 vector3 = vector2 - center;
			Vector3 size = boxCollider.size;
			float num2 = size.x / 2f;
			float num3 = size.y / 2f;
			float num4 = size.z / 2f;
			if (Mathf.Abs(vector3.x) < num2 && Mathf.Abs(vector3.y) < num3 && Mathf.Abs(vector3.z) < num4)
			{
				return point;
			}
			Vector3 vector4 = new Vector3(center.x - num2, center.y - num3, center.z - num4);
			Vector3 vector5 = new Vector3(center.x + num2, center.y + num3, center.z + num4);
			Vector3 vector6 = new Vector3(Mathf.Clamp(vector2.x, vector4.x, vector5.x), Mathf.Clamp(vector2.y, vector4.y, vector5.y), Mathf.Clamp(vector2.z, vector4.z, vector5.z));
			float num5 = Vector3.Distance(vector2, vector6);
			if (num5 < num)
			{
				num = num5;
				if (num5 > 1f)
				{
					Vector3 vector7 = Vector3.Normalize(vector2 - vector6);
					vector = boxCollider.transform.TransformPoint(vector6 + vector7 * 1f);
				}
				else
				{
					vector = point;
				}
			}
		}
		return vector;
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x000E4C60 File Offset: 0x000E2E60
	private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider1)
	{
		bool flag = projectile.CompareTag(this.foodProjectileTag);
		FlockingManager.FishFood fishFood = new FlockingManager.FishFood
		{
			collider = (collider1 as BoxCollider),
			isRealFood = flag,
			slingshotProjectile = projectile
		};
		UnityAction<FlockingManager.FishFood> unityAction = this.onFoodDetected;
		if (unityAction == null)
		{
			return;
		}
		unityAction(fishFood);
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x000E4CAB File Offset: 0x000E2EAB
	private void ProjectileHitExit(SlingshotProjectile projectile, Collider collider2)
	{
		UnityAction<BoxCollider> unityAction = this.onFoodDestroyed;
		if (unityAction == null)
		{
			return;
		}
		unityAction(collider2 as BoxCollider);
	}

	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x06002A5B RID: 10843 RVA: 0x000E4CC3 File Offset: 0x000E2EC3
	// (set) Token: 0x06002A5C RID: 10844 RVA: 0x000E4CED File Offset: 0x000E2EED
	[Networked]
	[NetworkedWeaved(0, 337)]
	public unsafe FlockingData Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(FlockingData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing FlockingManager.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(FlockingData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x06002A5D RID: 10845 RVA: 0x000E4D18 File Offset: 0x000E2F18
	public override void WriteDataFusion()
	{
		this.Data = new FlockingData(this.allFish);
	}

	// Token: 0x06002A5E RID: 10846 RVA: 0x000E4D2C File Offset: 0x000E2F2C
	public override void ReadDataFusion()
	{
		for (int i = 0; i < this.Data.count; i++)
		{
			Vector3 vector = this.Data.Positions[i];
			Quaternion quaternion = this.Data.Rotations[i];
			this.allFish[i].SetSyncPosRot(vector, quaternion);
		}
	}

	// Token: 0x06002A5F RID: 10847 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002A60 RID: 10848 RVA: 0x00002C2D File Offset: 0x00000E2D
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06002A61 RID: 10849 RVA: 0x000E4D97 File Offset: 0x000E2F97
	public static void RegisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Add(obj);
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000E4DA4 File Offset: 0x000E2FA4
	public static void UnregisterAvoidPoint(GameObject obj)
	{
		FlockingManager.avoidPoints.Remove(obj);
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000E4DF2 File Offset: 0x000E2FF2
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000E4E0A File Offset: 0x000E300A
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04003724 RID: 14116
	public List<GameObject> fishAreaContainer;

	// Token: 0x04003725 RID: 14117
	public string foodProjectileTag = "WaterBalloonProjectile";

	// Token: 0x04003726 RID: 14118
	private Dictionary<string, Vector3> areaToWaypointDict = new Dictionary<string, Vector3>();

	// Token: 0x04003727 RID: 14119
	private List<FlockingManager.FishArea> fishAreaList = new List<FlockingManager.FishArea>();

	// Token: 0x04003728 RID: 14120
	private List<Flocking> allFish = new List<Flocking>();

	// Token: 0x04003729 RID: 14121
	public UnityAction<FlockingManager.FishFood> onFoodDetected;

	// Token: 0x0400372A RID: 14122
	public UnityAction<BoxCollider> onFoodDestroyed;

	// Token: 0x0400372B RID: 14123
	private bool hasBeenSerialized;

	// Token: 0x0400372C RID: 14124
	public static readonly List<GameObject> avoidPoints = new List<GameObject>();

	// Token: 0x0400372D RID: 14125
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Data", 0, 337)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private FlockingData _Data;

	// Token: 0x020006A1 RID: 1697
	public class FishArea
	{
		// Token: 0x0400372E RID: 14126
		public string id;

		// Token: 0x0400372F RID: 14127
		public List<Flocking> fishList = new List<Flocking>();

		// Token: 0x04003730 RID: 14128
		public Vector3 colliderCenter;

		// Token: 0x04003731 RID: 14129
		public BoxCollider[] colliders;

		// Token: 0x04003732 RID: 14130
		public Vector3 nextWaypoint = Vector3.zero;

		// Token: 0x04003733 RID: 14131
		public ZoneBasedObject zoneBasedObject;
	}

	// Token: 0x020006A2 RID: 1698
	public class FishFood
	{
		// Token: 0x04003734 RID: 14132
		public BoxCollider collider;

		// Token: 0x04003735 RID: 14133
		public bool isRealFood;

		// Token: 0x04003736 RID: 14134
		public SlingshotProjectile slingshotProjectile;
	}
}
